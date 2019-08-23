using MySql.Data.MySqlClient;
using System;

namespace MySql.Data.Types
{
	internal struct MySqlUByte : IMySqlValue
	{
		private byte mValue;

		private bool isNull;

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
				return MySqlDbType.UByte;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public byte Value
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
				return typeof(byte);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "TINYINT";
			}
		}

		public MySqlUByte(bool isNull)
		{
			this.isNull = isNull;
			this.mValue = 0;
		}

		public MySqlUByte(byte val)
		{
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			byte b = (val is byte) ? ((byte)val) : Convert.ToByte(val);
			if (binary)
			{
				packet.WriteByte(b);
				return;
			}
			packet.WriteStringNoNull(b.ToString());
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlUByte(true);
			}
			if (length == -1L)
			{
				return new MySqlUByte(packet.ReadByte());
			}
			return new MySqlUByte(byte.Parse(packet.ReadString(length)));
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			packet.ReadByte();
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "TINY INT";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.UByte;
			mySqlSchemaRow["ColumnSize"] = 0;
			mySqlSchemaRow["CreateFormat"] = "TINYINT UNSIGNED";
			mySqlSchemaRow["CreateParameters"] = null;
			mySqlSchemaRow["DataType"] = "System.Byte";
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
