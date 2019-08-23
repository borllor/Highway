using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    /// <summary>
    /// 消息枚举类型
    /// </summary>
    public class NotifyMessageTypeModel
    {
        /// <summary>
        /// 消息类型自增ID
        /// </summary>
        public int MessageTypeId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 消息类型名称
        /// </summary>
        public string MessageTypeName { get; set; }

        /// <summary>
        /// 消息说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 消息状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 消息优先级 
        /// </summary>
        public MessagePriorityEnum MessagePriority { get; set; }

        public string MessagePriorityName { get; set; }

        public NotifyMessageTypeModel()
        {
            MessagePriority = MessagePriorityEnum.None;
        }
    }
}
