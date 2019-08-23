using System;

namespace MySql.Data.MySqlClient
{
	internal class CommandTimer : IDisposable
	{
		private bool timeoutSet;

		private MySqlConnection connection;

		public CommandTimer(MySqlConnection connection, int timeout)
		{
			this.connection = connection;
			if (connection != null)
			{
				this.timeoutSet = connection.SetCommandTimeout(timeout);
			}
		}

		public void Dispose()
		{
			if (this.timeoutSet)
			{
				this.timeoutSet = false;
				this.connection.ClearCommandTimeout();
				this.connection = null;
			}
		}
	}
}
