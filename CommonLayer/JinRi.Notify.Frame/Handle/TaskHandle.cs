using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Text;

namespace JinRi.Notify.Frame
{
    public abstract class TaskHandle : ITaskHandle
    {
        public virtual ITaskInfo Handle(string serverCode, ITaskInfo taskInfo)
        {
            try
            {
                IServerInfo s = ServerManager.Get(serverCode);
                if (s.ServerStatus == ServerStatus.Active)
                {
                    if (s.TaskBeginHandler != null)
                    {
                        s.TaskBeginHandler(serverCode, taskInfo);
                    }
                    if (s.TaskHandler != null)
                    {
                        s.TaskHandler(serverCode, taskInfo);
                    }
                    if (s.TaskEndHandler != null)
                    {
                        s.TaskEndHandler(serverCode, taskInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                taskInfo.TaskRemark = ex.Message + "|" + ex.Source + "|" + ex.StackTrace;
                taskInfo.TaskProcess = TaskProcess.Completed;
                taskInfo.TaskStatus = TaskStatus.Failed;
            }
            return taskInfo;
        }

        protected virtual void TaskDistribution(object obj)
        {
            object[] objArr = obj as object[];
            if (objArr != null && objArr.Length == 2)
            {
                string serverCode = (string)objArr[0];
                ITaskInfo taskInfo = (ITaskInfo)objArr[1];
                TaskDistribution(serverCode, taskInfo);
            }
        }

        protected virtual void TaskDistribution(string serverCode, ITaskInfo taskInfo)
        {
            IEnumerator enumor = ServerManager.GetServerEnumerator();
            taskInfo.EmitServerCode = serverCode;
            if (enumor != null)
            {
                while (enumor.MoveNext())
                {
                    IServerInfo s = (IServerInfo)enumor.Current;
                    if (string.Compare(serverCode, s.ServerCode, true) != 0 //去掉自己发给自己
                        && taskInfo.EmitServerCode != s.ServerCode)  //去掉发给源服务器
                    {
                        if (s.ServerStatus == ServerStatus.Active && 
                            TaskAllocAlgFactory.GetProvider().Alloc(taskInfo, s))
                        {
                            try
                            {
                                Console.WriteLine("正在往" + s.ServerCode + "服务器分发数据");
                                ITaskRequest request = RegisterService.TaskRequestService.GetService(s.ServerCode);
                                ITaskInfo disTaskInfo = request.SendRequest(s.ServerCode, taskInfo);
                            }
                            catch (Exception ex)
                            {
                                s.ServerStatus = ServerStatus.Shutdown;
                                Console.WriteLine("向" + s.ServerCode + "服务器分发数据失败，异常信息为:" + ex.Message + "|" + ex.Source + "|" + ex.StackTrace);
                            }
                        }
                    }
                }
            }
        }
    }
}
