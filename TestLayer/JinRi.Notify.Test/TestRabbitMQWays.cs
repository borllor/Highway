using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyNetQ;
using JinRi.Notify.Utility;
using EasyNetQ.Loggers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using EasyNetQ.Topology;
using System.Text;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestRabbitMQWays
    {
        private IBus m_bus = null;
        private StringBuilder m_builder = new StringBuilder();
        private static readonly object m_builderLocker = new object();

        [TestMethod]
        public void TestQueueWay()
        {
            m_bus = RabbitHutch.CreateBus(ConfigurationAppSetting.RabbitMQHost, reg => reg.Register<IEasyNetQLogger>(log => new Log4NetLogger()));
            string queueName = "JinRi.Notify.Test.TestQueueWay";
            m_bus.Receive<string>(queueName, (a) =>
            {
                WriteString(string.Format("Thread1, {0}", a));
            });
            m_bus.Receive<string>(queueName, (a) =>
            {
                WriteString(string.Format("Thread2, {0}", a));
            });
            m_bus.Receive<string>(queueName, (a) =>
            {
                WriteString(string.Format("Thread3, {0}", a));
            });
            m_bus.Receive<string>(queueName, (a) =>
            {
                WriteString(string.Format("Thread4, {0}", a));
            });
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                m_bus.Send<string>(queueName, i.ToString());
            }

            //List<Task> taskList = new List<Task>();
            //for (int i = 0; i < 5; i++)
            //{
            //    Task t = new Task(() =>
            //    {

            //    });
            //    taskList.Add(t);
            //    t.Start();
            //}
            Thread.Sleep(5000);
            string s = m_builder.ToString();
            Debug.WriteLine(Environment.NewLine);
            Debug.WriteLine(Environment.NewLine);
            Debug.WriteLine(Environment.NewLine);
            Debug.WriteLine(Environment.NewLine);
            Debug.WriteLine(Environment.NewLine);
            Debug.WriteLine(s);
        }

        private void WriteString(string s)
        {
            lock (m_builderLocker)
            {
                m_builder.AppendLine(s);
            }
        }


        [TestMethod]
        public void TestQueueWay2()
        {
            IAdvancedBus bus = RabbitHutch.CreateBus(ConfigurationAppSetting.RabbitMQHost, reg => reg.Register<IEasyNetQLogger>(log => new Log4NetLogger())).Advanced;
            IExchange ex = bus.ExchangeDeclare("JinRi.Notify.Test.TestQueueWay2Ex", "fanout");
            IQueue qu1 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay2Queue1");
            IQueue qu2 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay2Queue2");
            IQueue qu3 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay2Queue3");
            IBinding bi1 = bus.Bind(ex, qu1, "");
            IBinding bi2 = bus.Bind(ex, qu2, "");
            IBinding bi3 = bus.Bind(ex, qu3, "");
            for (int i = 0; i < 100; i++)
            {
                int md = i % 3;
                bus.Publish<string>(ex, "", false, false, new Message<string>(i.ToString()));
            }
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void TestQueueWay3()
        {
            IAdvancedBus bus = RabbitHutch.CreateBus(ConfigurationAppSetting.RabbitMQHost, reg => reg.Register<IEasyNetQLogger>(log => new Log4NetLogger())).Advanced;
            IExchange ex = bus.ExchangeDeclare("JinRi.Notify.Test.TestQueueWay3Ex", "direct");
            IQueue qu1 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay3Queue1");
            IQueue qu2 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay3Queue2");
            IQueue qu3 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay3Queue3");
            IBinding bi1 = bus.Bind(ex, qu1, "0");
            IBinding bi2 = bus.Bind(ex, qu2, "1");
            IBinding bi3 = bus.Bind(ex, qu3, "2");
            for (int i = 0; i < 100; i++)
            {
                int md = i % 3;
                bus.Publish<string>(ex, md.ToString(), false, false, new Message<string>(i.ToString()));
            }
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void TestQueueWay4()
        {
            IAdvancedBus bus = RabbitHutch.CreateBus(ConfigurationAppSetting.RabbitMQHost, reg => reg.Register<IEasyNetQLogger>(log => new Log4NetLogger())).Advanced;
            IExchange ex = bus.ExchangeDeclare("JinRi.Notify.Test.TestQueueWay4Ex", "topic");
            IQueue qu1 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay4Queue1");
            IQueue qu2 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay4Queue2");
            IQueue qu3 = bus.QueueDeclare("JinRi.Notify.Test.TestQueueWay4Queue3");
            IBinding bi1 = bus.Bind(ex, qu1, "*.0.*");
            IBinding bi2 = bus.Bind(ex, qu2, "*.1.*");
            IBinding bi3 = bus.Bind(ex, qu3, "*.2.*");
            for (int i = 0; i < 100; i++)
            {
                int md = i % 3;
                bus.Publish<string>(ex, "a." + md.ToString() + ".b", false, false, new Message<string>(i.ToString()));
            }

            bus.Consume<string>(qu1, (a, b) =>
            {
                Debug.WriteLine(a.Body);
            });
            Thread.Sleep(5000);
        }
    }
}
