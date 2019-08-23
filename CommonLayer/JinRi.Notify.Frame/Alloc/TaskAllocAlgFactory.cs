using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 分布式任务分配算法工厂
    /// </summary>
    public class TaskAllocAlgFactory
    {
        private static object m_taskAllocAlgObj = new object();
        private static ITaskAllocAlg m_taskAllocAlg;

        /// <summary>
        /// 获取分布式任务分配算法提供程序(默认配置点名称：TaskAllocAlgProvider)
        /// </summary>
        /// <returns>分布式任务分配算法</returns>
        public static ITaskAllocAlg GetProvider()
        {
            return GetProvider(TaskAllocAlgProviderManager.DefaultProvider);
        }

        /// <summary>
        /// 获取分布式任务分配算法提供程序
        /// </summary>
        /// <param name="taskInfo">配置名称</param>
        /// <returns>分布式任务分配算法</returns>
        public static ITaskAllocAlg GetProvider(string settingName)
        {
            if (m_taskAllocAlg == null)
            {
                lock (m_taskAllocAlgObj)
                {
                    if (m_taskAllocAlg == null)
                    {
                        string assemblyInfo = InitProvider(settingName);
                        string[] arr = assemblyInfo.Split(new char[] { ',' });
                        if (arr != null && arr.Length == 2)
                        {
                            Type t = Type.GetType(assemblyInfo);
                            ObjectHandle oh = Activator.CreateInstance(arr[1].Trim(), arr[0].Trim());
                            if (oh != null)
                            {
                                m_taskAllocAlg = oh.Unwrap() as ITaskAllocAlg;
                            }
                        }
                    }
                }
            }
            return m_taskAllocAlg;
        }

        private static string InitProvider(string settingName)
        {
            return TaskAllocAlgProviderManager.GetProvider(settingName);
        }
    }
}
