using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 服务器节点对象
    /// </summary>
    public class ServerElement : ConfigurationElement
    {
        private static bool _displayIt = false;

        public ServerElement()
        {
        }

        public ServerElement(string serverCode)
        {
            Code = serverCode;
        }

        public ServerElement(string serverCode, string address,
            int port, string protocal, int performanceValue,
            int creditValue, string serverStatus, int isDistribution)
        {
            Code = serverCode;
            Address = address;
            Port = port;
            Protocal = protocal;
            PerformanceValue = performanceValue;
            CreditValue = creditValue;
            ServerStatus = serverStatus;
            IsProvideService = 0;
            IsDistribution = isDistribution;
        }

        public ServerElement(string serverCode, string address,
            int port, string protocal, int performanceValue,
            int creditValue, string serverStatus, int isProvideService, int isDistribution)
        {
            Code = serverCode;
            Address = address;
            Port = port;
            Protocal = protocal;
            PerformanceValue = performanceValue;
            CreditValue = creditValue;
            ServerStatus = serverStatus;
            IsProvideService = isProvideService;
            IsDistribution = isDistribution;
        }

        /// <summary>
        /// 服务器唯一标示
        /// </summary>
        [ConfigurationProperty("Code", DefaultValue = "服务器唯一标识", IsRequired = true, IsKey = true)]
        public string Code
        {
            get
            {
                return (string)this["Code"];
            }
            set
            {
                this["Code"] = value;
            }
        }

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        [ConfigurationProperty("Address", DefaultValue = "127.0.0.1", IsRequired = true)]
        [RegexStringValidator(@"^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$")]
        public string Address
        {
            get
            {
                return (string)this["Address"];
            }
            set
            {
                this["Address"] = value;
            }
        }

        /// <summary>
        /// 服务端口号
        /// </summary>
        [ConfigurationProperty("Port", DefaultValue = (int)9090, IsRequired = true)]
        [IntegerValidator(MinValue = 1024, MaxValue = 65535, ExcludeRange = false)]
        public int Port
        {
            get
            {
                return (int)this["Port"];
            }
            set
            {
                this["Port"] = value;
            }
        }

        /// <summary>
        /// 采用协议
        /// </summary>
        [ConfigurationProperty("Protocal", DefaultValue = "Tcp", IsRequired = true)]
        [RegexStringValidator(@"(^Tcp$)|(^Http$)")]
        public string Protocal
        {
            get
            {
                return (string)this["Protocal"];
            }
            set
            {
                this["Protocal"] = value;
            }
        }

        /// <summary>
        /// 服务器性能指数
        /// </summary>
        [ConfigurationProperty("PerformanceValue", DefaultValue = (int)60, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
        public int PerformanceValue
        {
            get
            {
                return (int)this["PerformanceValue"];
            }
            set
            {
                this["PerformanceValue"] = value;
            }
        }

        /// <summary>
        /// 服务器信用指数
        /// </summary>
        [ConfigurationProperty("CreditValue", DefaultValue = (int)60, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
        public int CreditValue
        {
            get
            {
                return (int)this["CreditValue"];
            }
            set
            {
                this["CreditValue"] = value;
            }
        }

        /// <summary>
        /// 服务端口号
        /// </summary>
        [ConfigurationProperty("Timeout", DefaultValue = (int)1000, IsRequired = false)]
        public int Timeout
        {
            get
            {
                return (int)this["Timeout"];
            }
            set
            {
                this["Timeout"] = value;
            }
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        [ConfigurationProperty("ServerStatus", DefaultValue = "Active", IsRequired = true)]
        [RegexStringValidator(@"(^Shutdown$)|(^Active$)|(^Busy$)|(^Easy$)")]
        public string ServerStatus
        {
            get
            {
                return (string)this["ServerStatus"];
            }
            set
            {
                this["ServerStatus"] = value;
            }
        }

        /// <summary>
        /// 是否作为服务器提供服务
        /// </summary>
        [ConfigurationProperty("IsProvideService", DefaultValue = 0, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 1, ExcludeRange = false)]
        public int IsProvideService
        {
            get
            {
                return (int)this["IsProvideService"];
            }
            set
            {
                this["IsProvideService"] = value;
            }
        }
        /// <summary>
        /// 是否是分发服务器
        /// </summary>
        [ConfigurationProperty("IsDistribution", DefaultValue = 0, IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = 1, ExcludeRange = false)]
        public int IsDistribution
        {
            get
            {
                return (int)this["IsDistribution"];
            }
            set
            {
                this["IsDistribution"] = value;
            }
        }

        protected override bool IsModified()
        {
            bool ret = base.IsModified();

            // Enter your custom processing code here.

            Console.WriteLine("UrlConfigElement.IsModified() called.");

            return ret;
        }
    }
}
