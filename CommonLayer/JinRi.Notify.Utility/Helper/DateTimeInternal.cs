using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Utility
{
    public class DateTimeInternal
    {
        /// <summary>
        /// 用于设置系统时间
        /// </summary>
        /// <param name="sysTime"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        private static extern bool SetLocalTime(ref SystemTime sysTime);

        public static void SynLocalTime(DateTime now)
        {
            SystemTime systemTime = new SystemTime();
            systemTime.wYear = (ushort)now.Year;
            systemTime.wMonth = (ushort)now.Month;
            systemTime.wDayOfWeek = (ushort)now.DayOfWeek;
            systemTime.wDay = (ushort)now.Day;
            systemTime.wHour = (ushort)now.Hour;
            systemTime.wMinute = (ushort)now.Minute;
            systemTime.wSecond = (ushort)now.Second;
            systemTime.wMiliseconds = (ushort)now.Millisecond;
            SetLocalTime(ref systemTime);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }
}
