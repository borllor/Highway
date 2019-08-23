using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public enum ServerStatus
    {
        /// <summary>
        /// 服务器不能使用
        /// </summary>
        None = 0,
        /// <summary>
        /// 服务器已关闭
        /// </summary>
        Shutdown = 1,
        /// <summary>
        /// 服务器可用
        /// </summary>
        Active = 2,
        /// <summary>
        /// 服务器现在很忙
        /// </summary>
        Busy = 4,
        /// <summary>
        /// 服务器现在很闲
        /// </summary>
        Easy = 8,
        /// <summary>
        /// 备份服务器
        /// </summary>
        Backup = 128
    }
}
