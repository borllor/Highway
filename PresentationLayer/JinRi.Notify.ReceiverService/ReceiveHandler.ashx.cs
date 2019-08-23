using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.Business;
using JinRi.Notify.DTO;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Utility;
using Newtonsoft.Json;


namespace JinRi.Notify.ReceiverService
{
    public class ReceiveHandler : IHttpHandler
    {

        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(ReceiveService));
        private static readonly ReceiveFacade m_receiveFacade = new ReceiveFacade();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = Notify(context);
            context.Response.Write(result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string Notify(HttpContext context)
        {
            RequestProfile.RequestType = "JinRi.Notify.ReceiverService.ReceiveHandler";
            string data = context.Request["data"];
            if (string.IsNullOrEmpty(data))
            {
                byte[] bs = new byte[context.Request.InputStream.Length];
                context.Request.InputStream.Read(bs, 0, bs.Length);
                data = Encoding.UTF8.GetString(bs);
            }
            NotifyMessage notifyMsg;
            string result = "";
            try
            {
                Handle.Info(string.Format("请求信息，data【{0}】", data), "请求开始");
                Check.CanDeserializeObject(data, "请求参数data", out notifyMsg);
                notifyMsg.NotifyData = HttpUtility.UrlDecode(notifyMsg.NotifyData);//编码消除 '=','&'特殊字符 
                RequestProfile.RequestKey = notifyMsg.MessageKey;
                NotifyMessageResult response = DelegateHelper.Invoke<NotifyMessage, NotifyMessageResult>(m_receiveFacade.Receive, notifyMsg, MetricsKeys.ReceiveHandler);
                result = response.Success ? "Success" : "Failed";
                MetricsKeys.ReceiveHandler.MeterMark("Success");
            }
            catch (Exception ex)
            {
                MetricsKeys.ReceiveHandler.MeterMark("Error");
                Handle.Fatal(string.Format("序列化NotifyMessage类异常，ex：{0}", ex.GetString()));
                result = "Failed";
            }
            return result;
        }
    }
}