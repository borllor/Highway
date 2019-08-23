using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel;
using JinRi.Notify.ServiceModel.Condition;
using Newtonsoft.Json;


namespace JinRi.Notify.Business
{
    public class ReceiveMessageBusiness
    {
        private static readonly RabbitMQBusiness m_rabbitMQBus = new RabbitMQBusiness();
        private Dictionary<MessagePriorityEnum, RabbitMQBus> m_rabbitBusDic = new Dictionary<MessagePriorityEnum, RabbitMQBus>();
        JinRi.Notify.ServiceAgent.BuilderReceiveServiceSOA.DirectReceiveServiceClient m_client = new JinRi.Notify.ServiceAgent.BuilderReceiveServiceSOA.DirectReceiveServiceClient();
        private IDataBufferPool _highPool;
        private IDataBufferPool _middlePool;
        private IDataBufferPool _normalPool;
        private IDataBufferPool _lowPool;

        public ReceiveMessageBusiness()
        {
            if (ReceiveServiceSetting.IsOpenBatchReceiveHighMessage)
            {
                _highPool = new DataBufferPool(BatchHighPublisher, ReceiveServiceSetting.AutoFlushReceiveHighMessage, false);
            }
            if (ReceiveServiceSetting.IsOpenBatchReceiveMiddleMessage)
            {
                _middlePool = new DataBufferPool(BatchMiddlePublisher, ReceiveServiceSetting.AutoFlushReceiveMiddleMessage, false);
            }
            if (ReceiveServiceSetting.IsOpenBatchReceiveNormalMessage)
            {
                _normalPool = new DataBufferPool(BatchNormalPublisher, ReceiveServiceSetting.AutoFlushReceiveNormalMessage, false);
            }
            if (ReceiveServiceSetting.IsOpenBatchReceiveLowMessage)
            {
                _lowPool = new DataBufferPool(BatchLowPublisher, ReceiveServiceSetting.AutoFlushReceiveLowMessage, false);
            }
            Array enumArr = Enum.GetValues(typeof(MessagePriorityEnum));
            foreach (MessagePriorityEnum e in enumArr)
            {
                if (e != MessagePriorityEnum.None)
                {
                    RabbitMQBus bus = new RabbitMQBus();
                    m_rabbitBusDic.Add(e, bus);
                }
            }
        }

        public NotifyMessageResult Receive(NotifyMessage message)
        {
            Check.IsNull(message, "NotifyMessage为空");
            bool isSendToBuffer = false;
            bool isDirectSendMessage = false;
            InitNotifyMessage(message);
            Process.Debug(message.MessageId, "消息接收中心", "Receive", message.MessageKey, string.Format("消息接收内容：{0}", JsonConvert.SerializeObject(message)), "");          
            NotifyMessageResult response;
            string errMsg = "";
            if (!Valid(message, out errMsg))
            {
                response = new NotifyMessageResult();
                response.Success = false;
                response.ErrMsg = errMsg;
                return response;
            }
            if (ReceiveServiceSetting.EnableJudgeHasReceived && HasReceived(message))
            {
                Process.Debug(message.MessageId, "消息接收中心", "Receive", message.MessageKey, string.Format("此类型消息已经接收过了，消息接收内容：{0}", JsonConvert.SerializeObject(message)), "");
                response = new NotifyMessageResult();
                response.Success = true;
                response.ErrMsg = string.Format("此类型消息已经接收过了，MessageId: {0}", message.MessageId);
                return response;
            }
            if (ReceiveServiceSetting.SystemStatus == SystemStatusEnum.Stopping ||
                ReceiveServiceSetting.SystemStatus == SystemStatusEnum.Stopped)
            {
                SendToMQ(message);
            }
            else
            {
                switch (message.MessagePriority)
                {
                    case MessagePriorityEnum.High:
                        isDirectSendMessage = ReceiveServiceSetting.IsDirectRouteHighToBuilderService;
                        if (_highPool != null)
                            isSendToBuffer = true;
                        break;

                    case MessagePriorityEnum.Middle:
                        isDirectSendMessage = ReceiveServiceSetting.IsDirectRouteMiddleToBuilderService;
                        if (_middlePool != null)
                            isSendToBuffer = true;
                        break;

                    case MessagePriorityEnum.Normal:
                        isDirectSendMessage = ReceiveServiceSetting.IsDirectRouteNormalToBuilderService;
                        if (_normalPool != null)
                            isSendToBuffer = true;
                        break;

                    case MessagePriorityEnum.Low:
                        isDirectSendMessage = ReceiveServiceSetting.IsDirectRouteLowToBuilderService;
                        if (_lowPool != null)
                            isSendToBuffer = true;
                        break;
                }
                if (!isDirectSendMessage)
                {
                    if (isSendToBuffer)
                    {
                        switch (message.MessagePriority)
                        {
                            case MessagePriorityEnum.High:
                                _highPool.Write(message);
                                break;

                            case MessagePriorityEnum.Middle:
                                _middlePool.Write(message);
                                break;

                            case MessagePriorityEnum.Normal:
                                _normalPool.Write(message);
                                break;

                            case MessagePriorityEnum.Low:
                                _lowPool.Write(message);
                                break;
                        }
                    }
                    else
                    {
                        SendToMQ(message);
                    }
                }
                else
                {
                    SendToBuilderService(message);
                }
            }
            response = new NotifyMessageResult();
            response.Success = true;
            response.ErrMsg = string.Format("消息接收成功，MessageId：{0}", message.MessageId);
            return response;
        }

