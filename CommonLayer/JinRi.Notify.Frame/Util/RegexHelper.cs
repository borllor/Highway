using System;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Frame
{
    public class RegexHelper
    {
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsValidIP(string tempString)
        {
            return Regex.IsMatch(tempString, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 中文的范围:\u4e00 - \u9fa5
        /// </summary>
        /// <param name="tmpString"></param>
        /// <returns></returns>
        public static bool IsChinese(string tmpString)
        {
            return Regex.IsMatch(tmpString, @"[\u4e00-\u9fa5]+");
        }

        /// <summary>
        /// 英文的范围 ^[A-Za-z]|/+$    
        /// </summary>
        /// <param name="tmpString"></param>
        /// <returns></returns>
        public static bool IsEnglish(string tmpString)     // added by Rachel  2013.4.22
        {
            return Regex.IsMatch(tmpString, @"^[A-Za-z]|/+$");
        }

        /// <summary>
        /// 日文在\u0800 - \u4e00
        /// </summary>
        /// <param name="tempString"></param>
        /// <returns></returns>
        public static bool IsJapanese(string tempString)
        {
            return Regex.IsMatch(tempString, "[\u0800-\u4e00]+");
        }

        /// <summary>
        /// 韩文为\xAC00-\xD7A3
        /// </summary>
        /// <param name="tempString"></param>
        /// <returns></returns>
        public static bool IsKorean(string tempString)
        {
            return Regex.IsMatch(tempString, "[\xAC00-\xD7A3]+");
        }

        /// <summary>
        /// 检查是否是符合规范的PNR
        /// </summary>
        /// <param name="pnr"></param>
        /// <returns></returns>
        public static bool IsValidPNR(string pnr)
        {
            return Regex.IsMatch(pnr, @"[0-9A-Z]{5,6}", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证舱位代码，只能验证单个舱位是否合法，不能验证列表
        /// </summary>
        /// <param name="cabinCode"></param>
        /// <returns></returns>
        public static bool IsCabin(string cabinCode)
        {
            Regex reg = new Regex(@"^[A-Za-z]{1}[0-9]{0,1}$");
            return reg.IsMatch(cabinCode);
        }
        /// <summary>
        /// 验证返点
        /// </summary>
        /// <param name="disc"></param>
        /// <returns></returns>
        public static bool IsDiscount(string disc)
        {
            Regex reg = new Regex(@"^[0-9]{1,2}(\.[0-9]{1,2})?$");
            return reg.IsMatch(disc);
        }

        /// <summary>
        /// 验证日期格式是否合法
        /// </summary>
        /// <returns>false: 不合法， true: 合法</returns>
        public static bool IsValidDate(string data)
        {
            string pattern = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";

            return Regex.IsMatch(data, pattern);
        }

        /// <summary>
        /// 验证日期格式是否合法
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDate(string data)
        {
            try
            {
                Convert.ToDateTime(data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证Decimal格式是否合法
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDecimal(string data)
        {
            try
            {
                Convert.ToDecimal(data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证Int32格式是否合法
        /// </summary>
        /// <returns>false: 不合法， true: 合法</returns>
        public static bool IsValidInt32(string data)
        {
            try
            {
                Convert.ToInt32(data);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static bool IsFlightNo(string data)
        {
            const string regex = "^[A-Za-z1-9]{2}[0-9]{3,4}$";
            return Regex.IsMatch(data, regex);
        }

        public static bool IsFlightAirport(string data)
        {
            const string regex = "^[A-Za-z]{3}$";
            return Regex.IsMatch(data, regex);
        }
    }
}
