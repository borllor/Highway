using JinRi.Notify.Business;
using JinRi.Notify.Business.Common;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Web.Filters;
using JinRi.Notify.Web.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    public class DifferentShowController : Controller
    {
        //
        // GET: /DifferentShow/
        private static readonly DifferentShowBusiness m_diffBus = new DifferentShowBusiness();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendToReceive()
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            ViewBag.NotifyMessageTypeList = business.GetNotifyMessageTypeList(new NotifyMessageTypeCondition());
            return View();
        }


        public ActionResult List(DifferentShowCondition model)
        {
            List<NotifyMessageModel> list = m_diffBus.GetDifferentShowList(model);
            var json = JsonConvert.SerializeObject(new
            {
                rows = list,
                total = 0
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SendToReceiveFromModel(NotifyMessageModel model)
        {
            string errMsg;
            model.MessagePriority = JinRi.Notify.Model.MessagePriorityEnum.None;
            bool isSuccess = m_diffBus.SendToReceive(model, out errMsg);
            return Content(JsonConvert.SerializeObject(new ReturnMessage { Success = isSuccess, Msg = errMsg }));
        }


        [HttpPost]
        public ActionResult SendToReceiveFromJson(string jsonNotify)
        {
            bool isSuccess;
            string errMsg;
            if (!string.IsNullOrWhiteSpace(jsonNotify))
            {
                isSuccess = m_diffBus.SendToReceive(jsonNotify, out errMsg);
            }
            else
            {
                isSuccess = false;
                errMsg = "参数无效";
            }
            return Content(JsonConvert.SerializeObject(new { IsSuccess = isSuccess, ErrMsg = errMsg }));
        }

        public ActionResult GetDetail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetDetailByDay(PushMessageCondition con)
        {
            List<StatisticPushMessageModel> list = m_diffBus.GetDetailByTime(con);
            var json = JsonConvert.SerializeObject(new
            {
                rows = list,
                total = 0
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        public ActionResult GetServerTime()
        {
            DateTime sqldt;
            DateTime mysqldt;
            DateTime serverTime_203;
            m_diffBus.ShowServerTimeDifferent(out sqldt, out mysqldt, out serverTime_203);
            var json = JsonConvert.SerializeObject(new
            {
                SqlDBTime = sqldt,
                MySqlDBTime = mysqldt,
                ServerTime_203 = serverTime_203
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        [SkipCheckLogin]
        [HttpGet]
        public ActionResult DataMove()
        {
            string ikey = Guid.NewGuid().ToString();
            try
            {
                Handle.Info(ikey, "数据迁移", "DifferentShowController", "", "数据迁移", "数据");
                DateTime dt = DateTime.Now;
                int count = m_diffBus.ExecNotifyDataMove();
                Process.Info(ikey, "数据迁移", "DifferentShowController", "", string.Format("Notify数据迁移记录数【{0}】,执行时间【{1}】s", count, (DateTime.Now - dt).TotalSeconds), "数据");
                dt = DateTime.Now;
                count = m_diffBus.ExecPushDataMove();
                Process.Info(ikey, "数据迁移", "DifferentShowController", "", string.Format("Push数据迁移记录数【{0}】,执行时间【{1}】s", count, (DateTime.Now - dt).TotalSeconds), "数据");
            }
            catch (Exception ex)
            {
                Handle.Info(ikey, "数据迁移", "DifferentShowController", "", "数据迁移异常：" + ex.ToString(), "数据");
            }
            return new EmptyResult();
        }

        public ActionResult OrderIndex()
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            ViewBag.NotifyMessageTypeList = business.GetNotifyMessageTypeList(new NotifyMessageTypeCondition());
            return View();
        }

        public ActionResult OrderList(ScanOrderCondition model)
        {      
#if DEBUG
            model.ScanOrderIdInit = 0;
#else
             model.ScanOrderIdInit = 86000000;
#endif
            if (model.Status == 1)
            {
                model.OrderBy = "PayTime";
            }
            else if (model.Status == 2)
            {            
                model.OrderBy = "OutTime";           
            }
            else if (model.Status == 7)
            {
                model.OrderBy = "Contingent7";
            }
            else if (model.Status == 3)
            {
                model.OrderBy = "Rtime";
            }
            else if (model.Status == 4)
            {
                model.OrderBy = "Rtime";
            }
            else if (model.Status == 5)
            {
                model.OrderBy = "OverTime";
            }
            List<NotifyOrderModel> list = m_diffBus.QueryOrdersList(model);
            var json = JsonConvert.SerializeObject(new
            {
                rows = list,
                total = model.RecordCount
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }
    }
}
