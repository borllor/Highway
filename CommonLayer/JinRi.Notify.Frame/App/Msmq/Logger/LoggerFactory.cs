using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 消息记录器工厂类
    /// </summary>
    public class LoggerFactory
    {
        /// <summary>
        /// 工厂方法
        /// </summary>
        /// <returns></returns>
        public static IAppLogger CreateLogger()
        {
            return new DBLogger();
        }
    }
}
