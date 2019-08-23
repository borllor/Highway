using System;
using System.Collections.Generic;

namespace JinRi.Notify.Frame
{
    public class HeartBeatTaskHandle : TaskHandle
    {
        public override ITaskInfo Handle(string serverCode, ITaskInfo taskInfo)
        {
            taskInfo.TaskProcess = TaskProcess.Completed;
            taskInfo.TaskStatus = TaskStatus.Success;
            return taskInfo;
        }
    }
}
