using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Frame
{
    public class IPHelper
    {
        /// <summary>
        /// 验证IP地址是否是本地IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsLocalIP(string ip)
        {
            bool isLocal = false;
            List<string> list = GetLocalIPList();
            if (list != null && list.Count > 0)
            {
                isLocal = list.Contains(ip);
            }
            return isLocal;
        }

        /// <summary>
        /// 把IP地址转换为长整数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public static long IPToLong(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        /// <summary>
        /// 长整形数值，转IP地址格式
        /// </summary>
        /// <param name="ipLong">长整数IP数值</param>
        /// <returns></returns>
        public static string LongToIP(long ipLong)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipLong >> 24) & 0xFF).Append(".");
            sb.Append((ipLong >> 16) & 0xFF).Append(".");
            sb.Append((ipLong >> 8) & 0xFF).Append(".");
            sb.Append(ipLong & 0xFF);
            return sb.ToString();
        }

        /// <summary>
        /// 获取本机IP地址(多个默认取第一个)
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            List<string> list = GetLocalIPList();
            if (list != null && list.Count > 0) return list[0];
            return string.Empty;
        }

        /// <summary>
        /// 获取本机IP地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIPList()
        {
            List<string> list = new List<string>();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in ipHostEntry.AddressList)
            {

                if (IsIP(ip.ToString())) list.Add(ip.ToString());
            }
            return list;
        }

        public static string GetAddress(string Host, AddressType AddressFormat)
        {
            string IPAddress = string.Empty;
            AddressFamily addrFamily = AddressFamily.InterNetwork;
            switch (AddressFormat)
            {
                case AddressType.IPv4:
                    addrFamily = AddressFamily.InterNetwork;
                    break;
                case AddressType.IPv6:
                    addrFamily = AddressFamily.InterNetworkV6;
                    break;
            }
            IPHostEntry IPE = Dns.GetHostEntry(Host);
            if (Host != IPE.HostName)
            {
                IPE = Dns.GetHostEntry(IPE.HostName);
            }
            foreach (IPAddress IPA in IPE.AddressList)
            {
                if (IPA.AddressFamily == addrFamily)
                {
                    return IPA.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }

    public enum AddressType
    {
        IPv4,
        IPv6
    }
}
