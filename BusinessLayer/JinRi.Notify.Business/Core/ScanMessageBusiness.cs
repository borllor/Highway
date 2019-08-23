using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DB;
using JinRi.Notify.Entity.JinRiDB;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.ServiceModel;
using JinRi.Notify.ServiceAgent.ReceiverReceiveServiceSOA;
using JinRi.Notify.DTO;
using JinRi.Notify.Utility;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using Newtonsoft.Json;

namespace JinRi.Notify.Business.Core
{
    public class ScanMessageBusiness
    {
        private static readonly Dictionary<string, Thread> m_threadDic;
        private static readonly Dictionary<string, ScanServiceSetting.ScanSetting> m_scanConditionDic;
        private static Thread m_thread = null;
        ReceiveServiceClient m_client = new ReceiveServiceClient();
        private IDataBufferPool _repeatNotifyPool; 

        static ScanMessageBusiness()
        {
            m_threadDic = new Dictionary<string, Thread>();
            m_scanConditionDic = new Dictionary<string, ScanServiceSetting.ScanSetting>();
            InitMessageScanCondition();
        }

        public ScanMessageBusiness()
        {
            _repeatNotifyPool = new DataBufferPool(RepeatNotifyOrder, 5, false);
        }

        public void Scan()
        {
            Process.Debug("m_scanConditionDic", "m_scanConditionDic", string.Format("m_scanConditionDic：【{0}】", m_scanConditionDic.Count), "");
            if (m_threadDic.Count == 0 ||
                m_threadDic.Count != m_scanConditionDic.Keys.Count)
            {
                Process.Debug("订单扫描初始化", "ScanMessageBusiness", "订单扫描初始化((m_threadDic.Count == 0)", "");
                foreach (KeyValuePair<string, ScanServiceSetting.ScanSetting> kv in m_scanConditionDic)
                {
                    Process.Debug("m_scanConditionDic", "m_scanConditionDic", string.Format("m_scanConditionDic：【{0}】", kv.Key), "");
                    CreateThread(kv.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<string, ScanServiceSetting.ScanSetting> kv in m_scanConditionDic)
                {
                    Thread thread = m_threadDic[kv.Key];
                    if (thread.ThreadState == ThreadState.Running ||
                        thread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        //Do Nothing
                    }
                    else if (thread.ThreadState == ThreadState.Unstarted)
                    {
                        thread.Start();
                    }
                    else if (thread.ThreadState == ThreadState.Stopped ||
                        thread.ThreadState == ThreadState.Aborted)
                    {
                        CreateThread(m_scanConditionDic[kv.Key]);
                    }
                }
            }
            //启动补扫线程
            //CreateBuSaoThread();
        }

        private void CreateThread(ScanServiceSetting.ScanSetting setting)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(GetScanMessageList));
            thread.Name = setting.MessType + "_" + setting.OrderStatus.ToString();
            thread.Start(setting);
            m_threadDic[thread.Name] = thread;
        }

