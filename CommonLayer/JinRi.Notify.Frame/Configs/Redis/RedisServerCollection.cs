using System;
using System.Configuration;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 定义任务处理句柄节点集合
    /// </summary>
    public class RedisServerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RedisServerElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RedisServerElement)element).Address;
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

        public RedisServerElement this[int index]
        {
            get
            {
                return (RedisServerElement)BaseGet(index);
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
