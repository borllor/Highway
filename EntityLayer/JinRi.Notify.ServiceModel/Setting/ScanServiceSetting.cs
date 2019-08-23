using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.Frame;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Utility;
using Newtonsoft.Json;

namespace JinRi.Notify.ServiceModel
{
    [Serializable]
    public class ScanServiceSetting
    {
        private static readonly object SyncObj = new object();
        [JsonProperty(PropertyName = "ScanSettingList")]
        private List<ScanSetting> _scanSettingList;

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
            get{
                return Current._scanSettingList;
            }
        }

        private static ScanServiceSetting Current
        {
            get
            {
                ScanServiceSetting tmpObj = DataCache.Get(CacheKeys.ScanServiceSettingCacheKey) as ScanServiceSetting;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = DataCache.Get(CacheKeys.ScanServiceSettingCacheKey) as ScanServiceSetting;
                        if (null == tmpObj)
                        {
                            tmpObj = NewSetting();
                            DataCache.Set(CacheKeys.ScanServiceSettingCacheKey, tmpObj, DateTime.Now.AddSeconds(CacheKeys.ScanServiceSettingCacheKey_Timeout));
                        }
                    }
                }
                return tmpObj;
            }
        }

        private static ScanServiceSetting NewSetting()
        {
            ScanServiceSetting setting = null;
            string settingStr = "";
            settingStr = ConfigurationAppSetting.ScanServiceSetting;
            if (!string.IsNullOrWhiteSpace(settingStr))
            {
                string errMsg = "";
                try
                {
                    setting = JsonConvert.DeserializeObject<ScanServiceSetting>(settingStr);
                    errMsg = "配置初始化成功";
                }
                catch (Exception ex)
                {
                    errMsg = "配置初始化异常：" + ex.GetString();
                }
                SettingHelper.LogInfo("ScanServiceSetting", settingStr, "JinRi.Notify.ServiceModel.ScanServiceSetting.NewSetting()", errMsg);
            }
            if (setting == null)
            {
                setting = new ScanServiceSetting();
                setting._scanSettingList = new List<ScanSetting>();
            }
            return setting;
        }

        public class ScanSetting
        {
            private int _orderStatus;
            private int _intervalTime;
            private string _orderBy;
            private int _scanOrderIdInit;
            private int _scanCount;
            private string _include;
            private int _idleSleepTime;
            public int _nextSpan;
            public string _messType;

            public int OrderStatus
            {
                get { return _orderStatus; }
                set { _orderStatus = value; }
            }
            public int IntervalTime
            {
                get { return _intervalTime; }
                set { _intervalTime = value; }
            }
            public string OrderBy
            {
                get { return _orderBy; }
                set { _orderBy = value; }
            }
            public int ScanOrderIdInit
            {
                get { return _scanOrderIdInit; }
                set { _scanOrderIdInit = value; }
            }
            public int ScanCount
            {
                get { return _scanCount; }
                set { _scanCount = value; }
            }
            public string Include
            {
                get { return _include; }
                set { _include = value; }
            }
            public int IdleSleepTime
            {
                get { return _idleSleepTime; }
                set { _idleSleepTime = value; }
            }
            public int NextSpan
            {
                get { return _nextSpan; }
                set { _nextSpan = value; }
            }
            public string MessType
            {
                get { return _messType; }
                set { _messType = value; }
            }

        }
    }
}
