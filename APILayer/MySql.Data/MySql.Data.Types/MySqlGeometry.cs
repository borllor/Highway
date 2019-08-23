using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	public struct MySqlGeometry : IMySqlValue
	{
		private const int GEOMETRY_LENGTH = 25;

		private MySqlDbType _type;

		private double _xValue;

		private double _yValue;

		private int _srid;

		private byte[] _valBinary;

		private bool _isNull;

		public double? XCoordinate
		{
			get
			{
				return new double?(this._xValue);
			}
		}

		public double? YCoordinate
		{
			get
			{
				return new double?(this._yValue);
			}
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get
			{
				return this._type;
			}
		}

		public bool IsNull
		{
			get
			{
				return this._isNull;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this._valBinary;
			}
		}

		public byte[] Value
		{
			get
			{
				return this._valBinary;
			}
		}

		Type IMySqlValue.SystemType
		{
			get
			{
				return typeof(byte[]);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				return "GEOMETRY";
			}
		}

		public MySqlGeometry(bool isNull)
		{
			this = new MySqlGeometry(MySqlDbType.Geometry, isNull);
		}

		public MySqlGeometry(double xValue, double yValue)
		{
			this = new MySqlGeometry(MySqlDbType.Geometry, xValue, yValue, 0);
		}

		public MySqlGeometry(double xValue, double yValue, int srid)
		{
			this = new MySqlGeometry(MySqlDbType.Geometry, xValue, yValue, srid);
		}

		internal MySqlGeometry(MySqlDbType type, bool isNull)
		{
			this._type = type;
			isNull = true;
			this._xValue = 0.0;
			this._yValue = 0.0;
			this._srid = 0;
			this._valBinary = null;
			this._isNull = isNull;
		}

		internal MySqlGeometry(MySqlDbType type, double xValue, double yValue, int srid)
		{
			this._type = type;
			this._xValue = xValue;
			this._yValue = yValue;
			this._isNull = false;
			this._srid = srid;
			this._valBinary = new byte[25];
			byte[] bytes = BitConverter.GetBytes(srid);
			for (int i = 0; i < bytes.Length; i++)
			{
				this._valBinary[i] = bytes[i];
			}
			long num = BitConverter.DoubleToInt64Bits(xValue);
			long num2 = BitConverter.DoubleToInt64Bits(yValue);
			this._valBinary[4] = 1;
			this._valBinary[5] = 1;
			for (int j = 0; j < 8; j++)
			{
				this._valBinary[j + 9] = (byte)(num & 255L);
				num >>= 8;
			}
			for (int k = 0; k < 8; k++)
			{
				this._valBinary[k + 17] = (byte)(num2 & 255L);
				num2 >>= 8;
			}
		}

		public MySqlGeometry(MySqlDbType type, byte[] val)
		{
			if (val == null)
			{
				throw new ArgumentNullException("val");
			}
			byte[] array = new byte[val.Length];
			for (int i = 0; i < val.Length; i++)
			{
				array[i] = val[i];
			}
			int startIndex = (val.Length == 25) ? 9 : 5;
			int startIndex2 = (val.Length == 25) ? 17 : 13;
			this._valBinary = array;
			this._xValue = BitConverter.ToDouble(val, startIndex);
			this._yValue = BitConverter.ToDouble(val, startIndex2);
			this._srid = ((val.Length == 25) ? BitConverter.ToInt32(val, 0) : 0);
			this._isNull = false;
			this._type = type;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			byte[] array = null;
			try
			{
				array = ((MySqlGeometry)val)._valBinary;
			}
			catch
			{
				array = (val as byte[]);
			}
			if (array == null)
			{
				MySqlGeometry mySqlGeometry = new MySqlGeometry(0.0, 0.0);
				MySqlGeometry.TryParse(val.ToString(), out mySqlGeometry);
				array = mySqlGeometry._valBinary;
			}
			byte[] array2 = new byte[25];
			for (int i = 0; i < array.Length; i++)
			{
				if (array.Length < 25)
				{
					array2[i + 4] = array[i];
				}
				else
				{
					array2[i] = array[i];
				}
			}
			packet.WriteStringNoNull("_binary ");
			packet.WriteByte(39);
			MySqlGeometry.EscapeByteArray(array2, 25, packet);
			packet.WriteByte(39);
		}

		private static void EscapeByteArray(byte[] bytes, int length, MySqlPacket packet)
		{
			for (int i = 0; i < length; i++)
			{
				byte b = bytes[i];
				if (b == 0)
				{
					packet.WriteByte(92);
					packet.WriteByte(48);
				}
				else if (b == 92 || b == 39 || b == 34)
				{
					packet.WriteByte(92);
					packet.WriteByte(b);
				}
				else
				{
					packet.WriteByte(b);
				}
			}
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			MySqlGeometry mySqlGeometry;
			if (nullVal)
			{
				mySqlGeometry = new MySqlGeometry(this._type, true);
			}
			else
			{
				if (length == -1L)
				{
					length = packet.ReadFieldLength();
				}
				byte[] array = new byte[length];
				packet.Read(array, 0, (int)length);
				mySqlGeometry = new MySqlGeometry(this._type, array);
			}
			return mySqlGeometry;
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadFieldLength();
			packet.Position += num;
		}

		public override string ToString()
		{
			if (this._isNull)
			{
				return string.Empty;
			}
			if (this._srid == 0)
			{
				return string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", new object[]
				{
					this._xValue,
					this._yValue
				});
			}
			return string.Format(CultureInfo.InvariantCulture.NumberFormat, "SRID={2};POINT({0} {1})", new object[]
			{
				this._xValue,
				this._yValue,
				this._srid
			});
		}

		public static MySqlGeometry Parse(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			if (!value.Contains("SRID") && !value.Contains("POINT(") && !value.Contains("POINT ("))
			{
				throw new FormatException("String does not contain a valid geometry value");
			}
			MySqlGeometry result = new MySqlGeometry(0.0, 0.0);
			MySqlGeometry.TryParse(value, out result);
			return result;
		}

		public static bool TryParse(string value, out MySqlGeometry mySqlGeometryValue)
		{
			string[] array = new string[0];
			string text = string.Empty;
			bool flag = false;
			bool flag2 = false;
			double xValue = 0.0;
			double yValue = 0.0;
			int srid = 0;
			try
			{
				if (value.Contains(";"))
				{
					array = value.Split(new char[]
					{
						';'
					});
				}
				else
				{
					text = value;
				}
				if (array.Length > 1 || text != string.Empty)
				{
					string text2 = (text != string.Empty) ? text : array[1];
					text2 = text2.Replace("POINT (", "").Replace("POINT(", "").Replace(")", "");
					string[] array2 = text2.Split(new char[]
					{
						' '
					});
					if (array2.Length > 1)
					{
						flag = double.TryParse(array2[0], out xValue);
						flag2 = double.TryParse(array2[1], out yValue);
					}
					if (array.Length >= 1)
					{
						int.TryParse(array[0].Replace("SRID=", ""), out srid);
					}
				}
				if (flag && flag2)
				{
					mySqlGeometryValue = new MySqlGeometry(xValue, yValue, srid);
					return true;
				}
			}
			catch
			{
			}
			mySqlGeometryValue = new MySqlGeometry(true);
			return false;
		}

		public static void SetDSInfo(MySqlSchemaCollection dsTable)
		{
			MySqlSchemaRow mySqlSchemaRow = dsTable.AddRow();
			mySqlSchemaRow["TypeName"] = "GEOMETRY";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.Geometry;
			mySqlSchemaRow["ColumnSize"] = 25;
			mySqlSchemaRow["CreateFormat"] = "GEOMETRY";
			mySqlSchemaRow["CreateParameters"] = DBNull.Value;
			mySqlSchemaRow["DataType"] = "System.Byte[]";
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
