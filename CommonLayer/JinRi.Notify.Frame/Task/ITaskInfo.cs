using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public interface ITaskInfo : ICloneable
    {
        string TaskKey { get; set; }
        string TaskName { get; set; }
        string TaskRemark { get; set; }
        string EmitServerCode { get; set; }
        DateTime TaskCreateTime { get; set; }
        DateTime TaskStartTime { get; set; }
        DateTime TaskCompleteTime { get; set; }
        TaskProcess TaskProcess { get; set; }
        TaskStatus TaskStatus { get; set; }
    }
}
