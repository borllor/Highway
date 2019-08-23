using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	internal struct MySqlInt32 : IMySqlValue
	{
		private int mValue;

		private bool isNull;

		private bool is24Bit;

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
				return MySqlDbType.Int32;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public int Value
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
				return typeof(int);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				if (!this.is24Bit)
				{
					return "INT";
				}
				return "MEDIUMINT";
			}
		}

		private MySqlInt32(MySqlDbType type)
		{
			this.is24Bit = (type == MySqlDbType.Int24);
			this.isNull = true;
			this.mValue = 0;
		}

		public MySqlInt32(MySqlDbType type, bool isNull)
		{
			this = new MySqlInt32(type);
			this.isNull = isNull;
		}

		public MySqlInt32(MySqlDbType type, int val)
		{
			this = new MySqlInt32(type);
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			int num = (val is int) ? ((int)val) : Convert.ToInt32(val);
			if (binary)
			{
				packet.WriteInteger((long)num, this.is24Bit ? 3 : 4);
				return;
			}
			packet.WriteStringNoNull(num.ToString());
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlInt32(((IMySqlValue)this).MySqlDbType, true);
			}
			if (length == -1L)
			{
				return new MySqlInt32(((IMySqlValue)this).MySqlDbType, packet.ReadInteger(4));
			}
			return new MySqlInt32(((IMySqlValue)this).MySqlDbType, int.Parse(packet.ReadString(length), CultureInfo.InvariantCulture));
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			packet.Position += 4;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			string[] array = new string[]
			{
				"INT",
				"YEAR",
				"MEDIUMINT"
			};
			MySqlDbType[] array2 = new MySqlDbType[]
			{
				MySqlDbType.Int32,
				MySqlDbType.Year,
				MySqlDbType.Int24
			};
			for (int i = 0; i < array.Length; i++)
			{
				MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
				mySqlSchemaRow["TypeName"] = array[i];
				mySqlSchemaRow["ProviderDbType"] = array2[i];
				mySqlSchemaRow["ColumnSize"] = 0;
				mySqlSchemaRow["CreateFormat"] = array[i];
				mySqlSchemaRow["CreateParameters"] = null;
				mySqlSchemaRow["DataType"] = "System.Int32";
				mySqlSchemaRow["IsAutoincrementable"] = (array2[i] != MySqlDbType.Year);
				mySqlSchemaRow["IsBestMatch"] = true;
				mySqlSchemaRow["IsCaseSensitive"] = false;
				mySqlSchemaRow["IsFixedLength"] = true;
				mySqlSchemaRow["IsFixedPrecisionScale"] = true;
				mySqlSchemaRow["IsLong"] = false;
				mySqlSchemaRow["IsNullable"] = true;
				mySqlSchemaRow["IsSearchable"] = true;
				mySqlSchemaRow["IsSearchableWithLike"] = false;
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
