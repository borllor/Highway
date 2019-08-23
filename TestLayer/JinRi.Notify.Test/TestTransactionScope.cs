using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestTransactionScope
    {
        [TestMethod]
        public void Test1()
        {
            int i = 2;
            string s = "";
            using (var scope = new TransactionScope())
            {
                if (i == 2)
                {
                    s = i + Environment.NewLine;
                }
                if (i -1 == 1)
                {
                    s = s + (i - 1) + Environment.NewLine;
                }
                if (i != 3)
                {
                    throw new Exception("test");
                }

                // Commit the transaction
                scope.Complete();
            }
            Debug.WriteLine(s);
            Thread.Sleep(10000000);
        }
    }
}
