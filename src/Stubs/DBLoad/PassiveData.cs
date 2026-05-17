using System;

namespace DBLoad
{
	// Token: 0x0200033A RID: 826
	public class PassiveData
	{
		// Token: 0x06000DF2 RID: 3570 RVA: 0x001C62F0 File Offset: 0x001C44F0
		public PassiveData(int _id, string _name, int _quality, int _unacted, int[] _effect, int[][] _value, string _desc, string _brief)
		{
			this.m_id = _id;
			this.m_name = _name;
			this.m_quality = _quality;
			this.m_unacted = _unacted;
			this.m_effect = _effect;
			this.m_value = _value;
			this.m_desc = _desc;
			this.m_brief = _brief;
		}

		// Token: 0x04000CD5 RID: 3285
		public readonly int m_id;

		// Token: 0x04000CD6 RID: 3286
		public readonly string m_name;

		// Token: 0x04000CD7 RID: 3287
		public readonly int m_quality;

		// Token: 0x04000CD8 RID: 3288
		public readonly int m_unacted;

		// Token: 0x04000CD9 RID: 3289
		public readonly int[] m_effect;

		// Token: 0x04000CDA RID: 3290
		public readonly int[][] m_value;

		// Token: 0x04000CDB RID: 3291
		public readonly string m_desc;

		// Token: 0x04000CDC RID: 3292
		public readonly string m_brief;
	}
}
