using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Entity
{
    public class NotifyMessageEntity
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 应用编号
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 消息优先级
        /// </summary>
        public string MessagePriority { get; set; }
        /// <summary>
        /// 数据key
        /// </summary>
        public string MessageKey { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// 通知数据
        /// </summary>
        public string NotifyData { get; set; }
        /// <summary>
        /// 数据来源
        /// </summary>
        public string SourceFrom { get; set; }
        /// <summary>
        /// 客户端ip
        /// </summary>
        public string ClientIP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
