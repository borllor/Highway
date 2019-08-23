using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using JinRi.Notify.DTO;

namespace JinRi.Notify.SenderService
{
    [ServiceContract]
    [ServiceKnownType(typeof(PushMessage))]
    public interface ISendService
    {      
        [OperationContract]
        PushMessageResult Receive(PushMessage message);
       
        [OperationContract]
        PushMessageResult ReceiveList(PushMessage[] array);

        [OperationContract]
        PushCallbackResult Callback(PushMessageResult result);
    }
}
