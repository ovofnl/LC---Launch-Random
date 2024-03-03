using BepInEx.Logging;
using HarmonyLib;
using LaunchRandom.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchRandom.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    class StartOfRoundPatch
    {

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void GetInstance(StartOfRound __instance)
        {
            RDManager.Instance.startOfRoundInstance = __instance;
            StartMatchLeverPatch.Instance = new StartMatchLeverPatch();
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void GetMoonsInfoPost()
        {
            RDManager.Instance.InputMoonsInfo();
            if(RDManager.Instance.cfg.LEVEL_NAME_ANOTHER.Length == 0)
            {
                RDManager.Instance.GetMoonsInfoExceptNull();
            }
            else
            {
                RDManager.Instance.GetMoonsInfoExcept();
            }
            
            RDManager.Instance.GetMoonsWeightExcept();
			RDManager.Instance.leverHasBeenPulled = true;
		}
    }
}
