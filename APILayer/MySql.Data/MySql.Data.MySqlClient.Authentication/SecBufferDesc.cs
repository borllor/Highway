using System;
using System.Runtime.InteropServices;

namespace MySql.Data.MySqlClient.Authentication
{
	internal struct SecBufferDesc : IDisposable
	{
		public int ulVersion;

		public int cBuffers;

		public IntPtr pBuffers;

		public SecBufferDesc(int bufferSize)
		{
			this.ulVersion = 0;
			this.cBuffers = 1;
			SecBuffer secBuffer = new SecBuffer(bufferSize);
			this.pBuffers = Marshal.AllocHGlobal(Marshal.SizeOf(secBuffer));
			Marshal.StructureToPtr(secBuffer, this.pBuffers, false);
		}

		public SecBufferDesc(byte[] secBufferBytes)
		{
			this.ulVersion = 0;
			this.cBuffers = 1;
			SecBuffer secBuffer = new SecBuffer(secBufferBytes);
			this.pBuffers = Marshal.AllocHGlobal(Marshal.SizeOf(secBuffer));
			Marshal.StructureToPtr(secBuffer, this.pBuffers, false);
		}

		public void Dispose()
		{
			if (this.pBuffers != IntPtr.Zero)
			{
				((SecBuffer)Marshal.PtrToStructure(this.pBuffers, typeof(SecBuffer))).Dispose();
				Marshal.FreeHGlobal(this.pBuffers);
				this.pBuffers = IntPtr.Zero;
			}
		}

		public byte[] GetSecBufferByteArray()
		{
			byte[] array = null;
			if (this.pBuffers == IntPtr.Zero)
			{
				throw new InvalidOperationException("Object has already been disposed!!!");
			}
			SecBuffer secBuffer = (SecBuffer)Marshal.PtrToStructure(this.pBuffers, typeof(SecBuffer));
			if (secBuffer.cbBuffer > 0)
			{
				array = new byte[secBuffer.cbBuffer];
				Marshal.Copy(secBuffer.pvBuffer, array, 0, secBuffer.cbBuffer);
			}
			return array;
		}
	}
}
