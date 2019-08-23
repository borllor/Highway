using System;
using System.Collections;
using System.Runtime.Remoting;

namespace JinRi.Notify.Frame
{
    public class DataCache
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DataCache));
        private const string CachePrefix = "JinRi_";

        #region CacheKeyCollection

        public const string SeachFlightDataCacheKey_Arg3 = "SeachFlightData_{0}_{1}_{2}";
        public const int SeachFlightDataTimeOut = 600;

        /// <summary>
        /// 距离供应商下班间隔(n)分钟内的政策不需要推送
        /// </summary>
        public const string RateToOffMinOfDutyConfigCacheKey = "RateToOffMinOfDutyConfig";
        public const int RateToOffMinOfDutyConfigTimeOut = 60 * 60;

        /// <summary>
        /// 所有航线舱位（不包含自动出票供应商信息）
        /// </summary>
        public const string SimpleAirlineCabinCacheKey = "SimpleAirlineCabinCacheKey";
        public const int SimpleAirlineCabinTimeOut = 60 * 60 * 12;

        /// <summary>
        /// 供应商上下班时间
        /// </summary>
        public const string ProviderWorkTimeCacheKey = "ProviderWorkTime";
        public const int ProviderWorkTimeTimeOut = 60 * 60 * 23;

        /// <summary>
        /// 通过审核的供应商列表
        /// </summary>
        public const string ProviderSetCacheKey = "ProviderSet";
        public const int ProviderSetTimeOut = 60 * 30;

        /// <summary>
        /// 用户信息
        /// </summary>
        public const string UserCacheKey_Arg1 = "User_{0}";
        public const int UserTimeOut = 60 * 20;

        /// <summary>
        /// 出发城市和航司是否VIP标记 【特殊政策专用】
        /// </summary>
        public const string SpecialRateConfigListCacheKey = "SpecialRateConfigList";
        public const int SpecialRateConfigListTimeOut = 60 * 60 * 2;

        /// <summary>
        /// 用户贴点信息缓存设置
        /// </summary>
        public const string FlightUserAllTieDianCacheKey_Args4 = "FlightUserAllTieDian_{0}_{1}_{2}_{3}";
        public const int FlightUserAllTieDianTimeOut = 60 * 40;

        /// <summary>
        /// App端用户贴点信息缓存设置
        /// </summary>
        public const string MobileUserTieDianCacheKey_Args1 = "MobileUserTieDian_{0}";
        public const int MobileUserTieDianTimeOut = 65;

        /// <summary>
        /// 区域限制表信息
        /// </summary>
        public const string RateAreaAgentCacheKey = "RateAreaAgent";
        public const int RateAreaAgentTimeOut = 60 * 40;

        /// <summary>
        /// 最高返点限制表信息
        /// </summary>
        public const string MaxDiscountInfoCacheKey = "MaxDiscountInfo";
        public const int MaxDiscountInfoTimeOut = 60 * 21;

        /// <summary>
        /// 指定航司最高返点限制表信息
        /// </summary>
        public const string MaxDiscountInfoByAircomECacheKey_Args1 = "MaxDiscountInfoByAircomE_{0}";
        public const int MaxDiscountInfoByAircomETimeOut = 60 * 22;

        /// <summary>
        /// 所有的供应商信息
        /// </summary>
        public const string AllProviderInfosCacheKey = "AllProviderInfos";
        public const int AllProviderInfosTimeOut = 60 * 60 * 24;

        /// <summary>
        /// 所有的供应商信息_Args1
        /// </summary>
        public const string AllProviderInfosCacheKey_Args1 = "AllProviderInfos_{0}";

        /// <summary>
        /// 所有的供应商信息
        /// </summary>
        public const string ProviderInfosCacheKey_Arg1 = "AllProviderInfos_{0}";
        public const int ProviderInfosTimeOut = 60 * 61 * 24;

        /// <summary>
        /// 城市三字码键值对（key 城市中文名 value 城市三字码）
        /// </summary>
        public const string CityCodePareListAsCNameForKeyCacheKey = "CityCodePareListAsCNameForKey";
        public const int CityCodePareListAsCNameForKeyTimeOut = 60 * 60 * 12;

        /// <summary>
        /// 特价信息
        /// </summary>
        public const string FlightSpecialPriceCacheKey_Args3 = "FlightSpecialPrice_{0}_{1}_{2}";
        public const int FlightSpecialPriceTimeOut = 20;

        /// <summary>
        /// 基础价格信息
        /// </summary>
        public const string FlightPriceCacheKey_Args3 = "FlightPrice_{0}_{1}_{2}";
        public const int FlightPriceTimeOut = 60 * 31;

        /// <summary>
        /// 基础全价信息
        /// </summary>
        public const string FlightFullPriceCacheKey_Args3 = "FlightFullPrice_{0}_{1}";
        public const int FlightFullPriceTimeOut = 60 * 60 * 12;

        /// <summary>
        /// 缓存类型属性
        /// </summary>
        public const string TypeOfProperties_Args1 = "Type_{0}_Properties";

        /// <summary>
        /// 舱位价格信息
        /// </summary>
        public const string GetPrice1CacheKey_Arg5 = "GetPrice1CacheKey_{0}_{1}_{2}_{3}_{4}";
        public const int GetPrice1TimeOut = 5;

        /// <summary>
        /// webConfig配置
        /// </summary>
        public const string WebConfigCacheKey_Args1 = "WebConfig_{0}";
        public const int WebConfigTimeOut = 60 * 15;

        /// <summary>
        /// 所有特殊政策
        /// </summary>
        public const string EntireOtherRateListCacheKey = "EntireOtherRateListCacheKey";
        public const int EntireOtherRateListTimeOut = 60 * 60 * 3;

        /// <summary>
        /// 供应商上下班时间
        /// </summary>
        public const string ProviderWorkTimeCacheKey_Args1 = "ProviderWorkTimeCacheKey_{0}";

        /// <summary>
        /// 公里数配置
        /// </summary>
        public const string FlightKmCacheKey_Args2 = "FlightKm_{0}_{1}";
        public const int FlightKmTimeOut = 60 * 15;

        /// <summary>
        /// 航班号 机型
        /// </summary>
        public const string FlightNumAndPlaneModelEntityKey = "FlightNumAndPlaneModelEntity";
        public const int FlightNumAndPlaneModelEntityTimeOut = 60 * 60 * 8;

        /// <summary>
        /// 航班号 机型 By 机型
        /// </summary>
        public const string FlightNumAndPlaneModelEntityKey_Args1 = "FlightNumAndPlaneModelEntity_{0}";

        /// <summary>
        /// 所有当天有效的舱位信息
        /// </summary>
        public const string CabinListKey_Args2 = "CabinList_{0}_{1}";
        public const int CabinListKeyTimeOut = 60 * 60 * 12;

        /// <summary>
        /// 特价舱位列表
        /// </summary>
        public const string CabinSpecialY_Args3 = "CabinSpecialY_{0}_{1}_{2}";
        public const int CabinSpecialYKeyTimeOut = 60 * 60 * 13;

        /// <summary>
        /// 所有航司当天有效的特殊舱位
        /// </summary>
        public const string CabinSpecialAll_Args1 = "CabinSpecialAll_{0}";
        public const int CabinSpecialAllKeyTimeOut = 60 * 60 * 14;

        /// <summary>
        /// 所有航司当天有效的特殊舱位中文名称
        /// </summary>
        public const string CabinSpecialNameAll_Args1 = "CabinSpecialNameAll_{0}";
        public const int CabinSpecialNameAllKeyTimeOut = 60 * 60 * 15;

        /// <summary>
        /// OfficeNum
        /// </summary>
        public const string GetOfficeNumKey_Args1 = "GetOfficeNum_{0}";
        public const int GetOfficeNumTimeOut = 60 * 60 * 10;

        /// <summary>
        /// 可获取高反的用户
        /// </summary>
        public const string ShowTypeUserList = "ShowTypeUserList";
        public const int ShowTypeUserListTimeOut = 60 * 60 * 11;

        /// <summary>
        /// 最高返点限制城市（接口）
        /// </summary>
        public const string TblInterfaceLimitMaxDiscountCacheKey_Args1 = "TblInterfaceLimitMaxDiscount_{0}";
        public const int TblInterfaceLimitMaxDiscountTimeOut = 60 * 20;

        /// <summary>
        /// 定经销商指定出发城市的政策最高允许的返点
        /// 格式：177298^177299|SZX|6
        /// </summary>
        public const string MaxDiscountLimitCacheKey = "MaxDiscountLimit";
        public const int MaxDiscountLimitTimeOut = 60 * 61 * 10;

        /// <summary>
        /// 绿色通道 用户配置
        /// </summary>
        public const string GreenChannelCacheKey = "GreenChannel";
        public const int GreenChannelTimeOut = 60 * 11;

        /// <summary>
        /// 所有用户的排除/指定供应商
        /// </summary>
        public const string AppointProvider = "AppointProvider";
        public const int AppointProviderTimeOut = 60 * 33;


        /// <summary>
        /// 改签信息
        /// </summary>
        public const string CancelStipulates_Args1 = "CancelStipulates_{0}";
        public const int CancelStipulatesTimeOut = 60 * 33;

        /// <summary>
        /// 退废信息
        /// </summary>
        public const string RefundStipulateV2Entitys_Args1 = "RefundStipulateV2Entitys_{0}";
        public const int RefundStipulateV2EntitysTimeOut = 60 * 33;

        public const string MaxDiscountLimitOpenCacheKey = "MaxDiscountLimitOpen";
        public const int MaxDiscountLimitOpenTimeOut = 60 * 12;

        /// <summary>
        /// 退废信息
        /// </summary>
        public const string RefundStipulateNewCacheKey_Arg2 = "RefundStipulateNew_{0}_{1}";
        public const int RefundStipulateNewTimeOut = 60;

        /// <summary>
        /// 改签信息
        /// </summary>
        public const string CancelStipulateNewCacheKey_Arg2 = "CancelStipulateNew_{0}_{1}";
        public const int CancelStipulateNewTimeOut = 61;

        /// <summary>
        /// OLD改签信息
        /// </summary>
        public const string CancelStipulates_Args2 = "CancelStipulates_{0}_{1}";

        /// <summary>
        /// OLD退废信息
        /// </summary>
        public const string RefundStipulateV2Entitys_Args3 = "RefundStipulateV2Entitys_{0}_{1}_{2}";


        /// <summary>
        /// 退废信息
        /// </summary>
        public const string RefundCancelStipulate_Args4 = "RefundCancelStipulate_{0}_{1}_{2}_{3}";
        public const int RefundCancelStipulateTimeOut = 60 * 33;

        /// <summary>
        /// 通过审核的供应商列表，接口专用
        /// </summary>
        public const string SetPassProviderIDListCacheKey = "SetPassProviderIDList";
        public const int SetPassProviderIDListTimeOut = 62 * 10;

        /// <summary>
        /// 所有小飞机机型
        /// </summary>
        public const string JasonTaxEntityCacheKey = "JasonTaxEntity";
        public const int JasonTaxEntityTimeOut = 63 * 30;

        /// <summary>
        /// 航班查询1分钟缓存
        /// </summary>
        public const string FltSearchResponseCacheKey_Args1 = "FS_{0}";
        public const int FltSearchResponseTimeOut = 60;

        /// <summary>
        ///排除供应商
        /// </summary>
        public const string MinValidateRateIDCacheKey = "MinValidateRateIDCacheKey";
        public const int MinValidateRateIDTimeOut = 600;

        /// <summary>
        ///最优政策
        /// </summary>
        public const string GetBestRateCacheKey = "GetBestRateCacheKey";
        public const int GetBestRateTimeOut = 120;

        /// <summary>
        /// 星期格式
        /// </summary>
        public const string WeekProductByPolicyCacheKey_Args2 = "WeekProductByPolicyCacheKey_{0}_{1}";
        public const int WeekProductByPolicyTimeOut = int.MaxValue;


        /// <summary>
        /// 公务员供应商配置
        /// </summary>
        public const string GetGWYProviderCacheKey = "GetGWYProviderCacheKey_{0}";
        public const int GetGWYProviderTimeOut = 60 * 12;

        /// <summary>
        /// 供应商分数
        /// </summary>
        public const string GetProviderScoreCacheKey = "GetProviderScoreCacheKey_{0}";
        public const int GetProviderScoreTimeOut = 60 * 12;

        /// <summary>
        /// 定位的配置 设置缓存
        /// </summary>
        public const string GetEtermScoreCacheKey = "GetEtermScoreCacheKey_{0}";
        public const int GetEtermScoreScoreTimeOut = 60 * 12;

        /// <summary>
        /// 通过订单编号获取供应商编号
        /// </summary>
        public const string GetOfficeNoByOrderNoCacheKey = "GetOfficeNoByOrderNoCacheKey_{0}";
        public const int GetOfficeNoByOrderNoScoreTimeOut = 60 * 12;

        /// <summary>
        /// 通过用户名获取用户地址
        /// </summary>
        public const string GetUrlByUserNameNoCacheKey = "GetUrlByUserNameNoCacheKey{0}";
        public const int GetUrlByUserNameNoScoreTimeOut = 60;
        
        /// <summary>
        /// 公务员航班缓存
        /// </summary>
        public const string GetGWYFlightCacheKey = "GetGWYFlightCacheKey_{0}";
        public const int GetGWYFlightTimeOut = 60 * 12;

        /// <summary>
        /// 默认政策
        /// </summary>
        public const string GetDefaultRateCacheKey_Args7 = "GetDefaultRateCacheKey_{0}_{1}_{2}_{3}_{4}_{5}_{6}";
        public const int GetDefaultRateTimeOut = 60 * 12;

        /// <summary>
        /// 政策用户缓存
        /// </summary>
        public const string UserInfoForRateCacheKey = "UserInfoForRateCacheKey";
        public const int UserInfoForRateCacheKeyTimeOut = 60;
        #endregion

        #region CacheKeyCollection4Up
        /// <summary>
        /// 用户设置的销售规则
        /// </summary>
        public const string UserRateSet_Arg2 = "UP_UserRateSet_{0}_{1}";
        public const int UserRateSetTimeOut = 60 * 30;

        /// <summary>
        /// 用户设置的变更销售规则
        /// </summary>
        public const string UserChangeRateSet_Arg2 = "UP_UserChangeRateSet_{0}_{1}";

