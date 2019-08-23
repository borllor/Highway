using System;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Configuration;
using System.Messaging;
using System.Collections.Generic;
using System.Collections;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 消息队列日志
    /// </summary>
    public class MsmqLogger : IAppLogger
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MsmqLogger));
        /// <summary>
        /// 配置文件中的消息队列地址
        /// </summary>
        public static string s_strMsmqPath = AppSetting.AppLoggerMSMQ;
        /// <summary>
        /// windows消息队列消息记录器对象
        /// </summary>
        private static MsmqLogger m_objInstance;
        /// <summary>
        /// 异步消息观察器
        /// </summary>
        private MsmqObserver m_objMsmqObserver;
        /// <summary>
        /// 实际消息记录对象
        /// </summary>
        private IAppLogger m_objRealLogger;
        /// <summary>
        /// ?⒍恿卸韵蛱
        /// </summary>
        private MessageQueue m_objQueue;
        /// <summary>
        /// 用于标记程序是否运行
        /// </summary>
        private bool m_blnRunning = false;
        /// <summary>
        /// 队列是否初始化成功
        /// </summary>
        private bool m_isInitMsmqSuccessed = false;
        private readonly static object m_initLockObject = new object();

        private IDataBufferPool m_handlePool;
        private IDataBufferPool m_processPool;

        /// <summary>
        /// 构造函数
        /// </summary>
        private MsmqLogger()
        {
            Setup();
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static MsmqLogger GetInstance()
        {
            if (m_objInstance != null)
            {
                return m_objInstance;
            }
            lock (m_initLockObject)
            {
                if (m_objInstance == null)
                {
                    MsmqLogger temp = new MsmqLogger();
                    m_objInstance = temp;
                }
            }
            return m_objInstance;
        }

        /// <summary>
        /// 初始化相关业务对象
        /// </summary>
        private void Setup()
        {
            try
            {
                m_objQueue = MsmqHelper.GetQueue(s_strMsmqPath);
                if (null == m_objQueue)
                {
                    m_isInitMsmqSuccessed = false;
                    Logger.Error("队列不存在 - " + s_strMsmqPath, new ApplicationException("队列不存在 - " + s_strMsmqPath));
                }
                else
                {
                    m_isInitMsmqSuccessed = true;
                    m_objQueue.Formatter = new XmlMessageFormatter(new string[] { "JinRi.Notify.Frame.LogMessage, JinRi.Notify.Frame" });
                    m_objMsmqObserver = new MsmqObserver(m_objQueue, new WaitCallback(NotifyMe));
                }
                m_objRealLogger = LoggerFactory.CreateLogger();
                m_handlePool = new DataBufferPool(new WaitCallback(LogHandleBatchMessage));
                m_processPool = new DataBufferPool(new WaitCallback(LogProcessBatchMessage));
            }
            catch (Exception ex)
            {
                Logger.Error("获取队列时产生异常 - " + s_strMsmqPath, ex);
            }
        }

        /// <summary>
        /// 发送消息到消息队列
        /// </summary>
        /// <param name="objMessage">消息对象</param>
        public bool SendMessage(LogMessage objMessage)
        {
            bool result = false;
            if (m_blnRunning)
            {
                if (m_isInitMsmqSuccessed)
                {
                    Message objRawMessage = new Message(objMessage);
                    try
                    {
                        m_objQueue.Send(objRawMessage);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                        m_isInitMsmqSuccessed = false;
                        ThreadPool.QueueUserWorkItem(x => LogMessage(objMessage));
                    }
                    finally
                    {
                        objRawMessage = null;
                    }
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(x => LogMessage(objMessage));
                }
            }
            return result;
        }

        /// <summary>
        /// 异步消息观察器回调方法
        /// </summary>
        /// <param name="obj">消息对象</param>
        private void NotifyMe(object obj)
        {
            Message objRawMsg = (Message)obj;
            LogMessage objActualMsg = (LogMessage)objRawMsg.Body;

            LogMessage(objActualMsg);

            objActualMsg = null;
            objRawMsg = null;
        }

        #region IAppLogger Members

        /// <summary>
        /// IAppLoggger.LogMessage 实现.
        /// </summary>
        /// <param name="objMessage">消息对象</param>
        public void LogMessage(object objMessage)
        {
            int count = 0;
            LogMessage objActualMsg = (LogMessage)objMessage;
            if (objActualMsg.IsHandle)
            {
                m_handlePool.Write(objActualMsg);
            }
            else
            {
                m_processPool.Write(objActualMsg);
            }
        }

        public void FlushMessage()
        {
            m_handlePool.Flush();
            m_processPool.Flush();
            while (true)
            {
                if (m_handlePool.IsFlushed())
                {
                    break;
                }
                Thread.Sleep(10);
            }
            while (true)
            {
                if (m_processPool.IsFlushed())
                {
                    break;
                }
                Thread.Sleep(10);
            }
        }

        private void LogHandleBatchMessage(object obj)
        {
            IDataBuffer buffer = obj as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                LogBatchMessage(buffer.GetEnumerator(), true);
            }
        }

        private void LogProcessBatchMessage(object obj)
        {
            IDataBuffer buffer = obj as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                LogBatchMessage(buffer.GetEnumerator(), false);
            }
        }

        private void LogBatchMessage(IEnumerator tor, bool isHandle)
        {
            object[] arr = new object[2];
            arr[0] = tor;
            arr[1] = isHandle;
            m_objRealLogger.LogMessage(arr);
        }

        #endregion

        #region 更改日志记录器运行状态

        /// <summary>
        /// 启动记录器
        /// </summary>
        public void Start()
        {
            if (!m_blnRunning)
            {
                if (m_objMsmqObserver != null) m_objMsmqObserver.Start();
                m_blnRunning = true;
                Logger.Debug("MsmqLogger is started");
            }
        }

        /// <summary>
        /// 关闭记录器
        /// </summary>
        public void Shutdown()
        {
            if (m_blnRunning)
            {
                if (m_objMsmqObserver != null) m_objMsmqObserver.Terminate();
                m_blnRunning = false;
                FlushMessage();
                Thread.Sleep(2000);
                Logger.Debug("MsmqLogger is shutdown");
            }
        }

        #endregion
    }
}
