using System.Collections.Generic;

namespace DBLoad
{
    public class FishRod
    {
        public static Dictionary<int, FishRodData> Dic
        {
            get
            {
                if (FishRod.m_dic == null)
                {
                    FishRod.Init();
                }
                return FishRod.m_dic;
            }
        }
        public static void Init() { }
        public static Dictionary<int, FishRodData> m_dic;
    }
}
