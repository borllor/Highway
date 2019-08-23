using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public delegate void BeginHandler(string serverCode, ITaskInfo taskInfo);
    public delegate void TaskHandler(string serverCode, ITaskInfo taskInfo);
    public delegate void EndHandler(string serverCode, ITaskInfo taskInfo);
}
