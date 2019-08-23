using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using EasyNetQ;
using EasyNetQ.Loggers;
using EasyNetQ.Topology;

namespace JinRi.Notify.Business
{
    public static class RabbitMQBusMgr
    {
        private static RabbitMQBus _bus;
        private static readonly object SyncObj = new object();

        public static RabbitMQBus Instance
        {
            get
            {
                if (_bus == null)
                {
                    lock (SyncObj)
                    {
                        if (_bus == null)
                        {
                            InitBus();
                        }
                    }
                }
                return _bus;
            }
        }

        public static void Close()
        {
            if (_bus != null)
            {
                _bus.Close();
            }
        }

        private static void InitBus()
        {
            if (_bus == null)
            {
                _bus = new RabbitMQBus();
            }
        }
    }
}
