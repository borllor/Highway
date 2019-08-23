using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    [Serializable]
    [Flags]
    public enum MessageTypeEnum
    {
        None = 0,
        OrderPayResult = 1,
        OrderTicketOut = 2,
        FlightDelay = 4,
        FlightCancel = 8,
        NotifyZBNTicketOut = 16,
        NotifyZBNReturn = 32,
        OrderZBNRefund = 64,
        OrderApplyReturn = 128,
        OrderApplyRefund = 256,
        OrderReturnResult = 512,
        OrderRefundResult = 1024,
        OrderCancel = 2048,
        OrderCreated = 4096
    }
}
