using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Framework
{
    /// <summary>
    /// 任务分配算法节点对象
    /// </summary>
    public class TaskAllocAlgProviderElement : ConfigurationElement
    {
        private static bool _displayIt = false;

        public TaskAllocAlgProviderElement()
        {
        }

        public TaskAllocAlgProviderElement(string name)
        {
            Name = name;
        }

        public TaskAllocAlgProviderElement(string name, string type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// 服务器唯一标示
        /// </summary>
        [ConfigurationProperty("Name", DefaultValue = "分配算法提供程序名称", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        [ConfigurationProperty("Type", DefaultValue = "*.*.*.*", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["Type"];
            }
            set
            {
                this["Type"] = value;
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
            return string.Format("Name:{0},Type:{1}", Name, Type);
        }
    }
}
