using System;
using System.Collections.Generic;
using System.Threading;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 数据缓冲池
    /// </summary>
    public class DataBufferPool : IDataBufferPool
    {
        private readonly WaitCallback _callback;
        private IDataBuffer m_dataBuffer;
        private readonly Stack<IDataBuffer> m_dataStack = new Stack<IDataBuffer>();
        private readonly object m_dataStackLockObj = new object();
        private readonly object m_newDataBufferLockObj = new object();
        private readonly object m_lastAutoFlushTimeLockObj = new object();
        private readonly Thread m_callStatckThread;
        private int _isFlush;
        private int _hasAllPoped;
        private int _count;
        private readonly int _poolSize;

        private DateTime _lastAutoFlushTime;
        private Thread _autoFlushLogThread;
        private bool _isBlockMainThread = true;

        public WaitCallback Callback
        {
            get
            {
                return _callback;
            }
        }

        public int PoolSize
        {
            get
            {
                return _poolSize;
            }
        }

        private int _autoFlushSeconds = 60;

        public int AutoFlushLogSeconds
        {
            get { return _autoFlushSeconds; }
            set { _autoFlushSeconds = value; }
        }

        public bool IsBlockMainThread
        {
            get { return _isBlockMainThread; }
            set { _isBlockMainThread = value; }
        }


        private void AutoFlushCallBack()
        {
            while (true)
            {
                if (DateTime.Now.AddSeconds(-AutoFlushLogSeconds) >= GetLastAutoFlushTime())
                {
                    UpdateLastAutoFlushTime();
                    if (m_dataBuffer.Count > 0)
                    {
                        lock (m_dataStackLockObj)
                        {
                            m_dataStack.Push(m_dataBuffer);
                        }
                        Interlocked.Increment(ref _count);
                        m_dataBuffer = new DataBuffer();
                    }
                }
                Thread.Sleep(1000 * AutoFlushLogSeconds * 1);
            }
        }

        public DataBufferPool(WaitCallback callback)
            : this(callback, 60, true)
        {
        }

        public DataBufferPool(WaitCallback callback, bool isBlockMainThread)
            : this(callback, 60, isBlockMainThread)
        {
        }

        public DataBufferPool(WaitCallback callback, int autoFlushSeconds)
            : this(callback, autoFlushSeconds, true)
        {

        }

        public DataBufferPool(WaitCallback callback, int autoFlushSeconds, bool isBlockMainThread)
            : this(callback, 0, autoFlushSeconds, true)
        {
        }

        public DataBufferPool(WaitCallback callback, int dataBufferSize, int autoFlushSeconds, bool isBlockMainThread)
        {
            _isBlockMainThread = isBlockMainThread;
            _autoFlushSeconds = autoFlushSeconds;
            _callback = callback;
            _poolSize = AppSetting.DataBufferPoolSize;
            if (dataBufferSize <= 0) m_dataBuffer = new DataBuffer();
            else m_dataBuffer = new DataBuffer(dataBufferSize);
            m_callStatckThread = new Thread(CallStack);
            m_callStatckThread.Start();

            _lastAutoFlushTime = DateTime.Now;
            _autoFlushLogThread = new Thread(AutoFlushCallBack);
            _autoFlushLogThread.Start();
        }

        public void Flush()
        {
            Interlocked.Exchange(ref _isFlush, 1);
            m_dataBuffer.Flush();
            lock (m_dataStackLockObj)
            {
                if (m_dataBuffer != null && m_dataBuffer.Count > 0)
                {
                    m_dataStack.Push(m_dataBuffer);
                }
                Interlocked.Increment(ref _count);
                NewDataBuffer();
            }
        }

        public void Write(object data)
        {
            bool result = m_dataBuffer.Write(data);
            if (!result)
            {
                lock (m_dataStackLockObj)
                {
                    if (m_dataBuffer.IsFull() && m_dataBuffer.Count > 0)
                    {
                        UpdateLastAutoFlushTime();
                        m_dataStack.Push(m_dataBuffer);
                        Interlocked.Increment(ref _count);
                        NewDataBuffer();
                    }
                }
                Write(data);
            }
            while (IsBlockMainThread)
            {
                long tmpCount = Interlocked.CompareExchange(ref _count, 0, 0);
                if (tmpCount < PoolSize)
                {
                    break;
                }
                Thread.Sleep(100);
            }
        }

        public bool IsFlushed()
        {
            if (1 == Interlocked.CompareExchange(ref _hasAllPoped, 0, 0))
            {
                return true;
            }
            return false;
        }

        private void NewDataBuffer()
        {
            lock (m_newDataBufferLockObj)
            {
                m_dataBuffer = new DataBuffer();
            }
        }

        private void UpdateLastAutoFlushTime()
        {
            lock (m_lastAutoFlushTimeLockObj)
            {
                _lastAutoFlushTime = DateTime.Now;
            }
        }

        private DateTime GetLastAutoFlushTime()
        {
            lock (m_lastAutoFlushTimeLockObj)
            {
                return _lastAutoFlushTime;
            }
        }

        private void CallStack()
        {
            while (true)
            {
                IDataBuffer dataBuffer = null;
                lock (m_dataStackLockObj)
                {
                    if (m_dataStack.Count > 0)
                    {
                        dataBuffer = m_dataStack.Pop();
                        Interlocked.Decrement(ref _count);
                    }
                }
                if (dataBuffer != null) Callback(dataBuffer);
                if (1 == Interlocked.CompareExchange(ref _isFlush, 0, 0))
                {
                    if (0 == Interlocked.CompareExchange(ref _count, 0, 0))
                    {
                        _hasAllPoped = 1;
                        break;
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}