using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Utility
{
    public static class DBParametersExtension
    {
        public static string GetParamKeyValues(this IList<IDbDataParameter> parameters)
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
