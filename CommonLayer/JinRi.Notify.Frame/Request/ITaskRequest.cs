using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public interface ITaskRequest
    {
        ITaskInfo SendRequest(string serverCode, ITaskInfo taskInfo);
    }
}
