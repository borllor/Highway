using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel
{
    [Flags]
    public enum SystemStatusEnum : long
    {
        Unstart = 0,
        Started = 1,
        Uninitialize = 2,
        Initializing = 4,
        Initialized = 8,
        Running = 16,
        Stopping = 32,
        Stopped = 64,
    }
}
