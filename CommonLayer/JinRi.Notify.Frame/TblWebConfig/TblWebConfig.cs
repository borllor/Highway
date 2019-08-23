using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace JinRi.Framework.Configs.TblWebConfig
{
    public class TblWebConfig : DbBase
    {
        string JinRiDB { get; set; }
        const int TimeSpanForCache = 10;   //数据缓存的时间设置   bob   2009-12-29
        const string PrefixOfCacheKey = "JinRi.dbo.tblWebConfig.";   //数据缓存键的前缀   bob   2010-01-29

        public TblWebConfig(string jinriDb)
        {
            JinRiDB = ConnectionStringFactory.CreateConnectionString(jinriDb);
        }

        public string GetCacheValue(string settingKey, string defaultValue)
        {
            string result = defaultValue;
            string cacheSettingKey = PrefixOfCacheKey + settingKey;
            try
            {
                object cacheObj = HttpRuntime.Cache.Get(cacheSettingKey);
                if (cacheObj != null)
                {
                    result = cacheObj.ToString();
                }
                else
                {
                    result = GetValue(settingKey);
                    if (!string.IsNullOrEmpty(result))
                    {
                        HttpRuntime.Cache.Insert(cacheSettingKey, result, null, DateTime.Now.AddMinutes(TimeSpanForCache), TimeSpan.Zero);
                    }
                }
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }
        /// <summary>
        /// 编写事务：根据配置键获取配置实例
        /// 编写内容：根据配置键获取配置实例
        /// 编 写 人：bob
        /// 编写时间：2009-09-17
        /// </summary>
        public string GetValue(string settingKey)
        {
            var selectSQL = "SELECT TOP 1 SettingValue FROM dbo.tblWebConfig WITH(NOLOCK) WHERE SettingKey = @SettingKey";
            List<DbParameter> dbDataParameterList = new List<DbParameter>();
            AddParameter(dbDataParameterList, "@SettingKey", settingKey);
            var obj = DbHelper.ExecuteScalar(JinRiDB, CommandType.Text, selectSQL, dbDataParameterList.ToArray());
            return obj.ToString();
        }
    }
}
