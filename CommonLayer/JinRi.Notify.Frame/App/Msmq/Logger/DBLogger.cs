using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class DBLogger : IAppLogger
    {
        private List<LogMessageTaskInfo> m_logMessage;

        #region IAppLogger 成员

        public void LogMessage(object obj)
        {
            LogMessageDAL.GetInstance().Insert(obj);
        }

        #endregion
    }
}
