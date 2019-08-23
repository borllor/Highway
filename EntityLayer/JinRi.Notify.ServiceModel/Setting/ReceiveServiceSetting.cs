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
    public class ReceiveServiceSetting
    {
        private static readonly object SyncObj = new object();

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
        [JsonProperty(PropertyName = "IsDirectRouteHighToBuilderService")]
        private bool _isDirectRouteHighToBuilderService;
        [JsonProperty(PropertyName = "IsDirectRouteMiddleToBuilderService")]
        private bool _isDirectRouteMiddleToBuilderService;
        [JsonProperty(PropertyName = "IsDirectRouteNormalToBuilderService")]
        private bool _isDirectRouteNormalToBuilderService;
        [JsonProperty(PropertyName = "IsDirectRouteLowToBuilderService")]
        private bool _isDirectRouteLowToBuilderService;
        [JsonProperty(PropertyName = "EnableJudgeHasReceived")]
        private bool _enableJudgeHasReceived;

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

        public static bool IsDirectRouteHighToBuilderService
        {
            get
            {
                return Current._isDirectRouteHighToBuilderService;
            }
            set
            {
                Current._isDirectRouteHighToBuilderService = value;
            }
        }

        public static bool IsDirectRouteMiddleToBuilderService
        {
            get
            {
                return Current._isDirectRouteMiddleToBuilderService;
            }
            set
            {
                Current._isDirectRouteMiddleToBuilderService = value;
            }
        }

        public static bool IsDirectRouteNormalToBuilderService
        {
            get
            {
                return Current._isDirectRouteNormalToBuilderService;
            }
            set
            {
                Current._isDirectRouteNormalToBuilderService = value;
            }
        }

        public static bool IsDirectRouteLowToBuilderService
        {
            get
            {
                return Current._isDirectRouteLowToBuilderService;
            }
            set
            {
                Current._isDirectRouteLowToBuilderService = value;
            }
        }

        public static bool EnableJudgeHasReceived
        {
            get
            {
                return Current._enableJudgeHasReceived;
            }
            set
            {
                Current._enableJudgeHasReceived = value;
            }
        }

        private static ReceiveServiceSetting Current
        {
            get
            {
                ReceiveServiceSetting tmpObj = DataCache.Get(CacheKeys.ReceiveServiceSettingCacheKey) as ReceiveServiceSetting;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = DataCache.Get(CacheKeys.ReceiveServiceSettingCacheKey) as ReceiveServiceSetting;
                        if (null == tmpObj)
                        {
                            tmpObj = NewSetting();
                            DataCache.Set(CacheKeys.ReceiveServiceSettingCacheKey, tmpObj, DateTime.Now.AddSeconds(CacheKeys.ReceiveServiceSettingCacheKey_Timeout));
                        }
                    }
                }
                return tmpObj;
            }
        }

        private static ReceiveServiceSetting NewSetting()
        {
            ReceiveServiceSetting setting = null;
            string settingStr = "";
#if !DEBUG
            settingStr = SettingHelper.GetNotifySettingValue("ReceiveServiceSetting");
#endif
            if (string.IsNullOrWhiteSpace(settingStr)) settingStr = ConfigurationAppSetting.ReceiveServiceSetting;
            if (!string.IsNullOrWhiteSpace(settingStr))
            {
                string errMsg = "";
                try
                {
                    setting = JsonConvert.DeserializeObject<ReceiveServiceSetting>(settingStr);
                    errMsg = "配置初始化成功";
                }
                catch (Exception ex)
                {
                    errMsg = "配置初始化异常：" + ex.GetString();
                }
                SettingHelper.LogInfo("ReceiveServiceSetting", settingStr, "JinRi.Notify.ServiceModel.ReceiveServiceSetting.NewSetting()", errMsg);
            }
            if (setting == null)
            {
                setting = new ReceiveServiceSetting();
                setting._isOpenBatchReceiveHighMessage = false;
                setting._autoFlushReceiveHighMessage = 1;
                setting._isOpenBatchReceiveMiddleMessage = true;
                setting._autoFlushReceiveMiddleMessage = 2;
                setting._isOpenBatchReceiveNormalMessage = true;
                setting._autoFlushReceiveNormalMessage = 3;
                setting._isOpenBatchReceiveLowMessage = true;
                setting._autoFlushReceiveLowMessage = 4;
                setting._isDirectRouteHighToBuilderService = false;
                setting._isDirectRouteMiddleToBuilderService = false;
                setting._isDirectRouteNormalToBuilderService = false;
                setting._isDirectRouteLowToBuilderService = false;
                setting._enableJudgeHasReceived = true;
            }

            return setting;
        }
    }
}
