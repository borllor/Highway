using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JinRi.Notify.Frame;
using JinRi.Notify.Utility;
using Newtonsoft.Json;

namespace JinRi.Notify.ServiceModel
{
    [Serializable]
    public class BuilderServiceSetting
    {
        private static readonly object SyncObj = new object();

        [JsonProperty(PropertyName = "ParallelSubscribeSettingList")]
        private List<ParallelSubscribeSetting> _parallelSubscribeSettingList;
        [JsonProperty(PropertyName = "IsOpenBatchSaveNotifyMessage")]
        private bool _isOpenBatchSaveNotifyMessage;
        [JsonProperty(PropertyName = "AutoFlushNotifyMessage")]
        private int _autoFlushNotifyMessage;
        [JsonProperty(PropertyName = "IsOpenBatchSavePushMessage")]
        private bool _isOpenBatchSavePushMessage;
        [JsonProperty(PropertyName = "AutoFlushPushMessage")]
        private int _autoFlushPushMessage;
        [JsonProperty(PropertyName = "IsOpenBatchSendPushMessage")]
        private bool _isOpenBatchSendPushMessage;
        [JsonProperty(PropertyName = "AutoFlushSendMessage")]
        private int _autoFlushSendMessage;

        [JsonProperty(PropertyName = "IsOpenBatchReceiveHighMessage")]
        private bool _isOpenBatchReceiveHighMessage;
        [JsonProperty(PropertyName = "AutoFlushReceiveHighMessage")]
        private int _autoFlushReceiveHighMessage;

        [JsonProperty(PropertyName = "IsOpenBatchReceiveMiddleMessage")]
        private bool _isOpenBatchReceiveMiddleMessage;
        [JsonProperty(PropertyName = "AutoFlushReceiveMiddleMessage")]
        private int _autoFlushReceiveMiddleMessage;

        [JsonProperty(PropertyName = "IsOpenBatchReceiveNormalMessage")]
        private bool _isOpenBatchReceiveNormalMessage;
        [JsonProperty(PropertyName = "AutoFlushReceiveNormalMessage")]
        private int _autoFlushReceiveNormalMessage;

        [JsonProperty(PropertyName = "IsOpenBatchReceiveLowMessage")]
        private bool _isOpenBatchReceiveLowMessage;
        [JsonProperty(PropertyName = "AutoFlushReceiveLowMessage")]
        private int _autoFlushReceiveLowMessage;

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

        public static List<ParallelSubscribeSetting> ParallelSubscribeSettingList
        {
            get
            {
                return Current._parallelSubscribeSettingList;
            }
            set
            {
                Current._parallelSubscribeSettingList = value;
            }
        }

        public static bool IsOpenBatchSaveNotifyMessage
        {
            get
            {
                return Current._isOpenBatchSaveNotifyMessage;
            }
            set
            {
                Current._isOpenBatchSaveNotifyMessage = value;
            }
        }

        public static int AutoFlushNotifyMessage
        {
            get
            {
                return Current._autoFlushNotifyMessage;
            }
            set
            {
                Current._autoFlushNotifyMessage = value;
            }
        }

        public static bool IsOpenBatchSavePushMessage
        {
            get
            {
                return Current._isOpenBatchSavePushMessage;
            }
            set
            {
                Current._isOpenBatchSavePushMessage = value;
            }
        }

