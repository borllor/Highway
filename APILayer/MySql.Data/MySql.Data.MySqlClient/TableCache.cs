using System;

namespace MySql.Data.MySqlClient
{
	internal class TableCache
	{
		private static BaseTableCache cache;

		static TableCache()
		{
			TableCache.cache = new BaseTableCache(480);
		}

		public static void AddToCache(string commandText, ResultSet resultSet)
		{
			TableCache.cache.AddToCache(commandText, resultSet);
		}

		public static ResultSet RetrieveFromCache(string commandText, int cacheAge)
		{
			return (ResultSet)TableCache.cache.RetrieveFromCache(commandText, cacheAge);
		}

		public static void RemoveFromCache(string commandText)
		{
			TableCache.cache.RemoveFromCache(commandText);
		}

		public static void DumpCache()
		{
			TableCache.cache.Dump();
		}
	}
}
