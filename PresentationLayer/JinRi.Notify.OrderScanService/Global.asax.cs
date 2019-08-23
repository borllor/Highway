using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using JinRi.Notify.Business;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Frame;
using System.Runtime.Remoting.Messaging;

namespace JinRi.Notify.OrderScanService
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog m_log = LoggerSource.Instance.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            FirstRequestInitialization.Initialize(HttpContext.Current, InitApplication);
            CallContext.FreeNamedDataSlot(ClientProfile.ContextKey);
            CallContext.FreeNamedDataSlot(RequestProfile.ContextKey);
        }

        private void InitApplication(HttpContext context)
        {
            //SystemHeatService.Register();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                m_log.Error(ex.GetString());
                Handle.Error(RequestProfile.RequestType, "JinRi.Notify.OrderScanService.OrderScanHandler", ex.GetString(), "");
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            m_log.Info(string.Format("订单扫描记录-订单扫描应用程序退出-ApplicationEnd"));
            Handle.Info("订单扫描记录", "JinRi.Notify.OrderScanService.OrderScanHandler", "订单扫描应用程序退出", "ApplicationEnd");
            DBLog.FlushLogMessage();
            System.Threading.Thread.Sleep(1000);
        }
    }
}