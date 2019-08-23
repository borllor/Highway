using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JinRi.Notify.DB;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class UnitJinRiNotifyFacade
    {
        [TestMethod]
        public void TestExecDataMove()
        {
            JinRiNotifyFacade facade = new JinRiNotifyFacade();
            int count = 0;
            int row = facade.ExecNotifyMessageDataMove(53);
            while (row > 0)
            {
                count += row / 3;
                row = facade.ExecNotifyMessageDataMove(53);
            }
        }

        [TestMethod]
        public void TestExecDataPushMove()
        {
            JinRiNotifyFacade facade = new JinRiNotifyFacade();
            int count = 0;
            int row = facade.ExecPushMessageDataMove(53);
            while (row > 0)
            {
                count += row / 3;
                row = facade.ExecPushMessageDataMove(53);
            }
        }
    }
}
