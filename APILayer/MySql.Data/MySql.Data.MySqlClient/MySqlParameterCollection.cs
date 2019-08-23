using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing.Design;

namespace MySql.Data.MySqlClient
{
	[Editor("MySql.Data.MySqlClient.Design.DBParametersEditor,MySql.Design", typeof(UITypeEditor)), ListBindable(true)]
	public sealed class MySqlParameterCollection : DbParameterCollection
	{
		private List<MySqlParameter> items = new List<MySqlParameter>();

		private Dictionary<string, int> indexHashCS;

		private Dictionary<string, int> indexHashCI;

		internal bool containsUnnamedParameters;

		public override bool IsFixedSize
		{
			get
			{
				return ((IList)this.items).IsFixedSize;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return ((IList)this.items).IsReadOnly;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return ((ICollection)this.items).IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return ((ICollection)this.items).SyncRoot;
			}
		}

		public override int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public new MySqlParameter this[int index]
		{
			get
			{
				return this.InternalGetParameter(index);
			}
			set
			{
				this.InternalSetParameter(index, value);
			}
		}

		public new MySqlParameter this[string name]
		{
			get
			{
				return this.InternalGetParameter(name);
			}
			set
			{
				this.InternalSetParameter(name, value);
			}
		}

		public MySqlParameter Add(string parameterName, MySqlDbType dbType, int size, string sourceColumn)
		{
			return this.Add(new MySqlParameter(parameterName, dbType, size, sourceColumn));
		}

		public override void AddRange(Array values)
		{
			foreach (DbParameter value in values)
			{
				this.Add(value);
			}
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			return this.InternalGetParameter(parameterName);
		}

		protected override DbParameter GetParameter(int index)
		{
			return this.InternalGetParameter(index);
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			this.InternalSetParameter(parameterName, value as MySqlParameter);
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			this.InternalSetParameter(index, value as MySqlParameter);
		}

		public override int Add(object value)
		{
			MySqlParameter mySqlParameter = value as MySqlParameter;
			if (mySqlParameter == null)
			{
				throw new MySqlException("Only MySqlParameter objects may be stored");
			}
			mySqlParameter = this.Add(mySqlParameter);
			return this.IndexOf(mySqlParameter);
		}

		public override bool Contains(string parameterName)
		{
			return this.IndexOf(parameterName) != -1;
		}

		public override bool Contains(object value)
		{
			MySqlParameter mySqlParameter = value as MySqlParameter;
			if (mySqlParameter == null)
			{
				throw new ArgumentException("Argument must be of type DbParameter", "value");
			}
			return this.items.Contains(mySqlParameter);
		}

		public override void CopyTo(Array array, int index)
		{
			this.items.ToArray().CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		public override void Insert(int index, object value)
		{
			MySqlParameter mySqlParameter = value as MySqlParameter;
			if (mySqlParameter == null)
			{
				throw new MySqlException("Only MySqlParameter objects may be stored");
			}
			this.InternalAdd(mySqlParameter, index);
		}

		public override void Remove(object value)
		{
			MySqlParameter mySqlParameter = value as MySqlParameter;
			mySqlParameter.Collection = null;
			int keyIndex = this.IndexOf(mySqlParameter);
			this.items.Remove(mySqlParameter);
			this.indexHashCS.Remove(mySqlParameter.ParameterName);
			this.indexHashCI.Remove(mySqlParameter.ParameterName);
			this.AdjustHashes(keyIndex, false);
		}

		public override void RemoveAt(string parameterName)
		{
			DbParameter parameter = this.GetParameter(parameterName);
			this.Remove(parameter);
		}

		public override void RemoveAt(int index)
		{
			object value = this.items[index];
			this.Remove(value);
		}

		internal MySqlParameterCollection(MySqlCommand cmd)
		{
			this.indexHashCS = new Dictionary<string, int>();
			this.indexHashCI = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
			this.containsUnnamedParameters = false;
			this.Clear();
		}

		public MySqlParameter Add(MySqlParameter value)
		{
			return this.InternalAdd(value, -1);
		}

		[Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value)")]
		public MySqlParameter Add(string parameterName, object value)
		{
			return this.Add(new MySqlParameter(parameterName, value));
		}

		public MySqlParameter AddWithValue(string parameterName, object value)
		{
			return this.Add(new MySqlParameter(parameterName, value));
		}

		public MySqlParameter Add(string parameterName, MySqlDbType dbType)
		{
			return this.Add(new MySqlParameter(parameterName, dbType));
		}

		public MySqlParameter Add(string parameterName, MySqlDbType dbType, int size)
		{
			return this.Add(new MySqlParameter(parameterName, dbType, size));
		}

		public override void Clear()
		{
			foreach (MySqlParameter current in this.items)
			{
				current.Collection = null;
			}
			this.items.Clear();
			this.indexHashCS.Clear();
			this.indexHashCI.Clear();
		}

		private void CheckIndex(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new IndexOutOfRangeException("Parameter index is out of range.");
			}
		}

		private MySqlParameter InternalGetParameter(int index)
		{
			this.CheckIndex(index);
			return this.items[index];
		}

		private MySqlParameter InternalGetParameter(string parameterName)
		{
			int num = this.IndexOf(parameterName);
			if (num < 0)
			{
				if (parameterName.StartsWith("@", StringComparison.Ordinal) || parameterName.StartsWith("?", StringComparison.Ordinal))
				{
					string parameterName2 = parameterName.Substring(1);
					num = this.IndexOf(parameterName2);
					if (num != -1)
					{
						return this.items[num];
					}
				}
				throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
			}
			return this.items[num];
		}

