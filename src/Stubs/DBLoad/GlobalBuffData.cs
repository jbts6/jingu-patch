using System;

namespace DBLoad
{
	// Token: 0x02000324 RID: 804
	public class GlobalBuffData
	{
		// Token: 0x06000DBB RID: 3515 RVA: 0x00098A87 File Offset: 0x00096C87
		public GlobalBuffData(int _id, string _name, int _quality, int[] _type, int[][] _value, string _desc)
		{
			this.m_id = _id;
			this.m_name = _name;
			this.m_quality = _quality;
			this.m_type = _type;
			this.m_value = _value;
			this.m_desc = _desc;
		}

		// Token: 0x04000C84 RID: 3204
		public readonly int m_id;

		// Token: 0x04000C85 RID: 3205
		public readonly string m_name;

		// Token: 0x04000C86 RID: 3206
		public readonly int m_quality;

		// Token: 0x04000C87 RID: 3207
		public readonly int[] m_type;

		// Token: 0x04000C88 RID: 3208
		public readonly int[][] m_value;

		// Token: 0x04000C89 RID: 3209
		public readonly string m_desc;
	}
}
