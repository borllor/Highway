using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

using System.Data;
using JinRi.Notify.Entity.JinRiDB;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Frame;
using JinRi.Notify.Entity;
using JinRi.Notify.ServiceModel;
using Newtonsoft.Json;

namespace JinRi.Notify.DB
{
    public class JinRiDBQuery
    {
        public static readonly JinRiDBQuery Instance = new JinRiDBQuery();

        /// <summary>
        /// 根据配置键获取配置实例
        /// </summary>
        /// <param name="settingKey">配置键</param>
        /// <returns></returns>
        public WebConfigEntity GetWebConfigBySettingKey(string settingKey)
        {
            const string sql =
                @"SELECT TOP 1 SettingKey,SettingValue,Remark,[Date] 
                FROM tblWebConfig WITH(NOLOCK) WHERE SettingKey = @SettingKey";

            using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
            {
                conn.Open();
                return conn.Query<WebConfigEntity>(sql, new { SettingKey = settingKey }).SingleOrDefault<WebConfigEntity>();
            }
        }

        /// <summary>
        /// 获取扫描订单信息
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public List<NotifyOrderEntity> GetOrdersList(ScanOrderCondition condition)
        {
            string sql = "";
            try
            {
                const string sqlStr =
@"SELECT OrderNo,  OrderStatus,{0} OutTime,SalesmanID,ProviderID,ProxyerID
FROM tblOrders WITH(NOLOCK) 
WHERE OrderStatus = @OrderStatus 
AND {0} BETWEEN @Stime AND @Etime 
AND OrderId > @OrderId  {1}
ORDER BY {0}";              
                string includes = "";    
                if (!string.IsNullOrWhiteSpace(condition.Includes))
                {
                    includes = string.Format(" AND SalesmanId IN ({0}) ", condition.Includes);
                }
                sql = string.Format(sqlStr, condition.OrderBy, includes);

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("OrderStatus", condition.Status);
                parameter.Add("Stime", condition.StartTime);
                parameter.Add("Etime", condition.EndTime);
                parameter.Add("OrderId", condition.ScanOrderIdInit);
                parameter.Add("OrderByField", condition.OrderBy);

                DBLog.Process("sql", "sql", "", "sql", "sql", "sql", string.Format("sql:【{0}】，扫描查询条件ScanOrderCondition：【{1}】", sql, JsonConvert.SerializeObject(condition)), "sql");
                using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
                {
                    return conn.Query<NotifyOrderEntity>(sql, parameter).ToList();
                }
            }
            catch (Exception ex)
            {
                DBLog.Process("", "", "", "JinRiDBQuery.GetOrdersList", "", "获取订单信息", string.Format("sql:【{0}】，扫描查询条件ScanOrderCondition：【{1}】，获取订单信息异常，ex：{2}", sql, JsonConvert.SerializeObject(condition), ex.Message), "Fatal");
                List<NotifyOrderEntity> list = new List<NotifyOrderEntity>();
                return list;
            }
        }

