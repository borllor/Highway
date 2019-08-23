using System;
using System.Collections.Generic;
using System.Threading;
using JinRi.Notify.Frame.Util;
#if THRIFT
using JinRi.Notify.Frame.Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using JinRi.Notify.Frame.Metrics;
#endif


namespace JinRi.Notify.Frame
{
    public static class DBLog
    {
        private static readonly IServerInfo m_localServer;
        private static readonly string m_logToServer;
        private static readonly string m_localServerIP;
        private static readonly string m_localServerCode;
        private static readonly ITaskRequest m_request;
        private static readonly IDataBufferPool m_logHandlePool;
        private static readonly IDataBufferPool m_logProcessPool;
        private static readonly ILog m_localLog;
        
#if THRIFT
        private static bool m_openLogCenter = true;
        private static LogService.Client m_client;
        private static List<TTransport> m_transportList;
        private static object m_lockObj = new object();
        private static int m_pollingSize = 3;
        private static int m_pollingIndex = 0;
        public static ManualResetEvent SyncLogServerEvent = new ManualResetEvent(true);
        private static ManualResetEvent m_initThriftServerEvent = new ManualResetEvent(true);
#else
        private static bool m_openLogCenter = false;
#endif
        public static int LogHandleCount = 0;
        public static int LogProcessCount = 0;
        //最后一次执行日志时间,默认为5分钟之前
        private static DateTime lastExecuteLogTime = DateTime.Now.AddMinutes(-5);
        static DBLog()
        {
            try
            {
                m_localLog = LoggerSource.Instance.GetLogger(typeof(DBLog));

                AppSetting.InitDistributionConfig();
                m_logToServer = AppSetting.LogToServer;
                m_localServer = ServerManager.GetLocalServer();
                m_localServerIP = IPHelper.GetLocalIP();

                m_localServerCode = m_localServer != null ? m_localServer.ServerCode : m_localServerIP;
                if (m_openLogCenter)
                {
#if THRIFT
                    ZKManager.Client.Init();
                    InitThriftServer();
#endif
                }

                m_request = RegisterService.TaskRequestService.GetService(m_logToServer);

                m_logHandlePool = new DataBufferPool(BatchSendHandleRequest)
                {
                    AutoFlushLogSeconds = 5,
                    IsBlockMainThread = false
                };
                m_logProcessPool = new DataBufferPool(BatchSendProcessRequest)
                {
                    AutoFlushLogSeconds = 3,
                    IsBlockMainThread = false
                };
            }
            catch (Exception ex)
            {
                m_localLog.Error(ex);
            }
        }

        public static void SetOpenLogCenterState(bool isOpen)
        {
            m_openLogCenter = isOpen;
        }

        public static void Process(this LogMessage logMessage)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                Interlocked.Increment(ref LogProcessCount);
                try
                {
                    m_logProcessPool.Write(logMessage);
                }
                catch (Exception ex)
                {

                    try
                    {
                        if (logMessage != null)
                        {
                            logMessage.Content = string.Format("ex: {0}, log: {1}", ex.GetString(), logMessage.Content);
                        }
                        InsertLog(logMessage, false);
                        m_localLog.Error(ex);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                Interlocked.Decrement(ref LogProcessCount);
            });
        }

        public static void Handle(this LogMessage logMessage)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                Interlocked.Increment(ref LogHandleCount);
                try
                {
                    m_logHandlePool.Write(logMessage);
                }
                catch (Exception ex)
                {

                    try
                    {
                        if (logMessage != null)
                        {
                            logMessage.Content = string.Format("ex: {0}, log: {1}", ex.GetString(), logMessage.Content);
                        }
                        InsertLog(logMessage, true);
                        m_localLog.Error(ex);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                Interlocked.Decrement(ref LogHandleCount);
            });
        }

