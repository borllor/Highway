using JinRi.Notify.DTO;
using JinRi.Notify.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestCheck
    {
        [TestMethod]
        public void TestJsonData()
        {
            string key = "t123";
            string val = @"{'AppId':'100700','CreateTime':'2019-08-02T18:20:04','MessageId':'df413b80-f830-4596-9d07-f0e5bcc798c8','MessageKey':'W2019080204024916885','MessagePriority':0,'MessageType':'OrderCancel','NotifyData':'orderno=W2019080204024916885&OutTime=2019-08-02 18:20:04&SalesmanID=0&ProviderID=898941&ProxyerID=955979','SourceFrom':'Interface'}";
            Check.IsNull(key, val);
            NotifyMessage o;
            try
            {
                Check.CanDeserializeObject(val, key, out o);
            }
            catch {

            }

        }
    }
}
