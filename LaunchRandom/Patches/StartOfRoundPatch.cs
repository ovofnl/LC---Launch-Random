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
            RDManager.Instance.GetMoonsInfoExcept();
            RDManager.Instance.GetMoonsWeightExcept();
        }
    }
}
