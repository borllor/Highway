using System;
using System.Configuration;

namespace JinRi.Framework
{
    /// <summary>
    /// 定义任务分配算法节点集合
    /// </summary>
    public class TaskAllocAlgProviderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskAllocAlgProviderElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TaskAllocAlgProviderElement)element).Name;
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

        public TaskAllocAlgProviderElement this[int index]
        {
            get
            {
                return (TaskAllocAlgProviderElement)BaseGet(index);
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
