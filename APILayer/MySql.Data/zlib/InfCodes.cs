using System;

namespace zlib
{
	internal sealed class InfCodes
	{
		private const int Z_OK = 0;

		private const int Z_STREAM_END = 1;

		private const int Z_NEED_DICT = 2;

		private const int Z_ERRNO = -1;

		private const int Z_STREAM_ERROR = -2;

		private const int Z_DATA_ERROR = -3;

		private const int Z_MEM_ERROR = -4;

		private const int Z_BUF_ERROR = -5;

		private const int Z_VERSION_ERROR = -6;

		private const int START = 0;

		private const int LEN = 1;

		private const int LENEXT = 2;

		private const int DIST = 3;

		private const int DISTEXT = 4;

		private const int COPY = 5;

		private const int LIT = 6;

		private const int WASH = 7;

		private const int END = 8;

		private const int BADCODE = 9;

		private static readonly int[] inflate_mask = new int[]
		{
			0,
			1,
			3,
			7,
			15,
			31,
			63,
			127,
			255,
			511,
			1023,
			2047,
			4095,
			8191,
			16383,
			32767,
			65535
		};

		internal int mode;

		internal int len;

		internal int[] tree;

		internal int tree_index;

		internal int need;

		internal int lit;

		internal int get_Renamed;

		internal int dist;

		internal byte lbits;

		internal byte dbits;

		internal int[] ltree;

		internal int ltree_index;

		internal int[] dtree;

		internal int dtree_index;

		internal InfCodes(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, ZStream z)
		{
			this.mode = 0;
			this.lbits = (byte)bl;
			this.dbits = (byte)bd;
			this.ltree = tl;
			this.ltree_index = tl_index;
			this.dtree = td;
			this.dtree_index = td_index;
		}

		internal InfCodes(int bl, int bd, int[] tl, int[] td, ZStream z)
		{
			this.mode = 0;
			this.lbits = (byte)bl;
			this.dbits = (byte)bd;
			this.ltree = tl;
			this.ltree_index = 0;
			this.dtree = td;
			this.dtree_index = 0;
		}

