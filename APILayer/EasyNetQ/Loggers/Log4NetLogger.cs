using System;

namespace EasyNetQ.Loggers
{
    /// <summary>
    /// noop logger
    /// </summary>
    public class Log4NetLogger : IEasyNetQLogger
    {
        private static readonly JinRi.Notify.Frame.ILog m_logger = JinRi.Notify.Frame.LoggerSource.Instance.GetLogger(typeof(Log4NetLogger));

        public void DebugWrite(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                m_logger.Debug(format);
            }
            else
            {
                m_logger.Debug(string.Format(format, args));
            }
        }

        public void InfoWrite(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                m_logger.Info(format);
            }
            else
            {
                m_logger.Info(string.Format(format, args));
            }
        }

        public void ErrorWrite(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                m_logger.Error(format);
            }
            else
            {
                m_logger.Error(string.Format(format, args));
            }
        }

        public void ErrorWrite(Exception exception)
        {
            m_logger.Error(exception.Message, exception);
        }
    }
}