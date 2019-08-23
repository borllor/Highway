using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel.Condition
{
    public class NotifyMessageTypeCondition : BaseCondition
    {
        /// <summary>
        /// 消息通知类型ID
        /// </summary>
        public int MessageTypeId { get; set; }
    }
}
