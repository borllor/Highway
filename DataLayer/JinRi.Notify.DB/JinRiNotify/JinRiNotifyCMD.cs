using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Dapper;
using JinRi.Notify.Entity;
using JinRi.Notify.Frame;

namespace JinRi.Notify.DB
{
    internal class JinRiNotifyCMD
    {
        public static readonly JinRiNotifyCMD Instance = new JinRiNotifyCMD();

        #region NotifyMessage

        public int SaveNotifyMessage(NotifyMessageEntity entity)
        {
            return SaveNotifyMessage(new List<NotifyMessageEntity>(new NotifyMessageEntity[] { entity }));
        }

        public int SaveNotifyMessage(List<NotifyMessageEntity> entityList)
        {
            const string sql =
@"INSERT INTO NotifyMessage(MessageId, AppId, MessagePriority, MessageKey, MessageType, NotifyData, SourceFrom, ClientIP, CreateTime)
VALUES";
            StringBuilder vb = new StringBuilder();
            vb.Append(sql);
            foreach (NotifyMessageEntity entity in entityList)
            {
                vb.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}'),",
                    entity.MessageId, entity.AppId, entity.MessagePriority, entity.MessageKey, entity.MessageType, entity.NotifyData, entity.SourceFrom, entity.ClientIP, entity.CreateTime);
            }

            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(vb.Remove(vb.Length - 1, 1).ToString());
            }
        }

        public int TruncateNotifyMessage()
        {
            const string sql =
@"TRUNCATE TABLE NotifyMessage;";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql);
            }
        }

        public int DeleteNotifyMessage(string messageId)
        {
            const string sql =
@"DELETE FROM  notifymessage 
WHERE MessageId=@MessageId;";
            DynamicParameters param = new DynamicParameters();
            param.Add("MessageId", messageId);
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, param);
            }
        }

        #endregion

        #region PushMessage

        public int SavePushMessage(PushMessageEntity entity)
        {
            return SavePushMessage(new List<PushMessageEntity>(new PushMessageEntity[] { entity }));
        }

        public static long PushNotifyMessageCount = 0;
        public int SavePushMessage(List<PushMessageEntity> entityList)
        {
            const string sql =
@"INSERT INTO PushMessage(PushId, MessageId, SettingId, MessagePriority, MessageKey, MessageType, PushData, PushStatus, NextPushTime, LastModifyTime, MessageCreateTime, CreateTime, PushCount, Memo)
VALUES";
            StringBuilder vb = new StringBuilder();
            vb.Append(sql);
            foreach (PushMessageEntity entity in entityList)
            {
                vb.AppendFormat("('{0}', '{1}', {2}, '{3}', '{4}', '{5}', '{6}', {7}, '{8}', '{10}', '{9}', '{10}', {11}, '{12}'),",
                    entity.PushId, entity.MessageId, entity.SettingId, entity.MessagePriority, entity.MessageKey, entity.MessageType, entity.PushData, entity.PushStatus, entity.NextPushTime, entity.MessageCreateTime, entity.CreateTime, entity.PushCount, entity.Memo);
            }

            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(vb.Remove(vb.Length - 1, 1).ToString());
            }
        }

        /// <summary>
        /// 批量更新推送消息
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public int EditPushMessage(List<PushMessageEntity> entityList)
        {
            const string sql =
@"UPDATE PushMessage SET NextPushTime = @NextPushTime, PushCount = @PushCount, LastModifyTime = @LastModifyTime, PushStatus = @PushStatus
WHERE PushId = @PushId;";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, entityList);
            }
        }

        public int EditPushMessage(PushMessageEntity entity)
        {
            const string sql =
@"UPDATE PushMessage SET LastModifyTime = @LastModifyTime, {0} WHERE PushId = @PushId";

            List<string> list = new List<string>();
            if (!Null.IsNull(entity.PushStatus))
                list.Add("PushStatus");
            if (!Null.IsNull(entity.NextPushTime))
                list.Add("NextPushTime");
            if (!Null.IsNull(entity.MessagePriority))
                list.Add("MessagePriority");
            if (!Null.IsNull(entity.Memo))
                list.Add("Memo");
            if (!Null.IsNull(entity.PushCount))
                list.Add("PushCount");
            if (!Null.IsNull(entity.MessageType))
                list.Add("MessageType");
            if (!Null.IsNull(entity.PushData))
                list.Add("PushData");
            if (Null.IsNull(entity.LastModifyTime))
                entity.LastModifyTime = DateTime.Now;
            list.Add("LastModifyTime");

            List<string> cmdList = new List<string>();
            foreach (string s in list)
            {
                if (s.Equals("Memo", StringComparison.OrdinalIgnoreCase))
                {
                    cmdList.Add(string.Format("{0} = CONCAT({0}, @{0})", s));
                }
                else
                {
                    cmdList.Add(string.Format("{0} = @{0}", s));
                }
            }
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(string.Format(sql, string.Join(", ", cmdList)), entity);
            }
        }

        public int TruncatePushMessage()
        {
            const string sql =
@"TRUNCATE TABLE pushmessage;";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql);
            }
        }

        #endregion

        #region NotifyInterfaceSetting

        /// <summary>
        /// 新增配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int SaveNotifyInterfaceSetting(NotifyInterfaceSettingEntity entity)
        {
            const string sql =
@"INSERT INTO NotifyInterfaceSetting (AppId, MessageType, Address, Method, PushLimitCount, PushInternalRule, Encoding, CreateTime, LastModifyTime, `Status`)
VALUES(@AppId, @MessageType, @Address, @Method, @PushLimitCount, @PushInternalRule, @Encoding, @CreateTime, @LastModifyTime, @Status)";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, entity);
            }
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int EditNotifyInterfaceSetting(NotifyInterfaceSettingEntity entity)
        {
            const string sql =
@"UPDATE NotifyInterfaceSetting SET AppId = @AppId, MessageType = @MessageType, Address = @Address, Method = @Method, PushLimitCount = @PushLimitCount, 
PushInternalRule = @PushInternalRule, Encoding = @Encoding, Status = @Status
WHERE SettingId = @SettingId";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, entity);
            }
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
            const string sql =
