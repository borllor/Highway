using JinRi.Notify.Frame;
using JinRi.Notify.DB;
using JinRi.Notify.DTO;
using JinRi.Notify.Entity;
using JinRi.Notify.Entity.JinRiDB;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceAgent.ReceiverReceiveServiceSOA;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JinRi.Notify.Business.Common
{
    public class DifferentShowBusiness
    {
        public List<NotifyMessageModel> GetDifferentShowList(DifferentShowCondition con)
        {
            List<NotifyMessageEntity> list = JinRiNotifyFacade.Instance.GetDifferentShowList(con);
            List<NotifyMessageModel> modelList = new List<NotifyMessageModel>();
            list.ForEach((x) =>
            {
                modelList.Add(MappingHelper.From<NotifyMessageModel, NotifyMessageEntity>(x));
            });
            return modelList;
        }

        public bool SendToReceive(NotifyMessageModel model, out string errMsg)
        {
            bool isSuccess;
            errMsg = "";
            NotifyMessage notifyMsg;
            try
            {
                Check.IsNull(model, string.Format("消息补推MessageId【{0}】对象", model.MessageId ?? ""));
                notifyMsg = MappingHelper.From<NotifyMessage, NotifyMessageModel>(model);
                Check.IsNull(notifyMsg, string.Format("消息补推MessageId【{0}】对象", model.MessageId ?? ""));
                ReceiveServiceClient rsClient = new ReceiveServiceClient();
                rsClient.Notify(notifyMsg);
                Process.Info("消息补推", "DifferentShowBusiness.SendToReceive", (model.MessageId ?? ""), "补推成功", "");
                isSuccess = true;
                if (!string.IsNullOrWhiteSpace(model.MessageId))
                {
                    NotifyMessageBusiness notifyBus = new NotifyMessageBusiness();
                    notifyBus.Delete(model.MessageId);
                }
            }
            catch (Exception ex)
            {
                Process.Error("消息补推", "DifferentShowBusiness.SendToReceive", (model.MessageId ?? ""), "补推失败，ex：" + ex.ToString(), "");
                errMsg = ex.GetString();
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool SendToReceive(string jsonNotify, out string errMsg)
        {
            bool isSuccess;
            errMsg = "";
            try
            {
                Check.IsNullOrEmpty(jsonNotify, "补推json字符串");
                NotifyMessageModel model = JsonConvert.DeserializeObject<NotifyMessageModel>(jsonNotify);
                Process.Info("消息补推", "DifferentShowBusiness.SendToReceive", "", "json格式字符串：" + jsonNotify, "");
                if (model == null)
                {
                    Process.Info("消息补推", "DifferentShowBusiness.SendToReceive", "", "序列化失败", "");
                    isSuccess = false;
                    errMsg = "序列化失败";
                    return isSuccess;
                }
                isSuccess = SendToReceive(model, out errMsg);
            }
            catch (Exception ex)
            {
                Process.Error("消息补推", "DifferentShowBusiness.SendToReceive", "", jsonNotify + "，补推失败，ex：" + ex.ToString(), "");
                errMsg = ex.GetString();
                isSuccess = false;
            }
            return isSuccess;
        }

        public List<StatisticPushMessageModel> GetDetailByTime(PushMessageCondition con)
        {
            List<StatisticPushMessageEntity> entityList = JinRiNotifyFacade.Instance.GetDetailByTime(con);
            List<StatisticPushMessageModel> list = new List<StatisticPushMessageModel>();
            if (entityList.Any())
            {
                entityList.ForEach(x => list.Add(MappingHelper.From<StatisticPushMessageModel, StatisticPushMessageEntity>(x)));
            }
            return list;
        }

        public void ShowServerTimeDifferent(out DateTime sqlDBdt, out DateTime mySqlDBdt, out DateTime serverTime_203)
        {          
            Task<DateTime> t = new Task<DateTime>(getMySqlDBTime);
            Task<DateTime> t2 = new Task<DateTime>(getSqlDBTime);
            Task<DateTime> t3 = new Task<DateTime>(getServerTime_203);    
            t.Start();
            t2.Start();
            t3.Start();
            Task.WaitAll(t, t2, t3);
            sqlDBdt = t.Result;
            mySqlDBdt = t2.Result;
            serverTime_203 = t3.Result;
        }
        private DateTime getSqlDBTime()
        {
            // TODO
            return DateTime.Now;
        }
        private DateTime getMySqlDBTime()
        {
            return JinRiNotifyFacade.Instance.GetDatabaseTime();
        }
        private DateTime getServerTime_203()
        {
            return DateTime.Now;
        }

        #region 数据迁移
        public int ExecNotifyDataMove()
        {
            WebConfigBusiness webConfig = new WebConfigBusiness();
            int day = webConfig.GetCacheValue("NotifyDataMoveDateSpan", 30);
            int count = 0;
            int row = JinRiNotifyFacade.Instance.ExecNotifyMessageDataMove(day);
            int callCount = 0;
            while (row > 0)
            {
                count += row / 3;
                row = JinRiNotifyFacade.Instance.ExecNotifyMessageDataMove(day);
                callCount++;
                if (callCount > 1000)
                {
                    Process.Error("", "数据迁移", "DifferentShowBusiness", "", string.Format("循环调用次数【{0}】", callCount), "异常");
                    break;
                }
            }
            Process.Info("", "数据迁移", "DifferentShowBusiness", "", string.Format("循环调用次数【{0}】", callCount), "统计"); 
            return count;
        }

        public int ExecPushDataMove()
        {
            WebConfigBusiness webConfig = new WebConfigBusiness();
            int day = webConfig.GetCacheValue("NotifyDataMoveDateSpan", 30);
            int count = 0;
            int row = JinRiNotifyFacade.Instance.ExecPushMessageDataMove(day);
            int callCount = 0;
            while (row > 0)
            {
                count += row / 3;
                row = JinRiNotifyFacade.Instance.ExecPushMessageDataMove(day);
                callCount++;
                if (callCount > 1000)
                {
                    Process.Error("", "数据迁移", "DifferentShowBusiness", "", string.Format("循环调用次数【{0}】", callCount), "异常");
                    break;
                }
            }
            Process.Info("", "数据迁移", "DifferentShowBusiness", "", string.Format("循环调用次数【{0}】", callCount), "统计");             
            return count;
        }

        #endregion

        #region 查询订单列表
        public List<NotifyOrderModel> QueryOrdersList(ScanOrderCondition condition)
        {
            List<NotifyOrderEntity> list = null;
            List<NotifyOrderModel> modList = new List<NotifyOrderModel>();
            list.ForEach(x => modList.Add(MappingHelper.From<NotifyOrderModel, NotifyOrderEntity>(x)));
            return modList;
        }
        #endregion
    }
}
