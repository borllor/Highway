using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

using JinRi.Notify.DTO;
using JinRi.Notify.Business;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;


namespace JinRi.Notify.SenderService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SendService : ISendService
    {
        private static readonly SendFacade m_sendFacade = new SendFacade();

        public PushMessageResult ReceiveList(PushMessage[] pushMessArray)
        {
            RequestProfile.RequestType = "JinRi.Notify.SenderService.Receive";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            //RequestProfile.MessageKey = message.PushId;
            RequestProfile.Username = "";
             List<PushMessage> list = pushMessArray == null ? new List<PushMessage>() : pushMessArray.ToList();
            return DelegateHelper.Invoke<List<PushMessage>, PushMessageResult>(m_sendFacade.ReceiveList, list, MetricsKeys.SenderService_Receive);
        }

        public PushMessageResult Receive(PushMessage message)
        {
            RequestProfile.RequestType = "JinRi.Notify.SenderService.Receive";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.MessageKey = message.PushId;
            RequestProfile.Username = "";

            return DelegateHelper.Invoke<PushMessage, PushMessageResult>(m_sendFacade.Receive, message, MetricsKeys.SenderService_Receive);
        }

        public PushCallbackResult Callback(PushMessageResult result)
        {
            RequestProfile.RequestType = "JinRi.Notify.SenderService.Callback";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.MessageKey = result.PushId;
            RequestProfile.Username = "";

            return DelegateHelper.Invoke<PushMessageResult, PushCallbackResult>(m_sendFacade.Callback, result, MetricsKeys.SenderService_Callback);
        }
    }
}
