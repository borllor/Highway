using MySql.Data.MySqlClient.Properties;
using System;
using System.Diagnostics;

namespace MySql.Data.MySqlClient
{
	internal class SystemPerformanceMonitor : PerformanceMonitor
	{
		private static PerformanceCounter procedureHardQueries;

		private static PerformanceCounter procedureSoftQueries;

		public SystemPerformanceMonitor(MySqlConnection connection) : base(connection)
		{
			string perfMonCategoryName = Resources.PerfMonCategoryName;
			if (connection.Settings.UsePerformanceMonitor && SystemPerformanceMonitor.procedureHardQueries == null)
			{
				try
				{
					SystemPerformanceMonitor.procedureHardQueries = new PerformanceCounter(perfMonCategoryName, "HardProcedureQueries", false);
					SystemPerformanceMonitor.procedureSoftQueries = new PerformanceCounter(perfMonCategoryName, "SoftProcedureQueries", false);
				}
				catch (Exception ex)
				{
					MySqlTrace.LogError(connection.ServerThread, ex.Message);
				}
			}
		}

		public new void AddHardProcedureQuery()
		{
			if (!base.Connection.Settings.UsePerformanceMonitor || SystemPerformanceMonitor.procedureHardQueries == null)
			{
				return;
			}
			SystemPerformanceMonitor.procedureHardQueries.Increment();
		}

		public new void AddSoftProcedureQuery()
		{
			if (!base.Connection.Settings.UsePerformanceMonitor || SystemPerformanceMonitor.procedureSoftQueries == null)
			{
				return;
			}
			SystemPerformanceMonitor.procedureSoftQueries.Increment();
		}
	}
}
