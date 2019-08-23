using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JinRi.Notify.Frame;
using JinRi.Notify.Utility;
using Newtonsoft.Json;

namespace JinRi.Notify.ServiceModel
{
    [Serializable]
    public class LogSetting
    {
        private static readonly object SyncObj = new object();
        [JsonProperty(PropertyName = "LogLevel")]
        private LogLevelEnum _logLevel;

        public static LogLevelEnum LogLevel
        {
            get
            {
                return Current._logLevel;
            }
            set
            {
                Current._logLevel = value;
            }
        }

        private static LogSetting Current
        {
            get
            {
                LogSetting tmpObj = DataCache.Get(CacheKeys.LogSettingCacheKey) as LogSetting;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = DataCache.Get(CacheKeys.LogSettingCacheKey) as LogSetting;
                        if (null == tmpObj)
                        {
                            tmpObj = NewSetting();
                            DataCache.Set(CacheKeys.LogSettingCacheKey, tmpObj, DateTime.Now.AddSeconds(CacheKeys.LogSettingCacheKey_Timeout));
                        }
                    }
                }
                return tmpObj;
            }
        }

        private static LogSetting NewSetting()
        {
            LogSetting setting = null;
            string settingStr = "";
#if !DEBUG
            settingStr = SettingHelper.GetNotifySettingValue("LogSetting");
#endif
            if (string.IsNullOrWhiteSpace(settingStr)) settingStr = ConfigurationAppSetting.LogSetting;
            if (!string.IsNullOrWhiteSpace(settingStr))
            {
                string errMsg = "";
                try
                {
                    setting = JsonConvert.DeserializeObject<LogSetting>(settingStr);
                    errMsg = "配置初始化成功";
                }
                catch (Exception ex)
                {
                    errMsg = "配置初始化异常：" + ex.GetString();
                }
                SettingHelper.LogInfo("LogSetting", settingStr, "JinRi.Notify.ServiceModel.LogSetting.NewSetting()", errMsg);
            }
            if (setting == null)
            {
                setting = new LogSetting();
                setting._logLevel = LogLevelEnum.Info;
            }
            return setting;
        }
    }
}
