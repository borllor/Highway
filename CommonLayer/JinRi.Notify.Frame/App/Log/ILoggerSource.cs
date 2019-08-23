using System;
using log4net;

namespace JinRi.Notify.Frame
{
    public interface ILoggerSource
    {
        ILog GetLogger(Type type);
        ILog GetLogger(string name);
    }
}