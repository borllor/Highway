using System;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using System.Collections;

namespace JinRi.Notify.Frame
{
    internal class MemcachedProvider : IMemcachedProvider
    {
        private static MemcachedSockIOPool m_pool;
        private static MemcachedClient m_memcachedClientIns;

        public MemcachedProvider()
        {
            m_pool = new MemcachedSockIOPool();
            m_memcachedClientIns = new MemcachedClient();
        }

        public long CompressionThreshold
        {
            get
            {
                return m_memcachedClientIns.CompressionThreshold;
            }
            set
            {
                m_memcachedClientIns.CompressionThreshold = value;
            }
        }

        public string DefaultEncoding
        {
            get
            {
                return m_memcachedClientIns.DefaultEncoding;
            }
            set
            {
                m_memcachedClientIns.DefaultEncoding = value;
            }
        }

        public bool EnableCompression
        {
            get
            {
                return m_memcachedClientIns.EnableCompression;
            }
            set
            {
                m_memcachedClientIns.EnableCompression = value;
            }
        }

        public string PoolName
        {
            get
            {
                return m_memcachedClientIns.PoolName;
            }
            set
            {
                m_memcachedClientIns.PoolName = value;
            }
        }

        public bool PrimitiveAsString
        {
            get
            {
                return m_memcachedClientIns.PrimitiveAsString;
            }
            set
            {
                m_memcachedClientIns.PrimitiveAsString = value;
            }
        }

        public bool Add(string key, object value)
        {
            return m_memcachedClientIns.Add(key, value);
        }

        public bool Add(string key, object value, DateTime expiry)
        {
            return m_memcachedClientIns.Add(key, value, expiry);
        }

        public bool Add(string key, object value, int hashCode)
        {
            return m_memcachedClientIns.Add(key, hashCode);
        }

        public bool Add(string key, object value, DateTime expiry, int hashCode)
        {
            return m_memcachedClientIns.Add(key, value, expiry, hashCode);
        }

        public long Decrement(string key)
        {
            return m_memcachedClientIns.Decrement(key);
        }

        public long Decrement(string key, long inc)
        {
            return m_memcachedClientIns.Decrement(key, inc);
        }

        public long Decrement(string key, long inc, int hashCode)
        {
            return m_memcachedClientIns.Decrement(key, inc, hashCode);
        }

        public bool Delete(string key)
        {
            return m_memcachedClientIns.Delete(key);
        }

        public bool Delete(string key, DateTime expiry)
        {
            return m_memcachedClientIns.Delete(key, expiry);
        }

        public bool Delete(string key, object hashCode, DateTime expiry)
        {
            return m_memcachedClientIns.Delete(key, hashCode, expiry);
        }

        public bool FlushAll()
        {
            return m_memcachedClientIns.FlushAll();
        }

        public bool FlushAll(ArrayList servers)
        {
            return m_memcachedClientIns.FlushAll(servers);
        }

        public object Get(string key)
        {
            return m_memcachedClientIns.Get(key);
        }

        public object Get(string key, int hashCode)
        {
            return m_memcachedClientIns.Get(key, hashCode);
        }

        public object Get(string key, object hashCode, bool asString)
        {
            return m_memcachedClientIns.Get(key, hashCode, asString);
        }

        public long GetCounter(string key)
        {
            return m_memcachedClientIns.GetCounter(key);
        }

        public long GetCounter(string key, object hashCode)
        {
            return m_memcachedClientIns.GetCounter(key, hashCode);
        }

        public Hashtable GetMultiple(string[] keys)
        {
            return m_memcachedClientIns.GetMultiple(keys);
        }

        public Hashtable GetMultiple(string[] keys, int[] hashCodes)
        {
            return m_memcachedClientIns.GetMultiple(keys, hashCodes);
        }

        public Hashtable GetMultiple(string[] keys, int[] hashCodes, bool asString)
        {
            return m_memcachedClientIns.GetMultiple(keys, hashCodes, asString);
        }

        public object[] GetMultipleArray(string[] keys)
        {
            return m_memcachedClientIns.GetMultipleArray(keys);
        }

        public object[] GetMultipleArray(string[] keys, int[] hashCodes)
        {
            return m_memcachedClientIns.GetMultipleArray(keys, hashCodes);
        }

        public object[] GetMultipleArray(string[] keys, int[] hashCodes, bool asString)
        {
            return m_memcachedClientIns.GetMultipleArray(keys, hashCodes, asString);
        }

        public long Increment(string key)
        {
            return m_memcachedClientIns.Increment(key);
        }

        public long Increment(string key, long inc)
        {
            return m_memcachedClientIns.Increment(key, inc);
        }

        public long Increment(string key, long inc, int hashCode)
        {
            return m_memcachedClientIns.Increment(key, inc, hashCode);
        }

        public bool KeyExists(string key)
        {
            return m_memcachedClientIns.KeyExists(key);
        }

        public bool Replace(string key, object value)
        {
            return m_memcachedClientIns.Replace(key, value);
        }

        public bool Replace(string key, object value, DateTime expiry)
        {
            return m_memcachedClientIns.Replace(key, value, expiry);
        }

        public bool Replace(string key, object value, int hashCode)
        {
            return m_memcachedClientIns.Replace(key, value, hashCode);
        }

        public bool Replace(string key, object value, DateTime expiry, int hashCode)
        {
            return m_memcachedClientIns.Replace(key, value, expiry, hashCode);
        }

        public bool Set(string key, object value)
        {
            return m_memcachedClientIns.Set(key, value);
        }

        public bool Set(string key, object value, DateTime expiry)
        {
            return m_memcachedClientIns.Set(key, value, expiry);
        }

        public bool Set(string key, object value, int hashCode)
        {
            return m_memcachedClientIns.Set(key, value, hashCode);
        }

        public bool Set(string key, object value, DateTime expiry, int hashCode)
        {
            return m_memcachedClientIns.Set(key, value, expiry, hashCode);
        }

        public Hashtable Stats()
        {
            return m_memcachedClientIns.Stats();
        }

        public Hashtable Stats(ArrayList servers)
        {
            return m_memcachedClientIns.Stats(servers);
        }

        public bool StoreCounter(string key, long counter)
        {
            return m_memcachedClientIns.StoreCounter(key, counter);
        }

        public bool StoreCounter(string key, long counter, int hashCode)
        {
            return m_memcachedClientIns.StoreCounter(key, counter, hashCode);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            Hashtable table = m_memcachedClientIns.Stats();
            return table != null ? table.GetEnumerator() : null;
        }
    }
}
