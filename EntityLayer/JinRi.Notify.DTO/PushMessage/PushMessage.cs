using System;
using System.Linq;
using System.Runtime.Serialization;

using JinRi.Notify.Model;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [Serializable]
    public class PushMessage
    {
        /// <summary>
        /// 推送消息Key
        /// </summary>
        [DataMember]
        public string PushId { get; set; }
        /// <summary>
        /// 消息Key
        /// </summary>
        [DataMember]
        public string MessageId { get; set; }
        /// <summary>
        /// 配制Key
        /// </summary>
        [DataMember]
        public int SettingId { get; set; }
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
        public string PushData { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public int PushCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }
    }
}
