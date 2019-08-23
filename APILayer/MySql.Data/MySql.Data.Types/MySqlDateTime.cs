using MySql.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	[Serializable]
	public struct MySqlDateTime : IConvertible, IMySqlValue, IComparable
	{
		private bool isNull;

		private MySqlDbType type;

		private int year;

		private int month;

		private int day;

		private int hour;

		private int minute;

		private int second;

		private int millisecond;

		public int TimezoneOffset;

		public bool IsValidDateTime
		{
			get
			{
				return this.year != 0 && this.month != 0 && this.day != 0;
			}
		}

		public int Year
		{
			get
			{
				return this.year;
			}
			set
			{
				this.year = value;
			}
		}

		public int Month
		{
			get
			{
				return this.month;
			}
			set
			{
				this.month = value;
			}
		}

		public int Day
		{
			get
			{
				return this.day;
			}
			set
			{
				this.day = value;
			}
		}

		public int Hour
		{
			get
			{
				return this.hour;
			}
			set
			{
				this.hour = value;
			}
		}

		public int Minute
		{
			get
			{
				return this.minute;
			}
			set
			{
				this.minute = value;
			}
		}

		public int Second
		{
			get
			{
				return this.second;
			}
			set
			{
				this.second = value;
			}
		}

		public int Millisecond
		{
			get
			{
				return this.millisecond;
			}
			set
			{
				this.millisecond = value;
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
				return this.type;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.GetDateTime();
			}
		}

		public DateTime Value
		{
			get
			{
				return this.GetDateTime();
			}
		}

		Type IMySqlValue.SystemType
		{
			get
			{
				return typeof(DateTime);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				MySqlDbType mySqlDbType = this.type;
				if (mySqlDbType == MySqlDbType.Timestamp)
				{
					return "TIMESTAMP";
				}
				if (mySqlDbType == MySqlDbType.Date)
				{
					return "DATE";
				}
				if (mySqlDbType != MySqlDbType.Newdate)
				{
					return "DATETIME";
				}
				return "NEWDATE";
			}
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return 0uL;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return 0;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return 0.0;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this.GetDateTime();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return 0f;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return false;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return 0;
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return 0;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return 0;
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return null;
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return 0;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return '\0';
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return 0L;
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Empty;
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return 0m;
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return null;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return 0u;
		}

		public MySqlDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			this = new MySqlDateTime(MySqlDbType.DateTime, year, month, day, hour, minute, second, millisecond);
		}

		public MySqlDateTime(DateTime dt)
		{
			this = new MySqlDateTime(MySqlDbType.DateTime, dt);
		}

		public MySqlDateTime(MySqlDateTime mdt)
		{
			this.year = mdt.Year;
			this.month = mdt.Month;
			this.day = mdt.Day;
			this.hour = mdt.Hour;
			this.minute = mdt.Minute;
			this.second = mdt.Second;
			this.millisecond = 0;
			this.type = MySqlDbType.DateTime;
			this.isNull = false;
			this.TimezoneOffset = 0;
		}

		public MySqlDateTime(string dateTime)
		{
			this = new MySqlDateTime(MySqlDateTime.Parse(dateTime));
		}

		internal MySqlDateTime(MySqlDbType type, int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			this.isNull = false;
			this.type = type;
			this.year = year;
			this.month = month;
			this.day = day;
			this.hour = hour;
			this.minute = minute;
			this.second = second;
			this.millisecond = ((millisecond > 0) ? millisecond : 0);
			this.TimezoneOffset = 0;
		}

		internal MySqlDateTime(MySqlDbType type, bool isNull)
		{
			this = new MySqlDateTime(type, 0, 0, 0, 0, 0, 0, 0);
			this.isNull = isNull;
		}

		internal MySqlDateTime(MySqlDbType type, DateTime val)
		{
			this = new MySqlDateTime(type, 0, 0, 0, 0, 0, 0, 0);
			this.isNull = false;
			this.year = val.Year;
			this.month = val.Month;
			this.day = val.Day;
			this.hour = val.Hour;
			this.minute = val.Minute;
			this.second = val.Second;
			this.millisecond = val.Millisecond;
		}

		private void SerializeText(MySqlPacket packet, MySqlDateTime value)
		{
			string text = string.Empty;
			text = string.Format("{0:0000}-{1:00}-{2:00}", value.Year, value.Month, value.Day);
			if (this.type != MySqlDbType.Date)
			{
				text = ((value.Millisecond > 0) ? string.Format("{0} {1:00}:{2:00}:{3:00}.{4:000}", new object[]
				{
					text,
					value.Hour,
					value.Minute,
					value.Second,
					value.Millisecond
				}) : string.Format("{0} {1:00}:{2:00}:{3:00} ", new object[]
				{
					text,
					value.Hour,
					value.Minute,
					value.Second
				}));
			}
			packet.WriteStringNoNull("'" + text + "'");
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object value, int length)
		{
			string text = value as string;
			MySqlDateTime value2;
			if (value is DateTime)
			{
				value2 = new MySqlDateTime(this.type, (DateTime)value);
			}
			else if (text != null)
			{
				value2 = MySqlDateTime.Parse(text);
			}
			else
			{
				if (!(value is MySqlDateTime))
				{
					throw new MySqlException("Unable to serialize date/time value.");
				}
				value2 = (MySqlDateTime)value;
			}
			if (!binary)
			{
				this.SerializeText(packet, value2);
				return;
			}
			if (value2.Millisecond > 0)
			{
				packet.WriteByte(11);
			}
			else
			{
				packet.WriteByte(7);
			}
			packet.WriteInteger((long)value2.Year, 2);
			packet.WriteByte((byte)value2.Month);
			packet.WriteByte((byte)value2.Day);
			if (this.type == MySqlDbType.Date)
			{
				packet.WriteByte(0);
				packet.WriteByte(0);
				packet.WriteByte(0);
			}
			else
			{
				packet.WriteByte((byte)value2.Hour);
				packet.WriteByte((byte)value2.Minute);
				packet.WriteByte((byte)value2.Second);
			}
			if (value2.Millisecond > 0)
			{
				long num = (long)((value2.Millisecond < 1000) ? (value2.Millisecond * 1000) : value2.Millisecond);
				for (int i = 0; i < 4; i++)
				{
					packet.WriteByte((byte)(num & 255L));
					num >>= 8;
				}
			}
		}

		internal static MySqlDateTime Parse(string s)
		{
			return default(MySqlDateTime).ParseMySql(s);
		}

		internal static MySqlDateTime Parse(string s, DBVersion version)
		{
			return default(MySqlDateTime).ParseMySql(s);
		}

		private MySqlDateTime ParseMySql(string s)
		{
			string[] array = s.Split(new char[]
			{
				'-',
				' ',
				':',
				'/',
				'.'
			});
			int num = int.Parse(array[0]);
			int num2 = int.Parse(array[1]);
			int num3 = int.Parse(array[2]);
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			if (array.Length > 3)
			{
				num4 = int.Parse(array[3]);
				num5 = int.Parse(array[4]);
				num6 = int.Parse(array[5]);
			}
			if (array.Length > 6)
			{
				num7 = int.Parse(array[6]);
			}
			return new MySqlDateTime(this.type, num, num2, num3, num4, num5, num6, num7);
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
			{
				return new MySqlDateTime(this.type, true);
			}
			if (length >= 0L)
			{
				string s = packet.ReadString(length);
				return this.ParseMySql(s);
			}
			long num = (long)((ulong)packet.ReadByte());
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			if (num >= 4L)
			{
				num2 = packet.ReadInteger(2);
				num3 = (int)packet.ReadByte();
				num4 = (int)packet.ReadByte();
			}
			if (num > 4L)
			{
				num5 = (int)packet.ReadByte();
				num6 = (int)packet.ReadByte();
				num7 = (int)packet.ReadByte();
			}
			if (num > 7L)
			{
				num8 = packet.Read3ByteInt();
				packet.ReadByte();
			}
			return new MySqlDateTime(this.type, num2, num3, num4, num5, num6, num7, num8);
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadByte();
			packet.Position += num;
		}

		public DateTime GetDateTime()
		{
			if (!this.IsValidDateTime)
			{
				throw new MySqlConversionException("Unable to convert MySQL date/time value to System.DateTime");
			}
			if (this.millisecond < 0 || this.millisecond >= 1000)
			{
				this.millisecond /= 1000;
			}
			DateTimeKind kind = DateTimeKind.Unspecified;
			if (this.type == MySqlDbType.Timestamp)
			{
				if (this.TimezoneOffset == 0)
				{
					kind = DateTimeKind.Utc;
				}
				else
				{
					kind = DateTimeKind.Local;
				}
			}
			return new DateTime(this.year, this.month, this.day, this.hour, this.minute, this.second, this.millisecond, kind);
		}

		private static string FormatDateCustom(string format, int monthVal, int dayVal, int yearVal)
		{
			format = format.Replace("MM", "{0:00}");
			format = format.Replace("M", "{0}");
			format = format.Replace("dd", "{1:00}");
			format = format.Replace("d", "{1}");
			format = format.Replace("yyyy", "{2:0000}");
			format = format.Replace("yy", "{3:00}");
			format = format.Replace("y", "{4:0}");
			int num = yearVal - yearVal / 1000 * 1000;
			num -= num / 100 * 100;
			int num2 = num - num / 10 * 10;
			return string.Format(format, new object[]
			{
				monthVal,
				dayVal,
				yearVal,
				num,
				num2
			});
		}

		public override string ToString()
		{
			if (this.IsValidDateTime)
			{
				DateTime dateTime = new DateTime(this.year, this.month, this.day, this.hour, this.minute, this.second, this.millisecond);
				if (this.type != MySqlDbType.Date)
				{
					return dateTime.ToString();
				}
				return dateTime.ToString("d");
			}
			else
			{
				string text = MySqlDateTime.FormatDateCustom(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern, this.month, this.day, this.year);
				if (this.type == MySqlDbType.Date)
				{
					return text;
				}
				DateTime dateTime2 = new DateTime(1, 2, 3, this.hour, this.minute, this.second, this.millisecond);
				return string.Format("{0} {1}", text, dateTime2.ToLongTimeString());
			}
		}

		public static explicit operator DateTime(MySqlDateTime val)
		{
			if (!val.IsValidDateTime)
			{
				return DateTime.MinValue;
			}
			return val.GetDateTime();
		}

		internal static void SetDSInfo(MySqlSchemaCollection sc)
		{
			string[] array = new string[]
			{
				"DATE",
				"DATETIME",
				"TIMESTAMP"
			};
			MySqlDbType[] array2 = new MySqlDbType[]
			{
				MySqlDbType.Date,
				MySqlDbType.DateTime,
				MySqlDbType.Timestamp
			};
			for (int i = 0; i < array.Length; i++)
			{
				MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
				mySqlSchemaRow["TypeName"] = array[i];
				mySqlSchemaRow["ProviderDbType"] = array2[i];
				mySqlSchemaRow["ColumnSize"] = 0;
				mySqlSchemaRow["CreateFormat"] = array[i];
				mySqlSchemaRow["CreateParameters"] = null;
				mySqlSchemaRow["DataType"] = "System.DateTime";
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

		int IComparable.CompareTo(object obj)
		{
			MySqlDateTime mySqlDateTime = (MySqlDateTime)obj;
			if (this.Year < mySqlDateTime.Year)
			{
				return -1;
			}
			if (this.Year > mySqlDateTime.Year)
			{
				return 1;
			}
			if (this.Month < mySqlDateTime.Month)
			{
				return -1;
			}
			if (this.Month > mySqlDateTime.Month)
			{
				return 1;
			}
			if (this.Day < mySqlDateTime.Day)
			{
				return -1;
			}
			if (this.Day > mySqlDateTime.Day)
			{
				return 1;
			}
			if (this.Hour < mySqlDateTime.Hour)
			{
				return -1;
			}
			if (this.Hour > mySqlDateTime.Hour)
			{
				return 1;
			}
			if (this.Minute < mySqlDateTime.Minute)
			{
				return -1;
			}
			if (this.Minute > mySqlDateTime.Minute)
			{
				return 1;
			}
			if (this.Second < mySqlDateTime.Second)
			{
				return -1;
			}
			if (this.Second > mySqlDateTime.Second)
			{
				return 1;
			}
			if (this.Millisecond < mySqlDateTime.Millisecond)
			{
				return -1;
			}
			if (this.Millisecond > mySqlDateTime.Millisecond)
			{
				return 1;
			}
			return 0;
		}
	}
}
