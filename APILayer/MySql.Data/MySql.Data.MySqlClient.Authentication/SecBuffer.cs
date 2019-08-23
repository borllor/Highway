using System;
using System.Runtime.InteropServices;

namespace MySql.Data.MySqlClient.Authentication
{
	public struct SecBuffer : IDisposable
	{
		public int cbBuffer;

		public int BufferType;

		public IntPtr pvBuffer;

		public SecBuffer(int bufferSize)
		{
			this.cbBuffer = bufferSize;
			this.BufferType = 2;
			this.pvBuffer = Marshal.AllocHGlobal(bufferSize);
		}

		public SecBuffer(byte[] secBufferBytes)
		{
			this.cbBuffer = secBufferBytes.Length;
			this.BufferType = 2;
			this.pvBuffer = Marshal.AllocHGlobal(this.cbBuffer);
			Marshal.Copy(secBufferBytes, 0, this.pvBuffer, this.cbBuffer);
		}

		public SecBuffer(byte[] secBufferBytes, SecBufferType bufferType)
		{
			this.cbBuffer = secBufferBytes.Length;
			this.BufferType = (int)bufferType;
			this.pvBuffer = Marshal.AllocHGlobal(this.cbBuffer);
			Marshal.Copy(secBufferBytes, 0, this.pvBuffer, this.cbBuffer);
		}

		public void Dispose()
		{
			if (this.pvBuffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.pvBuffer);
				this.pvBuffer = IntPtr.Zero;
			}
		}
	}
}
