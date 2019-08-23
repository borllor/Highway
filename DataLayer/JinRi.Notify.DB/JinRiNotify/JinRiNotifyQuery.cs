using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Dapper;
using JinRi.Notify.Entity;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Utility;
using JinRi.Notify.Frame;

namespace JinRi.Notify.DB
{
    internal class JinRiNotifyQuery
    {
        public static readonly JinRiNotifyQuery Instance = new JinRiNotifyQuery();

        #region NotifyInterfaceSetting

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        public List<NotifyInterfaceSettingEntity> GetNotifyInterfaceSettingList()
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                string query =
@"SELECT SettingId, AppId, MessageType, Address, Method, PushLimitCount, PushInternalRule, Encoding, CreateTime, LastModifyTime, Status 
FROM NotifyInterfaceSetting
WHERE Status = 2";
                return conn.Query<NotifyInterfaceSettingEntity>(query).ToList();
            }
        }

        /// <summary>
        /// 获取配置信息（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<NotifyInterfaceSettingEntity> GetNotifyInterfaceSettingList(NotifyInterfaceSettingCondition con)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string sql =
                  @"SELECT * FROM
                    (SELECT SettingId, AppId, MessageType, Address, Method, PushLimitCount, PushInternalRule, Encoding, CreateTime, LastModifyTime, Status
                    FROM NotifyInterfaceSetting 
                    WHERE {2} 
                    ORDER BY {0} {1} 
                    LIMIT @Offset, @PageSize) AS TMP ";
                string condition = string.Join(" AND ", GetNotifySettingConditionList(con).ToArray());
                string _sql = string.Format("SELECT COUNT(0) FROM NotifyInterfaceSetting WHERE {0}", condition);
                con.RecordCount = conn.Query<int>(_sql, con).FirstOrDefault();  //总记录数
                string trueSql = string.Format(sql, con.OrderBy, con.OrderDirection, condition);
                return conn.Query<NotifyInterfaceSettingEntity>(trueSql, con).ToList();
            }

        }

        /// <summary>
        /// 获取配置信息总数
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int GetNotifyInterfaceSettingCount(NotifyInterfaceSettingCondition con)
        {
            const string sql =
@"SELECT COUNT(0) FROM NotifyInterfaceSetting WHERE {0}";

            string trueSql = string.Format(sql, string.Join(" AND ", GetNotifySettingConditionList(con).ToArray()));
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                return conn.Query<int>(trueSql, con).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取配置列表查询条件
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private List<string> GetNotifySettingConditionList(NotifyInterfaceSettingCondition con)
        {
            List<string> list = new List<string>();
            list.Add(" 1=1 ");
            if (con != null)
            {
                if (!Null.IsNull(con.MessageType))
                {
                    list.Add(" MessageType = @MessageType ");
                }
                if (!Null.IsNull(con.Status))
                {
                    list.Add(" Status = @Status ");
                }
                if (!Null.IsNull(con.AppId))
                {
                    list.Add(" AppId = @AppId ");
                }
                if (!Null.IsNull(con.SettingId))
                {
                    list.Add(" SettingId = @SettingId ");
                }
            }
            return list;
        }

        #endregion

        #region NotifyMessage

        /// <summary>
        /// 获取请求消息列表（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<NotifyMessageEntity> GetNotifyMessageList(NotifyMessageCondition con)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string sql =
@"SELECT * FROM
(SELECT MessageId, AppId, MessagePriority, MessageKey, MessageType, NotifyData, SourceFrom, ClientIP, CreateTime 
FROM {3} 
WHERE {2} 
ORDER BY {0} {1} 
LIMIT @Offset, @PageSize) AS TMP ";
                string table = con.QuerySource == 2 ? "NotifyMessage_backup" : "NotifyMessage";
                string condition = string.Join(" AND ", GetNotifyMessageConditionList(con).ToArray());
                string _sql = string.Format("SELECT COUNT(0) FROM {1} WHERE {0}", condition, table);
                con.RecordCount = conn.Query<int>(_sql, con).FirstOrDefault();  //总记录数
                string trueSql = string.Format(sql, con.OrderBy, con.OrderDirection, condition,table);
                return conn.Query<NotifyMessageEntity>(trueSql, con).ToList();
            }

        }

        /// <summary>
        /// 消息列表条件
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private List<string> GetNotifyMessageConditionList(NotifyMessageCondition con)
        {
            List<string> list = new List<string>();
            list.Add(" 1=1 ");
            if (con != null)
            {
                if (!Null.IsNull(con.AppId))
                {
                    list.Add(" AppId = @AppId ");
                }
                if (!Null.IsNull(con.MessageType))
                {
                    list.Add(" MessageType = @MessageType ");
                }
                if (!string.IsNullOrWhiteSpace(con.MessageKey))
                {
                    if (con.MessageKey.Length <= 20)
                    {
                        list.Add(" MessageKey = @MessageKey ");
                    }
                    else
                    {
                        list.Add(" MessageKey LIKE CONCAT(@MessageKey, '%') ");
                    }
                }               
                if (!Null.IsNull(con.SCreateTime))
                {
                    list.Add(" CreateTime >= '" + con.SCreateTime + "' ");
                }
                if (!Null.IsNull(con.ECreateTime))
                {
                    list.Add(" CreateTime <= '" + con.ECreateTime + "' ");
                }
                if (!Null.IsNull(con.MessagePriority))
                {
                    list.Add(" MessagePriority = @MessagePriority ");
                }
                if (!Null.IsNull(con.MessageId))
                {
                    list.Add(" MessageId = @MessageId ");
                }
            }
            return list;
        }

        #endregion

        #region PushMessage

        public List<PushMessageEntity> GetPushMessageList(PushMessageCondition con)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string sql =
