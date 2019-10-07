using JinRi.Notify.Frame;
using JinRi.Notify.DB;
using JinRi.Notify.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JinRi.Notify.Business
{
    public class WebConfigBusiness
    {
        const string PrefixOfCacheKey = "JinRi.dbo.tblWebConfig.";   //数据缓存键的前缀

        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <returns></returns>
        public WebConfigEntity GetWebConfigBySettingKey(string settingKey)
        {
            // TODO
            return null;
        }

        #region   string 类型相关方法
        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <param name="defaultValue">缓存值</param>
        /// <returns></returns>
        private string GetValue(string settingKey, string defaultValue)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <param name="defaultValue">缓存值</param>
        /// <returns></returns>
        public string GetCacheValue(string settingKey, string defaultValue)
        {
            return GetCacheValue(settingKey, defaultValue, GetDefaultRandomCacheTime());
        }

        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <param name="defaultValue">缓存值</param>
        /// <param name="customTimeSpanForCache">缓存时间</param>
        /// <returns></returns>
        public string GetCacheValue(string settingKey, string defaultValue, int customTimeSpanForCache)
        {
            string result = defaultValue;
            string cacheSettingKey = PrefixOfCacheKey + settingKey;
            try
            {
                object cacheObj = DataCache.Get(cacheSettingKey);
                if (cacheObj != null)
                {
                    result = string.Format("{0}", cacheObj);
                }
                else
                {
                    result = GetValue(settingKey, defaultValue);
                    DataCache.Add(cacheSettingKey, result, DateTime.Now.AddMinutes(customTimeSpanForCache));
                }
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }
        #endregion

        #region   int 类型相关方法
        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <param name="defaultValue">缓存值</param>
        /// <returns></returns>
        private int GetValue(string settingKey, int defaultValue)
        {
            // TODO
            return 0;
        }

        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <param name="defaultValue">缓存值</param>
        /// <returns></returns>
        public int GetCacheValue(string settingKey, int defaultValue)
        {
            return GetCacheValue(settingKey, defaultValue, GetDefaultRandomCacheTime());
        }

        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <param name="settingKey">key</param>
        /// <param name="defaultValue">缓存值</param>
        /// <param name="customTimeSpanForCache">缓存时间(分钟)</param>
        /// <returns></returns>
        public int GetCacheValue(string settingKey, int defaultValue, int customTimeSpanForCache)
        {
            int result = defaultValue;
            string cacheSettingKey = PrefixOfCacheKey + settingKey;
            try
            {
                object cacheObj = DataCache.Get(cacheSettingKey);
                if (cacheObj != null)
                {
                    result = Convert.ToInt32(cacheObj);
                }
                else
                {
                    result = GetValue(settingKey, defaultValue);
                    DataCache.Add(cacheSettingKey, result, DateTime.Now.AddMinutes(customTimeSpanForCache));
                }
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }
        #endregion

        #region   bool 类型相关方法
        /// <summary>
        /// 编写事务：根据配置键获取配置实例
        /// 编写内容：根据配置键获取配置实例
        /// 编 写 人：bob
        /// 编写时间：2009-09-17
        /// </summary>
        private bool GetValue(string settingKey, bool defaultValue)
        {
            // TODO
            return false;
        }

        /// <summary>
        /// 编写事务：根据配置键获取配置实例
        /// 编写内容：根据配置键获取配置实例(带有cache)
        /// 编 写 人：bob
        /// 编写时间：2009-09-17
        /// 修 改 人：andy
        /// 修改时间：2009-09-24
        /// </summary>
        public bool GetCacheValue(string settingKey, bool defaultValue)
        {
            return GetCacheValue(settingKey, defaultValue, GetDefaultRandomCacheTime());
        }

        /// <summary>
        /// 编写事务：根据配置键获取配置实例
        /// 编写内容：根据配置键获取配置实例(带有cache)
        /// 编 写 人：bob
        /// 编写时间：2009-09-17
        /// 修 改 人：andy
        /// 修改时间：2009-09-24
        /// </summary>
        public bool GetCacheValue(string settingKey, bool defaultValue, int customTimeSpanForCache)
        {
            bool result = defaultValue;
            string cacheSettingKey = PrefixOfCacheKey + settingKey;
            try
            {
                object cacheObj = DataCache.Get(cacheSettingKey);
                if (cacheObj != null)
                {
                    result = Convert.ToBoolean(cacheObj);
                }
                else
                {
                    result = GetValue(settingKey, defaultValue);
                    DataCache.Add(cacheSettingKey, result, DateTime.Now.AddMinutes(customTimeSpanForCache));
                }
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }
        #endregion

        #region 随机数

        /// <summary>
        /// 数据缓存的时间设置(5-15分之间)
        /// </summary>
        /// <returns></returns>
        public int GetDefaultRandomCacheTime()
        {
            Random r = new Random();
            return r.Next(5, 15);
        }

        #endregion
    }
}
