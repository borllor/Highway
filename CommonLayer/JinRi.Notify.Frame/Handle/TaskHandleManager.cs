using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class TaskHandleManager
    {
        private static object m_taskHandleObj = new object();
        private static Dictionary<string, string> m_taskHandleDic = new Dictionary<string, string>();

        public static void Add(string taskName, string taskHandleType)
        {
            lock (m_taskHandleObj)
            {
                if (!m_taskHandleDic.ContainsKey(taskName))
                {
                    m_taskHandleDic.Add(taskName, taskHandleType);
                }
            }
        }

        public static void Remove(string taskName)
        {
            lock (m_taskHandleObj)
            {
                if (m_taskHandleDic.ContainsKey(taskName))
                {
                    m_taskHandleDic.Remove(taskName);
                }
            }
        }

        public static string GetHandle(string taskName)
        {
            lock (m_taskHandleObj)
            {
                return m_taskHandleDic[taskName];
            }
        }
    }
}
