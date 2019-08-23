using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using EasyNetQ;
using EasyNetQ.Loggers;
using EasyNetQ.Topology;
using JinRi.Notify.Utility;

namespace JinRi.Notify.Business
{
    public class RabbitMQBus
    {
        private IAdvancedBus _bus;

        public IAdvancedBus Bus
        {
            get { return _bus; }
        }

        public RabbitMQBus()
        {
            InitBus();
        }

        public void Close()
        {
            if (_bus != null)
            {
                _bus.Dispose();
                _bus = null;
            }
        }

        private void InitBus()
        {
            if (_bus == null)
            {
                string mqHost = ConfigurationAppSetting.RabbitMQHost;
                Check.IsNullOrEmpty(mqHost, "RabbitMQHost");
                _bus = RabbitHutch.CreateBus(mqHost, reg => reg.Register<IEasyNetQLogger>(log => new Log4NetLogger())).Advanced;
                Check.IsNull(_bus, "执行RabbitHutch.CreateBus后，IAdvancedBus");
            }
        }
    }
}
