using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.DB;
using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.Utility;
using Newtonsoft.Json;
using JinRi.Notify.ServiceModel;
using JinRi.Notify.ServiceModel.Condition;

namespace JinRi.Notify.Business
{
    public class RedoMessageBusiness
    {
        private static readonly Dictionary<string, Thread> m_threadDic;
        private static readonly PushMessageBusiness m_pushMessageBus;
        private static readonly Dictionary<string, RedoServiceSetting.ScanSetting> m_scanConditionDic;
        private static object threadDicLock = new object();

        static RedoMessageBusiness()
        {
            m_threadDic = new Dictionary<string, Thread>();
            m_pushMessageBus = new PushMessageBusiness(RedoServiceSetting.AutoFlushSendMessage);
            m_scanConditionDic = new Dictionary<string, RedoServiceSetting.ScanSetting>();
            InitMessagePriorityScanCondition();
        }

        public void Scan()
        {
            if (m_threadDic.Count == 0 ||
                m_threadDic.Count != m_scanConditionDic.Keys.Count)
            {
                foreach (KeyValuePair<string, RedoServiceSetting.ScanSetting> kv in m_scanConditionDic)
                {
                    CreateThread(kv.Key, kv.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<string, RedoServiceSetting.ScanSetting> kv in m_scanConditionDic)
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
                        CreateThread(kv.Key, m_scanConditionDic[kv.Key]);
                    }
                }
            }
        }

