using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    public class TaskMessageModel
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskID { get; set; }

        public long BatchNumber { get; set; }

        /// <summary>
        /// 执行对象
        /// </summary>
        public string TaskExecutor { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskTypeEnum TaskType { get; set; }

        public string TaskTypeName
        {
            get
            {
                return TaskType.ToString();
            }
        }

        /// <summary>
        /// 任务参数
        /// </summary>
        public string TaskParam { get; set; }

        /// <summary>
        /// 消息执行状态
        /// </summary>
        public TaskMessageStatusEnum Status { get; set; }

        public string StatusName
        {
            get
            {
                return Status.ToString();
            }
        }

        public DateTime CreateTime { get; set; }

        public TaskMessageModel()
        {
            Status = TaskMessageStatusEnum.None;
        }

    }
}
