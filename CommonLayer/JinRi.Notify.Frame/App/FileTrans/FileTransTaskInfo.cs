using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    [Serializable]
    public class FileTransTaskInfo : TaskInfo
    {
        private string _filename;
        private string _saveFilename;
        private int _totalLength;
        private int _transLength;
        private FileTransProcess _transProcess;
        private bool _isNeedSelectSaveFilePath;

        private byte[] _data;

        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// 在服务器保存的文件路径
        /// </summary>
        public string SaveFilename
        {
            get { return _saveFilename; }
            set { _saveFilename = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int TotalLength
        {
            get { return _totalLength; }
            set { _totalLength = value; }
        }

        /// <summary>
        /// 已传送大小
        /// </summary>
        public int TransLength
        {
            get { return _transLength; }
            set { _transLength = value; }
        }

        /// <summary>
        /// 文件传送所属阶段
        /// </summary>
        public FileTransProcess TransProcess
        {
            get { return _transProcess; }
            set { _transProcess = value; }
        }

        /// <summary>
        /// 传输的文件数据
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// 是否需要选择保存文件路径(如果为false，但SaveFilename不能为空)
        /// </summary>
        public bool IsNeedSelectSaveFilePath
        {
            get { return _isNeedSelectSaveFilePath; }
            set { _isNeedSelectSaveFilePath = value; }
        }
    }
}
