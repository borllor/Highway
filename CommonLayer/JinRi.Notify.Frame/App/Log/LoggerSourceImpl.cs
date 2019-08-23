using System;
using System.Globalization;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Util;

namespace JinRi.Notify.Frame
{
    public class LoggerSourceImpl : ILoggerSource
    {
        public ILog GetLogger(Type type)
        {
#if DEBUG
            //return new EmptyLog();
            return new Logger(LogManager.GetLogger(type).Logger, type);
#else
            return new Logger(LogManager.GetLogger(type).Logger, type);
#endif
        }

        public ILog GetLogger(string name)
        {
#if DEBUG
            //return new EmptyLog();
            return new Logger(LogManager.GetLogger(name).Logger, null);
#else
            return new Logger(LogManager.GetLogger(name).Logger, null);
#endif
        }

        class Logger : LoggerWrapperImpl, ILog
        {
            //FATAL> ERROR> WARN > INFO > DEBUG
            private static Level _levelTrace;
            private static Level _levelDebug;
            private static Level _levelInfo;
            private static Level _levelWarn;
            private static Level _levelError;
            private static Level _levelFatal;
            //add custom logging levels (below trace value of 20000)
            //internal static Level LevelLogInfo = new Level(10001, "LogInfo");
            //internal static Level LevelLogError = new Level(10002, "LogError");

            private readonly Type _stackBoundary = typeof(Logger);
            private const string ConfigFile = "JinRi.Notify.Frame.config";
            private static bool _configured;
            private static readonly object ConfigLock = new object();

            internal Logger(ILogger logger, Type type)
                : base(logger)
            {
                _stackBoundary = type ?? typeof(Logger);
                EnsureConfig();
                ReloadLevels(logger.Repository);
            }

            private static void EnsureConfig()
            {
                if (!_configured)
                {
                    lock (ConfigLock)
                    {
                        if (!_configured)
                        {
                            string configFilename = AppSetting.GetConfigFilename();
                            if (!string.IsNullOrEmpty(configFilename))
                            {
                                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilename));
                            }
                            _configured = true;
                        }

                    }
                }
            }

            private static void ReloadLevels(ILoggerRepository repository)
            {
                LevelMap levelMap = repository.LevelMap;

                _levelTrace = levelMap.LookupWithDefault(Level.Trace);
                _levelDebug = levelMap.LookupWithDefault(Level.Debug);
                _levelInfo = levelMap.LookupWithDefault(Level.Info);
                _levelWarn = levelMap.LookupWithDefault(Level.Warn);
                _levelError = levelMap.LookupWithDefault(Level.Error);
                _levelFatal = levelMap.LookupWithDefault(Level.Fatal);
                //                LevelLogError = levelMap.LookupWithDefault(LevelLogError);
                //                LevelLogInfo = levelMap.LookupWithDefault(LevelLogInfo);

                //// Register custom logging levels with the default LoggerRepository
                //                LogManager.GetRepository().LevelMap.Add(LevelLogInfo);
                //                LogManager.GetRepository().LevelMap.Add(LevelLogError);

            }

            public bool IsDebugEnabled { get { return Logger.IsEnabledFor(_levelDebug); } }
            public bool IsInfoEnabled { get { return Logger.IsEnabledFor(_levelInfo); } }
            public bool IsTraceEnabled { get { return Logger.IsEnabledFor(_levelTrace); } }
            public bool IsWarnEnabled { get { return Logger.IsEnabledFor(_levelWarn); } }
            public bool IsErrorEnabled { get { return Logger.IsEnabledFor(_levelError); } }
            public bool IsFatalEnabled { get { return Logger.IsEnabledFor(_levelFatal); } }

            public void Debug(object message)
            {
                Debug(message, null);
            }

            public void Debug(object message, Exception exception)
            {
                Logger.Log(_stackBoundary, _levelDebug, message, exception);
            }

