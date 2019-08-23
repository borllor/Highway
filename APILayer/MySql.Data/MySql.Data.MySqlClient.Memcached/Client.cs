using MySql.Data.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace MySql.Data.MySqlClient.Memcached
{
	public abstract class Client
	{
		protected uint port;

		protected string server;

		protected Stream stream;

		public static Client GetInstance(string server, uint port, MemcachedFlags flags)
		{
			if ((ushort)(flags | MemcachedFlags.TextProtocol) != 0)
			{
				return new TextClient(server, port);
			}
			if ((ushort)(flags | MemcachedFlags.BinaryProtocol) != 0)
			{
				return new BinaryClient(server, port);
			}
			return null;
		}

		public virtual void Open()
		{
			this.stream = StreamCreator.GetStream(this.server, this.port, null, 10u, default(DBVersion), 60u);
		}

		public virtual void Close()
		{
			this.stream.Dispose();
		}

		protected Client(string server, uint port)
		{
			this.server = server;
			this.port = port;
		}

		public abstract void Add(string key, object data, TimeSpan expiration);

		public abstract void Append(string key, object data);

		public abstract void Cas(string key, object data, TimeSpan expiration, ulong casUnique);

		public abstract void Decrement(string key, int amount);

		public abstract void Delete(string key);

		public abstract void FlushAll(TimeSpan delay);

		public abstract KeyValuePair<string, object> Get(string key);

		public abstract void Increment(string key, int amount);

		public abstract void Prepend(string key, object data);

		public abstract void Replace(string key, object data, TimeSpan expiration);

		public abstract void Set(string key, object data, TimeSpan expiration);
	}
}
