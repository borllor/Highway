using System;
using System.Data.SqlClient;
using System.Text;

namespace JinRi.Notify.Frame
{
    public static class SqlExceptionHelper
    {
        public static string Translate(this SqlException ex)
        {
            int i = 0;
            var errorMessages = new StringBuilder();
            for (i = 0; i <= ex.Errors.Count - 1; i++)
            {
                SqlError sqlError = ex.Errors[i];
                string filteredMessage = string.Empty;
                if (sqlError.Class == 16)
                {
                    if (sqlError.Number == 2812)
                    {
                        filteredMessage = "找不到存储过程";
                    }
                    else if (sqlError.Number == 208)
                    {
                        filteredMessage = "对象名无效";
                    }
                }
                switch (sqlError.Number)
                {
                    case 17:
                    case 53:
                        filteredMessage = "未找到或无法访问服务器";
                        break;
                    case 4060:
                        filteredMessage = "无效的数据库";
                        break;
                    case 18456:
                        filteredMessage = "数据库无法登陆";
                        break;
                    case 1205:
                        filteredMessage = "sql 死锁";
                        break;
                    default:
                        filteredMessage = ex.ToString();
                        break;
                }

                errorMessages.Append("<b>Index #:</b> " + i + "<br/>" + "<b>Source:</b> " + sqlError.Source + "<br/>" + "<b>Class:</b> " + sqlError.Class + "<br/>" + "<b>Number:</b> " +
                                     sqlError.Number + "<br/>" + "<b>Procedure:</b> " + sqlError.Procedure + "<br/>" + "<b>Message:</b> " + filteredMessage + "<br/>");
            }
            return errorMessages.ToString();
        }
    }
}
