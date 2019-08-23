using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    [Serializable]
    [Flags]
    public enum PushResultEnum
    {
        None = 0,
        Abort = 1,
        Success = 2,
        Failed = 4,
        Pushing = 8
    }
}
