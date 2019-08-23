using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class ServerInfo : IServerInfo
    {
        #region 字段

        protected string _serverCode;
        protected bool _isDistributionServer;
        protected bool _isProvideService;
        protected string _address;
        protected short _port;
        protected string _protocal;
        protected short _performanceValue;
        protected short _creditValue;
        protected int _timeout;
        protected string _remark;
        protected object _channel;
        protected ServerStatus _serverStatus;
        protected List<string> _distributionServerList;
        protected BeginHandler _taskBeginHandler;
        protected TaskHandler _taskHandler;
        protected EndHandler _taskEndHandler;

        #endregion

        #region 属性

        /// <summary>
        /// 服务器编号
        /// </summary>
        public string ServerCode
        {
            get { return _serverCode; }
            set { _serverCode = value; }
        }

        /// <summary>
        /// 是否分发服务器
        /// </summary>
        public bool IsDistributionServer
        {
            get { return _isDistributionServer; }
            set { _isDistributionServer = value; }
        }

        /// <summary>
        /// 是否作为服务器提供服务
        /// </summary>
        public bool IsProvideService
        {
            get { return _isProvideService; }
            set { _isProvideService = value; }
        }

        /// <summary>
        /// 服务地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// 服务端口
        /// </summary>
        public short Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// 使用协议
        /// </summary>
        public string Protocal
        {
            get { return _protocal; }
            set { _protocal = value; }
        }

        /// <summary>
        /// 性能指数(0-100)
        /// </summary>
        public short PerformanceValue
        {
            get { return _performanceValue; }
            set { _performanceValue = value; }
        }

        /// <summary>
        /// 信用指数(0-100)
        /// </summary>
        public short CreditValue
        {
            get { return _creditValue; }
            set { _creditValue = value; }
        }

        /// <summary>
        /// 服务器超时时间
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// 服务器关联的远程对象
        /// </summary>
        public object Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        public ServerStatus ServerStatus
        {
            get { return _serverStatus; }
            set { _serverStatus = value; }
        }

        /// <summary>
        /// 分发服务器列表
        /// </summary>
        public List<string> DistributionServerList
        {
            get { return _distributionServerList; }
            set { _distributionServerList = value; }
        }

        /// <summary>
        /// 处理任务之前，执行服务器端处理
        /// </summary>
        public BeginHandler TaskBeginHandler
        {
            get { return _taskBeginHandler; }
            set { _taskBeginHandler = value; }
        }

        /// <summary>
        /// 服务端自定义处理逻辑
        /// </summary>
        public TaskHandler TaskHandler
        {
            get { return _taskHandler; }
            set { _taskHandler = value; }
        }

        /// <summary>
        /// 处理任务结束，执行服务器端处理
        /// </summary>
        public EndHandler TaskEndHandler
        {
            get { return _taskEndHandler; }
            set { _taskEndHandler = value; }
        }

        #endregion

        #region 构造函数

        public ServerInfo(string serverCode)
        {
            _serverCode = serverCode;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("ServerCode:{0},Address:{1}\r\n", ServerCode, Address);
        }
    }
}