#if THRIFT


        /// <summary>
        /// 初始化Thrift远程服务
        /// </summary>
        public static void InitThriftServer()
        {
            m_initThriftServerEvent.WaitOne();
            List<TTransport> transportList = m_transportList;
            if (transportList == null)
            {
                transportList = new List<TTransport>();
            }

            ReleaseThriftServer(transportList);

            IServerInfo serverInfo = ZKManager.Client.GetAliveLogServer(m_logToServer);
            //IServerInfo serverInfo = ServerManager.Get(m_logToServer);
            m_localLog.Error(string.Format("serverip: {0}, port: {1}", serverInfo.Address, serverInfo.Port));
            if (serverInfo != null)
            {
                for (int i = 0; i < m_pollingSize; i++)
                {
                    TFramedTransport transport = new TFramedTransport(new TSocket(serverInfo.Address, serverInfo.Port, serverInfo.Timeout));
                    transportList.Add(transport);
                }
            }
            m_transportList = transportList;
        }

        private static void ReleaseThriftServer(List<TTransport> transportList)
        {
            if (transportList != null && transportList.Count > 0)
            {
                foreach (var transport in transportList)
                {
                    if (transport.IsOpen)
                    {
                        transport.Close();
                    }
                    transport.Dispose();
                }
                transportList.Clear();
            }
        }

        private static LogService.Client GetThriftClient()
        {
            lock (m_lockObj)
            {
                if (m_transportList == null || m_transportList.Count == 0)
                {
                    InitThriftServer();
                }
                m_pollingIndex = m_pollingIndex > (m_pollingSize - 1) ? 0 : m_pollingIndex;
                TTransport transport = m_transportList[m_pollingIndex];
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                TCompactProtocol protocol = new TCompactProtocol(transport);
                LogService.Client client = new LogService.Client(protocol);
                m_pollingIndex++;
                return client;
            }
        }

        private static void SendRequestToNewLogCenter(List<LogMessage> logMessages)
        {
            m_initThriftServerEvent.Reset();
            if (logMessages == null || logMessages.Count == 0)
            {
                return;
            }
            var logMessageList = new List<JinRi.Notify.Frame.Thrift.LogMessage>();
            try
            {
                logMessages.ForEach(m =>
                {
                    logMessageList.Add(m.Mapping());
                });
                m_client = GetThriftClient();
                m_client.submit(logMessageList);
                m_initThriftServerEvent.Set();
            }
            catch (Exception ex)
            {
                m_initThriftServerEvent.Set();
                if (m_transportList != null)
                {
                    m_transportList.Clear();
                }
                throw ex;
            }
        }

        private static JinRi.Notify.Frame.Thrift.LogMessage Mapping(this LogMessage logMessage)
        {
            JinRi.Notify.Frame.Thrift.LogMessage _logMessage = new Thrift.LogMessage();
            _logMessage.Id = logMessage.Id;
            _logMessage.Ikey = logMessage.Ikey;
            _logMessage.Username = logMessage.Username;
            _logMessage.LogTime = logMessage.LogTime.Ticks;
            _logMessage.ClientIP = logMessage.ClientIP;
            _logMessage.ServerIP = logMessage.ServerIP;
            _logMessage.Module = logMessage.Module;
            _logMessage.OrderNo = logMessage.OrderNo;
            _logMessage.LogType = logMessage.LogType;
            _logMessage.Keyword = logMessage.Keyword;
            _logMessage.Content = logMessage.Content;
            _logMessage.IsHandle = logMessage.IsHandle;
            return _logMessage;
        }
