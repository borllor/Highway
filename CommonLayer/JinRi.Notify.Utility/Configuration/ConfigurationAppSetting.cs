using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Utility
{
    public static class ConfigurationAppSetting
    {
        /// <summary>
        /// 机器编号
        /// </summary>
        private static string _machineId;
        public static string MachineId
        {
            get
            {
                if (string.IsNullOrEmpty(_machineId))
                {
                    _machineId = ConfigurationManager.AppSettings["MachineId"];
                }
                return _machineId;
            }
        }

        /// <summary>
        /// 应用程序ID
        /// </summary>
        private static string _appId;
        public static string AppId
        {
            get
            {
                if (string.IsNullOrEmpty(_appId))
                {
                    _appId = ConfigurationManager.AppSettings["AppId"];
                }
                return _appId;
            }
        }

        /// <summary>
        /// RabbitMQL连接配置
        /// </summary>
        private static string _rabbitMQHost;
        public static string RabbitMQHost
        {
            get
            {
                if (string.IsNullOrEmpty(_rabbitMQHost))
                {
                    _rabbitMQHost = ConfigurationManager.AppSettings["RabbitMQHost"];
                }
                return _rabbitMQHost;
            }
        }

        /// <summary>
        /// 获取生成中心的配置信息
        /// </summary>
        private static string _builderServiceSetting;
        public static string BuilderServiceSetting
        {
            get
            {
                if (string.IsNullOrEmpty(_builderServiceSetting))
                {
                    _builderServiceSetting = ConfigurationManager.AppSettings["BuilderServiceSetting"];
                }
                return _builderServiceSetting;
            }
        }

        /// <summary>
        /// 重扫服务的配置信息
        /// </summary>
        private static string _redoServiceSetting;
        public static string RedoServiceSetting
        {
            get
            {
                if (string.IsNullOrEmpty(_redoServiceSetting))
                {
                    _redoServiceSetting = ConfigurationManager.AppSettings["RedoServiceSetting"];
                }
                return _redoServiceSetting;
            }
        }

        /// <summary>
        /// 消息发送中心配置信息
        /// </summary>
        private static string _sendServiceSetting;
        public static string SendServiceSetting
        {
            get
            {
                if (string.IsNullOrEmpty(_sendServiceSetting))
                {
                    _sendServiceSetting = ConfigurationManager.AppSettings["SendServiceSetting"];
                }
                return _sendServiceSetting;
            }
        }

        /// <summary>
        /// 消息发送中心配置信息
        /// </summary>
        private static string _receiveServiceSetting;
        public static string ReceiveServiceSetting
        {
            get
            {
                if (string.IsNullOrEmpty(_receiveServiceSetting))
                {
                    _receiveServiceSetting = ConfigurationManager.AppSettings["ReceiveServiceSetting"];
                }
                return _receiveServiceSetting;
            }
        }

        /// <summary>
        /// 需要记录的日志级别
        /// </summary>
        private static string _logSetting;
        public static string LogSetting
        {
            get
            {
                if (string.IsNullOrEmpty(_logSetting))
                {
                    _logSetting = ConfigurationManager.AppSettings["LogSetting"];
                }
                return _logSetting;
            }
        }

        private static string _scanServiceSetting;
        public static string ScanServiceSetting
        {
            get
            {
                if (string.IsNullOrEmpty(_scanServiceSetting))
                {
                    _scanServiceSetting = ConfigurationManager.AppSettings["ScanServiceSetting"];
                }
                return _scanServiceSetting;
            }
        }
    }
}