        /// <summary>
        /// 补扫
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<NotifyOrderEntity> GetOrdersListBuSao(ScanOrderCondition condition)
        {
            string sql = "";
            try
            {
                List<NotifyOrderEntity> notifyList = new List<NotifyOrderEntity>();
                List<NotifyOrderEntity> list1 = null;
                List<NotifyOrderEntity> list2 = null;
                List<NotifyOrderEntity> list3 = null;               
                string includes = "";
                if (!string.IsNullOrWhiteSpace(condition.Includes))
                {
                    includes = string.Format(" AND SalesmanId IN ({0}) ", condition.Includes);
                }

                #region 出票
                string sqlStr =
@"SELECT OrderNo,  OrderStatus
FROM tblOrders WITH(NOLOCK) 
WHERE OrderStatus =2 
AND OutTime BETWEEN @Stime AND @Etime 
AND OrderId > @OrderId  {0}
ORDER BY OutTime";
                sql = string.Format(sqlStr, includes);
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("Stime", condition.StartTime);
                parameter.Add("Etime", condition.EndTime);
                parameter.Add("OrderId", condition.ScanOrderIdInit);
                using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
                {
                    list1 = conn.Query<NotifyOrderEntity>(sql, parameter).ToList();
                }
                #endregion

                #region 暂不能出票
                sqlStr =
@"SELECT OrderNo,  OrderStatus
FROM tblOrders WITH(NOLOCK) 
WHERE OrderStatus =7 
AND Contingent7 BETWEEN @Stime AND @Etime 
AND OrderId > @OrderId  {0}
ORDER BY Contingent7";
                sql = string.Format(sqlStr, includes);
                using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
                {
                    list2 = conn.Query<NotifyOrderEntity>(sql, parameter).ToList();
                }
                #endregion

                #region 退款成功
                sqlStr =
@"SELECT OrderNo,  OrderStatus
FROM tblOrders WITH(NOLOCK) 
WHERE OrderStatus =5 
AND OverTime BETWEEN @Stime AND @Etime 
AND OrderId > @OrderId  {0}
ORDER BY OverTime";
                sql = string.Format(sqlStr, includes);
                using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
                {
                    list3 = conn.Query<NotifyOrderEntity>(sql, parameter).ToList();
                }
                #endregion

                if (list1.Any())
                {
                    notifyList.AddRange(list1);
                }
                if (list2.Any())
                {
                    notifyList.AddRange(list2);
                }
                if (list3.Any())
                {
                    notifyList.AddRange(list3);
                }
                return notifyList;
            }
            catch (Exception ex)
            {
                DBLog.Process("", "", "", "JinRiDBQuery.GetOrdersList", "", "获取订单信息", string.Format("sql:【{0}】，扫描查询条件ScanOrderCondition：【{1}】，获取订单信息异常，ex：{2}", sql, JsonConvert.SerializeObject(condition), ex.Message), "Fatal");
                List<NotifyOrderEntity> list = new List<NotifyOrderEntity>();
                return list;
            }           
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        public DateTime GetDateTimeNow()
        {
            string sqlStr = "SELECT GETDATE()";
            using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
            {
                return conn.Query<DateTime>(sqlStr).SingleOrDefault();
            }
        }

        internal List<NotifyOrderEntity> QueryOrdersList(ScanOrderCondition condition)
        {
            string sql = "";
            try
            {
                const string table = @" dbo.tblOrders   WITH(NOLOCK) ";
                const string selectField = @"OrderId, OrderNo,  OrderStatus,{0} OutTime,SalesmanID,ProviderID,ProxyerID";
                string orderby = "OutTime" + " " + condition.OrderDirection;
                string sqlWhere = string.Join(" AND ", GetOrdersConditionList(condition).ToArray());              
                List<NotifyOrderEntity> list= CommonPage.GetData<NotifyOrderEntity, ScanOrderCondition>(table, string.Format(selectField, condition.OrderBy), orderby, sqlWhere, condition, out sql);
                DBLog.Process("", "", "", "JinRiDBQuery.QueryOrdersList", "", "获取订单信息", string.Format("sql:【{0}】，扫描查询条件ScanOrderCondition：【{1}】", sql, JsonConvert.SerializeObject(condition)), "Info");
                return list;              
            }
            catch (Exception ex)
            {
                DBLog.Process("", "", "", "JinRiDBQuery.QueryOrdersList", "", "获取订单信息", string.Format("sql:【{0}】，扫描查询条件ScanOrderCondition：【{1}】，获取订单信息异常，ex：{2}", sql, JsonConvert.SerializeObject(condition), ex.Message), "Fatal");
                List<NotifyOrderEntity> list = new List<NotifyOrderEntity>();
                return list;
            }          
        }
        private List<string> GetOrdersConditionList(ScanOrderCondition condition)
        {
            List<string> list = new List<string>();
            list.Add(" 1=1 ");
            list.Add(" OrderStatus = @Status  ");
            list.Add(" OutTime BETWEEN @StartTime AND @EndTime  ");
            list.Add(" OrderId > @ScanOrderIdInit  ");
            return list;
        }
    }
}

