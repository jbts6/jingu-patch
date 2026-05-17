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
            if (Reward.Dic != null)
            {
                RewardData[] rewardDataList = new RewardData[]
                {
                    new RewardData(1001, new int[] { 90007, 91010, 91010, 91010, 91010 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1002, new int[] { 90005, 91011, 91011, 91011, 91011 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1003, new int[] { 91007, 91014, 91014, 91014, 91014 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1004, new int[] { 91009, 91018, 91018, 91018, 91018 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1005, new int[] { 91005, 91013, 91013, 91013, 91013 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1006, new int[] { 90002, 90004, 90004, 90004, 90004 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1007, new int[] { 90004, 90001, 90001, 90001, 90001 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1008, new int[] { 91008, 91016, 91016, 91016, 91016 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1009, new int[] { 91016, 91022, 91022, 91022, 91022 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1010, new int[] { 91006, 91015, 91015, 91015, 91015 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1011, new int[] { 91015, 91019, 91019, 91019, 91019 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1012, new int[] { 90003, 91017, 91017, 91017, 91017 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1013, new int[] { 91017, 91020, 91020, 91020, 91020 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1014, new int[] { 90006, 91012, 91012, 91012, 91012 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1015, new int[] { 91012, 91021, 91021, 91021, 91021 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1016, new int[] { 90009, 90010, 90010, 90010, 90010 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1017, new int[] { 90010, 90011, 90011, 90011, 90011 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 308, 15),
                    new RewardData(1018, new int[] { 20025 }, new int[] { 100 }, new int[] { 100 }, new int[1], 308, 15),
                    new RewardData(2001, new int[] { 91001, 92207, 92207, 92207, 92207 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 128, 128, 128, 128 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2002, new int[] { 92201, 92204, 92204, 92204, 92204 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 128, 128, 128, 128 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2003, new int[] { 92202, 92210, 92210, 92210, 92210 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2004, new int[] { 92210, 92214, 92214, 92214, 92214 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2005, new int[] { 92205, 91003, 91003, 91003, 91003 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2006, new int[] { 91003, 92213, 92213, 92213, 92213 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2007, new int[] { 91002, 92208, 92208, 92208, 92208 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2008, new int[] { 92208, 91004, 91004, 91004, 91004 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 200, 200, 200, 200 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2009, new int[] { 92203, 92209, 92209, 92209, 92209 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2010, new int[] { 92209, 92212, 92212, 92212, 92212 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2011, new int[] { 92206, 92211, 92211, 92211, 92211 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 140, 140, 140, 140 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(2012, new int[] { 92211, 92215, 92215, 92215, 92215 }, new int[] { 100, 100, 100, 100, 100 }, new int[] { 100, 300, 300, 300, 300 }, new int[] { 0, 3, 5, 7, 9 }, 302, 15),
                    new RewardData(20001, new int[] { 41001, 41002, 41006, 41007, 41011, 41015, 41015, 41015, 41015 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 240, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20002, new int[] { 41003, 41004, 41008, 41009, 41013, 41016, 41016, 41016, 41016 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20003, new int[] { 41005, 41002, 41006, 41010, 41012, 41017, 41017, 41017, 41017 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20004, new int[] { 41005, 41002, 41006, 41010, 41012, 200010, 200010, 200010, 200010 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 0, 1, 2, 3 }, 303, 15),
                    new RewardData(20005, new int[] { 41001, 41003, 41007, 41006, 41014, 41022, 41022, 41022, 41022 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20006, new int[] { 41004, 41003, 41006, 41009, 41011, 41018, 41018, 41018, 41018 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20007, new int[] { 41005, 41002, 41006, 41010, 41013, 41019, 41019, 41019, 41019 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20008, new int[] { 41005, 41002, 41006, 41007, 41011, 41021, 41021, 41021, 41021 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20009, new int[] { 41001, 41003, 41007, 41006, 41014, 41023, 41023, 41023, 41023 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(20010, new int[] { 41005, 41002, 41006, 41010, 41012, 41020, 41020, 41020, 41020 }, new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 400 }, new int[] { 1, 1, 1, 1, 1, 55, 125, 125, 125 }, new int[] { 0, 0, 1, 1, 2, 3, 5, 7, 9 }, 303, 15),
                    new RewardData(30001, new int[] { 1, 2, 3, 7, 8, 13, 101 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120),
                    new RewardData(30002, new int[] { 2, 3, 4, 5, 6, 16, 104, 116 }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 4, 50, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5, 5 }, 304, 120),
                    new RewardData(30003, new int[] { 2, 4, 12, 7, 8, 14, 104 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120),
                    new RewardData(30004, new int[] { 203 }, new int[] { 1 }, new int[] { 100 }, new int[1], 304, 120),
                    new RewardData(30005, new int[] { 3 }, new int[] { 1 }, new int[] { 100 }, new int[1], 304, 120),
                    new RewardData(30006, new int[] { 1, 3, 12, 6, 11, 10, 101, 110 }, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 100, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5, 5 }, 304, 120),
                    new RewardData(30007, new int[] { 7, 8, 9, 10, 13, 15, 110 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 2, 2, 2, 50, 200 }, new int[] { 0, 0, 0, 0, 0, 0, 5 }, 304, 120),
                    new RewardData(30008, new int[] { 1, 3, 4, 6, 8, 15, 104 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120),
                    new RewardData(30009, new int[] { 1, 2, 12, 7, 12, 9, 101 }, new int[] { 1, 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 5, 5, 40, 200 }, new int[] { 0, 0, 0, 0, 2, 4, 5 }, 304, 120)
                };

                foreach (var data in rewardDataList)
                {
                    if (Reward.Dic.ContainsKey(data.m_id))   // 假设有 ID 属性；若无则可用字段名如 m_id
                    {
                        Reward.Dic[data.m_id] = data;
                    }
                }
            }
        }

        public static void Postfix_GlobalBuff_Init()
        {
            if (GlobalBuff.Dic != null)
            {
                GlobalBuffData[] globalBuffDataList = new GlobalBuffData[] {
                    new GlobalBuffData(101, "巧舌如簧", 3, new int[] { 9 }, new int[][] { new int[] { 702, 100 } }, "交易时卖出道具的价值提升100%"),
                    new GlobalBuffData(102, "赠礼有方", 2, new int[] { 9 }, new int[][] { new int[] { 709, 200 } }, "送礼后所增加的好感度提升200%"),
                    new GlobalBuffData(105, "化气之术", 1, new int[] { 9, 9 }, new int[][]
                        {
                            new int[] { 707, 100 },
                            new int[] { 708, 100 }
                        }, "战斗结束后所损失的内力减少100%,损失的气血减少100%"),
                    new GlobalBuffData(106, "健步如飞", 0, new int[] { 9 }, new int[][] { new int[] { 703, 100 } }, "场景中角色移动速度增快100%"),
                    new GlobalBuffData(110, "随身盘缠", 0, new int[] { 10 }, new int[][] { new int[] { 10001, 99999990 } }, "游戏开始时获得99999990钱币"),
                    new GlobalBuffData(116, "掘地三尺", 3, new int[] { 9, 8 }, new int[][]
                        {
                            new int[] { 712, 100 },
                            new int[] { 2, 102, 712 }
                        }, "挖掘时有100%概率连续收获两次,每有2点力道,此概率就提升1%"),
                    new GlobalBuffData(119, "眼明手快", 3, new int[] { 9, 8 }, new int[][]
                        {
                            new int[] { 713, 100 },
                            new int[] { 2, 105, 713 }
                        }, "采集时有100%概率连续收获两次,每有2点技巧,此概率就提升1%"),
                    new GlobalBuffData(122, "春风拂面", 4, new int[] { 9 }, new int[][] { new int[] { 802, 50 } }, "所有NPC对你的初始好感度增加50点"),
                    new GlobalBuffData(123, "声威日隆", 1, new int[] { 9 }, new int[][] { new int[] { 803, 50 } }, "每成功完成一项掌门事务,就额外获得50点名望值"),
                    new GlobalBuffData(125, "信步闲游", 3, new int[] { 9 }, new int[][] { new int[] { 718, -100 } }, "每次在养成模式闲逛时,消耗的体力降低100%"),
                    new GlobalBuffData(126, "炉火天成", 3, new int[] { 9 }, new int[][] { new int[] { 733, 100 } }, "炼丹出丹时,丹药有100%的概率提升一个品质"),
                    new GlobalBuffData(127, "熔火禀赋", 3, new int[] { 9 }, new int[][] { new int[] { 719, 100 } }, "炼丹和打造时,有100%概率不消耗精力和体力"),
                    new GlobalBuffData(135, "稳坐钓台", 4, new int[] { 9, 9 }, new int[][]
                        {
                            new int[] { 716, 100 },
                            new int[] { 727, -100 }
                        }, "钓鱼的倒计时延长100%,且按键失败惩罚时间降低100%"),
                    new GlobalBuffData(138, "草长莺飞", 3, new int[] { 9 }, new int[][] { new int[] { 729, -500 } }, "场景中草药,矿物和野怪资源刷新速度加快500%"),
                    new GlobalBuffData(140, "玉液琼浆", 1, new int[] { 10, 10, 10 }, new int[][]
                    {
                        new int[] { 40020, 999 },
                        new int[] { 40019, 999 },
                        new int[] { 40022, 999 }
                    }, "初始获得道具：999份流霞浆,999份冰堂春,999份密云龙酒"),
                    new GlobalBuffData(141, "珍馐美馔", 1, new int[] { 10, 10, 10 }, new int[][]
                        {
                            new int[] { 40112, 999 },
                            new int[] { 40113, 999 },
                            new int[] { 40114, 999 }
                        }, "初始获得道具：999份百珍佛跳墙,999份乾坤叫花鸡,999份九转大肠"),
                    new GlobalBuffData(142, "茗香四溢", 0, new int[] { 10, 10, 10 }, new int[][]
                        {
                            new int[] { 201039, 999 },
                            new int[] { 201040, 999 },
                            new int[] { 201041, 999 }
                        }, "初始获得道具：999份建州白茶,999份临江玉津,999份瑞云翔龙"),
                    new GlobalBuffData(216, "精力充沛", 1, new int[] { 9 }, new int[][] { new int[] { 601, 500 } }, "获得额外精力上限500点"),
                    new GlobalBuffData(217, "精力丰盈", 3, new int[] { 9 }, new int[][] { new int[] { 601, 1000 } }, "获得额外精力上限1000点"),
                    new GlobalBuffData(224, "肆无忌惮", 4, new int[] { 8 }, new int[][] { new int[] { 20, 401, 4 } }, "善恶值越高,破甲越高。每拥有20点善恶值,就额外提供1点破甲"),
                    new GlobalBuffData(1005, "炉定乾坤", 4, new int[] { 9 }, new int[][] { new int[] { 706, 300 } }, "炼丹进度速率增加300%"),
                    new GlobalBuffData(1003, "垂云钓月", 4, new int[] { 9 }, new int[][] { new int[] { 727, -100 } }, "钓鱼按键失败惩罚时间降低100%")
                };

                foreach (var data in globalBuffDataList)
                    if (GlobalBuff.Dic.ContainsKey(data.m_id))
                    {
                        GlobalBuff.Dic[data.m_id] = data;
                    }
            }
        }

        public static void Postfix_Custom_Init()
        {
            if (Custom.Dic != null && Custom.Dic.ContainsKey(141))
            {
                Custom.Dic[141] = new CustomData(141, 0, -50000, 0);
            }
        }
    }
}
