using System;
using System.Collections.Generic;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class NumericHelper
    {
        public static string BytesToString(string byteString, char sep)
        {
            return BytesToString(byteString, sep, Encoding.UTF8);
        }

        public static string BytesToString(string byteString, char sep, Encoding encoding)
        {
            string[] bytesString = byteString.Split(sep);
            byte[] bts = new byte[bytesString.Length];
            for (int i = 0; i < bts.Length; i++)
            {
                bts[i] = Convert.ToByte(Convert.ToInt32(bytesString[i], 16));
            }
            return BytesToString(bts, encoding);
        }

        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }
    }
}
