using BepInEx.Logging;
using LaunchRandom.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace LaunchRandom.Managers
{


    
    class RDManager : NetworkBehaviour
    {
		public static RDManager Instance;

		public PluginConfig cfg;

		internal static ManualLogSource mls;

		internal StartOfRound startOfRoundInstance;

		public string[] moonsListExcept;
		public float[] moonsWeightExcept;
		public int moonsAllowAmount = 0;
		public int moonsExceptAmount = 0;
        public string[] moonsLevelBalanceExcept;
        public int[] moonsBalance;
		public int[] moonsLevelIDExcept;
		public int preLevel = 0;
		public int tempLevel = 0;
		public int companyBuildingLevelID = -1;
		public int levelDay;
		public float levelBalancesC;
		public bool randomEnableAnother;
		public bool repeatLaunch;
		public bool threeDaysBefore;
		public bool leverHasBeenPulled = true;
		public bool moonsListExceptNull = false;
		public void InitSettings()
		{
            if (cfg.LEVEL_DAYS_AFTER.HasValue)
            {
				levelDay = (int)cfg.LEVEL_DAYS_AFTER;
			}
            else
            {
				levelDay = 4;
            }

			levelBalancesC = cfg.LEVEL_COEFFICIENT;

			if(cfg.LEVEL_NAME_ANOTHER.Length != 0)
            {
				moonsListExcept = cfg.LEVEL_NAME_ANOTHER.Split(",");
			}

			if (cfg.LEVEL_LV_EXCEPT.Length != 0)
            {
				moonsLevelBalanceExcept = cfg.LEVEL_LV_EXCEPT.Split(",");
			}
            else
            {
				moonsLevelBalanceExcept = new string[]{"D,C,B" };
            }

			randomEnableAnother = cfg.RANDOM_ENABLE_ANOTHER;
			repeatLaunch = cfg.REPEAT_LAUNCH;
			threeDaysBefore = cfg.THREE_DAYS_SETTING;
		}
		public void GetMoonsInfoExcept()
		{
			mls.LogInfo("GetMoonsInfoExcept");
			moonsBalance = new int[startOfRoundInstance.levels.Length];
			moonsLevelIDExcept = new int[startOfRoundInstance.levels.Length];
			for (int i = 0; i < moonsListExcept.Length; i++)
			{
				for (int j = 0; j < startOfRoundInstance.levels.Length; j++)
				{

                    for (int k = 0; k < moonsLevelBalanceExcept.Length; k++)
                    {
                        if (moonsBalance[j] != 1 && startOfRoundInstance.levels[j].riskLevel.IndexOf(moonsLevelBalanceExcept[k]) != -1)
                        {
                            moonsBalance[j] = 1;
                        }
                    }

                    if (moonsLevelIDExcept[j] != -1)
					{
						moonsLevelIDExcept[j] = j;
					}
					if (startOfRoundInstance.levels[j].ToString().Contains("CompanyBuilding"))
					{
						companyBuildingLevelID = j;
						moonsLevelIDExcept[j] = -1;
						moonsBalance[j] = 0;
					}
					if (startOfRoundInstance.levels[j].ToString().IndexOf(moonsListExcept[i]) != -1)
					{
						moonsLevelIDExcept[j] = -1;
					}
				}
			}
		}
		public void GetMoonsInfoExceptNull()
        {
			mls.LogInfo("GetMoonsInfoExceptNull");
			moonsBalance = new int[startOfRoundInstance.levels.Length];
			moonsLevelIDExcept = new int[startOfRoundInstance.levels.Length];
            for (int j = 0; j < startOfRoundInstance.levels.Length; j++)
            {
                for (int k = 0; k < moonsLevelBalanceExcept.Length; k++)
                {
                    if (moonsBalance[j] != 1 && startOfRoundInstance.levels[j].riskLevel.IndexOf(moonsLevelBalanceExcept[k]) != -1)
                    {
                        moonsBalance[j] = 1;
                        //mls.LogInfo(startOfRoundInstance.levels[j].ToString() + " - " + startOfRoundInstance.levels[j].riskLevel + " - " + moonsLevelBalanceExcept[k] + " - " + moonsBalance[j]);
                    }
                }
                if (moonsLevelIDExcept[j] != -1)
                {
                    moonsLevelIDExcept[j] = j;
                }
                if (startOfRoundInstance.levels[j].ToString().Contains("CompanyBuilding"))
                {
                    companyBuildingLevelID = j;
                    moonsLevelIDExcept[j] = -1;
                    moonsBalance[j] = 0;
                }
            }
        }
		public void GetMoonsWeightExcept()
		{
			float LEVEL_WEIGHT_ANOTHER_EASY = cfg.LEVEL_WEIGHT_ANOTHER_EASY;
			float LEVEL_WEIGHT_ANOTHER_DANGER = cfg.LEVEL_WEIGHT_ANOTHER_DANGER;
			moonsWeightExcept = new float[startOfRoundInstance.levels.Length];
			for (int i = 0; i < moonsBalance.Length; i++)
			{
				if (moonsLevelIDExcept[i] != -1)
				{
					if (moonsBalance[i] != 0)
					{
						moonsWeightExcept[i] = LEVEL_WEIGHT_ANOTHER_EASY;
					}
					else
					{
						moonsWeightExcept[i] = LEVEL_WEIGHT_ANOTHER_DANGER;
					}
				}
			}
		}
		public void InputMoonsInfo()
		{
			for (int i = 0; i < startOfRoundInstance.levels.Length; i++)
			{
				mls.LogInfo(i + " - " + startOfRoundInstance.levels[i].ToString());
			}
		}
		public void InputTestInfo()
		{
			for (int i = 0; i < moonsLevelIDExcept.Length; i++)
			{
				mls.LogInfo(moonsLevelIDExcept[i] + " - " + startOfRoundInstance.levels[i].ToString());
				mls.LogInfo("riskLevel:"+ startOfRoundInstance.levels[i].riskLevel + " - moonsBalance:" + moonsBalance[i] + " - moonsWeight:" + moonsWeightExcept[i]);
			}
		}
		public float GetMoonsWeightExceptSum()
		{
			float num = 0f;
			for (int i = 0; i < moonsLevelIDExcept.Length; i++)
			{
				if (moonsLevelIDExcept[i] != -1)
				{
					num += moonsWeightExcept[i];
				}
			}
			return num;
		}
		public static double NextDouble(Random rd, double Min, double Max)
		{
			return rd.NextDouble() * (Max - Min) + Min;
		}
		public int StartToRandomLevelAnother()
		{
			float num = 0f;
			float num2 = 0f;
			int result = 0;
			var seed = Guid.NewGuid().GetHashCode();
			Random rd = new Random(seed);
			double num3 = NextDouble(rd, 0.0, GetMoonsWeightExceptSum());
			for (int i = 0; i < moonsWeightExcept.Length; i++)
			{
				//mls.LogInfo("top:" + num + " bottom:" + num2 + " target:" + num3);
				if (moonsLevelIDExcept[i] == -1)
				{
					//mls.LogDebug("CONTINUE");
					continue;
				}
				num += moonsWeightExcept[i];
				if (num3 <= (double)num && num3 >= (double)num2)
				{
					result = i;
					break;
				}
				num2 += moonsWeightExcept[i];
			}
			return result;
		}
		public void BalanceOfMoonsExcept()
		{
			mls.LogInfo(cfg.LEVEL_WEIGHT_ANOTHER_DANGER + levelBalancesC * (float)(startOfRoundInstance.gameStats.daysSpent / levelDay));
			mls.LogInfo("daySpent: " + startOfRoundInstance.gameStats.daysSpent);
			mls.LogInfo("levelDay: " + levelDay);

			for (int i = 0; i < moonsBalance.Length; i++)
			{
                if (moonsBalance[i] == 0 && moonsLevelIDExcept[i] != -1)
                {
                    moonsWeightExcept[i] = cfg.LEVEL_WEIGHT_ANOTHER_DANGER + levelBalancesC * (float)(startOfRoundInstance.gameStats.daysSpent / levelDay);
                }
            }
		}
		public void InputBalanceOfMoonsWeight()
        {
			for(int i = 0;i < moonsBalance.Length; i++)
            {
				mls.LogInfo(i + " - moonsLevelIDExcept: " + moonsLevelIDExcept[i] + " - moonsBalance: " + moonsBalance[i] + " - moonsWeightExcept: " + moonsWeightExcept[i]);
            }
        }
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			cfg = Plugin.cfg;
			mls = Logger.CreateLogSource("LaunchRandom.RDManager");
			InitSettings();
		}
	}
}
