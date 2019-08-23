using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class TaskResult
    {
        #region 字段

        private bool _isSuccess;
        private string _message;
        private object _data;
        private bool _isContinuteGet;
        private ITaskInfo _taskInfo;

        #endregion

        #region 属性

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess
        {
            get { return _isSuccess; }
            set { _isSuccess = value; }
        }
        
        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// 是否继续获取数据
        /// </summary>
        public bool IsContinuteGet
        {
            get { return _isContinuteGet; }
            set { _isContinuteGet = value; }
        }

        /// <summary>
        /// 处理的任务对象
        /// </summary>
        public ITaskInfo TaskInfo
        {
            get { return _taskInfo; }
            set { _taskInfo = value; }
        }

        #endregion
    }
}
