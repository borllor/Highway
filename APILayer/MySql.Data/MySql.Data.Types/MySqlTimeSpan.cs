using System;
using System.Runtime.InteropServices;

using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MySqlTimeSpan : IMySqlValue
    {
        private TimeSpan mValue;
        private bool isNull;
        public MySqlTimeSpan(bool isNull)
        {
            this.isNull = isNull;
            this.mValue = TimeSpan.MinValue;
        }

        public MySqlTimeSpan(TimeSpan val)
        {
            this.isNull = false;
            this.mValue = val;
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
                return MySqlDbType.Time;
            }
        }
        object IMySqlValue.Value
        {
            get
            {
                return this.mValue;
            }
        }
        public TimeSpan Value
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
                return typeof(TimeSpan);
            }
        }
        string IMySqlValue.MySqlTypeName
        {
            get
            {
                return "TIME";
            }
        }
        void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
        {
            if (!(val is TimeSpan))
            {
                throw new MySqlException("Only TimeSpan objects can be serialized by MySqlTimeSpan");
            }
            TimeSpan span = (TimeSpan)val;
            bool flag = span.TotalMilliseconds < 0.0;
            span = span.Duration();
            if (binary)
            {
                if (span.Milliseconds > 0)
                {
                    packet.WriteByte(12);
                }
                else
                {
                    packet.WriteByte(8);
                }
                packet.WriteByte(flag ? ((byte)1) : ((byte)0));
                packet.WriteInteger((long)span.Days, 4);
                packet.WriteByte((byte)span.Hours);
                packet.WriteByte((byte)span.Minutes);
                packet.WriteByte((byte)span.Seconds);
                if (span.Milliseconds > 0)
                {
                    long v = span.Milliseconds * 0x3e8;
                    packet.WriteInteger(v, 4);
                }
            }
            else
            {
                string str = string.Format("'{0}{1} {2:00}:{3:00}:{4:00}.{5:000000}'", new object[] { flag ? "-" : "", span.Days, span.Hours, span.Minutes, span.Seconds, span.Ticks % 0x989680L });
                packet.WriteStringNoNull(str);
            }
        }

        IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
        {
            if (nullVal)
            {
                return new MySqlTimeSpan(true);
            }
            if (length >= 0L)
            {
                string s = packet.ReadString(length);
                this.ParseMySql(s);
                return this;
            }
            long num = packet.ReadByte();
            int num2 = 0;
            if (num > 0L)
            {
                num2 = packet.ReadByte();
            }
            this.isNull = false;
            switch (num)
            {
                case 0L:
                    this.isNull = true;
                    break;

                case 5L:
                    this.mValue = new TimeSpan(packet.ReadInteger(4), 0, 0, 0);
                    break;

                case 8L:
                    this.mValue = new TimeSpan(packet.ReadInteger(4), packet.ReadByte(), packet.ReadByte(), packet.ReadByte());
                    break;

                default:
                    this.mValue = new TimeSpan(packet.ReadInteger(4), packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadInteger(4) / 0xf4240);
                    break;
            }
            if (num2 == 1)
            {
                this.mValue = this.mValue.Negate();
            }
            return this;
        }

        void IMySqlValue.SkipValue(MySqlPacket packet)
        {
            int num = packet.ReadByte();
            packet.Position += num;
        }

        internal static void SetDSInfo(MySqlSchemaCollection sc)
        {
            MySqlSchemaRow row = sc.AddRow();
            row["TypeName"] = "TIME";
            row["ProviderDbType"] = MySqlDbType.Time;
            row["ColumnSize"] = 0;
            row["CreateFormat"] = "TIME";
            row["CreateParameters"] = null;
            row["DataType"] = "System.TimeSpan";
            row["IsAutoincrementable"] = false;
            row["IsBestMatch"] = true;
            row["IsCaseSensitive"] = false;
            row["IsFixedLength"] = true;
            row["IsFixedPrecisionScale"] = true;
            row["IsLong"] = false;
            row["IsNullable"] = true;
            row["IsSearchable"] = true;
            row["IsSearchableWithLike"] = false;
            row["IsUnsigned"] = false;
            row["MaximumScale"] = 0;
            row["MinimumScale"] = 0;
            row["IsConcurrencyType"] = DBNull.Value;
            row["IsLiteralSupported"] = false;
            row["LiteralPrefix"] = null;
            row["LiteralSuffix"] = null;
            row["NativeDataType"] = null;
        }

        public override string ToString()
        {
            return string.Format("{0} {1:00}:{2:00}:{3:00}", new object[] { this.mValue.Days, this.mValue.Hours, this.mValue.Minutes, this.mValue.Seconds });
        }

        private void ParseMySql(string s)
        {
            string[] strArray = s.Split(new char[] { ':', '.' });
            int hours = int.Parse(strArray[0]);
            int minutes = int.Parse(strArray[1]);
            int seconds = int.Parse(strArray[2]);
            int num4 = 0;
            if (strArray.Length > 3)
            {
                strArray[3] = strArray[3].PadRight(7, '0');
                num4 = int.Parse(strArray[3]);
            }
            if ((hours < 0) || strArray[0].StartsWith("-", StringComparison.Ordinal))
            {
                minutes *= -1;
                seconds *= -1;
                num4 *= -1;
            }
            int days = hours / 0x18;
            hours -= days * 0x18;
            this.mValue = new TimeSpan(days, hours, minutes, seconds).Add(new TimeSpan((long)num4));
            this.isNull = false;
        }
    }
}
