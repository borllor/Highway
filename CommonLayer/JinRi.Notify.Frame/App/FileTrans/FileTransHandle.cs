using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace JinRi.Notify.Frame
{
    public class FileTransHandle : TaskHandle
    {
        public override ITaskInfo Handle(string serverCode, ITaskInfo taskInfo)
        {
            FileTransTaskInfo task = taskInfo as FileTransTaskInfo;
            IServerInfo serverInfo = ServerManager.Get(serverCode);
            try
            {
                if (serverInfo.TaskBeginHandler != null)
                {
                    serverInfo.TaskBeginHandler(serverCode, taskInfo);
                }
                if (serverInfo.TaskHandler != null)
                {
                    serverInfo.TaskHandler(serverCode, taskInfo);
                }
                else
                {
                    if (task.TransProcess == FileTransProcess.TransDatas)
                    {
                        ITaskInfo newTask = null;
                        if (serverInfo.IsDistributionServer)
                        {
                            newTask = (ITaskInfo)taskInfo.Clone();
                        }
                        if (task.Data != null && task.Data.Length > 0)
                        {
                            string filename = task.SaveFilename;
                            FileStream fs = null;
                            if (!File.Exists(filename))
                            {
                                fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                            }
                            else
                            {
                                fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                            }
                            task.TaskStartTime = DateTime.Now;
                            fs.Write(task.Data, 0, task.Data.Length);
                            fs.Close();
                            task.TaskStatus = TaskStatus.Success;

                            if (serverInfo.IsDistributionServer)
                            {
                                Console.WriteLine("开始分发数据");
                                object[] objArr = new object[] { serverCode, newTask };
                                ThreadPool.QueueUserWorkItem(new WaitCallback(TaskDistribution), objArr);
                            }
                            task.EmitServerCode = serverInfo.ServerCode;
                            task.TransProcess = FileTransProcess.TransDatas;
                            task.Data = null;
                            task.TaskCompleteTime = DateTime.Now;
                        }
                    }
                }
                if (serverInfo.TaskEndHandler != null)
                {
                    serverInfo.TaskEndHandler(serverCode, taskInfo);
                }
            }
            catch (Exception ex)
            {
                task.TaskRemark = ex.Message + "|" + ex.Source + "|" + ex.StackTrace;
                task.TaskStatus = TaskStatus.Failed;
            }
            
            return task;
        }
    }
}
