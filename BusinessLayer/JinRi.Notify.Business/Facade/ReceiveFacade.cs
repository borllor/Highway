using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;


namespace JinRi.Notify.Business
{
    public class ReceiveFacade
    {
        private static readonly ReceiveMessageBusiness _receiveBus = new ReceiveMessageBusiness();

        public NotifyMessageResult Receive(NotifyMessage message)
        {
            return _receiveBus.Receive(message);
        }

        public void FlushPool()
        {
            _receiveBus.FlushPool();
        }
    }
}