		private void InternalSetParameter(string parameterName, MySqlParameter value)
		{
			int num = this.IndexOf(parameterName);
			if (num < 0)
			{
				throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
			}
			this.InternalSetParameter(num, value);
		}

		private void InternalSetParameter(int index, MySqlParameter value)
		{
			if (value == null)
			{
				throw new ArgumentException(Resources.NewValueShouldBeMySqlParameter);
			}
			this.CheckIndex(index);
			MySqlParameter mySqlParameter = this.items[index];
			this.indexHashCS.Remove(mySqlParameter.ParameterName);
			this.indexHashCI.Remove(mySqlParameter.ParameterName);
			this.items[index] = value;
			this.indexHashCS.Add(value.ParameterName, index);
			this.indexHashCI.Add(value.ParameterName, index);
		}

		public override int IndexOf(string parameterName)
		{
			int result = -1;
			if (!this.indexHashCS.TryGetValue(parameterName, out result) && !this.indexHashCI.TryGetValue(parameterName, out result))
			{
				return -1;
			}
			return result;
		}

		public override int IndexOf(object value)
		{
			MySqlParameter mySqlParameter = value as MySqlParameter;
			if (mySqlParameter == null)
			{
				throw new ArgumentException("Argument must be of type DbParameter", "value");
			}
			return this.items.IndexOf(mySqlParameter);
		}

		internal void ParameterNameChanged(MySqlParameter p, string oldName, string newName)
		{
			int value = this.IndexOf(oldName);
			this.indexHashCS.Remove(oldName);
			this.indexHashCI.Remove(oldName);
			this.indexHashCS.Add(newName, value);
			this.indexHashCI.Add(newName, value);
		}

		private MySqlParameter InternalAdd(MySqlParameter value, int index)
		{
			if (value == null)
			{
				throw new ArgumentException("The MySqlParameterCollection only accepts non-null MySqlParameter type objects.", "value");
			}
			if (string.IsNullOrEmpty(value.ParameterName))
			{
				value.ParameterName = string.Format("Parameter{0}", this.GetNextIndex());
			}
			if (this.IndexOf(value.ParameterName) >= 0)
			{
				throw new MySqlException(string.Format(Resources.ParameterAlreadyDefined, value.ParameterName));
			}
			string text = value.ParameterName;
			if (text[0] == '@' || text[0] == '?')
			{
				text = text.Substring(1, text.Length - 1);
			}
			if (this.IndexOf(text) >= 0)
			{
				throw new MySqlException(string.Format(Resources.ParameterAlreadyDefined, value.ParameterName));
			}
			if (index == -1)
			{
				this.items.Add(value);
				index = this.items.Count - 1;
			}
			else
			{
				this.items.Insert(index, value);
				this.AdjustHashes(index, true);
			}
			this.indexHashCS.Add(value.ParameterName, index);
			this.indexHashCI.Add(value.ParameterName, index);
			value.Collection = this;
			return value;
		}

		private int GetNextIndex()
		{
			int num = this.Count + 1;
			while (true)
			{
				string key = "Parameter" + num.ToString();
				if (!this.indexHashCI.ContainsKey(key))
				{
					break;
				}
				num++;
			}
			return num;
		}

		private static void AdjustHash(Dictionary<string, int> hash, string parameterName, int keyIndex, bool addEntry)
		{
			if (!hash.ContainsKey(parameterName))
			{
				return;
			}
			int num = hash[parameterName];
			if (num < keyIndex)
			{
				return;
			}
			hash[parameterName] = (addEntry ? (num + 1) : (num - 1));
		}

		private void AdjustHashes(int keyIndex, bool addEntry)
		{
			for (int i = 0; i < this.Count; i++)
			{
				string parameterName = this.items[i].ParameterName;
				MySqlParameterCollection.AdjustHash(this.indexHashCI, parameterName, keyIndex, addEntry);
				MySqlParameterCollection.AdjustHash(this.indexHashCS, parameterName, keyIndex, addEntry);
			}
		}

		private MySqlParameter GetParameterFlexibleInternal(string baseName)
		{
			int num = this.IndexOf(baseName);
			if (-1 == num)
			{
				num = this.IndexOf("?" + baseName);
			}
			if (-1 == num)
			{
				num = this.IndexOf("@" + baseName);
			}
			if (-1 != num)
			{
				return this[num];
			}
			return null;
		}

		internal MySqlParameter GetParameterFlexible(string parameterName, bool throwOnNotFound)
		{
			string baseName = parameterName;
			MySqlParameter parameterFlexibleInternal = this.GetParameterFlexibleInternal(baseName);
			if (parameterFlexibleInternal != null)
			{
				return parameterFlexibleInternal;
			}
			if (parameterName.StartsWith("@", StringComparison.Ordinal) || parameterName.StartsWith("?", StringComparison.Ordinal))
			{
				baseName = parameterName.Substring(1);
			}
			parameterFlexibleInternal = this.GetParameterFlexibleInternal(baseName);
			if (parameterFlexibleInternal != null)
			{
				return parameterFlexibleInternal;
			}
			if (throwOnNotFound)
			{
				throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
			}
			return null;
		}
	}
}
