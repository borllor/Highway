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

namespace JinRi.Notify.RedoService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ThreadPool.SetMaxThreads(500, 500);
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
        }
    }
}