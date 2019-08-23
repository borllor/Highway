using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel.Condition
{
    [Serializable]
    public class ScanOrderCondition : BaseCondition
    {
        public int ScanOrderIdInit { get; set; }
        public string Includes { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
