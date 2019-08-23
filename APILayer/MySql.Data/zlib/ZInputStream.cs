using System;
using System.IO;

namespace zlib
{
	internal class ZInputStream : BinaryReader
	{
		public long maxInput;

		protected internal ZStream z = new ZStream();

		protected internal int bufsize = 512;

		protected internal int flush;

		protected internal byte[] buf;

		protected internal byte[] buf1 = new byte[1];

		protected internal bool compress;

		private Stream in_Renamed;

		private bool nomoreinput;

		public virtual int FlushMode
		{
			get
			{
				return this.flush;
			}
			set
			{
				this.flush = value;
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

		private void InitBlock()
		{
			this.flush = 0;
			this.buf = new byte[this.bufsize];
		}

		public ZInputStream(Stream in_Renamed) : base(in_Renamed)
		{
			this.InitBlock();
			this.in_Renamed = in_Renamed;
			this.z.inflateInit();
			this.compress = false;
			this.z.next_in = this.buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		public ZInputStream(Stream in_Renamed, int level) : base(in_Renamed)
		{
			this.InitBlock();
			this.in_Renamed = in_Renamed;
			this.z.deflateInit(level);
			this.compress = true;
			this.z.next_in = this.buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		public override int Read()
		{
			if (this.read(this.buf1, 0, 1) == -1)
			{
				return -1;
			}
			return (int)(this.buf1[0] & 255);
		}

		public int read(byte[] b, int off, int len)
		{
			if (len == 0)
			{
				return 0;
			}
			this.z.next_out = b;
			this.z.next_out_index = off;
			this.z.avail_out = len;
			while (true)
			{
				if (this.z.avail_in == 0 && !this.nomoreinput)
				{
					this.z.next_in_index = 0;
					int count = this.bufsize;
					if (this.maxInput > 0L)
					{
						if (this.TotalIn < this.maxInput)
						{
							count = (int)Math.Min(this.maxInput - this.TotalIn, (long)this.bufsize);
						}
						else
						{
							this.z.avail_in = -1;
						}
					}
					if (this.z.avail_in != -1)
					{
						this.z.avail_in = SupportClass.ReadInput(this.in_Renamed, this.buf, 0, count);
					}
					if (this.z.avail_in == -1)
					{
						this.z.avail_in = 0;
						this.nomoreinput = true;
					}
				}
				int num;
				if (this.compress)
				{
					num = this.z.deflate(this.flush);
				}
				else
				{
					num = this.z.inflate(this.flush);
				}
				if (this.nomoreinput && num == -5)
				{
					break;
				}
				if (num != 0 && num != 1)
				{
					goto Block_12;
				}
				if (this.nomoreinput && this.z.avail_out == len)
				{
					return -1;
				}
				if (this.z.avail_out <= 0 || num != 0)
				{
					goto IL_184;
				}
			}
			return -1;
			Block_12:
			throw new ZStreamException((this.compress ? "de" : "in") + "flating: " + this.z.msg);
			IL_184:
			return len - this.z.avail_out;
		}

		public long skip(long n)
		{
			int num = 512;
			if (n < (long)num)
			{
				num = (int)n;
			}
			byte[] array = new byte[num];
			return (long)SupportClass.ReadInput(this.BaseStream, array, 0, array.Length);
		}

		public override void Close()
		{
			this.in_Renamed.Close();
		}
	}
}
