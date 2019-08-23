using MySql.Data.MySqlClient.Properties;
using System;
using System.Diagnostics;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
	public sealed class MySqlTrace
	{
		private static TraceSource source;

		protected static string qaHost;

		protected static bool qaEnabled;

		public static TraceListenerCollection Listeners
		{
			get
			{
				return MySqlTrace.source.Listeners;
			}
		}

		public static SourceSwitch Switch
		{
			get
			{
				return MySqlTrace.source.Switch;
			}
			set
			{
				MySqlTrace.source.Switch = value;
			}
		}

		public static bool QueryAnalysisEnabled
		{
			get
			{
				return MySqlTrace.qaEnabled;
			}
		}

		internal static TraceSource Source
		{
			get
			{
				return MySqlTrace.source;
			}
		}

		static MySqlTrace()
		{
			MySqlTrace.source = new TraceSource("mysql");
			MySqlTrace.qaEnabled = false;
			foreach (TraceListener traceListener in MySqlTrace.source.Listeners)
			{
				if (traceListener.GetType().ToString().Contains("MySql.EMTrace.EMTraceListener"))
				{
					MySqlTrace.qaEnabled = true;
					break;
				}
			}
		}

		public static void EnableQueryAnalyzer(string host, int postInterval)
		{
			if (MySqlTrace.qaEnabled)
			{
				return;
			}
			TraceListener traceListener = (TraceListener)Activator.CreateInstance("MySql.EMTrace", "MySql.EMTrace.EMTraceListener", false, BindingFlags.CreateInstance, null, new object[]
			{
				host,
				postInterval
			}, null, null, null).Unwrap();
			if (traceListener == null)
			{
				throw new MySqlException(Resources.UnableToEnableQueryAnalysis);
			}
			MySqlTrace.source.Listeners.Add(traceListener);
			MySqlTrace.Switch.Level = SourceLevels.All;
		}

		public static void DisableQueryAnalyzer()
		{
			MySqlTrace.qaEnabled = false;
			foreach (TraceListener traceListener in MySqlTrace.source.Listeners)
			{
				if (traceListener.GetType().ToString().Contains("EMTraceListener"))
				{
					MySqlTrace.source.Listeners.Remove(traceListener);
					break;
				}
			}
		}

		internal static void LogInformation(int id, string msg)
		{
			MySqlTrace.Source.TraceEvent(TraceEventType.Information, id, msg, new object[]
			{
				MySqlTraceEventType.NonQuery,
				-1
			});
			Trace.TraceInformation(msg);
		}

		internal static void LogWarning(int id, string msg)
		{
			MySqlTrace.Source.TraceEvent(TraceEventType.Warning, id, msg, new object[]
			{
				MySqlTraceEventType.NonQuery,
				-1
			});
			Trace.TraceWarning(msg);
		}

		internal static void LogError(int id, string msg)
		{
			MySqlTrace.Source.TraceEvent(TraceEventType.Error, id, msg, new object[]
			{
				MySqlTraceEventType.NonQuery,
				-1
			});
			Trace.TraceError(msg);
		}

		internal static void TraceEvent(TraceEventType eventType, MySqlTraceEventType mysqlEventType, string msgFormat, params object[] args)
		{
			MySqlTrace.Source.TraceEvent(eventType, (int)mysqlEventType, msgFormat, args);
		}
	}
}
