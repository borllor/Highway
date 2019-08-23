using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinRi.Notify.Frame
{
    public interface ICacheProvider
    {
        bool Add(string key, object value);
        bool Add(string key, object value, DateTime expiry);

        object Get(string key);
        
        bool Delete(string key);

        bool KeyExists(string key);

        bool Set(string key, object value);
        bool Set(string key, object value, DateTime expiry);

        IDictionaryEnumerator GetEnumerator();
    }
}
