using JinRi.Notify.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel.Condition
{
    public class NotifyMessageCondition : BaseCondition
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 消息优先级
        /// </summary>
        public string MessagePriority { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime SCreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime ECreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MessageKey { get; set; }
        /// <summary>
        /// 查询来源，1=查询实时表，2=查询备份表
        /// </summary>
        public int QuerySource { get; set; }

        public NotifyMessageCondition()
        {
            OrderBy = "CreateTime";
            OrderDirection = OrderDirectionEnum.DESC;
            AppId = Null.NullInteger;
            SCreateTime = Null.NullDate;
            ECreateTime = Null.NullDate;
            QuerySource = 1;
        }
    }
}
