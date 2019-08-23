using MySql.Data.MySqlClient;
using System;

namespace MySql.Data.Types
{
	internal struct MySqlString : IMySqlValue
	{
		private string mValue;

		private bool isNull;

		private MySqlDbType type;

		public bool IsNull
		{
			get
			{
				return this.isNull;
			}
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get
			{
				return this.type;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public string Value
		{
			get
			{
				return this.mValue;
			}
		}

		Type IMySqlValue.SystemType
		{
			get
			{
				return typeof(string);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				if (this.type == MySqlDbType.Set)
				{
					return "SET";
				}
				if (this.type != MySqlDbType.Enum)
				{
					return "VARCHAR";
				}
				return "ENUM";
			}
		}

		public MySqlString(MySqlDbType type, bool isNull)
		{
			this.type = type;
			this.isNull = isNull;
			this.mValue = string.Empty;
		}

		public MySqlString(MySqlDbType type, string val)
		{
			this.type = type;
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			string text = val.ToString();
			if (length > 0)
			{
				length = Math.Min(length, text.Length);
				text = text.Substring(0, length);
			}
			if (binary)
			{
				packet.WriteLenString(text);
				return;
			}
			packet.WriteStringNoNull("'" + MySqlHelper.EscapeString(text) + "'");
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlString(this.type, true);
			}
			string val = string.Empty;
			if (length == -1L)
			{
				val = packet.ReadLenString();
			}
			else
			{
				val = packet.ReadString(length);
			}
			MySqlString mySqlString = new MySqlString(this.type, val);
			return mySqlString;
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadFieldLength();
			packet.Position += num;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			string[] array = new string[]
			{
				"CHAR",
				"NCHAR",
				"VARCHAR",
				"NVARCHAR",
				"SET",
				"ENUM",
				"TINYTEXT",
				"TEXT",
				"MEDIUMTEXT",
				"LONGTEXT"
			};
			MySqlDbType[] array2 = new MySqlDbType[]
			{
				MySqlDbType.String,
				MySqlDbType.String,
				MySqlDbType.VarChar,
				MySqlDbType.VarChar,
				MySqlDbType.Set,
				MySqlDbType.Enum,
				MySqlDbType.TinyText,
				MySqlDbType.Text,
				MySqlDbType.MediumText,
				MySqlDbType.LongText
			};
			for (int i = 0; i < array.Length; i++)
			{
				MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
				mySqlSchemaRow["TypeName"] = array[i];
				mySqlSchemaRow["ProviderDbType"] = array2[i];
				mySqlSchemaRow["ColumnSize"] = 0;
				mySqlSchemaRow["CreateFormat"] = ((i < 4) ? (array[i] + "({0})") : array[i]);
				mySqlSchemaRow["CreateParameters"] = ((i < 4) ? "size" : null);
				mySqlSchemaRow["DataType"] = "System.String";
				mySqlSchemaRow["IsAutoincrementable"] = false;
				mySqlSchemaRow["IsBestMatch"] = true;
				mySqlSchemaRow["IsCaseSensitive"] = false;
				mySqlSchemaRow["IsFixedLength"] = false;
				mySqlSchemaRow["IsFixedPrecisionScale"] = true;
				mySqlSchemaRow["IsLong"] = false;
				mySqlSchemaRow["IsNullable"] = true;
				mySqlSchemaRow["IsSearchable"] = true;
				mySqlSchemaRow["IsSearchableWithLike"] = true;
				mySqlSchemaRow["IsUnsigned"] = false;
				mySqlSchemaRow["MaximumScale"] = 0;
				mySqlSchemaRow["MinimumScale"] = 0;
				mySqlSchemaRow["IsConcurrencyType"] = DBNull.Value;
				mySqlSchemaRow["IsLiteralSupported"] = false;
				mySqlSchemaRow["LiteralPrefix"] = null;
				mySqlSchemaRow["LiteralSuffix"] = null;
				mySqlSchemaRow["NativeDataType"] = null;
			}
		}
	}
}
