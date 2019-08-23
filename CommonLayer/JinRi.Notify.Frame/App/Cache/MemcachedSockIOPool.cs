using System;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using System.Collections;
using JinRi.Notify.Frame;
using System.Configuration;

namespace JinRi.Notify.Frame
{
    internal class MemcachedSockIOPool
    {
        private SockIOPool m_pool;
        private readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MemcachedSockIOPool));
        private static readonly object m_lockObj = new object();

        public MemcachedSockIOPool()
        {
            Setup();
        }

        private void Setup()
        {
            if (m_pool == null)
            {
                lock (m_lockObj)
                {
                    if (m_pool == null)
                    {
                        Configuration config = AppSetting.GetConfiguration();
                        MemcachedSection memcachedSection = config.GetSection("Memcached") as MemcachedSection;
                        if (memcachedSection != null && memcachedSection.Servers != null && memcachedSection.Servers.Count > 0)
                        {
                            string[] servers = new string[memcachedSection.Servers.Count];
                            for (int i = 0; i < memcachedSection.Servers.Count; i++)
                            {
                                MemcachedServerElement elm = memcachedSection.Servers[i];
                                servers[i] = string.Format("{0}:{1}", elm.Address, elm.Port);
                            }
                            //分布Memcached服务IP 端口
                            SockIOPool pool = SockIOPool.GetInstance();
                            try
                            {
                                pool.SetServers(servers);
                                pool.InitConnections = memcachedSection.InitConnections;
                                pool.MinConnections = memcachedSection.MinConnections;
                                pool.MaxConnections = memcachedSection.MaxConnections;
                                pool.SocketConnectTimeout = memcachedSection.SocketConnectTimeout;
                                pool.SocketTimeout = memcachedSection.SocketTimeout;
                                pool.MaintenanceSleep = memcachedSection.MaintenanceSleep;
                                pool.Failover = memcachedSection.Failover > 0;
                                pool.Nagle = memcachedSection.Nagle > 0;
                                pool.Initialize();
                                m_pool = pool;
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("初始化Memcached的SockIOPool失败", ex);
                            }
                        }
                    }
                }
            }
        }

        ~MemcachedSockIOPool()
        {
            if (m_pool != null)
            {
                m_pool.Shutdown();
            }
        }
    }
}
