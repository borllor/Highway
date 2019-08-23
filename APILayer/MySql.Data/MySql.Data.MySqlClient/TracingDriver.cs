using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MySql.Data.MySqlClient
{
	internal class TracingDriver : Driver
	{
		private static long driverCounter;

		private long driverId;

		private ResultSet activeResult;

		private int rowSizeInBytes;

		public TracingDriver(MySqlConnectionStringBuilder settings) : base(settings)
		{
			this.driverId = Interlocked.Increment(ref TracingDriver.driverCounter);
		}

		public override void Open()
		{
			base.Open();
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.ConnectionOpened, Resources.TraceOpenConnection, new object[]
			{
				this.driverId,
				base.Settings.ConnectionString,
				base.ThreadID
			});
		}

		public override void Close()
		{
			base.Close();
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.ConnectionClosed, Resources.TraceCloseConnection, new object[]
			{
				this.driverId
			});
		}

		public override void SendQuery(MySqlPacket p)
		{
			this.rowSizeInBytes = 0;
			string text = base.Encoding.GetString(p.Buffer, 5, p.Length - 5);
			string text2 = null;
			if (text.Length > 300)
			{
				QueryNormalizer queryNormalizer = new QueryNormalizer();
				text2 = queryNormalizer.Normalize(text);
				text = text.Substring(0, 300);
			}
			base.SendQuery(p);
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.QueryOpened, Resources.TraceQueryOpened, new object[]
			{
				this.driverId,
				base.ThreadID,
				text
			});
			if (text2 != null)
			{
				MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.QueryNormalized, Resources.TraceQueryNormalized, new object[]
				{
					this.driverId,
					base.ThreadID,
					text2
				});
			}
		}

		protected override int GetResult(int statementId, ref int affectedRows, ref long insertedId)
		{
			int result2;
			try
			{
				int result = base.GetResult(statementId, ref affectedRows, ref insertedId);
				MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.ResultOpened, Resources.TraceResult, new object[]
				{
					this.driverId,
					result,
					affectedRows,
					insertedId
				});
				result2 = result;
			}
			catch (MySqlException ex)
			{
				MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.Error, Resources.TraceOpenResultError, new object[]
				{
					this.driverId,
					ex.Number,
					ex.Message
				});
				throw ex;
			}
			return result2;
		}

		public override ResultSet NextResult(int statementId, bool force)
		{
			if (this.activeResult != null)
			{
				if (base.Settings.UseUsageAdvisor)
				{
					this.ReportUsageAdvisorWarnings(statementId, this.activeResult);
				}
				MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.ResultClosed, Resources.TraceResultClosed, new object[]
				{
					this.driverId,
					this.activeResult.TotalRows,
					this.activeResult.SkippedRows,
					this.rowSizeInBytes
				});
				this.rowSizeInBytes = 0;
				this.activeResult = null;
			}
			this.activeResult = base.NextResult(statementId, force);
			return this.activeResult;
		}

		public override int PrepareStatement(string sql, ref MySqlField[] parameters)
		{
			int num = base.PrepareStatement(sql, ref parameters);
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.StatementPrepared, Resources.TraceStatementPrepared, new object[]
			{
				this.driverId,
				sql,
				num
			});
			return num;
		}

		public override void CloseStatement(int id)
		{
			base.CloseStatement(id);
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.StatementClosed, Resources.TraceStatementClosed, new object[]
			{
				this.driverId,
				id
			});
		}

		public override void SetDatabase(string dbName)
		{
			base.SetDatabase(dbName);
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.NonQuery, Resources.TraceSetDatabase, new object[]
			{
				this.driverId,
				dbName
			});
		}

		public override void ExecuteStatement(MySqlPacket packetToExecute)
		{
			base.ExecuteStatement(packetToExecute);
			int position = packetToExecute.Position;
			packetToExecute.Position = 1;
			int num = packetToExecute.ReadInteger(4);
			packetToExecute.Position = position;
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.StatementExecuted, Resources.TraceStatementExecuted, new object[]
			{
				this.driverId,
				num,
				base.ThreadID
			});
		}

		public override bool FetchDataRow(int statementId, int columns)
		{
			bool result;
			try
			{
				bool flag = base.FetchDataRow(statementId, columns);
				if (flag)
				{
					this.rowSizeInBytes += (this.handler as NativeDriver).Packet.Length;
				}
				result = flag;
			}
			catch (MySqlException ex)
			{
				MySqlTrace.TraceEvent(TraceEventType.Error, MySqlTraceEventType.Error, Resources.TraceFetchError, new object[]
				{
					this.driverId,
					ex.Number,
					ex.Message
				});
				throw ex;
			}
			return result;
		}

		public override void CloseQuery(MySqlConnection connection, int statementId)
		{
			base.CloseQuery(connection, statementId);
			MySqlTrace.TraceEvent(TraceEventType.Information, MySqlTraceEventType.QueryClosed, Resources.TraceQueryDone, new object[]
			{
				this.driverId
			});
		}

		public override List<MySqlError> ReportWarnings(MySqlConnection connection)
		{
			List<MySqlError> list = base.ReportWarnings(connection);
			foreach (MySqlError current in list)
			{
				MySqlTrace.TraceEvent(TraceEventType.Warning, MySqlTraceEventType.Warning, Resources.TraceWarning, new object[]
				{
					this.driverId,
					current.Level,
					current.Code,
					current.Message
				});
			}
			return list;
		}

		private bool AllFieldsAccessed(ResultSet rs)
		{
			if (rs.Fields == null || rs.Fields.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < rs.Fields.Length; i++)
			{
				if (!rs.FieldRead(i))
				{
					return false;
				}
			}
			return true;
		}

		private void ReportUsageAdvisorWarnings(int statementId, ResultSet rs)
		{
			if (!base.Settings.UseUsageAdvisor)
			{
				return;
			}
			if (base.HasStatus(ServerStatusFlags.NoIndex))
			{
				MySqlTrace.TraceEvent(TraceEventType.Warning, MySqlTraceEventType.UsageAdvisorWarning, Resources.TraceUAWarningNoIndex, new object[]
				{
					this.driverId,
					UsageAdvisorWarningFlags.NoIndex
				});
			}
			else if (base.HasStatus(ServerStatusFlags.BadIndex))
			{
				MySqlTrace.TraceEvent(TraceEventType.Warning, MySqlTraceEventType.UsageAdvisorWarning, Resources.TraceUAWarningBadIndex, new object[]
				{
					this.driverId,
					UsageAdvisorWarningFlags.BadIndex
				});
			}
			if (rs.SkippedRows > 0)
			{
				MySqlTrace.TraceEvent(TraceEventType.Warning, MySqlTraceEventType.UsageAdvisorWarning, Resources.TraceUAWarningSkippedRows, new object[]
				{
					this.driverId,
					UsageAdvisorWarningFlags.SkippedRows,
					rs.SkippedRows
				});
			}
			if (!this.AllFieldsAccessed(rs))
			{
				StringBuilder stringBuilder = new StringBuilder("");
				string arg = "";
				for (int i = 0; i < rs.Size; i++)
				{
					if (!rs.FieldRead(i))
					{
						stringBuilder.AppendFormat("{0}{1}", arg, rs.Fields[i].ColumnName);
						arg = ",";
					}
				}
				MySqlTrace.TraceEvent(TraceEventType.Warning, MySqlTraceEventType.UsageAdvisorWarning, Resources.TraceUAWarningSkippedColumns, new object[]
				{
					this.driverId,
					UsageAdvisorWarningFlags.SkippedColumns,
					stringBuilder.ToString()
				});
			}
			if (rs.Fields != null)
			{
				MySqlField[] fields = rs.Fields;
				for (int j = 0; j < fields.Length; j++)
				{
					MySqlField mySqlField = fields[j];
					StringBuilder stringBuilder2 = new StringBuilder();
					string arg2 = "";
					foreach (Type current in mySqlField.TypeConversions)
					{
						stringBuilder2.AppendFormat("{0}{1}", arg2, current.Name);
						arg2 = ",";
					}
					if (stringBuilder2.Length > 0)
					{
						MySqlTrace.TraceEvent(TraceEventType.Warning, MySqlTraceEventType.UsageAdvisorWarning, Resources.TraceUAWarningFieldConversion, new object[]
						{
							this.driverId,
							UsageAdvisorWarningFlags.FieldConversion,
							mySqlField.ColumnName,
							stringBuilder2.ToString()
						});
					}
				}
			}
		}
	}
}
