using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Threading;
using System.Web.Security;
using System.Web.SessionState;

using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Utility;
using JinRi.Notify.Business;
using JinRi.Notify.Business.Common;
using JinRi.Notify.DTO;
using JinRi.Notify.ServiceModel;

namespace JinRi.Notify.BuilderService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }
        protected void Application_PreSendRequestContent(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            JinRi.Notify.Frame.FirstRequestInitialization.Initialize(HttpContext.Current, InitApplication);
        }

        private void InitApplication(HttpContext context)
        {
            BuilderServiceSetting.SystemStatus = SystemStatusEnum.Started;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            CallContext.FreeNamedDataSlot(ClientProfile.ContextKey);
            CallContext.FreeNamedDataSlot(RequestProfile.ContextKey);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            BuilderServiceSetting.SystemStatus = SystemStatusEnum.Stopping;
            BuildFacade facade = new BuildFacade();
            //关闭订阅通道
            facade.CloseRabbitMQBus();
            Thread.Sleep(2);
            DateTime now = DateTime.Now;
            Process.Info("消息生成中心", "BeginFlushPool", string.Format("开始时间：{0}", now), "FlushPool");
            facade.FlushPool();
            Thread.Sleep(2);
            Process.Info("消息生成中心", "BeginFlushPool", string.Format("结束时间：{0}，耗时：{1}", DateTime.Now, (DateTime.Now - now).TotalMilliseconds), "FlushPool");
            BuilderServiceSetting.SystemStatus = SystemStatusEnum.Stopped;
        }
    }
}