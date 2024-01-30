using BepInEx.Logging;
using HarmonyLib;
using LaunchRandom.Configs;
using LaunchRandom.Managers;
using System;
using UnityEngine;
using Random = System.Random;

namespace LaunchRandom.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    
    public class StartMatchLeverPatch
    {
        public PluginConfig cfg;

        [HarmonyPatch("BeginHoldingInteractOnLever")]
        [HarmonyPrefix]
        static void ChangeHoldingLeverTime(ref InteractTrigger ___triggerScript)
        {
                ___triggerScript.timeToHold = 0.5f;
        }

        [HarmonyPatch("PullLever")]
        [HarmonyPrefix]
        static void Launch_RandomCore(ref bool ___leverHasBeenPulled)
        {
/*            for(int i = 0; i < RDManager.Instance.moonsWeightExcept.Length; i++)
            {
                Debug.Log("moonsWeight:" + RDManager.Instance.moonsWeightExcept[i] +
                    "moons:" + RDManager.Instance.moonsLevelIDExcept[i]);
            }*/

            //sceneName: Level + levelID + planetName(except route)
            if (TimeOfDay.Instance.daysUntilDeadline <= 0)
            {
                RDManager.Instance.startOfRoundInstance.ChangeLevel(RDManager.Instance.companyBuildingLevelID);
            }
            else
            {
                if (___leverHasBeenPulled && RDManager.Instance.cfg.RANDOM_ENABLE_ANOTHER)
                {
                    RDManager.Instance.BalanceOfMoonsExcept();
                    RDManager.Instance.startOfRoundInstance.ChangeLevel(RDManager.Instance.StartToRandomLevelAnother());
                    return;
                }
                if (___leverHasBeenPulled && RDManager.Instance.cfg.RANDOM_ENABLE)
                {
                    if (RDManager.Instance.startOfRoundInstance.gameStats.daysSpent != 0 
                        &&
                        (RDManager.Instance.startOfRoundInstance.gameStats.daysSpent % RDManager.Instance.levelDay == 0))
                    {
                        RDManager.Instance.BalanceOfMoons();
                    }
                    RDManager.Instance.startOfRoundInstance.ChangeLevel(RDManager.Instance.StartToRandomLevel());
                    return;
                }
            }
        }
    }
}
