using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using System.Threading;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.DB;
using JinRi.Notify.Entity;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceAgent;
using JinRi.Notify.ServiceModel;
using Newtonsoft.Json;

namespace JinRi.Notify.Business
{
    public class BuildMessageBusiness
    {
        private NotifyMessageBusiness _notifyMessageBus;
        private PushMessageBusiness _pushMessageBus;
        private NotifyInterfaceSettingBusiness _notifySettingBus;
        private IDataBufferPool _saveNotifyPool;
        private IDataBufferPool _savePushPool;
        private IDataBufferPool _highPool;
        private IDataBufferPool _middlePool;
        private IDataBufferPool _normalPool;
        private IDataBufferPool _lowPool;
        private IDataBufferPool _repeatSavePool;

        public BuildMessageBusiness()
        {
            _notifyMessageBus = new NotifyMessageBusiness();
            _pushMessageBus = new PushMessageBusiness(BuilderServiceSetting.AutoFlushSendMessage);
            _notifySettingBus = new NotifyInterfaceSettingBusiness();
            if (BuilderServiceSetting.IsOpenBatchSaveNotifyMessage)
            {
                _saveNotifyPool = new DataBufferPool(new WaitCallback(BatchSaveNotifyMessage), BuilderServiceSetting.AutoFlushNotifyMessage, false);
            }
            if (BuilderServiceSetting.IsOpenBatchSavePushMessage)
            {
                _savePushPool = new DataBufferPool(new WaitCallback(BatchSavePushMessage), BuilderServiceSetting.AutoFlushPushMessage, false);
            }
            if (BuilderServiceSetting.IsOpenBatchReceiveHighMessage)
            {
                _highPool = new DataBufferPool(new WaitCallback(BuildBufferMessage), BuilderServiceSetting.AutoFlushReceiveHighMessage, false);
            }
            if (BuilderServiceSetting.IsOpenBatchReceiveMiddleMessage)
            {
                _middlePool = new DataBufferPool(new WaitCallback(BuildBufferMessage), BuilderServiceSetting.AutoFlushReceiveMiddleMessage, false);
            }
            if (BuilderServiceSetting.IsOpenBatchReceiveNormalMessage)
            {
                _normalPool = new DataBufferPool(new WaitCallback(BuildBufferMessage), BuilderServiceSetting.AutoFlushReceiveNormalMessage, false);
            }
            if (BuilderServiceSetting.IsOpenBatchReceiveLowMessage)
            {
                _lowPool = new DataBufferPool(new WaitCallback(BuildBufferMessage), BuilderServiceSetting.AutoFlushReceiveLowMessage, false);
            }
            _repeatSavePool = new DataBufferPool(new WaitCallback(RepeatSaveMessage), 5, false);
        }

        public void Build(NotifyMessage message)
        {
            SaveNotifyMessage(message);
            //Process.Debug(message.MessageId, "消息生成中心", "BuildMessage", message.MessageId, string.Format("消息内容：{0}", JsonConvert.SerializeObject(message)), "");
            List<PushMessageModel> list = BuildMessage(message);
            Process.Debug(message.MessageId, "消息生成中心", "Build", message.MessageId, string.Format("通知消息编号：{0}，生成推送消息：【{1}】条", message.MessageId, (list != null ? list.Count : 0)), (list != null ? list.Count : 0) + "");
            if (list != null && list.Count > 0)
            {
                SavePushMessage(list);
                MetricsKeys.BuilderService_Build.MeterMark("Success");
                int aheadTime = BuilderServiceSetting.PushAheadTime;
                foreach (PushMessageModel m in list)
                {
                    if (m.NextPushTime <= DateTime.Now.AddSeconds(aheadTime))
                    {
                        PushMessage pushMsg = MappingHelper.From<PushMessage, PushMessageModel>(m);
                        bool isSended = false;
                        _pushMessageBus.SendPushMessage(pushMsg, BuilderServiceSetting.IsOpenBatchSendPushMessage, out isSended);
                    }
                }
            }
            else
            {
                Process.Debug(message.MessageId, "消息生成中心", "Build", message.MessageId, "没有生成推送消息", "");
            }
        }

