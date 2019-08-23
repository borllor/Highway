using System.Data.Common;

namespace JinRi.Notify.Frame
{
    public class ConnectionStringFactory
    {
        public static string CreateConnectionString(DatabaseEnum databaseEnum)
        {
            return CreateConnectionString(databaseEnum.ToString());
        }
        public static string CreateConnectionString(string databaseEnum)
        {
            return ConnectionHelper.GetConnectionString(databaseEnum, "BeiJing#2008");
        }

        public static DbConnection CreateConnection(DatabaseEnum databaseEnum)
        {
            return CreateConnection(databaseEnum.ToString());
        }
        public static DbConnection CreateConnection(string databaseEnum)
        {
            return ConnectionHelper.CreateConnection(databaseEnum, "BeiJing#2008");
        }
    }
}
