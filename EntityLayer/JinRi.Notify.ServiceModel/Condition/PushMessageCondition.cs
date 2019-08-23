using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JinRi.Notify.Model;
using JinRi.Notify.Frame;

namespace JinRi.Notify.ServiceModel.Condition
{
    public class PushMessageCondition : BaseCondition
    {
        public string MessageId { get; set; }
        public string PushId { get; set; }
        public List<string> MessageType { get; set; }
        public string MessagePriority { get; set; }  
        public DateTime SNextPushTime { get; set; }
        public DateTime ENextPushTime { get; set; }
        public int PushStatus { get; set; }
        public string MessageKey { get; set; }

        public List<string> PushIds { get; set; }

        /// <summary>
        /// 查询来源，1=查询实时表，2=查询备份表
        /// </summary>
        public int QuerySource { get; set; }

        public PushMessageCondition()
        {
            OrderBy = "CreateTime";
            OrderDirection = OrderDirectionEnum.DESC;         
            SNextPushTime = Null.NullDate;
            ENextPushTime = Null.NullDate;
            PushStatus = Null.NullInteger;
            QuerySource = 1;
        }
    }
}
