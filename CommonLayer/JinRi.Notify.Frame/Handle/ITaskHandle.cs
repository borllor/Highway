using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public interface ITaskHandle
    {
        ITaskInfo Handle(string serverCode, ITaskInfo taskInfo);
    }
}
