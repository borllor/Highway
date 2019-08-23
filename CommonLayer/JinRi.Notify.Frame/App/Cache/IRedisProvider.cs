#if NET40
using System;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using System.Collections;
using ServiceStack.Redis;

namespace JinRi.Notify.Frame
{
    public interface IRedisProvider : ICacheProvider, IRedisClient
    {
    }
}
#endif
