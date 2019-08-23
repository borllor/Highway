using JinRi.Notify.Frame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestRedis
    {
        [TestMethod]
        public void TestAdd()
        {
            object tmp = null;
            bool result = RedisCache.Add("orderno", "lixiaobo");
            result = RedisCache.Set("orderno111", "lixiaobo111");
            tmp = RedisCache.GetAndSetEntry("orderno333", "lixiaobo333");
        }
    }
}
