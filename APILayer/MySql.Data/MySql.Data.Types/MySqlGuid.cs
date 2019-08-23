using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Properties;
using System;

namespace MySql.Data.Types
{
	internal struct MySqlGuid : IMySqlValue
	{
		private Guid mValue;

		private bool isNull;

		private byte[] bytes;

		private bool oldGuids;

		public byte[] Bytes
		{
			get
			{
				return this.bytes;
			}
		}

		public bool OldGuids
		{
			get
			{
				return this.oldGuids;
			}
			set
			{
				this.oldGuids = value;
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
				return MySqlDbType.Guid;
			}
		}

		object IMySqlValue.Value
		{
			get
			{
				return this.mValue;
			}
		}

		public Guid Value
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
				return typeof(Guid);
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				if (!this.OldGuids)
				{
					return "CHAR(36)";
				}
				return "BINARY(16)";
			}
		}

		public MySqlGuid(byte[] buff)
		{
			this.oldGuids = false;
			this.mValue = new Guid(buff);
			this.isNull = false;
			this.bytes = buff;
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			Guid guid = Guid.Empty;
			string text = val as string;
			byte[] array = val as byte[];
			if (val is Guid)
			{
				guid = (Guid)val;
			}
			else
			{
				try
				{
					if (text != null)
					{
						guid = new Guid(text);
					}
					else if (array != null)
					{
						guid = new Guid(array);
					}
				}
				catch (Exception ex)
				{
					throw new MySqlException(Resources.DataNotInSupportedFormat, ex);
				}
			}
			if (this.OldGuids)
			{
				this.WriteOldGuid(packet, guid, binary);
				return;
			}
			guid.ToString("D");
			if (binary)
			{
				packet.WriteLenString(guid.ToString("D"));
				return;
			}
			packet.WriteStringNoNull("'" + MySqlHelper.EscapeString(guid.ToString("D")) + "'");
		}

		private void WriteOldGuid(MySqlPacket packet, Guid guid, bool binary)
		{
			byte[] array = guid.ToByteArray();
			if (binary)
			{
				packet.WriteLength((long)array.Length);
				packet.Write(array);
				return;
			}
			packet.WriteStringNoNull("_binary ");
			packet.WriteByte(39);
			MySqlGuid.EscapeByteArray(array, array.Length, packet);
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

		private MySqlGuid ReadOldGuid(MySqlPacket packet, long length)
		{
			if (length == -1L)
			{
				length = packet.ReadFieldLength();
			}
			byte[] array = new byte[length];
			packet.Read(array, 0, (int)length);
			return new MySqlGuid(array)
			{
				OldGuids = this.OldGuids
			};
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			MySqlGuid mySqlGuid = default(MySqlGuid);
			mySqlGuid.isNull = true;
			mySqlGuid.OldGuids = this.OldGuids;
			if (!nullVal)
			{
				if (this.OldGuids)
				{
					return this.ReadOldGuid(packet, length);
				}
				string g = string.Empty;
				if (length == -1L)
				{
					g = packet.ReadLenString();
				}
				else
				{
					g = packet.ReadString(length);
				}
				mySqlGuid.mValue = new Guid(g);
				mySqlGuid.isNull = false;
			}
			return mySqlGuid;
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
			int num = (int)packet.ReadFieldLength();
			packet.Position += num;
		}

		public static void SetDSInfo(MySqlSchemaCollection sc)
		{
			MySqlSchemaRow mySqlSchemaRow = sc.AddRow();
			mySqlSchemaRow["TypeName"] = "GUID";
			mySqlSchemaRow["ProviderDbType"] = MySqlDbType.Guid;
			mySqlSchemaRow["ColumnSize"] = 0;
			mySqlSchemaRow["CreateFormat"] = "BINARY(16)";
			mySqlSchemaRow["CreateParameters"] = null;
			mySqlSchemaRow["DataType"] = "System.Guid";
			mySqlSchemaRow["IsAutoincrementable"] = false;
			mySqlSchemaRow["IsBestMatch"] = true;
			mySqlSchemaRow["IsCaseSensitive"] = false;
			mySqlSchemaRow["IsFixedLength"] = true;
			mySqlSchemaRow["IsFixedPrecisionScale"] = true;
			mySqlSchemaRow["IsLong"] = false;
			mySqlSchemaRow["IsNullable"] = true;
			mySqlSchemaRow["IsSearchable"] = false;
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
