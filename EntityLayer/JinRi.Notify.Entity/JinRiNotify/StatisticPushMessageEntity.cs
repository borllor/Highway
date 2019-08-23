using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Entity
{
    public class StatisticPushMessageEntity
    {
        public string NextPushTime { get; set; }
        public string LastModifyTime { get; set; }
        public decimal AvgPushTime { get; set; }
    }
}
