using MySql.Data.MySqlClient;
using System;

namespace MySql.Data.Types
{
	internal struct MySqlBit : IMySqlValue
	{
		private ulong mValue;

		private bool isNull;

		private bool readAsString;

		public bool ReadAsString
		{
			get
			{
				return this.readAsString;
			}
			set
			{
				this.readAsString = value;
			}
		}

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
				return MySqlDbType.Bit;
			}
		}

		object IMySqlValue.Value
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
				return typeof(ulong);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "BIT";
			}
		}

		public MySqlBit(bool isnull)
		{
			this.mValue = 0uL;
			this.isNull = isnull;
			this.readAsString = false;
		}

		public void WriteValue(MySqlPacket packet, bool binary, object value, int length)
		{
			ulong v = (value is ulong) ? ((ulong)value) : Convert.ToUInt64(value);
			if (binary)
			{
				packet.WriteInteger((long)v, 8);
				return;
			}
			packet.WriteStringNoNull(v.ToString());
		}

		public IMySqlValue ReadValue(MySqlPacket packet, long length, bool isNull)
		{
			this.isNull = isNull;
			if (isNull)
			{
				return this;
			}
			if (length == -1L)
			{
				length = packet.ReadFieldLength();
			}
			if (this.ReadAsString)
			{
				this.mValue = ulong.Parse(packet.ReadString(length));
			}
			else
			{
				this.mValue = packet.ReadBitValue((int)length);
			}
			return this;
		}

		public void SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadFieldLength();
			packet.Position += num;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "BIT";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.Bit;
			mySqlSchemaRow["ColumnSize"] = 64;
			mySqlSchemaRow["CreateFormat"] = "BIT";
			mySqlSchemaRow["CreateParameters"] = DBNull.Value;
			mySqlSchemaRow["DataType"] = typeof(ulong).ToString();
			mySqlSchemaRow["IsAutoincrementable"] = false;
			mySqlSchemaRow["IsBestMatch"] = true;
			mySqlSchemaRow["IsCaseSensitive"] = false;
			mySqlSchemaRow["IsFixedLength"] = false;
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
			mySqlSchemaRow["LiteralPrefix"] = DBNull.Value;
			mySqlSchemaRow["LiteralSuffix"] = DBNull.Value;
			mySqlSchemaRow["NativeDataType"] = DBNull.Value;
		}
	}
}
