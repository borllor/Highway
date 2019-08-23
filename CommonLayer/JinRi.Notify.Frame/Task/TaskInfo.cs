using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JinRi.Notify.Frame
{
    public delegate void PropertyChanged(string propertyName, string originalValue, string value);

    [Serializable]
    public class TaskInfo : ITaskInfo
    {
        #region 成员

        protected string _taskKey;
        protected string _taskName;
        protected string _emitServerCode;
        protected TaskProcess _taskProcess;
        protected TaskStatus _taskStatus;
        protected DateTime _taskCreateTime;
        protected DateTime _taskStartTime;
        protected DateTime _taskCompleteTime;
        protected string _taskRemark;

        #endregion

        #region 属性

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskKey
        {
            get { return _taskKey; }
            set { _taskKey = value; }
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        /// <summary>
        /// 发出任务的服务器编号
        /// </summary>
        public string EmitServerCode
        {
            get { return _emitServerCode; }
            set { _emitServerCode = value; }
        }

        /// <summary>
        /// 任务处理进度
        /// </summary>
        public TaskProcess TaskProcess
        {
            get { return _taskProcess; }
            set { _taskProcess = value; }
        }

        /// <summary>
        /// 任务当前状态
        /// </summary>
        public TaskStatus TaskStatus
        {
            get { return _taskStatus; }
            set { _taskStatus = value; }
        }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime TaskCreateTime
        {
            get { return _taskCreateTime; }
            set { _taskCreateTime = value; }
        }

        /// <summary>
        /// 任务处理开始时间
        /// </summary>
        public DateTime TaskStartTime
        {
            get { return _taskStartTime; }
            set { _taskStartTime = value; }
        }

        /// <summary>
        /// 任务处理结束时间
        /// </summary>
        public DateTime TaskCompleteTime
        {
            get { return _taskCompleteTime; }
            set { _taskCompleteTime = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string TaskRemark
        {
            get { return _taskRemark; }
            set { _taskRemark = value; }
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memStream, this);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = binaryFormatter.Deserialize(memStream);
                return obj;
            }
        }

        #endregion
    }
}
