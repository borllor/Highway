using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySql.Data.MySqlClient.Memcached
{
	public class TextClient : Client
	{
		private Encoding encoding;

		private static readonly string PROTOCOL_ADD = "add";

		private static readonly string PROTOCOL_APPEND = "append";

		private static readonly string PROTOCOL_CAS = "cas";

		private static readonly string PROTOCOL_DECREMENT = "decr";

		private static readonly string PROTOCOL_DELETE = "delete";

		private static readonly string PROTOCOL_FLUSHALL = "flush_all";

		private static readonly string PROTOCOL_GET = "get";

		private static readonly string PROTOCOL_GETS = "gets";

		private static readonly string PROTOCOL_INCREMENT = "incr";

		private static readonly string PROTOCOL_PREPEND = "prepend";

		private static readonly string PROTOCOL_REPLACE = "replace";

		private static readonly string PROTOCOL_SET = "set";

		private static readonly string VALUE = "VALUE";

		private static readonly string END = "END";

		private static readonly string ERR_ERROR = "ERROR";

		private static readonly string ERR_CLIENT_ERROR = "CLIENT_ERROR";

		private static readonly string ERR_SERVER_ERROR = "SERVER_ERROR";

		protected internal TextClient(string server, uint port) : base(server, port)
		{
			this.encoding = Encoding.UTF8;
		}

		public override void Add(string key, object data, TimeSpan expiration)
		{
			this.SendCommand(TextClient.PROTOCOL_ADD, key, data, expiration);
		}

		public override void Append(string key, object data)
		{
			this.SendCommand(TextClient.PROTOCOL_APPEND, key, data);
		}

		public override void Cas(string key, object data, TimeSpan expiration, ulong casUnique)
		{
			this.SendCommand(TextClient.PROTOCOL_CAS, key, data, expiration, casUnique);
		}

		public override void Decrement(string key, int amount)
		{
			this.SendCommand(TextClient.PROTOCOL_DECREMENT, key, amount);
		}

		public override void Delete(string key)
		{
			this.SendCommand(TextClient.PROTOCOL_DELETE, key);
		}

		public override void FlushAll(TimeSpan delay)
		{
			this.SendCommand(TextClient.PROTOCOL_FLUSHALL, delay);
		}

		public override KeyValuePair<string, object> Get(string key)
		{
			KeyValuePair<string, object>[] array = this.Gets(new string[]
			{
				key
			});
			if (array.Length == 0)
			{
				throw new MemcachedException("Item does not exists.");
			}
			return array[0];
		}

		private KeyValuePair<string, object>[] Gets(params string[] keys)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0}", TextClient.PROTOCOL_GETS));
			for (int i = 0; i < keys.Length; i++)
			{
				stringBuilder.Append(string.Format(" {0}", keys[i]));
			}
			stringBuilder.Append("\r\n");
			this.SendData(stringBuilder.ToString());
			byte[] response = this.GetResponse();
			return this.ParseGetResponse(response);
		}

		public override void Increment(string key, int amount)
		{
			this.SendCommand(TextClient.PROTOCOL_INCREMENT, key, amount);
		}

		public override void Prepend(string key, object data)
		{
			this.SendCommand(TextClient.PROTOCOL_PREPEND, key, data);
		}

		public override void Replace(string key, object data, TimeSpan expiration)
		{
			this.SendCommand(TextClient.PROTOCOL_REPLACE, key, data, expiration);
		}

		public override void Set(string key, object data, TimeSpan expiration)
		{
			this.SendCommand(TextClient.PROTOCOL_SET, key, data, expiration);
		}

		private void SendCommand(string cmd, string key, object data, TimeSpan expiration, ulong casUnique)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} {1} 0 {2} ", cmd, key, (int)expiration.TotalSeconds));
			byte[] bytes = this.encoding.GetBytes(data.ToString());
			string @string = this.encoding.GetString(bytes, 0, bytes.Length);
			stringBuilder.Append(@string.Length.ToString());
			stringBuilder.AppendFormat(" {0}", casUnique);
			stringBuilder.Append("\r\n");
			stringBuilder.Append(@string);
			stringBuilder.Append("\r\n");
			this.SendData(stringBuilder.ToString());
			this.GetResponse();
		}

		private void SendCommand(string cmd, string key, object data, TimeSpan expiration)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} {1} 0 {2} ", cmd, key, (int)expiration.TotalSeconds));
			byte[] bytes = this.encoding.GetBytes(data.ToString());
			string @string = this.encoding.GetString(bytes, 0, bytes.Length);
			stringBuilder.Append(@string.Length.ToString());
			stringBuilder.Append("\r\n");
			stringBuilder.Append(@string);
			stringBuilder.Append("\r\n");
			this.SendData(stringBuilder.ToString());
			this.GetResponse();
		}

		private void SendCommand(string cmd, string key, object data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} {1} ", cmd, key));
			byte[] bytes = this.encoding.GetBytes(data.ToString());
			string @string = this.encoding.GetString(bytes, 0, bytes.Length);
			if (cmd == TextClient.PROTOCOL_APPEND || cmd == TextClient.PROTOCOL_PREPEND)
			{
				stringBuilder.Append("0 0 ");
			}
			stringBuilder.Append(@string.Length.ToString());
			stringBuilder.Append("\r\n");
			stringBuilder.Append(@string);
			stringBuilder.Append("\r\n");
			this.SendData(stringBuilder.ToString());
			this.GetResponse();
		}

		private void SendCommand(string cmd, string key)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} {1} ", cmd, key));
			stringBuilder.Append("\r\n");
			this.SendData(stringBuilder.ToString());
			this.GetResponse();
		}

		private void SendCommand(string cmd, string key, int amount)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} {1} {2}", cmd, key, amount));
			stringBuilder.Append("\r\n");
			this.SendData(stringBuilder.ToString());
			this.GetResponse();
		}

		private void SendCommand(string cmd, TimeSpan expiration)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} {1}\r\n", TextClient.PROTOCOL_FLUSHALL, expiration.TotalSeconds));
			this.SendData(stringBuilder.ToString());
			this.GetResponse();
		}

		private void ValidateErrorResponse(byte[] res)
		{
			string @string = this.encoding.GetString(res, 0, res.Length);
			if (@string.StartsWith(TextClient.ERR_ERROR, StringComparison.OrdinalIgnoreCase) || @string.StartsWith(TextClient.ERR_CLIENT_ERROR, StringComparison.OrdinalIgnoreCase) || @string.StartsWith(TextClient.ERR_SERVER_ERROR, StringComparison.OrdinalIgnoreCase))
			{
				throw new MemcachedException(@string);
			}
		}

		private void SendData(string sData)
		{
			byte[] bytes = this.encoding.GetBytes(sData);
			this.stream.Write(bytes, 0, bytes.Length);
		}

		private KeyValuePair<string, object>[] ParseGetResponse(byte[] input)
		{
			string[] array = this.encoding.GetString(input, 0, input.Length).Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.None);
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			int num = 0;
			string key = "";
			while (array[num] != TextClient.END && num < array.Length)
			{
				if (array[num].StartsWith(TextClient.VALUE, StringComparison.OrdinalIgnoreCase))
				{
					key = array[num].Split(new char[]
					{
						' '
					})[1];
				}
				else
				{
					KeyValuePair<string, object> item = new KeyValuePair<string, object>(key, array[num]);
					list.Add(item);
				}
				num++;
			}
			return list.ToArray();
		}

		private byte[] GetResponse()
		{
			byte[] buffer = new byte[1024];
			MemoryStream memoryStream = new MemoryStream();
			for (int i = this.stream.Read(buffer, 0, 1024); i > 0; i = this.stream.Read(buffer, 0, 1024))
			{
				memoryStream.Write(buffer, 0, i);
				if (i < 1024)
				{
					break;
				}
			}
			byte[] array = memoryStream.ToArray();
			this.ValidateErrorResponse(array);
			return array;
		}
	}
}
