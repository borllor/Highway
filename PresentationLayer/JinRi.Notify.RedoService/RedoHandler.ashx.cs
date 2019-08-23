using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using JinRi.Notify.Business;
using JinRi.Notify.ServiceModel.Profile;

namespace JinRi.Notify.RedoService
{
    public class RedoHandler : IHttpHandler
    {
        private static readonly RedoFacade m_redoFacade = new RedoFacade();

        public void ProcessRequest(HttpContext context)
        {
            RequestProfile.RequestType = "JinRi.Notify.SenderService.RedoHandler";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.MessageKey = "";
            RequestProfile.Username = "";
            string logtype = "重扫服务启动";
            string content = string.Format("{0}", "重扫服务启动");

            Handle.Info(logtype, "RedoHandler.ProcessRequest()", content, "");
            m_redoFacade.Redo();
            context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}