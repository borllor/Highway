using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JinRi.Notify.Frame
{
    public delegate int PollingCallback(object state);
    public delegate void PollingFailedCallback(object state);

    public class PollingPool
    {
        private List<PollingMessage> m_pollingMessageList;

        public PollingPool(PollingMessage message)
        {
            m_pollingMessageList = new List<PollingMessage>();
        }

    }

    public class PollingMessage
    {
        #region 字段

        public readonly static int[] DefaultPollingTimes = null;
        private object _state;
        private DateTime _creationTime;
        private DateTime _lastExecutedTime;
        private PollingCallback _pollingCallback;
        private PollingFailedCallback _pollingFailedCallback;
        private int[] _pollingTimes;
        private int _pollingTimeIndex;

        #endregion

        #region 属性

        public int[] PollingTimes
        {
            get { return _pollingTimes; }
            set { _pollingTimes = value; }
        }

        public int PollingTimeIndex
        {
            get { return _pollingTimeIndex; }
            set { _pollingTimeIndex = value; }
        }

        public object State
        {
            get { return _state; }
            set { _state = value; }
        }

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { _creationTime = value; }
        }

        public DateTime LastExecutedTime
        {
            get { return _lastExecutedTime; }
            set { _lastExecutedTime = value; }
        }

        public PollingCallback PollingCallback
        {
            get { return _pollingCallback; }
            set { _pollingCallback = value; }
        }

        public PollingFailedCallback PollingFailedCallback
        {
            get { return _pollingFailedCallback; }
            set { _pollingFailedCallback = value; }
        }

        #endregion

        static PollingMessage()
        {
            DefaultPollingTimes = new int[] { 1000, 10 * 1000, 60 * 1000 };
        }

        public PollingMessage(PollingCallback pollingCallback, PollingFailedCallback failedCallback, object state)
            : this(DefaultPollingTimes, pollingCallback, failedCallback, state)
        {
        }

        public PollingMessage(int[] pollingTimes, PollingCallback pollingCallback, PollingFailedCallback failedCallback, object state)
        {
            _pollingCallback = pollingCallback;
            _pollingFailedCallback = failedCallback;
            _state = state;
            _creationTime = DateTime.Now;
            _lastExecutedTime = DateTime.Now;
            _pollingTimes = pollingTimes;
            _pollingTimeIndex = 0;
        }
    }
}
