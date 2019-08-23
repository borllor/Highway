using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    [Serializable]
    [Flags]
    public enum MessagePriorityEnum : byte
    {
        None = 0,
        High = 9,
        Middle = 7,
        Normal = 5,
        Low = 3
    }
}
