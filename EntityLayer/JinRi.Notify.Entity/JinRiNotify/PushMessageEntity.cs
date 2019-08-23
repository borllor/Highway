using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Entity
{
    public class PushMessageEntity
    {
        /// <summary>
        /// 推送消息Key
        /// </summary>
        public string PushId { get; set; }
        /// <summary>
        /// 消息Key
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 配置Key
        /// </summary>
        public int SettingId { get; set; }
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
        public string PushData { get; set; }
        /// <summary>
        /// 通知状态
        /// </summary>
        public int PushStatus { get; set; }
        /// <summary>
        /// 下次推送时间
        /// </summary>
        public DateTime NextPushTime { get; set; }
        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }
        /// <summary>
        /// 消息创建时间
        /// </summary>
        public DateTime MessageCreateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 推送次数
        /// </summary>
        public int PushCount { get; set; }
        /// <summary>
        /// 备注(追加方式)
        /// </summary>
        public string Memo { get; set; }
    }
}
