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
    public class RedoServiceSetting
    {
        private static readonly object SyncObj = new object();

        [JsonProperty(PropertyName = "ScanSettingList")]
        private List<ScanSetting> _scanSettingList;
        [JsonProperty(PropertyName = "IsOpenBatchSendPushMessage")]
        private bool _isOpenBatchSendPushMessage;
        [JsonProperty(PropertyName = "AutoFlushSendMessage")]
        private int _autoFlushSendMessage;
        [JsonProperty(PropertyName = "PushAheadTime")]
        private int _pushAheadTime;

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

        public static List<ScanSetting> ScanSettingList
        {
            get
            {
                return Current._scanSettingList;
            }
            set
            {
                Current._scanSettingList = value;
            }
        }

        public static bool IsOpenBatchSendPushMessage
        {
            get
            {
                return Current._isOpenBatchSendPushMessage;
            }
            set
            {
                Current._isOpenBatchSendPushMessage = value;
            }
        }

        public static int AutoFlushSendMessage
        {
            get
            {
                return Current._autoFlushSendMessage;
            }
            set
            {
                Current._autoFlushSendMessage = value;
            }
        }

        public static int PushAheadTime
        {
            get
            {
                return Current._pushAheadTime;
            }
            set
            {
                Current._pushAheadTime = value;
            }
        }

        private static RedoServiceSetting Current
        {
            get
            {
                RedoServiceSetting tmpObj = DataCache.Get(CacheKeys.RedoServiceSettingCacheKey) as RedoServiceSetting;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = DataCache.Get(CacheKeys.RedoServiceSettingCacheKey) as RedoServiceSetting;
                        if (null == tmpObj)
                        {
                            tmpObj = NewSetting();
                            DataCache.Set(CacheKeys.RedoServiceSettingCacheKey, tmpObj, DateTime.Now.AddSeconds(CacheKeys.RedoServiceSettingCacheKey_Timeout));
                        }
                    }
                }
                return tmpObj;
            }
        }

        private static RedoServiceSetting NewSetting()
        {
            RedoServiceSetting setting = null;
            string settingStr = "";
#if !DEBUG
            settingStr = SettingHelper.GetNotifySettingValue("RedoServiceSetting");
#endif
            if (string.IsNullOrWhiteSpace(settingStr)) settingStr = ConfigurationAppSetting.RedoServiceSetting;
            if (!string.IsNullOrWhiteSpace(settingStr))
            {
                string errMsg = "";
                try
                {
                    setting = JsonConvert.DeserializeObject<RedoServiceSetting>(settingStr);
                    errMsg = "配置初始化成功";
                }
                catch (Exception ex)
                {
                    errMsg = "配置初始化异常：" + ex.GetString();
                }
                SettingHelper.LogInfo("RedoServiceSetting", settingStr, "JinRi.Notify.ServiceModel.RedoServiceSetting.NewSetting()", errMsg);
            }
            if (setting == null)
            {
                setting = new RedoServiceSetting();
                setting._isOpenBatchSendPushMessage = true;
                setting._autoFlushSendMessage = 1;
                setting._pushAheadTime = 1;
            }
            return setting;
        }

        public class ScanSetting
        {
            public string MessagePriority { get; set; }
            public int LimitCount { get; set; }
            public int PushStatus { get; set; }
            public int InternalTime { get; set; }
            public int PrevScanTimes { get; set; }
            public int NextScanTimes { get; set; }
            public int IdleSleepTime { get; set; }
            public List<string> MessageType { get; set; }
        }
    }
}
