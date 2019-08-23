using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using JinRi.Notify.DTO;

namespace JinRi.Notify.ReceiverService
{
    [ServiceContract]
    [ServiceKnownType(typeof(NotifyMessage))]
    public interface IReceiveService
    {
        [OperationContract]
        NotifyMessageResult Notify(NotifyMessage message);
    }
}
