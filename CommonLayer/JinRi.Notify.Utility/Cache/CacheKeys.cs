using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Utility
{
    public static class CacheKeys
    {
        /// <summary>
        /// 接收消息缓存Key，{0}表示MessageKey，{1}表示MessageType
        /// </summary>
        public const string ReceivedMessage_Arg2 = "JinRi_Notify_ReceivedMessage_{0}_{1}";
        /// <summary>
        /// 接收消息缓存时间
        /// </summary>
        public const int ReceivedMessage_TimeOut = 60 * 10;

        /// <summary>
        /// 消息通知中心是否接收过此类型消息缓存键
        /// </summary>
        public const string HasReceivedMessage_Arg2 = "JinRi_Notify_HasReceivedMessage_{0}_{1}";
        /// <summary>
        /// 消息通知中心是否接收过此类型消息缓存键缓存时间
        /// </summary>
        public const int HasReceivedMessage_TimeOut = 60 * 15;

        /// <summary>
        /// 正在发送到消息通知中心的消息缓存键
        /// </summary>
        public const string SendToMessageHandler_Arg2 = "JinRi_Notify_SendToMessageHandler_{0}_{1}";
        /// <summary>
        /// 正在发送到消息通知中心的消息缓存键缓存时间
        /// </summary>
        public const int SendToMessageHandler_TimeOut = 60 * 10;

        /// <summary>
        /// NotifySetting配置缓存Key
        /// </summary>
        public const string NotifySettingCacheKey = "JinRi_Notify_NotifySettingCacheKey";

        /// <summary>
        /// NotifySetting配置缓存时间
        /// </summary>
        public const int NotifySettingCache_TimeOut = 60 * 10;

        /// <summary>
        /// ScanMessage配置缓存Key
        /// </summary>
        public const string ScanMessage_Arg2 = "JinRi_Notify_ScanMessage_{0}_{1}";
        /// <summary>
        /// ScanMessage配置缓存缓存时间
        /// </summary>
        public const int ScanMessage_TimeOut = 60 * 60;


        /// <summary>
        /// ScanMessage配置缓存Key
        /// </summary>
        public const string ScanMessageEx_Arg1 = "JinRi_Notify_ScanMessageEx_{0}";
        /// <summary>
        /// ScanMessage配置缓存缓存时间
        /// </summary>
        public const int ScanMessageEx_TimeOut = 60 * 5;

        #region JinRi.Notify.Web 用户

        /// <summary>
        /// 通知中心后台用户缓存key
        /// </summary>
        public const string WebUserCacheKey = "JinRi_Notify_WebUserCacheKey";

        /// <summary>
        /// 通知中心后台用户缓存时间
        /// </summary>
        public const int WebUserCache_TimeOut = 60 * 5;

        /// <summary>
        /// 用户session Key
        /// </summary>
        public const string UserSessionKey_Arg1 = "JinRi_Notify_UserSessionKey_{0}";

        #endregion

        /// <summary>
        /// 通知中心消息类型配置表
        /// </summary>
        public const string NotifyInterfaceSettingListKey = "JinRi_Notify_NotifyInterfaceSettingListKey";

        /// <summary>
        /// 通知中心消息类型配置表缓存时间
        /// </summary>
        public const int NotifyInterfaceSettingListKey_TimeOut = 60 * 30;

        /// <summary>
        /// 消息类型key
        /// </summary>
        public const string NotifyMessageTypeKey = "JinRi_Notify_NotifyMessageTypeKey";

        /// <summary>
        /// 消息类型缓存时间
        /// </summary>
        public const int NotifyMessageTypeKey_TimeOut = 60 * 60 * 2;

        /// <summary>
        /// 消息类型key
        /// </summary>
        public const string PushMessageTypeData = "JinRi_Notify_MessageTypeData";

        /// <summary>
        /// 消息类型缓存时间
        /// </summary>
        public const int PushMessageTypeData_TimeOut = 60 * 60 * 8;

        /// <summary>
        /// 本地推送数据缓存key
        /// </summary>
        public const string PushMessageToSender_Arg2 = "JinRi_Notify_PushMessageToSender_{0}_{1}";

        /// <summary>
        /// 本地推送数据缓存时间
        /// </summary>
        public const int PushMessageToSender_TimeOut = 60 * 10;

        /// <summary>
        /// 本地推送数据缓存时间
        /// </summary>
        public const string GetPushMessage_Arg1 = "JinRi_Notify_GetPushMessage_{0}";

        /// <summary>
        /// 本地推送数据缓存时间
        /// </summary>
        public const int GetPushMessage_TimeOut = 5;

        public const string BuilderServiceSettingCacheKey = "JinRi_Notify_BuilderServiceSettingCacheKey";
        public const int BuilderServiceSettingCacheKey_Timeout = 62 * 60;

        public const string ReceiveServiceSettingCacheKey = "JinRi_Notify_ReceiveServiceSettingCacheKey";
        public const int ReceiveServiceSettingCacheKey_Timeout = 61 * 60;

        public const string RedoServiceSettingCacheKey = "JinRi_Notify_RedoServiceSettingCacheKey";
        public const int RedoServiceSettingCacheKey_Timeout = 63 * 60;

        public const string ScanServiceSettingCacheKey = "JinRi_Notify_ScanServiceSettingCacheKey";
        public const int ScanServiceSettingCacheKey_Timeout = 64 * 60;

        public const string SendServiceSettingCacheKey = "JinRi_Notify_SendServiceSettingCacheKey";
        public const int SendServiceSettingCacheKey_Timeout = 59 * 60;

        public const string LogSettingCacheKey = "JinRi_Notify_LogSettingCacheKey";
        public const int LogSettingCacheKey_Timeout = 58 * 60;

        public const string InstuctionTaskListCacheKey = "JinRi_Notify_InstuctionTaskListCacheKey";
        public const string InstuctionServerCacheKey = "JinRi_Notify_InstuctionServerCacheKey";
        
    }
}
