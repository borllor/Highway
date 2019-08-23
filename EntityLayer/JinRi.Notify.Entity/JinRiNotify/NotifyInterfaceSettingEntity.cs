using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Entity
{
    public class NotifyInterfaceSettingEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public int SettingId { get; set; }
        /// <summary>
        /// 应用编号
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// 消息接口处理地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 请求方法 POST/GET
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 推送次数阀值
        /// </summary>
        public int PushLimitCount { get; set; }
        /// <summary>
        /// 失败推送规则0,5,15
        /// </summary>
        public string PushInternalRule { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary>
        public string Encoding { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最新创建时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int Status { get; set; }
    }
}
