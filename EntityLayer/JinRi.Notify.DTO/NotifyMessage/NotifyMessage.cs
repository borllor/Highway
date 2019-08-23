using System;
using System.Runtime.Serialization;
using JinRi.Notify.Model;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [Serializable]
    public class NotifyMessage
    {
        /// <summary>
        /// 消息Key
        /// </summary>
        [DataMember]
        public string MessageId { get; set; }
        /// <summary>
        /// 应用编号
        /// </summary>
        [DataMember]
        public string AppId { get; set; }
        /// <summary>
        /// 消息优先级
        /// </summary>
        [DataMember]
        public MessagePriorityEnum MessagePriority { get; set; }
        /// <summary>
        /// 数据key
        /// </summary>
        [DataMember]
        public string MessageKey { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        [DataMember]
        public string MessageType { get; set; }
        /// <summary>
        /// 通知数据
        /// </summary>
        [DataMember]
        public string NotifyData { get; set; }
        /// <summary>
        /// 数据来源
        /// </summary>
        [DataMember]
        public string SourceFrom { get; set; }
        /// <summary>
        /// 客户端ip
        /// </summary>
        [DataMember]
        public string ClientIP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        public NotifyMessage()
        {
            MessagePriority = MessagePriorityEnum.None;
        }
    }
}
