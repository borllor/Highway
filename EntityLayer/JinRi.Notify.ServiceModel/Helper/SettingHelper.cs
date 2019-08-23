using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JinRi.Notify.Frame;
using JinRi.Notify.Utility;
using Newtonsoft.Json;
using JinRi.Notify.ServiceModel.Profile;

namespace JinRi.Notify.ServiceModel
{
    public class SettingHelper
    {
        public static string GetNotifySettingValue(string settingKey)
        {
            Type nsbType = Type.GetType("JinRi.Notify.Business.NotifySettingBusiness, JinRi.Notify.Business");
            object obj = nsbType.Assembly.CreateInstance("JinRi.Notify.Business.NotifySettingBusiness", true);
            return (string)nsbType.InvokeMember("GetNotifySettingValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, System.Type.DefaultBinder, obj, new object[] { settingKey });
        }

        public static void LogInfo(string settingName, string value, string module, string errMsg)
        {
            DBLog.Process(RequestProfile.Username, "", RequestProfile.ClientIP, module, "", "初始化服务配置", string.Format("配置名：{0}，配置值：{1}，消息：{2}", settingName, value, errMsg), "Info");
        }
    }
}
