using System;

namespace MySql.Data.MySqlClient
{
	internal class PerformanceMonitor
	{
		private MySqlConnection connection;

		public MySqlConnection Connection
		{
			get;
			private set;
		}

		public PerformanceMonitor(MySqlConnection connection)
		{
			this.Connection = connection;
		}

		public virtual void AddHardProcedureQuery()
		{
		}

		public virtual void AddSoftProcedureQuery()
		{
		}
	}
}
