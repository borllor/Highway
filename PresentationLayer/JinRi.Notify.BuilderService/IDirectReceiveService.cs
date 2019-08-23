using System;
using System.ServiceModel;

using JinRi.Notify.DTO;

namespace JinRi.Notify.BuilderService
{
    [ServiceContract]
    [ServiceKnownType(typeof(NotifyMessage))]
    public interface IDirectReceiveService
    {
        [OperationContract]
        NotifyMessageResult Receive(NotifyMessage message);

        [OperationContract]
        NotifyMessageResult ReceiveList(NotifyMessage[] array);
    }
}
