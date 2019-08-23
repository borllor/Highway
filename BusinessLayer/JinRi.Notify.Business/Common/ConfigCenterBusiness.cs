using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Business.Common
{
    /// <summary>
    /// TblWebconfig配置
    /// </summary>
    public static class ConfigCenterBusiness
    {
        #region StaticTime

        /// <summary>
        /// 锁
        /// </summary>
        static readonly object _lockObj;

        /// <summary>
        /// 随机256个byte数字
        /// </summary>
        static readonly byte[] _staticTimeArr;

        /// <summary>
        /// 取值索引
        /// </summary>
        static int _staticTimeIndex;

        /// <summary>
        /// 静态构造
        /// </summary>
        static ConfigCenterBusiness()
        {
            //初始化锁
            _lockObj = new object();

            //初始化索引
            _staticTimeIndex = 0;

            //获取随机数字数组
            _staticTimeArr = new byte[256];
            new Random().NextBytes(_staticTimeArr);
        }

        /// <summary>
        /// 获取一个随机数作为缓存时间
        /// 默认返回1-10之间的数字
        /// </summary>
        public static int StaticTime
        {
            get
            {
                return GetStaticTime(1, 10);
            }
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>满足范围的随机数</returns>
        public static int GetStaticTime(int minValue, int maxValue)
        {
            //保证输入大于0
            if (maxValue < 1)
            {
                maxValue = 1;
            }

            if (minValue < 1)
            {
                minValue = 1;
            }

            lock (_lockObj)
            {
                //当前取随机数索引递增
                _staticTimeIndex++;

                //每取255次就重新填充一下随机数组
                if (_staticTimeIndex >= 255)
                {
                    InitRandomArray();
                }

                //获取有效索引
                _staticTimeIndex = _staticTimeIndex % 255;

                //从随机数组中取值
                int tem = _staticTimeArr[_staticTimeIndex] % maxValue;

                //随机数映射
                int res = (int)((tem / 256.0) * (maxValue - minValue)) + minValue;
                return res;
            }
        }

        /// <summary>
        /// 初始化随机数组
        /// </summary>
        public static void InitRandomArray()
        {
            new Random().NextBytes(_staticTimeArr);
        }
        #endregion

        /// <summary>
        /// Web后台登录用户
        /// </summary>
        public static List<WebUserModel> WebUser
        {
            get
            {
                List<WebUserModel> users = new List<WebUserModel>();
                string webUserStr = new WebConfigBusiness().GetCacheValue(CacheKeys.WebUserCacheKey, "lixiaobo^li5811120");
                if (string.IsNullOrEmpty(webUserStr))
                {
                    return users;
                }
                webUserStr.Split('|').ToList().ForEach(t => {
                    
                    var _userStr =  t.Split('^');
                    if (_userStr.Length == 2)
                    {
                        WebUserModel model = new WebUserModel();
                        model.UserName = _userStr[0];
                        model.PassWord = _userStr[1];
                        users.Add(model);
                    }
                });
                return users;
            }
        }
    }
}
