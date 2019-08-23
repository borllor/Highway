using JinRi.Notify.Business;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Web.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    public class NotifyMessageController : Controller
    {
        //
        // GET: /NotifyMessage/

        public ActionResult Index()
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            ViewBag.NotifyMessageTypeList = business.GetNotifyMessageTypeList(new NotifyMessageTypeCondition());
            return View();
        }

        /// <summary>
        /// 获取通知信息
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult List(NotifyMessageCondition condition)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            condition.QuerySource = 1;
            var rows = business.GetNotifyMessageList(condition);
            var json = JsonConvert.SerializeObject(new
            {
                rows = rows,
                total = condition.RecordCount
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        public ActionResult HistoryList(NotifyMessageCondition condition)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            condition.QuerySource = 2;
            var rows = business.GetNotifyMessageList(condition);
            var json = JsonConvert.SerializeObject(new
            {
                rows = rows,
                total = condition.RecordCount
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        //public ActionResult Truncate()
        //{
        //    var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
        //    try
        //    {
        //        NotifyMessageBusiness business = new NotifyMessageBusiness();
        //        business.Truncate();
        //        message.Success = true;
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return Content(JsonConvert.SerializeObject(message));
        //}

    }
}
