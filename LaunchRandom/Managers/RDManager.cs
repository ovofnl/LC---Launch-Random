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

        internal StartOfRound startOfRoundInstance;
        public string[] moonsList;
        public string[] moonsListExcept;
        public string[] moonsWeight;
        public float[] moonsWeightExcept;
        public string[] moonsAmount;
        public string[] moonsLevelID;
        public string[] moonsLevelBalance;
        public int[] moonsBalance;
        public int[] moonsLevelIDExcept;

        public int companyBuildingLevelID = -1;
        public int levelDay;
        public float levelBalancesC;

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
            float sum = 0f;
            for (int i = 0; i < moonsWeight.Length; i++)
            {
                sum += float.Parse(moonsWeight[i]);
            }
            return sum;
        }
        public void GetMoonsInfoExcept()
        {
            moonsBalance = new int[startOfRoundInstance.levels.Length];
            moonsLevelIDExcept = new int[startOfRoundInstance.levels.Length];

            //3: 1.route + moonsName 2.moonsName 3.Level + LevelId + moonsName
            for (int i = 0; i < moonsListExcept.Length; i++)
            {
                for (int j = 0; j < startOfRoundInstance.levels.Length; j++)
                {
                    for (int k = 0; k < moonsLevelBalance.Length; k++)
                    {
                        if (startOfRoundInstance.levels[j].riskLevel.IndexOf(moonsLevelBalance[k]) != -1)
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
            float moonsWeightEasy = cfg.LEVEL_WEIGHT_ANOTHER_EASY;
            float moonsWeightDanger = cfg.LEVEL_WEIGHT_ANOTHER_DANGER;
            moonsWeightExcept = new float[startOfRoundInstance.levels.Length];
            for(int i = 0; i < moonsBalance.Length; i++)
            {
                if(moonsLevelIDExcept[i] == -1)
                {
                    continue;
                }
                if(moonsBalance[i] != 1)
                {
                    moonsWeightExcept[i] = moonsWeightEasy;
                }
                else
                {
                    moonsWeightExcept[i] = moonsWeightDanger;
                }
            }
        }

        public float GetMoonsWeightExceptSum()
        {
            float sum = 0f;
            for(int i = 0; i < moonsBalance.Length; i++)
            {
                sum += moonsWeightExcept[i];
            }
            return sum;
        }
        public static double NextDouble(Random rd, double Min, double Max)
        {
            return rd.NextDouble() * (Max - Min) + Min;
        }
        public int StartToRandomLevelAnother()
        {
            float top = 0f, bottom = 0f;
            int LevelID = 0;
            Random rd = new Random();
            double target = NextDouble(rd, 0, GetMoonsWeightExceptSum());
            for(int i = 0;i < moonsWeightExcept.Length; i++)
            {
                if(moonsLevelIDExcept[i] == -1)
                {
                    continue;
                }
                top += moonsWeightExcept[i];
                if(target <= top && target >= bottom)
                {
                    //Console.WriteLine("top:" + top + " " + "bottom:" + bottom + " " + "target:" + target);
                    LevelID = i;
                    break;
                }
                else
                {
                    bottom += moonsWeightExcept[i];
                }
            }
            return LevelID ;
        }
        public int StartToRandomLevel()
        {
            float top = 0f, bottom = 0f;
            int LevelID = 0;
            Random rd = new Random();
            double target = NextDouble(rd, 0, GetMoonsWeight());
            for (int i = 0; i < moonsWeight.Length; i++) 
            {
                top += float.Parse(moonsWeight[i]);
                if (target <= top && target >= bottom)
                {
                    //Console.WriteLine("temp:" + temp + " " + "temp2:" + temp2 + " " + "target:" + target);
                    LevelID = int.Parse(moonsLevelID[i]);
                    break;
                }
                else
                {
                    bottom += float.Parse(moonsWeight[i]);
                }
                
            }
            return LevelID;
        }
        public void BalanceOfMoons()
        {
            string[] moonsWeightTemp = cfg.LEVEL_WEIGHT.Split(","); 
            for (int i = 0; i < moonsWeight.Length; i++)
            {
                if(float.Parse(moonsAmount[i]) > 0)
                {
                    
                    moonsWeight[i] = (float.Parse(moonsWeightTemp[i]) + levelBalancesC * levelBalancesC * (startOfRoundInstance.gameStats.daysSpent / levelDay)).ToString();
                    
                }
                //Console.WriteLine("moonsList:" + moonsList[i] + "  moonsWeight:" + moonsWeight[i]);
            }
        }
        public void BalanceOfMoonsExcept()
        {
            for (int i = 0; i < moonsBalance.Length; i++)
            {
                if (moonsBalance[i] == 1)
                {
                    moonsWeightExcept[i] = cfg.LEVEL_WEIGHT_ANOTHER_DANGER + levelBalancesC * (startOfRoundInstance.gameStats.daysSpent / levelDay);
                }
                //Console.WriteLine("moonsList:" + moonsLevelIDExcept[i] + "  moonsWeight:" + moonsWeightExcept[i]);
            }
        }

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            cfg = Plugin.cfg;
            InitSettings();
        }
    }
}
