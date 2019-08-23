using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Framework
{
    public interface ITaskRequest
    {
        ITaskInfo SendRequest(string serverCode, ITaskInfo taskInfo);
    }
}
