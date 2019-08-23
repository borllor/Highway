#if THRIFT
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JinRi.Notify.Frame.Thrift
{
    public class ThriftServer
    {
        private static ThriftServer instance = new ThriftServer();
        public static ThriftServer CreateInstance()
        {
            return instance;
        }
        private ThriftServer()
        {
            Configuration config = AppSetting.GetConfiguration();
            ThriftSection thriftSection = config.GetSection("Thrift") as ThriftSection;
            if (thriftSection != null && thriftSection.Servers != null && thriftSection.Servers.Count > 0)
            {
                ThriftServerElement element = thriftSection.Servers[0];
                Address = element.Address;
                Port = element.Port;
                TimeOut = thriftSection.TimeOut;
            }
        }

        public string Address { get; set; }
        public int Port { get; set; }
        public int TimeOut { get; set; }
        
    }
}
#endif