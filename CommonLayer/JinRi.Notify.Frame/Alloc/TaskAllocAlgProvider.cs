using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 分布式任务分配算法提供程序
    /// </summary>
    public class TaskAllocAlgProvider : ITaskAllocAlg
    {
        /// <summary>
        /// 分布式任务分配算法
        /// </summary>
        /// <param name="taskInfo">任务信息</param>
        /// <param name="info">服务器信息</param>
        /// <returns>是否在此服务器上执行任务(true:是，false:否)</returns>
        public bool Alloc(ITaskInfo taskInfo, IServerInfo info)
        {
            return true;
        }
    }
}
