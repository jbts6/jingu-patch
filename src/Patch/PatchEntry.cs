using System.Collections.Generic;
using DBLoad;
using UnityEngine.Pool;
using UnityEngine;

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

        public static void Postfix_DBLoad_Hunt_Init()
        {
            if (Hunt.Dic != null)
            {
                if (Hunt.Dic.ContainsKey(101))
                {
                    Hunt.Dic[101] = new HuntData(101, new int[] { 40202 }, new int[] { 100 }, new int[] { 100 }, 120, 85, 12, 0, 10, 5);
                }
                if (Hunt.Dic.ContainsKey(104))
                {
                    Hunt.Dic[104] = new HuntData(104, new int[] { 40203 }, new int[] { 100 }, new int[] { 100 }, 250, 130, 14, 0, 15, 7);
                }
                if (Hunt.Dic.ContainsKey(110))
                {
                    Hunt.Dic[110] = new HuntData(110, new int[] { 40204 }, new int[] { 100 }, new int[] { 100 }, 500, 160, 20, 16, 25, 10);
                }
                if (Hunt.Dic.ContainsKey(116))
                {
                    Hunt.Dic[116] = new HuntData(116, new int[] { 40210 }, new int[] { 100 }, new int[] { 100 }, 450, 170, 18, 16, 25, 10);
                }

            }
        }

       public static void Postfix_Reward_Init()
        {
            if (Reward.Dic != null) {
                if (Reward.Dic.ContainsKey(1001))
                {
                    Reward.Dic[1001] = new RewardData(1001, new int[] { 90007, 91010, 91010, 91010, 91010 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1002))
                {
                    Reward.Dic[1002] = new RewardData(1002, new int[] { 90005, 91011, 91011, 91011, 91011 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1003))
                {
                    Reward.Dic[1003] = new RewardData(1003, new int[] { 91007, 91014, 91014, 91014, 91014 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1004))
                {
                    Reward.Dic[1004] = new RewardData(1004, new int[] { 91009, 91018, 91018, 91018, 91018 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1005))
                {
                    Reward.Dic[1005] = new RewardData(1005, new int[] { 91005, 91013, 91013, 91013, 91013 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1006))
                {
                    Reward.Dic[1006] = new RewardData(1006, new int[] { 90002, 90004, 90004, 90004, 90004 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1007))
                {
                    Reward.Dic[1007] = new RewardData(1007, new int[] { 90004, 90001, 90001, 90001, 90001 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1008))
                {
                    Reward.Dic[1008] = new RewardData(1008, new int[] { 91008, 91016, 91016, 91016, 91016 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1009))
                {
                    Reward.Dic[1009] = new RewardData(1009, new int[] { 91016, 91022, 91022, 91022, 91022 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1010))
                {
                    Reward.Dic[1010] = new RewardData(1010, new int[] { 91006, 91015, 91015, 91015, 91015 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1011))
                {
                    Reward.Dic[1011] = new RewardData(1011, new int[] { 91015, 91019, 91019, 91019, 91019 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1012))
                {
                    Reward.Dic[1012] = new RewardData(1012, new int[] { 90003, 91017, 91017, 91017, 91017 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1013))
                {
                    Reward.Dic[1013] = new RewardData(1013, new int[] { 91017, 91020, 91020, 91020, 91020 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1014))
                {
                    Reward.Dic[1014] = new RewardData(1014, new int[] { 90006, 91012, 91012, 91012, 91012 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1015))
                {
                    Reward.Dic[1015] = new RewardData(1015, new int[] { 91012, 91021, 91021, 91021, 91021 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1016))
                {
                    Reward.Dic[1016] = new RewardData(1016, new int[] { 90009, 90010, 90010, 90010, 90010 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1017))
                {
                    Reward.Dic[1017] = new RewardData(1017, new int[] { 90010, 90011, 90011, 90011, 90011 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15);
                }
                if (Reward.Dic.ContainsKey(1018))
                {
                    Reward.Dic[1018] = new RewardData(1018, new int[] { 20025 }, new int[] { 100 }, new int[] { 100 }, new int[1], 308, 15);
                }
                if (Reward.Dic.ContainsKey(2001))
                {
                    Reward.Dic[2001] = new RewardData(2001, new int[] { 91001, 92207, 92207, 92207, 92207 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 128, 128, 128, 128 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2002))
                {
                    Reward.Dic[2002] = new RewardData(2002, new int[] { 92201, 92204, 92204, 92204, 92204 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 128, 128, 128, 128 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2003))
                {
                    Reward.Dic[2003] = new RewardData(2003, new int[] { 92202, 92210, 92210, 92210, 92210 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2004))
                {
                    Reward.Dic[2004] = new RewardData(2004, new int[] { 92210, 92214, 92214, 92214, 92214 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2005))
                {
                    Reward.Dic[2005] = new RewardData(2005, new int[] { 92205, 91003, 91003, 91003, 91003 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2006))
                {
                    Reward.Dic[2006] = new RewardData(2006, new int[] { 91003, 92213, 92213, 92213, 92213 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2007))
                {
                    Reward.Dic[2007] = new RewardData(2007, new int[] { 91002, 92208, 92208, 92208, 92208 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2008))
                {
                    Reward.Dic[2008] = new RewardData(2008, new int[] { 92208, 91004, 91004, 91004, 91004 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2009))
                {
                    Reward.Dic[2009] = new RewardData(2009, new int[] { 92203, 92209, 92209, 92209, 92209 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2010))
                {
                    Reward.Dic[2010] = new RewardData(2010, new int[] { 92209, 92212, 92212, 92212, 92212 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2011))
                {
                    Reward.Dic[2011] = new RewardData(2011, new int[] { 92206, 92211, 92211, 92211, 92211 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(2012))
                {
                    Reward.Dic[2012] = new RewardData(2012, new int[] { 92211, 92215, 92215, 92215, 92215 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15);
                }
                if (Reward.Dic.ContainsKey(20001))
                {
                    Reward.Dic[20001] = new RewardData(20001, new int[] { 41001, 41002, 41006, 41007, 41011, 41015, 41015, 41015, 41015 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 240, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20002))
                {
                    Reward.Dic[20002] = new RewardData(20002, new int[] { 41003, 41004, 41008, 41009, 41013, 41016, 41016, 41016, 41016 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20003))
                {
                    Reward.Dic[20003] = new RewardData(20003, new int[] { 41005, 41002, 41006, 41010, 41012, 41017, 41017, 41017, 41017 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20004))
                {
                    Reward.Dic[20004] = new RewardData(20004, new int[] { 41005, 41002, 41006, 41010, 41012, 200010, 200010, 200010, 200010 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 0, 1, 2, 3 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20005))
                {
                    Reward.Dic[20005] = new RewardData(20005, new int[] { 41001, 41003, 41007, 41006, 41014, 41022, 41022, 41022, 41022 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20006))
                {
                    Reward.Dic[20006] = new RewardData(20006, new int[] { 41004, 41003, 41006, 41009, 41011, 41018, 41018, 41018, 41018 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20007))
                {
                    Reward.Dic[20007] = new RewardData(20007, new int[] { 41005, 41002, 41006, 41010, 41013, 41019, 41019, 41019, 41019 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20008))
                {
                    Reward.Dic[20008] = new RewardData(20008, new int[] { 41005, 41002, 41006, 41007, 41011, 41021, 41021, 41021, 41021 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20009))
                {
                    Reward.Dic[20009] = new RewardData(20009, new int[] { 41001, 41003, 41007, 41006, 41014, 41023, 41023, 41023, 41023 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(20010))
                {
                    Reward.Dic[20010] = new RewardData(20010, new int[] { 41005, 41002, 41006, 41010, 41012, 41020, 41020, 41020, 41020 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 400 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15);
                }
                if (Reward.Dic.ContainsKey(30001))
                {
                    Reward.Dic[30001] = new RewardData(30001, new int[] { 1, 2, 3, 7, 8, 13, 101 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120);
                }
                if (Reward.Dic.ContainsKey(30002))
                {
                    Reward.Dic[30002] = new RewardData(30002, new int[] { 2, 3, 4, 5, 6, 16, 104, 116 }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 4, 50, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5, 5 }, 304, 120);
                }
                if (Reward.Dic.ContainsKey(30003))
                {
                    Reward.Dic[30003] = new RewardData(30003, new int[] { 2, 4, 12, 7, 8, 14, 104 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120);
                }
                if (Reward.Dic.ContainsKey(30004))
                {
                    Reward.Dic[30004] = new RewardData(30004, new int[] { 203 }, new int[] { 1 }, new int[] { 100 }, new int[1], 304, 120);
                }
                if (Reward.Dic.ContainsKey(30005))
                {
                    Reward.Dic[30005] = new RewardData(30005, new int[] { 3 }, new int[] { 1 }, new int[] { 100 }, new int[1], 304, 120);
                }
                if (Reward.Dic.ContainsKey(30006))
                {
                    Reward.Dic[30006] = new RewardData(30006, new int[] { 1, 3, 12, 6, 11, 10, 101, 110 }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 100, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5, 5 }, 304, 120);
                }
                if (Reward.Dic.ContainsKey(30007))
                {
                    Reward.Dic[30007] = new RewardData(30007, new int[] { 7, 8, 9, 10, 13, 15, 110 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 2, 2, 2, 50, 200 }, new int[] { 0, 0, 0, 0, 0, 0, 5 }, 304, 120);
                }
                if (Reward.Dic.ContainsKey(30008))
                {
                    Reward.Dic[30008] = new RewardData(30008, new int[] { 1, 3, 4, 6, 8, 15, 104 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120);
                }
                if (Reward.Dic.ContainsKey(30009))
                {
                    Reward.Dic[30009] = new RewardData(30009, new int[] { 1, 2, 12, 7, 12, 9, 101 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120);
                }
            }
        }
    }
}
