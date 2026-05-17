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

        public static void Prefix_SaveData_AddEffectValue(EffectId id, ref float add, bool needTips)
        {
            add *= 100f;
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
    }
}
