using System;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using System.Collections;

namespace JinRi.Notify.Frame
{
    public interface IMemcachedProvider : ICacheProvider
    {
        long CompressionThreshold { get; set; }
        string DefaultEncoding { get; set; }
        bool EnableCompression { get; set; }
        string PoolName { get; set; }
        bool PrimitiveAsString { get; set; }

        bool Add(string key, object value, int hashCode);
        bool Add(string key, object value, DateTime expiry, int hashCode);
        long Decrement(string key);
        long Decrement(string key, long inc);
        long Decrement(string key, long inc, int hashCode);
        bool Delete(string key, DateTime expiry);
        bool Delete(string key, object hashCode, DateTime expiry);
        bool FlushAll();
        bool FlushAll(ArrayList servers);
        object Get(string key, int hashCode);
        object Get(string key, object hashCode, bool asString);
        long GetCounter(string key);
        long GetCounter(string key, object hashCode);
        Hashtable GetMultiple(string[] keys);
        Hashtable GetMultiple(string[] keys, int[] hashCodes);
        Hashtable GetMultiple(string[] keys, int[] hashCodes, bool asString);
        object[] GetMultipleArray(string[] keys);
        object[] GetMultipleArray(string[] keys, int[] hashCodes);
        object[] GetMultipleArray(string[] keys, int[] hashCodes, bool asString);
        long Increment(string key);
        long Increment(string key, long inc);
        long Increment(string key, long inc, int hashCode);
        bool Replace(string key, object value);
        bool Replace(string key, object value, DateTime expiry);
        bool Replace(string key, object value, int hashCode);
        bool Replace(string key, object value, DateTime expiry, int hashCode);
        bool Set(string key, object value, int hashCode);
        bool Set(string key, object value, DateTime expiry, int hashCode);
        Hashtable Stats();
        Hashtable Stats(ArrayList servers);
        bool StoreCounter(string key, long counter);
        bool StoreCounter(string key, long counter, int hashCode);
    }
}
