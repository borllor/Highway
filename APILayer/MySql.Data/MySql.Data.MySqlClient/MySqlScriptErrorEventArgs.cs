using System;

namespace MySql.Data.MySqlClient
{
	public class MySqlScriptErrorEventArgs : MySqlScriptEventArgs
	{
		private Exception exception;

		private bool ignore;

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public bool Ignore
		{
			get
			{
				return this.ignore;
			}
			set
			{
				this.ignore = value;
			}
		}

		public MySqlScriptErrorEventArgs(Exception exception)
		{
			this.exception = exception;
		}
	}
}
