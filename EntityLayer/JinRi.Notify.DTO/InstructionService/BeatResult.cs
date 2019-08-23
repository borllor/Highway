using JinRi.Notify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [Serializable]
    public class BeatResult
    {
        /// <summary>
        /// 执行任务列表
        /// </summary>
        [DataMember]
        public List<TaskMessage> TaskList { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        [DataMember]
        public BeatResultEnum Result { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [DataMember]
        public string ErrorMsg { get; set; }
    }
}
