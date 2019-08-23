using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel.Condition
{
    public class NotifyInterfaceSettingCondition : BaseCondition
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppId { get; set; }
        public int SettingId { get; set; }
    }
}
