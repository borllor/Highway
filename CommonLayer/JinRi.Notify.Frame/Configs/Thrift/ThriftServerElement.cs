using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// ThriftServer服务器节点
    /// </summary>
    public class ThriftServerElement : ConfigurationElement
    {
        public ThriftServerElement()
        {
        }

        public ThriftServerElement(string address, int port)
        {
            Address = address;
            Port = port;
        }

        /// <summary>
        /// 服务器地址
        /// </summary>
        [ConfigurationProperty("Address", DefaultValue = "*.*.*.*", IsRequired = true, IsKey = true)]
        public string Address
        {
            get
            {
                return (string)this["Address"];
            }
            set
            {
                this["Address"] = value;
            }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        [ConfigurationProperty("Port", DefaultValue = 8080, IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)this["Port"];
            }
            set
            {
                this["Port"] = value;
            }
        }

        protected override bool IsModified()
        {
            bool ret = base.IsModified();

            // Enter your custom processing code here.

            Console.WriteLine("UrlConfigElement.IsModified() called.");

            return ret;
        }

        public override string ToString()
        {
            return string.Format("Address:{0},Port:{1}", Address, Port);
        }
    }
}
