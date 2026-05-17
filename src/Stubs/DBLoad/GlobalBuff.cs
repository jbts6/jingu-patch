using System;
using System.Collections.Generic;

namespace DBLoad
{
	// Token: 0x02000325 RID: 805
	public class GlobalBuff
	{
		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000DBC RID: 3516 RVA: 0x00098ABC File Offset: 0x00096CBC
		public static Dictionary<int, GlobalBuffData> Dic
		{
			get
			{
				if (GlobalBuff.m_dic == null)
				{
					GlobalBuff.Init();
				}
				return GlobalBuff.m_dic;
			}
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x00098AD0 File Offset: 0x00096CD0
		public static void Init()
		{
			
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x0009AAE4 File Offset: 0x00098CE4
		public static GlobalBuffData Get(int id)
		{
			if (GlobalBuff.m_dic == null)
			{
				GlobalBuff.Init();
			}
			GlobalBuffData globalBuffData;
			if (GlobalBuff.m_dic.TryGetValue(id, out globalBuffData))
			{
				return globalBuffData;
			}
			return null;
		}

		// Token: 0x04000C8A RID: 3210
		private static Dictionary<int, GlobalBuffData> m_dic;
	}
}
