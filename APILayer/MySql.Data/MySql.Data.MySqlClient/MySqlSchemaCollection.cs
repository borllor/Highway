using System;
using System.Collections.Generic;
using System.Data;

namespace MySql.Data.MySqlClient
{
	public class MySqlSchemaCollection
	{
		private List<SchemaColumn> columns = new List<SchemaColumn>();

		private List<MySqlSchemaRow> rows = new List<MySqlSchemaRow>();

		private DataTable _table;

		internal Dictionary<string, int> Mapping;

		public string Name
		{
			get;
			set;
		}

		public IList<SchemaColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public IList<MySqlSchemaRow> Rows
		{
			get
			{
				return this.rows;
			}
		}

		public MySqlSchemaCollection()
		{
			this.Mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		}

		public MySqlSchemaCollection(string name) : this()
		{
			this.Name = name;
		}

		public MySqlSchemaCollection(DataTable dt) : this()
		{
			this._table = dt;
			int i = 0;
			foreach (DataColumn dataColumn in dt.Columns)
			{
				this.columns.Add(new SchemaColumn
				{
					Name = dataColumn.ColumnName,
					Type = dataColumn.DataType
				});
				this.Mapping.Add(dataColumn.ColumnName, i++);
			}
			foreach (DataRow dataRow in dt.Rows)
			{
				MySqlSchemaRow mySqlSchemaRow = new MySqlSchemaRow(this);
				for (i = 0; i < this.columns.Count; i++)
				{
					mySqlSchemaRow[i] = dataRow[i];
				}
				this.rows.Add(mySqlSchemaRow);
			}
		}

		internal SchemaColumn AddColumn(string name, Type t)
		{
			SchemaColumn schemaColumn = new SchemaColumn();
			schemaColumn.Name = name;
			schemaColumn.Type = t;
			this.columns.Add(schemaColumn);
			this.Mapping.Add(name, this.columns.Count - 1);
			return schemaColumn;
		}

		internal int ColumnIndex(string name)
		{
			int result = -1;
			for (int i = 0; i < this.columns.Count; i++)
			{
				SchemaColumn schemaColumn = this.columns[i];
				if (string.Compare(schemaColumn.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		internal void RemoveColumn(string name)
		{
			int num = this.ColumnIndex(name);
			if (num == -1)
			{
				throw new InvalidOperationException();
			}
			this.columns.RemoveAt(num);
			foreach (MySqlSchemaRow current in this.rows)
			{
				current.RemoveAt(num);
			}
		}

		internal bool ContainsColumn(string name)
		{
			return this.ColumnIndex(name) >= 0;
		}

		internal MySqlSchemaRow AddRow()
		{
			MySqlSchemaRow mySqlSchemaRow = new MySqlSchemaRow(this);
			this.rows.Add(mySqlSchemaRow);
			return mySqlSchemaRow;
		}

		internal MySqlSchemaRow NewRow()
		{
			return new MySqlSchemaRow(this);
		}

		internal DataTable AsDataTable()
		{
			if (this._table != null)
			{
				return this._table;
			}
			DataTable dataTable = new DataTable(this.Name);
			foreach (SchemaColumn current in this.Columns)
			{
				dataTable.Columns.Add(current.Name, current.Type);
			}
			foreach (MySqlSchemaRow current2 in this.Rows)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					dataRow[i] = ((current2[i] == null) ? DBNull.Value : current2[i]);
				}
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}
	}
}
