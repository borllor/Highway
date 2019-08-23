using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;

namespace JinRi.Notify.Frame
{
    public class ServerManager
    {
        private static object m_serverObj = new object();
        private static Dictionary<string, IServerInfo> m_serverDic = new Dictionary<string, IServerInfo>();

        public static IServerInfo Register(IServerInfo server)
        {
            if (server != null)
            {
                lock (m_serverObj)
                {
                    if (!m_serverDic.ContainsKey(server.ServerCode))
                    {
                        m_serverDic.Add(server.ServerCode, server);
                    }
                }
            }
            return server;
        }

        public static IServerInfo Unregister(IServerInfo server)
        {
            if (server != null)
            {
                lock (m_serverObj)
                {
                    if (!m_serverDic.ContainsKey(server.ServerCode))
                    {
                        m_serverDic.Add(server.ServerCode, server);
                    }
                }
            }
            return server;
        }

        /// <summary>
        /// 获取除给定服务器的枚举器
        /// </summary>
        /// <returns></returns>
        public static IEnumerator GetServerEnumerator(string serverCode)
        {
            List<IServerInfo> list = new List<IServerInfo>();
            lock (m_serverObj)
            {
                foreach (KeyValuePair<string, IServerInfo> kv in m_serverDic)
                {
                    if (string.Compare(serverCode, kv.Key, true) != 0)
                    {
                        list.Add(kv.Value);
                    }
                }
            }
            return list.GetEnumerator();
        }

        public static IEnumerator GetServerEnumerator()
        {
            lock (m_serverObj)
            {
                return m_serverDic.Values.GetEnumerator();
            }
        }

        public static IServerInfo Get(IServerInfo serverInfo)
        {
            if (serverInfo != null)
            {
                return Get(serverInfo.ServerCode);
            }
            return null;
        }

        public static IServerInfo Get(string serverCode)
        {
            IServerInfo serverInfo = null;
            if (serverCode != null)
            {
                lock (m_serverObj)
                {
                    m_serverDic.TryGetValue(serverCode, out serverInfo);
                }
            }
            return serverInfo;
        }

        public static IServerInfo GetLocalServer()
        {
            return GetByIP(IPHelper.GetLocalIPList());
        }

        public static IServerInfo GetByIP(List<string> ipList)
        {
            if (ipList != null && ipList.Count > 0)
            {
                lock (m_serverObj)
                {
                    foreach (KeyValuePair<string, IServerInfo> kv in m_serverDic)
                    {
                        foreach (string ip in ipList)
                        {
                            if (kv.Value.Address == ip)
                            {
                                return kv.Value;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static IServerInfo GetByIP(string ip)
        {
            if (ip != null)
            {
                lock (m_serverObj)
                {
                    foreach (KeyValuePair<string, IServerInfo> kv in m_serverDic)
                    {
                        if (kv.Value.Address == ip)
                        {
                            return kv.Value;
                        }
                    }
                }
            }
            return null;
        }

        #region 注册通道

        /// <summary>
        /// 注册服务端通道
        /// </summary>
        /// <param name="server"></param>
        public static void RegisterServerChannel(IServerInfo server)
        {
            IChannel channel = GetChannel(server);
            ChannelServices.RegisterChannel(channel, false);
            server.Channel = channel;

            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            RemotingConfiguration.CustomErrorsEnabled(false);
        }

        /// <summary>
        /// 注册客户端通道
        /// </summary>
        /// <param name="server"></param>
        public static void RegisterClientChannel(IServerInfo server)
        {
            RegisterClientChannel(server.Protocal);
        }

        /// <summary>
        /// 注册客户端通道
        /// </summary>
        /// <param name="server"></param>
        public static void RegisterClientChannel(string protocal)
        {
            IChannel channel = null;
            if ("tcp".Equals(protocal, StringComparison.OrdinalIgnoreCase))
            {
                channel = new TcpChannel();
            }
            else if ("http".Equals(protocal, StringComparison.OrdinalIgnoreCase))
            {
                channel = new HttpChannel();
            }
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            RemotingConfiguration.CustomErrorsEnabled(false);
        }

        /// <summary>
        /// 注销通道
        /// </summary>
        /// <param name="server"></param>
        public static void UnregisterChannel(IServerInfo server)
        {
            if (server.Channel != null && server.Channel is IChannel)
            {
                IChannel channel = (IChannel)server.Channel;
                ChannelServices.UnregisterChannel(channel);
            }
        }

        public static IChannel GetChannel(IServerInfo server)
        {
            IDictionary prop = new Hashtable();
            prop["name"] = server.ServerCode;
            prop["port"] = server.Port;
            prop["timeout"] = server.Timeout;
            IChannel channel = null;
            if ("tcp".Equals(server.Protocal, StringComparison.OrdinalIgnoreCase))
            {
                channel = new TcpChannel(prop, new BinaryClientFormatterSinkProvider(), new BinaryServerFormatterSinkProvider());
            }
            else if ("http".Equals(server.Protocal, StringComparison.OrdinalIgnoreCase))
            {
                channel = new HttpChannel(prop, new BinaryClientFormatterSinkProvider(), new BinaryServerFormatterSinkProvider());
            }
            return channel;
        }

        #endregion

        #region 注册服务

        /// <summary>
        /// 注册通道的服务
        /// </summary>
        /// <param name="server"></param>
        public static void RegisterService(IServerInfo server)
        {
            TaskRequest request = new TaskRequest();
            JinRi.Notify.Frame.RegisterService.TaskRequestService.Register(request, server.ServerCode);
        }

        /// <summary>
        /// 注销通道的服务
        /// </summary>
        /// <param name="server"></param>
        public static void UnregisterService(IServerInfo server)
        {
            JinRi.Notify.Frame.RegisterService.TaskRequestService.Unregister(server.ServerCode);
        }

        #endregion
    }
}
