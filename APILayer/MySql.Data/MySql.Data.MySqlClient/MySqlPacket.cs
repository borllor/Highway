using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using System;
using System.IO;
using System.Text;

namespace MySql.Data.MySqlClient
{
	internal class MySqlPacket
	{
		private byte[] tempBuffer = new byte[256];

		private Encoding encoding;

		private MemoryStream buffer = new MemoryStream(5);

		private DBVersion version;

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public bool HasMoreData
		{
			get
			{
				return this.buffer.Position < this.buffer.Length;
			}
		}

		public int Position
		{
			get
			{
				return (int)this.buffer.Position;
			}
			set
			{
				this.buffer.Position = (long)value;
			}
		}

		public int Length
		{
			get
			{
				return (int)this.buffer.Length;
			}
			set
			{
				this.buffer.SetLength((long)value);
			}
		}

		public bool IsLastPacket
		{
			get
			{
				byte[] array = this.buffer.GetBuffer();
				return array[0] == 254 && this.Length <= 5;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this.buffer.GetBuffer();
			}
		}

		public DBVersion Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		private MySqlPacket()
		{
			this.Clear();
		}

		public MySqlPacket(Encoding enc) : this()
		{
			this.Encoding = enc;
		}

		public MySqlPacket(MemoryStream stream) : this()
		{
			this.buffer = stream;
		}

		public void Clear()
		{
			this.Position = 4;
		}

		public byte ReadByte()
		{
			return (byte)this.buffer.ReadByte();
		}

		public int Read(byte[] byteBuffer, int offset, int count)
		{
			return this.buffer.Read(byteBuffer, offset, count);
		}

		public void WriteByte(byte b)
		{
			this.buffer.WriteByte(b);
		}

		public void Write(byte[] bytesToWrite)
		{
			this.Write(bytesToWrite, 0, bytesToWrite.Length);
		}

		public void Write(byte[] bytesToWrite, int offset, int countToWrite)
		{
			this.buffer.Write(bytesToWrite, offset, countToWrite);
		}

		public int ReadNBytes()
		{
			byte b = this.ReadByte();
			if (b < 1 || b > 4)
			{
				throw new MySqlException(Resources.IncorrectTransmission);
			}
			return this.ReadInteger((int)b);
		}

		public void SetByte(long position, byte value)
		{
			long position2 = this.buffer.Position;
			this.buffer.Position = position;
			this.buffer.WriteByte(value);
			this.buffer.Position = position2;
		}

		public long ReadFieldLength()
		{
			byte b = this.ReadByte();
			switch (b)
			{
			case 251:
				return -1L;
			case 252:
				return (long)this.ReadInteger(2);
			case 253:
				return (long)this.ReadInteger(3);
			case 254:
				return this.ReadLong(8);
			default:
				return (long)((ulong)b);
			}
		}

		public ulong ReadBitValue(int numbytes)
		{
			ulong num = 0uL;
			int num2 = (int)this.buffer.Position;
			byte[] array = this.buffer.GetBuffer();
			int num3 = 0;
			for (int i = 0; i < numbytes; i++)
			{
				num <<= num3;
				num |= (ulong)array[num2++];
				num3 = 8;
			}
			this.buffer.Position += (long)numbytes;
			return num;
		}

		public long ReadLong(int numbytes)
		{
			byte[] value = this.buffer.GetBuffer();
			int startIndex = (int)this.buffer.Position;
			this.buffer.Position += (long)numbytes;
			switch (numbytes)
			{
			case 2:
				return (long)((ulong)BitConverter.ToUInt16(value, startIndex));
			case 3:
				break;
			case 4:
				return (long)((ulong)BitConverter.ToUInt32(value, startIndex));
			default:
				if (numbytes == 8)
				{
					return BitConverter.ToInt64(value, startIndex);
				}
				break;
			}
			throw new NotSupportedException("Only byte lengths of 2, 4, or 8 are supported");
		}

