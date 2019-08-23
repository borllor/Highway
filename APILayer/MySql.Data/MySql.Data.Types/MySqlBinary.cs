using MySql.Data.MySqlClient;
using System;

namespace MySql.Data.Types
{
	internal struct MySqlBinary : IMySqlValue
	{
		private MySqlDbType type;

		private byte[] mValue;

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
				return this.type;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public byte[] Value
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
				return typeof(byte[]);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				switch (this.type)
				{
				case MySqlDbType.TinyBlob:
					return "TINY_BLOB";
				case MySqlDbType.MediumBlob:
					return "MEDIUM_BLOB";
				case MySqlDbType.LongBlob:
					return "LONG_BLOB";
				}
				return "BLOB";
			}
		}

		public MySqlBinary(MySqlDbType type, bool isNull)
		{
			this.type = type;
			this.isNull = isNull;
			this.mValue = null;
		}

		public MySqlBinary(MySqlDbType type, byte[] val)
		{
			this.type = type;
			this.isNull = false;
			this.mValue = val;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			byte[] array = val as byte[];
			if (array == null)
			{
				char[] array2 = val as char[];
				if (array2 != null)
				{
					array = packet.Encoding.GetBytes(array2);
				}
				else
				{
					string text = val.ToString();
					if (length == 0)
					{
						length = text.Length;
					}
					else
					{
						text = text.Substring(0, length);
					}
					array = packet.Encoding.GetBytes(text);
				}
			}
			if (length == 0)
			{
				length = array.Length;
			}
			if (array == null)
			{
				throw new MySqlException("Only byte arrays and strings can be serialized by MySqlBinary");
			}
			if (binary)
			{
				packet.WriteLength((long)length);
				packet.Write(array, 0, length);
				return;
			}
			packet.WriteStringNoNull("_binary ");
			packet.WriteByte(39);
			MySqlBinary.EscapeByteArray(array, length, packet);
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
			MySqlBinary mySqlBinary;
			if (nullVal)
			{
				mySqlBinary = new MySqlBinary(this.type, true);
			}
			else
			{
				if (length == -1L)
				{
					length = packet.ReadFieldLength();
				}
				byte[] array = new byte[length];
				packet.Read(array, 0, (int)length);
				mySqlBinary = new MySqlBinary(this.type, array);
			}
			return mySqlBinary;
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadFieldLength();
			packet.Position += num;
		}

		public static void SetDSInfo(MySqlSchemaCollection sc)
		{
			string[] array = new string[]
			{
				"BLOB",
				"TINYBLOB",
				"MEDIUMBLOB",
				"LONGBLOB",
				"BINARY",
				"VARBINARY"
			};
			MySqlDbType[] array2 = new MySqlDbType[]
			{
				MySqlDbType.Blob,
				MySqlDbType.TinyBlob,
				MySqlDbType.MediumBlob,
				MySqlDbType.LongBlob,
				MySqlDbType.Binary,
				MySqlDbType.VarBinary
			};
			long[] array3 = new long[]
			{
				65535L,
				255L,
				16777215L,
				4294967295L,
				255L,
				65535L
			};
			string[] array4 = new string[]
			{
				null,
				null,
				null,
				null,
				"binary({0})",
				"varbinary({0})"
			};
			string[] array5 = new string[]
			{
				null,
				null,
				null,
				null,
				"length",
				"length"
			};
			for (int i = 0; i < array.Length; i++)
			{
				MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
				mySqlSchemaRow["TypeName"] = array[i];
				mySqlSchemaRow["ProviderDbType"] = array2[i];
				mySqlSchemaRow["ColumnSize"] = array3[i];
				mySqlSchemaRow["CreateFormat"] = array4[i];
				mySqlSchemaRow["CreateParameters"] = array5[i];
				mySqlSchemaRow["DataType"] = "System.Byte[]";
				mySqlSchemaRow["IsAutoincrementable"] = false;
				mySqlSchemaRow["IsBestMatch"] = true;
				mySqlSchemaRow["IsCaseSensitive"] = false;
				mySqlSchemaRow["IsFixedLength"] = (i >= 4);
				mySqlSchemaRow["IsFixedPrecisionScale"] = false;
				mySqlSchemaRow["IsLong"] = (array3[i] > 255L);
				mySqlSchemaRow["IsNullable"] = true;
				mySqlSchemaRow["IsSearchable"] = false;
				mySqlSchemaRow["IsSearchableWithLike"] = false;
				mySqlSchemaRow["IsUnsigned"] = DBNull.Value;
				mySqlSchemaRow["MaximumScale"] = DBNull.Value;
				mySqlSchemaRow["MinimumScale"] = DBNull.Value;
				mySqlSchemaRow["IsConcurrencyType"] = DBNull.Value;
				mySqlSchemaRow["IsLiteralSupported"] = false;
				mySqlSchemaRow["LiteralPrefix"] = "0x";
				mySqlSchemaRow["LiteralSuffix"] = DBNull.Value;
				mySqlSchemaRow["NativeDataType"] = DBNull.Value;
			}
		}
	}
}
