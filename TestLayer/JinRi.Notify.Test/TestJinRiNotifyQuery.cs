using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JinRi.Notify.DB;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestJinRiNotifyQuery
    {
        [TestMethod]
        public void GetPushMessageByID_Test()
        {
            var result = JinRiNotifyFacade.Instance.GetPushMessageByID("726fb8956d92401c989c94e2dab09532");
            Console.Write(result.NextPushTime);
        }
    }
}
