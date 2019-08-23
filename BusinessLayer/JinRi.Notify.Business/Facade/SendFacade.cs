using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Model;


namespace JinRi.Notify.Business
{
    public class SendFacade
    {
        private SendMessageBusiness _sendPushMessageBus;

        public SendFacade()
        {
            _sendPushMessageBus = new SendMessageBusiness();
        }
        public PushMessageResult ReceiveList(List<PushMessage> list)
        {
            return _sendPushMessageBus.Send(list);
        }

        public PushMessageResult Receive(PushMessage message)
        {
            return _sendPushMessageBus.Send(message);
        }

        public PushCallbackResult Callback(PushMessageResult result)
        {
            return _sendPushMessageBus.Callback(result);
        }

        public void FlushPool()
        {
            _sendPushMessageBus.FlushPool();
        }
    }
}
