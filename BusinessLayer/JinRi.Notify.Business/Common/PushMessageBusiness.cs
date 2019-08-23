using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

using JinRi.Notify.DTO;
using JinRi.Notify.DB;
using JinRi.Notify.Entity;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Model;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using System.ServiceModel;

namespace JinRi.Notify.Business
{
    public class PushMessageBusiness
    {
        private List<Thread> _threadList = new List<Thread>();
        private IDataBufferPool _sendMessagePool;
        private static int callCount = 0;
        JinRi.Notify.ServiceAgent.SenderSendServiceSOA.SendServiceClient m_client = new JinRi.Notify.ServiceAgent.SenderSendServiceSOA.SendServiceClient();

        public PushMessageBusiness()
            : this(2)
        {
        }

        public PushMessageBusiness(int sendMessagePool)
        {
            _sendMessagePool = new DataBufferPool(new WaitCallback(BatchSendMessage), sendMessagePool, false);
        }

        public bool Save(PushMessage message)
        {
            return Save(new List<PushMessage>(new PushMessage[] { message }));
        }
        private bool Save(List<PushMessage> messageList)
        {
            List<PushMessageEntity> entityList = new List<PushMessageEntity>();
            messageList.ForEach((x) =>
            {
                entityList.Add(MappingHelper.From<PushMessageEntity, PushMessage>(x));
            });
            return JinRiNotifyFacade.Instance.SavePushMessage(entityList) > 0;
        }

        public bool Save(PushMessageModel message)
        {
            return Save(new List<PushMessageModel>(new PushMessageModel[] { message }));
        }

        public bool Save(List<PushMessageModel> messageList)
        {
            List<PushMessageEntity> entityList = new List<PushMessageEntity>();
            messageList.ForEach((x) =>
            {
                entityList.Add(MappingHelper.From<PushMessageEntity, PushMessageModel>(x));
            });
            return JinRiNotifyFacade.Instance.SavePushMessage(entityList) > 0;
        }

        public bool Edit(PushMessageModel message)
        {
            PushMessageEntity entity = MappingHelper.From<PushMessageEntity, PushMessageModel>(message);
            if (message.MessagePriority == MessagePriorityEnum.None)
            {
                entity.MessagePriority = Null.NullString;
            }
            if (Null.IsNull(message.MessageType))
            {
                entity.MessageType = Null.NullString;
            }
            return JinRiNotifyFacade.Instance.EditPushMessage(entity) > 0;
        }

        public List<PushMessage> GetList(PushMessageCondition con)
        {
            List<PushMessageModel> pushMessageModList = GetPushMessageList(con);
            List<PushMessage> list = new List<PushMessage>();
            foreach (PushMessageModel m in pushMessageModList)
            {
                list.Add(MappingHelper.From<PushMessage, PushMessageModel>(m));
            }

            return list;
        }

        /// <summary>
        /// 获取推送消息列表（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<PushMessageModel> GetPageList(PushMessageCondition con)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            var messageTypeList = business.GetNotifyMessageTypeList(new NotifyMessageTypeCondition());
            List<PushMessageEntity> entityList = JinRiNotifyFacade.Instance.GetPushMessagePageList(con);
            List<PushMessageModel> list = new List<PushMessageModel>();
            entityList.ForEach(x =>
            {
                var model = MappingHelper.From<PushMessageModel, PushMessageEntity>(x);
                var messageType = messageTypeList.Where(t => t.MessageType == model.MessageType).FirstOrDefault();
                if (messageType != null)
                {
                    model.MessageTypeCName = messageType.Remark;
                }
                model.MessagePriorityName = x.MessagePriority.ToString();
                model.PushStatusName = Enum.GetName(typeof(PushStatusEnum), x.PushStatus);
                model.MessageTypeEName = x.MessageType.ToString();
                list.Add(model);
            });
            return list;
        }

        /// <summary>
        /// 批量更新推送消息
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public bool EditPushMessage(List<PushMessageModel> messageList)
        {
            List<PushMessageEntity> entityList = new List<PushMessageEntity>();
            messageList.ForEach(t =>
            {
                var entity = MappingHelper.From<PushMessageEntity, PushMessageModel>(t);
                entityList.Add(entity);
            });
            return JinRiNotifyFacade.Instance.EditPushMessage(entityList) > 0;
        }

        public PushMessageModel Get(string pushId)
        {
            PushMessageEntity entity = JinRiNotifyFacade.Instance.GetPushMessageByID(pushId);
            PushMessageModel model = MappingHelper.From<PushMessageModel, PushMessageEntity>(entity);
            return model;
        }

        public PushMessageModel GetFromCache(string pushId)
        {
            string cacheKey = string.Format(CacheKeys.GetPushMessage_Arg1, pushId);
            PushMessageModel model = DataCache.Get(cacheKey) as PushMessageModel;
            if (model == null)
            {
                model = Get(pushId);
                if (model != null)
                {
                    DataCache.Set(cacheKey, model, DateTime.Now.AddSeconds(CacheKeys.GetPushMessage_TimeOut));
                }
            }
            return model;
        }