        private List<PushMessageModel> BuildMessage(NotifyMessage message)
        {
            List<PushMessageModel> pmlist = new List<PushMessageModel>();
            try
            {
                List<NotifyInterfaceSettingModel> nslist = _notifySettingBus.GetListFromCache();
                if (nslist == null || nslist.Count == 0 || message == null) return null;
                bool isPushNext = false;
                foreach (NotifyInterfaceSettingModel setting in nslist.FindAll(x => x.Status == 2 && x.MessageType.ToString().Equals(message.MessageType, StringComparison.OrdinalIgnoreCase)))
                {
                    PushMessageModel pushMod = new PushMessageModel();
                    pushMod.PushId = IdentityGenerator.New();
                    pushMod.MessageId = message.MessageId;
                    pushMod.SettingId = setting.SettingId;
                    pushMod.MessageKey = message.MessageKey;
                    pushMod.MessagePriority = message.MessagePriority;
                    pushMod.MessageType = message.MessageType;
                    pushMod.PushData = string.Format("msgid={0}&msgkey={1}&msgtype={2}&data={3}", pushMod.PushId, message.MessageKey, message.MessageType, HttpUtility.UrlEncode(message.NotifyData));
                    pushMod.PushStatus = PushStatusEnum.UnPush;
                    pushMod.MessageCreateTime = message.CreateTime;
                    pushMod.CreateTime = DateTime.Now;
                    pushMod.LastModifyTime = pushMod.CreateTime;
                    pushMod.PushCount = 0;
                    pushMod.Memo = string.Format("Add：{0:yyyy-MM-dd HH:mm:ss}；{1}", message.CreateTime, Environment.NewLine);
                    pushMod.NextPushTime = ComputePushTimeBusiness.ComputeNextPushTime(pushMod, out isPushNext);

                    pmlist.Add(pushMod);
                }
            }
            catch (Exception ex)
            {
                MetricsKeys.BuilderService_Build.MeterMark("Error");
                Process.Error(message.MessageId, "消息生成中心", "Exception", message.MessageId, "生成推送消息，发生异常：" + ex.GetString(), "");
            }
            finally
            {
                Process.Debug(message.MessageId, "消息生成中心", "finally", message.MessageId, "生成推送消息，条数：" + pmlist.Count, "");
            }
            return pmlist;
        }

        public void FlushPool()
        {
            Parallel.Invoke(
                ()=>
                {
                    _pushMessageBus.FlushPool();
                },
                () =>
                {
                    if (_saveNotifyPool != null)
                    {
                        while (!_saveNotifyPool.IsFlushed())
                        {
                            _saveNotifyPool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                },
                () =>
                {
                    if (_savePushPool != null)
                    {
                        while (!_savePushPool.IsFlushed())
                        {
                            _savePushPool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                },
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
                },
                () =>
                {
                    if (_repeatSavePool != null)
                    {
                        while (!_repeatSavePool.IsFlushed())
                        {
                            _repeatSavePool.Flush();
                            Thread.Sleep(100);
                        }
                    }
                }
            );           
        }

        #region 接收来自 消息接收中心的直连消息

        public NotifyMessageResult Receive(NotifyMessage message)
        {
            Check.IsNull(message, "NotifyMessage为空");
            bool isSendMessage = true;
            switch (message.MessagePriority)
            {
                case MessagePriorityEnum.High:
                    if (_highPool != null)
                        _highPool.Write(message);
                    else
                        isSendMessage = false;
                    break;

                case MessagePriorityEnum.Middle:
                    if (_middlePool != null)
                        _middlePool.Write(message);
                    else
                        isSendMessage = false;
                    break;

                case MessagePriorityEnum.Normal:
                    if (_normalPool != null)
                        _normalPool.Write(message);
                    else
                        isSendMessage = false;
                    break;

                case MessagePriorityEnum.Low:
                    if (_lowPool != null)
                        _lowPool.Write(message);
                    else
                        isSendMessage = false;
                    break;
            }
            if (!isSendMessage)
            {
                Build(message);
            }

            NotifyMessageResult response = new NotifyMessageResult();
            response.Success = true;
            return response;
        }

        public NotifyMessageResult ReceiveList(List<NotifyMessage> list)
        {
            Check.IsNull(list, "List<NotifyMessage> 为空");
            list.ForEach(x => Receive(x));
            NotifyMessageResult response = new NotifyMessageResult();
            response.Success = true;
            return response;
        }

        private void BuildBufferMessage(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                List<NotifyMessage> list = buffer.GetList<NotifyMessage>();
                foreach (NotifyMessage message in list)
                {
                    Build(message);
                }
            }
        }

        #endregion

        #region 保存NotifyMessage

        private void SaveNotifyMessage(NotifyMessage message)
        {
            if (_saveNotifyPool != null)
            {
                _saveNotifyPool.Write(message);
            }
            else
            {
                DirectSaveNotifyMessage(message);
            }
        }

        private void DirectSaveNotifyMessage(NotifyMessage message)
        {
            DirectSaveNotifyMessage(new List<NotifyMessage>() { message });
        }

        private void DirectSaveNotifyMessage(List<NotifyMessage> messageList)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                bool ret = false;
                object[] arr = (object[])x;
                List<NotifyMessage> list = (List<NotifyMessage>)arr[0];
                IDataBufferPool pool = (IDataBufferPool)arr[1];
                object lockObj = arr[2];
                Dictionary<string, int> repeatSaveTimes = (Dictionary<string, int>)arr[3];
                try
                {
                    ret = _notifyMessageBus.Save(list);
                    Process.Debug("保存消息通知", "_notifyMessageBus.Save", string.Format("结果：【{0}】，条数：【{1}】", ret, list.Count), "");
                }
                catch (Exception ex)
                {
                    Process.Error("保存消息通知", "_notifyMessageBus.Save", string.Format("结果：【{0}】，条数：【{1}】，异常：【{2}】", ret, list.Count, ex.GetString()), "");
                }
                if (!ret)
                {
                    list.ForEach(a =>
                    {
                        pool.Write(a);
                        lock (lockObj)
                        {
                            repeatSaveTimes[a.MessageId] = 1;
                        }
                    });
                }
            }, new object[] { messageList, _repeatSavePool, m_repeatSaveTimesLockObj, m_repeatSaveTimes });
        }

