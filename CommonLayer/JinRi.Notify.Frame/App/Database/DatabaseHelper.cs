using System;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using System.Data.Common;
using log4net;

namespace JinRi.Notify.Frame
{
    public class ConnectionHelper
    {
        private static readonly log4net.ILog _logger = LogManager.GetLogger(typeof(ConnectionHelper));
        public static string GetConnectionString(string keyName, string decodeKey)
        {
            try
            {
                string cacheKey = "JinRi_App_Framework_" + keyName;
                string cacheVal = DataCache.Get(cacheKey) as string;
                if (cacheVal == null)
                {
                    string strTmpValue = GetLocalConfigConnectionString(keyName);
                    if (string.IsNullOrEmpty(strTmpValue))
                    {
                        return string.Empty;
                    }
                    DataCache.Set(cacheKey, strTmpValue, DateTime.Today.AddDays(1));
                    return Decrypt(strTmpValue, decodeKey);
                }
                return Decrypt(cacheVal, decodeKey);
            }
            catch(Exception ex)
            {
                _logger.Error("GetConnectionString出现异常：" + ex.ToString());
                return string.Empty;
            }
        }

        public static DbConnection CreateConnection(string keyName, string decodeKey)
        {
            try
            {
                string cacheKey = "JinRi_App_Framework_" + keyName + decodeKey;
                ConnectionStringSettings setting = GetLocalConfigConnection(keyName);
                string cacheVal = DataCache.Get(cacheKey) as string;
                if (cacheVal == null)
                {
                    string strTmpValue = GetLocalConfigConnectionString(keyName);
                    if (string.IsNullOrEmpty(strTmpValue))
                    {
                        return null;
                    }
                    cacheVal = Decrypt(strTmpValue, decodeKey);
                    DataCache.Set(cacheKey, cacheVal, DateTime.Today.AddDays(1));
                }
                DbProviderFactory factory = DbProviderFactories.GetFactory(setting.ProviderName);
                DbConnection conn = factory.CreateConnection();
                conn.ConnectionString = cacheVal;
                return conn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 使用指定密钥解密
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        private static string Decrypt(string encrypted, string key)
        {
            Encoding encoding = Encoding.Default;
            byte[] encryptedBytes = Convert.FromBase64String(encrypted);
            byte[] keyBytes = Encoding.Default.GetBytes(key);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            des.Key = hashmd5.ComputeHash(keyBytes);
            hashmd5 = null;
            des.Mode = CipherMode.ECB;
            byte[] bytes = des.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return encoding.GetString(bytes);
        }

        private static string GetLocalConfigConnectionString(string keyName)
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[keyName];
            if (setting == null)
            {
                setting = ConfigurationManager.ConnectionStrings[keyName.Replace("_CMD", "").Replace("_SELECT", "")];
            }
            return setting.ConnectionString;
        }

        private static ConnectionStringSettings GetLocalConfigConnection(string keyName)
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[keyName];
            if (setting == null)
            {
                setting = ConfigurationManager.ConnectionStrings[keyName.Replace("_CMD", "").Replace("_SELECT", "")];
            }
            return setting;
        }
    }
}
