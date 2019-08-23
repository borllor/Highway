using JinRi.Notify.Frame.Configs.TblWebConfig;
using JinRi.Notify.Frame.SmsForJinRi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Frame.Util
{
    public class SmsHelper
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="sMobilephone">手机号</param>
        /// <param name="scontext">发送内容</param>
        /// <param name="guid">日志guid</param>
        /// <param name="jinriDBKey">数据库链接key</param>
        /// <returns></returns>
        public static bool SendSms(string sMobilephone, string scontext, string guid, string jinriDBKey)
        {
            try
            {
                string doResult = string.Empty;
                string smsFlag =new TblWebConfig(jinriDBKey).GetCacheValue("JinriSMSPostMsgFlag", "0");
                string smsKey = new TblWebConfig(jinriDBKey).GetCacheValue("JinriSMSKeyCode", "");//密钥
                string requestStr = smsFlag + "^" + sMobilephone + "^" + scontext;
                requestStr = CryptographyHelper.Encrypt(requestStr, smsKey);
                try
                {
                    SmsService sendSms = new SmsService();
                    doResult = sendSms.SendSms(requestStr);
                }
                catch (Exception ex)
                {
                    DBLog.Process("", guid, "", "SmsHelper.cs", "", "短信发送失败", "短信发送失败,[" + ex.Message + "]" + ex.StackTrace);
                    return false;
                }
                string[] resultStr = doResult.Split('^');
                if (resultStr[0].Contains("全部成功"))
                {
                    return true;
                }
                else
                {
                    DBLog.Process("", guid, "", "SmsHelper.cs", "", "短信发送失败", "短信发送失败,[" + doResult + "]");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DBLog.Process("", guid, "", "SmsHelper.cs", "", "短信发送失败", "短信发送失败,[" + ex.Message + "]" + ex.StackTrace);
                return false;
            }
        }
    }
}
