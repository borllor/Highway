using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace JinRi.Notify.Frame
{
    public class GetFileTransHandle : TaskHandle
    {
        public override ITaskInfo Handle(string serverCode, ITaskInfo taskInfo)
        {
            GetFileTransTaskInfo task = taskInfo as GetFileTransTaskInfo;
            try
            {
                string[] res = Directory.GetFiles(@"D:\APP\JinRi.App.Server\", "*.txt");
                task.AllFiles = string.Join("|", res);
            }
            catch (Exception)
            {
            }

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
                        string filename = task.Filename;
                        if (File.Exists(filename))
                        {
                            using (FileStream stream = new FileStream(filename, FileMode.Open))
                            {
                                task.FileLength = (int)stream.Length;

                                task.ReadedBytes = Math.Min(task.FileLength - task.StartPos, task.ReadedBytes);

                                if (task.ReadedBytes > 0)
                                {
                                    stream.Position = task.StartPos;
                                    task.Data = new byte[task.ReadedBytes];
                                    task.ReadedBytes = stream.Read(task.Data, 0, task.ReadedBytes);
                                    stream.Close();
                                }
                                else
                                {
                                    task.ReadedBytes = 0;
                                }
                            }
                        }
                        else
                        {
                            task.ReadedBytes = 0;
                        }

                        task.TaskStatus = TaskStatus.Success;
                        task.EmitServerCode = serverInfo.ServerCode;
                        task.TransProcess = FileTransProcess.TransDatas;
                        task.TaskCompleteTime = DateTime.Now;
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
