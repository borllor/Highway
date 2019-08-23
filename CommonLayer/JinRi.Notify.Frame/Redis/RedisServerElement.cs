using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Framework
{
    /// <summary>
    /// Memcached服务器节点
    /// </summary>
    public class RedisServerElement : ConfigurationElement
    {
        public RedisServerElement()
        {
        }

        public RedisServerElement(string address, int port, int access)
        {
            Address = address;
            Port = port;
            Access = access;
        }

        /// <summary>
        /// 服务器唯一标示
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
        /// 类型
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

        /// <summary>
        /// 读写属性(1表示只写，2表示只读，4表示读写)
        /// </summary>
        [ConfigurationProperty("Access", DefaultValue = 4, IsRequired = true)]
        public int Access
        {
            get
            {
                return (int)this["Access"];
            }
            set
            {
                this["Access"] = value;
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
            return string.Format("Address:{0},Port:{1},Access:{2}", Address, Port, Access);
        }
    }
}
