using System;
using System.Web;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Utility
{
    /// <summary>
    /// 异常信息扩展类
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// 序列化异常实例ex
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetString(this Exception ex)
        {
            try
            {
                if (ex == null)
                {
                    return "";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("ex.Message:").Append(ex.Message).Append("|");
                if (!string.IsNullOrEmpty(ex.Source))
                {
                    sb.Append("ex.Source:").Append((ex.Source ?? "")).Append("|");
                }
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    sb.Append("ex.StackTrace:").Append((ex.StackTrace ?? "")).Append("|");
                }
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.ToString()))
                {
                    sb.Append(GetString(ex.InnerException));
                }
                sb.Append(ex);
                return sb.ToString();
            }
            catch (Exception e)
            {
                return "二次异常：" + e + "|" + (ex == null ? "null" : ex.ToString());
            }
        }

    }
}
