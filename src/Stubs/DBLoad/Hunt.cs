using System;
using System.Collections.Generic;

namespace DBLoad
{
	// Token: 0x02000329 RID: 809
	public class Hunt
	{
		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x0009AE6C File Offset: 0x0009906C
		public static Dictionary<int, HuntData> Dic
		{
			get
			{
				if (DBLoad.Hunt.m_dic == null)
				{
					DBLoad.Hunt.Init();
				}
				return DBLoad.Hunt.m_dic;
			}
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x0009AE80 File Offset: 0x00099080
		public static void Init()
		{
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x0009B55C File Offset: 0x0009975C
		public static HuntData Get(int id)
		{
			if (DBLoad.Hunt.m_dic == null)
			{
				DBLoad.Hunt.Init();
			}
			HuntData huntData;
			if (DBLoad.Hunt.m_dic.TryGetValue(id, out huntData))
			{
				return huntData;
			}
			return null;
		}

		// Token: 0x04000C98 RID: 3224
		private static Dictionary<int, HuntData> m_dic;
	}
}
