using JinRi.Notify.Business;
using JinRi.Notify.ServiceModel.Condition;
using System;
using System.Linq;
using System.Web.Mvc;

using Newtonsoft.Json;
using JinRi.Notify.Model;
using JinRi.Notify.Web.Helper;

namespace JinRi.Notify.Web.Controllers
{
    public class NotifyInterfaceSettingController : Controller
    {
        //
        // GET: /NotifySetting/

        public ActionResult Index(int? settingId)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            ViewBag.NotifyMessageTypeList = business.GetNotifyMessageTypeList(new NotifyMessageTypeCondition());
            if(settingId.HasValue)
            {
                var setting = new NotifyInterfaceSettingBusiness().Get(settingId.Value);
                ViewBag.AppId = setting.AppId;
                ViewBag.SettingId = setting.SettingId;
            }
            
            return View();
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult List(NotifyInterfaceSettingCondition condition)
        {
            NotifyInterfaceSettingBusiness business = new NotifyInterfaceSettingBusiness();
            var rows = business.GetList(condition);
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
        public ActionResult Save(NotifyInterfaceSettingModel model, int OptType)
        {
            var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                NotifyInterfaceSettingBusiness business = new NotifyInterfaceSettingBusiness();
                if (OptType == 2)       //新增
                {
                    var settingList = business.GetList();
                    if (settingList.Where(t => t.AppId == model.AppId && t.MessageType == model.MessageType
                        && t.Address.Equals(model.Address)).Count() > 0)
                    {
                        message.Msg = "配置重复!";
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    model.CreateTime = DateTime.Now;
                    model.LastModifyTime = model.CreateTime;
                    model.Status = 2;
                    message.Success = business.SaveNotifySetting(model);
                }
                else
                {                  //修改
                    var setting = business.GetNoCache(model.SettingId);
                    if (setting == null)
                    {
                        message.Msg = "配置不存在!";
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    model.Status = setting.Status;
                    if(OptType == 3)    //删除
                    {
                        model = setting;
                        model.Status = 3;
                    }
                    model.LastModifyTime = DateTime.Now;
                    message.Success = business.UpdateNotifySetting(model);
                }
                
            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        }

    }
}
