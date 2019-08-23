using System;
using System.Collections;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 数据缓冲器
    /// </summary>
    public interface IDataBuffer
    {
        WaitCallback Callback { get; }
        string BufferId { get; }
        int BufferSize { get; }
        int Count { get; }
        bool IsFull();
        void Flush();
        bool Write(object data);
        IEnumerator GetEnumerator();
        List<T> GetList<T>() where T : class;
    }
}



