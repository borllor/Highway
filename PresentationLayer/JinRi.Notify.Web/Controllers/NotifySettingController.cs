using JinRi.Notify.Business;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Web.Helper;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    public class NotifySettingController : BaseController
    {
        //
        // GET: /NotifyConfig/

        public ActionResult Index()
        {
            NotifySettingBusiness bus = new NotifySettingBusiness();
            ViewBag.Data = bus.GetNotifySettingList(new NotifySettingCondition());
            return View();
        }

        public ActionResult GetNotifySetting(NotifySettingCondition condition)
        {
            NotifySettingBusiness bus = new NotifySettingBusiness();
            var notifySetting = bus.GetNotifySettingList(condition).FirstOrDefault();
            if (notifySetting == null)
            {
                return new EmptyResult();
            }
            var result = JsonConvert.DeserializeObject(notifySetting.SettingValue);
            return Content(JsonConvert.SerializeObject(result));
        }

        #region 编辑

        public ActionResult Edit(NotifySettingModel model)
        {
            ReturnMessage message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                NotifySettingBusiness bus = new NotifySettingBusiness();
                var notifySetting = bus.GetNotifySetting(model.SettingKey);
                if (notifySetting == null)
                {
                    message.Msg = "配置不存在!";
                    return Content(JsonConvert.SerializeObject(message));
                }
                notifySetting.SettingValue = model.SettingValue;
                notifySetting.Memo = string.Format("编辑，操作人：{0}，操作时间: {1}，操作人IP：{2}|", UserName, DateTime.Now, ClientIP);
                notifySetting.LastModifyTime = DateTime.Now;
                message.Success = bus.Edit(notifySetting);
            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        } 
        #endregion

        #region 新增

        public ActionResult Add(NotifySettingModel model)
        {
            ReturnMessage message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                NotifySettingBusiness bus = new NotifySettingBusiness();
                var notifySetting = bus.GetNotifySetting(model.SettingKey);
                if (notifySetting != null)
                {
                    message.Msg = "配置已存在!";
                    return Content(JsonConvert.SerializeObject(message));
                }
                model.ClassName = string.Empty;
                model.Remark = "服务配置";
                model.Memo = string.Format("新增，操作人：{0}，操作时间: {1}，操作人IP：{2}|", UserName, DateTime.Now, ClientIP);
                message.Success = bus.Save(model);
            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        } 
        #endregion

        #region 删除

        public ActionResult Delete(NotifySettingModel model)
        {
            ReturnMessage message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                NotifySettingBusiness bus = new NotifySettingBusiness();
                var notifySetting = bus.GetNotifySetting(model.SettingKey);
                if (notifySetting == null)
                {
                    message.Msg = "配置不存在!";
                    return Content(JsonConvert.SerializeObject(message));
                }
                message.Success = bus.Delete(model);
            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        } 
        #endregion

    }
}
