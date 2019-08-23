using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Framework
{
    /// <summary>
    /// 任务分配算法节点配置域
    /// </summary>
    public class TaskAllocAlgProvidersSection : ConfigurationSection
    {
        [ConfigurationProperty("DefaultProvider", IsRequired = true)]
        public string DefaultProvider
        {
            get
            {
                return (string)base["DefaultProvider"];
            }
            set
            {
                base["DefaultProvider"] = value;
            }
        }


        [ConfigurationProperty("", IsDefaultCollection = true)]
        public TaskAllocAlgProviderCollection Providers
        {
            get
            {
                TaskAllocAlgProviderCollection col = (TaskAllocAlgProviderCollection)base[""];
                return col;
            }
        }
    }
}
