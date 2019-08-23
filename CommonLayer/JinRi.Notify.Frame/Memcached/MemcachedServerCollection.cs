using System;
using System.Configuration;

namespace JinRi.Framework
{
    /// <summary>
    /// 定义任务处理句柄节点集合
    /// </summary>
    public class MemcachedServerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MemcachedServerElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MemcachedServerElement)element).Address;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "add";
            }
        }

        public MemcachedServerElement this[int index]
        {
            get
            {
                return (MemcachedServerElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }
}
