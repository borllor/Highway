using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Frame
{
    public abstract class DbHelper
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DbHelper));

        public static void SetTimeoutDefault()
        {
            Timeout = 60;
        }
        public static int Timeout = 60;
        public static IDbBase Provider = new DbBase();
        private static string ReturnValueName = "ReturnValue";

        public static int ExecuteNonQuery(DatabaseEnum database, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = Provider.CreateCommand();

            using (DbConnection conn = Provider.CreateConnection())
            {
                conn.ConnectionString = ConnectionStringFactory.CreateConnectionString(database);
                return ExecuteNonQuery(conn, cmdType, cmdText, commandParameters);
            }
        }

        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                using (DbConnection conn = Provider.CreateConnection())
                {
                    conn.ConnectionString = connectionString;
                    return ExecuteNonQuery(conn, cmdType, cmdText, commandParameters);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    string conStr = connectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static int ExecuteNonQuery(DbConnection connection, CommandType cmdType, string cmdText,
            params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                if (cmdType == CommandType.StoredProcedure)
                {
                    val = (int)cmd.Parameters[cmd.Parameters.Count - 1].Value;
                }
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                if (cmdType == CommandType.StoredProcedure)
                {
                    val = (int)cmd.Parameters[cmd.Parameters.Count - 1].Value;
                }
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                IDbConnection connection = trans.Connection;
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static DbDataReader ExecuteReader(DatabaseEnum database, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteReader(ConnectionStringFactory.CreateConnectionString(database), cmdType, cmdText, commandParameters);
        }

        public static DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = Provider.CreateCommand();
            DbConnection conn = Provider.CreateConnection();
            conn.ConnectionString = connectionString;
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (SqlException ex)
            {
                conn.Close();
                Logger.Fatal(ex.Translate(), ex);
                throw;
            }
            catch (Exception ex)
            {
                conn.Close();
                if (!string.IsNullOrEmpty(connectionString))
                {
                    string conStr = connectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
            finally
            {
                //conn.Close();
            }
        }

        public static DbDataReader ExecuteReader(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception ex)
            {
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
        }
        }

        public static object ExecuteScalar(DatabaseEnum database, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteScalar(ConnectionStringFactory.CreateConnectionString(database), cmdType, cmdText, commandParameters);
        }

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] commandParameters)
        {
            DbCommand cmd = Provider.CreateCommand();
            try
            {
                using (DbConnection connection = Provider.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    string conStr = connectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
            finally
            {
                if (cmd != null && cmd.Parameters != null) cmd.Parameters.Clear();
            }
        }

        public static object ExecuteScalar(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                IDbConnection connection = trans.Connection;
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static DataTable ExecuteTable(DatabaseEnum database, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteTable(ConnectionStringFactory.CreateConnectionString(database), cmdType, cmdText, commandParameters);
        }

        public static DataTable ExecuteTable(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                using (DbConnection connection = Provider.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    DbDataAdapter ap = Provider.CreateDataAdapter();
                    ap.SelectCommand = cmd;
                    DataSet st = new DataSet();
                    ap.Fill(st, "Result");
                    cmd.Parameters.Clear();
                    return st.Tables["Result"];
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    string conStr = connectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static DataSet ExecuteDataset(DatabaseEnum databaseName, string spName, params object[] parameterValues)
        {
            string connectionString = ConnectionStringFactory.CreateConnectionString(databaseName);
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, (SqlParameter[])parameterValues);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                command.Parameters.Clear();

                //CloseSqlObject(null, command);

                return dataSet;
            }
            catch (Exception ex)
            {
                IDbConnection connection = transaction.Connection;
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", commandText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static DataTable ExecuteDataTable(DatabaseEnum databaseName, CommandType commandType, string commandText, params SqlParameter[] parms)
        {
            DataSet ds = ExecuteDataset( ConnectionStringFactory.CreateConnectionString(databaseName), commandType, commandText, parms);
            if (ds != null
                && ds.Tables[0] != null)
            {
                return ds.Tables[0];
            }

            return null;
        }

        public static DataSet ExecuteDataset(DatabaseEnum databaseName, CommandType commandType, string commandText, params SqlParameter[] parames)
        {
            return ExecuteDataset(ConnectionStringFactory.CreateConnectionString(databaseName), commandType, commandText, parames);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                command.Parameters.Clear();

                return dataSet;
            }
            catch (Exception ex)
            {
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", commandText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        public static DataTable ExecuteTable(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            try
            {
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                DbDataAdapter ap = Provider.CreateDataAdapter();
                ap.SelectCommand = cmd;
                DataSet st = new DataSet();
                ap.Fill(st, "Result");
                cmd.Parameters.Clear();
                return st.Tables["Result"];
            }
            catch (Exception ex)
            {
                if (connection != null && !string.IsNullOrEmpty(connection.ConnectionString))
                {
                    string conStr = connection == null ? "dbConnection为空" : string.IsNullOrEmpty(connection.ConnectionString) == true ? "ConnectionString为空" : connection.ConnectionString;
                    string errorMsg = string.Format("{0};{1};{2};{3};{4};{5}", cmdText, commandParameters.GetParamKeyValues(), connection == null ? conStr : conStr.Substring(0, Regex.Matches(conStr, ";")[1].Index), ex.Message, ex.Source, ex.StackTrace);
                    Logger.Fatal(errorMsg, ex);
                    DBLog.Process("", "", ClientHelper.GetClientIP(), "", "", "sql_exception", errorMsg);
                }
                throw ex;
            }
            }

        public static object ExecuteDataObject(DatabaseEnum databaseName, CommandType commandType, string commandText, params SqlParameter[] parms)
        {
            DataSet ds = ExecuteDataset(ConnectionStringFactory.CreateConnectionString(databaseName), commandType, commandText, parms);
            if (ds != null
                && ds.Tables[0] != null)
            {
                return ds.Tables[0].Rows[0][0];
            }

            return "";
        }

        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    Logger.Fatal(ex.Translate(), ex);
                    throw;
                }
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            cmd.CommandTimeout = Timeout;

            if (cmdParms != null)
            {
                foreach (DbParameter parm in cmdParms)
                    if (parm != null)
                        cmd.Parameters.Add(parm);
            }

            if (cmdType == CommandType.StoredProcedure)
            {
                DbParameter para = Provider.CreateParameter();
                para.ParameterName = ReturnValueName;
                para.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(para);
            }
        }

        #region 分页查询算法

        /// <summary>
        /// 执行对默认数据库有自定义排序的分页的查询
        /// </summary>
        /// <param name="connectionString">连接字符串
        /// <param name="SqlAllFields">查询字段，如果是多表查询，请将必要的表名或别名加上，如:a.id,a.name,b.score</param>
        /// <param name="SqlTablesAndWhere">查询的表如果包含查询条件，也将条件带上，但不要包含order by子句，也不要包含"from"关键字，如:students a inner join achievement b on a.... where ....</param>
        /// <param name="IndexField">用以分页的不能重复的索引字段名，最好是主表的自增长字段，如果是多表查询，请带上表名或别名，如:a.id</param>
        /// <param name="OrderASC">排序方式,如果为true则按升序排序,false则按降序排</param>
        /// <param name="OrderFields">排序字段以及方式如：a.OrderID desc,CnName desc</OrderFields>
        /// <param name="PageIndex">当前页的页码</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="RecordCount">输出参数，返回查询的总记录条数</param>
        /// <param name="PageCount">输出参数，返回查询的总页数</param>
        /// <returns>返回查询结果</returns>
        public static DbDataReader ExecuteReaderPage(string connectionString, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string GroupClause, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            DbConnection conn = Provider.CreateConnection();
            conn.ConnectionString = connectionString;
            try
            {
                conn.Open();
                DbCommand cmd = Provider.CreateCommand();
                PrepareCommand(cmd, conn, null, CommandType.Text, "", commandParameters);
                string Sql = GetPageSql(conn, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out  RecordCount, out  PageCount);
                if (GroupClause != null && GroupClause.Trim() != "")
                {
                    int n = Sql.ToLower().LastIndexOf(" order by ");
                    Sql = Sql.Substring(0, n) + " " + GroupClause + " " + Sql.Substring(n);
                }
                cmd.CommandText = Sql;
                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch(Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                throw ex;
            }
        }


        /// <summary>
        /// 执行有自定义排序的分页的查询
        /// </summary>
        /// <param name="connectionString">SQL数据库连接字符串</param>
        /// <param name="SqlAllFields">查询字段，如果是多表查询，请将必要的表名或别名加上，如:a.id,a.name,b.score</param>
        /// <param name="SqlTablesAndWhere">查询的表如果包含查询条件，也将条件带上，但不要包含order by子句，也不要包含"from"关键字，如:students a inner join achievement b on a.... where ....</param>
        /// <param name="IndexField">用以分页的不能重复的索引字段名，最好是主表的自增长字段，如果是多表查询，请带上表名或别名，如:a.id</param>
        /// <param name="OrderASC">排序方式,如果为true则按升序排序,false则按降序排</param>
        /// <param name="OrderFields">排序字段以及方式如：a.OrderID desc,CnName desc</OrderFields>
        /// <param name="PageIndex">当前页的页码</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="RecordCount">输出参数，返回查询的总记录条数</param>
        /// <param name="PageCount">输出参数，返回查询的总页数</param>
        /// <returns>返回查询结果</returns>
        public static DataTable ExecutePage(string connectionString, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            using (DbConnection connection = Provider.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                return ExecutePage(connection, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out RecordCount, out PageCount, commandParameters);
            }
        }

        /// <summary>
        /// 执行有自定义排序的分页的查询
        /// </summary>
        /// <param name="connection">SQL数据库连接对象</param>
        /// <param name="SqlAllFields">查询字段，如果是多表查询，请将必要的表名或别名加上，如:a.id,a.name,b.score</param>
        /// <param name="SqlTablesAndWhere">查询的表如果包含查询条件，也将条件带上，但不要包含order by子句，也不要包含"from"关键字，如:students a inner join achievement b on a.... where ....</param>
        /// <param name="IndexField">用以分页的不能重复的索引字段名，最好是主表的自增长字段，如果是多表查询，请带上表名或别名，如:a.id</param>
        /// <param name="OrderASC">排序方式,如果为true则按升序排序,false则按降序排</param>
        /// <param name="OrderFields">排序字段以及方式如：a.OrderID desc,CnName desc</OrderFields>
        /// <param name="PageIndex">当前页的页码</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="RecordCount">输出参数，返回查询的总记录条数</param>
        /// <param name="PageCount">输出参数，返回查询的总页数</param>
        /// <returns>返回查询结果</returns>
        public static DataTable ExecutePage(DbConnection connection, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            DbCommand cmd = Provider.CreateCommand();
            PrepareCommand(cmd, connection, null, CommandType.Text, "", commandParameters);
            string Sql = GetPageSql(connection, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out  RecordCount, out  PageCount);
            cmd.CommandText = Sql;
            DbDataAdapter ap = Provider.CreateDataAdapter();
            ap.SelectCommand = cmd;
            DataSet st = new DataSet();
            ap.Fill(st, "PageResult");
            cmd.Parameters.Clear();
            return st.Tables["PageResult"];
        }
        /// <summary>
        /// 取得分页的SQL语句
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmd"></param>
        /// <param name="SqlAllFields"></param>
        /// <param name="SqlTablesAndWhere"></param>
        /// <param name="IndexField"></param>
        /// <param name="OrderFields"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="RecordCount"></param>
        /// <param name="PageCount"></param>
        /// <returns></returns>
        private static string GetPageSql(DbConnection connection, DbCommand cmd, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount)
        {
            RecordCount = 0;
            PageCount = 0;
            if (PageSize <= 0)
            {
                PageSize = 10;
            }
            string SqlCount = "select count(" + IndexField + ") from " + SqlTablesAndWhere;
            cmd.CommandText = SqlCount;
            RecordCount = (int)cmd.ExecuteScalar();
            if (RecordCount % PageSize == 0)
            {
                PageCount = RecordCount / PageSize;
            }
            else
            {
                PageCount = RecordCount / PageSize + 1;
            }
            if (PageIndex > PageCount)
                PageIndex = PageCount;
            if (PageIndex < 1)
                PageIndex = 1;
            string Sql = null;
            if (PageIndex == 1)
            {
                Sql = "select top " + PageSize + " " + SqlAllFields + " from " + SqlTablesAndWhere + " " + OrderFields;
            }
            else
            {
                Sql = "select top " + PageSize + " " + SqlAllFields + " from ";
                if (SqlTablesAndWhere.ToLower().IndexOf(" where ") > 0)
                {
                    string _where = Regex.Replace(SqlTablesAndWhere, @"\ where\ ", " where (", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    Sql += _where + ") and (";
                }
                else
                {
                    Sql += SqlTablesAndWhere + " where (";
                }
                Sql += IndexField + " not in (select top " + (PageIndex - 1) * PageSize + " " + IndexField + " from " + SqlTablesAndWhere + " " + OrderFields;
                Sql += ")) " + OrderFields;
            }
            return Sql;
        }

        #endregion
    }

    public static class DBParametersExtension
    {
        public static string GetParamKeyValues(this DbParameter[] parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return "";
            }

            StringBuilder sbRes = new StringBuilder("(");
            foreach (DbParameter dp in parameters)
            {
                sbRes.Append(string.Format("{0}:{1}", dp.ParameterName, dp.Value)).Append("|");
            }
            sbRes.Append(")");
            return sbRes.ToString();
        }
    }
}