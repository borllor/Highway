using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using EasyNetQ.Topology;
using EasyNetQ;
using Newtonsoft.Json;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.ServiceModel;


namespace JinRi.Notify.Business
{
    public class RabbitMQBusiness
    {
        private static Dictionary<string, IQueue> QueueDic = new Dictionary<string, IQueue>();
        private static Dictionary<string, IExchange> ExchangeDic = new Dictionary<string, IExchange>();
        private static Dictionary<string, IBinding> BindingDic = new Dictionary<string, IBinding>();
        public static RabbitMQBusiness Instance = new RabbitMQBusiness();
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(RabbitMQBusiness));

        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            DateFormatString = "yyyy-MM-dd HH:mm:ss.fff"
        };

        static RabbitMQBusiness()
        {
            InitQuene(RabbitMQBusMgr.Instance);
        }

        #region Subscribe

        public void Publisher(NotifyMessage message)
        {
            Publisher(RabbitMQBusMgr.Instance, message);
        }

        public void Publisher(RabbitMQBus rabbitMQ, NotifyMessage message)
        {
            try
            {
                string topic = message.MessagePriority.ToString().ToUpper();
                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, serializerSettings));
                MessageProperties prop = new MessageProperties()
                {
                    AppId = ConfigurationAppSetting.AppId,
                    Priority = (byte)message.MessagePriority,
                    MessageId = message.MessageId,
                    Type = "S"
                };
                rabbitMQ.Bus.OpenPublishChannel().Publish(ExchangeDic[topic], "", prop, body);
                MetricsKeys.RabbitMQ_Publish.MeterMark("Success");
            }
            catch (Exception ex)
            {
                MetricsKeys.RabbitMQ_Publish.MeterMark("Error");
                m_logger.Error("推送消息发生异常：" + ex.GetString());
                throw new RabbitMQException("推送消息发生异常", ex);
            }
        }

        public void Publisher(MessagePriorityEnum priority, List<NotifyMessage> messageList)
        {
            Publisher(RabbitMQBusMgr.Instance, priority, messageList);
        }

        public void Publisher(RabbitMQBus rabbitMQ, MessagePriorityEnum priority, List<NotifyMessage> messageList)
        {
            try
            {
                string topic = priority.ToString().ToUpper();
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageList, serializerSettings));
                MessageProperties prop = new MessageProperties()
                {
                    AppId = ConfigurationAppSetting.AppId,
                    Priority = (byte)priority,
                    MessageId = messageList[0].MessageId,
                    Type = "M"
                };
                rabbitMQ.Bus.OpenPublishChannel().Publish(ExchangeDic[topic], "", prop, body);
                MetricsKeys.RabbitMQ_Publish.MeterMark("Success");
            }
            catch (Exception ex)
            {
                MetricsKeys.RabbitMQ_Publish.MeterMark("Error");
                m_logger.Error("推送消息发生异常：" + ex.GetString());
                throw new RabbitMQException("推送消息发生异常", ex);
            }
        }

        #endregion

        #region Subscribe

        public void Subscribe(MessagePriorityEnum priority, Action<NotifyMessage> handler)
        {
            Subscribe(RabbitMQBusMgr.Instance, priority, handler);
        }

        public static long TotalPublisherMessage = 0;
        public static long TotalConsumeTimes = 0;
        public static long TotalConsumeMessage = 0;
        public static object TotalConsumeHandleTimeObj = new object();
        public static long TotalConsumeHandleTime = 0;
        public static Dictionary<string, int> RepeatMessageDic = new Dictionary<string, int>();
        public void Subscribe(RabbitMQBus rabbitMQ, MessagePriorityEnum priority, Action<NotifyMessage> handler)
        {
            try
            {
                string topic = priority.ToString().ToUpper();
                rabbitMQ.Bus.Subscribe(QueueDic[topic], (body, props, info) => Task.Factory.StartNew(() =>
                {
                    MetricsKeys.RabbitMQ_Subscribe.MeterMark("Success");
                    if (DataCache.Add(props.MessageId, 1, DateTime.Now.AddMinutes(10)))
                    {
                        try
                        {
                            if (props.Type == "M")
                            {
#if DEBUG
                                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
#endif
                                List<NotifyMessage> list = JsonConvert.DeserializeObject<List<NotifyMessage>>(Encoding.UTF8.GetString(body), serializerSettings);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    handler(list[i]);
                                }
#if DEBUG
                                watch.Stop();
                                Process.Debug("订阅消息", "Consume", string.Format("数据模式：【{0}】，条数：【{1}】，耗时：【{2}】", props.Type, list.Count, watch.ElapsedMilliseconds), "");
#endif
                            }
                            else if (props.Type == "S")
                            {
                                handler(JsonConvert.DeserializeObject<NotifyMessage>(Encoding.UTF8.GetString(body), serializerSettings));
#if DEBUG
                                Process.Debug("订阅消息", "Consume", string.Format("数据模式：【{0}】，条数：【{1}】", props.Type, 1), "");
#endif
                            }
                        }
                        catch (Exception ex)
                        {
                            m_logger.Error("处理消息发生异常：" + ex.GetString());
                        }
                    }
                    else
                    {
                        RepeatMessageDic[props.MessageId] = RepeatMessageDic[props.MessageId] + 1;
                        ComsumeMessage(props.MessageId, RepeatMessageDic[props.MessageId]);
                    }
                }));
            }
            catch (Exception ex)
            {
                MetricsKeys.RabbitMQ_Subscribe.MeterMark("Error");
                m_logger.Error("订阅消息发生异常：" + ex.GetString());
                throw new RabbitMQException("订阅消息发生异常", ex);
            }
        }

        #endregion

        private static void InitQuene(RabbitMQBus rabbitMQ)
        {
            Array enumArr = Enum.GetValues(typeof(MessagePriorityEnum));
            string queueFormat = "JinRi.Notify.Business.Queue.{0}";
            string exchangeFormat = "JinRi.Notify.Business.Exchange.{0}";
            IQueue queue = null;
            IExchange exchange = null;
            IBinding binding = null;
            foreach (MessagePriorityEnum e in enumArr)
            {
                if (e != MessagePriorityEnum.None)
                {
                    string s = e.ToString().ToUpper();

                    //queue = rabbitMQ.Bus.QueueDeclare(string.Format(queueFormat, s), false, true, false, false, null, null, null, null, null, null, null);
                    //exchange = rabbitMQ.Bus.ExchangeDeclare(string.Format(exchangeFormat, s), ExchangeType.Topic);
                    //binding = rabbitMQ.Bus.Bind(exchange, queue, "");
                    QueueDic.Add(s, queue);
                    ExchangeDic.Add(s, exchange);
                    BindingDic.Add(s, binding);
                }
            }
        }

        private static readonly object WriteFileLockObj = new object();
        public static void ComsumeMessage(NotifyMessage mess)
        {
            string message = string.Format("--------------------------------------------{0}--------------------------------------------", DateTime.Now) + Environment.NewLine;
            message = message + "MessageKey=" + mess.MessageKey + ",MessagePriority =" + mess.MessagePriority.ToString() + ",MessageType =" + mess.MessageType.ToString() + ",MessageId =" + mess.MessageId + Environment.NewLine;
            lock (WriteFileLockObj)
            {
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\a.txt", message);
            }
        }

        public static void ComsumeMessage(string messageId, int count)
        {
            lock (WriteFileLockObj)
            {
                System.IO.File.AppendAllText(@"D:\a.txt", messageId + "-" + count);
            }
        }
    }
}
