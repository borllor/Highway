using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace MySql.Data.MySqlClient
{
	internal class ResultSet
	{
		private Driver driver;

		private bool hasRows;

		private bool[] uaFieldsUsed;

		private MySqlField[] fields;

		private IMySqlValue[] values;

		private Dictionary<string, int> fieldHashCS;

		private Dictionary<string, int> fieldHashCI;

		private int rowIndex;

		private bool readDone;

		private bool isSequential;

		private int seqIndex;

		private bool isOutputParameters;

		private int affectedRows;

		private long insertedId;

		private int statementId;

		private int totalRows;

		private int skippedRows;

		private bool cached;

		private List<IMySqlValue[]> cachedValues;

		public bool HasRows
		{
			get
			{
				return this.hasRows;
			}
		}

		public int Size
		{
			get
			{
				if (this.fields != null)
				{
					return this.fields.Length;
				}
				return 0;
			}
		}

		public MySqlField[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		public IMySqlValue[] Values
		{
			get
			{
				return this.values;
			}
		}

		public bool IsOutputParameters
		{
			get
			{
				return this.isOutputParameters;
			}
			set
			{
				this.isOutputParameters = value;
			}
		}

		public int AffectedRows
		{
			get
			{
				return this.affectedRows;
			}
		}

		public long InsertedId
		{
			get
			{
				return this.insertedId;
			}
		}

		public int TotalRows
		{
			get
			{
				return this.totalRows;
			}
		}

		public int SkippedRows
		{
			get
			{
				return this.skippedRows;
			}
		}

		public bool Cached
		{
			get
			{
				return this.cached;
			}
			set
			{
				this.cached = value;
				if (this.cached && this.cachedValues == null)
				{
					this.cachedValues = new List<IMySqlValue[]>();
				}
			}
		}

		public IMySqlValue this[int index]
		{
			get
			{
				if (this.rowIndex < 0)
				{
					throw new MySqlException(Resources.AttemptToAccessBeforeRead);
				}
				this.uaFieldsUsed[index] = true;
				if (this.isSequential && index != this.seqIndex)
				{
					if (index < this.seqIndex)
					{
						throw new MySqlException(Resources.ReadingPriorColumnUsingSeqAccess);
					}
					while (this.seqIndex < index - 1)
					{
						this.driver.SkipColumnValue(this.values[++this.seqIndex]);
					}
					this.values[index] = this.driver.ReadColumnValue(index, this.fields[index], this.values[index]);
					this.seqIndex = index;
				}
				return this.values[index];
			}
		}

		public ResultSet(int affectedRows, long insertedId)
		{
			this.affectedRows = affectedRows;
			this.insertedId = insertedId;
			this.readDone = true;
		}

		public ResultSet(Driver d, int statementId, int numCols)
		{
			this.affectedRows = -1;
			this.insertedId = -1L;
			this.driver = d;
			this.statementId = statementId;
			this.rowIndex = -1;
			this.LoadColumns(numCols);
			this.isOutputParameters = this.IsOutputParameterResultSet();
			this.hasRows = this.GetNextRow();
			this.readDone = !this.hasRows;
		}

		public int GetOrdinal(string name)
		{
			int result;
			if (this.fieldHashCS.TryGetValue(name, out result))
			{
				return result;
			}
			if (this.fieldHashCI.TryGetValue(name, out result))
			{
				return result;
			}
			throw new IndexOutOfRangeException(string.Format(Resources.CouldNotFindColumnName, name));
		}

		private bool GetNextRow()
		{
			bool flag = this.driver.FetchDataRow(this.statementId, this.Size);
			if (flag)
			{
				this.totalRows++;
			}
			return flag;
		}

		public bool NextRow(CommandBehavior behavior)
		{
			if (this.readDone)
			{
				return this.Cached && this.CachedNextRow(behavior);
			}
			if ((behavior & CommandBehavior.SingleRow) != CommandBehavior.Default && this.rowIndex == 0)
			{
				return false;
			}
			this.isSequential = ((behavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default);
			this.seqIndex = -1;
			if (this.rowIndex >= 0)
			{
				bool flag = false;
				try
				{
					flag = this.GetNextRow();
				}
				catch (MySqlException ex)
				{
					if (ex.IsQueryAborted)
					{
						this.readDone = true;
					}
					throw;
				}
				if (!flag)
				{
					this.readDone = true;
					return false;
				}
			}
			if (!this.isSequential)
			{
				this.ReadColumnData(false);
			}
			this.rowIndex++;
			return true;
		}

		private bool CachedNextRow(CommandBehavior behavior)
		{
			if ((behavior & CommandBehavior.SingleRow) != CommandBehavior.Default && this.rowIndex == 0)
			{
				return false;
			}
			if (this.rowIndex == this.totalRows - 1)
			{
				return false;
			}
			this.rowIndex++;
			this.values = this.cachedValues[this.rowIndex];
			return true;
		}

		public void Close()
		{
			if (!this.readDone)
			{
				if (this.HasRows && this.rowIndex == -1)
				{
					this.skippedRows++;
				}
				try
				{
					while (this.driver.IsOpen && this.driver.SkipDataRow())
					{
						this.totalRows++;
						this.skippedRows++;
					}
				}
				catch (IOException)
				{
				}
				this.readDone = true;
			}
			else if (this.driver == null)
			{
				this.CacheClose();
			}
			this.driver = null;
			if (this.Cached)
			{
				this.CacheReset();
			}
		}

		private void CacheClose()
		{
			this.skippedRows = this.totalRows - this.rowIndex - 1;
		}

		private void CacheReset()
		{
			if (!this.Cached)
			{
				return;
			}
			this.rowIndex = -1;
			this.affectedRows = -1;
			this.insertedId = -1L;
			this.skippedRows = 0;
		}

		public bool FieldRead(int index)
		{
			return this.uaFieldsUsed[index];
		}

		public void SetValueObject(int i, IMySqlValue valueObject)
		{
			this.values[i] = valueObject;
		}

		private bool IsOutputParameterResultSet()
		{
			if (this.driver.HasStatus(ServerStatusFlags.OutputParameters))
			{
				return true;
			}
			if (this.fields.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (!this.fields[i].ColumnName.StartsWith("@_cnet_param_", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		private void LoadColumns(int numCols)
		{
			this.fields = this.driver.GetColumns(numCols);
			this.values = new IMySqlValue[numCols];
			this.uaFieldsUsed = new bool[numCols];
			this.fieldHashCS = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			this.fieldHashCI = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < this.fields.Length; i++)
			{
				string columnName = this.fields[i].ColumnName;
				if (!this.fieldHashCS.ContainsKey(columnName))
				{
					this.fieldHashCS.Add(columnName, i);
				}
				if (!this.fieldHashCI.ContainsKey(columnName))
				{
					this.fieldHashCI.Add(columnName, i);
				}
				this.values[i] = this.fields[i].GetValueObject();
			}
		}

		private void ReadColumnData(bool outputParms)
		{
			for (int i = 0; i < this.Size; i++)
			{
				this.values[i] = this.driver.ReadColumnValue(i, this.fields[i], this.values[i]);
			}
			if (this.Cached)
			{
				this.cachedValues.Add((IMySqlValue[])this.values.Clone());
			}
			if (outputParms)
			{
				bool flag = this.driver.FetchDataRow(this.statementId, this.fields.Length);
				this.rowIndex = 0;
				if (flag)
				{
					throw new MySqlException(Resources.MoreThanOneOPRow);
				}
			}
		}
	}
}
