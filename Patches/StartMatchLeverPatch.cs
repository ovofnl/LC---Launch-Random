using BepInEx.Logging;
using HarmonyLib;
using LaunchRandom.Configs;
using LaunchRandom.Managers;
using System;
using UnityEngine;
using Random = System.Random;

namespace LaunchRandom.Patches
{
	delegate bool judge();
    [HarmonyPatch(typeof(StartMatchLever))]
    public class StartMatchLeverPatch
    {
		public PluginConfig cfg;

		public ManualLogSource mls;

		[HarmonyPatch("BeginHoldingInteractOnLever")]
		[HarmonyPrefix]
		private static void ChangeHoldingLeverTime(ref InteractTrigger ___triggerScript)
		{
			___triggerScript.timeToHold = 0.5f;
		}

		[HarmonyPatch("PullLever")]
		[HarmonyPrefix]
		private static void Launch_RandomCore(ref bool ___leverHasBeenPulled)
		{
			int num = 0;
			StartMatchLeverPatch instance = new StartMatchLeverPatch();
			RDManager.Instance.preLevel = RDManager.Instance.startOfRoundInstance.currentLevel.levelID;
			//RDManager.mls.LogInfo("- preLevel:" + RDManager.Instance.preLevel);
			if (TimeOfDay.Instance.daysUntilDeadline <= 0)
			{
				RDManager.Instance.startOfRoundInstance.ChangeLevel(RDManager.Instance.companyBuildingLevelID);
			}
			else if (___leverHasBeenPulled && RDManager.Instance.randomEnableAnother)
			{
				RDManager.Instance.BalanceOfMoonsExcept();
				num = RDManager.Instance.StartToRandomLevelAnother();
				if (!RDManager.Instance.repeatLaunch)
				{
					while (instance.ThreeDaysJudge(RDManager.Instance.preLevel, num))
					{
						num = RDManager.Instance.StartToRandomLevelAnother();
					}
				}
				//RDManager.Instance.InputTestInfo();
				RDManager.Instance.startOfRoundInstance.ChangeLevel(num);
			}
			else
			{
				if (!___leverHasBeenPulled || !RDManager.Instance.randomEnable)
				{
					return;
				}
				RDManager.Instance.BalanceOfMoons();
				num = RDManager.Instance.StartToRandomLevel();
				if (!RDManager.Instance.repeatLaunch)
				{
					while (num == RDManager.Instance.preLevel)
					{
						num = RDManager.Instance.StartToRandomLevel();
					}
				}
				RDManager.Instance.startOfRoundInstance.ChangeLevel(num);
			}
		}

		//当当前天数<=4, 随机到paid moon时将会继续随机生成。
		public bool ThreeDaysJudge(int preLevel, int currentLevel)
        {
			bool judge = false;
			if(RDManager.Instance.startOfRoundInstance.gameStats.daysSpent <= 4)
            {
				if (RDManager.Instance.balanceRiskLevelEnable)
				{
					if (RDManager.Instance.moonsBalance[currentLevel] == 0)
					{
						judge = true;
					}
				}
				else
				{
					if (RDManager.Instance.moonsBalance[currentLevel] == 1)
					{
						judge = true;
					}
				}
			}
			//RDManager.mls.LogInfo("judge:" + judge);
			return (preLevel == currentLevel) || judge;
        }
	}
}