@"SELECT PushId, MessageId, SettingId, MessagePriority, MessageKey, MessageType, PushData, PushStatus, NextPushTime, LastModifyTime, MessageCreateTime, CreateTime, PushCount
FROM PushMessage 
WHERE {2}
ORDER BY {0} {1}
LIMIT @PageSize";
                string condition = string.Join(" AND ", GetPushMessageConditionList(con).ToArray());
                string trueSql = string.Format(sql, con.OrderBy, con.OrderDirection, condition);
                DBLog.Process("", con.MessagePriority, "", "GetPushMessageList", "", "获取重推数据列表", trueSql);

                return conn.Query<PushMessageEntity>(trueSql, con).ToList();
            }
        }

        public PushMessageEntity GetPushMessageByID(string pushId)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string query =
@"SELECT PushId, MessageId, SettingId, MessagePriority, MessageKey, MessageType, PushData, PushStatus, NextPushTime, LastModifyTime, MessageCreateTime, CreateTime, PushCount, Memo 
FROM PushMessage
WHERE PushId = @PushId";
                return conn.Query<PushMessageEntity>(query, new { PushId = pushId }).SingleOrDefault();
            }
        }

        /// <summary>
        /// 获取推送消息列表（分页）
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<PushMessageEntity> GetPushMessagePageList(PushMessageCondition con)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string sql =
@"SELECT * FROM
(SELECT PushId, MessageId, SettingId, MessagePriority, MessageKey, MessageType, PushData, PushStatus, NextPushTime, LastModifyTime, MessageCreateTime, CreateTime, PushCount, Memo 
FROM {3}  
WHERE {2} 
ORDER BY {0} {1} 
LIMIT @Offset, @PageSize) AS TMP ";
                string table = con.QuerySource == 2 ? "PushMessage_backup" : "PushMessage";
                string condition = string.Join(" AND ", GetPushMessageConditionList(con).ToArray());
                string _sql = string.Format("SELECT COUNT(0) FROM {1} WHERE {0}", condition, table);
                con.RecordCount = conn.Query<int>(_sql, con).FirstOrDefault();  //总记录数
                string trueSql = string.Format(sql, con.OrderBy, con.OrderDirection, condition, table);
                return conn.Query<PushMessageEntity>(trueSql, con).ToList();
            }

        }

        /// <summary>
        /// 消息列表条件
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private List<string> GetPushMessageConditionList(PushMessageCondition con)
        {
            List<string> list = new List<string>();
            list.Add(" 1=1 ");
            if (con != null)
            {
                if (!Null.IsNull(con.MessagePriority))
                {
                    list.Add(" MessagePriority = @MessagePriority ");
                }
                if (!Null.IsNull(con.SNextPushTime))
                {
                    list.Add(" NextPushTime >= @SNextPushTime ");
                }
                if (!Null.IsNull(con.ENextPushTime))
                {
                    list.Add(" NextPushTime <= @ENextPushTime ");
                }
                if (!Null.IsNull(con.MessageId))
                {
                    list.Add(" MessageId = @MessageId ");
                }
                if (!string.IsNullOrWhiteSpace(con.MessageKey))
                {
                    if (con.MessageKey.Length <= 20)
                    {
                        list.Add(" MessageKey = @MessageKey ");
                    }
                    else
                    {
                        list.Add(" MessageKey LIKE CONCAT(@MessageKey, '%') ");
                    }
                }

                if (con.PushIds != null && con.PushIds.Where(t => !string.IsNullOrEmpty(t)).Count() > 0)
                {
                    list.Add(string.Format(" PushId IN ('{0}') ", string.Join("', '", con.PushIds.ToArray())));
                }
                if (!Null.IsNull(con.PushStatus))
                {
                    list.Add(" PushStatus & @PushStatus = PushStatus ");
                }
                if (!Null.IsNull(con.PushId))
                {
                    list.Add(" PushId = @PushId ");
                }
                if (con.MessageType != null && con.MessageType.Where(t => !string.IsNullOrEmpty(t)).Count() > 0)
                {
                    list.Add(string.Format(" MessageType IN ('{0}') ", string.Join("', '", con.MessageType.ToArray())));
                }
            }
            return list;
        }

        #endregion

        #region 通知类型

        /// <summary>
        /// 获取通知类型信息
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessageTypeEntity> GetNotifyMessageTypeList(NotifyMessageTypeCondition con)
        {
            const string sql =
@"SELECT MessageTypeId, MessageType, Remark, MessagePriority, Status, CreateTime FROM MessageTypeEnum WHERE {0}";
            string condition = string.Join(" AND ", GetNotifyMessageTypeConditon(con));
            string trueSql = string.Format(sql, condition);
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                return conn.Query<NotifyMessageTypeEntity>(trueSql, con).ToList();
            }
        }

        /// <summary>
        /// 获取配置信息总数
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int GetNotifyMessageTypeCount(NotifyMessageTypeCondition con)
        {
            const string sql =
@"SELECT COUNT(0) FROM MessageTypeEnum WHERE {0}";

            string trueSql = string.Format(sql, string.Join(" AND ", GetNotifyMessageTypeConditon(con).ToArray()));
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                return conn.Query<int>(trueSql, con).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取通知类型查询条件
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private List<string> GetNotifyMessageTypeConditon(NotifyMessageTypeCondition con)
        {
            List<string> condition = new List<string>();
            condition.Add(" 1=1 ");
            if (con != null)
            {
                if (!Null.IsNull(con.Status))
                {
                    condition.Add(" Status = @Status ");
                }
                if (!Null.IsNull(con.MessageTypeId))
                {
                    condition.Add(" MessageTypeId = @MessageTypeId ");
                }
            }
            return condition;
        }

        #endregion

        #region NotifyConfig

        /// <summary>
        /// 获取AppConfig配置信息
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<NotifySettingEntity> GetNotifySettingList(NotifySettingCondition condition)
        {
            const string sql =
@"SELECT NotifyId,SettingKey,SettingValue,Remark,Memo,ClassName,CreateTime,LastModifyTime FROM notifysetting
WHERE {0}";
            var _sql = string.Format(sql, GetNotifySettingCondition(condition));
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                return conn.Query<NotifySettingEntity>(_sql, condition).ToList();
            }
        }

        private string GetNotifySettingCondition(NotifySettingCondition con)
        {
            List<string> conditon = new List<string>();
            conditon.Add(" 1=1 ");
            if (!Null.IsNull(con.SettingKey))
            {
                conditon.Add("SettingKey=@SettingKey");
            }
            return string.Join(" AND ", conditon.ToArray());
        }

        #endregion

        #region 差异数据显示
        public List<NotifyMessageEntity> GetDifferentShowList(DifferentShowCondition con)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string strSql =
