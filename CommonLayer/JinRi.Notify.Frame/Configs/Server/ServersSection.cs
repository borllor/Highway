using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 服务器节点配置域
    /// </summary>
    public class ServersSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ServerCollection Servers
        {
            get
            {
                ServerCollection col = (ServerCollection)base[""];
                return col;
            }
        }
    }
}
