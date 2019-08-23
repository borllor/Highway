using System.Xml;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;

namespace JinRi.Notify.Frame
{
    public class XmlHelper
    {
        /// <summary>
        /// 获取节点属性值
        /// </summary>
        /// <param name="xmlNode">XML节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns></returns>
        public static string GetAttributeValue(XmlNode xmlNode, string attributeName)
        {
            return GetAttributeValue(xmlNode, attributeName, false);
        }

        /// <summary>
        /// 获取节点属性值
        /// </summary>
        /// <param name="xmlNode">XML节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="toUpper">是否转换为大写(默认不做任何转换)</param>
        /// <returns></returns>
        public static string GetAttributeValue(XmlNode xmlNode, string attributeName, bool toUpper)
        {
            string tmpAttributeValue = string.Empty;
            if (xmlNode.Attributes[attributeName] != null)
            {
                tmpAttributeValue = xmlNode.Attributes[attributeName].Value.Trim();
            }
            if (string.IsNullOrEmpty(tmpAttributeValue))
            {
                foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                {
                    if (string.Compare(xmlAttribute.Name, attributeName, true) == 0)
                    {
                        tmpAttributeValue = xmlAttribute.Value.Trim();
                        break;
                    }
                }
            }
            if (toUpper && !string.IsNullOrEmpty(tmpAttributeValue))
            {
                tmpAttributeValue = tmpAttributeValue.ToUpper();
            }
            return tmpAttributeValue;
        }

        #region 解析XML文档

        /// <summary>
        /// 解析XML文档，不支持属性
        /// </summary>
        /// <param name="xmlContent">XML文档</param>
        /// <returns>字典</returns>
        public static NameObjectCollection ParseXML(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            NameObjectCollection col = new NameObjectCollection();
            foreach (XmlNode n in xmlDoc.ChildNodes)
            {
                col.Add(n.Name, ParseXML(n));
            }
            return col;
        }

        /// <summary>
        /// 解析XML文档，不支持属性
        /// </summary>
        /// <param name="xmlContent">XML文档</param>
        /// <param name="rootNodeName">要解析的节点名称</param>
        /// <returns>字典</returns>
        public static NameObjectCollection ParseXML(string xmlContent, string rootNodeName)
        {
            NameObjectCollection col = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);
                XmlNode node = xmlDoc.SelectSingleNode(rootNodeName);
                col = new NameObjectCollection();
                col.Add(rootNodeName, ParseXML(node));
            }
            catch
            {
            }
            return col;
        }

        private static NameObjectCollection ParseXML(XmlNode node)
        {
            NameObjectCollection col = null;
            if (node != null)
            {
                col = new NameObjectCollection();
                if (node.HasChildNodes)
                {
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        bool hasAttr = false;
                        if (n.Attributes != null && n.Attributes.Count > 0)
                        {
                            hasAttr = true;
                            NameObjectCollection attrCol = new NameObjectCollection();
                            foreach (XmlAttribute a in n.Attributes)
                            {
                                attrCol.Add(a.Name, a.InnerText);
                            }
                            col.Add(n.Name, attrCol);
                        }
                        if (n.NodeType == XmlNodeType.Element && n.ChildNodes.Count == 0)
                        {
                            if (!hasAttr || !string.IsNullOrEmpty(n.InnerText))
                            {
                                col.Add(n.Name, string.Format("{0}", n.InnerText));
                            }
                        }
                        else if (n.FirstChild == n.LastChild &&
                            (n.FirstChild.NodeType == XmlNodeType.Text ||
                            n.FirstChild.NodeType == XmlNodeType.CDATA))
                        {
                            col.Add(n.Name, n.InnerText);
                        }
                        else
                        {
                            col.Add(n.Name, ParseXML(n));
                        }
                    }
                }
                else
                {
                    col.Add(node.Name, node.InnerText);
                }
            }
            return col;
        }

        #endregion

        #region 写XML文档

        /// <summary>
        /// 根据字典些XML文档，不支持属性
        /// </summary>
        /// <param name="xmlCol">XML数据字典</param>
        /// <returns>XML文档</returns>
        public static string WriteXML(NameObjectCollection xmlCol)
        {
            return WriteXML(xmlCol, "gb2312");
        }

        /// <summary>
        /// 根据字典些XML文档，不支持属性
        /// </summary>
        /// <param name="xmlCol">XML数据字典</param>
        /// <param name="encodingName">文档采用的编码</param>
        /// <returns>XML文档</returns>
        public static string WriteXML(NameObjectCollection xmlCol, string encodingName)
        {
            List<string> xmlList = new List<string>();
            xmlList.Add(string.Format("<?xml version=\"1.0\" encoding=\"{0}\" ?>", encodingName));
            if (xmlCol != null && xmlCol.Count > 0)
            {
                WriteXML(xmlCol, ref xmlList);
            }
            return string.Join(Environment.NewLine, xmlList.ToArray());
        }

        private static void WriteXML(NameObjectCollection xmlCol, ref List<string> xmlList)
        {
            if (xmlCol != null && xmlCol.Count > 0)
            {
                int count = xmlCol.Count;
                for (int i = 0; i < count; i++)
                {
                    string key = xmlCol.GetKey(i);
                    object[] values = xmlCol.GetValues(i);
                    if (values != null)
                    {
                        for (int j = 0; j < values.Length; j++)
                        {
                            object obj = values[j];
                            if (obj != null)
                            {
                                if (obj is NameObjectCollection)
                                {
                                    xmlList.Add(string.Format("<{0}>", key));
                                    WriteXML((NameObjectCollection)obj, ref xmlList);
                                    xmlList.Add(string.Format("</{0}>", key));
                                }
                                else
                                {
                                    xmlList.Add(string.Format("<{0}>{1}</{0}>", key, obj));
                                }
                            }
                            else
                            {
                                xmlList.Add(string.Format("<{0} />", key));
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
