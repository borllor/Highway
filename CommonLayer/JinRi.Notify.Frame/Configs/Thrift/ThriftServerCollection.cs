using System;
using System.Configuration;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 定义任务处理句柄节点集合
    /// </summary>
    public class ThriftServerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ThriftServerElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ThriftServerElement)element).Address;
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

        public ThriftServerElement this[int index]
        {
            get
            {
                return (ThriftServerElement)BaseGet(index);
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
