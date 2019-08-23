using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 文件日志记录器
    /// </summary>
    public class FileLog
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs\\log{0}.txt");
        private static readonly object m_lockLogFileObj1 = new object();
        private static readonly object m_lockLogFileObj2 = new object();

        public static void WriteTextLog(string log)
        {
            string filename = string.Format(LogFilePath, LogMessageDAL.GetLogTableSuffix());
            try
            {
                lock (m_lockLogFileObj1)
                {
                    if (!File.Exists(filename))
                    {
                        string dirPath = filename.Remove(filename.LastIndexOf("\\") + 1);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                    }
                    File.AppendAllText(filename, string.Format(@"-----------------------------------------{1}-----------------------------------------{0}{2}{0}", Environment.NewLine, DateTime.Now.ToString(), log));
                }
            }
            catch { }
        }

        public static void WriteTextLog(string log, string filename)
        {
            lock (m_lockLogFileObj2)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs\\" + filename);
                FileStream fs = new FileStream(path, FileMode.Append);
                StreamWriter streamWriter = new StreamWriter(fs);
                streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + log);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                fs.Close();
                fs.Dispose();
            }
        }
    }
}