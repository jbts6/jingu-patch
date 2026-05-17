using System.Collections.Generic;

namespace DBLoad
{
    public enum EffectId { }

    public class HuntBow
    {
        // ֻ������������ǩ�����õ��ĳ�Ա
        // ���� Init �����õ�ʲô�ֶΣ��ͼ�ʲô
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

    public class HuntBowData
    {
        public HuntBowData(int _id, int _speed, int _shootSpeed, int _damage)
        {
            
        }
        // Token: 0x04000CA2 RID: 3234
        public readonly int m_id;

        // Token: 0x04000CA3 RID: 3235
        public readonly int m_speed;

        // Token: 0x04000CA4 RID: 3236
        public readonly int m_shootSpeed;

        // Token: 0x04000CA5 RID: 3237
        public readonly int m_damage;
    }

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

    public class FishRodData
    {
        // Token: 0x06000DB0 RID: 3504 RVA: 0x000939F7 File Offset: 0x00091BF7
        public FishRodData(int _id, int _pull, int _time, int _speed)
        {
        }

        // Token: 0x04000C78 RID: 3192
        public readonly int m_id;

        // Token: 0x04000C79 RID: 3193
        public readonly int m_pull;

        // Token: 0x04000C7A RID: 3194
        public readonly int m_time;

        // Token: 0x04000C7B RID: 3195
        public readonly int m_speed;
    }
}