		internal int proc(InfBlocks s, ZStream z, int r)
		{
			int num = z.next_in_index;
			int num2 = z.avail_in;
			int num3 = s.bitb;
			int i = s.bitk;
			int num4 = s.write;
			int num5 = (num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4);
			while (true)
			{
				int num6;
				switch (this.mode)
				{
				case 0:
					if (num5 >= 258 && num2 >= 10)
					{
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += (long)(num - z.next_in_index);
						z.next_in_index = num;
						s.write = num4;
						r = this.inflate_fast((int)this.lbits, (int)this.dbits, this.ltree, this.ltree_index, this.dtree, this.dtree_index, s, z);
						num = z.next_in_index;
						num2 = z.avail_in;
						num3 = s.bitb;
						i = s.bitk;
						num4 = s.write;
						num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
						if (r != 0)
						{
							this.mode = ((r == 1) ? 7 : 9);
							continue;
						}
					}
					this.need = (int)this.lbits;
					this.tree = this.ltree;
					this.tree_index = this.ltree_index;
					this.mode = 1;
					goto IL_199;
				case 1:
					goto IL_199;
				case 2:
					num6 = this.get_Renamed;
					while (i < num6)
					{
						if (num2 == 0)
						{
							goto IL_34F;
						}
						r = 0;
						num2--;
						num3 |= (int)(z.next_in[num++] & 255) << i;
						i += 8;
					}
					this.len += (num3 & InfCodes.inflate_mask[num6]);
					num3 >>= num6;
					i -= num6;
					this.need = (int)this.dbits;
					this.tree = this.dtree;
					this.tree_index = this.dtree_index;
					this.mode = 3;
					goto IL_412;
				case 3:
					goto IL_412;
				case 4:
					num6 = this.get_Renamed;
					while (i < num6)
					{
						if (num2 == 0)
						{
							goto IL_596;
						}
						r = 0;
						num2--;
						num3 |= (int)(z.next_in[num++] & 255) << i;
						i += 8;
					}
					this.dist += (num3 & InfCodes.inflate_mask[num6]);
					num3 >>= num6;
					i -= num6;
					this.mode = 5;
					goto IL_635;
				case 5:
					goto IL_635;
				case 6:
					if (num5 == 0)
					{
						if (num4 == s.end && s.read != 0)
						{
							num4 = 0;
							num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
						}
						if (num5 == 0)
						{
							s.write = num4;
							r = s.inflate_flush(z, r);
							num4 = s.write;
							num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							if (num4 == s.end && s.read != 0)
							{
								num4 = 0;
								num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							}
							if (num5 == 0)
							{
								goto Block_44;
							}
						}
					}
					r = 0;
					s.window[num4++] = (byte)this.lit;
					num5--;
					this.mode = 0;
					continue;
				case 7:
					goto IL_8DB;
				case 8:
					goto IL_98A;
				case 9:
					goto IL_9D4;
				}
				break;
				IL_199:
				num6 = this.need;
				while (i < num6)
				{
					if (num2 == 0)
					{
						goto IL_1AB;
					}
					r = 0;
					num2--;
					num3 |= (int)(z.next_in[num++] & 255) << i;
					i += 8;
				}
				int num7 = (this.tree_index + (num3 & InfCodes.inflate_mask[num6])) * 3;
				num3 = SupportClass.URShift(num3, this.tree[num7 + 1]);
				i -= this.tree[num7 + 1];
				int num8 = this.tree[num7];
				if (num8 == 0)
				{
					this.lit = this.tree[num7 + 2];
					this.mode = 6;
					continue;
				}
				if ((num8 & 16) != 0)
				{
					this.get_Renamed = (num8 & 15);
					this.len = this.tree[num7 + 2];
					this.mode = 2;
					continue;
				}
				if ((num8 & 64) == 0)
				{
					this.need = num8;
					this.tree_index = num7 / 3 + this.tree[num7 + 2];
					continue;
				}
				if ((num8 & 32) != 0)
				{
					this.mode = 7;
					continue;
				}
				goto IL_2DF;
				IL_412:
				num6 = this.need;
				while (i < num6)
				{
					if (num2 == 0)
					{
						goto IL_424;
					}
					r = 0;
					num2--;
					num3 |= (int)(z.next_in[num++] & 255) << i;
					i += 8;
				}
				num7 = (this.tree_index + (num3 & InfCodes.inflate_mask[num6])) * 3;
				num3 >>= this.tree[num7 + 1];
				i -= this.tree[num7 + 1];
				num8 = this.tree[num7];
				if ((num8 & 16) != 0)
				{
					this.get_Renamed = (num8 & 15);
					this.dist = this.tree[num7 + 2];
					this.mode = 4;
					continue;
				}
				if ((num8 & 64) == 0)
				{
					this.need = num8;
					this.tree_index = num7 / 3 + this.tree[num7 + 2];
					continue;
				}
				goto IL_526;
				IL_635:
				int j;
				for (j = num4 - this.dist; j < 0; j += s.end)
				{
				}
				while (this.len != 0)
				{
					if (num5 == 0)
					{
						if (num4 == s.end && s.read != 0)
						{
							num4 = 0;
							num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
						}
						if (num5 == 0)
						{
							s.write = num4;
							r = s.inflate_flush(z, r);
							num4 = s.write;
							num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							if (num4 == s.end && s.read != 0)
							{
								num4 = 0;
								num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							}
							if (num5 == 0)
							{
								goto Block_32;
							}
						}
					}
					s.window[num4++] = s.window[j++];
					num5--;
					if (j == s.end)
					{
						j = 0;
					}
					this.len--;
				}
				this.mode = 0;
			}
			r = -2;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_1AB:
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_2DF:
			this.mode = 9;
			z.msg = "invalid literal/length code";
			r = -3;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_34F:
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_424:
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_526:
			this.mode = 9;
			z.msg = "invalid distance code";
			r = -3;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_596:
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			Block_32:
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			Block_44:
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_8DB:
			if (i > 7)
			{
				i -= 8;
				num2++;
				num--;
			}
			s.write = num4;
			r = s.inflate_flush(z, r);
			num4 = s.write;
			int arg_92C_0 = (num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4);
			if (s.read != s.write)
			{
				s.bitb = num3;
				s.bitk = i;
				z.avail_in = num2;
				z.total_in += (long)(num - z.next_in_index);
				z.next_in_index = num;
				s.write = num4;
				return s.inflate_flush(z, r);
			}
			this.mode = 8;
			IL_98A:
			r = 1;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
			IL_9D4:
			r = -3;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return s.inflate_flush(z, r);
		}

		internal void free(ZStream z)
		{
		}

