using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.DTO;
using JinRi.Notify.Business;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;

namespace JinRi.Notify.ReceiverService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReceiveService : IReceiveService
    {
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(ReceiveService));
        private static readonly ReceiveFacade m_receiveFacade = new ReceiveFacade();

        public NotifyMessageResult Notify(NotifyMessage message)
        {
            RequestProfile.RequestType = "JinRi.Notify.ReceiverService.Notify";
            RequestProfile.RequestKey = Guid.NewGuid().ToString();

            return DelegateHelper.Invoke<NotifyMessage, NotifyMessageResult>(m_receiveFacade.Receive, message, MetricsKeys.ReceiverService);
        }
    }
}
