using System;
using System.Collections;
using System.Messaging;
using System.Threading;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 数据缓冲器，管理池
    /// </summary>
    public interface IDataBufferPool
    {
        WaitCallback Callback { get; }
        int PoolSize { get; }
        int AutoFlushLogSeconds { get; set; }
        void Flush();
        bool IsFlushed();
        void Write(object data);
    }
}



