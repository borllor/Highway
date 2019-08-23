using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	internal struct MySqlByte : IMySqlValue
	{
		private sbyte mValue;

		private bool isNull;

		private bool treatAsBool;

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
				return MySqlDbType.Byte;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				if (this.TreatAsBoolean)
				{
					return Convert.ToBoolean(this.mValue);
				}
				return this.mValue;
			}
		}

		public sbyte Value
		{
			get
			{
				return this.mValue;
			}
			set
			{
				this.mValue = value;
			}
		}

		Type IMySqlValue.SystemType
		{
			get
			{
				if (this.TreatAsBoolean)
				{
					return typeof(bool);
				}
				return typeof(sbyte);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "TINYINT";
			}
		}

		internal bool TreatAsBoolean
		{
			get
			{
				return this.treatAsBool;
			}
			set
			{
				this.treatAsBool = value;
			}
		}

		public MySqlByte(bool isNull)
		{
			this.isNull = isNull;
			this.mValue = 0;
			this.treatAsBool = false;
		}

		public MySqlByte(sbyte val)
		{
			this.isNull = false;
			this.mValue = val;
			this.treatAsBool = false;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			sbyte b = (val is sbyte) ? ((sbyte)val) : Convert.ToSByte(val);
			if (binary)
			{
				packet.WriteByte((byte)b);
				return;
			}
			packet.WriteStringNoNull(b.ToString());
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlByte(true);
			}
			if (length == -1L)
			{
				return new MySqlByte((sbyte)packet.ReadByte());
			}
			string s = packet.ReadString(length);
			return new MySqlByte(sbyte.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture))
			{
				TreatAsBoolean = this.TreatAsBoolean
			};
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			packet.ReadByte();
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "TINYINT";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.Byte;
			mySqlSchemaRow["ColumnSize"] = 0;
			mySqlSchemaRow["CreateFormat"] = "TINYINT";
			mySqlSchemaRow["CreateParameters"] = null;
			mySqlSchemaRow["DataType"] = "System.SByte";
			mySqlSchemaRow["IsAutoincrementable"] = true;
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
