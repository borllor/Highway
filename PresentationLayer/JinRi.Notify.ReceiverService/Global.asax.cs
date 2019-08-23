using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using JinRi.Notify.Business;
using JinRi.Notify.Frame;
using JinRi.Notify.ServiceModel;
using JinRi.Notify.ServiceModel.Profile;

namespace LoudJinRi.NotifyeiverService
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
            ReceiveServiceSetting.SystemStatus = SystemStatusEnum.Started;
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
            ReceiveServiceSetting.SystemStatus = SystemStatusEnum.Stopping;
            DateTime now = DateTime.Now;
            Process.Info("消息接收中心", "BeginFlushPool", string.Format("开始时间：{0}", now), "FlushPool");
            new ReceiveFacade().FlushPool();
            Thread.Sleep(3);
            Process.Info("消息接收中心", "BeginFlushPool", string.Format("结束时间：{0}，耗时：{1}", DateTime.Now, (DateTime.Now - now).TotalMilliseconds), "FlushPool");
            ReceiveServiceSetting.SystemStatus = SystemStatusEnum.Stopped;
        }
    }
}