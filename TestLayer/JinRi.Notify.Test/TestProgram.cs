using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyNetQ;
using EasyNetQ.Loggers;
using EasyNetQ.Topology;
using JinRi.Notify.Frame;
using JinRi.Notify.Business;
using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Utility;
using System.Diagnostics;
using JinRi.Notify.ServiceModel;
using JinRi.Notify.DB;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestProgram
    {
        [TestMethod]
        public void TestDataBufferPool()
        {
            try
            {
                DateTime d = default(DateTime);

                DateTime d1 = JinRiDBFacade.Instance.GetDateTimeNow();

            }
            catch
            {

            }
            int count = 0;
            IDataBufferPool pool = new DataBufferPool((x) =>
            {
                List<object> arr = ((DataBuffer)x).GetList<object>();
                Interlocked.Add(ref count, arr.Count);
                Debug.WriteLine(count);
            }, false);
            for (int i = 0; i < 1000000; i++)
            {
                pool.Write(i);
                //if (i % 100 == 0)
                //System.out.println(i);
            }

            pool.Flush();
            Thread.Sleep(15000);
            Assert.Equals(1000000, count);
        }

        /// <summary>
        /// 接收程序
        /// </summary>
        [TestMethod]
        public void TestReceiveFacade()
        {
            ReceiveFacade reFad = new ReceiveFacade();

            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            //High队列,OrderTicketOut
            List<NotifyMessage> list = GetList(MessagePriorityEnum.High, "OrderTicketOut");
            foreach (NotifyMessage mess in list)
            {
                reFad.Receive(mess);
            }

            //Middle队列,OrderPayResult
            list = GetList(MessagePriorityEnum.Middle, "OrderPayResult");
            foreach (NotifyMessage mess in list)
            {
                reFad.Receive(mess);
            }
            //Normal队列,NotifyZBNTicketOut
            list = GetList(MessagePriorityEnum.Normal, "OrderReturnResult");
            foreach (NotifyMessage mess in list)
            {
                reFad.Receive(mess);
            }
            //Low队列,NotifyZBNTicketOut
            list = GetList(MessagePriorityEnum.Low, "OrderCancel");
            foreach (NotifyMessage mess in list)
            {
                reFad.Receive(mess);
            }
            watch.Stop();
            long min = watch.ElapsedMilliseconds;
            string s = "";
        }


        /// <summary>
        /// 多线程接收程序
        /// </summary>
        [TestMethod]
        public void TestReceiveFacadeByThread()
        {
            Parallel.Invoke(TestReceiveHigh, TestReceiveMiddle, TestReceiveNormal, TestReceiveLow);        
        }

        /// <summary>
        /// 生成程序
        /// </summary>
        [TestMethod]
        public void TestBuildFacade()
        {
            BuildFacade buFad = new BuildFacade();
            buFad.Build();              
        }

        JinRi.Notify.ServiceAgent.ReceiverReceiveServiceSOA.ReceiveServiceClient receiveServiceClient = new ServiceAgent.ReceiverReceiveServiceSOA.ReceiveServiceClient();
        [TestMethod]
        public void TestReceiveHigh()
        {
            ReceiveFacade reFad = new ReceiveFacade();
            //High队列,OrderTicketOut
            List<NotifyMessage> list = GetList(MessagePriorityEnum.High, "OrderTicketOut");
            foreach (NotifyMessage mess in list)
            {
                receiveServiceClient.Notify(mess);
                //reFad.Receive(mess);                
            }           
        }
        [TestMethod]
        public void TestReceiveMiddle()
        {
            ReceiveFacade reFad = new ReceiveFacade();
            //Middle队列,OrderPayResult
            List<NotifyMessage> list = GetList(MessagePriorityEnum.Middle, "OrderPayResult");
            foreach (NotifyMessage mess in list)
            {
                receiveServiceClient.Notify(mess);
                //reFad.Receive(mess);
            }
        }
        [TestMethod]
        public void TestReceiveNormal()
        {
            ReceiveFacade reFad = new ReceiveFacade();
            //Normal队列,NotifyZBNTicketOut
            List<NotifyMessage> list = GetList(MessagePriorityEnum.Normal, "OrderReturnResult");
            foreach (NotifyMessage mess in list)
            {
                receiveServiceClient.Notify(mess);
                //reFad.Receive(mess);
            }
        }
        [TestMethod]
        public void TestReceiveLow()
        {
            ReceiveFacade reFad = new ReceiveFacade();
            //Low队列,OrderCancel
            List<NotifyMessage> list = GetList(MessagePriorityEnum.Low, "OrderCancel");
            foreach (NotifyMessage mess in list)
            {
                receiveServiceClient.Notify(mess);
               // reFad.Receive(mess);
            }
        }

        /// <summary>
        /// 重扫程序
        /// </summary>
        [TestMethod]
        public void TestRedoFacade()
        {
            RedoFacade reFad = new RedoFacade();
            reFad.Redo();
            Thread.Sleep(500000);
        }

        /// <summary>
        /// 测试流程
        /// </summary>
        [TestMethod]
        public void TestProcess()
        {
            //TestReceiveFacade();
            TestReceiveFacadeByThread();
            TestBuildFacade();
            Thread.Sleep(50000000);
        }

        /// <summary>
        /// 多线程接收程序
        /// </summary>
        [TestMethod]
        public void TestPublish()
        {
            JinRi.Notify.ServiceAgent.BuilderReceiveServiceSOA.DirectReceiveServiceClient client = new JinRi.Notify.ServiceAgent.BuilderReceiveServiceSOA.DirectReceiveServiceClient();
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            TestReceiveHigh();
            TestReceiveMiddle();
            TestReceiveNormal();
            TestReceiveLow();
            watch.Stop();
            long min = watch.ElapsedMilliseconds;
            Thread.Sleep(50000000);
        }

        /// <summary>
        /// 测试流程
        /// </summary>
        [TestMethod]
        public void TestBuild()
        {
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            TestBuildFacade();
            Thread.Sleep(50000000);
            long min = watch.ElapsedMilliseconds;
        }

        [TestMethod]
        public void TestProcessAysan()
        {
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            ThreadPool.QueueUserWorkItem(x => { TestReceiveFacadeByThread(); });
            TestBuildFacade();
            watch.Stop();
            long min = watch.ElapsedMilliseconds;
            Thread.Sleep(50000000);
        }

        [TestMethod]
        public void TestRequestProfile()
        {
            RequestProfile.RequestType = "JinRi.Notify.SenderService.Receive";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.Username = "";
            DateTime dt = ComputePushTimeBusiness.GetDatabaseTime();
        }
        [TestMethod]
        public List<NotifyMessage> GetList(MessagePriorityEnum mesPrvEnm, string messTypeEnm)
        {
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            List<NotifyMessage> list = new List<NotifyMessage>();
            int count = 1000;
            for (int i = 0; i < count; i++)
            {
                NotifyMessage mess = new NotifyMessage()
                {
                    MessageId = IdentityGenerator.New(),
                    AppId = "11011",
                    MessagePriority = mesPrvEnm,
                    MessageKey = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + mesPrvEnm.ToString() + "_" + i.ToString(),
                    MessageType = messTypeEnm.ToString(),
                    NotifyData = "a=1&b=2",
                    SourceFrom = "test",
                    ClientIP = "127.0.0.1",
                    CreateTime = DateTime.Now
                };
                list.Add(mess);
            }

            watch.Stop();
            long min = watch.ElapsedMilliseconds;
            return list;
        }

        [TestMethod]
        public void SaveNotifyMessage()
        {
            NotifyMessageBusiness bus = new NotifyMessageBusiness();
            int count = 10;
            List<NotifyMessage> list = new List<NotifyMessage>();
            for (int i = 0; i < count; i++)
            {
                NotifyMessage mess = new NotifyMessage()
                {
                    MessageId = IdentityGenerator.New(),
                    AppId = "11011",
                    MessagePriority = MessagePriorityEnum.High,
                    MessageKey = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + "OrderTicketOut" + "_" + i.ToString(),
                    MessageType = "OrderTicketOut",
                    NotifyData = "a=1&b=2",
                    SourceFrom = "test",
                    ClientIP = "127.0.0.1",
                    CreateTime = DateTime.Now
                };
                list.Add(mess);
            }
            Stopwatch watch = Stopwatch.StartNew();

            bus.Save(list);

            watch.Stop();
            long min = watch.ElapsedMilliseconds;
        }

        [TestMethod]
        public void ReceiveHandler_Notify()
        {         
            string messJson = "{AppId:11011,MessagePriority:'High',MessageKey:'20151025_OrderTicketOut_2',MessageType:'OrderTicketOut',NotifyData:'a=1&b=2',SourceFrom:'Test',ClientIP:'127.0.0.1'}";
            NotifyMessage notifyMess = JsonConvert.DeserializeObject<NotifyMessage>(messJson);
        }

          [TestMethod]
        public void DeteleNotifyMess()
        {
            NotifyMessageBusiness bus = new NotifyMessageBusiness();
            bool row = bus.Delete("2bb9d5507b0a422fb60336a41bed722c");
        }
    }
}
