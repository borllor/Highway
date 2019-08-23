using JinRi.App.Framework;

namespace JinRi.Notify.Data
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    internal class ConnectionString
    {
        #region JinRiNotify

        private static string m_JinRiNotifyDB_SELECT;

        /// <summary>
        /// JinRi库只读连接
        /// </summary>
        public static string JinRiNotifyDB_SELECT
        {
            get
            {
                if (string.IsNullOrEmpty(m_JinRiNotifyDB_SELECT))
                {
                    m_JinRiNotifyDB_SELECT = ConnectionStringFactory.CreateConnectionString(DatabaseEnum.JinRiNotifyDB_SELECT);
                }
                return m_JinRiNotifyDB_SELECT;
            }
        }
        private static string m_JinRiNotifyDB_CMD;

        /// <summary>
        /// JinRi库写连接
        /// </summary>
        public static string JinRiNotifyDB_CMD
        {
            get
            {
                if (string.IsNullOrEmpty(m_JinRiNotifyDB_CMD))
                {
                    m_JinRiNotifyDB_CMD = ConnectionStringFactory.CreateConnectionString(DatabaseEnum.JinRiNotifyDB_CMD);
                }
                return m_JinRiNotifyDB_CMD;
            }
        }

        #endregion
    }
}
