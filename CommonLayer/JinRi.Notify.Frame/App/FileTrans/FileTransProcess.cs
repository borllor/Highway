using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 文件传送流程
    /// </summary>
    public enum FileTransProcess
    {
        /// <summary>
        /// 请求发送文件
        /// </summary>
        TransFileTo = 1,
        /// <summary>
        /// 请求传送的文件
        /// </summary>
        TransFileFrom = 2,
        /// <summary>
        /// 接收到请求，正在准备
        /// </summary>
        TransPreparing = 4,
        /// <summary>
        /// 传送确认(做一些准备接收或传送文件的准备)
        /// </summary>
        TransConfirmed = 8,
        /// <summary>
        /// 正在准备传送数据
        /// </summary>
        TransDataPreparing = 16,
        /// <summary>
        /// 传送文件数据
        /// </summary>
        TransDatas = 32,
        /// <summary>
        /// 文件传送流程完成
        /// </summary>
        TransCompeleted = 64
    }
}