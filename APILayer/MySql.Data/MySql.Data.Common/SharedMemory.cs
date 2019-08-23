using MySql.Data.MySqlClient;
using System;

namespace MySql.Data.Common
{
	internal class SharedMemory : IDisposable
	{
		private const uint FILE_MAP_WRITE = 2u;

		private IntPtr fileMapping;

		private IntPtr view;

		public IntPtr View
		{
			get
			{
				return this.view;
			}
		}

		public SharedMemory(string name, IntPtr size)
		{
			this.fileMapping = NativeMethods.OpenFileMapping(2u, false, name);
			if (this.fileMapping == IntPtr.Zero)
			{
				throw new MySqlException("Cannot open file mapping " + name);
			}
			this.view = NativeMethods.MapViewOfFile(this.fileMapping, 2u, 0u, 0u, size);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.view != IntPtr.Zero)
				{
					NativeMethods.UnmapViewOfFile(this.view);
					this.view = IntPtr.Zero;
				}
				if (this.fileMapping != IntPtr.Zero)
				{
					NativeMethods.CloseHandle(this.fileMapping);
					this.fileMapping = IntPtr.Zero;
				}
			}
		}
	}
}
