using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;

namespace JinRi.Framework
{
    public class AppSetting
    {
        #region 初始化配置节点

        private static string m_configFilename = null;
        private static bool m_isInitDistributionConfig = false;
        private static Configuration m_config;
        private static readonly object m_configLockObj = new object();
        private static readonly object m_configFilenameLockObj = new object();
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(AppSetting));

        public static Configuration GetConfiguration()
        {
            if (m_config == null)
            {
                lock (m_configLockObj)
                {
                    if (m_config == null)
                    {
                        string configFilename = GetConfigFilename();
                        if (!string.IsNullOrEmpty(configFilename))
                        {
                            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilename };
                            m_config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                        }
                    }
                }
            }
            return m_config;
        }

        public static string GetConfigFilename()
        {
            if (m_configFilename == null)
            {
                lock (m_configFilenameLockObj)
                {
                    if (m_configFilename == null)
                    {
                        m_configFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JinRi.Framework.config");
                        if (!File.Exists(m_configFilename))
                        {
                            m_configFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs\\JinRi.Framework.config");
                        }
                        if (!File.Exists(m_configFilename))
                        {
                            m_configFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\JinRi.Framework.config");
                        }
                        if (!File.Exists(m_configFilename))
                        {
                            m_configFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigFiles\\JinRi.Framework.config");
                        }
                        if (!File.Exists(m_configFilename))
                        {
                            m_configFilename = "";
                            Logger.Fatal("初始化配置文件JinRi.Framework.config时，未找到配置文件", new FileNotFoundException("未找到配置文件"));
                        }
                    }
                }
            }
            return m_configFilename;
        }

        #region 初始化分布式服务配置项

        /// <summary>
        /// 初始化分布式服务的服务器，任务处理器，分配算法器对象
        /// </summary>
        public static string InitDistributionConfig()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                if (m_isInitDistributionConfig)
                {
                    return "已执行";
                }
                m_isInitDistributionConfig = true;
                Configuration config = GetConfiguration();
                //Logger.Debug("配置文件已经读取成功");
                ServersSection serversSection = config.GetSection("Servers") as ServersSection;
                //Logger.Debug("ServersSection初始化成功，取到的Servers数量为:" + serversSection.Servers.Count);
                TaskHandlesSection taskHandlesSection = config.GetSection("TaskHandles") as TaskHandlesSection;
                //Logger.Debug("TaskHandles初始化成功，取到的TaskHandles数量为:" + taskHandlesSection.Handles.Count);
                TaskAllocAlgProvidersSection taskAllocAlgProvidersSection = config.GetSection("TaskAllocAlgProviders") as TaskAllocAlgProvidersSection;
                //Logger.Debug("TaskAllocAlgProvidersSection初始化成功，取到的TaskAllocAlgProviders数量为:" + taskAllocAlgProvidersSection.Providers.Count);

                if (serversSection != null)
                {
                    string tem = InitServer(serversSection.Servers);
                    sb.Append(tem);
                }
                else
                {
                    sb.Append("没有找到任何服务提供商");
                }
                if (taskHandlesSection != null)
                {
                    string tem = InitTaskHandle(taskHandlesSection.Handles);
                    sb.Append(tem);
                }
                else
                {
                    sb.Append("没有找到任何任务处理程序");
                }

                if (taskAllocAlgProvidersSection != null)
                {
                    string tem = InitTaskAllocAlgProvider(taskAllocAlgProvidersSection.Providers, taskAllocAlgProvidersSection.DefaultProvider);
                    sb.Append(tem);
                }
                else
                {
                    sb.Append("没有找到任何任务任务分配算法");
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("读取配置JinRi.Framework.config文件产生严重错误", ex);
            }
            Logger.Debug("读取到的配置信息" + sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 初始化服务器
        /// </summary>
        /// <param name="col"></param>
        private static string InitServer(ServerCollection col)
        {
            StringBuilder sb = new StringBuilder();
            bool hasRegisterLocalServer = false;
            if (col != null && col.Count > 0)
            {
                IList<IServerInfo> serverList = ConfigHelper.GetServerInfoList(col);
                foreach (IServerInfo s in serverList)
                {
                    sb.Append(s);
                    ServerManager.Register(s);
                    //服务器如果是自己才注册通道和服务
                    if (IPHelper.IsLocalIP(s.Address))
                    {
                        if (s.IsProvideService)
                        {
                            ServerManager.RegisterServerChannel(s);
                            ServerManager.RegisterService(s);
                        }
                        else
                        {
                            ServerManager.RegisterClientChannel(s);
                        }
                        hasRegisterLocalServer = true;
                    }
                }
            }
            //如果没有注册本地服务，则在此注册
            if (!hasRegisterLocalServer)
            {
                ServerManager.RegisterClientChannel("tcp");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 初始化任务处理器
        /// </summary>
        /// <param name="col"></param>
        private static string InitTaskHandle(TaskHandleCollection col)
        {
            StringBuilder sb = new StringBuilder();
            if (col != null && col.Count > 0)
            {
                foreach (TaskHandleElement t in col)
                {
                    sb.Append(t);
                    TaskHandleManager.Add(t.Task, t.Handle);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 初始化任务分配算法器
        /// </summary>
        /// <param name="col"></param>
        /// <param name="defaultProvider"></param>
        private static string InitTaskAllocAlgProvider(TaskAllocAlgProviderCollection col, string defaultProvider)
        {
            StringBuilder sb = new StringBuilder();
            if (col != null && col.Count > 0)
            {
                foreach (TaskAllocAlgProviderElement t in col)
                {
                    sb.Append(t);
                    TaskAllocAlgProviderManager.Add(t.Name, t.Type, t.Name.Equals(defaultProvider));
                }
            }
            return sb.ToString();
        }

        #endregion

        #endregion

        #region AppSetting 节点

        /// <summary>
        /// 数据缓冲器大小
        /// </summary>
        public static int DataBufferSize
        {
            get
            {
                string val = GetAppValue("DataBufferSize");
                int ival = 0;
                int.TryParse(val, out ival);
                return ival;
            }
        }

        /// <summary>
        /// 数据库提供程序名称
        /// </summary>
        public static int DataBufferPoolSize
        {
            get
            {
                string val = GetAppValue("DataBufferPoolSize");
                int ival = 0;
                int.TryParse(val, out ival);
                return ival;
            }
        }

        /// <summary>
        /// Telnet端口号
        /// </summary>
        public static int TelnetPort
        {
            get
            {
                string val = GetAppValue("TelnetPort");
                int ival = 0;
                int.TryParse(val, out ival);
                return ival;
            }
        }

        /// <summary>
        /// 数据库链接信息
        /// </summary>
        public static string AppLoggerMSMQ
        {
            get
            {
                return GetAppValue("AppLogger.MSMQ");
            }
        }

        /// <summary>
        /// 日志记录到那个服务器
        /// </summary>
        public static string LogToServer
        {
            get
            {
                return GetAppValue("LogToServer");
            }
        }

        /// <summary>
        /// 数据库链接信息
        /// </summary>
        public static string Log4NetConnectionString
        {
            get
            {
                return GetAppValue("Log4Net");
            }
        }

        /// <summary>
        /// 数据库提供程序名称
        /// </summary>
        public static string ProviderName
        {
            get
            {
                return GetAppValue("ProviderName");
            }
        }

        /// <summary>
        /// 缓存提供程序
        /// </summary>
        public static string DefaultCacheProvider
        {
            get
            {
                string val = GetAppValue("DefaultCacheProvider");
                return string.IsNullOrEmpty(val) ? "JinRi.Framework.WebCacheProvider, JinRi.Framework" : val;
            }
        }

        #endregion

        #region 辅助函数

        /// <summary>
        /// 获取配置项的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAppValue(string name)
        {
            Configuration config = GetConfiguration();
            if (config != null)
            {
                KeyValueConfigurationElement elm = config.AppSettings.Settings[name];
                if (elm != null)
                {
                    return elm.Value;
                }
            }
            return "";
        }

        #endregion
    }
}
