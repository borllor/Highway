using System;

namespace MySql.Data.MySqlClient
{
	public class MySqlError
	{
		private string level;

		private int code;

		private string message;

		public string Level
		{
			get
			{
				return this.level;
			}
		}

		public int Code
		{
			get
			{
				return this.code;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public MySqlError(string level, int code, string message)
		{
			this.level = level;
			this.code = code;
			this.message = message;
		}
	}
}
