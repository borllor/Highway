using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MySql.Data.MySqlClient
{
	internal class ProcedureCache
	{
		private Dictionary<int, ProcedureCacheEntry> procHash;

		private Queue<int> hashQueue;

		private int maxSize;

		public ProcedureCache(int size)
		{
			this.maxSize = size;
			this.hashQueue = new Queue<int>(this.maxSize);
			this.procHash = new Dictionary<int, ProcedureCacheEntry>(this.maxSize);
		}

		public ProcedureCacheEntry GetProcedure(MySqlConnection conn, string spName, string cacheKey)
		{
			ProcedureCacheEntry procedureCacheEntry = null;
			if (cacheKey != null)
			{
				int hashCode = cacheKey.GetHashCode();
				lock (this.procHash)
				{
					this.procHash.TryGetValue(hashCode, out procedureCacheEntry);
				}
			}
			if (procedureCacheEntry == null)
			{
				procedureCacheEntry = this.AddNew(conn, spName);
				conn.PerfMonitor.AddHardProcedureQuery();
				if (conn.Settings.Logging)
				{
					MySqlTrace.LogInformation(conn.ServerThread, string.Format(Resources.HardProcQuery, spName));
				}
			}
			else
			{
				conn.PerfMonitor.AddSoftProcedureQuery();
				if (conn.Settings.Logging)
				{
					MySqlTrace.LogInformation(conn.ServerThread, string.Format(Resources.SoftProcQuery, spName));
				}
			}
			return procedureCacheEntry;
		}

		internal string GetCacheKey(string spName, ProcedureCacheEntry proc)
		{
			string str = string.Empty;
			StringBuilder stringBuilder = new StringBuilder(spName);
			stringBuilder.Append("(");
			string text = "";
			if (proc.parameters != null)
			{
				foreach (MySqlSchemaRow current in proc.parameters.Rows)
				{
					if (current["ORDINAL_POSITION"].Equals(0))
					{
						str = "?=";
					}
					else
					{
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}?", new object[]
						{
							text
						});
						text = ",";
					}
				}
			}
			stringBuilder.Append(")");
			return str + stringBuilder.ToString();
		}

		private ProcedureCacheEntry AddNew(MySqlConnection connection, string spName)
		{
			ProcedureCacheEntry procData = ProcedureCache.GetProcData(connection, spName);
			if (this.maxSize > 0)
			{
				string cacheKey = this.GetCacheKey(spName, procData);
				int hashCode = cacheKey.GetHashCode();
				lock (this.procHash)
				{
					if (this.procHash.Keys.Count >= this.maxSize)
					{
						this.TrimHash();
					}
					if (!this.procHash.ContainsKey(hashCode))
					{
						this.procHash[hashCode] = procData;
						this.hashQueue.Enqueue(hashCode);
					}
				}
			}
			return procData;
		}

		private void TrimHash()
		{
			int key = this.hashQueue.Dequeue();
			this.procHash.Remove(key);
		}

		private static ProcedureCacheEntry GetProcData(MySqlConnection connection, string spName)
		{
			string text = string.Empty;
			string text2 = spName;
			int num = spName.IndexOf(".");
			if (num != -1)
			{
				text = spName.Substring(0, num);
				text2 = spName.Substring(num + 1, spName.Length - num - 1);
			}
			string[] array = new string[4];
			array[1] = ((text.Length > 0) ? text : connection.CurrentDatabase());
			array[2] = text2;
			MySqlSchemaCollection schemaCollection = connection.GetSchemaCollection("procedures", array);
			if (schemaCollection.Rows.Count > 1)
			{
				throw new MySqlException(Resources.ProcAndFuncSameName);
			}
			if (schemaCollection.Rows.Count == 0)
			{
				throw new MySqlException(string.Format(Resources.InvalidProcName, text2, text));
			}
			ProcedureCacheEntry procedureCacheEntry = new ProcedureCacheEntry();
			procedureCacheEntry.procedure = schemaCollection;
			ISSchemaProvider iSSchemaProvider = new ISSchemaProvider(connection);
			string[] restrictions = iSSchemaProvider.CleanRestrictions(array);
			MySqlSchemaCollection procedureParameters = iSSchemaProvider.GetProcedureParameters(restrictions, schemaCollection);
			procedureCacheEntry.parameters = procedureParameters;
			return procedureCacheEntry;
		}
	}
}
