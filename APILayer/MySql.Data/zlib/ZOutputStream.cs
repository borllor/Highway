using System;
using System.IO;

namespace zlib
{
	internal class ZOutputStream : Stream
	{
		protected internal ZStream z = new ZStream();

		protected internal int bufsize = 512;

		protected internal int flush_Renamed_Field;

		protected internal byte[] buf;

		protected internal byte[] buf1 = new byte[1];

		protected internal bool compress;

		private Stream out_Renamed;

		public virtual int FlushMode
		{
			get
			{
				return this.flush_Renamed_Field;
			}
			set
			{
				this.flush_Renamed_Field = value;
			}
		}

		public virtual long TotalIn
		{
			get
			{
				return this.z.total_in;
			}
		}

		public virtual long TotalOut
		{
			get
			{
				return this.z.total_out;
			}
		}

		public override bool CanRead
		{
			get
			{
				return false;
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
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return 0L;
			}
		}

		public override long Position
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		private void InitBlock()
		{
			this.flush_Renamed_Field = 0;
			this.buf = new byte[this.bufsize];
		}

		public ZOutputStream(Stream out_Renamed)
		{
			this.InitBlock();
			this.out_Renamed = out_Renamed;
			this.z.inflateInit();
			this.compress = false;
		}

		public ZOutputStream(Stream out_Renamed, int level)
		{
			this.InitBlock();
			this.out_Renamed = out_Renamed;
			this.z.deflateInit(level);
			this.compress = true;
		}

		public void WriteByte(int b)
		{
			this.buf1[0] = (byte)b;
			this.Write(this.buf1, 0, 1);
		}

		public override void WriteByte(byte b)
		{
			this.WriteByte((int)b);
		}

		public override void Write(byte[] b1, int off, int len)
		{
			if (len == 0)
			{
				return;
			}
			byte[] array = new byte[b1.Length];
			Array.Copy(b1, array, b1.Length);
			this.z.next_in = array;
			this.z.next_in_index = off;
			this.z.avail_in = len;
			while (true)
			{
				this.z.next_out = this.buf;
				this.z.next_out_index = 0;
				this.z.avail_out = this.bufsize;
				int num;
				if (this.compress)
				{
					num = this.z.deflate(this.flush_Renamed_Field);
				}
				else
				{
					num = this.z.inflate(this.flush_Renamed_Field);
				}
				if (num != 0)
				{
					break;
				}
				this.out_Renamed.Write(this.buf, 0, this.bufsize - this.z.avail_out);
				if (this.z.avail_in <= 0 && this.z.avail_out != 0)
				{
					return;
				}
			}
			throw new ZStreamException((this.compress ? "de" : "in") + "flating: " + this.z.msg);
		}

		public virtual void finish()
		{
			while (true)
			{
				this.z.next_out = this.buf;
				this.z.next_out_index = 0;
				this.z.avail_out = this.bufsize;
				int num;
				if (this.compress)
				{
					num = this.z.deflate(4);
				}
				else
				{
					num = this.z.inflate(4);
				}
				if (num != 1 && num != 0)
				{
					break;
				}
				if (this.bufsize - this.z.avail_out > 0)
				{
					this.out_Renamed.Write(this.buf, 0, this.bufsize - this.z.avail_out);
				}
				if (this.z.avail_in <= 0 && this.z.avail_out != 0)
				{
					goto Block_6;
				}
			}
			throw new ZStreamException((this.compress ? "de" : "in") + "flating: " + this.z.msg);
			Block_6:
			try
			{
				this.Flush();
			}
			catch
			{
			}
		}

		public virtual void end()
		{
			if (this.compress)
			{
				this.z.deflateEnd();
			}
			else
			{
				this.z.inflateEnd();
			}
			this.z.free();
			this.z = null;
		}

		public override void Close()
		{
			try
			{
				this.finish();
			}
			catch
			{
			}
			finally
			{
				this.end();
				this.out_Renamed.Close();
				this.out_Renamed = null;
			}
		}

		public override void Flush()
		{
			this.out_Renamed.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return 0;
		}

		public override void SetLength(long value)
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0L;
		}
	}
}
