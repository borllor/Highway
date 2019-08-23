using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	public struct MySqlDecimal : IMySqlValue
	{
		private byte precision;

		private byte scale;

		private string mValue;

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
				return MySqlDbType.Decimal;
			}
		}

		public byte Precision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
			}
		}

		public byte Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.Value;
			}
		}

		public decimal Value
		{
			get
			{
				return Convert.ToDecimal(this.mValue, CultureInfo.InvariantCulture);
			}
		}

		Type IMySqlValue.SystemType
		{
			get
			{
				return typeof(decimal);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "DECIMAL";
			}
		}

		internal MySqlDecimal(bool isNull)
		{
			this.isNull = isNull;
			this.mValue = null;
			this.precision = (this.scale = 0);
		}

		internal MySqlDecimal(string val)
		{
			this.isNull = false;
			this.precision = (this.scale = 0);
			this.mValue = val;
		}

		public double ToDouble()
		{
			return double.Parse(this.mValue);
		}

		public override string ToString()
		{
			return this.mValue;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			string text = ((val is decimal) ? ((decimal)val) : Convert.ToDecimal(val)).ToString(CultureInfo.InvariantCulture);
			if (binary)
			{
				packet.WriteLenString(text);
				return;
			}
			packet.WriteStringNoNull(text);
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlDecimal(true);
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
			return new MySqlDecimal(val);
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadFieldLength();
			packet.Position += num;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "DECIMAL";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.NewDecimal;
			mySqlSchemaRow["ColumnSize"] = 0;
			mySqlSchemaRow["CreateFormat"] = "DECIMAL({0},{1})";
			mySqlSchemaRow["CreateParameters"] = "precision,scale";
			mySqlSchemaRow["DataType"] = "System.Decimal";
			mySqlSchemaRow["IsAutoincrementable"] = false;
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
