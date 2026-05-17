namespace DBLoad
{
    public class RewardData
    {

        public RewardData(int _id, int[] _rewardId, int[] _rewardNum, int[] _rewardValue, int[] _lv, int _effectId, int _effectValue)
        {
            this.m_id = _id;
            this.m_rewardId = _rewardId;
            this.m_rewardNum = _rewardNum;
            this.m_rewardValue = _rewardValue;
            this.m_lv = _lv;
            this.m_effectId = _effectId;
            this.m_effectValue = _effectValue;
        }
        public int m_id;
        public int[] m_rewardId;
        public int[] m_rewardNum;
        public int[] m_rewardValue;
        public int[] m_lv;
        public int m_effectId;
        public int m_effectValue;
    }
}
