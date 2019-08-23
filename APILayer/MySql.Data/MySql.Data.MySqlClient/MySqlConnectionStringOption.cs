using MySql.Data.MySqlClient.Properties;
using System;

namespace MySql.Data.MySqlClient
{
	internal class MySqlConnectionStringOption
	{
		public delegate void SetterDelegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value);

		public delegate object GetterDelegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender);

		public string[] Synonyms
		{
			get;
			private set;
		}

		public bool Obsolete
		{
			get;
			private set;
		}

		public Type BaseType
		{
			get;
			private set;
		}

		public string Keyword
		{
			get;
			private set;
		}

		public object DefaultValue
		{
			get;
			private set;
		}

		public MySqlConnectionStringOption.SetterDelegate Setter
		{
			get;
			private set;
		}

		public MySqlConnectionStringOption.GetterDelegate Getter
		{
			get;
			private set;
		}

		public MySqlConnectionStringOption(string keyword, string synonyms, Type baseType, object defaultValue, bool obsolete, MySqlConnectionStringOption.SetterDelegate setter, MySqlConnectionStringOption.GetterDelegate getter)
		{
			this.Keyword = StringUtility.ToLowerInvariant(keyword);
			if (synonyms != null)
			{
				this.Synonyms = StringUtility.ToLowerInvariant(synonyms).Split(new char[]
				{
					','
				});
			}
			this.BaseType = baseType;
			this.Obsolete = obsolete;
			this.DefaultValue = defaultValue;
			this.Setter = setter;
			this.Getter = getter;
		}

		public MySqlConnectionStringOption(string keyword, string synonyms, Type baseType, object defaultValue, bool obsolete) : this(keyword, synonyms, baseType, defaultValue, obsolete, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
		{
			sender.ValidateValue(ref value);
			msb.SetValue(sender.Keyword, Convert.ChangeType(value, sender.BaseType));
		}, (MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender) => msb.values[sender.Keyword])
		{
		}

		public bool HasKeyword(string key)
		{
			if (this.Keyword == key)
			{
				return true;
			}
			if (this.Synonyms == null)
			{
				return false;
			}
			string[] synonyms = this.Synonyms;
			for (int i = 0; i < synonyms.Length; i++)
			{
				string a = synonyms[i];
				if (a == key)
				{
					return true;
				}
			}
			return false;
		}

		public void Clean(MySqlConnectionStringBuilder builder)
		{
			builder.Remove(this.Keyword);
			if (this.Synonyms == null)
			{
				return;
			}
			string[] synonyms = this.Synonyms;
			for (int i = 0; i < synonyms.Length; i++)
			{
				string keyword = synonyms[i];
				builder.Remove(keyword);
			}
		}

		public void ValidateValue(ref object value)
		{
			if (value == null)
			{
				return;
			}
			string name = this.BaseType.Name;
			Type type = value.GetType();
			bool flag;
			if (type.Name == "String")
			{
				if (this.BaseType == type)
				{
					return;
				}
				if (this.BaseType == typeof(bool))
				{
					if (string.Compare("yes", (string)value, StringComparison.OrdinalIgnoreCase) == 0)
					{
						value = true;
						return;
					}
					if (string.Compare("no", (string)value, StringComparison.OrdinalIgnoreCase) == 0)
					{
						value = false;
						return;
					}
					if (bool.TryParse(value.ToString(), out flag))
					{
						value = flag;
						return;
					}
					throw new ArgumentException(string.Format(Resources.ValueNotCorrectType, value));
				}
			}
			if (name == "Boolean" && bool.TryParse(value.ToString(), out flag))
			{
				value = flag;
				return;
			}
			ulong num;
			if (name.StartsWith("UInt64") && ulong.TryParse(value.ToString(), out num))
			{
				value = num;
				return;
			}
			uint num2;
			if (name.StartsWith("UInt32") && uint.TryParse(value.ToString(), out num2))
			{
				value = num2;
				return;
			}
			long num3;
			if (name.StartsWith("Int64") && long.TryParse(value.ToString(), out num3))
			{
				value = num3;
				return;
			}
			int num4;
			if (name.StartsWith("Int32") && int.TryParse(value.ToString(), out num4))
			{
				value = num4;
				return;
			}
			Type baseType = this.BaseType.BaseType;
			object obj;
			if (baseType != null && baseType.Name == "Enum" && this.ParseEnum(value.ToString(), out obj))
			{
				value = obj;
				return;
			}
			throw new ArgumentException(string.Format(Resources.ValueNotCorrectType, value));
		}

		private bool ParseEnum(string requestedValue, out object value)
		{
			value = null;
			bool result;
			try
			{
				value = Enum.Parse(this.BaseType, requestedValue, true);
				result = true;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}
	}
}
