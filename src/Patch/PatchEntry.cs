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

        /* 生活技艺 100倍经验 */
        public static void Prefix_SaveData_AddEffectValue(EffectId id, ref float add, bool needTips)
        {
            add *= 100f;
        }

        /* 修改猎弓数据 */
        public static bool Prefix_HuntBow_Init()
        {
            HuntBow.m_dic = new Dictionary<int, HuntBowData>(8);
            HuntBowData huntBowData = new HuntBowData(230001, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230002, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230003, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230004, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230005, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230006, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230007, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            huntBowData = new HuntBowData(230008, 220, 300, 500);
            HuntBow.m_dic[huntBowData.m_id] = huntBowData;
            return false;
        }


        /* 修改渔具数据 */
        public static bool Prefix_FishRod_Init()
        {
            FishRod.m_dic = new Dictionary<int, FishRodData>(8);
            FishRodData fishRodData = new FishRodData(220001, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220002, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220003, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220004, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220005, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220006, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220007, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            fishRodData = new FishRodData(220008, 300, 300, 70);
            FishRod.m_dic[fishRodData.m_id] = fishRodData;
            return false;
        }
    }
}