		public ulong ReadULong(int numbytes)
		{
			byte[] value = this.buffer.GetBuffer();
			int startIndex = (int)this.buffer.Position;
			this.buffer.Position += (long)numbytes;
			switch (numbytes)
			{
			case 2:
				return (ulong)BitConverter.ToUInt16(value, startIndex);
			case 3:
				break;
			case 4:
				return (ulong)BitConverter.ToUInt32(value, startIndex);
			default:
				if (numbytes == 8)
				{
					return BitConverter.ToUInt64(value, startIndex);
				}
				break;
			}
			throw new NotSupportedException("Only byte lengths of 2, 4, or 8 are supported");
		}

		public int Read3ByteInt()
		{
			int num = 0;
			int num2 = (int)this.buffer.Position;
			byte[] array = this.buffer.GetBuffer();
			int num3 = 0;
			for (int i = 0; i < 3; i++)
			{
				num |= (int)array[num2++] << (num3 & 31);
				num3 += 8;
			}
			this.buffer.Position += 3L;
			return num;
		}

		public int ReadInteger(int numbytes)
		{
			if (numbytes == 3)
			{
				return this.Read3ByteInt();
			}
			return (int)this.ReadLong(numbytes);
		}

		public void WriteInteger(long v, int numbytes)
		{
			long num = v;
			for (int i = 0; i < numbytes; i++)
			{
				this.tempBuffer[i] = (byte)(num & 255L);
				num >>= 8;
			}
			this.Write(this.tempBuffer, 0, numbytes);
		}

		public int ReadPackedInteger()
		{
			byte result = this.ReadByte();
			switch (result)
			{
			case 251:
				return -1;
			case 252:
				return this.ReadInteger(2);
			case 253:
				return this.ReadInteger(3);
			case 254:
				return this.ReadInteger(4);
			default:
				return (int)result;
			}
		}

		public void WriteLength(long length)
		{
			if (length < 251L)
			{
				this.WriteByte((byte)length);
				return;
			}
			if (length < 65536L)
			{
				this.WriteByte(252);
				this.WriteInteger(length, 2);
				return;
			}
			if (length < 16777216L)
			{
				this.WriteByte(253);
				this.WriteInteger(length, 3);
				return;
			}
			this.WriteByte(254);
			this.WriteInteger(length, 4);
		}

		public void WriteLenString(string s)
		{
			byte[] bytes = this.encoding.GetBytes(s);
			this.WriteLength((long)bytes.Length);
			this.Write(bytes, 0, bytes.Length);
		}

		public void WriteStringNoNull(string v)
		{
			byte[] bytes = this.encoding.GetBytes(v);
			this.Write(bytes, 0, bytes.Length);
		}

		public void WriteString(string v)
		{
			this.WriteStringNoNull(v);
			this.WriteByte(0);
		}

		public string ReadLenString()
		{
			long length = (long)this.ReadPackedInteger();
			return this.ReadString(length);
		}

		public string ReadAsciiString(long length)
		{
			if (length == 0L)
			{
				return string.Empty;
			}
			this.Read(this.tempBuffer, 0, (int)length);
			return Encoding.GetEncoding("us-ascii").GetString(this.tempBuffer, 0, (int)length);
		}

		public string ReadString(long length)
		{
			if (length == 0L)
			{
				return string.Empty;
			}
			if (this.tempBuffer == null || length > (long)this.tempBuffer.Length)
			{
				this.tempBuffer = new byte[length];
			}
			this.Read(this.tempBuffer, 0, (int)length);
			return this.encoding.GetString(this.tempBuffer, 0, (int)length);
		}

		public string ReadString()
		{
			return this.ReadString(this.encoding);
		}

		public string ReadString(Encoding theEncoding)
		{
			byte[] array = this.buffer.GetBuffer();
			int num = (int)this.buffer.Position;
			while (num < (int)this.buffer.Length && array[num] != 0 && (int)array[num] != -1)
			{
				num++;
			}
			string @string = theEncoding.GetString(array, (int)this.buffer.Position, num - (int)this.buffer.Position);
			this.buffer.Position = (long)(num + 1);
			return @string;
		}
	}
}
