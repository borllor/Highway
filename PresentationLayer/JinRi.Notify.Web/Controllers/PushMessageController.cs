using JinRi.Notify.Business;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel.Condition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using JinRi.Notify.Web.Helper;

namespace JinRi.Notify.Web.Controllers
{
    public class PushMessageController : Controller
    {
        //
        // GET: /PushMessage/

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
        public ActionResult List(PushMessageCondition condition)
        {
            PushMessageBusiness business = new PushMessageBusiness();
            condition.QuerySource = 1;
            var rows = business.GetPageList(condition);
            var json = JsonConvert.SerializeObject(new
            {
                rows = rows,
                total = condition.RecordCount
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        public ActionResult HistoryList(PushMessageCondition condition)
        {
            PushMessageBusiness business = new PushMessageBusiness();
            condition.QuerySource = 2;
            var rows = business.GetPageList(condition);
            var json = JsonConvert.SerializeObject(new
            {
                rows = rows,
                total = condition.RecordCount
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        /// <summary>
        /// 保存推送信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(PushMessageModel model, string pushIds)
        {
            var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                PushMessageBusiness business = new PushMessageBusiness();
                if (string.IsNullOrEmpty(pushIds))       //单条编辑
                {
                    var pushMessage = business.GetFromCache(model.PushId);
                    if (pushMessage == null)
                    {
                        message.Msg = "推送消息不存在!";
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    pushMessage.MessagePriority = model.MessagePriority;
                    pushMessage.MessageType = model.MessageType;
                    pushMessage.PushData = model.PushData;
                    pushMessage.PushStatus = model.PushStatus;
                    pushMessage.NextPushTime = model.NextPushTime;
                    pushMessage.PushCount = model.PushCount;
                    pushMessage.Memo = string.Empty;
                    message.Success = business.Edit(pushMessage);
                }
                else
                {
                    //批量编辑
                    PushMessageCondition condition = new PushMessageCondition();
                    condition.PushIds = new List<string>(pushIds.Split(','));
                    var data = business.GetPushMessageList(condition);
                    data.ForEach(t =>
                    {
                        t.NextPushTime = model.NextPushTime;
                        t.PushCount = model.PushCount;
                        t.LastModifyTime = DateTime.Now;
                        t.PushStatus = PushStatusEnum.UnPush;
                    });
                    if (data.Count > 0)
                    {
                        message.Success = business.EditPushMessage(data);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        }

        //public ActionResult Truncate()
        //{
        //    var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
        //    try
        //    {
        //        PushMessageBusiness business = new PushMessageBusiness();
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
