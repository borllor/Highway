using System;

namespace MySql.Data.MySqlClient.Authentication
{
	public struct SECURITY_INTEGER
	{
		public uint LowPart;

		public int HighPart;

		public SECURITY_INTEGER(int dummy)
		{
			this.LowPart = 0u;
			this.HighPart = 0;
		}
	}
}
