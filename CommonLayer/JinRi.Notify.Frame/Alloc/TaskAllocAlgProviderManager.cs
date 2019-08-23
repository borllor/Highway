using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 分布式任务分配算法管理器
    /// </summary>
    public class TaskAllocAlgProviderManager
    {
        private static object m_taskAllocObj = new object();
        private static Dictionary<string, string> m_taskAllocDic = new Dictionary<string, string>();
        public static string DefaultProvider = "";

        public static void Add(string providerName, string providerType)
        {
            Add(providerName, providerType, false);
        }

        public static void Add(string providerName, string providerType, bool isDefault)
        {
            lock (m_taskAllocObj)
            {
                if (!m_taskAllocDic.ContainsKey(providerName))
                {
                    if (isDefault) DefaultProvider = providerName;
                    m_taskAllocDic.Add(providerName, providerType);
                }
            }
        }

        public static void Remove(string providerName)
        {
            lock (m_taskAllocObj)
            {
                if (m_taskAllocDic.ContainsKey(providerName))
                {
                    m_taskAllocDic.Remove(providerName);
                }
            }
        }

        public static string GetProvider(string providerName)
        {
            lock (m_taskAllocObj)
            {
                return m_taskAllocDic[providerName];
            }
        }

        public static string GetDefaultProvider()
        {
            return GetProvider(DefaultProvider);
        }
    }
}
