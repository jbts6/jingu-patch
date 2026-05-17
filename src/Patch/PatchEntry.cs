using System.Collections.Generic;
using DBLoad;

namespace JinguModPatch
{
    public static class PatchEntry
    {
        public static readonly bool Loaded;

        static PatchEntry()
        {
            Loaded = true;
        }

        /* 采集 100 倍经验 */
        public static void Prefix_SaveData_AddEffectValue(EffectId id, ref float add, bool needTips)
        {
            add *= 100f;
        }

        /* 狩猎初始猎弓 */
        public static void Postfix_HuntBow_Init()
        {
            if (HuntBow.m_dic != null && HuntBow.m_dic.ContainsKey(230001))
            {
                HuntBow.m_dic[230001] = new HuntBowData(230001, 220, 300, 500);
            }
        }


        /* 狩猎初始钓竿 */
        public static void Postfix_FishRod_Init()
        {
            if (FishRod.m_dic != null && FishRod.m_dic.ContainsKey(220001))
            {
                FishRod.m_dic[220001] = new FishRodData(220001, 300, 300, 80);
            }
            
        }
    }
}
