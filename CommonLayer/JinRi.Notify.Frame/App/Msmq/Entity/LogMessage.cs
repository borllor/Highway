using System;

namespace JinRi.Notify.Frame
{
    [Serializable]
    public class LogMessage
    {
        #region 字段

        private int _id;
        private string _ikey;
        private string _username;
        private DateTime _logTime;
        private string _clientIP;
        private string _serverIP;
        private string _module;
        private string _orderNo;
        private string _logType;
        private string _keyword;
        private string _content;
        private bool _isHandle;

        #endregion

        #region 属性

        /// <summary>
        /// 日志编号
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// ikey，表示一次请求的所有日志相同的Key值
        /// </summary>
        public string Ikey
        {
            get { return _ikey; }
            set { _ikey = value; }
        }

        /// <summary>
        /// 请求者的用户名
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime LogTime
        {
            get { return _logTime; }
            set { _logTime = value; }
        }

        /// <summary>
        /// 请求者的IP地址
        /// </summary>
        public string ClientIP
        {
            get { return _clientIP; }
            set { _clientIP = value; }
        }

        /// <summary>
        /// 服务器的IP地址
        /// </summary>
        public string ServerIP
        {
            get { return _serverIP; }
            set { _serverIP = value; }
        }

        /// <summary>
        /// 日志所在的模块信息
        /// </summary>
        public string Module
        {
            get { return _module; }
            set { _module = value; }
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public string LogType
        {
            get { return _logType; }
            set { _logType = value; }
        }

        /// <summary>
        /// 供检索所用的关键词(主要用于统计数据)
        /// </summary>
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// 是输入输出日志，还是过程日志(true: 输入输出日志，反之)
        /// </summary>
        public bool IsHandle
        {
            get { return _isHandle; }
            set { _isHandle = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}:{1}, ", "Ikey", Ikey) +
                   string.Format("{0}:{1}, ", "Username", Username) +
                   string.Format("{0}:{1}, ", "LogTime", LogTime) +
                   string.Format("{0}:{1}, ", "ClientIp", ClientIP) +
                   string.Format("{0}:{1}, ", "ServerIP", ServerIP) +
                   string.Format("{0}:{1}, ", "Module", Module) +
                   string.Format("{0}:{1}, ", "OrderNo", OrderNo) +
                   string.Format("{0}:{1}, ", "LogType", LogType) +
                   string.Format("{0}:{1}, ", "Keyword", Keyword) +
                   string.Format("{0}:{1}", "IsHandle", IsHandle);
        }
    }

    public static class LogMessageFactory
    {
        public static LogMessage CreateHandleLogEntity(string logType, string iKey)
        {
            var handlelog = new LogMessage
            {
                Ikey = iKey,
                Username = "nouser",
                Module = "nomodule",
                LogType = logType,
                Content = "nocontent",
                IsHandle = false,
                Keyword = "nokeyword",
                OrderNo = "noorderno",
                ClientIP = ClientHelper.GetClientIP(),
                ServerIP = IPHelper.GetLocalIP(),
            };
            if (string.IsNullOrEmpty(iKey))
            {
                handlelog.Ikey = Guid.NewGuid().ToString();
            }
            return handlelog;
        }
    }
}
