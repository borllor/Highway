using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    [Serializable]
    [Flags]
    public enum TaskTypeEnum
    {
        InterfaceClearCache = 1,
        Reboot = 2
    }
}
