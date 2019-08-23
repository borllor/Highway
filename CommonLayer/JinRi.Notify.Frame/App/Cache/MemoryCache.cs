using System;
using System.Collections;
using System.Runtime.Remoting;

namespace JinRi.Notify.Frame
{
    public class MemoryCache
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MemoryCache));
        private const string CachePrefix = "JinRi_";

        #region 单例对象实现

        private static readonly ICacheProvider m_providerInstance = new MemoryCacheProvider();

        public static ICacheProvider GetProvider()
        {          
            return m_providerInstance;
        }

        #endregion

        #region CacheKey处理方法

        public static string CleanCacheKey(string CacheKey)
        {
            if (String.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            return CacheKey.Substring(CachePrefix.Length);
        }

        public static string GetCacheKey(string CacheKey)
        {
            if (string.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            return CachePrefix + CacheKey;
        }

        #endregion

        #region ICacheProvider 成员

        public static bool Add(string key, object value)
        {
            return GetProvider().Add(GetCacheKey(key), value);
        }

        public static bool Add(string key, object value, DateTime expiry)
        {
            return GetProvider().Add(GetCacheKey(key), value, expiry);
        }

        public static object Get(string key)
        {
            return GetProvider().Get(GetCacheKey(key));
        }

        public static bool Delete(string key)
        {
            return GetProvider().Delete(GetCacheKey(key));
        }

        public static bool KeyExists(string key)
        {
            return GetProvider().KeyExists(GetCacheKey(key));
        }

        public static bool Set(string key, object value)
        {
            return GetProvider().Set(GetCacheKey(key), value);
        }

        public static bool Set(string key, object value, DateTime expiry)
        {
            return GetProvider().Set(GetCacheKey(key), value, expiry);
        }

        public static IDictionaryEnumerator GetEnumerator()
        {
            return GetProvider().GetEnumerator();
        }

        #endregion
    }
}
