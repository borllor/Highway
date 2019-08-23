using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JinRi.Notify.ServiceAgent.InstructionServiceSOA;
using JinRi.Notify.DTO;
using JinRi.Notify.Frame;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Condition;

namespace JinRi.Notify.Business
{
    public class InstructionServiceBusiness
    {
        public static bool RegisterServer(BeatMessage beatMessage)
        {
            //注册到指令中心
            InstructionServiceClient client = null;
            bool result = false;
            try
            {
                if (beatMessage != null)
                {
                    client = new InstructionServiceClient();
                    result = client.RegisterServer(beatMessage);
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 接收来自指令中心的指令
        /// </summary>
        /// <param name="beatMessage"></param>
        /// <returns></returns>
        public static BeatResult HeartBeat(BeatMessage beatMessage)
        {
            BeatResult result = new BeatResult() { Result = Model.BeatResultEnum.Failed };
            InstructionServiceClient client = null;
            try
            {
                if (beatMessage != null)
                {
                    client = new InstructionServiceClient();
                    result = client.HeartBeat(beatMessage);
                    if (result != null && result.Result == Model.BeatResultEnum.Success)
                    {
                        ExecInstruction(result.TaskList.Where(t => t.TaskExecutor.Equals(beatMessage.HostIP) && t.Status == Model.TaskMessageStatusEnum.None));
                    }
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="taskList"></param>
        private static void ExecInstruction(IEnumerable<TaskMessage> taskList)
        {
            if (taskList != null && taskList.Count() > 0)
            {
                foreach (TaskMessage task in taskList)
                {
                    switch (task.TaskType)
                    {
                        case Model.TaskTypeEnum.InterfaceClearCache:
                            ClearCache(task);
                            break;
                        case Model.TaskTypeEnum.Reboot:
                            break;
                        default:
                            break;
                    }
                }
            }
           
        }

        /// <summary>
        /// 指令清理缓存
        /// </summary>
        /// <param name="task"></param>
        private static void ClearCache(TaskMessage task)
        {
            if (task == null)
            {
                return;
            }
            string key = task.TaskParam;
            if (!string.IsNullOrEmpty(key))
            {
                if (DataCache.KeyExists(key) != null)     //HttpRuntime缓存
                {
                    DataCache.Delete(key);
                }
                if (DistributedCache.KeyExists(key) != null)    //分布式缓存
                {
                    DistributedCache.Delete(key);
                }
            }
        }

        /// <summary>
        /// 获取所有服务器
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllServers()
        {
            List<string> servers = new List<string>();
            InstructionServiceClient client = null;
            try
            {
                client = new InstructionServiceClient();
                servers = client.GetAllServers();
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            return servers;
        }

        /// <summary>
        /// 获取所有任务数据
        /// </summary>
        /// <returns></returns>
        public List<TaskMessageModel> GetTaskMessageList(InstructionCondition con)
        {
            List<TaskMessageModel> retData = new List<TaskMessageModel>();
            InstructionServiceClient client = null;
            try
            {

                client = new InstructionServiceClient();
                List<TaskMessage> data = client.GetTaskMessageList();
                data.ForEach(t =>
                {
                    TaskMessageModel model = MappingHelper.From<TaskMessageModel, TaskMessage>(t);
                    retData.Add(model);
                });
                con.RecordCount = retData.Count;
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            return retData.Skip(con.Offset).Take(con.PageSize).ToList();
        }

        public bool CreateTask(TaskMessageModel taskMessage)
        {
            bool result = false;
            InstructionServiceClient client = null;
            try
            {
                client = new InstructionServiceClient();
                TaskMessage message = MappingHelper.From<TaskMessage, TaskMessageModel>(taskMessage);
                result = client.CreateTask(message);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            return result;
        }
    }
}
