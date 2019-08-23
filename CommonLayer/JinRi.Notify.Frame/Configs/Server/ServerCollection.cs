using System;
using System.Configuration;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 定义Server节点集合
    /// </summary>
    public class ServerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerElement)element).Code;
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

        public ServerElement this[int index]
        {
            get
            {
                return (ServerElement)BaseGet(index);
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
