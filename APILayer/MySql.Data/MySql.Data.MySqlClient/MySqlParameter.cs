using MySql.Data.Types;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;

namespace MySql.Data.MySqlClient
{
	[TypeConverter(typeof(MySqlParameterConverter))]
	public sealed class MySqlParameter : DbParameter, IDbDataParameter, IDataParameter, ICloneable
	{
		private const int UNSIGNED_MASK = 32768;

		private const int GEOMETRY_LENGTH = 25;

		private DbType dbType;

		private object paramValue;

		private string paramName;

		private MySqlDbType mySqlDbType;

		private bool inferType = true;

		private IMySqlValue _valueObject;

		[Category("Data")]
		public override DataRowVersion SourceVersion
		{
			get;
			set;
		}

		[Category("Data")]
		public override string SourceColumn
		{
			get;
			set;
		}

		public override bool SourceColumnNullMapping
		{
			get;
			set;
		}

		public override DbType DbType
		{
			get
			{
				return this.dbType;
			}
			set
			{
				this.SetDbType(value);
				this.inferType = false;
			}
		}

		[Category("Misc")]
		public override string ParameterName
		{
			get
			{
				return this.paramName;
			}
			set
			{
				this.SetParameterName(value);
			}
		}

		internal MySqlParameterCollection Collection
		{
			get;
			set;
		}

		internal Encoding Encoding
		{
			get;
			set;
		}

		internal bool TypeHasBeenSet
		{
			get
			{
				return !this.inferType;
			}
		}

		internal string BaseName
		{
			get
			{
				if (this.ParameterName.StartsWith("@", StringComparison.Ordinal) || this.ParameterName.StartsWith("?", StringComparison.Ordinal))
				{
					return this.ParameterName.Substring(1);
				}
				return this.ParameterName;
			}
		}

		[Category("Data")]
		public override ParameterDirection Direction
		{
			get;
			set;
		}

		[Browsable(false)]
		public override bool IsNullable
		{
			get;
			set;
		}

		[Category("Data"), DbProviderSpecificTypeProperty(true)]
		public MySqlDbType MySqlDbType
		{
			get
			{
				return this.mySqlDbType;
			}
			set
			{
				this.SetMySqlDbType(value);
				this.inferType = false;
			}
		}

		[Category("Data")]
		public new byte Precision
		{
			get;
			set;
		}

		[Category("Data")]
		public new byte Scale
		{
			get;
			set;
		}

		[Category("Data")]
		public override int Size
		{
			get;
			set;
		}

		[Category("Data"), TypeConverter(typeof(StringConverter))]
		public override object Value
		{
			get
			{
				return this.paramValue;
			}
			set
			{
				this.paramValue = value;
				byte[] array = value as byte[];
				string text = value as string;
				if (array != null)
				{
					this.Size = array.Length;
				}
				else if (text != null)
				{
					this.Size = text.Length;
				}
				if (this.inferType)
				{
					this.SetTypeFromValue();
				}
			}
		}

		internal IMySqlValue ValueObject
		{
			get
			{
				return this._valueObject;
			}
			private set
			{
				this._valueObject = value;
			}
		}

		public IList PossibleValues
		{
			get;
			internal set;
		}

		public MySqlParameter(string parameterName, MySqlDbType dbType, int size, string sourceColumn) : this(parameterName, dbType)
		{
			this.Size = size;
			this.Direction = ParameterDirection.Input;
			this.SourceColumn = sourceColumn;
			this.SourceVersion = DataRowVersion.Current;
		}

		public MySqlParameter(string parameterName, MySqlDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value) : this(parameterName, dbType, size, sourceColumn)
		{
			this.Direction = direction;
			this.SourceVersion = sourceVersion;
			this.IsNullable = isNullable;
			this.Precision = precision;
			this.Scale = scale;
			this.Value = value;
		}

		internal MySqlParameter(string name, MySqlDbType type, ParameterDirection dir, string col, DataRowVersion ver, object val) : this(name, type)
		{
			this.Direction = dir;
			this.SourceColumn = col;
			this.SourceVersion = ver;
			this.Value = val;
		}

		private void Init()
		{
			this.SourceVersion = DataRowVersion.Current;
			this.Direction = ParameterDirection.Input;
		}

		public override void ResetDbType()
		{
			this.inferType = true;
		}

