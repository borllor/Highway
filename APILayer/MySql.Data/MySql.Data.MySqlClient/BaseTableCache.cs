using System;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
	public class BaseTableCache
	{
		private struct CacheEntry
		{
			public DateTime CacheTime;

			public object CacheElement;
		}

		protected int MaxCacheAge;

		private Dictionary<string, BaseTableCache.CacheEntry> cache = new Dictionary<string, BaseTableCache.CacheEntry>();

		public BaseTableCache(int maxCacheAge)
		{
			this.MaxCacheAge = maxCacheAge;
		}

		public virtual void AddToCache(string commandText, object resultSet)
		{
			this.CleanCache();
			BaseTableCache.CacheEntry value = default(BaseTableCache.CacheEntry);
			value.CacheTime = DateTime.Now;
			value.CacheElement = resultSet;
			lock (this.cache)
			{
				if (!this.cache.ContainsKey(commandText))
				{
					this.cache.Add(commandText, value);
				}
			}
		}

		public virtual object RetrieveFromCache(string commandText, int cacheAge)
		{
			this.CleanCache();
			object result;
			lock (this.cache)
			{
				if (!this.cache.ContainsKey(commandText))
				{
					result = null;
				}
				else
				{
					BaseTableCache.CacheEntry cacheEntry = this.cache[commandText];
					if (DateTime.Now.Subtract(cacheEntry.CacheTime).TotalSeconds > (double)cacheAge)
					{
						result = null;
					}
					else
					{
						result = cacheEntry.CacheElement;
					}
				}
			}
			return result;
		}

		public void RemoveFromCache(string commandText)
		{
			lock (this.cache)
			{
				if (this.cache.ContainsKey(commandText))
				{
					this.cache.Remove(commandText);
				}
			}
		}

		public virtual void Dump()
		{
			lock (this.cache)
			{
				this.cache.Clear();
			}
		}

		protected virtual void CleanCache()
		{
			DateTime now = DateTime.Now;
			List<string> list = new List<string>();
			lock (this.cache)
			{
				foreach (string current in this.cache.Keys)
				{
					if (now.Subtract(this.cache[current].CacheTime).TotalSeconds > (double)this.MaxCacheAge)
					{
						list.Add(current);
					}
				}
				foreach (string current2 in list)
				{
					this.cache.Remove(current2);
				}
			}
		}
	}
}
