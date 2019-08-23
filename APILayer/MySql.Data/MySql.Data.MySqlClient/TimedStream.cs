using MySql.Data.Common;
using System;
using System.IO;

namespace MySql.Data.MySqlClient
{
	internal class TimedStream : Stream
	{
		private enum IOKind
		{
			Read,
			Write
		}

		private Stream baseStream;

		private int timeout;

		private int lastReadTimeout;

		private int lastWriteTimeout;

		private LowResolutionStopwatch stopwatch;

		private bool isClosed;

		internal bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.baseStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.baseStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.baseStream.CanWrite;
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

		public TimedStream(Stream baseStream)
		{
			this.baseStream = baseStream;
			this.timeout = baseStream.ReadTimeout;
			this.isClosed = false;
			this.stopwatch = new LowResolutionStopwatch();
		}

		private bool ShouldResetStreamTimeout(int currentValue, int newValue)
		{
			return (newValue == -1 && currentValue != newValue) || newValue > currentValue || currentValue >= newValue + 100;
		}

		private void StartTimer(TimedStream.IOKind op)
		{
			int num;
			if (this.timeout == -1)
			{
				num = -1;
			}
			else
			{
				num = this.timeout - (int)this.stopwatch.ElapsedMilliseconds;
			}
			if (op == TimedStream.IOKind.Read)
			{
				if (this.ShouldResetStreamTimeout(this.lastReadTimeout, num))
				{
					this.baseStream.ReadTimeout = num;
					this.lastReadTimeout = num;
				}
			}
			else if (this.ShouldResetStreamTimeout(this.lastWriteTimeout, num))
			{
				this.baseStream.WriteTimeout = num;
				this.lastWriteTimeout = num;
			}
			if (this.timeout == -1)
			{
				return;
			}
			this.stopwatch.Start();
		}

		private void StopTimer()
		{
			if (this.timeout == -1)
			{
				return;
			}
			this.stopwatch.Stop();
			if (this.stopwatch.ElapsedMilliseconds > (long)this.timeout)
			{
				this.ResetTimeout(-1);
				throw new TimeoutException("Timeout in IO operation");
			}
		}

		public override void Flush()
		{
			try
			{
				this.StartTimer(TimedStream.IOKind.Write);
				this.baseStream.Flush();
				this.StopTimer();
			}
			catch (Exception e)
			{
				this.HandleException(e);
				throw;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int result;
			try
			{
				this.StartTimer(TimedStream.IOKind.Read);
				int num = this.baseStream.Read(buffer, offset, count);
				this.StopTimer();
				result = num;
			}
			catch (Exception e)
			{
				this.HandleException(e);
				throw;
			}
			return result;
		}

		public override int ReadByte()
		{
			int result;
			try
			{
				this.StartTimer(TimedStream.IOKind.Read);
				int num = this.baseStream.ReadByte();
				this.StopTimer();
				result = num;
			}
			catch (Exception e)
			{
				this.HandleException(e);
				throw;
			}
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.baseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.baseStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			try
			{
				this.StartTimer(TimedStream.IOKind.Write);
				this.baseStream.Write(buffer, offset, count);
				this.StopTimer();
			}
			catch (Exception e)
			{
				this.HandleException(e);
				throw;
			}
		}

		public override void Close()
		{
			if (this.isClosed)
			{
				return;
			}
			this.isClosed = true;
			this.baseStream.Close();
			this.baseStream.Dispose();
		}

		public void ResetTimeout(int newTimeout)
		{
			if (newTimeout == -1 || newTimeout == 0)
			{
				this.timeout = -1;
			}
			else
			{
				this.timeout = newTimeout;
			}
			this.stopwatch.Reset();
		}

		private void HandleException(Exception e)
		{
			this.stopwatch.Stop();
			this.ResetTimeout(-1);
		}
	}
}
