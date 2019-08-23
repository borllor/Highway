using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class TaskRequest : MarshalByRefObject, ITaskRequest
    {
        public ITaskInfo SendRequest(string serverCode, ITaskInfo taskInfo)
        {
            ITaskHandle taskHandle = TaskHandleFactory.CreateTaskHandle(taskInfo);
            if (taskHandle != null)
            {
                return taskHandle.Handle(serverCode, taskInfo);
            }
            return null;
        }

        public override object InitializeLifetimeService()
        {
            //Remoting对象 无限生存期
            return null;
        }
    }
}
