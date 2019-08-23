using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	internal struct MySqlUInt32 : IMySqlValue
	{
		private uint mValue;

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
				return MySqlDbType.UInt32;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public uint Value
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
				return typeof(uint);
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

		private MySqlUInt32(MySqlDbType type)
		{
			this.is24Bit = (type == MySqlDbType.Int24);
			this.isNull = true;
			this.mValue = 0u;
		}

		public MySqlUInt32(MySqlDbType type, bool isNull)
		{
			this = new MySqlUInt32(type);
			this.isNull = isNull;
		}

		public MySqlUInt32(MySqlDbType type, uint val)
		{
			this = new MySqlUInt32(type);
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object v, int length)
		{
			uint num = (v is uint) ? ((uint)v) : Convert.ToUInt32(v);
			if (binary)
			{
				packet.WriteInteger((long)((ulong)num), this.is24Bit ? 3 : 4);
				return;
			}
			packet.WriteStringNoNull(num.ToString());
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlUInt32(((IMySqlValue)this).MySqlDbType, true);
			}
			if (length == -1L)
			{
				return new MySqlUInt32(((IMySqlValue)this).MySqlDbType, (uint)packet.ReadInteger(4));
			}
			return new MySqlUInt32(((IMySqlValue)this).MySqlDbType, uint.Parse(packet.ReadString(length), NumberStyles.Any, CultureInfo.InvariantCulture));
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			packet.Position += 4;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			string[] array = new string[]
			{
				"MEDIUMINT",
				"INT"
			};
			MySqlDbType[] array2 = new MySqlDbType[]
			{
				MySqlDbType.UInt24,
				MySqlDbType.UInt32
			};
			for (int i = 0; i < array.Length; i++)
			{
				MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
				mySqlSchemaRow["TypeName"] = array[i];
				mySqlSchemaRow["ProviderDbType"] = array2[i];
				mySqlSchemaRow["ColumnSize"] = 0;
				mySqlSchemaRow["CreateFormat"] = array[i] + " UNSIGNED";
				mySqlSchemaRow["CreateParameters"] = null;
				mySqlSchemaRow["DataType"] = "System.UInt32";
				mySqlSchemaRow["IsAutoincrementable"] = true;
				mySqlSchemaRow["IsBestMatch"] = true;
				mySqlSchemaRow["IsCaseSensitive"] = false;
				mySqlSchemaRow["IsFixedLength"] = true;
				mySqlSchemaRow["IsFixedPrecisionScale"] = true;
				mySqlSchemaRow["IsLong"] = false;
				mySqlSchemaRow["IsNullable"] = true;
				mySqlSchemaRow["IsSearchable"] = true;
				mySqlSchemaRow["IsSearchableWithLike"] = false;
				mySqlSchemaRow["IsUnsigned"] = true;
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
