using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace MySql.Data.Common
{
	internal class SharedMemoryStream : Stream
	{
		private const int BUFFERLENGTH = 16004;

		private string memoryName;

		private EventWaitHandle serverRead;

		private EventWaitHandle serverWrote;

		private EventWaitHandle clientRead;

		private EventWaitHandle clientWrote;

		private EventWaitHandle connectionClosed;

		private SharedMemory data;

		private int bytesLeft;

		private int position;

		private int connectNumber;

		private int readTimeout = -1;

		private int writeTimeout = -1;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("SharedMemoryStream does not support seeking - length");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("SharedMemoryStream does not support seeking - position");
			}
			set
			{
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.readTimeout;
			}
			set
			{
				this.readTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.writeTimeout;
			}
			set
			{
				this.writeTimeout = value;
			}
		}

		public SharedMemoryStream(string memName)
		{
			this.memoryName = memName;
		}

		public void Open(uint timeOut)
		{
			EventWaitHandle arg_06_0 = this.connectionClosed;
			this.GetConnectNumber(timeOut);
			this.SetupEvents();
		}

		public override void Close()
		{
			if (this.connectionClosed != null)
			{
				if (!this.connectionClosed.WaitOne(0))
				{
					this.connectionClosed.Set();
					this.connectionClosed.Close();
				}
				this.connectionClosed = null;
				EventWaitHandle[] array = new EventWaitHandle[]
				{
					this.serverRead,
					this.serverWrote,
					this.clientRead,
					this.clientWrote
				};
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						array[i].Close();
					}
				}
				if (this.data != null)
				{
					this.data.Dispose();
					this.data = null;
				}
			}
		}

		private void GetConnectNumber(uint timeOut)
		{
			EventWaitHandle eventWaitHandle;
			try
			{
				eventWaitHandle = EventWaitHandle.OpenExisting(this.memoryName + "_CONNECT_REQUEST");
			}
			catch (Exception)
			{
				string str = "Global\\" + this.memoryName;
				eventWaitHandle = EventWaitHandle.OpenExisting(str + "_CONNECT_REQUEST");
				this.memoryName = str;
			}
			EventWaitHandle eventWaitHandle2 = EventWaitHandle.OpenExisting(this.memoryName + "_CONNECT_ANSWER");
			using (SharedMemory sharedMemory = new SharedMemory(this.memoryName + "_CONNECT_DATA", (IntPtr)4))
			{
				if (!eventWaitHandle.Set())
				{
					throw new MySqlException("Failed to open shared memory connection");
				}
				if (!eventWaitHandle2.WaitOne((int)(timeOut * 1000u), false))
				{
					throw new MySqlException("Timeout during connection");
				}
				this.connectNumber = Marshal.ReadInt32(sharedMemory.View);
			}
		}

		private void SetupEvents()
		{
			string str = this.memoryName + "_" + this.connectNumber;
			this.data = new SharedMemory(str + "_DATA", (IntPtr)16004);
			this.serverWrote = EventWaitHandle.OpenExisting(str + "_SERVER_WROTE");
			this.serverRead = EventWaitHandle.OpenExisting(str + "_SERVER_READ");
			this.clientWrote = EventWaitHandle.OpenExisting(str + "_CLIENT_WROTE");
			this.clientRead = EventWaitHandle.OpenExisting(str + "_CLIENT_READ");
			this.connectionClosed = EventWaitHandle.OpenExisting(str + "_CONNECTION_CLOSED");
			this.serverRead.Set();
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this.readTimeout;
			WaitHandle[] array = new WaitHandle[]
			{
				this.serverWrote,
				this.connectionClosed
			};
			LowResolutionStopwatch lowResolutionStopwatch = new LowResolutionStopwatch();
			while (this.bytesLeft == 0)
			{
				lowResolutionStopwatch.Start();
				int num2 = WaitHandle.WaitAny(array, num);
				lowResolutionStopwatch.Stop();
				if (num2 == 258)
				{
					throw new TimeoutException("Timeout when reading from shared memory");
				}
				if (array[num2] == this.connectionClosed)
				{
					throw new MySqlException("Connection to server lost", true, null);
				}
				if (this.readTimeout != -1)
				{
					num = this.readTimeout - (int)lowResolutionStopwatch.ElapsedMilliseconds;
					if (num < 0)
					{
						throw new TimeoutException("Timeout when reading from shared memory");
					}
				}
				this.bytesLeft = Marshal.ReadInt32(this.data.View);
				this.position = 4;
			}
			int num3 = Math.Min(count, this.bytesLeft);
			long num4 = this.data.View.ToInt64() + (long)this.position;
			int i = 0;
			while (i < num3)
			{
				buffer[offset + i] = Marshal.ReadByte((IntPtr)(num4 + (long)i));
				i++;
				this.position++;
			}
			this.bytesLeft -= num3;
			if (this.bytesLeft == 0)
			{
				this.clientRead.Set();
			}
			return num3;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("SharedMemoryStream does not support seeking");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int i = count;
			int num = offset;
			WaitHandle[] array = new WaitHandle[]
			{
				this.serverRead,
				this.connectionClosed
			};
			LowResolutionStopwatch lowResolutionStopwatch = new LowResolutionStopwatch();
			int num2 = this.writeTimeout;
			while (i > 0)
			{
				lowResolutionStopwatch.Start();
				int num3 = WaitHandle.WaitAny(array, num2);
				lowResolutionStopwatch.Stop();
				if (array[num3] == this.connectionClosed)
				{
					throw new MySqlException("Connection to server lost", true, null);
				}
				if (num3 == 258)
				{
					throw new TimeoutException("Timeout when reading from shared memory");
				}
				if (this.writeTimeout != -1)
				{
					num2 = this.writeTimeout - (int)lowResolutionStopwatch.ElapsedMilliseconds;
					if (num2 < 0)
					{
						throw new TimeoutException("Timeout when writing to shared memory");
					}
				}
				int num4 = Math.Min(i, 16004);
				long value = this.data.View.ToInt64() + 4L;
				Marshal.WriteInt32(this.data.View, num4);
				Marshal.Copy(buffer, num, (IntPtr)value, num4);
				num += num4;
				i -= num4;
				if (!this.clientWrote.Set())
				{
					throw new MySqlException("Writing to shared memory failed");
				}
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("SharedMemoryStream does not support seeking");
		}
	}
}