        private void CreateThread(string name, RedoServiceSetting.ScanSetting setting)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(GetPushMessageList));
            thread.Name = name;
            thread.Start(setting);
            m_threadDic[thread.Name] = thread;
        }

        private void GetPushMessageList(object setting)
        {
            RedoServiceSetting.ScanSetting scanSetting = (RedoServiceSetting.ScanSetting)setting;
            int scanStartTimes = scanSetting.PrevScanTimes * scanSetting.InternalTime * (-1);
            int scanEndTimes = scanSetting.NextScanTimes * scanSetting.InternalTime;
            int aheadTime = RedoServiceSetting.PushAheadTime;
            PushMessageCondition con = new PushMessageCondition();
            con.PushStatus = scanSetting.PushStatus;
            con.PageSize = scanSetting.LimitCount;
            con.MessagePriority = Null.NullString;
            con.OrderDirection = OrderDirectionEnum.ASC;
            con.OrderBy = "NextPushTime";
            if (!string.IsNullOrWhiteSpace(scanSetting.MessagePriority))
            {
                con.MessagePriority = scanSetting.MessagePriority;
            }
            if (scanSetting.MessageType != null && scanSetting.MessageType.Count > 0)
            {
                con.MessageType = scanSetting.MessageType;
            }
            else
            {
                con.MessageType = null;
            }
            con.ENextPushTime = DateTime.Now;
            List<PushMessageModel> pushMessageList;
            try
            {
                int i = 0;
                while (true)
                {
                    MetricsKeys.RedoService.MeterMark();
                    DateTime dtNow = DateTime.Now;
                    con.SNextPushTime = con.ENextPushTime.AddSeconds(scanStartTimes);
                    con.ENextPushTime = con.ENextPushTime.AddSeconds(scanEndTimes);

                    pushMessageList = m_pushMessageBus.GetPushMessageList(con);
                    StringBuilder builder = new StringBuilder();
                    int canSendNum = 0;
                    int sendedNum = 0;
                    if (pushMessageList != null && pushMessageList.Count > 0)
                    {
                        int count = pushMessageList.Count;
                        foreach (PushMessageModel msgMod in pushMessageList)
                        {
                            if (msgMod.NextPushTime <= DateTime.Now.AddSeconds(aheadTime))
                            {
                                canSendNum++;
                                PushMessage pushMsg = MappingHelper.From<PushMessage, PushMessageModel>(msgMod);
                                bool isSended = false;
                                m_pushMessageBus.SendPushMessage(pushMsg, RedoServiceSetting.IsOpenBatchSendPushMessage, out isSended);
                                if (isSended)
                                {
                                    sendedNum++;
                                }
                                builder.AppendFormat("消息编号：{0}，消息类型：{1}，是否推送：{2}；", msgMod.PushId, msgMod.MessageType, isSended);
                            }

                            Thread.Sleep(100);
                        }
                        //while (count > num)
                        //{
                        //    PushMessageModel msgMod = pushMessageList[idx];
                        //    if (!"1".Equals(msgMod.Memo) && msgMod.NextPushTime <= DateTime.Now.AddSeconds(aheadTime))
                        //    {
                        //        num++;
                        //        PushMessage pushMsg = MappingHelper.From<PushMessage, PushMessageModel>(msgMod);
                        //        bool isSended = false;
                        //        m_pushMessageBus.SendPushMessage(pushMsg, RedoServiceSetting.IsOpenBatchSendPushMessage, out isSended);
                        //        msgMod.Memo = "1";
                        //        builder.AppendFormat("消息编号：{0}，消息类型：{1}，是否推送：{2}；", msgMod.PushId, msgMod.MessageType, isSended);
                        //    }
                        //    idx++;
                        //    if (idx == count) idx = 0;
                        //}
                    }
                    else
                    {
                        Thread.Sleep(scanSetting.IdleSleepTime);
                    }
                    Process.Debug("消息重扫记录", "GetPushMessageList", string.Format("扫描次数【{0}】，优先级：【{1}】，推送开始时间：【{2}】，推送截止时间：【{3}】，扫描条数【{4}】，执行时间【{5}】，查询条件：【{6}】，可推送条数：【{7}】，实际推送条数：【{8}】，消息：【{9}】", ++i, con.MessagePriority.ToString(), con.SNextPushTime.ToString("yyyy-MM-dd HH:mm:ss"), con.ENextPushTime.ToString("yyyy-MM-dd HH:mm:ss"), pushMessageList.Count, (DateTime.Now - dtNow).TotalMilliseconds, JsonConvert.SerializeObject(con), canSendNum, sendedNum, builder.ToString()), pushMessageList.Count.ToString());

                    TimeSpan timeSpan = con.ENextPushTime - DateTime.Now;
                    double sleepTime = timeSpan.TotalMilliseconds;
                    if (sleepTime > 0)
                    {
                        Thread.Sleep((int)sleepTime);
                        Process.Debug("消息重扫记录", "Thread.Sleep", string.Format("扫描次数【{0}】，线程休眠【{1}】，优先级：【{2}】，推送开始时间：【{3}】，推送截止时间：【{4}】", ++i, sleepTime, con.MessagePriority.ToString(), con.SNextPushTime.ToString("yyyy-MM-dd HH:mm:ss"), con.ENextPushTime.ToString("yyyy-MM-dd HH:mm:ss")), "");
                    }
                }
            }
            catch (Exception ex)
            {
                Process.Fatal("消息重扫记录", "GetPushMessageList", string.Format("重扫线程异常结束ex【{3}】，优先级：【{0}】，推送开始时间：【{1}】，推送截止时间：【{2}】", con.MessagePriority.ToString(), con.SNextPushTime.ToString("yyyy-MM-dd HH:mm:ss"), con.ENextPushTime.ToString("yyyy-MM-dd HH:mm:ss"), ex.GetString()), "");
            }
        }

        private static void InitMessagePriorityScanCondition()
        {
            foreach (RedoServiceSetting.ScanSetting setting in RedoServiceSetting.ScanSettingList)
            {
                MessagePriorityEnum priorityEnum;
                if (Enum.TryParse<MessagePriorityEnum>(setting.MessagePriority, out priorityEnum))
                {
                    m_scanConditionDic[priorityEnum.ToString().ToUpper()] = setting;
                }
            }
        }
    }
}
