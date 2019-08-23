using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using JinRi.Notify.Utility;
using JinRi.Notify.Business;
using JinRi.Notify.ServiceModel.Profile;

namespace JinRi.Notify.OrderScanService
{
    /// <summary>
    /// OrderScanHandler 的摘要说明
    /// </summary>
    public class OrderScanHandler : IHttpHandler
    {
        private ScanFacade _scanFacade = new ScanFacade();

        public void ProcessRequest(HttpContext context)
        {
            RequestProfile.RequestType = "内部扫描服务启动";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            Handle.Info(RequestProfile.RequestType, "JinRi.Notify.OrderScanService.OrderScanHandler", "扫描服务启动", "");
            try
            {
                _scanFacade.Scan();

                context.Response.ContentType = "text/plain";
                context.Response.Write("OK");
            }
            catch (Exception ex)
            {
                Handle.Error(RequestProfile.RequestType, "JinRi.Notify.OrderScanService.OrderScanHandler", ex.GetString(), "");

                context.Response.ContentType = "text/plain";
                context.Response.Write(ex.GetString());
            }
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