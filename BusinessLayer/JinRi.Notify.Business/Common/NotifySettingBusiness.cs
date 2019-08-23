using JinRi.Notify.Frame;
using JinRi.Notify.DB;
using JinRi.Notify.Entity;
using JinRi.Notify.Model;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Business
{
    public class NotifySettingBusiness
    {
         /// <summary>
        /// 获取AppConfig配置信息
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<NotifySettingModel> GetNotifySettingListFromCache(NotifySettingCondition condition)
        {
            var data = DataCache.Get(CacheKeys.NotifySettingCacheKey) as List<NotifySettingModel>;
            if (data == null)
            {              
                data = GetNotifySettingList(condition);              
                DataCache.Set(CacheKeys.NotifySettingCacheKey, data, DateTime.Now.AddSeconds(CacheKeys.NotifySettingCache_TimeOut));
            }
            return data;
        }

        public List<NotifySettingModel> GetNotifySettingList(NotifySettingCondition condition)
        {
            List<NotifySettingEntity> entityList = JinRiNotifyFacade.Instance.GetNotifySettingList(condition);
            List<NotifySettingModel> data = new List<NotifySettingModel>();
            entityList.ForEach(t =>
               {
                   var model = MappingHelper.From<NotifySettingModel, NotifySettingEntity>(t);
                   data.Add(model);
               });
            return data;
        }

        public NotifySettingModel GetNotifySettingFromCache(string settingkey)
        {
            NotifySettingModel model = null;
            List<NotifySettingModel> list = GetNotifySettingListFromCache(new NotifySettingCondition { SettingKey = settingkey });
            if (list != null && list.Count > 0)
            {
                model = list[0];
            }
            return model;
        }

        public NotifySettingModel GetNotifySetting(string settingkey)
        {
            NotifySettingModel model = null;
            List<NotifySettingModel> list = GetNotifySettingList(new NotifySettingCondition { SettingKey = settingkey });
            if (list != null && list.Count > 0)
            {
                model = list[0];
            }
            return model;
        }

        public string GetNotifySettingValue(string settingkey)
        {
            NotifySettingModel model = GetNotifySetting(settingkey);
            return model != null ? model.SettingValue : "";
        }

        public bool Edit(NotifySettingModel model)
        {
            var entity = MappingHelper.From<NotifySettingEntity, NotifySettingModel>(model);
            var result = JinRiNotifyFacade.Instance.EditNotifySetting(entity) > 0;
            if (result)     //请空缓存任务
            {
                CreateTask(model);
            }
            return result;
        }

        public bool Save(NotifySettingModel model)
        {
            var entity = MappingHelper.From<NotifySettingEntity, NotifySettingModel>(model);
            var result = JinRiNotifyFacade.Instance.SaveNotifySetting(entity) > 0;
            if (result)     //请空缓存任务
            {
                CreateTask(model);
            }
            return result;
        }

        private bool CreateTask(NotifySettingModel model)
        {
            InstructionServiceBusiness bus = new InstructionServiceBusiness();
            TaskMessageModel task = new TaskMessageModel();
            task.TaskType = TaskTypeEnum.InterfaceClearCache;
            switch (model.SettingKey)
            {
                case "BuilderServiceSetting":
                    task.TaskParam = CacheKeys.BuilderServiceSettingCacheKey;
                    break;
                case "ScanServiceSetting":
                    task.TaskParam = CacheKeys.ScanServiceSettingCacheKey;
                    break;
                case "ReceiveServiceSetting":
                    task.TaskParam = CacheKeys.ReceiveServiceSettingCacheKey;
                    break;
                case "RedoServiceSetting":
                    task.TaskParam = CacheKeys.RedoServiceSettingCacheKey;
                    break;
                case "SendServiceSetting":
                    task.TaskParam = CacheKeys.SendServiceSettingCacheKey;
                    break;
                case "LogSetting":
                    task.TaskParam = CacheKeys.LogSettingCacheKey;
                    break;
            }
            return bus.CreateTask(task);
        }

         /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(NotifySettingModel model)
        {
            var entity = MappingHelper.From<NotifySettingEntity, NotifySettingModel>(model);
            return JinRiNotifyFacade.Instance.DelNotifySetting(entity) > 0;
        }

        public bool ExecSqlCommand(string sql)
        {
            return JinRiNotifyFacade.Instance.ExecSqlCommand(sql) > 0;
        }

        public List<NotifyMessageModel> GetNotifyMessageData(string sql)
        {
            List<NotifyMessageEntity> list = JinRiNotifyFacade.Instance.GetDataExecSql<NotifyMessageEntity>(sql);
            List<NotifyMessageModel> modList = new List<NotifyMessageModel>();
            list.ForEach(x => modList.Add(MappingHelper.From<NotifyMessageModel, NotifyMessageEntity>(x)));
            return modList;
        }

        public List<PushMessageModel> GetPushMessageData(string sql)
        {
            List<PushMessageEntity> list = JinRiNotifyFacade.Instance.GetDataExecSql<PushMessageEntity>(sql);
            List<PushMessageModel> modList = new List<PushMessageModel>();
            list.ForEach(x => modList.Add(MappingHelper.From<PushMessageModel, PushMessageEntity>(x)));
            return modList;
        }      

    }
}
