#if NET40
using System;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using System.Collections;
using JinRi.Notify.Frame;
using System.Configuration;
using ServiceStack.Redis;

namespace JinRi.Notify.Frame
{
    internal class RedisSockIOPool
    {
        private PooledRedisClientManager m_redisManager;
        private readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(RedisSockIOPool));
        private static readonly object m_lockObj = new object();

        public RedisSockIOPool()
        {
            Setup();
        }

        public IRedisClient GetClient()
        {
            return m_redisManager.GetClient();
        }

        private void Setup()
        {
            if (m_redisManager == null)
            {
                lock (m_lockObj)
                {
                    if (m_redisManager == null)
                    {
                        Configuration config = AppSetting.GetConfiguration();
                        RedisSection redisSection = config.GetSection("Redis") as RedisSection;
                        if (redisSection != null && redisSection.Servers != null && redisSection.Servers.Count > 0)
                        {
                            List<string> readWriteHosts = new List<string>();
                            List<string> readOnlyHosts = new List<string>();
                            for (int i = 0; i < redisSection.Servers.Count; i++)
                            {
                                RedisServerElement elm = redisSection.Servers[i];
                                string address = string.Format("{0}:{1}", elm.Address, elm.Port);
                                //1表示只写，2表示只读，4表示读写
                                if (elm.Access == 1 || elm.Access == 4)
                                {
                                    readWriteHosts.Add(address);
                                }
                                if (elm.Access == 2 || elm.Access == 4)
                                {
                                    readOnlyHosts.Add(address);
                                }
                            }

                            RedisClientManagerConfig redisConfig = new RedisClientManagerConfig();
                            redisConfig.AutoStart = redisSection.AutoStart;
                            redisConfig.MaxReadPoolSize = redisSection.MaxReadPoolSize;
                            redisConfig.MaxWritePoolSize = redisSection.MaxWritePoolSize;
                            if (redisSection.DefaultDb > 0)
                            {
                                redisConfig.DefaultDb = redisSection.DefaultDb;
                            }

                            m_redisManager = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, redisConfig);
                        }
                    }
                }
            }
        }

        //~RedisSockIOPool()
        //{
        //    if (m_client != null)
        //    {
        //        m_client = null;
        //    }
        //}
    }
}
#endif