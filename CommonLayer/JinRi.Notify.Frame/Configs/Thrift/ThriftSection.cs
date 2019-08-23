using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Notify.Frame
{
    public class ThriftSection : ConfigurationSection
    {
        [ConfigurationProperty("TimeOut", DefaultValue = 1000, IsRequired = false)]
        public int TimeOut
        {
            get
            {
                return (int)base["TimeOut"];
            }
            set
            {
                base["TimeOut"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ThriftServerCollection Servers
        {
            get
            {
                ThriftServerCollection col = (ThriftServerCollection)base[""];
                return col;
            }
        }
    }
}
