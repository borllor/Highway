using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace JinRi.Notify.Frame
{
    public class RegisterService
    {
        private static object m_taskRequestObj = new object();
        private static int DefaultHeartbeatPeriod = 30000;
        private static Dictionary<string, string> m_taskRequestDic = new Dictionary<string, string>();
        private static readonly ILog m_log = LoggerSource.Instance.GetLogger(typeof(RegisterService));

        public class TaskRequestService
        {
            public static readonly string serviceName = "TaskRequest";
            public static void Register(TaskRequest taskRequest, string serverCode)
            {
                lock (m_taskRequestObj)
                {
                    if (!m_taskRequestDic.ContainsKey(serverCode))
                    {
                        m_taskRequestDic.Add(serverCode, serviceName);
                        RemotingServices.Marshal(taskRequest, serverCode + "." + serviceName);
                    }
                }
            }

            public static void Unregister(string serverCode)
            {
                lock (m_taskRequestObj)
                {
                    if (m_taskRequestDic.ContainsKey(serverCode))
                    {
                        TaskRequest handle = GetService(serverCode) as TaskRequest;
                        if (handle != null)
                        {
                            RemotingServices.Unmarshal(handle.CreateObjRef(handle.GetType()));
                        }
                        m_taskRequestDic.Remove(serverCode);
                    }
                }
            }

            public static ITaskRequest GetService(string serverCode)
            {
                ITaskRequest request = null;
                IServerInfo serverInfo = ServerManager.Get(serverCode);
                if (serverInfo != null)
                {
                    request = (ITaskRequest)Activator.GetObject(typeof(TaskRequest),
                        string.Format("{0}://{1}:{2}/{3}.{4}",
                            serverInfo.Protocal, serverInfo.Address, serverInfo.Port, serverInfo.ServerCode, serviceName));
                }
                else
                {
                    throw new Exception("服务未注册");
                }
                return request;
            }
        }

        public static Timer StartHeartBeat(string serverCode, ITaskRequest request)
        {
            return StartHeartBeat(serverCode, request, DefaultHeartbeatPeriod);
        }

        public static Timer StartHeartBeat(string serverCode, ITaskRequest request, int heartbeatPeriod)
        {
            //Timer timer = new Timer(o =>
            //{
            //    object[] arr = (object[])o;
            //    string code = (string)arr[0];
            //    try
            //    {
            //        ITaskRequest req = (ITaskRequest)arr[1];
            //        HeartBeatTaskInfo beatInfo = new HeartBeatTaskInfo()
            //        {
            //            TaskName = "HeartBeatTaskHandle",
            //            TaskCreateTime = DateTime.Now,
            //            EmitServerCode = code
            //        };
            //        ITaskInfo result = req.SendRequest(code, beatInfo);
            //        if (result.TaskStatus != TaskStatus.Success)
            //        {
            //            string msg = result.TaskStatus == TaskStatus.Success ? "成功" : "失败：" + result.TaskRemark;
            //            m_log.Info(string.Format("{0}(LocalIP:{1})于{2}，执行心跳检测结果，{3}", code, IPHelper.GetLocalIP(), DateTime.Now, msg));
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        m_log.Error(string.Format("{0}(LocalIP:{1})于{2}，执行心跳检测异常：{3}", code, IPHelper.GetLocalIP(), DateTime.Now, ex.Message));
            //    }
            //}, new object[] { serverCode, request }, heartbeatPeriod, heartbeatPeriod);
            
            //return timer;
            return null;
        }

        public static string GetServiceName(string serviceCode)
        {
            lock (m_taskRequestObj)
            {
                return m_taskRequestDic[serviceCode];
            }
        }

        public static bool HasRegister(string serviceCode)
        {
            return GetServiceName(serviceCode) != null ? true : false;
        }
    }
}
