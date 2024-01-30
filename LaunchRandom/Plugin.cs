using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LaunchRandom.Configs;
using LaunchRandom.Managers;
using LaunchRandom.Patches;
using System;
using UnityEngine;

namespace LaunchRandom
{
    [BepInPlugin(modGUID,modName ,modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "ovofnl.LaunchRandom";
        private const string modName = "Launch Random";
        private const string modVersion = "1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static Plugin Instance;

        internal static ManualLogSource mls;

        public static PluginConfig cfg;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            cfg = new PluginConfig(Config);
            cfg.InitBindings();

            GameObject RD = new GameObject("RDManager");
            RD.AddComponent<RDManager>();

            mls.LogInfo("Message on mod load");

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(StartMatchLeverPatch));
            
        }
    }
}