#endif

        public static void Process(string userName, string iKey, string clientIP, string module, string orderNo,
            string logType, string content, string keyword, DateTime ctime)
        {

            LogMessage logMessage = new LogMessage
            {
                Ikey = iKey,
                Username = userName,
                Module = module,
                LogType = logType,
                Content = content,
                IsHandle = false,
                Keyword = keyword,
                OrderNo = orderNo,
                ClientIP = clientIP,
                ServerIP = m_localServerIP,
                LogTime = ctime
            };

            Process(logMessage);
        }

        /// <summary>
        /// 写入日志(执行过程)
        /// </summary>
        /// <param name="userName">用户账号</param>
        /// <param name="iKey">GUID</param>
        /// <param name="clientIP">客户端IP</param>
        /// <param name="module">操作模块(类名)</param>
        /// <param name="orderNo">订单号</param>
        /// <param name="logType">日志类型</param>
        /// <param name="content">日志内容</param>
        /// <param name="keyword">关键字(可用于统计信息的记录)</param>
        public static void Process(string userName, string iKey, string clientIP, string module, string orderNo, string logType, string content, string keyword = "")
        {
            Process(userName, iKey, clientIP, module, orderNo, logType, content, keyword, DateTime.Now);
        }

        /// <summary>
        /// 写入日志(请求值/返回值)
        /// </summary>
        /// <param name="userName">用户账号</param>
        /// <param name="iKey">GUID</param>
        /// <param name="clientIP">客户端IP</param>
        /// <param name="module">操作模块(类名)</param>
        /// <param name="orderNo">订单号</param>
        /// <param name="logType">日志类型</param>
        /// <param name="content">日志内容</param>
        /// <param name="keyword">关键字(可用于统计信息的记录)</param>
        public static void Handle(string userName, string iKey, string clientIP, string module, string orderNo, string logType, string content, string keyword = "")
        {
            LogMessage logMessage = new LogMessage
            {
                Ikey = iKey,
                Username = userName,
                Module = module,
                LogType = logType,
                Content = content,
                IsHandle = true,
                Keyword = keyword,
                OrderNo = orderNo,
                ClientIP = clientIP,
                ServerIP = m_localServerIP,
                LogTime = DateTime.Now
            };
            Handle(logMessage);
        }

        //直接插入日志表
        /// <summary>
        /// 直接插入日志表
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="isHandle"></param>
        private static void InsertLog(LogMessage logMessage, bool isHandle)
        {
            LogMessageDAL.GetInstance().Insert(new List<LogMessage> { logMessage }.GetEnumerator(), isHandle);
        }

        //分布式批量提交Handle数据到远程
        /// <summary>
        /// 分布式批量提交Handle数据到远程
        /// </summary>
        /// <param name="obj"></param>
        private static void BatchSendHandleRequest(object obj)
        {
            BatchSendRequest(obj, true);
        }

        //分布式批量提交Process数据到远程
        /// <summary>
        /// 分布式批量提交Process数据到远程
        /// </summary>
        /// <param name="obj"></param>
        private static void BatchSendProcessRequest(object obj)
        {
            BatchSendRequest(obj, false);
        }

        //分布式批量提交数据到远程
        /// <summary>
        /// 分布式批量提交数据到远程
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isHandle"></param>
        private static void BatchSendRequest(object obj, bool isHandle)
        {
            //获取日志实体列表
            IDataBuffer buffer = obj as IDataBuffer;
            if (buffer == null)
            {
                return;
            }
            List<LogMessage> logMessageList = buffer.GetList<LogMessage>();
            if (logMessageList == null)
            {
                return;
            }

            if (m_openLogCenter)
            {
#if THRIFT
                try
                {
                    SyncLogServerEvent.WaitOne();
                    SendRequestToNewLogCenter(logMessageList);
                    if (logMessageList.Count > 1 && DateTime.Now > lastExecuteLogTime.AddMinutes(5))
                    {
                        lastExecuteLogTime = DateTime.Now;
                        string content = string.Format("本次批量插入{0}条数据，结果：{1}，IsHandle：{2}", logMessageList.Count, "Success", isHandle);
                        RecordLogCenterState("分布式日志2.0", content, isHandle, true);
                    }
                    AppMetricsKeys.LogCenterClient.MeterMark(MetricsEnum.Success.ToString(), new string[] { string.Format("Client={0}", MetricsTagEnum.V2.ToString()) });
                    return;
                }
                catch (Exception ex)
                {
                    AppMetricsKeys.LogCenterClient.MeterMark(MetricsEnum.Failed.ToString(), new string[] { string.Format("Client={0}", MetricsTagEnum.V2.ToString()) });
                    m_localLog.Error(ex);
                    string content = string.Format("批量插入数据异常，原因：{0}，IsHandle：{1}", ex.ToString(), isHandle);
                    RecordLogCenterState("分布式日志2.0", content, isHandle, false);
                }
#endif
            }
            else
            {
                try
                {
                    LogMessageTaskInfo taskInfo = new LogMessageTaskInfo
                    {
                        CurrLogMessageObj = logMessageList,
                        TaskCreateTime = DateTime.Now,
                        TaskName = "LogMessage",
                        EmitServerCode = m_localServerCode
                    };

                    //发送日志实体列表到远程消息队列
                    ITaskInfo retTask = m_request.SendRequest(m_logToServer, taskInfo);

                    if (logMessageList.Count > 1
                        && DateTime.Now > lastExecuteLogTime.AddMinutes(5))
                    {
                        lastExecuteLogTime = DateTime.Now;
                        string content = string.Format("本次批量插入{0}条数据，结果：{1}，IsHandle：{2}", logMessageList.Count, retTask.TaskStatus, isHandle);
                        RecordLogCenterState("分布式日志1.0", content, isHandle, retTask.TaskStatus == TaskStatus.Success);
                    }

                    if (retTask.TaskStatus == TaskStatus.Success)
                    {
#if THRIFT
                        AppMetricsKeys.LogCenterClient.MeterMark(MetricsEnum.Success.ToString(), new string[] { string.Format("Client={0}", MetricsTagEnum.V1.ToString()) });
#endif
                        return;
                    }
                }
                catch (Exception ex)
                {
#if THRIFT
                    AppMetricsKeys.LogCenterClient.MeterMark(MetricsEnum.Failed.ToString(), new string[] { string.Format("Client={0}", MetricsTagEnum.V1.ToString()) });
#endif
                    m_localLog.Error(ex);
                    string content = string.Format("批量插入数据异常，原因：{0}，IsHandle：{1}", ex.ToString(), isHandle);
                    RecordLogCenterState("分布式日志1.0", content, isHandle, false);
                }
            }
            int insertResult = LogMessageDAL.GetInstance().Insert(buffer.GetEnumerator(), isHandle);
#if THRIFT
            if (insertResult > 0)
            {
                AppMetricsKeys.LogCenterClient.MeterMark(MetricsEnum.Success.ToString(), new string[] { string.Format("Client={0}", MetricsTagEnum.Local.ToString()) });
            }
            else
            {
                AppMetricsKeys.LogCenterClient.MeterMark(MetricsEnum.Failed.ToString(), new string[] { string.Format("Client={0}", MetricsTagEnum.Local.ToString()) });
            }
#endif
            RecordLogCenterState("本地批量保存日志", string.Format("本地批量插入{0}条数据, IsHandler: {1}", logMessageList.Count, isHandle), isHandle, false);
        }

        private static void RecordLogCenterState(string logType, string content, bool isHandle, bool isSucess)
        {
            LogMessage logMessage = new LogMessage()
            {
                Ikey = Guid.NewGuid().ToString(),
                LogType = logType,
                Module = "",
                Content = content ,
                IsHandle = isHandle,
                Username = "nouser",
                Keyword = isSucess ? "Success" : "Failed",
                OrderNo = "",
                ClientIP = "",
                ServerIP = m_localServerIP,
                LogTime = DateTime.Now
            };
            InsertLog(logMessage, isHandle);
        }

        //立即批量提交本地缓冲池
        /// <summary>
        /// 立即批量提交本地缓冲池
        /// </summary>
        public static void FlushLogMessage()
        {
            try
            {
                m_logHandlePool.Flush();
                m_logProcessPool.Flush();
                while (true)
                {
                    if (m_logHandlePool.IsFlushed())
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                while (true)
                {
                    if (m_logProcessPool.IsFlushed())
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                m_localLog.Error(ex);
            }
        }

    }
}
