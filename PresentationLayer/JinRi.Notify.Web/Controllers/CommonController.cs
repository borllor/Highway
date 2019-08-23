using JinRi.Notify.Business;
using JinRi.Notify.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Web.Controllers
{
    public class CommonController : BaseController
    {
        //
        // GET: /Common/

        public ActionResult ExecSql()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult ExecSql(string content)
        //{
        //    var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
        //    try
        //    {
        //        NotifySettingBusiness bus = new NotifySettingBusiness();
        //        message.Success = bus.ExecSqlCommand(content);
        //    }
        //    catch (Exception ex)
        //    {
        //        message.Msg = ex.ToString();
        //    }
        //    return Content(JsonConvert.SerializeObject(message));
        //}

        [HttpPost]
        public ActionResult GetDataByExecSql(string content)
        {
            var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                NotifySettingBusiness bus = new NotifySettingBusiness();
                Regex regex = new Regex(@"\s+notifymessage_backup\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (regex.IsMatch(content))
                {
                    List<NotifyMessageModel> list = bus.GetNotifyMessageData(content);
                    message.Msg = JsonConvert.SerializeObject(new { total = list.Count, rows = list });
                }
                else
                {
                    List<PushMessageModel> list = bus.GetPushMessageData(content);
                    message.Msg = JsonConvert.SerializeObject(new { total = list.Count, rows = list });                 
                }
                message.Success = true;                 
            }
            catch (Exception ex)
            {
                message.Msg = ex.ToString();               
            }
            return Content(JsonConvert.SerializeObject(message));
        }

    }
}
