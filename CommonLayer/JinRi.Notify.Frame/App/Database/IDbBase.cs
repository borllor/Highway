using System;
using System.Data;
using System.Data.Common;

namespace JinRi.Notify.Frame
{
    public interface IDbBase
    {
        DbCommand CreateCommand();
        DbConnection CreateConnection();
        DbDataAdapter CreateDataAdapter();
        DbParameter CreateParameter();
    }
}
