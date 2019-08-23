using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Web;

namespace JinRi.Notify.Frame
{
    public class ClientHelper
    {
        /// <summary>
        /// 根据 User Agent 获取操作系统名称
        /// </summary>
        public static string GetCilentOSByUserAgent()
        {
            string osVersion = "未知";
            if (HttpContext.Current.Request == null) return osVersion;
            string userAgent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            if (userAgent == null) return osVersion;
            if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                osVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            return osVersion;
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string returnResult = string.Empty;
            try
            {
                if (HttpContext.Current.Request != null)
                {
                    returnResult = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (null == returnResult || returnResult == string.Empty)
                    {
                        returnResult = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (null == returnResult || returnResult == string.Empty)
                    {
                        returnResult = HttpContext.Current.Request.UserHostAddress;
                    }
                    if (null == returnResult || returnResult == string.Empty || !RegexHelper.IsValidIP(returnResult))
                    {
                        return "0.0.0.0";
                    }
                }
                else
                {
                    returnResult = "0.0.0.0";
                }
            }
            catch
            {
                returnResult = "0.0.0.0";
            }
            return returnResult;
        }

        /// <summary>
        /// 获取MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientMAC()
        {
            string returnResult = "";
            try
            {
                ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();

                foreach (ManagementObject managementObject in managementObjectCollection)
                {
                    if ((bool)managementObject["IPEnabled"] == true)
                    {
                        returnResult = managementObject["MacAddress"].ToString();
                        break;
                    }
                }
                if (System.Web.HttpContext.Current.Application["MacAddress"] == null)
                {
                    System.Web.HttpContext.Current.Application.Lock();
                    System.Web.HttpContext.Current.Application["MacAddress"] = returnResult;
                    System.Web.HttpContext.Current.Application.UnLock();
                }
            }
            catch (System.Exception ex)
            {
                returnResult = "";
            }
            return returnResult;
        }

        /// <summary>
        /// 获取主机头
        /// </summary>
        /// <returns></returns>
        public static string GetClientHost()
        {
            return HttpContext.Current.Request != null ?
                System.Web.HttpContext.Current.Request.Url.Host.ToString() : "";
        }

        /// <summary>  
        /// 获取浏览器版本号  
        /// </summary>  
        /// <returns></returns>  
        public static string GetBrowser()
        {
            if (System.Web.HttpContext.Current.Request != null)
            {
                HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                return bc.Browser + bc.Version;
            }
            else
            {
                return "";
            }
        }
    }
}
