using System;
using System.Collections.Generic;

using JinRi.Notify.Entity;
using JinRi.Notify.Frame;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Condition;

namespace JinRi.Notify.DB
{
    public class JinRiNotifyFacade
    {
        public static readonly JinRiNotifyFacade Instance = new JinRiNotifyFacade();

        #region NotifyMessage

        public int SaveNotifyMessage(NotifyMessageEntity entity)
        {
            return JinRiNotifyCMD.Instance.SaveNotifyMessage(entity);
        }

        public int SaveNotifyMessage(List<NotifyMessageEntity> entityList)
        {
            return JinRiNotifyCMD.Instance.SaveNotifyMessage(entityList);
        }      

        /// <summary>
        /// 获取请求消息列表（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<NotifyMessageEntity> GetNotifyMessageList(NotifyMessageCondition con)
        {
            return JinRiNotifyQuery.Instance.GetNotifyMessageList(con);
        }

        public int TruncateNotifyMessage()
        {
            return JinRiNotifyCMD.Instance.TruncateNotifyMessage();
        }
        public int DeleteNotifyMessage(string messageId)
        {
            return JinRiNotifyCMD.Instance.DeleteNotifyMessage(messageId);
        }

        #endregion

        #region PushMessage

        public int SavePushMessage(PushMessageEntity entity)
        {
            return JinRiNotifyCMD.Instance.SavePushMessage(entity);
        }

        public int SavePushMessage(List<PushMessageEntity> entityList)
        {
            return JinRiNotifyCMD.Instance.SavePushMessage(entityList);
        }

        public int EditPushMessage(PushMessageEntity entity)
        {
            return JinRiNotifyCMD.Instance.EditPushMessage(entity);
        }

        public PushMessageEntity GetPushMessageByID(string pushId)
        {
            return JinRiNotifyQuery.Instance.GetPushMessageByID(pushId);
        }

        public int TruncatePushMessage()
        {
            return JinRiNotifyCMD.Instance.TruncatePushMessage();
        }

        /// <summary>
        /// 批量更新推送消息
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public int EditPushMessage(List<PushMessageEntity> entityList)
        {
            return JinRiNotifyCMD.Instance.EditPushMessage(entityList);
        }

        /// <summary>
        /// 获取推送消息列表（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<PushMessageEntity> GetPushMessagePageList(PushMessageCondition con)
        {
            return JinRiNotifyQuery.Instance.GetPushMessagePageList(con);
        }

        /// <summary>
        /// 获取重推数据
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<PushMessageEntity> GetPushMessageList(PushMessageCondition con)
        {
            return JinRiNotifyQuery.Instance.GetPushMessageList(con);
        }

        public List<StatisticPushMessageEntity> GetDetailByTime(PushMessageCondition con)
        {
            return JinRiNotifyQuery.Instance.GetDetailByTime(con);
        }
        

        #endregion

        #region 通知类型

        /// <summary>
        /// 获取通知类型信息
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessageTypeEntity> GetNotifyMessageTypeList(NotifyMessageTypeCondition con)
        {
            return JinRiNotifyQuery.Instance.GetNotifyMessageTypeList(con);
        }

        /// <summary>
        /// 获取配置信息总数
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int GetNotifyMessageTypeCount(NotifyMessageTypeCondition con)
        {
            return JinRiNotifyQuery.Instance.GetNotifyMessageTypeCount(con);
        }

        #endregion

        #region NotifyInterfaceSetting

        /// <summary>
        /// 获取配置信息(分页)
        /// </summary>
        /// <returns></returns>
        public List<NotifyInterfaceSettingEntity> GetNotifyInterfaceSettingList(NotifyInterfaceSettingCondition con)
        {
            return JinRiNotifyQuery.Instance.GetNotifyInterfaceSettingList(con);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        public List<NotifyInterfaceSettingEntity> GetNotifyInterfaceSettingList()
        {
            return JinRiNotifyQuery.Instance.GetNotifyInterfaceSettingList();
        }

        /// <summary>
        /// 获取配置信息总数
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int GetNotifyInterfaceSettingCount(NotifyInterfaceSettingCondition con)
        {
            return JinRiNotifyQuery.Instance.GetNotifyInterfaceSettingCount(con);
        }

        /// <summary>
        /// 新增配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int SaveNotifySetting(NotifyInterfaceSettingEntity entity)
        {
            return JinRiNotifyCMD.Instance.SaveNotifyInterfaceSetting(entity);
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int EditNotifyInterfaceSetting(NotifyInterfaceSettingEntity entity)
        {
            return JinRiNotifyCMD.Instance.EditNotifyInterfaceSetting(entity);
        }

        #endregion

        #region NotifyMessageType

        /// <summary>
        /// 新增消息类型
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int SaveNotifyMessageType(NotifyMessageTypeEntity entity)
        {
            return JinRiNotifyCMD.Instance.SaveNotifyMessageType(entity);
        }

        /// <summary>
        /// 更新消息类型
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int EditNotifyMessageType(NotifyMessageTypeEntity entity)
        {
            return JinRiNotifyCMD.Instance.EditNotifyMessageType(entity);
        }

        #endregion


        /// <summary>
        /// 差异数据显示
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<NotifyMessageEntity> GetDifferentShowList(DifferentShowCondition con)
        {
            return JinRiNotifyQuery.Instance.GetDifferentShowList(con);
        }

        /// <summary>
        /// 获取数据服务器时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetDatabaseTime()
        {
            return JinRiNotifyQuery.Instance.GetDatabaseTime();
        }

        #region NotifySetting

        public List<NotifySettingEntity> GetNotifySettingList(NotifySettingCondition condition)
        {
            return JinRiNotifyQuery.Instance.GetNotifySettingList(condition);
        }

        public int EditNotifySetting(NotifySettingEntity entity)
        {
            return JinRiNotifyCMD.Instance.EditNotifySetting(entity);
        }

        public int SaveNotifySetting(NotifySettingEntity entity)
        {
            return JinRiNotifyCMD.Instance.SaveNotifySetting(entity);
        }

        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DelNotifySetting(NotifySettingEntity entity)
        {
            return JinRiNotifyCMD.Instance.DelNotifySetting(entity);
        }

        #endregion


        public int ExecSqlCommand(string sql)
        {
            return JinRiNotifyCMD.Instance.ExecSqlCommand(sql);
        }

        public List<T> GetDataExecSql<T>(string sql)
        {
            return JinRiNotifyCMD.Instance.GetDataExecSql<T>(sql);
        }

        public int ExecNotifyMessageDataMove()
        {
            return ExecNotifyMessageDataMove(30);
        }

        public int ExecNotifyMessageDataMove(int day)
        {
            return JinRiNotifyCMD.Instance.ExecNotifyMessageDataMove(day);
        }

        public int ExecPushMessageDataMove()
        {
            return ExecPushMessageDataMove(30);
        }

        public int ExecPushMessageDataMove(int day)
        {
            return JinRiNotifyCMD.Instance.ExecPushMessageDataMove(day);
        }        

    }
}