		private void SetDbTypeFromMySqlDbType()
		{
			MySqlDbType mySqlDbType = this.mySqlDbType;
			if (mySqlDbType <= MySqlDbType.String)
			{
				switch (mySqlDbType)
				{
				case MySqlDbType.Decimal:
					break;
				case MySqlDbType.Byte:
					this.dbType = DbType.SByte;
					return;
				case MySqlDbType.Int16:
					this.dbType = DbType.Int16;
					return;
				case MySqlDbType.Int32:
				case MySqlDbType.Int24:
					this.dbType = DbType.Int32;
					return;
				case MySqlDbType.Float:
					this.dbType = DbType.Single;
					return;
				case MySqlDbType.Double:
					this.dbType = DbType.Double;
					return;
				case (MySqlDbType)6:
				case MySqlDbType.VarString:
					return;
				case MySqlDbType.Timestamp:
				case MySqlDbType.DateTime:
					this.dbType = DbType.DateTime;
					return;
				case MySqlDbType.Int64:
					this.dbType = DbType.Int64;
					return;
				case MySqlDbType.Date:
				case MySqlDbType.Year:
				case MySqlDbType.Newdate:
					this.dbType = DbType.Date;
					return;
				case MySqlDbType.Time:
					this.dbType = DbType.Time;
					return;
				case MySqlDbType.Bit:
					this.dbType = DbType.UInt64;
					return;
				default:
					switch (mySqlDbType)
					{
					case MySqlDbType.NewDecimal:
						break;
					case MySqlDbType.Enum:
					case MySqlDbType.Set:
					case MySqlDbType.VarChar:
						this.dbType = DbType.String;
						return;
					case MySqlDbType.TinyBlob:
					case MySqlDbType.MediumBlob:
					case MySqlDbType.LongBlob:
					case MySqlDbType.Blob:
						this.dbType = DbType.Object;
						return;
					case MySqlDbType.String:
						this.dbType = DbType.StringFixedLength;
						return;
					default:
						return;
					}
					break;
				}
				this.dbType = DbType.Decimal;
				return;
			}
			switch (mySqlDbType)
			{
			case MySqlDbType.UByte:
				this.dbType = DbType.Byte;
				return;
			case MySqlDbType.UInt16:
				this.dbType = DbType.UInt16;
				return;
			case MySqlDbType.UInt32:
			case MySqlDbType.UInt24:
				this.dbType = DbType.UInt32;
				return;
			case (MySqlDbType)504:
			case (MySqlDbType)505:
			case (MySqlDbType)506:
			case (MySqlDbType)507:
				break;
			case MySqlDbType.UInt64:
				this.dbType = DbType.UInt64;
				return;
			default:
				if (mySqlDbType != MySqlDbType.Guid)
				{
					return;
				}
				this.dbType = DbType.Guid;
				break;
			}
		}

		private void SetDbType(DbType db_type)
		{
			this.dbType = db_type;
			switch (this.dbType)
			{
			case DbType.AnsiString:
			case DbType.String:
				this.mySqlDbType = MySqlDbType.VarChar;
				goto IL_14B;
			case DbType.Byte:
			case DbType.Boolean:
				this.mySqlDbType = MySqlDbType.UByte;
				goto IL_14B;
			case DbType.Currency:
			case DbType.Decimal:
				this.mySqlDbType = MySqlDbType.Decimal;
				goto IL_14B;
			case DbType.Date:
				this.mySqlDbType = MySqlDbType.Date;
				goto IL_14B;
			case DbType.DateTime:
				this.mySqlDbType = MySqlDbType.DateTime;
				goto IL_14B;
			case DbType.Double:
				this.mySqlDbType = MySqlDbType.Double;
				goto IL_14B;
			case DbType.Guid:
				this.mySqlDbType = MySqlDbType.Guid;
				goto IL_14B;
			case DbType.Int16:
				this.mySqlDbType = MySqlDbType.Int16;
				goto IL_14B;
			case DbType.Int32:
				this.mySqlDbType = MySqlDbType.Int32;
				goto IL_14B;
			case DbType.Int64:
				this.mySqlDbType = MySqlDbType.Int64;
				goto IL_14B;
			case DbType.SByte:
				this.mySqlDbType = MySqlDbType.Byte;
				goto IL_14B;
			case DbType.Single:
				this.mySqlDbType = MySqlDbType.Float;
				goto IL_14B;
			case DbType.Time:
				this.mySqlDbType = MySqlDbType.Time;
				goto IL_14B;
			case DbType.UInt16:
				this.mySqlDbType = MySqlDbType.UInt16;
				goto IL_14B;
			case DbType.UInt32:
				this.mySqlDbType = MySqlDbType.UInt32;
				goto IL_14B;
			case DbType.UInt64:
				this.mySqlDbType = MySqlDbType.UInt64;
				goto IL_14B;
			case DbType.AnsiStringFixedLength:
			case DbType.StringFixedLength:
				this.mySqlDbType = MySqlDbType.String;
				goto IL_14B;
			}
			this.mySqlDbType = MySqlDbType.Blob;
			IL_14B:
			if (this.dbType == DbType.Object)
			{
				byte[] array = this.paramValue as byte[];
				if (array != null && array.Length == 25)
				{
					this.mySqlDbType = MySqlDbType.Geometry;
				}
			}
			this.ValueObject = MySqlField.GetIMySqlValue(this.mySqlDbType);
		}

		public MySqlParameter()
		{
			this.Init();
		}

		public MySqlParameter(string parameterName, object value) : this()
		{
			this.ParameterName = parameterName;
			this.Value = value;
		}

