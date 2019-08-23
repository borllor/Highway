using System;
using System.Configuration;

namespace JinRi.Framework
{
    /// <summary>
    /// 定义任务处理句柄节点集合
    /// </summary>
    public class TaskHandleCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskHandleElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TaskHandleElement)element).Task;
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

        public TaskHandleElement this[int index]
        {
            get
            {
                return (TaskHandleElement)BaseGet(index);
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