		internal int inflate_fast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InfBlocks s, ZStream z)
		{
			int num = z.next_in_index;
			int num2 = z.avail_in;
			int num3 = s.bitb;
			int i = s.bitk;
			int num4 = s.write;
			int num5 = (num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4);
			int num6 = InfCodes.inflate_mask[bl];
			int num7 = InfCodes.inflate_mask[bd];
			int num9;
			int num10;
			while (true)
			{
				if (i >= 20)
				{
					int num8 = num3 & num6;
					if ((num9 = tl[(tl_index + num8) * 3]) == 0)
					{
						num3 >>= tl[(tl_index + num8) * 3 + 1];
						i -= tl[(tl_index + num8) * 3 + 1];
						s.window[num4++] = (byte)tl[(tl_index + num8) * 3 + 2];
						num5--;
					}
					else
					{
						while (true)
						{
							num3 >>= tl[(tl_index + num8) * 3 + 1];
							i -= tl[(tl_index + num8) * 3 + 1];
							if ((num9 & 16) != 0)
							{
								break;
							}
							if ((num9 & 64) != 0)
							{
								goto IL_4D3;
							}
							num8 += tl[(tl_index + num8) * 3 + 2];
							num8 += (num3 & InfCodes.inflate_mask[num9]);
							if ((num9 = tl[(tl_index + num8) * 3]) == 0)
							{
								goto Block_20;
							}
						}
						num9 &= 15;
						num10 = tl[(tl_index + num8) * 3 + 2] + (num3 & InfCodes.inflate_mask[num9]);
						num3 >>= num9;
						for (i -= num9; i < 15; i += 8)
						{
							num2--;
							num3 |= (int)(z.next_in[num++] & 255) << i;
						}
						num8 = (num3 & num7);
						num9 = td[(td_index + num8) * 3];
						while (true)
						{
							num3 >>= td[(td_index + num8) * 3 + 1];
							i -= td[(td_index + num8) * 3 + 1];
							if ((num9 & 16) != 0)
							{
								break;
							}
							if ((num9 & 64) != 0)
							{
								goto IL_3D9;
							}
							num8 += td[(td_index + num8) * 3 + 2];
							num8 += (num3 & InfCodes.inflate_mask[num9]);
							num9 = td[(td_index + num8) * 3];
						}
						num9 &= 15;
						while (i < num9)
						{
							num2--;
							num3 |= (int)(z.next_in[num++] & 255) << i;
							i += 8;
						}
						int num11 = td[(td_index + num8) * 3 + 2] + (num3 & InfCodes.inflate_mask[num9]);
						num3 >>= num9;
						i -= num9;
						num5 -= num10;
						int num12;
						if (num4 >= num11)
						{
							num12 = num4 - num11;
							if (num4 - num12 > 0 && 2 > num4 - num12)
							{
								s.window[num4++] = s.window[num12++];
								num10--;
								s.window[num4++] = s.window[num12++];
								num10--;
							}
							else
							{
								Array.Copy(s.window, num12, s.window, num4, 2);
								num4 += 2;
								num12 += 2;
								num10 -= 2;
							}
						}
						else
						{
							num12 = num4 - num11;
							do
							{
								num12 += s.end;
							}
							while (num12 < 0);
							num9 = s.end - num12;
							if (num10 > num9)
							{
								num10 -= num9;
								if (num4 - num12 > 0 && num9 > num4 - num12)
								{
									do
									{
										s.window[num4++] = s.window[num12++];
									}
									while (--num9 != 0);
								}
								else
								{
									Array.Copy(s.window, num12, s.window, num4, num9);
									num4 += num9;
									num12 += num9;
								}
								num12 = 0;
							}
						}
						if (num4 - num12 > 0 && num10 > num4 - num12)
						{
							do
							{
								s.window[num4++] = s.window[num12++];
							}
							while (--num10 != 0);
							goto IL_5E0;
						}
						Array.Copy(s.window, num12, s.window, num4, num10);
						num4 += num10;
						num12 += num10;
						goto IL_5E0;
						Block_20:
						num3 >>= tl[(tl_index + num8) * 3 + 1];
						i -= tl[(tl_index + num8) * 3 + 1];
						s.window[num4++] = (byte)tl[(tl_index + num8) * 3 + 2];
						num5--;
					}
					IL_5E0:
					if (num5 < 258 || num2 < 10)
					{
						goto IL_5F2;
					}
				}
				else
				{
					num2--;
					num3 |= (int)(z.next_in[num++] & 255) << i;
					i += 8;
				}
			}
			IL_3D9:
			z.msg = "invalid distance code";
			num10 = z.avail_in - num2;
			num10 = ((i >> 3 < num10) ? (i >> 3) : num10);
			num2 += num10;
			num -= num10;
			i -= num10 << 3;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return -3;
			IL_4D3:
			if ((num9 & 32) != 0)
			{
				num10 = z.avail_in - num2;
				num10 = ((i >> 3 < num10) ? (i >> 3) : num10);
				num2 += num10;
				num -= num10;
				i -= num10 << 3;
				s.bitb = num3;
				s.bitk = i;
				z.avail_in = num2;
				z.total_in += (long)(num - z.next_in_index);
				z.next_in_index = num;
				s.write = num4;
				return 1;
			}
			z.msg = "invalid literal/length code";
			num10 = z.avail_in - num2;
			num10 = ((i >> 3 < num10) ? (i >> 3) : num10);
			num2 += num10;
			num -= num10;
			i -= num10 << 3;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return -3;
			IL_5F2:
			num10 = z.avail_in - num2;
			num10 = ((i >> 3 < num10) ? (i >> 3) : num10);
			num2 += num10;
			num -= num10;
			i -= num10 << 3;
			s.bitb = num3;
			s.bitk = i;
			z.avail_in = num2;
			z.total_in += (long)(num - z.next_in_index);
			z.next_in_index = num;
			s.write = num4;
			return 0;
		}
	}
}