		public MySqlParameter(string parameterName, MySqlDbType dbType) : this(parameterName, null)
		{
			this.MySqlDbType = dbType;
		}

		public MySqlParameter(string parameterName, MySqlDbType dbType, int size) : this(parameterName, dbType)
		{
			this.Size = size;
		}

		private void SetParameterName(string name)
		{
			if (this.Collection != null)
			{
				this.Collection.ParameterNameChanged(this, this.paramName, name);
			}
			this.paramName = name;
		}

		public override string ToString()
		{
			return this.paramName;
		}

		internal int GetPSType()
		{
			MySqlDbType mySqlDbType = this.mySqlDbType;
			if (mySqlDbType != MySqlDbType.Bit)
			{
				switch (mySqlDbType)
				{
				case MySqlDbType.UByte:
					return 32769;
				case MySqlDbType.UInt16:
					return 32770;
				case MySqlDbType.UInt32:
					return 32771;
				case MySqlDbType.UInt64:
					return 32776;
				case MySqlDbType.UInt24:
					return 32771;
				}
				return (int)this.mySqlDbType;
			}
			return 32776;
		}

		internal void Serialize(MySqlPacket packet, bool binary, MySqlConnectionStringBuilder settings)
		{
			if (!binary && (this.paramValue == null || this.paramValue == DBNull.Value))
			{
				packet.WriteStringNoNull("NULL");
				return;
			}
			if (this.ValueObject.MySqlDbType == MySqlDbType.Guid)
			{
				MySqlGuid mySqlGuid = (MySqlGuid)this.ValueObject;
				mySqlGuid.OldGuids = settings.OldGuids;
				this.ValueObject = mySqlGuid;
			}
			if (this.ValueObject.MySqlDbType == MySqlDbType.Geometry)
			{
				MySqlGeometry mySqlGeometry = (MySqlGeometry)this.ValueObject;
				this.ValueObject = mySqlGeometry;
			}
			this.ValueObject.WriteValue(packet, binary, this.paramValue, this.Size);
		}

		private void SetMySqlDbType(MySqlDbType mysql_dbtype)
		{
			this.mySqlDbType = mysql_dbtype;
			this.ValueObject = MySqlField.GetIMySqlValue(this.mySqlDbType);
			this.SetDbTypeFromMySqlDbType();
		}

		private void SetTypeFromValue()
		{
			if (this.paramValue == null || this.paramValue == DBNull.Value)
			{
				return;
			}
			if (this.paramValue is Guid)
			{
				this.MySqlDbType = MySqlDbType.Guid;
				return;
			}
			if (this.paramValue is TimeSpan)
			{
				this.MySqlDbType = MySqlDbType.Time;
				return;
			}
			if (this.paramValue is bool)
			{
				this.MySqlDbType = MySqlDbType.Byte;
				return;
			}
			Type type = this.paramValue.GetType();
			string name;
			switch (name = type.Name)
			{
			case "SByte":
				this.MySqlDbType = MySqlDbType.Byte;
				return;
			case "Byte":
				this.MySqlDbType = MySqlDbType.UByte;
				return;
			case "Int16":
				this.MySqlDbType = MySqlDbType.Int16;
				return;
			case "UInt16":
				this.MySqlDbType = MySqlDbType.UInt16;
				return;
			case "Int32":
				this.MySqlDbType = MySqlDbType.Int32;
				return;
			case "UInt32":
				this.MySqlDbType = MySqlDbType.UInt32;
				return;
			case "Int64":
				this.MySqlDbType = MySqlDbType.Int64;
				return;
			case "UInt64":
				this.MySqlDbType = MySqlDbType.UInt64;
				return;
			case "DateTime":
				this.MySqlDbType = MySqlDbType.DateTime;
				return;
			case "String":
				this.MySqlDbType = MySqlDbType.VarChar;
				return;
			case "Single":
				this.MySqlDbType = MySqlDbType.Float;
				return;
			case "Double":
				this.MySqlDbType = MySqlDbType.Double;
				return;
			case "Decimal":
				this.MySqlDbType = MySqlDbType.Decimal;
				return;
			}
			if (type.BaseType == typeof(Enum))
			{
				this.MySqlDbType = MySqlDbType.Int32;
				return;
			}
			this.MySqlDbType = MySqlDbType.Blob;
		}

		public MySqlParameter Clone()
		{
			return new MySqlParameter(this.paramName, this.mySqlDbType, this.Direction, this.SourceColumn, this.SourceVersion, this.paramValue)
			{
				inferType = this.inferType
			};
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		internal long EstimatedSize()
		{
			if (this.Value == null || this.Value == DBNull.Value)
			{
				return 4L;
			}
			if (this.Value is byte[])
			{
				return (long)(this.Value as byte[]).Length;
			}
			if (this.Value is string)
			{
				return (long)((this.Value as string).Length * 4);
			}
			if (this.Value is decimal || this.Value is float)
			{
				return 64L;
			}
			return 32L;
		}
	}
}
