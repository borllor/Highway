using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using JinRi.Notify.Entity;


namespace JinRi.Notify.Data.DB
{
    internal class JinRiNotifyQuery
    {
        public static readonly JinRiNotifyQuery Instance = new JinRiNotifyQuery();

        public IEnumerable<NotifyMessageTypeEntity> GetNotifyMessageTypeList()
        {
            using (IDbConnection connection = new  SqlConnection (ConnectionString.JinRiNotifyDB_SELECT))
            {
                 string query =
        @"SELECT ID AS DapperDemoID, [Name] AS DapperDemoName, ModifiedDate 
  FROM dbo.DapperDemo
  WHERE ParentID > 0";
                return connection.Query<NotifyMessageTypeEntity>(query);
            }
        }

    }
}
