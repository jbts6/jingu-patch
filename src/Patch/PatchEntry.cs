using System.Collections.Generic;
using DBLoad;
using UnityEngine.Pool;

namespace JinguModPatch
{
    public static class PatchEntry
    {
        public static readonly bool Loaded;

        static PatchEntry()
        {
            Loaded = true;
        }

        public static void Prefix_SaveData_AddEffectValue(EffectId id, ref float add, bool needTips)
        {
            switch (id)
            {
                case EffectId.驯兽:
                case EffectId.挖掘:
                case EffectId.钓鱼:
                case EffectId.打猎:
                case EffectId.炼丹:
                case EffectId.下棋:
                case EffectId.打造:
                case EffectId.采集:
                    add *= 100f;
                    break;
            }
        }

        public static void Postfix_HuntBow_Init()
        {
            if (HuntBow.Dic != null && HuntBow.Dic.ContainsKey(230001))
            {
                HuntBow.Dic[230001] = new HuntBowData(230001, 220, 300, 500);
            }
        }

        public static void Postfix_FishRod_Init()
        {
            if (FishRod.Dic != null && FishRod.Dic.ContainsKey(220001))
            {
                FishRod.Dic[220001] = new FishRodData(220001, 300, 300, 80);
            }
        }

        // public static void Prefix_FishWindow_GameLoop(FishWindow __instance)
        // {
        //     __instance.m_reward.m_rewardNum[__instance.m_rewardIndex] = 100;
        //     __instance.Time = 0f;
        //     __instance.CurPull = 0f;
        // }
    }
}