        private static CountdownEvent m_sendMessageCDE = new CountdownEvent(0);

        public void SendPushMessage(PushMessage message, bool isWriteToBuffer, out bool isSended)
        {
            isSended = IsSending(message);
            if (!isSended)
            {
                if (isWriteToBuffer)
                {
                    _sendMessagePool.Write(message);
                }
                else
                {
                    while (m_sendMessageCDE.CurrentCount > 1000)
                    {
                        Process.Info(message.PushId, "消息通知单条发送接口", "SendPushMessage", "", "m_sendMessageCDE：" + m_sendMessageCDE.CurrentCount.ToString(), "阻塞");
                        Thread.Sleep(100);
                    }

                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        try
                        {
                            Process.Info(message.PushId, "消息通知单条发送接口", "SendPushMessage", "", string.Format("批次【{0}】，数量【{1}】", ++callCount, 1), "");
                            m_sendMessageCDE.TryAddCount(1);
                            m_client.Receive(message);
                            if (m_sendMessageCDE.CurrentCount > 0)
                            {
                                m_sendMessageCDE.TryAddCount(-1);
                            }
                            Process.Info(message.PushId, "消息通知单条发送接口_返回", "SendPushMessage", "", string.Format("批次【{0}】，数量【{1}】", ++callCount, 1), "");
                        }
                        catch (Exception ex)
                        {
                            if (m_sendMessageCDE.CurrentCount > 0)
                            {
                                m_sendMessageCDE.TryAddCount(-1);
                            }
                            if (m_client.State == CommunicationState.Closed ||
                                m_client.State == CommunicationState.Faulted)
                            {
                                m_client = new ServiceAgent.SenderSendServiceSOA.SendServiceClient();
                            }
                            //如果推送异常，去掉标示的推送缓存
                            string cacheKey = string.Format(CacheKeys.PushMessageToSender_Arg2, message.PushId, message.PushCount);
                            DistributedCache.Delete(cacheKey);
                            Process.Error(message.PushId, "消息通知单条发送接口", "SendPushMessage", "", "异步发送推送消息到消息通知接口出现异常：" + ex.GetString(), "Error");
                        }
                    });
                }
            }
        }

        private void BatchSendMessage(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                List<PushMessage> list = buffer.GetList<PushMessage>();
                try
                {
                    Process.Info("消息通知批量发送接口", "BatchSendMessage", "", string.Format("批次【{0}】，数量【{1}】", ++callCount, list.Count), "");
                    JinRi.Notify.ServiceAgent.SenderSendServiceSOA.SendServiceClient client = new JinRi.Notify.ServiceAgent.SenderSendServiceSOA.SendServiceClient();
                    PushMessageResult result = client.ReceiveList(list.ToArray());
                    Process.Info("消息通知批量发送接口_返回", "BatchSendMessage", "", result.Success + "", "");
                }
                catch (Exception ex)
                {
                    //如果推送异常，去掉标示的推送缓存
                    foreach (PushMessage message in list)
                    {
                        string cacheKey = string.Format(CacheKeys.PushMessageToSender_Arg2, message.PushId, message.PushCount);
                        DistributedCache.Delete(cacheKey);
                    }
                    Process.Error("消息通知批量发送接口", "BatchSendMessage", "", "异步发送推送消息到消息通知接口出现异常：" + ex.GetString(), "Error");
                }
            }
        }

        public List<PushMessageModel> GetPushMessageList(PushMessageCondition con)
        {
            List<PushMessageEntity> pushMessageEntList = JinRiNotifyFacade.Instance.GetPushMessageList(con);
            List<PushMessageModel> pushMessageModList = new List<PushMessageModel>();
            pushMessageEntList.ForEach(x =>
            {
                pushMessageModList.Add(MappingHelper.From<PushMessageModel, PushMessageEntity>(x));
            });
            return pushMessageModList;
        }

        private bool IsSending(PushMessage message)
        {
            string cacheKey = string.Format(CacheKeys.PushMessageToSender_Arg2, message.PushId, message.PushCount);
            try
            {
                return !DistributedCache.Add(cacheKey, "1", DateTime.Now.AddSeconds(CacheKeys.PushMessageToSender_TimeOut));
            }
            catch (Exception ex)
            {
                MetricsKeys.Cache_Error.MeterMark();
                string errMsg = "分布式缓存出现异常：" + DistributedCache.ProviderName + ", " + ex.GetString();
                Process.Error("分布式缓存", "m_cacheProvider.Add", message.PushId, errMsg, "");
                return true;
            }
        }

        public void FlushPool()
        {
            if (_sendMessagePool != null)
            {
                while (!_sendMessagePool.IsFlushed())
                {
                    _sendMessagePool.Flush();
                    Thread.Sleep(100);
                }
            }
        }


        public void Truncate()
        {
            JinRiNotifyFacade.Instance.TruncatePushMessage();
        }
    }
}