        public static int AutoFlushPushMessage
        {
            get
            {
                return Current._autoFlushPushMessage;
            }
            set
            {
                Current._autoFlushPushMessage = value;
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

        public static bool IsOpenBatchReceiveHighMessage
        {
            get
            {
                return Current._isOpenBatchReceiveHighMessage;
            }
            set
            {
                Current._isOpenBatchReceiveHighMessage = value;
            }
        }

        public static int AutoFlushReceiveHighMessage
        {
            get
            {
                return Current._autoFlushReceiveHighMessage;
            }
            set
            {
                Current._autoFlushReceiveHighMessage = value;
            }
        }

        public static bool IsOpenBatchReceiveMiddleMessage
        {
            get
            {
                return Current._isOpenBatchReceiveMiddleMessage;
            }
            set
            {
                Current._isOpenBatchReceiveMiddleMessage = value;
            }
        }

        public static int AutoFlushReceiveMiddleMessage
        {
            get
            {
                return Current._autoFlushReceiveMiddleMessage;
            }
            set
            {
                Current._autoFlushReceiveMiddleMessage = value;
            }
        }

        public static bool IsOpenBatchReceiveNormalMessage
        {
            get
            {
                return Current._isOpenBatchReceiveNormalMessage;
            }
            set
            {
                Current._isOpenBatchReceiveNormalMessage = value;
            }
        }

        public static int AutoFlushReceiveNormalMessage
        {
            get
            {
                return Current._autoFlushReceiveNormalMessage;
            }
            set
            {
                Current._autoFlushReceiveNormalMessage = value;
            }
        }

        public static bool IsOpenBatchReceiveLowMessage
        {
            get
            {
                return Current._isOpenBatchReceiveLowMessage;
            }
            set
            {
                Current._isOpenBatchReceiveLowMessage = value;
            }
        }

        public static int AutoFlushReceiveLowMessage
        {
            get
            {
                return Current._autoFlushReceiveLowMessage;
            }
            set
            {
                Current._autoFlushReceiveLowMessage = value;
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

        private static BuilderServiceSetting Current
        {
            get
            {
                BuilderServiceSetting tmpObj = DataCache.Get(CacheKeys.BuilderServiceSettingCacheKey) as BuilderServiceSetting;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = DataCache.Get(CacheKeys.BuilderServiceSettingCacheKey) as BuilderServiceSetting;
                        if (null == tmpObj)
                        {
                            tmpObj = NewSetting();
                            DataCache.Set(CacheKeys.BuilderServiceSettingCacheKey, tmpObj, DateTime.Now.AddSeconds(CacheKeys.BuilderServiceSettingCacheKey_Timeout));
                        }
                    }
                }
                return tmpObj;
            }
        }

        private static BuilderServiceSetting NewSetting()
        {
            BuilderServiceSetting setting = null;
            string settingStr = "";
#if !DEBUG
            settingStr = SettingHelper.GetNotifySettingValue("BuilderServiceSetting");
#endif
            if (string.IsNullOrWhiteSpace(settingStr)) settingStr = ConfigurationAppSetting.BuilderServiceSetting;
            if (!string.IsNullOrWhiteSpace(settingStr))
            {
                string errMsg = "";
                try
                {
                    setting = JsonConvert.DeserializeObject<BuilderServiceSetting>(settingStr);
                    errMsg = "配置初始化成功";
                }
                catch (Exception ex)
                {
                    errMsg = "配置初始化异常：" + ex.GetString();
                }
                SettingHelper.LogInfo("BuilderServiceSetting", settingStr, "JinRi.Notify.ServiceModel.BuilderServiceSetting.NewSetting()", errMsg);
            }
            if (setting == null)
            {
                setting = new BuilderServiceSetting();
                setting._isOpenBatchSaveNotifyMessage = true;
                setting._autoFlushNotifyMessage = 5;
                setting._isOpenBatchSavePushMessage = true;
                setting._autoFlushPushMessage = 1;
                setting._isOpenBatchSendPushMessage = true;
                setting._autoFlushSendMessage = 1;
                setting._isOpenBatchReceiveHighMessage = false;
                setting._autoFlushReceiveHighMessage = 1;
                setting._isOpenBatchReceiveMiddleMessage = true;
                setting._autoFlushReceiveMiddleMessage = 2;
                setting._isOpenBatchReceiveNormalMessage = true;
                setting._autoFlushReceiveNormalMessage = 3;
                setting._isOpenBatchReceiveLowMessage = true;
                setting._autoFlushReceiveLowMessage = 5;
                setting._pushAheadTime = 1;
            }

            return setting;
        }

        public class ParallelSubscribeSetting
        {
            public string MessagePriority { get; set; }
        }
    }
}
