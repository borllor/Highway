using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	internal struct MySqlDouble : IMySqlValue
	{
		private double mValue;

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
				return MySqlDbType.Double;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public double Value
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
				return typeof(double);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "DOUBLE";
			}
		}

		public MySqlDouble(bool isNull)
		{
			this.isNull = isNull;
			this.mValue = 0.0;
		}

		public MySqlDouble(double val)
		{
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			double value = (val is double) ? ((double)val) : Convert.ToDouble(val);
			if (binary)
			{
				packet.Write(BitConverter.GetBytes(value));
				return;
			}
			packet.WriteStringNoNull(value.ToString("R", CultureInfo.InvariantCulture));
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlDouble(true);
			}
			if (length == -1L)
			{
				byte[] array = new byte[8];
				packet.Read(array, 0, 8);
				return new MySqlDouble(BitConverter.ToDouble(array, 0));
			}
			string text = packet.ReadString(length);
			double val;
			try
			{
				val = double.Parse(text, CultureInfo.InvariantCulture);
			}
			catch (OverflowException)
			{
				if (text.StartsWith("-", StringComparison.Ordinal))
				{
					val = -1.7976931348623157E+308;
				}
				else
				{
					val = 1.7976931348623157E+308;
				}
			}
			return new MySqlDouble(val);
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			packet.Position += 8;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "DOUBLE";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.Double;
			mySqlSchemaRow["ColumnSize"] = 0;
			mySqlSchemaRow["CreateFormat"] = "DOUBLE";
			mySqlSchemaRow["CreateParameters"] = null;
			mySqlSchemaRow["DataType"] = "System.Double";
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
