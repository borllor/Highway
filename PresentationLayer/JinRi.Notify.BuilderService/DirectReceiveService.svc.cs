using System;
using System.ServiceModel.Activation;
using System.Linq;

using JinRi.Notify.DTO;
using JinRi.Notify.Business;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;
using System.Collections.Generic;

namespace JinRi.Notify.BuilderService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DirectReceiveService : IDirectReceiveService
    {
        private static readonly BuildFacade m_buildFacade = new BuildFacade();

        public NotifyMessageResult Receive(NotifyMessage message)
        {
            RequestProfile.RequestType = "JinRi.Notify.BuilderService.Receive";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.MessageKey = message.MessageKey;
            RequestProfile.Username = "";

            return DelegateHelper.Invoke<NotifyMessage, NotifyMessageResult>(m_buildFacade.Receive, message, MetricsKeys.BuilderService_DirectReceive);
        }

        public NotifyMessageResult ReceiveList(NotifyMessage[] notifyMessArray)
        {
            RequestProfile.RequestType = "JinRi.Notify.BuilderService.Receive";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();
            RequestProfile.MessageKey = "";
            RequestProfile.Username = "";
            List<NotifyMessage> list = notifyMessArray == null ? new List<NotifyMessage>() : notifyMessArray.ToList();
            return DelegateHelper.Invoke<List<NotifyMessage>, NotifyMessageResult>(m_buildFacade.ReceiveList, list, MetricsKeys.BuilderService_DirectReceive);
        }
    }
}
