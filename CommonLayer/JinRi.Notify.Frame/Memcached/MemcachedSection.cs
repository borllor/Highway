using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Framework
{
    /// <summary>
    /// 任务分配算法节点配置域
    /// </summary>
    public class MemcachedSection : ConfigurationSection
    {
        [ConfigurationProperty("InitConnections", IsRequired = true)]
        public int InitConnections
        {
            get
            {
                return (int)base["InitConnections"];
            }
            set
            {
                base["InitConnections"] = value;
            }
        }

        [ConfigurationProperty("MinConnections", IsRequired = true)]
        public int MinConnections
        {
            get
            {
                return (int)base["MinConnections"];
            }
            set
            {
                base["MinConnections"] = value;
            }
        }

        [ConfigurationProperty("MaxConnections", IsRequired = true)]
        public int MaxConnections
        {
            get
            {
                return (int)base["MaxConnections"];
            }
            set
            {
                base["MaxConnections"] = value;
            }
        }

        [ConfigurationProperty("SocketConnectTimeout", IsRequired = true)]
        public int SocketConnectTimeout
        {
            get
            {
                return (int)base["SocketConnectTimeout"];
            }
            set
            {
                base["SocketConnectTimeout"] = value;
            }
        }

        [ConfigurationProperty("SocketTimeout", IsRequired = true)]
        public int SocketTimeout
        {
            get
            {
                return (int)base["SocketTimeout"];
            }
            set
            {
                base["SocketTimeout"] = value;
            }
        }

        [ConfigurationProperty("MaintenanceSleep", IsRequired = true)]
        public int MaintenanceSleep
        {
            get
            {
                return (int)base["MaintenanceSleep"];
            }
            set
            {
                base["MaintenanceSleep"] = value;
            }
        }

        [ConfigurationProperty("Failover", IsRequired = true)]
        public int Failover
        {
            get
            {
                return (int)base["Failover"];
            }
            set
            {
                base["Failover"] = value;
            }
        }


        [ConfigurationProperty("Nagle", IsRequired = true)]
        public int Nagle
        {
            get
            {
                return (int)base["Nagle"];
            }
            set
            {
                base["Nagle"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public MemcachedServerCollection Servers
        {
            get
            {
                MemcachedServerCollection col = (MemcachedServerCollection)base[""];
                return col;
            }
        }
    }
}
