using System;
using System.Collections.Generic;
using System.Linq;
using JinRi.Notify.Business;
using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;

namespace JinRi.Notify.InstuctionService
{
    public class InstructionService : IInstructionService
    {
        private static List<string> _servers = new List<string>();
        private static readonly object _lockObj = new object();
        private static List<TaskMessage> _taskList = new List<TaskMessage>();
        /// <summary>
        /// 请求并接收指令中心的数据
        /// </summary>
        /// <param name="beatMessage"></param>
        /// <returns></returns>
        public BeatResult HeartBeat(BeatMessage beatMessage)
        {
            lock (_lockObj)
            {
                BeatResult beatResult = new BeatResult();
                beatResult.TaskList = new List<TaskMessage>();
                var tempList = _taskList.Where(t => t.TaskExecutor.Equals(beatMessage.HostIP) && t.Status == TaskMessageStatusEnum.None);
                foreach (var item in tempList)
                {
                    var msg = MappingHelper.From<TaskMessage, TaskMessage>(item);
                    beatResult.TaskList.Add(msg);
                }
                beatResult.Result = BeatResultEnum.Success;
                _taskList.Where(t => t.TaskExecutor.Equals(beatMessage.HostIP)).ToList()
                    .ForEach(t => t.Status = TaskMessageStatusEnum.Executed);
                DistributedCache.Add(CacheKeys.InstuctionTaskListCacheKey, _taskList, DateTime.Now.AddYears(1));
                return beatResult;
 
            }
            
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="taskMessage"></param>
        /// <returns></returns>
        public bool CreateTask(TaskMessage taskMessage)
        {
            lock (_lockObj)
            {
                long batchNumber = DateTime.Now.Ticks;
                DateTime currentTime = DateTime.Now;
                _servers = GetAllServers();
                foreach (var item in _servers)
                {
                    TaskMessage task = new TaskMessage();
                    task.BatchNumber = batchNumber;
                    task.TaskID = Guid.NewGuid().ToString();
                    task.TaskExecutor = item;
                    task.TaskType = taskMessage.TaskType;
                    task.TaskParam = taskMessage.TaskParam;
                    task.Status = TaskMessageStatusEnum.None;
                    task.CreateTime = currentTime;
                    _taskList.Add(task);
                }
                DistributedCache.Set(CacheKeys.InstuctionTaskListCacheKey, _taskList, DateTime.Now.AddYears(1));
            }
            return true;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="beatMessage"></param>
        /// <returns></returns>
        public bool RegisterServer(BeatMessage beatMessage)
        {
            lock (_lockObj)
            {
                if (!string.IsNullOrEmpty(beatMessage.HostIP) && !_servers.Contains(beatMessage.HostIP))
                {
                    _servers.Add(beatMessage.HostIP);
                }
                DistributedCache.Set(CacheKeys.InstuctionServerCacheKey, _servers, DateTime.Now.AddYears(1));
            }
            return true;
        }

        /// <summary>
        /// 获取任务数据
        /// </summary>
        /// <returns></returns>
        public List<TaskMessage> GetTaskMessageList()
        {
            return _taskList.OrderByDescending(t => t.BatchNumber).ToList();
        }

        /// <summary>
        /// 获取服务器列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllServers()
        {
            if (_servers.Count == 0)
            {
                _servers = DistributedCache.Get(CacheKeys.InstuctionServerCacheKey) as List<string>;
            }
            if (_servers == null)
            {
                _servers = new List<string>();
            }
            return _servers;
        }
    }
}
