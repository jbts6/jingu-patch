using System;
using System.Collections.Generic;

namespace DBLoad
{
	// Token: 0x0200033B RID: 827
	public class Passive
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x001C6340 File Offset: 0x001C4540
		public static Dictionary<int, PassiveData> Dic
		{
			get
			{
				if (Passive.m_dic == null)
				{
					Passive.Init();
				}
				return Passive.m_dic;
			}
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x001C6354 File Offset: 0x001C4554
		public static void Init()
		{
			
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x001D1BF0 File Offset: 0x001CFDF0
		public static PassiveData Get(int id)
		{
			if (Passive.m_dic == null)
			{
				Passive.Init();
			}
			PassiveData passiveData;
			if (Passive.m_dic.TryGetValue(id, out passiveData))
			{
				return passiveData;
			}
			return null;
		}

		// Token: 0x04000CDD RID: 3293
		private static Dictionary<int, PassiveData> m_dic;
	}
}
