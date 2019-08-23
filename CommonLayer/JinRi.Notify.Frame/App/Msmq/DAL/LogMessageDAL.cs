using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
#if APPSERVER
using Npgsql;
using NpgsqlTypes;
#endif

namespace JinRi.Notify.Frame
{
    public class LogMessageDAL : DbBase
    {

        private static readonly string m_localServerIP = IPHelper.GetLocalIP();
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MsmqLogger));
        private readonly static LogMessageDAL m_LogMessageDAL = new LogMessageDAL();
        private IDataBufferPool _repeatBatchSaveHandlePool;
        private IDataBufferPool _repeatBatchSaveProcessPool;

#if APPSERVER
        private IDataBufferPool _repeatRescovryDataPool;
        private static string _failedLogMessageStorePath;
        static LogMessageDAL()
        {
            _failedLogMessageStorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogBack");
            if (!Directory.Exists(_failedLogMessageStorePath)) Directory.CreateDirectory(_failedLogMessageStorePath);
        }
#endif

        public LogMessageDAL()
        {
            _repeatBatchSaveHandlePool = new DataBufferPool(new WaitCallback(RepeatBatchSaveHandleLog), 10, false);
            _repeatBatchSaveProcessPool = new DataBufferPool(new WaitCallback(RepeatBatchSaveProcessLog), 12, false);
#if APPSERVER
            _repeatRescovryDataPool = new DataBufferPool(new WaitCallback(RepeatRescovryLog), 60, false);
            _repeatRescovryDataPool.Write(1);
#endif
        }

        public static LogMessageDAL GetInstance()
        {
            return m_LogMessageDAL;
        }

        public int Insert(object obj)
        {
            try
            {
                if (obj is LogMessageTaskInfo)
                {
                    return Insert((LogMessageTaskInfo)obj);
                }
                else
                {
                    if (obj is object[])
                    {
                        object[] objArr = (object[])obj;
                        if (objArr.Length == 2)
                        {
                            IEnumerator tor;
                            if (objArr[0] is List<object>)
                            {
                                tor = ((List<object>)objArr[0]).GetEnumerator();
                            }
                            else
                            {
                                tor = objArr[0] as IEnumerator;
                            }
                            bool isHandle = (bool)objArr[1];
                            return Insert(tor, isHandle, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("保存日志数据到数据库发生异常，异常:{0}", GetString(ex));
            }
            return 0;
        }

        public int Insert(IEnumerator tor, bool isHandle)
        {
            return Insert(tor, isHandle, false);
        }

        public int Insert(IEnumerator tor, bool isHandle, bool hasSendToMQWhileFailed)
        {
            return Insert(GetLogMessageTable(tor), isHandle, hasSendToMQWhileFailed);
        }

        public int Insert(DataTable table, bool isHandle, bool hasSendToMQWhileFailed)
        {
            string errMsg = "";
            int count = InsertInternal(table, isHandle, out errMsg);
            if (count == 0)
            {
                lock (m_repeatBatchSaveTimesLockObj)
                {
                    m_repeatBatchSaveTimes[table.TableName] = 1;
                }
                if (isHandle)
                {
                    _repeatBatchSaveHandlePool.Write(table);
                }
                else
                {
                    _repeatBatchSaveProcessPool.Write(table);
                }
                Logger.Fatal(string.Format("往{0}表插入数据出现严重错误，影响数据{1}条，出现错误：{2}", table.TableName, table.Rows.Count, errMsg));
            }
            return count;
        }

        public int InsertInternal(DataTable table, bool isHandle, out string errMsg)
        {
            int count = 0;
            string tableName = "";
            errMsg = "";
            try
            {
                using (SqlBulkCopy bult =
                     new SqlBulkCopy(ConnectionStringFactory.CreateConnectionString(DatabaseEnum.Log4Net_CMD))
                     {
                         BatchSize = 800
                     })
                {

                    tableName = "dbo.tbl_Interface_ProcessLog";
                    if (isHandle)
                    {
                        tableName = "dbo.tbl_Interface_HandleLog";
                    }
                    tableName = string.Format("{0}{1}", tableName, GetLogTableSuffix());

                    bult.ColumnMappings.Add("IKey", "ikey");
                    bult.ColumnMappings.Add("Username", "username");
                    bult.ColumnMappings.Add("LogTime", "logtime");
                    bult.ColumnMappings.Add("ClientIP", "clientip");
                    bult.ColumnMappings.Add("Module", "module");
                    bult.ColumnMappings.Add("OrderNo", "orderno");
                    bult.ColumnMappings.Add("LogType", "logtype");
                    bult.ColumnMappings.Add("Content", "content");
                    bult.ColumnMappings.Add("ServerIP", "ServerIP");
                    bult.ColumnMappings.Add("Keyword", "KeyWord");

                    bult.DestinationTableName = tableName;
                    bult.WriteToServer(table);
                    count = table.Rows.Count;
                    Logger.Debug(string.Format("批量成功往{0}-{1}表插入{2}条数据", tableName, table.TableName, count));
                }
            }
            catch (Exception ex)
            {
#if APPSERVER
                //插入失败后写入文件，暂存，文件名为 表的名字.json
                File.AppendAllText(GetBackFileName(table.TableName, isHandle), Newtonsoft.Json.JsonConvert.SerializeObject(table));
#endif
                errMsg = GetString(ex);
                count = 0;
            }

#if APPSERVER
            //ThreadPool.QueueUserWorkItem((state) =>
            //{
            //    string msg = "";
            //    object[] arr = state as object[];
            //    LogMessageDAL d = arr[0] as LogMessageDAL;
            //    int cnt = d.InsertPostgres((DataTable)arr[1], (bool)arr[2], out msg);
            //    if (cnt == 0 && !string.IsNullOrEmpty(msg))
            //    {
            //        Logger.Error(string.Format("Postgresql批量失败，失败原因：{0}", msg));
            //    }
            //    else
            //    {
            //        Logger.Debug(string.Format("Postgresql批量成功，成功条数：{0}", cnt));
            //    }
            //}, new object[] { this, table, isHandle });
#endif
            return count;
        }

#if APPSERVER
        private int InsertPostgres(DataTable table, bool isHandle, out string errMsg)
        {
            errMsg = "";
            int count = 0;
            string tableName = "tbl_Interface_ProcessLog";
            if (isHandle)
            {
                tableName = "tbl_Interface_HandleLog";
            }

            NpgsqlConnection conn = new NpgsqlConnection(ConnectionStringFactory.CreateConnectionString(DatabaseEnum.PGLog4Net_CMD));
            try
            {
                conn.Open();
                DataSet dataSet = new DataSet();
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(string.Format("SELECT * FROM {0} WHERE id = -1", tableName), ConnectionStringFactory.CreateConnectionString(DatabaseEnum.PGLog4Net_CMD));
                dataAdapter.InsertCommand = new NpgsqlCommand(string.Format("INSERT INTO {0}(ikey, username, logtime, clientip, module, orderno, logtype, content, serverip, keyword) ", tableName) +
                                        " values (:ikey, :username, :logtime, :clientip, :module, :orderno, :logtype, :content, :serverip, :keyword)", conn);
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("ikey", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("username", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("logtime", NpgsqlDbType.Timestamp));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("clientip", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("module", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("orderno", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("logtype", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("content", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("serverip", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter("keyword", NpgsqlDbType.Varchar));
                dataAdapter.InsertCommand.Parameters[0].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[1].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[2].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[3].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[4].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[5].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[6].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[7].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[8].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[9].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[0].SourceColumn = "ikey";
                dataAdapter.InsertCommand.Parameters[1].SourceColumn = "username";
                dataAdapter.InsertCommand.Parameters[2].SourceColumn = "logtime";
                dataAdapter.InsertCommand.Parameters[3].SourceColumn = "clientip";
                dataAdapter.InsertCommand.Parameters[4].SourceColumn = "module";
                dataAdapter.InsertCommand.Parameters[5].SourceColumn = "orderno";
                dataAdapter.InsertCommand.Parameters[6].SourceColumn = "logtype";
                dataAdapter.InsertCommand.Parameters[7].SourceColumn = "content";
                dataAdapter.InsertCommand.Parameters[8].SourceColumn = "serverip";
                dataAdapter.InsertCommand.Parameters[9].SourceColumn = "keyword";

                dataAdapter.Fill(dataSet);

                DataTable newTable = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    DataRow newRow = newTable.NewRow();
                    newRow["ikey"] = row["ikey"];
                    newRow["username"] = row["username"];
                    newRow["logtime"] = row["logtime"];
                    newRow["clientip"] = row["clientip"];
                    newRow["serverip"] = row["serverip"];
                    newRow["module"] = row["module"];
                    newRow["keyword"] = row["keyword"];
                    newRow["orderno"] = row["orderno"];
                    newRow["logtype"] = row["logtype"];
                    newRow["content"] = row["content"];
                    newTable.Rows.Add(newRow);
                }
                DataSet ds2 = dataSet.GetChanges();
                count = dataAdapter.Update(ds2);
                dataSet.Merge(ds2);
                dataSet.AcceptChanges();
            }
            catch (Exception ex)
            {
                errMsg = GetString(ex);
                count = 0;
            }
            finally
            {
                conn.Close();
            }

            return count;
        }

        private void RepeatRescovryLog(object state)
        {
            try 
	        {	        
                string[] filenameArr = Directory.GetFiles(_failedLogMessageStorePath, "*.json");
                if (filenameArr != null && filenameArr.Length > 0)
                {
                    foreach (string filename in filenameArr)
                    {
                        bool isHandle = filename.Substring(filename.Length - 6, 1) == "1" ? true : false;
                        DataTable table = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(File.ReadAllText(filename));
                        string key = table.TableName;
                        int count = m_repeatBatchSaveTimes[key];
                        string errMsg = "";
                        bool ret = InsertInternal(table, isHandle, out errMsg) > 0;
                        Logger.Info(string.Format("从备份文件补偿保存日志结果：【{0}】，errMsg：{1}", ret, errMsg));
                        if (ret)
                        {
                            DeleteBackFile(table.TableName, isHandle);
                        }
                        Thread.Sleep(10);
                    }
                }
            }
	        catch (Exception ex)
	        {
                Logger.Info(string.Format("从备份文件补偿保存日志产生异常：{0}", GetString(ex)));
            }
            finally
            {
                _repeatRescovryDataPool.Write(1);
            }
        }

        private void DeleteBackFile(string key, bool isHandle)
        {
            string filename = GetBackFileName(key, isHandle);
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("删除{0}文件失败，{1}", filename, GetString(ex)));
            }
        }

        private string GetBackFileName(string key, bool isHandle)
        {
            return Path.Combine(_failedLogMessageStorePath, string.Format("{0}.{1}.json", key, isHandle ? 1 : 0));
        }
#endif

        public static DataTable QueryLogDB(string sql)
        {
            try
            {
                DataSet dataset = new DataSet();
                SqlConnection conn = new SqlConnection(ConnectionStringFactory.CreateConnectionString(DatabaseEnum.Log4Net_CMD));
                SqlCommand command = conn.CreateCommand();
                command.CommandText = sql;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataset);
                return dataset.Tables[0];
            }
            catch
            {
                return null;
            }
        }

        private static object m_repeatBatchSaveTimesLockObj = new object();
        private static Dictionary<string, int> m_repeatBatchSaveTimes = new Dictionary<string, int>(100);

        private void RepeatBatchSaveHandleLog(object dataBuffer)
        {
            RepeatBatchSaveLog(dataBuffer, true);
        }

        private void RepeatBatchSaveProcessLog(object dataBuffer)
        {
            RepeatBatchSaveLog(dataBuffer, false);
        }

        private void RepeatBatchSaveLog(object dataBuffer, bool isHandle)
        {
            string errMsg = "";
            IDataBuffer buffer = dataBuffer as IDataBuffer;
            if (buffer != null && buffer.Count > 0)
            {
                List<DataTable> list = buffer.GetList<DataTable>();
                foreach (DataTable table in list)
                {
                    string key = table.TableName;
                    int count = m_repeatBatchSaveTimes[key];
                    bool ret = InsertInternal(table, isHandle, out errMsg) > 0;
                    if (!ret)
                    {
                        if (count < 5)
                        {
                            m_repeatBatchSaveTimes[key] = count + 1;
                            if (isHandle)
                            {
                                _repeatBatchSaveHandlePool.Write(table);
                            }
                            else
                            {
                                _repeatBatchSaveProcessPool.Write(table);
                            }
                        }
                        else
                        {
                            lock (m_repeatBatchSaveTimesLockObj)
                            {
                                m_repeatBatchSaveTimes.Remove(key);
                            }

                            Logger.Info(string.Format("日志补偿保存，表名：【{0}】，更新次数：【{1}】，更新结果：失败-超过补偿次数，影响数据条数：【{2}】", key, count + 1, table.Rows.Count));

                        }
                        Logger.Info(string.Format("日志补偿保存，表名：【{0}】，更新次数：【{1}】，更新结果：【{2}】", key, count + 1, ret));
                    }
                    else
                    {
                        lock (m_repeatBatchSaveTimesLockObj)
                        {
                            m_repeatBatchSaveTimes.Remove(key);
                        }
#if APPSERVER
                        DeleteBackFile(table.TableName, isHandle);
#endif
                    }
                    Thread.Sleep(100);
                }
            }
        }

        private int Insert(LogMessage log)
        {
            int ret = 0;
            string tableName = "";
            try
            {
                tableName = "dbo.tbl_Interface_ProcessLog";
                if (log.IsHandle)
                {
                    tableName = "dbo.tbl_Interface_HandleLog";
                }
                tableName = string.Format("{0}{1}", tableName, LogMessageDAL.GetLogTableSuffix());
                string sql = string.Format(@"insert into {0}(IKey, Username, LogTime, ClientIP, ServerIP, Module, Keyword, OrderNo, LogType, Content) 
                    values (@IKey, @Username, @LogTime, @ClientIP, @ServerIP, @Module, @Keyword, @OrderNo, @LogType, @Content)", tableName);

                List<DbParameter> paraList = new List<DbParameter>();
                AddParameter(paraList, "@IKey", log.Ikey);
                AddParameter(paraList, "@Username", log.Username);
                AddParameter(paraList, "@LogTime", log.LogTime);
                AddParameter(paraList, "@ClientIP", log.ClientIP);
                AddParameter(paraList, "@ServerIP", log.ServerIP);
                AddParameter(paraList, "@Module", log.Module);
                AddParameter(paraList, "@Keyword", log.Keyword);
                AddParameter(paraList, "@OrderNo", log.OrderNo);
                AddParameter(paraList, "@LogType", log.LogType);
                AddParameter(paraList, "@Content", log.Content);

                ret = DbHelper.ExecuteNonQuery(DatabaseEnum.Log4Net_CMD, CommandType.Text, sql, paraList.ToArray());
            }
            catch (Exception ex)
            {
                //丢弃日志
                Logger.Fatal(string.Format("往{0}表插入数据出现严重错误，{1}", tableName, log.ToString()), ex);
            }
            return ret;
        }

        private DataTable GetLogMessageTable(IEnumerator tor)
        {
            DataTable table = new DataTable(Guid.NewGuid().ToString("N"));

            #region 构建列

            DataColumn col = new DataColumn("IKey", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 100;
            table.Columns.Add(col);

            col = new DataColumn("Username", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 50;
            table.Columns.Add(col);

            col = new DataColumn("LogTime", typeof(DateTime));
            col.DefaultValue = DateTime.Now;
            table.Columns.Add(col);

            col = new DataColumn("ClientIP", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 50;
            table.Columns.Add(col);

            col = new DataColumn("ServerIP", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 25;
            table.Columns.Add(col);

            col = new DataColumn("Module", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 255;
            table.Columns.Add(col);

            col = new DataColumn("Keyword", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 255;
            table.Columns.Add(col);

            col = new DataColumn("OrderNo", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 50;
            table.Columns.Add(col);

            col = new DataColumn("LogType", typeof(String));
            col.DefaultValue = "";
            col.MaxLength = 100;
            table.Columns.Add(col);

            col = new DataColumn("Content", typeof(String));
            col.DefaultValue = "";
            table.Columns.Add(col);

            #endregion

            while (tor.MoveNext())
            {
                LogMessage log = tor.Current as LogMessage;
                if (log != null)
                {
                    DataRow newRow = table.NewRow();
                    newRow["IKey"] = GetDefaultString(GetSubstring(log.Ikey, newRow.Table.Columns["IKey"].MaxLength), "noIkey");
                    newRow["Username"] = GetDefaultString(GetSubstring(log.Username, newRow.Table.Columns["Username"].MaxLength), "nouser");
                    newRow["LogTime"] = log.LogTime;
                    newRow["ClientIP"] = GetDefaultString(GetSubstring(log.ClientIP, newRow.Table.Columns["ClientIP"].MaxLength), "noip");
                    newRow["ServerIP"] = GetDefaultString(GetSubstring(log.ServerIP, newRow.Table.Columns["ServerIP"].MaxLength), "noip");
                    //newRow["ServerIP"] = GetDefaultString(GetSubstring(m_localServerIP, newRow.Table.Columns["ServerIP"].MaxLength), "noip");

                    newRow["Module"] = GetDefaultString(GetSubstring(log.Module, newRow.Table.Columns["Module"].MaxLength), "nomodule");
                    newRow["Keyword"] = GetDefaultString(GetSubstring(log.Keyword, newRow.Table.Columns["Keyword"].MaxLength), "nokeyword");
                    newRow["OrderNo"] = GetDefaultString(GetSubstring(log.OrderNo, newRow.Table.Columns["OrderNo"].MaxLength), "noorderno");
                    newRow["LogType"] = GetDefaultString(GetSubstring(log.LogType, newRow.Table.Columns["LogType"].MaxLength), "nologtype");
                    newRow["Content"] = GetDefaultString(log.Content, "nocontent");
                    table.Rows.Add(newRow);
                }
            }

            return table;
        }

        public static string GetLogTableSuffix()
        {
            string cacheKey = "JinRi.Notify.Frame.LogMessageDAL.TableSuffix";
            string cacheVal = DataCache.Get(cacheKey) as string;
            if (cacheVal == null)
            {
                DateTime now = GetLogServerTime();
                int leftSeconds = 60;
                cacheVal = string.Format("_{0:yyyyMM}", now);
                DateTime monEnd = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
                monEnd.AddMonths(1);
                if ((monEnd - now).TotalSeconds < leftSeconds)
                {
                    return cacheVal;
                }
                DataCache.Set(cacheKey, cacheVal, monEnd.AddSeconds(-1 * leftSeconds));
            }
            return cacheVal;
        }

        public static DateTime GetLogServerTime()
        {
            return (DateTime)DbHelper.ExecuteScalar(DatabaseEnum.Log4Net_CMD, CommandType.Text, "select getdate()");
        }

        private string GetDefaultString(string input, string defaultValue)
        {
            return input ?? defaultValue;
        }

        private string GetSubstring(string input, int len)
        {
            if (!string.IsNullOrEmpty(input))
            {
                byte[] buff = System.Text.Encoding.GetEncoding("GB2312").GetBytes(input);
                if (buff.Length > len)
                {
                    input = System.Text.Encoding.GetEncoding("GB2312").GetString(buff, 0, len).Trim();
                }
            }
            return input;
        }

        public static string GetString(Exception ex)
        {
            try
            {
                if (ex == null)
                {
                    return "";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("ex.Message:").Append(ex.Message).Append("|");
                if (!string.IsNullOrEmpty(ex.Source))
                {
                    sb.Append("ex.Source:").Append((ex.Source ?? "")).Append("|");
                }
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    sb.Append("ex.StackTrace:").Append((ex.StackTrace ?? "")).Append("|");
                }
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.ToString()))
                {
                    sb.Append(GetString(ex.InnerException));
                }
                sb.Append(ex);
                return sb.ToString();
            }
            catch (Exception e)
            {
                return "二次异常：" + e + "|" + (ex == null ? "null" : ex.ToString());
            }
        }
    }
}
