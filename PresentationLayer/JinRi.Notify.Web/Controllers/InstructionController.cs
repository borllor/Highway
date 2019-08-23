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
    public class InstructionController : Controller
    {
        //
        // GET: /Instruction/

        public ActionResult Index()
        {
            InstructionServiceBusiness bus = new InstructionServiceBusiness();
            var servers = bus.GetAllServers();
            ViewBag.Servers = servers != null ? string.Join(",", servers.ToArray()) : string.Empty;
            return View();
        }

        public ActionResult List(InstructionCondition con)
        {
            InstructionServiceBusiness bus = new InstructionServiceBusiness();
            List<TaskMessageModel> messageList = bus.GetTaskMessageList(con);
            string json = JsonConvert.SerializeObject(
                new { rows = messageList, total = con.RecordCount },
                JsonConvertHelper.IsoDateTimeConverter
                );
            return Content(json);
        }

        public ActionResult CreateTask(TaskMessageModel taskMessage)
        {
            ReturnMessage message = new ReturnMessage() { Success = false, Msg = "操作失败!" };
            try
            {
                InstructionServiceBusiness bus = new InstructionServiceBusiness();
                message.Success = bus.CreateTask(taskMessage);
            }
            catch (Exception ex)
            {
                message.Msg = ex.ToString();
            }
            return Content(JsonConvert.SerializeObject(message));
        }

    }
}
