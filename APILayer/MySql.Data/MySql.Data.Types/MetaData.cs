using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace MySql.Data.Types
{
	internal class MetaData
	{
		public static bool IsNumericType(string typename)
		{
			string text = typename.ToLower(CultureInfo.InvariantCulture);
			string key;
			switch (key = text)
			{
			case "int":
			case "integer":
			case "numeric":
			case "decimal":
			case "dec":
			case "fixed":
			case "tinyint":
			case "mediumint":
			case "bigint":
			case "real":
			case "double":
			case "float":
			case "serial":
			case "smallint":
				return true;
			}
			return false;
		}

		public static bool IsTextType(string typename)
		{
			string text = typename.ToLower(CultureInfo.InvariantCulture);
			string key;
			switch (key = text)
			{
			case "varchar":
			case "char":
			case "text":
			case "longtext":
			case "tinytext":
			case "mediumtext":
			case "nchar":
			case "nvarchar":
			case "enum":
			case "set":
				return true;
			}
			return false;
		}

		public static bool SupportScale(string typename)
		{
			string text = StringUtility.ToLowerInvariant(typename);
			string a;
			return (a = text) != null && (a == "numeric" || a == "decimal" || a == "dec" || a == "real");
		}

		public static MySqlDbType NameToType(string typeName, bool unsigned, bool realAsFloat, MySqlConnection connection)
		{
			string key;
			switch (key = StringUtility.ToUpperInvariant(typeName))
			{
			case "CHAR":
				return MySqlDbType.String;
			case "VARCHAR":
				return MySqlDbType.VarChar;
			case "DATE":
				return MySqlDbType.Date;
			case "DATETIME":
				return MySqlDbType.DateTime;
			case "NUMERIC":
			case "DECIMAL":
			case "DEC":
			case "FIXED":
				if (connection.driver.Version.isAtLeast(5, 0, 3))
				{
					return MySqlDbType.NewDecimal;
				}
				return MySqlDbType.Decimal;
			case "YEAR":
				return MySqlDbType.Year;
			case "TIME":
				return MySqlDbType.Time;
			case "TIMESTAMP":
				return MySqlDbType.Timestamp;
			case "SET":
				return MySqlDbType.Set;
			case "ENUM":
				return MySqlDbType.Enum;
			case "BIT":
				return MySqlDbType.Bit;
			case "TINYINT":
				if (!unsigned)
				{
					return MySqlDbType.Byte;
				}
				return MySqlDbType.UByte;
			case "BOOL":
			case "BOOLEAN":
				return MySqlDbType.Byte;
			case "SMALLINT":
				if (!unsigned)
				{
					return MySqlDbType.Int16;
				}
				return MySqlDbType.UInt16;
			case "MEDIUMINT":
				if (!unsigned)
				{
					return MySqlDbType.Int24;
				}
				return MySqlDbType.UInt24;
			case "INT":
			case "INTEGER":
				if (!unsigned)
				{
					return MySqlDbType.Int32;
				}
				return MySqlDbType.UInt32;
			case "SERIAL":
				return MySqlDbType.UInt64;
			case "BIGINT":
				if (!unsigned)
				{
					return MySqlDbType.Int64;
				}
				return MySqlDbType.UInt64;
			case "FLOAT":
				return MySqlDbType.Float;
			case "DOUBLE":
				return MySqlDbType.Double;
			case "REAL":
				if (!realAsFloat)
				{
					return MySqlDbType.Double;
				}
				return MySqlDbType.Float;
			case "TEXT":
				return MySqlDbType.Text;
			case "BLOB":
				return MySqlDbType.Blob;
			case "LONGBLOB":
				return MySqlDbType.LongBlob;
			case "LONGTEXT":
				return MySqlDbType.LongText;
			case "MEDIUMBLOB":
				return MySqlDbType.MediumBlob;
			case "MEDIUMTEXT":
				return MySqlDbType.MediumText;
			case "TINYBLOB":
				return MySqlDbType.TinyBlob;
			case "TINYTEXT":
				return MySqlDbType.TinyText;
			case "BINARY":
				return MySqlDbType.Binary;
			case "VARBINARY":
				return MySqlDbType.VarBinary;
			}
			throw new MySqlException("Unhandled type encountered");
		}
	}
}
