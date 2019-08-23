using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    [Serializable]
    [Flags]
    public enum PushStatusEnum
    {
        /// <summary>
        /// 表示无效推送的数据
        /// </summary>
        None = 0,
        /// <summary>
        /// 表示终止推送的数据
        /// </summary>
        Abort = 1,
        /// <summary>
        /// 表示已经推送的数据
        /// </summary>
        Pushed = 2,
        /// <summary>
        /// 表示等待推送的数据
        /// </summary>
        UnPush = 4,
        /// <summary>
        /// 表示正在推送的数据
        /// </summary>
        Pushing = 8,
        /// <summary>
        /// 表示推送失败的
        /// </summary>
        Failed = 16
    }
}