#if DEBUG
        public const int UserChangeRateSetTimeOut = 60 * 1;
#else
        public const int UserChangeRateSetTimeOut = 60 * 10;
#endif

        /// <summary>
        /// 所有航司
        /// </summary>
        public const string AllAricom = "UP_AllAircom";
        public const int AllAricomTimeOut = 60 * 60 * 12;

        /// <summary>
        /// 所有航司（政策设置的缓存）
        /// </summary>
        public const string RateAllAricom = "UP_RateAllAricom";
        public const int RateAllAricomTimeOut = 60 * 60 * 12;

        /// <summary>
        /// 所有城市
        /// </summary>
        public const string AllCity = "AllCity";
        public const int AllCityTimeOut = 60 * 60 * 12;


        /// <summary>
        /// 淘宝航班
        /// </summary>
        public const string TaoBaoFlight = "TaoBaoFlight_{0}_{1}_{2}_{3}_{4}_{5}";
        public const int TaoBaoFlightTimeOut = 20;

        #endregion

        /// <summary>
        /// tblRate表结构
        /// </summary>
        public const string TblRateStruct = "TblRateStruct";
        public const int TblRateStructTimeOut = 60 * 60 * 24;

        #region 单例对象实现

        private static ICacheProvider m_providerInstance;
        private static readonly object m_lockObj = new object();

        public static ICacheProvider GetProvider()
        {
            if (m_providerInstance == null)
            {
                lock (m_lockObj)
                {
                    if (m_providerInstance == null)
                    {
                        ICacheProvider temp = null;
                        string type = AppSetting.DefaultCacheProvider;
                        if (!string.IsNullOrEmpty(type))
                        {
                            Type t = Type.GetType(type);
                            if (type.IndexOf(",", StringComparison.CurrentCulture) > 0)
                            {
                                string[] arr = type.Split(new char[] { ',' });
                                ObjectHandle oh = Activator.CreateInstance(arr[1].Trim(), arr[0].Trim());
                                if (oh != null)
                                {
                                    temp = oh.Unwrap() as ICacheProvider;
                                }
                            }
                        }
                        if (temp == null)
                        {
                            temp = new WebCacheProvider();
                        }
                        m_providerInstance = temp;
                    }
                }
            }
            return m_providerInstance;
        }

        #endregion

        #region CacheKey处理方法

        public static string CleanCacheKey(string CacheKey)
        {
            if (String.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            return CacheKey.Substring(CachePrefix.Length);
        }

        public static string GetCacheKey(string CacheKey)
        {
            if (string.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            return CachePrefix + CacheKey;
        }

        #endregion

        #region ICacheProvider 成员

        public static bool Add(string key, object value)
        {
            return GetProvider().Add(GetCacheKey(key), value);
        }

        public static bool Add(string key, object value, DateTime expiry)
        {
            return GetProvider().Add(GetCacheKey(key), value, expiry);
        }

        public static object Get(string key)
        {
            return GetProvider().Get(GetCacheKey(key));
        }

        public static bool Delete(string key)
        {
            return GetProvider().Delete(GetCacheKey(key));
        }

        public static bool KeyExists(string key)
        {
            return GetProvider().KeyExists(GetCacheKey(key));
        }

        public static bool Set(string key, object value)
        {
            return GetProvider().Set(GetCacheKey(key), value);
        }

        public static bool Set(string key, object value, DateTime expiry)
        {
            return GetProvider().Set(GetCacheKey(key), value, expiry);
        }

        public static IDictionaryEnumerator GetEnumerator()
        {
            return GetProvider().GetEnumerator();
        }

        #endregion
    }
}
