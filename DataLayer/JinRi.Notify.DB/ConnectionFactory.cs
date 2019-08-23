using System.Data.Common;
using JinRi.Notify.Frame;
using MySql.Data.MySqlClient;

namespace JinRi.Notify.DB
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    internal class ConnectionFactory
    {
        #region JinRiNotify


        /// <summary>
        /// JinRiNotify库只读连接
        /// </summary>
        public static DbConnection JinRiNotify_SELECT
        {
            get
            {
                return ConnectionStringFactory.CreateConnection(DatabaseEnum.JinRiNotify_SELECT);
            }
        }

        /// <summary>
        /// JinRiNotify库写连接
        /// </summary>
        public static DbConnection JinRiNotify_CMD
        {
            get
            {
                return ConnectionStringFactory.CreateConnection(DatabaseEnum.JinRiNotify_CMD);
            }
        }

        #endregion
    }
}
