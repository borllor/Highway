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
    public class DataBuffer : IDataBuffer
    {
        private readonly WaitCallback _callback;
        private readonly int _bufferSize;
        private readonly List<object> m_buffer;
        private string _bufferId;
        private int _isFull;
        private int _count;
        private readonly object m_bufferLockObj = new object();

        public WaitCallback Callback
        {
            get
            {
                return _callback;
            }
        }

        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
        }

        public int Count
        {
            get
            {
                return Interlocked.CompareExchange(ref _count, 0, 0);
            }
        }

        public string BufferId
        {
            get { return _bufferId; }
            set { _bufferId = value; }
        }

        public DataBuffer()
            : this(null, AppSetting.DataBufferSize)
        {
        }

        public DataBuffer(int bufferSize)
            : this(null, bufferSize)
        {
        }

        public DataBuffer(WaitCallback callback, int bufferSize)
        {
            _callback = callback;
            _bufferSize = bufferSize;
            m_buffer = new List<object>((int)_bufferSize);
            _count = 0;
            _isFull = 0;
            _bufferId = Guid.NewGuid().ToString();
        }

        public bool IsFull()
        {
            int tmpCount = Interlocked.CompareExchange(ref _isFull, 0, 0); //先判断标志
            if (tmpCount == 1)
            {
                return true;
            }
            tmpCount = Interlocked.CompareExchange(ref _count, 0, 0);
            if (tmpCount < _bufferSize) //再判断容量
            {
                return false;
            }
            return true;
        }

        public void Flush()
        {
            Interlocked.Exchange(ref _isFull, 1);
            if (Callback != null)
            {
                ThreadPool.QueueUserWorkItem(Callback, this);
            }
        }

        public bool Write(object data)
        {
            if (!IsFull())
            {
                lock (m_bufferLockObj)
                {
                    if (!IsFull())
                    {
                        m_buffer.Add(data);
                        Interlocked.Increment(ref _count);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            if (IsFull())
            {
                Flush();
            }
            return true;
        }

        public IEnumerator GetEnumerator()
        {
            lock (m_bufferLockObj)
            {
                object[] arr = new object[m_buffer.Count];
                m_buffer.CopyTo(arr, 0);
                return arr.GetEnumerator();
            }
        }

        public List<T> GetList<T>() where T : class
        {
            lock (m_bufferLockObj)
            {
                //空处理
                if (m_buffer == null || m_buffer.Count == 0)
                {
                    return null;
                }
                List<T> res = new List<T>();
                foreach (object obj in m_buffer)
                {
                    if (obj is T)
                    {
                        res.Add(obj as T);
                    }
                }
                return res;
            }
        }
    }
}