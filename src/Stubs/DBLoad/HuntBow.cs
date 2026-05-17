using System.Collections.Generic;

namespace DBLoad
{
    public class HuntBow
    {
        public static Dictionary<int, HuntBowData> Dic
        {
            get
            {
                if (HuntBow.m_dic == null)
                {
                    HuntBow.Init();
                }
                return HuntBow.m_dic;
            }
        }
        public static void Init() { }
        public static Dictionary<int, HuntBowData> m_dic;
    }
}
