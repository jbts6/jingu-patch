namespace DBLoad
{
    public class FishReward
    {
        public int[] m_rewardId;
        public int[] m_rewardNum;
        public int m_effectId;
        public int m_effectValue;
    }

    public class FishWindow
    {
        public FishReward m_reward;
        public int m_rewardIndex;
        public float CurPull { get; set; }
        public float Time { get; set; }
        public bool m_end;
        public bool m_pause;
    }
}
