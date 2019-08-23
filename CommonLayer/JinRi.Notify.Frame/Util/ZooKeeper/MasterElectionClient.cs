using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZooKeeperNet;

namespace JinRi.Notify.Frame
{
    public class MasterElectionClient
    {
        private ZooKeeper zk;
        private static readonly ILog m_localLog = LoggerSource.Instance.GetLogger(typeof(MasterElectionClient));

        public void Init()
        {
            try
            {
                zk = new ZooKeeper(ZKServerInfo.ZKServer, TimeSpan.FromSeconds(ZKServerInfo.ZKSessionTimeOut), new LogCenterWatcher(this));
                ZooKeeper.WaitUntilConnected(zk);

                if (zk.Exists(ZKServerInfo.ZKLogCenterRootPath, false) != null)
                {
                    zk.GetChildren(ZKServerInfo.ZKLogCenterRootPath, true);
                }
            }
            catch (Exception ex)
            {
                m_localLog.Error("连接Zookeeper服务失败", ex);
            }
        }

        public IServerInfo GetAliveLogServer(string logToServer)
        {
            IServerInfo logServer = ServerManager.Get(logToServer);
            try
            {
                if (zk != null && zk.Exists(ZKServerInfo.ZKLogCenterRootPath, false) != null)
                {
                    IEnumerable<string> aliveServers = zk.GetChildren(ZKServerInfo.ZKLogCenterRootPath, false);
                    if (aliveServers != null && aliveServers.Count() >= 1)
                    {
                        IEnumerable<string> masterServer = aliveServers.Where(t =>
                        {
                            byte[] data = zk.GetData(BuildFullPath(t), true, null);
                            return !"OFF".Equals(Encoding.UTF8.GetString(data).ToUpper());
                        });
                        List<string> tempList = masterServer.Count() > 0 ? masterServer.ToList() : aliveServers.ToList();
                        ShellSort(tempList);
                        string serverIP = tempList.FirstOrDefault();
                        if (!string.IsNullOrEmpty(serverIP))
                        {
                            string[] serverArr = serverIP.Replace("-", ".").Split('_');
                            short port = logServer.Port;
                            short.TryParse(serverArr[1], out port);
                            IServerInfo serverInfo = new ServerInfo("LogCenter_Thrift");
                            serverInfo.Address = serverArr[0];
                            serverInfo.Port = port;
                            serverInfo.Timeout = logServer.Timeout;
                            return serverInfo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_localLog.Error("从Zookeeper获取日志服务器IP失败", ex);
            }
            return logServer;
        }

        private string BuildFullPath(string path)
        {
            return string.Format("{0}/{1}", ZKServerInfo.ZKLogCenterRootPath, path);
        }

        internal void ProcessWatchedEvent(WatchedEvent @event)
        {
            try
            {
                if (@event.Type == EventType.NodeChildrenChanged || @event.Type == EventType.NodeDataChanged
                    || @event.Type == EventType.NodeDeleted)
                {
                    DBLog.SyncLogServerEvent.Reset();
                    DBLog.InitThriftServer();
                    DBLog.SyncLogServerEvent.Set();

                }
                zk.GetChildren(ZKServerInfo.ZKLogCenterRootPath, true);
            }
            catch (Exception ex)
            {
               m_localLog.Error("ProcessWatchedEvent失败", ex);
            }
        }

        private void ShellSort(List<string> nameList)
        {
            int length = nameList.Count;

            int d = length / 2;
            string tmpNodeName;
            string[] tmpNodeNames;
            long tmpSessionId;
            long baseSessionId;
            int j;

            while (d > 0)
            {
                for (int i = d; i < length; i++)
                {
                    tmpNodeName = nameList[i];
                    tmpNodeNames = tmpNodeName.Split('_');
                    tmpSessionId = long.Parse(tmpNodeNames[2]);

                    j = i - d;
                    baseSessionId = long.Parse(nameList[j].Split('_')[2]);
                    while (j >= 0 && tmpSessionId > baseSessionId)
                    {
                        nameList[j + d] = nameList[j];
                        j = j - d;
                        if (j >= 0)
                        {
                            baseSessionId = long.Parse(nameList[j].Split('_')[2]);
                        }
                    }
                    nameList[j + d] = tmpNodeName;
                }
                d = d / 2;
            }
        }
    }

    public class LogCenterWatcher : IWatcher
    {
        private MasterElectionClient _masterElectionClient;
        public LogCenterWatcher(MasterElectionClient masterElectionClient)
        {
            _masterElectionClient = masterElectionClient;
        }
        public void Process(WatchedEvent @event)
        {
            _masterElectionClient.ProcessWatchedEvent(@event);
        }
    }
}
