using System;

namespace MySql.Data.MySqlClient
{
	public enum MySqlSslMode
	{
		None,
		Preferred,
		Prefered = 1,
		Required,
		VerifyCA,
		VerifyFull
	}
}
