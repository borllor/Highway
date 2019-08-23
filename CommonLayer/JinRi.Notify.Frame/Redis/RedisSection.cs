using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Framework
{
    /// <summary>
    /// 任务分配算法节点配置域
    /// </summary>
    public class RedisSection : ConfigurationSection
    {
        [ConfigurationProperty("AutoStart", DefaultValue = true, IsRequired = true)]
        public bool AutoStart
        {
            get
            {
                return (bool)base["AutoStart"];
            }
            set
            {
                base["AutoStart"] = value;
            }
        }

        [ConfigurationProperty("DefaultDb", DefaultValue = (long)0, IsRequired = false)]
        public long DefaultDb
        {
            get
            {
                return (long)base["DefaultDb"];
            }
            set
            {
                base["DefaultDb"] = value;
            }
        }

        [ConfigurationProperty("MaxReadPoolSize", IsRequired = true)]
        public int MaxReadPoolSize
        {
            get
            {
                return (int)base["MaxReadPoolSize"];
            }
            set
            {
                base["MaxReadPoolSize"] = value;
            }
        }

        [ConfigurationProperty("MaxWritePoolSize", IsRequired = true)]
        public int MaxWritePoolSize
        {
            get
            {
                return (int)base["MaxWritePoolSize"];
            }
            set
            {
                base["MaxWritePoolSize"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public RedisServerCollection Servers
        {
            get
            {
                RedisServerCollection col = (RedisServerCollection)base[""];
                return col;
            }
        }
    }
}
