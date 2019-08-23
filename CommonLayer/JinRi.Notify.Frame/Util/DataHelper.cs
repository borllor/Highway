using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class DataHelper
    {
        public static void PushData(string filename, int bufferSize, CallPushData callback)
        {
            if (File.Exists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                PushData(fs, bufferSize, callback);
            }
        }

        public static void PushData(Stream input, int bufferSize, CallPushData callback)
        {
            if (input != null)
            {
                if (bufferSize <= 0) bufferSize = 1024;
                byte[] data = null;
                int totalLen = (int)input.Length;
                int bytesRead = 0;
                int bytesReaded = 0;
                int pos = 0;
                while (pos < totalLen)
                {
                    if (bufferSize > (totalLen - pos)) bufferSize = totalLen - pos;
                    data = new byte[bufferSize];
                    bytesRead = input.Read(data, 0, bufferSize);
                    bytesReaded = bytesReaded + bytesRead;
                    pos = pos + bytesRead;
                    callback(totalLen, bytesReaded, data);
                }
            }
        }
    }
}
