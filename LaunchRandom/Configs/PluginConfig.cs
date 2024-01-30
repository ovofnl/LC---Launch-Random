using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchRandom.Configs
{
    public class PluginConfig
    {
        readonly ConfigFile configFile;
        public bool RANDOM_ENABLE { get; private set; }
        public bool RANDOM_ENABLE_ANOTHER { get; private set; }
        public bool LEVEL_BALANCE_ENABLE { get; private set; }
        public string LEVEL_NAME { get; private set; }
        public string LEVEL_NAME_ANOTHER { get; private set; }
        public string LEVEL_WEIGHT { get; private set; }
        public float LEVEL_COEFFICIENT { get; private set; }
        public int LEVEL_DAYS_AFTER { get; private set; }
        public string LEVEL_AMOUNT { get; private set; }
        public string LEVEL_ID { get; private set; }
        public string LEVEL_LV { get; private set; }
        public float LEVEL_WEIGHT_ANOTHER_EASY { get; private set; }
        public float LEVEL_WEIGHT_ANOTHER_DANGER { get; private set; }

        public PluginConfig(ConfigFile config)
        {
            configFile = config;
        }

        private T ConfigEntry<T>(string section, string key, T defaultValue, string description)
        {
            return configFile.Bind(section, key, defaultValue, description).Value;
        }

        public void InitBindings()
        {
            string topSection = "Core";
            RANDOM_ENABLE = ConfigEntry(
                topSection,
                "Allow Random Launch",
                false,
                "If enable, when the lever has been pulled, it will launch to moon randomly," +
                "for the last day it will go to company building.");
            RANDOM_ENABLE_ANOTHER = ConfigEntry(
                topSection,
                "Allow Random Launch(Except)",
                true,
                "If enable, when the lever has been pulled, it will launch to moon randomly," +
                "for the last day it will go to company building.");
            LEVEL_BALANCE_ENABLE = ConfigEntry(
                topSection,
                "Allow Level Balance",
                true,
                "" +
                "Enable the balance setting.");
            topSection = "Moons Settings";
            LEVEL_NAME = ConfigEntry(
                topSection,
                "Moons List",
                "Assurance,Vow,March,Rend,Dine,Offense,Titan",
                "These moons will be added in the random list." +
                "If need to add new moon, use ',' split moons" +
                "Be sure don't have blank between moons."
                );
            LEVEL_WEIGHT = ConfigEntry(
                topSection,
                "Moons Weight",
                "5,5,5,1,1,5,1",
                "Set the moons probability, high value means high probability of random"
                );
            LEVEL_AMOUNT = ConfigEntry(
                topSection,
                "Moons Amount",
                "0,0,0,550,600,0,700",
                "If Amount > 0, its Weight will increase after 3 days by the Balance Coefficient."
                );
            LEVEL_ID = ConfigEntry(
                topSection,
                "Moons LevelID",
                "1,2,4,5,6,7,8",
                ""
                );
            topSection = "Moons Settings Another";
            LEVEL_NAME_ANOTHER = ConfigEntry(
                topSection,
                "Moons List(Except)",
                "Experimentation,Assurance",
                "These moons will be removed in the random list."
                );
            LEVEL_LV = ConfigEntry(
                topSection,
                "Moons Level",
                "S,A",
                "Be like Titan's Level is S, " +
                "so after N(Balance Days) days, " +
                "the probability of titan will increase N(Balance Coefficient)."
                );
            LEVEL_WEIGHT_ANOTHER_EASY = ConfigEntry(
                topSection,
                "Moons Weight Another Easy",
                5f,
                "The Weight data is for those riskLevel less than the Moons Level data."
                );
            LEVEL_WEIGHT_ANOTHER_DANGER = ConfigEntry(
                topSection,
                "Moons Weight Another Danger",
                1f,
                "The Weight data is for those riskLevel more than the Moons Level data."
                );


            topSection = "Balance Settings";
            LEVEL_COEFFICIENT = ConfigEntry(
                topSection,
                "Balance Coefficient",
                1f,
                "The higher the value, the higher the probability of those paid moons and" +
                "the lower the probability of those free moons"
                );
            LEVEL_DAYS_AFTER = ConfigEntry(
                topSection,
                "Balance Days",
                4,
                "After 4 days, the probability will increase once by the balance coefficient"
                );

        }

    }
}
