using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Properties;
using System;
using System.Globalization;

namespace MySql.Data.Common
{
	internal struct DBVersion
	{
		private int major;

		private int minor;

		private int build;

		private string srcString;

		public int Major
		{
			get
			{
				return this.major;
			}
		}

		public int Minor
		{
			get
			{
				return this.minor;
			}
		}

		public int Build
		{
			get
			{
				return this.build;
			}
		}

		public DBVersion(string s, int major, int minor, int build)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
			this.srcString = s;
		}

		public static DBVersion Parse(string versionString)
		{
			int num = 0;
			int num2 = versionString.IndexOf('.', num);
			if (num2 == -1)
			{
				throw new MySqlException(Resources.BadVersionFormat);
			}
			string value = versionString.Substring(num, num2 - num).Trim();
			int num3 = Convert.ToInt32(value, NumberFormatInfo.InvariantInfo);
			num = num2 + 1;
			num2 = versionString.IndexOf('.', num);
			if (num2 == -1)
			{
				throw new MySqlException(Resources.BadVersionFormat);
			}
			value = versionString.Substring(num, num2 - num).Trim();
			int num4 = Convert.ToInt32(value, NumberFormatInfo.InvariantInfo);
			num = num2 + 1;
			int num5 = num;
			while (num5 < versionString.Length && char.IsDigit(versionString, num5))
			{
				num5++;
			}
			value = versionString.Substring(num, num5 - num).Trim();
			int num6 = Convert.ToInt32(value, NumberFormatInfo.InvariantInfo);
			return new DBVersion(versionString, num3, num4, num6);
		}

		public bool isAtLeast(int majorNum, int minorNum, int buildNum)
		{
			return this.major > majorNum || (this.major == majorNum && this.minor > minorNum) || (this.major == majorNum && this.minor == minorNum && this.build >= buildNum);
		}

		public override string ToString()
		{
			return this.srcString;
		}
	}
}
