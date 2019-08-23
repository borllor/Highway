using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace JinRi.Notify.Frame
{
    public class FileTransEventArgs : EventArgs
    {
        #region 字段

        private string _msg;

        #endregion

        #region 属性

        public string Message
        {
            get { return _msg; }
            set { _msg = value; }
        }

        #endregion

        #region 构造函数

        public FileTransEventArgs(string msg)
        {
            Message = msg;
        }

        #endregion
    }
}
