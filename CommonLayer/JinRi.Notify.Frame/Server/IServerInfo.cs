using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

namespace JinRi.Notify.Frame
{
    public interface IServerInfo
    {
        string Address { get; set; }
        short CreditValue { get; set; }
        List<string> DistributionServerList { get; set; }
        bool IsProvideService { get; set; }
        bool IsDistributionServer { get; set; }
        short PerformanceValue { get; set; }
        short Port { get; set; }
        string Protocal { get; set; }
        string Remark { get; set; }
        string ServerCode { get; set; }
        int Timeout { get; set; }
        object Channel { get; set; }
        ServerStatus ServerStatus { get; set; }
        BeginHandler TaskBeginHandler { get; set; }
        TaskHandler TaskHandler { get; set; }
        EndHandler TaskEndHandler { get; set; }
    }
}
