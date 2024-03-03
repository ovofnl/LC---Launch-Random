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
		public static StartMatchLeverPatch Instance;

		public PluginConfig cfg;

		public ManualLogSource mls;

		[HarmonyPatch("BeginHoldingInteractOnLever")]
		[HarmonyPrefix]
		private static void ChangeHoldingLeverTime(ref InteractTrigger ___triggerScript)
		{
			___triggerScript.timeToHold = 0.5f;

			if (RDManager.Instance.startOfRoundInstance.levels.Length - RDManager.Instance.cfg.LEVEL_NAME_ANOTHER.Length <= 2)
			{
				RDManager.Instance.repeatLaunch = true;
			}

			if (RDManager.Instance.leverHasBeenPulled == true)
			{
				RDManager.Instance.leverHasBeenPulled = false;
				
				int num;
				RDManager.Instance.preLevel = RDManager.Instance.startOfRoundInstance.currentLevel.levelID;
				//RDManager.mls.LogInfo("preLevel:" + RDManager.Instance.preLevel + " tempLevel: " + RDManager.Instance.tempLevel);
				//RDManager.mls.LogInfo("- preLevel:" + RDManager.Instance.preLevel);
				if (TimeOfDay.Instance.daysUntilDeadline <= 0)
				{
					RDManager.Instance.startOfRoundInstance.ChangeLevelServerRpc(RDManager.Instance.companyBuildingLevelID, UnityEngine.Object.FindObjectOfType<Terminal>().groupCredits);
				}
				else if (!RDManager.Instance.leverHasBeenPulled && RDManager.Instance.randomEnableAnother)
				{
					RDManager.Instance.BalanceOfMoonsExcept();
					RDManager.Instance.InputBalanceOfMoonsWeight();
					num = RDManager.Instance.StartToRandomLevelAnother();
					if (!RDManager.Instance.repeatLaunch)
					{
						while (Instance.ThreeDaysJudge(RDManager.Instance.preLevel, num))
						{
							num = RDManager.Instance.StartToRandomLevelAnother();
						}
					}
					RDManager.Instance.startOfRoundInstance.ChangeLevelServerRpc(num, UnityEngine.Object.FindObjectOfType<Terminal>().groupCredits);
				}
			}
		}

		[HarmonyPatch("PullLever")]
		[HarmonyPrefix]
		private static void Launch_RandomCore(ref bool ___leverHasBeenPulled)
        {
            if (___leverHasBeenPulled)
            {
                RDManager.Instance.leverHasBeenPulled = false;
            }
            else
            {
                RDManager.Instance.leverHasBeenPulled = true;
            }
        }

		//当当前天数<4, 随机到A/S难度星球时将会继续随机生成。
		public bool ThreeDaysJudge(int preLevel, int currentLevel)
        {
			bool judge = false;
			if(RDManager.Instance.startOfRoundInstance.gameStats.daysSpent < 4)
            {
				if (RDManager.Instance.moonsBalance[currentLevel] == 0)
				{
					judge = true;
				}
			}
			//RDManager.mls.LogInfo("judge:" + judge);
			return (preLevel == currentLevel) || judge;
        }
	}
}