        private void GetScanMessageList(object setting)
        {
            string ikey = Guid.NewGuid().ToString();
            ScanServiceSetting.ScanSetting scanSetting = (ScanServiceSetting.ScanSetting)setting;
            Process.Debug("m_scanConditionDic", "m_scanConditionDic", string.Format("scanSetting：【{0}】", scanSetting.ScanCount), "");
            int scanIntervalTime = scanSetting.IntervalTime * (-1);
            ScanOrderCondition con = new ScanOrderCondition();
            con.OrderBy = scanSetting.OrderBy;
            con.ScanOrderIdInit = scanSetting.ScanOrderIdInit;
            con.PageSize = scanSetting.ScanCount;
            con.Status = scanSetting.OrderStatus;
            con.Includes = string.Join(",", scanSetting.Include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            DateTime endTime = GetDateTimeNow(ikey);
            con.StartTime = endTime.AddSeconds(scanIntervalTime);
            con.EndTime = endTime;
            Handle.Debug(ikey, "订单扫描开始", "GetScanMessageList", scanSetting.MessType, string.Format("条件参数【{0}】,订单扫描初开始,间隔：{1}", JsonConvert.SerializeObject(con), scanIntervalTime.ToString()), "");
            try
            {
                System.Diagnostics.Stopwatch execTime = new System.Diagnostics.Stopwatch();
                while (true)
                {
                    execTime.Restart();
                    ikey = Guid.NewGuid().ToString();
                    List<NotifyOrderEntity> orderList = GetOrdersList(con);
                    if (orderList != null && orderList.Count > 0)
                    {
                        foreach (NotifyOrderEntity orderEnt in orderList)
                        {
                            if (!IsSendingDataCache(orderEnt, scanSetting.MessType))
                            {
                                string errMsg = "";
                                NotifyMessage message = new NotifyMessage();
                                message.MessageKey = SetMessageKey(orderEnt, scanSetting.MessType);
                                message.SourceFrom = "OrderScan手动扫描";
                                message.AppId = ConfigurationAppSetting.AppId;
                                string data = string.Format("orderno={0}&OutTime={1}&SalesmanID={2}&ProviderID={3}&ProxyerID={4}",
                                    orderEnt.OrderNo, orderEnt.OutTime, orderEnt.SalesmanID, orderEnt.ProviderID, orderEnt.ProxyerID);
                                message.NotifyData = data;
                                message.MessagePriority = JinRi.Notify.Model.MessagePriorityEnum.None;
                                message.MessageType = scanSetting.MessType;
                                DateTime dt = DateTime.Now;
                                bool ret = SendOrder(message, out errMsg);
                                Double spendTime = (DateTime.Now - dt).TotalMilliseconds;
                                Process.Debug(ikey, "订单扫描记录", "GetScanMessageList", orderEnt.OrderNo, string.Format("订单扫描记录：发送时间【{0}】，调用耗时【{1}】ms，返回结果：【{2}】", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), spendTime, errMsg), "发送");
                                if (!ret)
                                {
                                    lock (m_repeatNotifyTimesLockObj)
                                    {
                                        //进入补偿推送队列
                                        m_repeatNotifyTimes[orderEnt.OrderNo] = 1;
                                    }
                                    _repeatNotifyPool.Write(message);
                                }
                            }
                            Process.Debug(ikey, "订单扫描记录", "GetScanMessageList", orderEnt.OrderNo, "订单扫描记录", "订单");
                        }
                    }
                    execTime.Stop();
                    Process.Debug(ikey, "订单扫描记录", "GetScanMessageList", scanSetting.MessType, string.Format("扫描开始时间：【{0}】，扫描截止时间：【{1}】，扫描条数【{2}】，执行时间【{3}】ms", con.StartTime, con.EndTime, orderList.Count, execTime.ElapsedMilliseconds), "");
                    int sleep = (int)execTime.ElapsedMilliseconds;
                    if (scanSetting.IdleSleepTime > sleep)
                    {
                        Thread.Sleep(scanSetting.IdleSleepTime - sleep);
                    }
                    con.StartTime = endTime.AddSeconds(scanIntervalTime);
                    con.EndTime = GetDateTimeNow(ikey);
                    endTime = con.EndTime;
                    TimeSpan timeSpan = con.EndTime - DateTime.Now;
                    double sleepTime = timeSpan.TotalMilliseconds;
                    if (sleepTime > 0)
                    {
                        Process.Debug(ikey, "订单扫描记录", "GetScanMessageList", scanSetting.MessType, string.Format("休眠时间【{0}】", sleepTime.ToString(), ""));
                        Thread.Sleep((int)sleepTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Process.Fatal(ikey, "订单扫描记录", "GetScanMessageList", scanSetting.MessType, string.Format("重扫线程异常结束ex【{2}】，扫描开始时间：【{0}】，扫描截止时间：【{1}】", con.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), con.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), ex.ToString()), "");
                ThreadPool.QueueUserWorkItem(x =>
                    {
                        try { Scan(); }
                        catch { }
                    }
               );
            }
        }
        private static void InitMessageScanCondition()
        {
            Process.Debug("m_scanConditionDic", "m_scanConditionDic", "InitMessageScanCondition", "");
            List<ScanServiceSetting.ScanSetting> settingList = ScanServiceSetting.ScanSettingList;
            Process.Debug("m_scanConditionDic", "m_scanConditionDic", "List<ScanServiceSetting.ScanSetting> settingList = ScanServiceSetting.ScanSettingList;", "");
            foreach (ScanServiceSetting.ScanSetting setting in settingList)
            {
                m_scanConditionDic[setting.MessType + "_" + setting.OrderStatus.ToString()] = setting;
            }
        }

        private DateTime GetDateTimeNow(string ikey)
        {
            try
            {
                return JinRiDBFacade.Instance.GetDateTimeNow();
            }
            catch (Exception ex)
            {
                Process.Debug(ikey, "订单扫描记录", "GetDateTimeNow", "扫描", "获取服务器时间异常：" + ex.ToString(), "");
                return DateTime.Now;
            }
        }

        private List<NotifyOrderEntity> GetOrdersList(ScanOrderCondition condition)
        {
            return JinRiDBQuery.Instance.GetOrdersList(condition);
        }
        private List<NotifyOrderEntity> GetOrdersListBuSao(ScanOrderCondition condition)
        {
            return JinRiDBQuery.Instance.GetOrdersListBuSao(condition);
        }

        private static object m_repeatNotifyTimesLockObj = new object();
        private static Dictionary<string, int> m_repeatNotifyTimes = new Dictionary<string, int>(1000);
        private void RepeatNotifyOrder(object dataBuffer)
        {
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                List<NotifyMessage> list = buffer.GetList<NotifyMessage>();
                foreach (NotifyMessage message in list)
                {
                    int count = m_repeatNotifyTimes[message.MessageKey];
                    string errMsg = "";
                    bool ret = SendOrder(message, out errMsg);
                    if (!ret)
                    {
                        if (count < 5)
                        {
                            m_repeatNotifyTimes[message.MessageKey] = count + 1;
                            _repeatNotifyPool.Write(message);
                        }
                        else
                        {
                            lock (m_repeatNotifyTimesLockObj)
                            {
                                m_repeatNotifyTimes.Remove(message.MessageKey);
                            }
                        }
                    }
                    else
                    {
                        lock (m_repeatNotifyTimesLockObj)
                        {
                            m_repeatNotifyTimes.Remove(message.MessageKey);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
        }

        private bool SendOrder(NotifyMessage message, out string errMsg)
        {
            try
            {
                NotifyMessageResult result = m_client.Notify(message);
                errMsg = result.ErrMsg;
                return result.Success;
            }
            catch (Exception ex)
            {
                errMsg = "发送订单信息到消息接收中心-发送异常";
                if (m_client.State == CommunicationState.Closed ||
                    m_client.State == CommunicationState.Faulted)
                {
                    m_client = new ReceiveServiceClient();
                }
                Process.Error(message.MessageKey, "发送订单信息到消息接收中心", "SendOrder", message.MessageKey, string.Format("异常，ex【{0}】", ex.GetString()), "");
                return false;
            }
        }

        private bool IsSendingDataCache(NotifyOrderEntity orderEnt, string messageType)
        {
            try
            {
                bool ret = false;
                string orderNo = SetMessageKey(orderEnt, messageType);
                string cacheKey = string.Format(CacheKeys.HasReceivedMessage_Arg2, orderNo, messageType);
                if (!HasReceived(cacheKey))
                {
                    WebCache.Set(cacheKey, "1", DateTime.Now.AddSeconds(CacheKeys.HasReceivedMessage_TimeOut));
                    ret = false;
                }
                else
                {
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                string errMsg = "本地缓存出现异常：" + typeof(DataCache).GetType().Name + ", " + ex.GetString();
                Process.Error("本地缓存", "DataCache.Set", orderEnt.OrderNo, errMsg, "");
                return false;
            }
        }

        private string SetMessageKey(NotifyOrderEntity orderEnt, string messageType)
        {
            string orderNo = orderEnt.OrderNo;
            if (messageType.Equals("OrderApplyReturn", StringComparison.OrdinalIgnoreCase)
                || messageType.Equals("OrderApplyRefund", StringComparison.OrdinalIgnoreCase))
            {
                orderNo = orderNo + "_" + orderEnt.OutTime.Replace("-", "").Replace(" ", "").Replace(":", "");
            }
            return orderNo;
        }

        private bool HasReceived(string cacheKey)
        {
            try
            {
                return DistributedCache.Get(cacheKey) != null;
            }
            catch (Exception ex)
            {
                MetricsKeys.Cache_Error.MeterMark();
                string errMsg = "分布式缓存出现异常：" + DistributedCache.ProviderName + ", " + ex.GetString();
                Process.Error(cacheKey, "分布式缓存", "DistributedCacheProvider.Provider.Add", cacheKey, errMsg, "");
                return false;
            }
        }
    }
}
