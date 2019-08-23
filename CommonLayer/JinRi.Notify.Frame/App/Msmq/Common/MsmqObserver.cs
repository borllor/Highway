using System;
using System.Messaging;
using System.Threading;

namespace JinRi.Notify.Frame
{
	/// <summary>
    /// 观察者模式，用于异步监听消息队列，有消息时，回调的方法
	/// </summary>
	public class MsmqObserver
	{
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MsmqObserver));
        /// <summary>
        /// 用于异步监听的消息队列对象
        /// </summary>
        private MessageQueue m_objMsmq2Listen;
        /// <summary>
        /// 有消息时，回调的方法
        /// </summary>
        private WaitCallback m_objSubscriberMethod;
        /// <summary>
        /// 用于标记程序是否运行
        /// </summary>
        private bool m_blnRunning = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="objMQ">用于异步监听的消息队列对象</param>
        /// <param name="objSubscriberMethod">有消息时，回调的方法</param>
        public MsmqObserver(MessageQueue objMQ, WaitCallback objSubscriberMethod)
        {
            m_objMsmq2Listen = objMQ;
            objMQ.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessageReceived);
            m_objSubscriberMethod = objSubscriberMethod;
        }

        /// <summary>
        /// 开始异步监听消息
        /// </summary>
        private void Observe()
        {
            m_objMsmq2Listen.BeginReceive();
        }

        private void OnMessageReceived(object sender, ReceiveCompletedEventArgs objArgs)
        {
            try
            {
                Message objRawMsg = m_objMsmq2Listen.EndReceive(objArgs.AsyncResult);

                //改为同步方式
                m_objSubscriberMethod(objRawMsg);

                objRawMsg.Dispose();
                objRawMsg = null;
            }
            catch(Exception ex)
            {
                Logger.Error("接收消息发生异常：" + LogMessageDAL.GetString(ex));
            }
            finally
            {
                if (m_blnRunning)
                {
                    m_objMsmq2Listen.BeginReceive();
                }
            }
        }

        /// <summary>
        /// 开启消息监听
        /// </summary>
        public void Start()
        {
            if (!m_blnRunning)
            {
                Console.WriteLine("开始接收消息队列消息");
                m_blnRunning = true;
                Observe();
            }
        }

        /// <summary>
        /// 停止消息监听
        /// </summary>
        public void Terminate()
        {
            if (m_blnRunning)
            {
                Console.WriteLine("停止接收消息队列消息");
                m_blnRunning = false;
            }
        }
	}
}



