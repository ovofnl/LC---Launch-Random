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

		public string[] moonsList;
		public string[] moonsListExcept;
		public string[] moonsWeight;
		public float[] moonsWeightExcept;
		public string[] moonsAmount;
		public string[] moonsLevelID;
		public string[] moonsLevelBalance;
        private string[] moonsLevelBalanceExcept;
        public int[] moonsBalance;
		public int[] moonsLevelIDExcept;
		public int preLevel = 0;
		public int companyBuildingLevelID = -1;
		public int levelDay;
		public float levelBalancesC;
		public bool randomEnable;
		public bool randomEnableAnother;
		public bool repeatLaunch;
		public bool balanceRiskLevelEnable;

		public void InitSettings()
		{
			levelDay = cfg.LEVEL_DAYS_AFTER;
			levelBalancesC = cfg.LEVEL_COEFFICIENT;
			moonsList = cfg.LEVEL_NAME.Split(",");
			moonsWeight = cfg.LEVEL_WEIGHT.Split(",");
			moonsAmount = cfg.LEVEL_AMOUNT.Split(",");
			moonsLevelID = cfg.LEVEL_ID.Split(",");
			moonsListExcept = cfg.LEVEL_NAME_ANOTHER.Split(",");
			moonsLevelBalance = cfg.LEVEL_LV.Split(",");
			moonsLevelBalanceExcept = cfg.LEVEL_LV_EXCEPT.Split(",");
			randomEnable = cfg.RANDOM_ENABLE;
			randomEnableAnother = cfg.RANDOM_ENABLE_ANOTHER;
			repeatLaunch = cfg.REPEAT_LAUNCH;
			balanceRiskLevelEnable = cfg.BALANCE_RISKLEVEL_ENABLE;
		}
		public void GetMoonsInfo()
		{
			moonsList = cfg.LEVEL_NAME.Split(",");
			moonsWeight = cfg.LEVEL_WEIGHT.Split(",");
			moonsAmount = cfg.LEVEL_AMOUNT.Split(",");
			moonsLevelID = cfg.LEVEL_ID.Split(",");
		}
		public float GetMoonsWeight()
		{
			float num = 0f;
			for (int i = 0; i < moonsWeight.Length; i++)
			{
				num += float.Parse(moonsWeight[i]);
			}
			return num;
		}
		public void GetMoonsInfoExcept()
		{
			moonsBalance = new int[startOfRoundInstance.levels.Length];
			moonsLevelIDExcept = new int[startOfRoundInstance.levels.Length];
			for (int i = 0; i < moonsListExcept.Length; i++)
			{
				for (int j = 0; j < startOfRoundInstance.levels.Length; j++)
				{
                    if (balanceRiskLevelEnable)
                    {
						for (int k = 0; k < moonsLevelBalanceExcept.Length; k++)
						{
							if (moonsBalance[j] != 1 && startOfRoundInstance.levels[j].riskLevel.IndexOf(moonsLevelBalanceExcept[k]) != -1)
							{
								moonsBalance[j] = 1;
								mls.LogInfo(startOfRoundInstance.levels[j].ToString() + " - " + startOfRoundInstance.levels[j].riskLevel + " - " + moonsLevelBalanceExcept[k] + " - " + moonsBalance[j]);
							}
						}
					}
                    else
                    {
						for(int k = 0; k < moonsLevelBalance.Length; k++)
                        {
							if (moonsBalance[j] != 1 && startOfRoundInstance.levels[j].riskLevel.IndexOf(moonsLevelBalance[k]) != -1)
							{
								moonsBalance[j] = 1;
							}
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
		public void GetMoonsWeightExcept()
		{
			float LEVEL_WEIGHT_ANOTHER_EASY = cfg.LEVEL_WEIGHT_ANOTHER_EASY;
			float LEVEL_WEIGHT_ANOTHER_DANGER = cfg.LEVEL_WEIGHT_ANOTHER_DANGER;
			moonsWeightExcept = new float[startOfRoundInstance.levels.Length];
			for (int i = 0; i < moonsBalance.Length; i++)
			{
				if (moonsLevelIDExcept[i] != -1)
				{                
					if (balanceRiskLevelEnable)
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
                    else
                    {
						if (moonsBalance[i] != 1)
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
			Random rd = new Random();
			double num3 = NextDouble(rd, 0.0, GetMoonsWeightExceptSum());
			for (int i = 0; i < moonsWeightExcept.Length; i++)
			{
				mls.LogInfo("top:" + num + " bottom:" + num2 + " target:" + num3);
				if (moonsLevelIDExcept[i] == -1)
				{
					mls.LogDebug("CONTINUE");
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
		public int StartToRandomLevel()
		{
			float num = 0f;
			float num2 = 0f;
			int result = 0;
			Random rd = new Random();
			double num3 = NextDouble(rd, 0.0, GetMoonsWeight());
			for (int i = 0; i < moonsWeight.Length; i++)
			{
				num += float.Parse(moonsWeight[i]);
				if (num3 <= (double)num && num3 >= (double)num2)
				{
					mls.LogInfo("top:" + num + " bottom:" + num2 + " target:" + num3);
					result = int.Parse(moonsLevelID[i]);
					break;
				}
				num2 += float.Parse(moonsWeight[i]);
			}
			return result;
		}
		public void BalanceOfMoons()
		{
			string[] array = cfg.LEVEL_WEIGHT.Split(",");
			for (int i = 0; i < moonsWeight.Length; i++)
			{
				if (float.Parse(moonsAmount[i]) > 0f)
				{
					moonsWeight[i] = (float.Parse(array[i]) + levelBalancesC * (float)(startOfRoundInstance.gameStats.daysSpent / levelDay)).ToString();
				}
			}
		}
		public void BalanceOfMoonsExcept()
		{
			for (int i = 0; i < moonsBalance.Length; i++)
			{
                if (balanceRiskLevelEnable)
                {
					if (moonsBalance[i] == 0 && moonsLevelIDExcept[i] != -1)
					{
						moonsWeightExcept[i] = cfg.LEVEL_WEIGHT_ANOTHER_DANGER + levelBalancesC * (float)(startOfRoundInstance.gameStats.daysSpent / levelDay);
					}
				}
                else
                {
					if (moonsBalance[i] == 1 && moonsLevelIDExcept[i] != -1)
					{
						moonsWeightExcept[i] = cfg.LEVEL_WEIGHT_ANOTHER_DANGER + levelBalancesC * (float)(startOfRoundInstance.gameStats.daysSpent / levelDay);
					}
				}
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
