using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JinRi.Notify.Frame
{
    [Serializable]
    public class GetFileTransTaskInfo : TaskInfo
    {
        private string _filename;
        private string _saveFilename;
        private int _totalLength;
        private int _transLength;
        private FileTransProcess _transProcess;
        private int _startPos;
        private int _endPos;
        private int _readedBytes;
        private int _fileLength;

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
        /// 传送文件字节流的起始位置
        /// </summary>
        public int StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }

        /// <summary>
        /// 实际读取的字节数
        /// </summary>
        public int ReadedBytes
        {
            get { return _readedBytes; }
            set { _readedBytes = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileLength
        {
            get { return _fileLength; }
            set { _fileLength = value; }
        }

        /// <summary>
        /// 传输的文件数据
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public string AllFiles
        {
            get;
            set;
        }

    }
}
