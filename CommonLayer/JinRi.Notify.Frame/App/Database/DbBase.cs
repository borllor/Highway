using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// DbBase 的摘要说明
    /// </summary>
    public class DbBase : IDbBase
    {
        private DbProviderFactory m_provider = null;

        public DbBase()
        {
            m_provider = DbProviderFactories.GetFactory(AppSetting.ProviderName);
            DbHelper.Provider = this;
        }

        public DbCommand CreateCommand()
        {
            return new SqlCommand { CommandTimeout = 360 };
        }

        public DbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        public DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public DbParameter CreateParameter()
        {
            return new SqlParameter();
        }

        protected void AddParameter(List<DbParameter> list, DbParameter para)
        {
            if (list == null)
            {
                list = new List<DbParameter>();
            }
            list.Add(para);
        }

        protected void AddParameter(List<DbParameter> list, string name, object value)
        {
            DbParameter para = this.CreateParameter();
            para.ParameterName = name;
            para.Value = value;
            para.SourceColumn = name;
            AddParameter(list, para);
        }

        protected void AddParameter(List<DbParameter> list, string name, object value, object defaultValue)
        {
            AddParameter(list, name, name, value, defaultValue);
        }

        protected void AddParameter(List<DbParameter> list, string sourceColumn, string name, object value, object defaultValue)
        {
            DbParameter para = this.CreateParameter();
            para.ParameterName = name;
            para.Value = value ?? defaultValue;
            para.SourceColumn = sourceColumn;
            AddParameter(list, para);
        }
        protected void AddParameter(List<SqlParameter> parms, string p, object v)
        {
            parms.Add(new SqlParameter(p, v));
        }
    }
}
