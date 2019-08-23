using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class ZKServerInfo
    {
        public static string ZKServer
        {
            get
            {
                return ConfigurationManager.AppSettings["ZKServer"];
            }
        }
        public static double ZKSessionTimeOut
        {
            get
            {
                string zkSessionTimeOut = ConfigurationManager.AppSettings["ZKSessionTimeOut"];
                if (string.IsNullOrWhiteSpace(zkSessionTimeOut))
                {
                    return 5;
                }
                return double.Parse(zkSessionTimeOut);
            }
        }
        public static string ZKLogCenterRootPath
        {
            get
            {
                return string.Format("/ConfigService/200001/{0}", ConfigurationManager.AppSettings["ZKLogCenterRootPath"]);
            }
        }
    }
}
