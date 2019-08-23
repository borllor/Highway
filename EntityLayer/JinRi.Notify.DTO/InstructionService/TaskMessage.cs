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
    public class TaskMessage
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [DataMember]
        public string TaskID { get; set; }

        [DataMember]
        public long BatchNumber { get; set; }

        /// <summary>
        /// 执行对象
        /// </summary>
        [DataMember]
        public string TaskExecutor { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        [DataMember]
        public TaskTypeEnum TaskType { get; set; }

        /// <summary>
        /// 任务参数
        /// </summary>
        [DataMember]
        public string TaskParam { get; set; }

        /// <summary>
        /// 消息执行状态
        /// </summary>
        [DataMember]
        public TaskMessageStatusEnum Status { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
        public TaskMessage()
        {
            Status = TaskMessageStatusEnum.None;
        }

    }
}
