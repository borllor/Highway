using MySql.Data.Common;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MySql.Data.MySqlClient
{
	internal class MySqlField
	{
		public string CatalogName;

		public int ColumnLength;

		public string ColumnName;

		public string OriginalColumnName;

		public string TableName;

		public string RealTableName;

		public string DatabaseName;

		public Encoding Encoding;

		public int maxLength;

		protected ColumnFlags colFlags;

		protected int charSetIndex;

		protected byte precision;

		protected byte scale;

		protected MySqlDbType mySqlDbType;

		protected DBVersion connVersion;

		protected Driver driver;

		protected bool binaryOk;

		protected List<Type> typeConversions = new List<Type>();

		public int CharacterSetIndex
		{
			get
			{
				return this.charSetIndex;
			}
			set
			{
				this.charSetIndex = value;
				this.SetFieldEncoding();
			}
		}

		public MySqlDbType Type
		{
			get
			{
				return this.mySqlDbType;
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

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
			set
			{
				this.maxLength = value;
			}
		}

		public ColumnFlags Flags
		{
			get
			{
				return this.colFlags;
			}
		}

		public bool IsAutoIncrement
		{
			get
			{
				return (this.colFlags & ColumnFlags.AUTO_INCREMENT) > (ColumnFlags)0;
			}
		}

		public bool IsNumeric
		{
			get
			{
				return (this.colFlags & ColumnFlags.NUMBER) > (ColumnFlags)0;
			}
		}

		public bool AllowsNull
		{
			get
			{
				return (this.colFlags & ColumnFlags.NOT_NULL) == (ColumnFlags)0;
			}
		}

		public bool IsUnique
		{
			get
			{
				return (this.colFlags & ColumnFlags.UNIQUE_KEY) > (ColumnFlags)0;
			}
		}

		public bool IsPrimaryKey
		{
			get
			{
				return (this.colFlags & ColumnFlags.PRIMARY_KEY) > (ColumnFlags)0;
			}
		}

		public bool IsBlob
		{
			get
			{
				return (this.mySqlDbType >= MySqlDbType.TinyBlob && this.mySqlDbType <= MySqlDbType.Blob) || (this.mySqlDbType >= MySqlDbType.TinyText && this.mySqlDbType <= MySqlDbType.Text) || (this.colFlags & ColumnFlags.BLOB) > (ColumnFlags)0;
			}
		}

		public bool IsBinary
		{
			get
			{
				return this.binaryOk && this.CharacterSetIndex == 63;
			}
		}

		public bool IsUnsigned
		{
			get
			{
				return (this.colFlags & ColumnFlags.UNSIGNED) > (ColumnFlags)0;
			}
		}

		public bool IsTextField
		{
			get
			{
				return this.Type == MySqlDbType.VarString || this.Type == MySqlDbType.VarChar || this.Type == MySqlDbType.String || (this.IsBlob && !this.IsBinary);
			}
		}

		public int CharacterLength
		{
			get
			{
				return this.ColumnLength / this.MaxLength;
			}
		}

		public List<Type> TypeConversions
		{
			get
			{
				return this.typeConversions;
			}
		}

		public MySqlField(Driver driver)
		{
			this.driver = driver;
			this.connVersion = driver.Version;
			this.maxLength = 1;
			this.binaryOk = true;
		}

		public void SetTypeAndFlags(MySqlDbType type, ColumnFlags flags)
		{
			this.colFlags = flags;
			this.mySqlDbType = type;
			if (string.IsNullOrEmpty(this.TableName) && string.IsNullOrEmpty(this.RealTableName) && this.IsBinary && this.driver.Settings.FunctionsReturnString)
			{
				this.CharacterSetIndex = this.driver.ConnectionCharSetIndex;
			}
			if (this.IsUnsigned)
			{
				switch (type)
				{
				case MySqlDbType.Byte:
					this.mySqlDbType = MySqlDbType.UByte;
					return;
				case MySqlDbType.Int16:
					this.mySqlDbType = MySqlDbType.UInt16;
					return;
				case MySqlDbType.Int32:
					this.mySqlDbType = MySqlDbType.UInt32;
					return;
				case MySqlDbType.Int64:
					this.mySqlDbType = MySqlDbType.UInt64;
					return;
				case MySqlDbType.Int24:
					this.mySqlDbType = MySqlDbType.UInt24;
					return;
				}
			}
			if (this.IsBlob)
			{
				if (this.IsBinary && this.driver.Settings.TreatBlobsAsUTF8)
				{
					bool flag = false;
					Regex blobAsUTF8IncludeRegex = this.driver.Settings.GetBlobAsUTF8IncludeRegex();
					Regex blobAsUTF8ExcludeRegex = this.driver.Settings.GetBlobAsUTF8ExcludeRegex();
					if (blobAsUTF8IncludeRegex != null && blobAsUTF8IncludeRegex.IsMatch(this.ColumnName))
					{
						flag = true;
					}
					else if (blobAsUTF8IncludeRegex == null && blobAsUTF8ExcludeRegex != null && !blobAsUTF8ExcludeRegex.IsMatch(this.ColumnName))
					{
						flag = true;
					}
					if (flag)
					{
						this.binaryOk = false;
						this.Encoding = Encoding.GetEncoding("UTF-8");
						this.charSetIndex = -1;
						this.maxLength = 4;
					}
				}
				if (!this.IsBinary)
				{
					if (type == MySqlDbType.TinyBlob)
					{
						this.mySqlDbType = MySqlDbType.TinyText;
					}
					else if (type == MySqlDbType.MediumBlob)
					{
						this.mySqlDbType = MySqlDbType.MediumText;
					}
					else if (type == MySqlDbType.Blob)
					{
						this.mySqlDbType = MySqlDbType.Text;
					}
					else if (type == MySqlDbType.LongBlob)
					{
						this.mySqlDbType = MySqlDbType.LongText;
					}
				}
			}
			if (this.driver.Settings.RespectBinaryFlags)
			{
				this.CheckForExceptions();
			}
			if (this.Type == MySqlDbType.String && this.CharacterLength == 36 && !this.driver.Settings.OldGuids)
			{
				this.mySqlDbType = MySqlDbType.Guid;
			}
			if (!this.IsBinary)
			{
				return;
			}
			if (this.driver.Settings.RespectBinaryFlags)
			{
				if (type == MySqlDbType.String)
				{
					this.mySqlDbType = MySqlDbType.Binary;
				}
				else if (type == MySqlDbType.VarChar || type == MySqlDbType.VarString)
				{
					this.mySqlDbType = MySqlDbType.VarBinary;
				}
			}
			if (this.CharacterSetIndex == 63)
			{
				this.CharacterSetIndex = this.driver.ConnectionCharSetIndex;
			}
			if (this.Type == MySqlDbType.Binary && this.ColumnLength == 16 && this.driver.Settings.OldGuids)
			{
				this.mySqlDbType = MySqlDbType.Guid;
			}
		}

		public void AddTypeConversion(Type t)
		{
			if (this.TypeConversions.Contains(t))
			{
				return;
			}
			this.TypeConversions.Add(t);
		}

		private void CheckForExceptions()
		{
			string text = string.Empty;
			if (this.OriginalColumnName != null)
			{
				text = StringUtility.ToUpperInvariant(this.OriginalColumnName);
			}
			if (text.StartsWith("CHAR(", StringComparison.Ordinal))
			{
				this.binaryOk = false;
			}
		}

		public IMySqlValue GetValueObject()
		{
			IMySqlValue mySqlValue = MySqlField.GetIMySqlValue(this.Type);
			if (mySqlValue is MySqlByte && this.ColumnLength == 1 && this.driver.Settings.TreatTinyAsBoolean)
			{
				MySqlByte mySqlByte = (MySqlByte)mySqlValue;
				mySqlByte.TreatAsBoolean = true;
				mySqlValue = mySqlByte;
			}
			else if (mySqlValue is MySqlGuid)
			{
				MySqlGuid mySqlGuid = (MySqlGuid)mySqlValue;
				mySqlGuid.OldGuids = this.driver.Settings.OldGuids;
				mySqlValue = mySqlGuid;
			}
			return mySqlValue;
		}

		public static IMySqlValue GetIMySqlValue(MySqlDbType type)
		{
			if (type <= MySqlDbType.UInt24)
			{
				switch (type)
				{
				case MySqlDbType.Decimal:
					break;
				case MySqlDbType.Byte:
					return default(MySqlByte);
				case MySqlDbType.Int16:
					return default(MySqlInt16);
				case MySqlDbType.Int32:
				case MySqlDbType.Int24:
				case MySqlDbType.Year:
					return new MySqlInt32(type, true);
				case MySqlDbType.Float:
					return default(MySqlSingle);
				case MySqlDbType.Double:
					return default(MySqlDouble);
				case (MySqlDbType)6:
				case MySqlDbType.VarString:
					goto IL_1D4;
				case MySqlDbType.Timestamp:
				case MySqlDbType.Date:
				case MySqlDbType.DateTime:
				case MySqlDbType.Newdate:
					return new MySqlDateTime(type, true);
				case MySqlDbType.Int64:
					return default(MySqlInt64);
				case MySqlDbType.Time:
					return default(MySqlTimeSpan);
				case MySqlDbType.Bit:
					return default(MySqlBit);
				default:
					switch (type)
					{
					case MySqlDbType.NewDecimal:
						break;
					case MySqlDbType.Enum:
					case MySqlDbType.Set:
					case MySqlDbType.VarChar:
					case MySqlDbType.String:
						goto IL_1D4;
					case MySqlDbType.TinyBlob:
					case MySqlDbType.MediumBlob:
					case MySqlDbType.LongBlob:
					case MySqlDbType.Blob:
						goto IL_1EE;
					case MySqlDbType.Geometry:
						return new MySqlGeometry(type, true);
					default:
						switch (type)
						{
						case MySqlDbType.UByte:
							return default(MySqlUByte);
						case MySqlDbType.UInt16:
							return default(MySqlUInt16);
						case MySqlDbType.UInt32:
						case MySqlDbType.UInt24:
							return new MySqlUInt32(type, true);
						case (MySqlDbType)504:
						case (MySqlDbType)505:
						case (MySqlDbType)506:
						case (MySqlDbType)507:
							goto IL_20B;
						case MySqlDbType.UInt64:
							return default(MySqlUInt64);
						default:
							goto IL_20B;
						}
						break;
					}
					break;
				}
				return default(MySqlDecimal);
			}
			switch (type)
			{
			case MySqlDbType.Binary:
			case MySqlDbType.VarBinary:
				goto IL_1EE;
			default:
				switch (type)
				{
				case MySqlDbType.TinyText:
				case MySqlDbType.MediumText:
				case MySqlDbType.LongText:
				case MySqlDbType.Text:
					break;
				default:
					if (type != MySqlDbType.Guid)
					{
						goto IL_20B;
					}
					return default(MySqlGuid);
				}
				break;
			}
			IL_1D4:
			return new MySqlString(type, true);
			IL_1EE:
			return new MySqlBinary(type, true);
			IL_20B:
			throw new MySqlException("Unknown data type");
		}

		private void SetFieldEncoding()
		{
			Dictionary<int, string> characterSets = this.driver.CharacterSets;
			DBVersion version = this.driver.Version;
			if (characterSets == null || this.CharacterSetIndex == -1)
			{
				return;
			}
			if (characterSets[this.CharacterSetIndex] == null)
			{
				return;
			}
			CharacterSet characterSet = CharSetMap.GetCharacterSet(version, characterSets[this.CharacterSetIndex]);
			if (characterSet.name.ToLower(CultureInfo.InvariantCulture) == "utf-8" && version.Major >= 6)
			{
				this.MaxLength = 4;
			}
			else
			{
				this.MaxLength = characterSet.byteCount;
			}
			this.Encoding = CharSetMap.GetEncoding(version, characterSets[this.CharacterSetIndex]);
		}
	}
}
