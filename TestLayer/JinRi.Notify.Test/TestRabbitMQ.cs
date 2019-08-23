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
using Newtonsoft.Json;
using JinRi.Notify.Frame;
using JinRi.Notify.Business;
using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using System.Configuration;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestRabbitMQ
    {
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(TestRabbitMQ));

        /// <summary>
        /// 生产消息
        /// Step1
        /// </summary>
        [TestMethod]
        public void TestPublish()
        {
            int count = 3000;
            for (int i = 0; i < count; i++)
            {
                NotifyMessage mess = new NotifyMessage()
              {
                  MessageId = IdentityGenerator.New(),
                  AppId = "11011",
                  MessagePriority = MessagePriorityEnum.High,
                  MessageKey = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + MessagePriorityEnum.High.ToString() + "_" + i.ToString(),
                  MessageType = "OrderTicketOut",
                  NotifyData = "a=1&b=2",
                  SourceFrom = "test",
                  ClientIP = "127.0.0.1",// ClientProfile.Current.ClientIP,
                  CreateTime = DateTime.Now
              };
                RabbitMQBusiness.Instance.Publisher(mess);
                Thread.Sleep(10);
            }

            for (int i = 0; i < count; i++)
            {
                NotifyMessage mess = new NotifyMessage()
                {
                    MessageId = IdentityGenerator.New(),
                    AppId = "11012",
                    MessagePriority = MessagePriorityEnum.Middle,
                    MessageKey = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + MessagePriorityEnum.Middle.ToString() + "_" + i.ToString(),
                    MessageType = "OrderPayResult",
                    NotifyData = "a=1&b=2",
                    SourceFrom = "test",
                    ClientIP = "127.0.0.1",// ClientProfile.Current.ClientIP,                  
                    CreateTime = DateTime.Now
                };

                RabbitMQBusiness.Instance.Publisher(mess);
                Thread.Sleep(10);
            }

            for (int i = 0; i < count; i++)
            {
                NotifyMessage mess = new NotifyMessage()
                {
                    MessageId = IdentityGenerator.New(),
                    AppId = "11013",
                    MessagePriority = MessagePriorityEnum.Normal,
                    MessageKey = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + MessagePriorityEnum.Normal.ToString() + "_" + i.ToString(),
                    MessageType = "NotifyZBNTicketOut",
                    NotifyData = "a=1&b=2",
                    SourceFrom = "test",
                    ClientIP = "127.0.0.1",// ClientProfile.Current.ClientIP,
                    CreateTime = DateTime.Now
                };

                RabbitMQBusiness.Instance.Publisher(mess);
                Thread.Sleep(10);
            }

            for (int i = 0; i < count; i++)
            {
                NotifyMessage mess = new NotifyMessage()
                {
                    MessageId = IdentityGenerator.New(),
                    AppId = "11014",
                    MessagePriority = MessagePriorityEnum.Low,
                    MessageKey = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + MessagePriorityEnum.Low.ToString() + "_" + i.ToString(),
                    MessageType = "OrderCancel",
                    NotifyData = "a=1&b=2",
                    SourceFrom = "test",
                    ClientIP = "127.0.0.1",//ClientProfile.Current.ClientIP,
                    CreateTime = DateTime.Now
                };

                RabbitMQBusiness.Instance.Publisher(mess);
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 接收消息
        /// Step2
        /// </summary>
        [TestMethod]
        public void TestParallel()
        {
            ParallelSubscribeBusiness _bus = new ParallelSubscribeBusiness();
            _bus.Parallel();
        }

        [TestMethod]
        public void TestPrograme()
        {
            TestPublish();
            TestParallel();
            Thread.Sleep(5000000);
        }

        [TestMethod]
        public void TestMQ()
        {
            var bus = RabbitHutch.CreateBus(ConfigurationAppSetting.RabbitMQHost, reg => reg.Register<IEasyNetQLogger>(log => new Log4NetLogger())).Advanced;
            var queue = bus.QueueDeclare("Test");
            var exchange = bus.ExchangeDeclare("JinRiNotifyExchangeTest", ExchangeType.Topic);
            var binding = bus.Bind(exchange, queue, "");
            var properties = new MessageProperties();
            for (int i = 0; i < 100; i++)
            {
                NotifyMessage mess = new NotifyMessage()
                {
                    MessageId = IdentityGenerator.New(),
                    AppId = "11011",
                    MessagePriority = MessagePriorityEnum.High,
                    MessageKey = MessagePriorityEnum.High.ToString(),
                    MessageType = "OrderTicketOut",
                    NotifyData = "a=1&b=2",
                    SourceFrom = "test",
                    ClientIP = "127.0.0.1"
                };
                var body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mess));
                bus.Publish(exchange, "", false, false, properties, body);
            }
            bus.Consume(queue, (b, ps, info) => Task.Factory.StartNew(() =>
            {
                var message = Encoding.UTF8.GetString(b);
                RabbitMQBusiness.ComsumeMessage(JsonConvert.DeserializeObject<NotifyMessage>(message));
            }));
        }

        [TestMethod]
        public void TestProfile()
        {
            //var clientIP = ClientProfile.ClientIP;
            //var serverIP = ServerProfile.ServerIP;
        }


        /// <summary>
        /// 测试保存NotifyMessage
        /// </summary>
        [TestMethod]
        public void TestSaveNotifyMessage()
        {
            NotifyMessageBusiness notify = new NotifyMessageBusiness();
            NotifyMessage mess = new NotifyMessage()
               {
                   MessageId = IdentityGenerator.New(),
                   AppId = "11011",
                   MessagePriority = MessagePriorityEnum.High,
                   MessageKey = MessagePriorityEnum.High.ToString() + "_" + new Random().Next(1, 11).ToString(),
                   MessageType = "OrderTicketOut",
                   NotifyData = "a=1&b=2",
                   SourceFrom = "test",
                   ClientIP = "127.0.0.1"
               };
            notify.Save(mess);

        }

        [TestMethod]
        public void TestBuild()
        {
            BuildMessageBusiness notify = new BuildMessageBusiness();
            NotifyMessage mess = new NotifyMessage()
            {
                MessageId = IdentityGenerator.New(),
                AppId = "11011",
                MessagePriority = MessagePriorityEnum.High,
                MessageKey = MessagePriorityEnum.High.ToString() + "_" + new Random().Next(1, 11).ToString(),
                MessageType = "OrderTicketOut",
                NotifyData = "a=1&b=2",
                SourceFrom = "test",
                ClientIP = "127.0.0.1"
            };
            notify.Build(mess);

        }

        /// <summary>
        /// 测试保存PushMessage
        /// </summary>
        [TestMethod]
        public void TestSavePushMessage()
        {
            BuildMessageBusiness buildBus = new BuildMessageBusiness();
            PushMessageBusiness pushBus = new PushMessageBusiness();
            NotifyMessage mess = new NotifyMessage()
            {
                MessageId = IdentityGenerator.New(),
                AppId = "11012",
                MessagePriority = MessagePriorityEnum.High,
                MessageKey = MessagePriorityEnum.High.ToString() + "_" + new Random().Next(1, 11).ToString(),
                MessageType = "OrderTicketOut",
                NotifyData = "a=1&b=2",
                SourceFrom = "test",
                ClientIP = "127.0.0.1"
            };

            List<PushMessageModel> list = buildBus.GetType().InvokeMember("BuildMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance, System.Type.DefaultBinder, buildBus, new object[] { mess }) as List<PushMessageModel>;
            pushBus.Save(list);
        }
        /// <summary>
        /// 测试mysql数据库连接
        /// </summary>
        [TestMethod]
        public void TestMySqlDBConn()
        {
            NotifyInterfaceSettingBusiness _pushSettingBus = new NotifyInterfaceSettingBusiness();
            _pushSettingBus.Get(1);
        }

        /// <summary>
        /// 测试发送
        /// </summary>
        [TestMethod]
        public void TestSendPushMessage()
        {
            SendMessageBusiness sendBus = new SendMessageBusiness();
            PushMessageBusiness pushBus = new PushMessageBusiness();

            List<PushMessageModel> list = new List<PushMessageModel>();
            list.Add(pushBus.GetFromCache("0907396caecb4ce9be68362b07276e64"));
            foreach (PushMessageModel m in list)
            {
                if (m.NextPushTime <= DateTime.Now)
                {
                    PushMessage pushDTO = MappingHelper.From<PushMessage, PushMessageModel>(m);
                    //notifyBus.SendPushMessage(pushDTO);
                    sendBus.Send(pushDTO, sendBus.Callback);
                }
            }
            Thread.Sleep(100000);
        }

        [TestMethod]
        public void TestSubscribe()
        {
            BuildMessageBusiness _buildBus = new BuildMessageBusiness();
            RabbitMQBusiness.Instance.Subscribe(MessagePriorityEnum.High, _buildBus.Build);
        }

        [TestMethod]
        public void TestThread()
        {
            Thread thread = new Thread(new ThreadStart(StartComsume));
            thread.Name = "TestThread";
            thread.Start();
            Thread.Sleep(10000);
            string s = "ss";
            Thread.Sleep(100000);
        }

        private void StartComsume()
        {
            RabbitMQBusiness bus = new RabbitMQBusiness();
            bus.Subscribe(MessagePriorityEnum.High, Comsume);
        }

        private void Comsume(NotifyMessage message)
        {
            if (message != null)
            {

            }
        }

        [TestMethod]
        public void TestNormalPublish()
        {
            for (int i = 0; i < 10000; i++)
            {
                RoyaltyDTO dto = new RoyaltyDTO()
                {
                    AppID = 100000,
                    OrderNo = "00000000000000000000",
                    UserID = 11,
                    UserName = "1111111111"
                };
                EasyNetQHelper.GetInstance().Publish(dto);
                Thread.Sleep(100);
            }
        }
    }

    public static class EasyNetQHelper
    {
        private static IBus _bus;
        private static readonly object syncRoot = new object();
        static EasyNetQHelper()
        {
            string mqHost = ConfigurationManager.AppSettings["RabbitMQHost"];
            _bus = RabbitHutch.CreateBus(mqHost, reg => reg.Register<IEasyNetQLogger>(log => new NullLogger()));
        }

        public static IBus GetInstance()
        {
            if (_bus == null)
            {
                lock (syncRoot)
                {
                    if (_bus == null)
                    {
                        string mqHost = ConfigurationManager.AppSettings["RabbitMQHost"];
                        _bus = RabbitHutch.CreateBus(mqHost, reg => reg.Register<IEasyNetQLogger>(log => new NullLogger()));
                    }
                }
            }

            return _bus;
        }
    }

    public class RoyaltyDTO
    {
        /// <summary>
        /// 应用程序ID
        /// </summary>
        public int AppID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }
    }
}
