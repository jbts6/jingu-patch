using System;

namespace DBLoad
{
	// Token: 0x0200030A RID: 778
	public class CustomData
	{
		// Token: 0x06000D7A RID: 3450 RVA: 0x0007FD7B File Offset: 0x0007DF7B
		public CustomData(int _id, int _kind, int _cost, int _achieve)
		{
			this.m_id = _id;
			this.m_kind = _kind;
			this.m_cost = _cost;
			this.m_achieve = _achieve;
		}

		// Token: 0x04000C2E RID: 3118
		public readonly int m_id;

		// Token: 0x04000C2F RID: 3119
		public readonly int m_kind;

		// Token: 0x04000C30 RID: 3120
		public readonly int m_cost;

		// Token: 0x04000C31 RID: 3121
		public readonly int m_achieve;
	}
}