        public void SendToBuilderService(NotifyMessage message)
        {
            try
            {
                m_client.Receive(message);
            }
            catch (Exception ex)
            {
                if (m_client.State == CommunicationState.Closed ||
                    m_client.State == CommunicationState.Faulted)
                {
                    m_client = new JinRi.Notify.ServiceAgent.BuilderReceiveServiceSOA.DirectReceiveServiceClient();
                }
                Process.Error(message.MessageKey, "发送消息到消息生成中心", "SendToBuilderService", message.MessageKey, string.Format("异常，ex【{0}】", ex.GetString()), "");
            }
        }

        private void SendToMQ(NotifyMessage message)
        {
            try
            {
                m_rabbitMQBus.Publisher(message);
            }
            catch (Exception ex)
            {
                SendToBuilderService(message);
                throw ex;
            }
        }

        private void BatchHighPublisher(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                Process.Debug("批量发布消息到队列", "BatchHighPublisher", string.Format("条数：【{0}】", buffer.Count), "");
                m_rabbitMQBus.Publisher(m_rabbitBusDic[MessagePriorityEnum.High], MessagePriorityEnum.High, buffer.GetList<NotifyMessage>());
            }
        }

        private void BatchMiddlePublisher(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                Process.Debug("批量发布消息到队列", "BatchMiddlePublisher", string.Format("条数：【{0}】", buffer.Count), "");
                m_rabbitMQBus.Publisher(m_rabbitBusDic[MessagePriorityEnum.Middle], MessagePriorityEnum.Middle, buffer.GetList<NotifyMessage>());
            }
        }

        private void BatchNormalPublisher(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                Process.Debug("批量发布消息到队列", "BatchNormalPublisher", string.Format("条数：【{0}】", buffer.Count), "");
                m_rabbitMQBus.Publisher(m_rabbitBusDic[MessagePriorityEnum.Normal], MessagePriorityEnum.Normal, buffer.GetList<NotifyMessage>());
            }
        }

        private void BatchLowPublisher(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                Process.Debug("批量发布消息到队列", "BatchLowPublisher", string.Format("条数：【{0}】", buffer.Count), "");
                m_rabbitMQBus.Publisher(m_rabbitBusDic[MessagePriorityEnum.Low], MessagePriorityEnum.Low, buffer.GetList<NotifyMessage>());
            }
        }

        private void InitNotifyMessage(NotifyMessage message)
        {
            message.MessageId = IdentityGenerator.New();
            message.ClientIP = ClientProfile.ClientIP;
            message.CreateTime = DateTime.Now;
            RequestProfile.MessageKey = message.MessageId;
        }

        private bool Valid(NotifyMessage message, out string errMsg)
        {
            errMsg = "";
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            if (string.IsNullOrEmpty(message.AppId))
            {
                errMsg = "消息的AppId不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(message.MessageKey))
            {
                errMsg = "消息的MessageKey不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(message.MessageType))
            {
                errMsg = "消息的MessageType不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(message.NotifyData))
            {
                errMsg = "消息的NotifyData不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(message.SourceFrom))
            {
                errMsg = "消息的SourceFrom不能为空";
                return false;
            }
            List<NotifyMessageTypeModel> messageTypeList = business.GetNotifyMessageTypeListFromCache(new NotifyMessageTypeCondition());
            NotifyMessageTypeModel messageTypeMod = messageTypeList.Find(x => x.MessageType.ToString().Equals(message.MessageType, StringComparison.OrdinalIgnoreCase));
            if (messageTypeMod == null)
            {
                errMsg = "消息类型与系统定义不一致";
                Process.Info(message.MessageId, "消息类型初始化验证", "ReceiveMessageBusiness.Valid()", message.MessageKey, string.Format("消息类型与系统定义不一致，消息ID【{0}】，传值类型为【{1}】", message.MessageId, message.MessageType), "");
                return false;
            }
            if (message.MessagePriority == MessagePriorityEnum.None)
            {
                message.MessagePriority = messageTypeMod.MessagePriority;
            }
            return true;
        }

        private bool HasReceived(NotifyMessage message)
        {
            string cacheKey = string.Format(CacheKeys.HasReceivedMessage_Arg2, message.MessageKey, message.MessageType);
            try
            {
                return !DistributedCache.Add(cacheKey, "1", DateTime.Now.AddSeconds(CacheKeys.HasReceivedMessage_TimeOut));
            }
            catch (Exception ex)
            {
                MetricsKeys.Cache_Error.MeterMark();
                string errMsg = "分布式缓存出现异常：" + DistributedCache.ProviderName + ", " + ex.GetString();
                Process.Error(message.MessageId, "分布式缓存", "DistributedCacheProvider.Provider.Add", message.MessageId, errMsg, "");
                return false;
            }
        }

        public void FlushPool()
        {
            Parallel.Invoke(
                () =>
                {
                    if (_highPool != null)
                    {
                        while (!_highPool.IsFlushed())
                        {
                            _highPool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                },
                () =>
                {
                    if (_middlePool != null)
                    {
                        while (!_middlePool.IsFlushed())
                        {
                            _middlePool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                },
                () =>
                {
                    if (_normalPool != null)
                    {
                        while (!_normalPool.IsFlushed())
                        {
                            _normalPool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                },
                () =>
                {
                    if (_lowPool != null)
                    {
                        while (!_lowPool.IsFlushed())
                        {
                            _lowPool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                }
            );   
        }
    }
}
