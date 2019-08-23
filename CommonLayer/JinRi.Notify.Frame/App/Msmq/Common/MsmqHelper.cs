using System;
using System.Messaging;

namespace JinRi.Notify.Frame
{
	/// <summary>
	/// windows消息队列帮助类
	/// </summary>
	public class MsmqHelper
	{
        /// <summary>
        /// 禁用初始化函数
        /// </summary>
		private MsmqHelper()
		{
		}


        /// <summary>
        /// 根据消息队列路径，返回消息队列对象
        /// </summary>
        /// <param name="strMsmqPath">消息队列路径</param>
        /// <returns>消息队列对象</returns>
        public static MessageQueue GetQueue(string strMsmqPath)
        {
            MessageQueue objQueue = null;
            if (MessageQueue.Exists(strMsmqPath))
            {
                objQueue = new MessageQueue(strMsmqPath, false);
            }
            return objQueue;
        }

        /// <summary>
        /// 创建给定路径消息队列，如果存在，直接返回消息队列对象
        /// </summary>
        /// <param name="strMsmqPath">消息队列路径</param>
        /// <returns>消息队列对象</returns>
        public static MessageQueue CreateQueue(string strMsmqPath)
        {
            // Create the queue if not exist
            MessageQueue objTheQueue;
            try
            {
                if (!MessageQueue.Exists(strMsmqPath))
                {
                    objTheQueue = MessageQueue.Create(strMsmqPath);
                }
                else
                {
                    objTheQueue = new MessageQueue(strMsmqPath, false);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("创建队列出现错误", ex);
            }
            return objTheQueue;
        }

	}
}
