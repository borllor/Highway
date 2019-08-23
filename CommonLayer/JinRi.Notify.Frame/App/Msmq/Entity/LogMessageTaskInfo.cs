using System;
using System.Collections.Generic;

namespace JinRi.Notify.Frame
{
    [Serializable]
    public class LogMessageTaskInfo : TaskInfo
    {
        #region 字段

        private object _currLogMessageObj;

        #endregion

        #region 属性

        public object CurrLogMessageObj
        {
            get { return _currLogMessageObj; }
            set { _currLogMessageObj = value; }
        }

        #endregion

        public bool IsList
        {
            get
            {
                return CurrLogMessageObj is List<LogMessage>;
            }
        }

        public List<LogMessage> CurrLogMessageList
        {
            get
            {
                return CurrLogMessageObj as List<LogMessage>;
            }
        }

        public LogMessage CurrLogMessage
        {
            get
            {
                return CurrLogMessageObj as LogMessage;
            }
        }

    }
}