        private void BatchSaveNotifyMessage(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                DirectSaveNotifyMessage(buffer.GetList<NotifyMessage>());
            }
        }

        private static object m_repeatSaveTimesLockObj = new object();
        private static Dictionary<string, int> m_repeatSaveTimes = new Dictionary<string, int>(1000);
        private void RepeatSaveMessage(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                List<object> list = buffer.GetList<object>();
                foreach (object obj in list)
                {
                    bool ret = false;
                    string key = "";
                    if (obj is NotifyMessage)
                    {
                        key = ((NotifyMessage)obj).MessageId;
                        ret = _notifyMessageBus.Save((NotifyMessage)obj);
                    }
                    else if (obj is PushMessageModel)
                    {
                        key = ((PushMessageModel)obj).PushId;
                        ret = _pushMessageBus.Save((PushMessageModel)obj);
                    }

                    int count = m_repeatSaveTimes[key];
                    if (!ret)
                    {
                        if (count <= 5)
                        {
                            m_repeatSaveTimes[key] = count + 1;
                            _repeatSavePool.Write(obj);
                        }
                        else
                        {
                            lock (m_repeatSaveTimesLockObj)
                            {
                                m_repeatSaveTimes.Remove(key);
                            }
                        }
                    }
                    else
                    {
                        lock (m_repeatSaveTimesLockObj)
                        {
                            m_repeatSaveTimes.Remove(key);
                        }
                    }
                    Process.Debug(key, "消息补偿保存记录", "RepeatSaveMessage", key, string.Format("保存次数：【{0}】，更新结果：【{1}】", count + 1, ret), "");
                    Thread.Sleep(100);
                }
            }
        }

        #endregion

        #region 保存PushMessage

        private void SavePushMessage(List<PushMessageModel> messageList)
        {
            if (_savePushPool != null)
            {
                messageList.ForEach(x => _savePushPool.Write(x));
            }
            else
            {
                DirectSavePushMessage(messageList);
            }
        }

        private void DirectSavePushMessage(List<PushMessageModel> messageList)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                bool ret = false;
                object[] arr = (object[])x;
                List<PushMessageModel> list = (List<PushMessageModel>)arr[0];
                IDataBufferPool pool = (IDataBufferPool)arr[1];
                object lockObj = arr[2];
                Dictionary<string, int> repeatSaveTimes = (Dictionary<string, int>)arr[3];
                try
                {
                    ret = _pushMessageBus.Save(list);
                    Process.Debug("保存消息通知", "_pushMessageBus.Save", string.Format("结果：【{0}】，条数：【{1}】", ret, list.Count), "");
                }
                catch (Exception ex)
                {
                    Process.Error("保存消息通知", "_pushMessageBus.Save", string.Format("结果：【{0}】，条数：【{1}】，异常：【{2}】", ret, list.Count, ex.GetString()), "");
                }
                if (!ret)
                {
                    list.ForEach(a => { 
                        _repeatSavePool.Write(a);
                        lock (m_repeatSaveTimesLockObj)
                        {
                            m_repeatSaveTimes[a.MessageId] = 1;
                        }
                    });
                }
            }, new object[] { messageList, _repeatSavePool, m_repeatSaveTimesLockObj, m_repeatSaveTimes });
        }

        public static long BatchSavePushMessageCount = 0;
        private void BatchSavePushMessage(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                DirectSavePushMessage(buffer.GetList<PushMessageModel>());
            }
        }

        #endregion
    }
}
