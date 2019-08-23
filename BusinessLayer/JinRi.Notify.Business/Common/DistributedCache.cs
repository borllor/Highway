using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JinRi.Notify.Frame;
using JinRi.Notify.Utility;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;

namespace JinRi.Notify.Business
{
    /// <summary>
    /// 针对消息中心的分布式缓存帮助类，采用了本地缓存与分布式缓存组合策略
    /// </summary>
    public class DistributedCache
    {
        private static ICacheProvider Provider
        {
            get
            {
                return MemcachedCache.GetProvider();
            }
        }

        public static string ProviderName
        {
            get
            {
                return Provider.GetType().Name;
            }
        }

        public static bool Add(string key, object value, DateTime expiry)
        {
            if (WebCache.KeyExists(key))
            {
                return false;
            }
            WebCache.Add(key, value, expiry);
            return Provider.Add(key, value, expiry);
        }

        public static bool Set(string key, object value, DateTime expiry)
        {
            WebCache.Set(key, value, expiry);
            return Provider.Set(key, value, expiry);
        }

        public static object Get(string key)
        {
            object obj = WebCache.Get(key);
            if (obj == null)
            {
                obj = Provider.Get(key);
            }
            return obj;
        }

        public static bool Delete(string key)
        {
            WebCache.Delete(key);
            return Provider.Delete(key);
        }

        public static bool KeyExists(string key)
        {
            if (WebCache.KeyExists(key))
            {
                return true;
            }
            return Provider.KeyExists(key);
        }

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllCache()
        {
            Dictionary<string, string> allCache = new Dictionary<string, string>();
            try
            {
                Type type = typeof(CacheKeys);
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo fi in fieldInfos)
                {
                    if (fi.FieldType != typeof(String))
                    {
                        continue;
                    }
                    string key = fi.GetValue(null).ToString();
                    object obj = DistributedCache.Provider.Get(key);
                    if (obj == null)
                    {
                        allCache.Add(key, FormatCacheValue(obj));
                        continue;
                    }
                    allCache.Add(key, FormatCacheValue(obj));
                }
            }
            catch (Exception)
            {
            }
            return allCache;
        }

        /// <summary>
        /// 格式化缓存值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FormatCacheValue(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            else if (obj.GetType().IsPrimitive || obj.GetType() == typeof(String))
            {
                return obj.ToString();
            }
            else
            {
                return obj.GetType().ToString();
            }
        }
    }
}
