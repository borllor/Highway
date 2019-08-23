using System;

namespace MySql.Data.MySqlClient
{
	public class MySqlSchemaRow
	{
		private object[] data;

		internal MySqlSchemaCollection Collection
		{
			get;
			private set;
		}

		internal object this[string s]
		{
			get
			{
				return this.GetValueForName(s);
			}
			set
			{
				this.SetValueForName(s, value);
			}
		}

		internal object this[int i]
		{
			get
			{
				return this.data[i];
			}
			set
			{
				this.data[i] = value;
			}
		}

		public MySqlSchemaRow(MySqlSchemaCollection c)
		{
			this.Collection = c;
			this.InitMetadata();
		}

		internal void InitMetadata()
		{
			this.data = new object[this.Collection.Mapping.Count];
		}

		internal void RemoveAt(int i)
		{
			if (i < 0 || i >= this.data.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			object[] array = new object[this.data.Length - 1];
			for (int j = 0; j < i; j++)
			{
				array[j] = this.data[j];
			}
			for (int j = i + 1; j < this.data.Length; j++)
			{
				array[j - 1] = this.data[j];
			}
			this.data = array;
		}

		private void SetValueForName(string colName, object value)
		{
			int i = this.Collection.Mapping[colName];
			this[i] = value;
		}

		private object GetValueForName(string colName)
		{
			int i = this.Collection.Mapping[colName];
			return this[i];
		}

		internal void CopyRow(MySqlSchemaRow row)
		{
			if (this.Collection.Columns.Count != row.Collection.Columns.Count)
			{
				throw new InvalidOperationException("column count doesn't match");
			}
			for (int i = 0; i < this.Collection.Columns.Count; i++)
			{
				row[i] = this[i];
			}
		}
	}
}
