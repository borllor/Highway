using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace JinRi.Notify.Frame
{
    internal class WebCacheProvider : ICacheProvider
    {
        private static Cache _cache;
		
        protected static Cache Cache
        {
            get
            {
                return _cache ?? (_cache = HttpRuntime.Cache);
            }
        }

        public WebCacheProvider()
        {
            _cache = HttpRuntime.Cache;
        }

        #region ICacheProvider 成员

        public bool Add(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;
            object obj = Cache.Add(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            return obj == null;
        }

        public bool Add(string key, object value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key) || value == null || expiry < DateTime.Now)
                return false;
            object obj = Cache.Add(key, value, null, expiry, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            return obj == null;
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
            Cache.Insert(key, value);
            return true;
        }

        public bool Set(string key, object value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key) || value == null || expiry < DateTime.Now)
                return false;
            Cache.Insert(key, value, null, expiry, Cache.NoSlidingExpiration);
            return true;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return Cache.GetEnumerator();
        }

        #endregion
    }
}
