using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace JinRi.Notify.Utility
{
    public static class Check
    {
        public static void IsNull(object val, string key)
        {
            if (val == null)
            {
                throw new BusinessException(string.Format("{0}不能为空", key));
            }
        }

        public static void IsNullOrEmpty(string val, string key)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new BusinessException(string.Format("{0}不能为空", key));
            }
        }

        public static void CanDeserializeObject<T>(string val, string key, out T output)
        {
            output = default(T);
            if (string.IsNullOrEmpty(val))
            {
                throw new BusinessException(string.Format("{0}不能为空", key));
            }
            else
            {
                try
                {
                    output = JsonConvert.DeserializeObject<T>(val);
                }
                catch
                {
                    throw new BusinessException(string.Format("【{0}】不是有效的Json字符串对象", val));
                }
            }
        }
    }
}
