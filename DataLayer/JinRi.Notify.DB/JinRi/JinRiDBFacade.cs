using JinRi.Notify.Entity;
using JinRi.Notify.Entity.JinRiDB;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.DB
{
    public class JinRiDBFacade
    {
        public static readonly JinRiDBFacade Instance = new JinRiDBFacade();
        /// <summary>
        /// 根据配置键获取配置实例
        /// </summary>
        /// <param name="settingKey">配置键</param>
        /// <returns></returns>
        public WebConfigEntity GetWebConfigBySettingKey(string settingKey)
        {
            return JinRiDBQuery.Instance.GetWebConfigBySettingKey(settingKey);
        }
        public List<NotifyOrderEntity> GetOrdersList(ScanOrderCondition condition)
        {
            return JinRiDBQuery.Instance.GetOrdersList(condition);
        }

        public List<NotifyOrderEntity> QueryOrdersList(ScanOrderCondition condition)
        {
            return JinRiDBQuery.Instance.QueryOrdersList(condition);
        }        

        public List<NotifyOrderEntity> GetOrdersListBuSao(ScanOrderCondition condition)
        {
            return JinRiDBQuery.Instance.GetOrdersListBuSao(condition);
        }

        public DateTime GetDateTimeNow()
        {
            return JinRiDBQuery.Instance.GetDateTimeNow();
        }
    }
}
