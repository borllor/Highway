using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using JinRi.Notify.ServiceModel.Condition;

namespace JinRi.Notify.DB
{
    public static class CommonPage
    {
        /// <summary>
        /// 通用分页程序
        /// </summary>
        /// <typeparam name="T">返回的实体</typeparam>
        /// <typeparam name="F">查询条件实体</typeparam>
        /// <param name="table">使用的表 请自行加上WITH(NOLOCK)</param>
        /// <param name="selectField">显示的字段和查询条件中用的字段</param>
        /// <param name="orderby">排序</param>
        /// <param name="sqlWhere">查询条件</param>
        /// <param name="condition">查询条件基类实体</param>
        /// <param name="count">查询总数</param>
        /// <returns>List<实体></returns>
        internal static List<T> GetData<T, F>(string table, string selectField, string orderby, string sqlWhere, F condition,out string sql)
            where T : class
            where F : BaseCondition
        {
            string strSql =
@"SELECT * FROM (
SELECT ROW_NUMBER() OVER(ORDER BY {3}) rownum,ta.*
FROM  (SELECT {0} FROM {1}) ta
WHERE {2}
) t
WHERE t.rownum BETWEEN @LowerBound AND  @UpperBound
";
            string strCountSql = @"SELECT count(1) cnt FROM (SELECT {0} FROM {1}) ta  WHERE {2}";
            strCountSql = string.Format(strCountSql, selectField, table, sqlWhere);
            strSql = string.Format(strSql, selectField, table, sqlWhere, orderby);
            sql = strSql;
            using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
            {
                condition.RecordCount = conn.ExecuteScalar<int>(strCountSql, condition);
                return conn.Query<T>(strSql, condition).ToList();
            }
        }

        internal static DataTable GetData<F>(string table, string selectField, string orderby, string sqlWhere, F condition, Func<IDataReader, DataTable> outTable)
            where F : BaseCondition
        {
            string strSql =
@"SELECT * FROM (
SELECT ROW_NUMBER() OVER(ORDER BY {3}) rownum,ta.*
FROM  (SELECT {0} FROM {1}) ta
WHERE {2}
) t
WHERE t.rownum BETWEEN @LowerBound AND  @UpperBound
";
            strSql = string.Format(strSql, selectField, table, sqlWhere, orderby);
            using (var conn = new SqlConnection(ConnectionString.JinRiDB_SELECT))
            {
                IDataReader dReader = conn.ExecuteReader(strSql, condition);
                if (outTable == null) return new DataTable();
                return outTable(dReader);
            }
        }
    }
}
