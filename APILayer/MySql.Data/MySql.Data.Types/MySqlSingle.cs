using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	internal struct MySqlSingle : IMySqlValue
	{
		private float mValue;

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
				return MySqlDbType.Float;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public float Value
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
				return typeof(float);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "FLOAT";
			}
		}

		public MySqlSingle(bool isNull)
		{
			this.isNull = isNull;
			this.mValue = 0f;
		}

		public MySqlSingle(float val)
		{
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			float value = (val is float) ? ((float)val) : Convert.ToSingle(val);
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
				return new MySqlSingle(true);
			}
			if (length == -1L)
			{
				byte[] array = new byte[4];
				packet.Read(array, 0, 4);
				return new MySqlSingle(BitConverter.ToSingle(array, 0));
			}
			return new MySqlSingle(float.Parse(packet.ReadString(length), CultureInfo.InvariantCulture));
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			packet.Position += 4;
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "FLOAT";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.Float;
			mySqlSchemaRow["ColumnSize"] = 0;
			mySqlSchemaRow["CreateFormat"] = "FLOAT";
			mySqlSchemaRow["CreateParameters"] = null;
			mySqlSchemaRow["DataType"] = "System.Single";
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
