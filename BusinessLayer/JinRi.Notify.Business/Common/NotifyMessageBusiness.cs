using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.DB;
using JinRi.Notify.Entity;
using JinRi.Notify.Utility;
using JinRi.Notify.Model;
using JinRi.Notify.Frame;
using JinRi.Notify.ServiceModel.Condition;

namespace JinRi.Notify.Business
{
    public class NotifyMessageBusiness
    {
        public bool Save(NotifyMessage message)
        {
            return Save(new List<NotifyMessage>(new NotifyMessage[] { message }));
        }

        public bool Save(List<NotifyMessage> messageList)
        {
            List<NotifyMessageEntity> entityList = new List<NotifyMessageEntity>();
            messageList.ForEach((x) =>
            {
                entityList.Add(MappingHelper.From<NotifyMessageEntity, NotifyMessage>(x));
            });
            return JinRiNotifyFacade.Instance.SaveNotifyMessage(entityList) > 0;
        }

        /// <summary>
        /// 获取请求消息列表（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<NotifyMessageModel> GetNotifyMessageList(NotifyMessageCondition con)
        {
            NotifyMessageBusiness business = new NotifyMessageBusiness();
            var messageTypeList = business.GetNotifyMessageTypeListFromCache(new NotifyMessageTypeCondition());
            List<NotifyMessageEntity> entityList = JinRiNotifyFacade.Instance.GetNotifyMessageList(con);
            List<NotifyMessageModel> list = new List<NotifyMessageModel>();
            entityList.ForEach(x =>
            {
                var model = MappingHelper.From<NotifyMessageModel, NotifyMessageEntity>(x);
                var messageType = messageTypeList.Where(t => t.MessageType.ToString().Equals(model.MessageType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (messageType != null)
                {
                    model.MessageTypeCName = messageType.Remark;
                }             
                list.Add(model);
            });
            return list;
        }


        public void Truncate()
        {
            JinRiNotifyFacade.Instance.TruncateNotifyMessage();
        }

        public bool Delete(string messageId)
        {
            return JinRiNotifyFacade.Instance.DeleteNotifyMessage(messageId) > 0;
        }

        #region 通知类型

        /// <summary>
        /// 获取通知类型
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessageTypeModel> GetNotifyMessageTypeList(NotifyMessageTypeCondition con)
        {
            List<NotifyMessageTypeEntity> entityList = JinRiNotifyFacade.Instance.GetNotifyMessageTypeList(con);
            List<NotifyMessageTypeModel> list = new List<NotifyMessageTypeModel>();
            entityList.ForEach(t =>
            {
                NotifyMessageTypeModel model = MappingHelper.From<NotifyMessageTypeModel, NotifyMessageTypeEntity>(t);
                model.MessageTypeName = t.MessageType.ToString();
                model.MessagePriorityName = t.MessagePriority.ToString();
                list.Add(model);
            });
            return list;
        }


        /// <summary>
        /// 获取通知类型(缓存)
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessageTypeModel> GetNotifyMessageTypeListFromCache(NotifyMessageTypeCondition con)
        {
            var data = DataCache.Get(CacheKeys.NotifyMessageTypeKey) as List<NotifyMessageTypeModel>;
            if (data == null)
            {
                data = GetNotifyMessageTypeList(con);
                if (data == null)
                {
                    data = new List<NotifyMessageTypeModel>();
                }
                DataCache.Set(CacheKeys.NotifyMessageTypeKey, data, DateTime.Now.AddSeconds(CacheKeys.NotifyMessageTypeKey_TimeOut));
            }
            return data;
        }

        /// <summary>
        /// 获取配置信息总数
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int GetNotifyMessageTypeCount(NotifyMessageTypeCondition con)
        {
            return JinRiNotifyFacade.Instance.GetNotifyMessageTypeCount(con);
        }

         /// <summary>
        /// 新增配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveNotifyMessageType(NotifyMessageTypeModel model)
        {
            var entity = MappingHelper.From<NotifyMessageTypeEntity, NotifyMessageTypeModel>(model);
            entity.MessageType = model.MessageTypeName;
            return JinRiNotifyFacade.Instance.SaveNotifyMessageType(entity) > 0;
        }

        /// <summary>
        /// 新增配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool EditNotifyMessageType(NotifyMessageTypeModel model)
        {
            var entity = MappingHelper.From<NotifyMessageTypeEntity, NotifyMessageTypeModel>(model);
            entity.MessageType = model.MessageTypeName;
            return JinRiNotifyFacade.Instance.EditNotifyMessageType(entity) > 0;
        }

        #endregion
    }
}
