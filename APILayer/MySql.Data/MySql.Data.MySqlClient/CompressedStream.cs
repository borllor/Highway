using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using System;
using System.IO;
using zlib;

namespace MySql.Data.MySqlClient
{
	internal class CompressedStream : Stream
	{
		private Stream baseStream;

		private MemoryStream cache;

		private byte[] localByte;

		private byte[] inBuffer;

		private byte[] lengthBytes;

		private WeakReference inBufferRef;

		private int inPos;

		private int maxInPos;

		private ZInputStream zInStream;

		public override bool CanRead
		{
			get
			{
				return this.baseStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.baseStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.baseStream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				return this.baseStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.baseStream.Position;
			}
			set
			{
				this.baseStream.Position = value;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return this.baseStream.CanTimeout;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.baseStream.ReadTimeout;
			}
			set
			{
				this.baseStream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.baseStream.WriteTimeout;
			}
			set
			{
				this.baseStream.WriteTimeout = value;
			}
		}

		public CompressedStream(Stream baseStream)
		{
			this.baseStream = baseStream;
			this.localByte = new byte[1];
			this.lengthBytes = new byte[7];
			this.cache = new MemoryStream();
			this.inBufferRef = new WeakReference(this.inBuffer, false);
		}

		public override void Close()
		{
			base.Close();
			this.baseStream.Close();
			this.cache.Dispose();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(Resources.CSNoSetLength);
		}

		public override int ReadByte()
		{
			int result;
			try
			{
				this.Read(this.localByte, 0, 1);
				result = (int)this.localByte[0];
			}
			catch (EndOfStreamException)
			{
				result = -1;
			}
			return result;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Resources.BufferCannotBeNull);
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", Resources.OffsetMustBeValid);
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentException(Resources.BufferNotLargeEnough, "buffer");
			}
			if (this.inPos == this.maxInPos)
			{
				this.PrepareNextPacket();
			}
			int num = Math.Min(count, this.maxInPos - this.inPos);
			int num2;
			if (this.zInStream != null)
			{
				num2 = this.zInStream.read(buffer, offset, num);
			}
			else
			{
				num2 = this.baseStream.Read(buffer, offset, num);
			}
			this.inPos += num2;
			if (this.inPos == this.maxInPos)
			{
				this.zInStream = null;
				if (!Platform.IsMono())
				{
					this.inBufferRef = new WeakReference(this.inBuffer, false);
					this.inBuffer = null;
				}
			}
			return num2;
		}

		private void PrepareNextPacket()
		{
			MySqlStream.ReadFully(this.baseStream, this.lengthBytes, 0, 7);
			int num = (int)this.lengthBytes[0] + ((int)this.lengthBytes[1] << 8) + ((int)this.lengthBytes[2] << 16);
			int num2 = (int)this.lengthBytes[4] + ((int)this.lengthBytes[5] << 8) + ((int)this.lengthBytes[6] << 16);
			if (num2 == 0)
			{
				num2 = num;
				this.zInStream = null;
			}
			else
			{
				this.ReadNextPacket(num);
				MemoryStream in_Renamed = new MemoryStream(this.inBuffer);
				this.zInStream = new ZInputStream(in_Renamed);
				this.zInStream.maxInput = (long)num;
			}
			this.inPos = 0;
			this.maxInPos = num2;
		}

		private void ReadNextPacket(int len)
		{
			if (!Platform.IsMono())
			{
				this.inBuffer = (this.inBufferRef.Target as byte[]);
			}
			if (this.inBuffer == null || this.inBuffer.Length < len)
			{
				this.inBuffer = new byte[len];
			}
			MySqlStream.ReadFully(this.baseStream, this.inBuffer, 0, len);
		}

		private MemoryStream CompressCache()
		{
			if (this.cache.Length < 50L)
			{
				return null;
			}
			byte[] buffer = this.cache.GetBuffer();
			MemoryStream memoryStream = new MemoryStream();
			ZOutputStream zOutputStream = new ZOutputStream(memoryStream, -1);
			zOutputStream.Write(buffer, 0, (int)this.cache.Length);
			zOutputStream.finish();
			if (memoryStream.Length >= this.cache.Length)
			{
				return null;
			}
			return memoryStream;
		}

		private void CompressAndSendCache()
		{
			byte[] buffer = this.cache.GetBuffer();
			byte b = buffer[3];
			buffer[3] = 0;
			MemoryStream memoryStream = this.CompressCache();
			long length;
			long num;
			MemoryStream memoryStream2;
			if (memoryStream == null)
			{
				length = this.cache.Length;
				num = 0L;
				memoryStream2 = this.cache;
			}
			else
			{
				length = memoryStream.Length;
				num = this.cache.Length;
				memoryStream2 = memoryStream;
			}
			long length2 = memoryStream2.Length;
			int num2 = (int)length2 + 7;
			memoryStream2.SetLength((long)num2);
			byte[] buffer2 = memoryStream2.GetBuffer();
			Array.Copy(buffer2, 0, buffer2, 7, (int)length2);
			buffer2[0] = (byte)(length & 255L);
			buffer2[1] = (byte)(length >> 8 & 255L);
			buffer2[2] = (byte)(length >> 16 & 255L);
			buffer2[3] = b;
			buffer2[4] = (byte)(num & 255L);
			buffer2[5] = (byte)(num >> 8 & 255L);
			buffer2[6] = (byte)(num >> 16 & 255L);
			this.baseStream.Write(buffer2, 0, num2);
			this.baseStream.Flush();
			this.cache.SetLength(0L);
			if (memoryStream != null)
			{
				memoryStream.Dispose();
			}
		}

		public override void Flush()
		{
			if (!this.InputDone())
			{
				return;
			}
			this.CompressAndSendCache();
		}

		private bool InputDone()
		{
			if (this.baseStream is TimedStream && ((TimedStream)this.baseStream).IsClosed)
			{
				return false;
			}
			if (this.cache.Length < 4L)
			{
				return false;
			}
			byte[] buffer = this.cache.GetBuffer();
			int num = (int)buffer[0] + ((int)buffer[1] << 8) + ((int)buffer[2] << 16);
			return this.cache.Length >= (long)(num + 4);
		}

		public override void WriteByte(byte value)
		{
			this.cache.WriteByte(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.cache.Write(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.baseStream.Seek(offset, origin);
		}
	}
}
