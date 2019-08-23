using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 消息记录实现接口
    /// </summary>
    public interface IAppLogger
    {
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="obj">消息对象</param>
        void LogMessage(object obj);
    }
}
