using Microsoft.Win32.SafeHandles;
using MySql.Data.MySqlClient.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace MySql.Data.Common
{
	[SuppressUnmanagedCodeSecurity]
	internal class NamedPipeStream : Stream
	{
		private const int ERROR_PIPE_BUSY = 231;

		private const int ERROR_SEM_TIMEOUT = 121;

		private SafeFileHandle handle;

		private Stream fileStream;

		private int readTimeout = -1;

		private int writeTimeout = -1;

		public override bool CanRead
		{
			get
			{
				return this.fileStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.fileStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				throw new NotSupportedException(Resources.NamedPipeNoSeek);
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException(Resources.NamedPipeNoSeek);
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(Resources.NamedPipeNoSeek);
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

		public NamedPipeStream(string path, FileAccess mode, uint timeout)
		{
			this.Open(path, mode, timeout);
		}

		private void CancelIo()
		{
			if (!NativeMethods.CancelIo(this.handle.DangerousGetHandle()))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		public void Open(string path, FileAccess mode, uint timeout)
		{
			IntPtr intPtr;
			LowResolutionStopwatch lowResolutionStopwatch;
			while (true)
			{
				NativeMethods.SecurityAttributes securityAttributes = new NativeMethods.SecurityAttributes();
				securityAttributes.inheritHandle = true;
				securityAttributes.Length = Marshal.SizeOf(securityAttributes);
				intPtr = NativeMethods.CreateFile(path, 3221225472u, 0u, securityAttributes, 3u, 1073741824u, 0u);
				if (intPtr != IntPtr.Zero)
				{
					goto IL_AC;
				}
				if (Marshal.GetLastWin32Error() != 231)
				{
					break;
				}
				lowResolutionStopwatch = LowResolutionStopwatch.StartNew();
				bool flag = NativeMethods.WaitNamedPipe(path, timeout);
				lowResolutionStopwatch.Stop();
				if (!flag)
				{
					goto Block_2;
				}
				timeout -= (uint)lowResolutionStopwatch.ElapsedMilliseconds;
			}
			throw new Win32Exception(Marshal.GetLastWin32Error(), "Error opening pipe");
			Block_2:
			if ((ulong)timeout < (ulong)lowResolutionStopwatch.ElapsedMilliseconds || Marshal.GetLastWin32Error() == 121)
			{
				throw new TimeoutException("Timeout waiting for named pipe");
			}
			throw new Win32Exception(Marshal.GetLastWin32Error(), "Error waiting for pipe");
			IL_AC:
			this.handle = new SafeFileHandle(intPtr, true);
			this.fileStream = new FileStream(this.handle, mode, 4096, true);
		}

		public override void Flush()
		{
			this.fileStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.readTimeout == -1)
			{
				return this.fileStream.Read(buffer, offset, count);
			}
			IAsyncResult asyncResult = this.fileStream.BeginRead(buffer, offset, count, null, null);
			if (asyncResult.CompletedSynchronously)
			{
				return this.fileStream.EndRead(asyncResult);
			}
			if (!asyncResult.AsyncWaitHandle.WaitOne(this.readTimeout))
			{
				this.CancelIo();
				throw new TimeoutException("Timeout in named pipe read");
			}
			return this.fileStream.EndRead(asyncResult);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.writeTimeout == -1)
			{
				this.fileStream.Write(buffer, offset, count);
				return;
			}
			IAsyncResult asyncResult = this.fileStream.BeginWrite(buffer, offset, count, null, null);
			if (asyncResult.CompletedSynchronously)
			{
				this.fileStream.EndWrite(asyncResult);
			}
			if (!asyncResult.AsyncWaitHandle.WaitOne(this.readTimeout))
			{
				this.CancelIo();
				throw new TimeoutException("Timeout in named pipe write");
			}
			this.fileStream.EndWrite(asyncResult);
		}

		public override void Close()
		{
			if (this.handle != null && !this.handle.IsInvalid && !this.handle.IsClosed)
			{
				this.fileStream.Close();
				try
				{
					this.handle.Close();
				}
				catch (Exception)
				{
				}
			}
		}

		public override void SetLength(long length)
		{
			throw new NotSupportedException(Resources.NamedPipeNoSetLength);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(Resources.NamedPipeNoSeek);
		}

		internal static Stream Create(string pipeName, string hostname, uint timeout)
		{
			string path;
			if (string.Compare(hostname, "localhost", true) == 0)
			{
				path = "\\\\.\\pipe\\" + pipeName;
			}
			else
			{
				path = string.Format("\\\\{0}\\pipe\\{1}", hostname, pipeName);
			}
			return new NamedPipeStream(path, FileAccess.ReadWrite, timeout);
		}
	}
}
