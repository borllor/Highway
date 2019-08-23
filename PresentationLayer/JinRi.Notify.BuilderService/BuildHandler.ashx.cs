using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using JinRi.Notify.Business;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.ServiceModel;

namespace JinRi.Notify.BuilderService
{
    /// <summary>
    /// BuildHandler 的摘要说明
    /// </summary>
    public class BuildHandler : IHttpHandler
    {
        private static readonly BuildFacade _buildFacade = new BuildFacade();

        public void ProcessRequest(HttpContext context)
        {
            RequestProfile.RequestType = "JinRi.Notify.BuilderService.BuildHandler";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.MessageKey = "";
            RequestProfile.Username = "";
            string logtype = "生成服务启动";
            string content = string.Format("{0}", "生成服务启动");

            Handle.Info(logtype, "BuildHandler.ProcessRequest()", content, "");

            _buildFacade.Build();

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