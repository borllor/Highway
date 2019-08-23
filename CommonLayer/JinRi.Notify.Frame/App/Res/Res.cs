using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace JinRi.Notify.Frame
{
    public static class Res
    {
        private static Hashtable m_res = null;
        private static readonly ILog m_logger;

        static Res()
        {
            //m_logger = LoggerSource.Instance.GetLogger(typeof(Res));
            m_res = new Hashtable();
            InitRes();
        }

        private static void InitRes()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res\\");
            if (Directory.Exists(dir))
            {
                string[] xmlFileArr = Directory.GetFiles(dir, "*.xml");
                foreach (string s in xmlFileArr)
                {
                    ReadXmlFile(s);
                }
            }
            else
            {
                //m_logger.Error(string.Format("未找到资源文件夹{0}", dir));
            }
        }

        private static void ReadXmlFile(string xmlPath)
        {
            try
            {
                using (var reader = new XmlTextReader(xmlPath))
                {
                    bool exit = false;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.LocalName.Equals("Message", StringComparison.OrdinalIgnoreCase))
                                {
                                    m_res[reader.GetAttribute("Name")] = reader.GetAttribute("Value");
                                }
                                break;
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                //m_logger.Error(string.Format("xml{0}文件未找到", xmlPath), ex);
            }
            catch (Exception ex)
            {
                //m_logger.Error(ex);
            }
        }

        public static string String(object name, params object[] args)
        {
            try
            {
                string key = string.Format("{0}", name);
                string s = string.Format("{0}", m_res[key]);
                return s == null ? "" : string.Format(s, args);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
