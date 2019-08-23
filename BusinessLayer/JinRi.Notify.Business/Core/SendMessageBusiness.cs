using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using EasyNetQ;
using EasyNetQ.Loggers;
using EasyNetQ.Topology;
using JinRi.Notify.DTO;
using JinRi.Notify.Utility.Helper;
using JinRi.Notify.Model;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.ServiceModel;
using Newtonsoft.Json;

namespace JinRi.Notify.Business 
{
    public class SendMessageBusiness
    {
        private PushMessageBusiness _pushMessageBus;
        private NotifyInterfaceSettingBusiness _pushSettingBus;
        private IDataBufferPool _highPool;
        private IDataBufferPool _middlePool; 
        private IDataBufferPool _normalPool;
        private IDataBufferPool _lowPool;
        private IDataBufferPool _repeatUpdatePool;
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(SendMessageBusiness));

        public SendMessageBusiness()
        {
            _pushMessageBus = new PushMessageBusiness();
            _pushSettingBus = new NotifyInterfaceSettingBusiness();
            if (SendServiceSetting.IsOpenBatchSendHighMessage)
            {
                _highPool = new DataBufferPool(new WaitCallback(SendBatch), SendServiceSetting.AutoFlushSendHighMessage, false);
            }
            if (SendServiceSetting.IsOpenBatchSendMiddleMessage)
            {
                _middlePool = new DataBufferPool(new WaitCallback(SendBatch), SendServiceSetting.AutoFlushSendMiddleMessage, false);
            }
            if (SendServiceSetting.IsOpenBatchSendNormalMessage)
            {
                _normalPool = new DataBufferPool(new WaitCallback(SendBatch), SendServiceSetting.AutoFlushSendNormalMessage, false);
            }
            if (SendServiceSetting.IsOpenBatchSendLowMessage)
            {
                _lowPool = new DataBufferPool(new WaitCallback(SendBatch), SendServiceSetting.AutoFlushSendLowMessage, false);
            }
            _repeatUpdatePool = new DataBufferPool(new WaitCallback(RepeatUpdatePushMessage), 10, false);
        }

        public PushMessageResult Send(List<PushMessage> list)
        {
            Check.IsNull(list, "List<PushMessage>为空");
            list.ForEach(x => Send(x));

            PushMessageResult response = new PushMessageResult();
            response.Success = true;
            return response;
        }

        public PushMessageResult Send(PushMessage message)
        {
            Process.Debug(message.PushId, "消息发送中心", "Send", message.MessageId, string.Format("消息内容：{0}", JsonConvert.SerializeObject(message)), "");
            Check.IsNull(message, "PushMessage为空");
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
                Send(message, Callback);
            }

