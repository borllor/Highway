using System;
using System.Collections.Generic;
using System.Threading;

using JinRi.Notify.Frame;
using JinRi.Notify.Entity;
using JinRi.Notify.Utility;
using JinRi.Notify.DB;
using JinRi.Notify.Model;

namespace JinRi.Notify.Business
{
    public static class ComputePushTimeBusiness
    {
        private static readonly PushMessageBusiness _pushMessageBus = new PushMessageBusiness();
        private static readonly NotifyInterfaceSettingBusiness _settingBus = new NotifyInterfaceSettingBusiness(); 

        public static DateTime ComputeNextPushTime(string pushId, out bool isPushNext)
        {
            return ComputeNextPushTime(_pushMessageBus.GetFromCache(pushId), out isPushNext);
        }

        public static DateTime ComputeNextPushTime(PushMessageModel message, out bool isPushNext)
        {
            return ComputeNextPushTime(message, _settingBus.Get(message.SettingId), out isPushNext);
        }

        public static DateTime ComputeNextPushTime(PushMessageModel message, NotifyInterfaceSettingModel setting, out bool isPushNext)
        {
            DateTime pushNextTime = Null.NullDate;
            isPushNext = false;
            int pushCount = message.PushCount;
            int limitCount = setting.PushLimitCount;
            string rule = setting.PushInternalRule;
            if (pushCount < limitCount)
            {
                if (string.IsNullOrWhiteSpace(rule))
                {
                    Process.Info("推送配置", "", string.Format("{0}的推送规则为空", setting.SettingId), "");
                }
                int second = 0;
                isPushNext = true;
                if (pushCount < setting.PushInternalRuleList.Count)
                {
                    second = setting.PushInternalRuleList[pushCount];
                }
                else
                {
                    second = setting.PushInternalRuleList[setting.PushInternalRuleList.Count - 1];
                }
                pushNextTime = DateTime.Now.AddSeconds(second);
            }
            return pushNextTime;
        }

        public static DateTime GetDatabaseTime()
        {
            return JinRiNotifyFacade.Instance.GetDatabaseTime();
        }
    }
}
