using System;
using System.Collections;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;

namespace JinRi.Notify.Frame
{
    internal class MemoryCacheProvider : ICacheProvider
    {
        private static System.Runtime.Caching.MemoryCache _cache;

        protected static System.Runtime.Caching.MemoryCache Cache
        {
            get
            {
                return _cache ?? (_cache = System.Runtime.Caching.MemoryCache.Default);
            }
        }

        public MemoryCacheProvider()
        {
            _cache = System.Runtime.Caching.MemoryCache.Default;
        }

        #region ICacheProvider 成员

        public bool Add(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;
            return Cache.Add(key, value, new CacheItemPolicy());
        }

        public bool Add(string key, object value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key) || value == null || expiry < DateTime.Now)
                return false;
            var cacheItem = new CacheItem(key, value);
            var cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = expiry;
            return Cache.Add(cacheItem, cacheItemPolicy);
        }

        public object Get(string key)
        {
            return Cache.Get(key);
        }

        public bool Delete(string key)
        {
            return Cache.Remove(key) != null;
        }

        public bool KeyExists(string key)
        {
            return Cache.Get(key) != null;
        }

        public bool Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;
            Cache.Set(key, value, new CacheItemPolicy());
            return true;
        }

        public bool Set(string key, object value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key) || value == null || expiry < DateTime.Now)
                return false;
            var cacheItem = new CacheItem(key, value);
            var cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = expiry;
            Cache.Set(cacheItem, cacheItemPolicy);
            return true;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
