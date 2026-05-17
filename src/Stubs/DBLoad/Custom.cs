using System;
using System.Collections.Generic;

namespace DBLoad
{
	// Token: 0x0200030B RID: 779
	public class Custom
	{
		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000D7B RID: 3451 RVA: 0x0007FDA0 File Offset: 0x0007DFA0
		public static Dictionary<int, CustomData> Dic
		{
			get
			{
				if (Custom.m_dic == null)
				{
					Custom.Init();
				}
				return Custom.m_dic;
			}
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0007FDB4 File Offset: 0x0007DFB4
		public static void Init()
		{
			
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00080B4C File Offset: 0x0007ED4C
		public static CustomData Get(int id)
		{
			if (Custom.m_dic == null)
			{
				Custom.Init();
			}
			CustomData customData;
			if (Custom.m_dic.TryGetValue(id, out customData))
			{
				return customData;
			}
			return null;
		}

		// Token: 0x04000C32 RID: 3122
		private static Dictionary<int, CustomData> m_dic;
	}
}
