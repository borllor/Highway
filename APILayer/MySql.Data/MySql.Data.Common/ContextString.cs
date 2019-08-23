using System;
using System.Collections;
using System.Text;

namespace MySql.Data.Common
{
	internal class ContextString
	{
		private string contextMarkers;

		private bool escapeBackslash;

		public string ContextMarkers
		{
			get
			{
				return this.contextMarkers;
			}
			set
			{
				this.contextMarkers = value;
			}
		}

		public ContextString(string contextMarkers, bool escapeBackslash)
		{
			this.contextMarkers = contextMarkers;
			this.escapeBackslash = escapeBackslash;
		}

		public int IndexOf(string src, string target)
		{
			return this.IndexOf(src, target, 0);
		}

		public int IndexOf(string src, string target, int startIndex)
		{
			int num = src.IndexOf(target, startIndex);
			while (num != -1 && this.IndexInQuotes(src, num, startIndex))
			{
				num = src.IndexOf(target, num + 1);
			}
			return num;
		}

		private bool IndexInQuotes(string src, int index, int startIndex)
		{
			char c = '\0';
			bool flag = false;
			for (int i = startIndex; i < index; i++)
			{
				char c2 = src[i];
				int num = this.contextMarkers.IndexOf(c2);
				if (num > -1 && c == this.contextMarkers[num] && !flag)
				{
					c = '\0';
				}
				else if (c == '\0' && num > -1 && !flag)
				{
					c = c2;
				}
				else if (c2 == '\\' && this.escapeBackslash)
				{
					flag = !flag;
				}
			}
			return c != '\0' || flag;
		}

		public int IndexOf(string src, char target)
		{
			char c = '\0';
			bool flag = false;
			int num = 0;
			for (int i = 0; i < src.Length; i++)
			{
				char c2 = src[i];
				int num2 = this.contextMarkers.IndexOf(c2);
				if (num2 > -1 && c == this.contextMarkers[num2] && !flag)
				{
					c = '\0';
				}
				else if (c == '\0' && num2 > -1 && !flag)
				{
					c = c2;
				}
				else
				{
					if (c == '\0' && c2 == target)
					{
						return num;
					}
					if (c2 == '\\' && this.escapeBackslash)
					{
						flag = !flag;
					}
				}
				num++;
			}
			return -1;
		}

		public string[] Split(string src, string delimiters)
		{
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			char c = '\0';
			for (int i = 0; i < src.Length; i++)
			{
				char c2 = src[i];
				if (delimiters.IndexOf(c2) != -1 && !flag)
				{
					if (c != '\0')
					{
						stringBuilder.Append(c2);
					}
					else if (stringBuilder.Length > 0)
					{
						arrayList.Add(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
					}
				}
				else if (c2 == '\\' && this.escapeBackslash)
				{
					flag = !flag;
				}
				else
				{
					int num = this.contextMarkers.IndexOf(c2);
					if (!flag && num != -1)
					{
						if (num % 2 == 1)
						{
							if (c == this.contextMarkers[num - 1])
							{
								c = '\0';
							}
						}
						else if (c == this.contextMarkers[num + 1])
						{
							c = '\0';
						}
						else if (c == '\0')
						{
							c = c2;
						}
					}
					stringBuilder.Append(c2);
				}
			}
			if (stringBuilder.Length > 0)
			{
				arrayList.Add(stringBuilder.ToString());
			}
			return (string[])arrayList.ToArray(typeof(string));
		}
	}
}