@"INSERT INTO MessageTypeEnum(MessageType, Remark, MessagePriority, Status, CreateTime)
VALUE(@MessageType, @Remark, @MessagePriority, @Status, @CreateTime)";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, entity);
            }
        }

        /// <summary>
        /// 更新消息类型
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int EditNotifyMessageType(NotifyMessageTypeEntity entity)
        {
            const string sql =
@"UPDATE MessageTypeEnum SET MessageType = @MessageType, Remark = @Remark, MessagePriority = @MessagePriority, Status= @Status
WHERE MessageTypeId = @MessageTypeId";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, entity);
            }
        }

        #endregion

        #region NotifySetting

        public int SaveNotifySetting(NotifySettingEntity entity)
        {
            const string sql =
@"INSERT INTO NotifySetting(SettingKey, SettingValue, Remark, Memo, ClassName, CreateTime, LastModifyTime) 
VALUES(@SettingKey, @SettingValue, @Remark ,@Memo, @ClassName, CURRENT_TIMESTAMP(), CURRENT_TIMESTAMP())";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                int ret = conn.Execute(sql, entity);
                return ret;
            }
        }

        public int EditNotifySetting(NotifySettingEntity entity)
        {
            const string sql =
@"UPDATE NotifySetting SET LastModifyTime = CURRENT_TIMESTAMP(), {0} WHERE NotifyId = @NotifyId";

            List<string> list = new List<string>();
            if (!Null.IsNull(entity.SettingKey))
                list.Add("SettingKey");
            if (!Null.IsNull(entity.SettingValue))
                list.Add("SettingValue");
            if (!Null.IsNull(entity.Remark))
                list.Add("Remark");
            if (!Null.IsNull(entity.Memo))
                list.Add("Memo");
            if (!Null.IsNull(entity.ClassName))
                list.Add("ClassName");

            List<string> cmdList = new List<string>();
            foreach (string s in list)
            {
                if (s.Equals("Memo", StringComparison.OrdinalIgnoreCase))
                {
                    cmdList.Add(string.Format("{0} = CONCAT({0}, @{0})", s));
                }
                else
                {
                    cmdList.Add(string.Format("{0} = @{0}", s));
                }
            }
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(string.Format(sql, string.Join(", ", cmdList)), entity);
            }
        }

        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DelNotifySetting(NotifySettingEntity entity)
        {
            const string sql =
@"DELETE FROM NotifySetting WHERE SettingKey=@SettingKey";
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql, entity);
            }
        }

        #endregion

        public int ExecSqlCommand(string sql)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Execute(sql);
            }
        }

        public List<T> GetDataExecSql<T>(string sql)
        {
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                return conn.Query<T>(sql).ToList();
            }
        }

        #region 数据迁移
        public int ExecNotifyMessageDataMove(int day)
        {
            string notifySql =
@"CREATE TEMPORARY TABLE temp_notifymessage  (SELECT * FROM notifymessage WHERE DATEDIFF(CURRENT_TIME(),createtime) >=@Day  LIMIT 30000) ;
INSERT INTO notifymessage_backup(messageid ,appid,messagepriority,clientip,createtime,sourcefrom,messagekey,messagetype,notifydata)
SELECT messageid ,appid,messagepriority,clientip,createtime,sourcefrom,messagekey,messagetype,notifydata 
FROM temp_notifymessage ;
DELETE a FROM notifymessage a,temp_notifymessage b  
WHERE a.mid=b.mid ;
DROP TABLE temp_notifymessage;";

            DynamicParameters param = new DynamicParameters();
            param.Add("Day", day);
            int row = 0;
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                try
                {
                    conn.Open();
                    IDbTransaction tran = conn.BeginTransaction();
                    row = conn.Execute(notifySql, param);
                    if (row > 0)
                    {
                        tran.Commit();
                    }                 
                }
                catch(Exception ex) {
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "数据迁移", string.Format("数据迁移异常【{0}】", ex.ToString()), "数据");
                }
                return row;
            }
        }

        public int ExecPushMessageDataMove(int day)
        {
            string notifySql =
@"CREATE TEMPORARY TABLE temp_pushmessage  (SELECT * FROM pushmessage WHERE DATEDIFF(CURRENT_TIME(),createtime) >=@Day  LIMIT 30000) ;
INSERT INTO pushmessage_backup(pushid,messageid,settingid,pushstatus,nextpushtime,lastmodifytime,messagecreatetime,createtime,pushcount,messagepriority,messagekey,
messagetype,pushdata,memo)
SELECT pushid,messageid,settingid,pushstatus,nextpushtime,lastmodifytime,messagecreatetime,createtime,pushcount,messagepriority,messagekey,
messagetype,pushdata,memo 
FROM temp_pushmessage ;
DELETE a FROM pushmessage a,temp_pushmessage b  
WHERE a.pid=b.pid ;
DROP TABLE temp_pushmessage;";

            DynamicParameters param = new DynamicParameters();
            param.Add("Day", day);
            int row = 0;
            using (IDbConnection conn = ConnectionFactory.JinRiNotify_CMD)
            {
                try
                {
                    conn.Open();
                    IDbTransaction tran = conn.BeginTransaction();
                    row = conn.Execute(notifySql, param);
                    if (row > 0)
                    {
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "数据迁移", string.Format("数据迁移异常【{0}】", ex.ToString()), "数据");
                }
                return row;
            }
        }
        #endregion
    }
}
