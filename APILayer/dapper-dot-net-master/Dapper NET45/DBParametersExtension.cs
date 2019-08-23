using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    /// <summary>
    /// DBParametersExtension实现
    /// </summary>
    public static class DBParametersExtension
    {
        /// <summary>
        /// GetParamKeyValues实现
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetParamKeyValues(this IDataParameterCollection parameters)
        {
            if (parameters == null || parameters.Count <= 0)
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
