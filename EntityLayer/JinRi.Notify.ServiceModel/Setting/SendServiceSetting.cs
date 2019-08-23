using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.Frame;
using JinRi.Notify.Utility;
using Newtonsoft.Json;

namespace JinRi.Notify.ServiceModel
{
    [Serializable]
    public class SendServiceSetting
    {
        private static readonly object SyncObj = new object();

        [JsonProperty(PropertyName = "IsOpenBatchSendHighMessage")]
        private bool _isOpenBatchSendHighMessage;
        [JsonProperty(PropertyName = "AutoFlushSendHighMessage")]
        private int _autoFlushSendHighMessage;
        [JsonProperty(PropertyName = "IsOpenBatchSendMiddleMessage")]
        private bool _isOpenBatchSendMiddleMessage;
        [JsonProperty(PropertyName = "AutoFlushSendMiddleMessage")]
        private int _autoFlushSendMiddleMessage;
        [JsonProperty(PropertyName = "IsOpenBatchSendNormalMessage")]
        private bool _isOpenBatchSendNormalMessage;
        [JsonProperty(PropertyName = "AutoFlushSendNormalMessage")]
        private int _autoFlushSendNormalMessage;
        [JsonProperty(PropertyName = "IsOpenBatchSendLowMessage")]
        private bool _isOpenBatchSendLowMessage;
        [JsonProperty(PropertyName = "AutoFlushSendLowMessage")]
        private int _autoFlushSendLowMessage;
        [JsonProperty(PropertyName = "SendToClientTimeout")]
        private int _sendToClientTimeout;

        [JsonIgnore]
        private static long _systemStatus = 0;
        public static SystemStatusEnum SystemStatus
        {
            get
            {
                return (SystemStatusEnum)Interlocked.Read(ref _systemStatus);
            }
            set
            {
                Interlocked.Exchange(ref _systemStatus, (long)value);
            }
        }

        public static bool IsOpenBatchSendHighMessage
        {
            get
            {
                return Current._isOpenBatchSendHighMessage;
            }
            set
            {
                Current._isOpenBatchSendHighMessage = value;
            }
        }

        public static int AutoFlushSendHighMessage
        {
            get
            {
                return Current._autoFlushSendHighMessage;
            }
            set
            {
                Current._autoFlushSendHighMessage = value;
            }
        }

        public static bool IsOpenBatchSendMiddleMessage
        {
            get
            {
                return Current._isOpenBatchSendMiddleMessage;
            }
            set
            {
                Current._isOpenBatchSendMiddleMessage = value;
            }
        }

        public static int AutoFlushSendMiddleMessage
        {
            get
            {
                return Current._autoFlushSendMiddleMessage;
            }
            set
            {
                Current._autoFlushSendMiddleMessage = value;
            }
        }

        public static bool IsOpenBatchSendNormalMessage
        {
            get
            {
                return Current._isOpenBatchSendNormalMessage;
            }
            set
            {
                Current._isOpenBatchSendNormalMessage = value;
            }
        }

        public static int AutoFlushSendNormalMessage
        {
            get
            {
                return Current._autoFlushSendNormalMessage;
            }
            set
            {
                Current._autoFlushSendNormalMessage = value;
            }
        }

        public static bool IsOpenBatchSendLowMessage
        {
            get
            {
                return Current._isOpenBatchSendLowMessage;
            }
            set
            {
                Current._isOpenBatchSendLowMessage = value;
            }
        }

        public static int AutoFlushSendLowMessage
        {
            get
            {
                return Current._autoFlushSendLowMessage;
            }
            set
            {
                Current._autoFlushSendLowMessage = value;
            }
        }

        public static int SendToClientTimeout
        {
            get
            {
                return Current._sendToClientTimeout;
            }
            set
            {
                Current._sendToClientTimeout = value;
            }
        }

        private static SendServiceSetting Current
        {
            get
            {
                SendServiceSetting tmpObj = DataCache.Get(CacheKeys.SendServiceSettingCacheKey) as SendServiceSetting;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = DataCache.Get(CacheKeys.SendServiceSettingCacheKey) as SendServiceSetting;
                        if (null == tmpObj)
                        {
                            tmpObj = NewSetting();
                            DataCache.Set(CacheKeys.SendServiceSettingCacheKey, tmpObj, DateTime.Now.AddSeconds(CacheKeys.SendServiceSettingCacheKey_Timeout));
                        }
                    }
                }
                return tmpObj;
            }
        }

        private static SendServiceSetting NewSetting()
        {
            SendServiceSetting setting = null;
            string settingStr = "";
#if !DEBUG
            settingStr = SettingHelper.GetNotifySettingValue("SendServiceSetting");
#endif
            if (string.IsNullOrWhiteSpace(settingStr)) settingStr = ConfigurationAppSetting.SendServiceSetting;
            if (!string.IsNullOrWhiteSpace(settingStr))
            {
                string errMsg = "";
                try
                {
                    setting = JsonConvert.DeserializeObject<SendServiceSetting>(settingStr);
                    errMsg = "配置初始化成功";
                }
                catch (Exception ex)
                {
                    errMsg = "配置初始化异常：" + ex.GetString();
                }
                SettingHelper.LogInfo("SendServiceSetting", settingStr, "JinRi.Notify.ServiceModel.SendServiceSetting.NewSetting()", errMsg);
            }
            if (setting == null)
            {
                setting = new SendServiceSetting();
                setting._isOpenBatchSendHighMessage = false;
                setting._autoFlushSendHighMessage = 1;
                setting._isOpenBatchSendMiddleMessage = true;
                setting._autoFlushSendMiddleMessage = 3;
                setting._isOpenBatchSendNormalMessage = true;
                setting._autoFlushSendNormalMessage = 4;
                setting._isOpenBatchSendLowMessage = true;
                setting._autoFlushSendLowMessage = 5;
                setting._sendToClientTimeout = 15000;
            }

            return setting;
        }
    }
}
