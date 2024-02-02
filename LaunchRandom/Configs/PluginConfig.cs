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
		private readonly ConfigFile configFile;

		public bool RANDOM_ENABLE { get; private set; }
		public bool RANDOM_ENABLE_ANOTHER { get; private set; }
		public bool LEVEL_BALANCE_ENABLE { get; private set; }
		public bool REPEAT_LAUNCH { get; private set; }
        public bool BALANCE_RISKLEVEL_ENABLE { get; private set; }
        public string LEVEL_NAME { get; private set; }
		public string LEVEL_NAME_ANOTHER { get; private set; }
		public string LEVEL_WEIGHT { get; private set; }
		public float LEVEL_COEFFICIENT { get; private set; }
		public int LEVEL_DAYS_AFTER { get; private set; }
		public string LEVEL_AMOUNT { get; private set; }
		public string LEVEL_ID { get; private set; }
		public string LEVEL_LV { get; private set; }
        public string LEVEL_LV_EXCEPT { get; private set; }
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
			string section = "Core";
			RANDOM_ENABLE = ConfigEntry(
				section, 
				"Allow Random Launch", 
				defaultValue: false, 
				"If enable, when the lever has been pulled, it will launch to moon randomly,for the last day it will go to company building."
				);
			RANDOM_ENABLE_ANOTHER = ConfigEntry(
				section, 
				"Allow Random Launch(Except)", 
				defaultValue: true, 
				"If true, when the lever has been pulled, it will launch to moon randomly,for the last day it will go to company building."
				);
			LEVEL_BALANCE_ENABLE = ConfigEntry(
				section, 
				"Allow Level Balance", 
				defaultValue: true, 
				"If true, enable the balance setting."
				);
			REPEAT_LAUNCH = ConfigEntry(
				section, 
				"Allow Repeat Launch", 
				defaultValue: false, 
				"If true, it will allow to launch to last moon"
				);
			BALANCE_RISKLEVEL_ENABLE = ConfigEntry(
				section,
				"Allow Moons Level(Except)",
				defaultValue: true,
				"If you install moon mod, take it to true is better."
				);
			section = "Moons Settings";
			LEVEL_NAME = ConfigEntry(
				section, 
				"Moons List", 
				"Assurance,Vow,March,Rend,Dine,Offense,Titan", "These moons will be added in the random list.If need to add new moon, use ',' split moonsBe sure don't have blank between moons."
				);
			LEVEL_WEIGHT = ConfigEntry(
				section, 
				"Moons Weight", 
				"5,5,5,1,1,5,1", 
				"Set the moons probability, high value means high probability of random"
				);
			LEVEL_AMOUNT = ConfigEntry(
				section, 
				"Moons Amount", 
				"0,0,0,550,600,0,700", 
				"If Amount > 0, its Weight will increase after 3 days by the Balance Coefficient."
				);
			LEVEL_ID = ConfigEntry(
				section, 
				"Moons LevelID", 
				"1,2,4,5,6,7,8", ""
				);
			section = "Moons Settings Another";
			LEVEL_NAME_ANOTHER = ConfigEntry(
				section, 
				"Moons List(Except)", 
				"Experimentation,Assurance", 
				"These moons will be removed in the random list."
				);
			LEVEL_LV = ConfigEntry(
				section, 
				"Moons Level", 
				"A,S", 
				"Be like Titan's Level is S, so after N(Balance Days) days, the probability of titan will increase N(Balance Coefficient)."
				);
			LEVEL_LV_EXCEPT = ConfigEntry(
				section,
				"Moons Level(Except)",
				"D,C,B",
				"Except the RiskLevel in this list.It's for adept to some mod moons."
				);
			LEVEL_WEIGHT_ANOTHER_EASY = ConfigEntry(
				section, 
				"Moons Weight Another Easy", 
				5f, 
				"The Weight data is for those riskLevel less than the Moons Level data."
				);
			LEVEL_WEIGHT_ANOTHER_DANGER = ConfigEntry(
				section, 
				"Moons Weight Another Danger", 
				1f, 
				"The Weight data is for those riskLevel more than the Moons Level data."
				);
			section = "Balance Settings";
			LEVEL_COEFFICIENT = ConfigEntry(
				section, 
				"Balance Coefficient", 
				1f, 
				"The higher the value, the higher the probability of those paid moons andthe lower the probability of those free moons"
				);
			LEVEL_DAYS_AFTER = ConfigEntry(
				section, 
				"Balance Days", 
				4, 
				"After 4 days, the probability will increase once by the balance coefficient"
				);
		}

	}
}
