using JinRi.Notify.Frame;

namespace JinRi.Notify.DB
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    internal class ConnectionString
    {
        #region Log4Net

        private static string m_Log4Net_CMD;
        private static string m_JinRi_SELECT;

        /// <summary>
        /// Log4Net库写连接
        /// </summary>
        public static string Log4Net_CMD
        {
            get
            {
                if (string.IsNullOrEmpty(m_Log4Net_CMD))
                {
                    m_Log4Net_CMD = ConnectionStringFactory.CreateConnectionString(DatabaseEnum.Log4Net_CMD);
                }
                return m_Log4Net_CMD;
            }
        }

        /// <summary>
        /// Log4Net库写连接
        /// </summary>
        public static string JinRiDB_SELECT
        {
            get
            {
                if (string.IsNullOrEmpty(m_JinRi_SELECT))
                {
                    m_JinRi_SELECT = ConnectionStringFactory.CreateConnectionString(DatabaseEnum.JinRiDB_SELECT);
                }
                return m_JinRi_SELECT;
            }
        }

        #endregion
    }
}