            PushMessageResult response = new PushMessageResult();
            response.Success = true;
            return response;
        }

        public void Send(PushMessage message, Action<object> callback)
        {
            string cacheKey = string.Format(CacheKeys.SendToMessageHandler_Arg2, message.PushId, message.PushCount);
            if (!IsSending(message))
            {
                NotifyInterfaceSettingModel settingMod = _pushSettingBus.Get(message.SettingId);
                if (settingMod != null && settingMod.Status == 2)
                {
                    PushEntity entity = new PushEntity();
                    entity.TimeOut = SendServiceSetting.SendToClientTimeout;
                    entity.PushId = message.PushId;
                    entity.Url = settingMod.Address;
                    entity.Data = message.PushData;
                    entity.Callback = callback;
                    entity.ReferObject = message;
                    entity.Encode = Encoding.GetEncoding(settingMod.Encoding);

                    HttpAsyncHelper httpHelper = new HttpAsyncHelper(entity);
                    MethodType methodType = (MethodType)Enum.Parse(typeof(MethodType), settingMod.Method);
                    httpHelper.OnRequestDataComplected += HttpRequestCompleted;
                    try
                    {
                        httpHelper.Request(methodType);
                        MetricsKeys.SenderService_Send.MeterMark("Success");
                        Process.Debug(message.PushId, "消息推送记录", "Send", message.PushId, string.Format("消息推送中，key【{0}】，地址：{1}", cacheKey, settingMod.Address), "");
                    }
                    catch (Exception ex)
                    {
                        MetricsKeys.SenderService_Send.MeterMark("Error");
                        PushMessageResult result = new PushMessageResult();
                        result.PushId = message.PushId;
                        result.PushStatus = PushResultEnum.Failed;
                        Callback(result);
                        Process.Error(message.PushId, "消息推送记录", "Send", message.PushId, string.Format("消息推送中，key【{0}】，异常：{1}", cacheKey, ex.GetString()), "");
                    }
                }
                else
                {
                    PushMessageResult result = new PushMessageResult();
                    result.ReferObject = message;
                    result.PushId = message.PushId;
                    result.PushStatus = PushResultEnum.Abort;
                    result.ErrMsg = "配置被删除";
                    Callback(result);
                }
            }
            else
            {
                Process.Debug(message.PushId, "消息推送记录", "Send", message.PushId, string.Format("缓存已存在，key【{0}】", cacheKey), "");
            }
        }

        public void Callback(object obj)
        {
            PushMessageResult result = obj as PushMessageResult;
            CallbackInternal(result);
        }

        public PushCallbackResult Callback(PushMessageResult result)
        {
            CallbackInternal(result);
            PushCallbackResult response = new PushCallbackResult();
            response.Success = true;
            return response;
        }

        private void CallbackInternal(PushMessageResult result)
        {
            Check.IsNull(result, "CallbackInternal的参数Obj");
            PushMessageModel model = new PushMessageModel();
            Check.IsNull(result.PushId, "PushMessageResult.PushId");
            PushMessage pushMsg = result.ReferObject as PushMessage;
            Check.IsNull(pushMsg, "PushId查找到的PushMessageModel对象");
            model.PushCount = pushMsg.PushCount;
            model.SettingId = pushMsg.SettingId;
            RequestProfile.RequestKey = result.PushId + "_" + pushMsg.PushCount.ToString();
            RequestProfile.MessageKey = result.PushId;

            switch (result.PushStatus)
            {
                case PushResultEnum.Abort:
                    model.PushId = result.PushId;
                    model.PushStatus = PushStatusEnum.Abort;
                    model.PushCount++;
                    model.Memo = string.Format("Abort：{0},{1:yyyy-MM-dd HH:mm:ss}；{2}", result.ErrMsg, DateTime.Now, Environment.NewLine);
                    break;

                case PushResultEnum.Success:
                    model.PushId = result.PushId;
                    model.PushStatus = PushStatusEnum.Pushed;
                    model.PushCount++;
                    model.Memo = string.Format("Success：{0},{1:yyyy-MM-dd HH:mm:ss}；{2}", result.ErrMsg, DateTime.Now, Environment.NewLine);
                    break;

                case PushResultEnum.Failed:
                    model.PushId = result.PushId;
                    model.PushStatus = PushStatusEnum.Failed;
                    bool isPushNext = false;
                    //model.PushCount == 3
                    //第四次推送的返回
                    model.PushCount++;
                    model.NextPushTime = ComputePushTimeBusiness.ComputeNextPushTime(model, out isPushNext);
                    if (isPushNext)
                    {
                        model.PushStatus = PushStatusEnum.UnPush;
                    }
                    model.Memo = string.Format("Failed：{0},B-{1:yyyy-MM-dd HH:mm:ss},N-{2:yyyy-MM-dd HH:mm:ss}；{3}", result.ErrMsg, DateTime.Now, model.NextPushTime, Environment.NewLine);
                    break;

                case PushResultEnum.Pushing:
                    model.PushId = result.PushId;
                    model.PushStatus = PushStatusEnum.Pushing;
                    model.Memo = string.Format("Pushing：{0},{1:yyyy-MM-dd HH:mm:ss}；{2}", result.ErrMsg, DateTime.Now, Environment.NewLine);
                    break;

                default:
                    break;
            }
            if (!string.IsNullOrWhiteSpace(model.PushId) &&
               (model.PushStatus &
                    (PushStatusEnum.Abort |
                    PushStatusEnum.Pushed |
                    PushStatusEnum.Failed |
                    PushStatusEnum.UnPush |
                    PushStatusEnum.Pushing)) == model.PushStatus)
            {
                model.LastModifyTime = DateTime.Now;
                bool ret = _pushMessageBus.Edit(model);
                if (!ret)
                {
                    lock (m_repeatUpdateTimesLockObj)
                    {
                        //进入补偿更新队列
                        m_repeatUpdateTimes[model.PushId] = 1;
                    }
                    _repeatUpdatePool.Write(model);
                }
                else
                {
                    if (model.PushStatus == PushStatusEnum.UnPush)
                    {
                        string cacheKey = string.Format(CacheKeys.SendToMessageHandler_Arg2, model.PushId, model.PushCount - 1);
                        DistributedCache.Delete(cacheKey);
                    }
                }
                Process.Debug(result.PushId, "消息推送记录", "Callback", result.PushId, string.Format("推送状态：【{0}】，推送结果：【{1}】，更新结果：【{2}】", result.PushStatus.ToString(), result.ErrMsg, ret), "");
            }
        }

        private static object m_repeatUpdateTimesLockObj = new object();
        private static Dictionary<string, int> m_repeatUpdateTimes = new Dictionary<string, int>(1000);
        private void RepeatUpdatePushMessage(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                List<PushMessageModel> list = buffer.GetList<PushMessageModel>();
                foreach (PushMessageModel message in list)
                {
                    int count = m_repeatUpdateTimes[message.PushId];
                    bool ret = _pushMessageBus.Edit(message);
                    if (!ret)
                    {
                        if (count < 5)
                        {
                            m_repeatUpdateTimes[message.PushId] = count + 1;
                            _repeatUpdatePool.Write(message);
                        }
                        else
                        {
                            //如果是待推送状态，怎去掉缓存，以便进行下一次推送
                            if (message.PushStatus == PushStatusEnum.UnPush)
                            {
                                string cacheKey = string.Format(CacheKeys.SendToMessageHandler_Arg2, message.PushId, message.PushCount - 1);
                                DistributedCache.Delete(cacheKey);
                            }
                            lock (m_repeatUpdateTimesLockObj)
                            {
                                m_repeatUpdateTimes.Remove(message.PushId);
                            }
                        }
                    }
                    else
                    {
                        lock (m_repeatUpdateTimesLockObj)
                        {
                            m_repeatUpdateTimes.Remove(message.PushId);
                        }
                    }
                    Process.Debug(message.PushId, "消息补偿更新推送记录", "RepeatUpdatePushMessage", message.PushId, string.Format("更新次数：【{0}】，更新结果：【{1}】", count + 1, ret), "");
                    Thread.Sleep(100);
                }
            }
        }

        private void SendBatch(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                Process.Debug("批量发送推送消息", "SendBatch", string.Format("数量【{0}】", buffer.Count), "");
                List<PushMessage> list = buffer.GetList<PushMessage>();
                foreach (PushMessage message in list)
                {
                    Send(message, Callback);
                }
            }
        }

        private void HttpRequestCompleted(object target, DataResponseEventArgs e)
        {
            PushEntity entity = e.PushEntity;
            HttpStatusCode statusCode = e.HttpStatusCode;
            string data = e.RecievedData; //Success,Failed,Pushing,Abort
            if (e.Exception == null)
            {
                Process.Info(entity.PushId, "消息推送记录", "HttpRequestCompleted", entity.PushId, string.Format("异步请求完成，返回结果：【{0}】", data), "");
                if (statusCode == HttpStatusCode.OK && entity.Callback != null)
                {
                    PushMessageResult result = new PushMessageResult();
                    result.ReferObject = entity.ReferObject;
                    result.PushId = entity.PushId;
                    switch (data)
                    {
                        case "Success":
                            result.PushStatus = PushResultEnum.Success;
                            break;

                        case "Failed":
                            result.PushStatus = PushResultEnum.Failed;
                            break;

                        case "Pushing":
                            result.PushStatus = PushResultEnum.Pushing;
                            break;

                        case "Abort":
                            result.PushStatus = PushResultEnum.Abort;
                            break;

                        default:
                            Process.Warning(entity.PushId, "消息推送记录", "HttpRequestCompleted", entity.PushId, string.Format("此消息返回异常结果：{0}", data), "返回异常结果");
                            result.PushStatus = PushResultEnum.Abort;
                            break;
                    }

                    result.ErrMsg = data;
                    entity.Callback(result);
                }
            }
            else
            {
                Process.Error(entity.PushId, "消息推送记录", "HttpRequestCompleted", entity.PushId, string.Format("异步请求完成，返回结果：【{0}】，异常：{1}", data, e.Exception.GetString()), "");
                PushMessageResult result = new PushMessageResult();
                result.ReferObject = entity.ReferObject;
                result.PushId = entity.PushId;
                result.PushStatus = PushResultEnum.Failed;
                Callback(result);
            }
        }

        private bool IsSending(PushMessage message)
        {
            string cacheKey = string.Format(CacheKeys.SendToMessageHandler_Arg2, message.PushId, message.PushCount);
            try
            {
                return !DistributedCache.Add(cacheKey, "1", DateTime.Now.AddSeconds(CacheKeys.SendToMessageHandler_TimeOut));
            }
            catch (Exception ex)
            {
                MetricsKeys.Cache_Error.MeterMark();
                string errMsg = "分布式缓存出现异常：" + DistributedCache.ProviderName + ", " + ex.GetString();
                Process.Error(message.PushId, "分布式缓存", "DistributedCacheProvider.Provider.Add", message.PushId, errMsg, "");
                return false;
            }
        }

        public void FlushPool()
        {
            Parallel.Invoke(
                () =>
                {
                    if (_repeatUpdatePool != null)
                    {
                        while (!_repeatUpdatePool.IsFlushed())
                        {
                            _repeatUpdatePool.Flush();
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
                }
            );
        }
    }
}
