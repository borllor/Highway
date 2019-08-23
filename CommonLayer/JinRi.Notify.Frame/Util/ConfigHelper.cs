using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class ConfigHelper
    {
        public static IList<IServerInfo> GetServerInfoList(ServerCollection col)
        {
            IList<IServerInfo> list = null;
            if (col != null && col.Count > 0)
            {
                list = new List<IServerInfo>();
                foreach (ServerElement s in col)
                {
                    IServerInfo s3 = new ServerInfo(s.Code);
                    s3.Protocal = s.Protocal;
                    s3.Address = s.Address;
                    s3.Port = (short)s.Port;
                    s3.PerformanceValue = (short)s.PerformanceValue;
                    s3.CreditValue = (short)s.CreditValue;
                    s3.Timeout = s.Timeout;
                    s3.ServerStatus = (ServerStatus)Enum.Parse(typeof(ServerStatus), s.ServerStatus, false);
                    s3.IsProvideService = s.IsProvideService == 1 ? true : false;
                    s3.IsDistributionServer = s.IsDistribution == 1 ? true : false;
                    list.Add(s3);
                }
            }
            return list;
        }
    }
}
