using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel
{
    /// <summary>
    /// 系统运行模式
    /// </summary>
    public enum RunningModeEnum
    {
        Normal = 1,
        Publish = 2,
        Stressful = 4,
        Night = 8,
    }
}
