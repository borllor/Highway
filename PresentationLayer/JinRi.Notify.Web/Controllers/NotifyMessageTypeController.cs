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
    public class NotifyMessageTypeController : Controller
    {
        //
        // GET: /NotifyMessageType/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取通知类型
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult List(NotifyMessageTypeCondition condition)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            var rows = business.GetNotifyMessageTypeList(condition);
            condition.RecordCount = business.GetNotifyMessageTypeCount(condition);
            var json = JsonConvert.SerializeObject(new
            {
                rows = rows,
                total = condition.RecordCount
            }, JsonConvertHelper.IsoDateTimeConverter);
            return Content(json);
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(NotifyMessageTypeModel model, int OptType)
        {
            var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                NotifyMessageTypeCondition condition = new NotifyMessageTypeCondition();
                NotifyMessageBusiness business = new NotifyMessageBusiness();
                if (OptType == 2)       //新增
                {
                    var messageTypeList = business.GetNotifyMessageTypeList(condition);
                    if (messageTypeList.Where(t => t.MessageType == model.MessageType).Count() > 0)
                    {
                        message.Msg = "消息类型重复!";
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    model.CreateTime = DateTime.Now;
                    model.Status = 2;
                    message.Success = business.SaveNotifyMessageType(model);
                }
                else                    //修改
                {
                    condition.MessageTypeId = model.MessageTypeId;
                    var messageType = business.GetNotifyMessageTypeList(condition).FirstOrDefault();
                    if (messageType == null)
                    {
                        message.Msg = "消息类型不存在!";
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    model.Status = messageType.Status;
                    if (OptType == 3)    //删除
                    {
                        model = messageType;
                        model.Status = 3;
                    }
                    message.Success = business.EditNotifyMessageType(model);
                }

            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        }

    }
}