            public void DebugFormat(string format, params object[] args)
            {
                DebugFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void DebugFormat(IFormatProvider provider, string format, params object[] args)
            {
                Logger.Log(_stackBoundary, _levelDebug, new SystemStringFormat(provider, format, args), null);
            }

            public void Info(object message)
            {
                Info(message, null);
            }

            public void Info(object message, Exception exception)
            {
                Logger.Log(_stackBoundary, _levelInfo, message, exception);
            }

            public void InfoFormat(string format, params object[] args)
            {
                InfoFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void InfoFormat(IFormatProvider provider, string format, params object[] args)
            {
                Logger.Log(_stackBoundary, _levelInfo, new SystemStringFormat(provider, format, args), null);
            }

            public void Trace(object message)
            {
                Trace(message, null);
            }

            public void Trace(object message, Exception exception)
            {
                Logger.Log(_stackBoundary, _levelTrace, message, exception);
            }

            public void TraceFormat(string format, params object[] args)
            {
                TraceFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void TraceFormat(IFormatProvider provider, string format, params object[] args)
            {
                Logger.Log(_stackBoundary, _levelTrace, new SystemStringFormat(provider, format, args), null);
            }

            public void Warn(object message)
            {
                Warn(message, null);
            }

            public void Warn(object message, Exception exception)
            {
                Logger.Log(_stackBoundary, _levelWarn, message, exception);
            }

            public void WarnFormat(string format, params object[] args)
            {
                WarnFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void WarnFormat(IFormatProvider provider, string format, params object[] args)
            {
                Logger.Log(_stackBoundary, _levelWarn, new SystemStringFormat(provider, format, args), null);
            }

            public void Error(object message)
            {
                Error(message, null);
            }

            public void Error(object message, Exception exception)
            {
                Logger.Log(_stackBoundary, _levelError, message, exception);
            }

            public void ErrorFormat(string format, params object[] args)
            {
                ErrorFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
            {
                Logger.Log(_stackBoundary, _levelError, new SystemStringFormat(provider, format, args), null);
            }

            public void Fatal(object message)
            {
                Fatal(message, null);
            }

            public void Fatal(object message, Exception exception)
            {
                Logger.Log(_stackBoundary, _levelFatal, message, exception);
            }

            public void FatalFormat(string format, params object[] args)
            {
                FatalFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void FatalFormat(IFormatProvider provider, string format, params object[] args)
            {
                Logger.Log(_stackBoundary, _levelFatal, new SystemStringFormat(provider, format, args), null);
            }
        }
    }
    public class EmptyLog : ILog
    {
        bool ILog.IsDebugEnabled
        {
            get { return false; }
        }

        bool ILog.IsInfoEnabled
        {
            get { return false; }
        }

        bool ILog.IsTraceEnabled
        {
            get { return false; }
        }

        bool ILog.IsWarnEnabled
        {
            get { return false; }
        }

        bool ILog.IsErrorEnabled
        {
            get { return false; }
        }

        bool ILog.IsFatalEnabled
        {
            get { return false; }
        }

        void ILog.Debug(object message)
        {
        }

        void ILog.Debug(object message, Exception exception)
        {
        }

        void ILog.DebugFormat(string format, params object[] args)
        {
        }

        void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        void ILog.Info(object message)
        {
        }

        void ILog.Info(object message, Exception exception)
        {
        }

        void ILog.InfoFormat(string format, params object[] args)
        {
        }

        void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        void ILog.Trace(object message)
        {
        }

        void ILog.Trace(object message, Exception exception)
        {
        }

        void ILog.TraceFormat(string format, params object[] args)
        {
        }

        void ILog.TraceFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        void ILog.Warn(object message)
        {
        }

        void ILog.Warn(object message, Exception exception)
        {
        }

        void ILog.WarnFormat(string format, params object[] args)
        {
        }

        void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        void ILog.Error(object message)
        {
        }

        void ILog.Error(object message, Exception exception)
        {
        }

        void ILog.ErrorFormat(string format, params object[] args)
        {
        }

        void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        void ILog.Fatal(object message)
        {
        }

        void ILog.Fatal(object message, Exception exception)
        {
        }

        void ILog.FatalFormat(string format, params object[] args)
        {
        }

        void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
        }
    }

}