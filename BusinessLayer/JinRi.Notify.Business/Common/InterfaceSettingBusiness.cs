using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DB;
using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using JinRi.Notify.Frame;
using JinRi.Notify.Entity;
using JinRi.Notify.ServiceModel.Condition;

namespace JinRi.Notify.Business
{
    public class NotifyInterfaceSettingBusiness
    {
        private static readonly object GetListLockObj = new object();

        public NotifyInterfaceSettingModel Get(int settingId)
        {
            List<NotifyInterfaceSettingModel> modelList = GetListFromCache();
            return modelList.Find(x => x.SettingId == settingId);
        }

        public NotifyInterfaceSettingModel GetNoCache(int settingId)
        {
            List<NotifyInterfaceSettingModel> modelList = GetList();
            return modelList.Find(x => x.SettingId == settingId);
        }

        public List<NotifyInterfaceSettingModel> GetList()
        {
            List<NotifyInterfaceSettingEntity> entityList = JinRiNotifyFacade.Instance.GetNotifyInterfaceSettingList();
            List<NotifyInterfaceSettingModel> messageList = new List<NotifyInterfaceSettingModel>();
            entityList.ForEach((x) =>
            {
                messageList.Add(MappingHelper.From<NotifyInterfaceSettingModel, NotifyInterfaceSettingEntity>(x));
            });
            return messageList;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="con">查询条件</param>
        /// <returns></returns>
        public List<NotifyInterfaceSettingModel> GetList(NotifyInterfaceSettingCondition condition)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            var messageTypeList = business.GetNotifyMessageTypeListFromCache(new NotifyMessageTypeCondition());
            List<NotifyInterfaceSettingEntity> entityList = JinRiNotifyFacade.Instance.GetNotifyInterfaceSettingList(condition);
            List<NotifyInterfaceSettingModel> messageList = new List<NotifyInterfaceSettingModel>();
            entityList.ForEach((x) =>
            {
                var model = MappingHelper.From<NotifyInterfaceSettingModel, NotifyInterfaceSettingEntity>(x);
                var messageType = messageTypeList.Where(t => t.MessageType == model.MessageType).FirstOrDefault();
                if (messageType != null)
                {
                    model.MessageTypeName = messageType.Remark;
                }
                model.MessageTypeEName = x.MessageType.ToString();
                messageList.Add(model);
            });
            return messageList;
        }


        /// <summary>
        /// 新增配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveNotifySetting(NotifyInterfaceSettingModel model)
        {
            var entity = MappingHelper.From<NotifyInterfaceSettingEntity, NotifyInterfaceSettingModel>(model);
            return JinRiNotifyFacade.Instance.SaveNotifySetting(entity) > 0;
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateNotifySetting(NotifyInterfaceSettingModel model)
        {
            var entity = MappingHelper.From<NotifyInterfaceSettingEntity, NotifyInterfaceSettingModel>(model);
            return JinRiNotifyFacade.Instance.EditNotifyInterfaceSetting(entity) > 0;
        }

        /// <summary>
        /// 获取配置信息总数
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int GetNotifyInterfaceSettingCount(NotifyInterfaceSettingCondition con)
        {
            return JinRiNotifyFacade.Instance.GetNotifyInterfaceSettingCount(con);
        }

        public List<NotifyInterfaceSettingModel> GetListFromCache()
        {
            List<NotifyInterfaceSettingModel> nsList = DataCache.Get(CacheKeys.NotifyInterfaceSettingListKey) as List<NotifyInterfaceSettingModel>;
            if (nsList == null)
            {
                nsList = GetList() ?? new List<NotifyInterfaceSettingModel>();
                DataCache.Set(CacheKeys.NotifyInterfaceSettingListKey, nsList, DateTime.Now.AddSeconds(CacheKeys.NotifyInterfaceSettingListKey_TimeOut));
            }
            return nsList;
        }
    }
}
