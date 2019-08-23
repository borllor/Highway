using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Entity.JinRiDB
{
    public class NotifyOrderEntity
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public String OrderNo { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
        /// <summary>
        /// 订单状态(T-出票完成 F-暂不能出票 1-退款成功)
        /// </summary>
        public String ParameterStatus { get; set; }
        /// <summary>
        /// 出票时间OR暂不能时间OR退款时间
        /// </summary>
        public String OutTime { get; set; }

        /// <summary>
        /// 经销商ID
        /// </summary>
        public int SalesmanID { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int ProviderID { get; set; }
        /// <summary>
        /// 采购商ID
        /// </summary>
        public int ProxyerID { get; set; }

    }
}