@"SELECT a.MessageId, a.AppId, a.MessagePriority, a.MessageType, a.MessageKey, a.NotifyData, a.SourceFrom, a.ClientIP, a.CreateTime
 FROM notifymessage a LEFT JOIN pushmessage b ON a.MessageId=b.MessageId
 WHERE  a.CreateTime >= @StartDate
 AND a.CreateTime <= @EndDate
 AND b.MessageId IS NULL";
                return conn.Query<NotifyMessageEntity>(strSql, con).ToList();
            }
        }
        #endregion

        public List<PushMessageEntity> GetDetailByTime2(PushMessageCondition con)
        {
            const string strSql = @"SELECT  PushId, MessageId,  SettingId, MessagePriority,MessageKey,  MessageType, PushData, PushData, NextPushTime, LastModifyTime, MessageCreateTime, PushCount
 FROM PushMessage WHERE LastModifyTime BETWEEN @StartTime AND  @EndTime ";
            DynamicParameters param = new DynamicParameters();
            param.Add("StartTime", con.SNextPushTime);
            param.Add("EndTime", con.ENextPushTime);
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                return conn.Query<PushMessageEntity>(strSql, param).ToList();
            }
        }

        public List<StatisticPushMessageEntity> GetDetailByTime(PushMessageCondition con)
        {
            const string strSql =
@"SELECT  ''  NextPushTime,''  LastModifyTime,avg(TIMEDIFF(LastModifyTime,MessageCreateTime)) AvgPushTime   
FROM PushMessage 
WHERE PushCount=1
AND PushStatus=2
AND LastModifyTime>MessageCreateTime
AND LastModifyTime BETWEEN @StartTime AND  @EndTime ";
            DynamicParameters param = new DynamicParameters();
            param.Add("StartTime", con.SNextPushTime);
            param.Add("EndTime", con.ENextPushTime);
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                return conn.Query<StatisticPushMessageEntity>(strSql, param).ToList();
            }
        }

        #region 公用

        public DateTime GetDatabaseTime()
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_SELECT)
            {
                const string sql = @"SELECT CURRENT_TIMESTAMP() ";
                return conn.Query<DateTime>(sql).SingleOrDefault();
            }
        }

        #endregion
    }
}
