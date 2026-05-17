using System.Collections.Generic;

namespace DBLoad
{
    public class Reward
    {
        public static Dictionary<int, RewardData> Dic
        {
            get
            {
                if (Reward.m_dic == null)
                {
                    Reward.Init();
                }
                return Reward.m_dic;
            }
        }
        public static void Init() { }
        public static Dictionary<int, RewardData> m_dic;
    }
}
