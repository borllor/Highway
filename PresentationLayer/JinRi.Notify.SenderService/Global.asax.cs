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
using JinRi.Notify.Frame;

namespace JinRi.Notify.SenderService
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
            FirstRequestInitialization.Initialize(HttpContext.Current, InitApplication);
        }

        private void InitApplication(HttpContext context)
        {
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
            DateTime now = DateTime.Now;
            Process.Info("消息发送中心", "BeginFlushPool", string.Format("开始时间：{0}", now), "FlushPool");
            new SendFacade().FlushPool();
            Thread.Sleep(3);
            Process.Info("消息发送中心", "BeginFlushPool", string.Format("结束时间：{0}，耗时：{1}", DateTime.Now, (DateTime.Now - now).TotalMilliseconds), "FlushPool");

        }
    }
}