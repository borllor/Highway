using System;
using System.Collections.Generic;

namespace JinRi.Notify.Frame
{
    public class LogMessageTaskHandle : TaskHandle
    {
        public override ITaskInfo Handle(string serverCode, ITaskInfo taskInfo)
        {
            try
            {
                LogMessageTaskInfo log = taskInfo as LogMessageTaskInfo;
                IServerInfo s = ServerManager.Get(serverCode);

                if (log != null && s != null && s.ServerStatus == ServerStatus.Active)
                {
                    if (s.TaskBeginHandler != null)
                    {
                        s.TaskBeginHandler(serverCode, taskInfo);
                    }
                    if (s.TaskHandler != null)
                    {
                        s.TaskHandler(serverCode, taskInfo);
                    }
                    else
                    {
                        MsmqLogger logger = MsmqLogger.GetInstance();
                        if (log.IsList)
                        {
                            List<LogMessage> ls = log.CurrLogMessageList;
                            foreach (LogMessage logMessage in ls)
                            {
                                logger.SendMessage(logMessage);
                            }
                            taskInfo.TaskProcess = TaskProcess.Completed;
                            taskInfo.TaskStatus = TaskStatus.Success;
                        }
                        else
                        {
                            LogMessage lm = log.CurrLogMessage;
                            logger.SendMessage(lm);
                            taskInfo.TaskProcess = TaskProcess.Completed;
                            taskInfo.TaskStatus = TaskStatus.Success;
                        }
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
    }
}
