using System;
using System.Collections.Generic;
using System.Threading;

using JinRi.Notify.Frame;
using JinRi.Notify.Entity;
using JinRi.Notify.Utility;
using JinRi.Notify.DB;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.ServiceModel;

namespace JinRi.Notify.Business
{
    public static class Handle
    {
        public static void Debug(string content)
        {
            Debug(content, "");
        }

        public static void Debug(string content, string keyword)
        {
            Debug(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Debug(string logtype, string module, string content, string keyword)
        {
            Debug(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Debug(string logtype, string module, string orderNo, string content, string keyword)
        {
            Debug(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Debug(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Debug >= LogSetting.LogLevel)
                DBLog.Handle(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Debug" : keyword);
        }

        public static void Info(string content)
        {
            Info(content, "");
        }

        public static void Info(string content, string keyword)
        {
            Info(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Info(string logtype, string module, string content, string keyword)
        {
            Info(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Info(string logtype, string module, string orderNo, string content, string keyword)
        {
            Info(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Info(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Info >= LogSetting.LogLevel)
                DBLog.Handle(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Info" : keyword);
        }

        public static void Warning(string content)
        {
            Warning(content, "");
        }

        public static void Warning(string content, string keyword)
        {
            Warning(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Warning(string logtype, string module, string content, string keyword)
        {
            Warning(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Warning(string logtype, string module, string orderNo, string content, string keyword)
        {
            Warning(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Warning(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Warning >= LogSetting.LogLevel)
                DBLog.Handle(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Warning" : keyword);
        }

        public static void Error(string content)
        {
            Error(content, "");
        }

        public static void Error(string content, string keyword)
        {
            Error(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Error(string logtype, string module, string content, string keyword)
        {
            Error(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Error(string logtype, string module, string orderNo, string content, string keyword)
        {
            Error(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Error(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Error >= LogSetting.LogLevel)
                DBLog.Handle(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Error" : keyword);
        }

        public static void Fatal(string content)
        {
            Fatal(content, "");
        }

        public static void Fatal(string content, string keyword)
        {
            Fatal(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Fatal(string logtype, string module, string content, string keyword)
        {
            Fatal(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Fatal(string logtype, string module, string orderNo, string content, string keyword)
        {
            Fatal(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Fatal(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Fatal >= LogSetting.LogLevel)
                DBLog.Handle(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Fatal" : keyword);
        }
    }

    public static class Process
    {
        public static void Debug(string content)
        {
            Debug(content, "");
        }

        public static void Debug(string content, string keyword)
        {
            Debug(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Debug(string logtype, string module, string content, string keyword)
        {
            Debug(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Debug(string logtype, string module, string orderNo, string content, string keyword)
        {
            Debug(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Debug(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Debug >= LogSetting.LogLevel)
                DBLog.Process(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Debug" : keyword);
        }

        public static void Info(string content)
        {
            Info(content, "");
        }

        public static void Info(string content, string keyword)
        {
            Info(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Info(string logtype, string module, string content, string keyword)
        {
            Info(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Info(string logtype, string module, string orderNo, string content, string keyword)
        {
            Info(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Info(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Info >= LogSetting.LogLevel)
                DBLog.Process(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Info" : keyword);
        }

        public static void Warning(string content)
        {
            Warning(content, "");
        }

        public static void Warning(string content, string keyword)
        {
            Warning(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Warning(string logtype, string module, string content, string keyword)
        {
            Warning(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Warning(string logtype, string module, string orderNo, string content, string keyword)
        {
            Warning(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Warning(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Warning >= LogSetting.LogLevel)
                DBLog.Process(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Warning" : keyword);
        }

        public static void Error(string content)
        {
            Error(content, "");
        }

        public static void Error(string content, string keyword)
        {
            Error(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Error(string logtype, string module, string content, string keyword)
        {
            Error(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Error(string logtype, string module, string orderNo, string content, string keyword)
        {
            Error(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Error(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Error >= LogSetting.LogLevel)
                DBLog.Process(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Error" : keyword);
        }

        public static void Fatal(string content)
        {
            Fatal(content, "");
        }

        public static void Fatal(string content, string keyword)
        {
            Fatal(RequestProfile.RequestType, "", content, keyword);
        }

        public static void Fatal(string logtype, string module, string content, string keyword)
        {
            Fatal(logtype, module, RequestProfile.MessageKey, content, keyword);
        }

        public static void Fatal(string logtype, string module, string orderNo, string content, string keyword)
        {
            Fatal(RequestProfile.RequestKey, logtype, module, orderNo, content, keyword);
        }

        public static void Fatal(string ikey, string logtype, string module, string orderNo, string content, string keyword)
        {
            if (LogLevelEnum.Fatal >= LogSetting.LogLevel)
                DBLog.Process(RequestProfile.Username, ikey, RequestProfile.ClientIP, module, orderNo, logtype, content, string.IsNullOrWhiteSpace(keyword) ? "Fatal" : keyword);
        }
    }
}
