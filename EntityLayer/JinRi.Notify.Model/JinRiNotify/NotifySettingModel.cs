using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Model
{
    public class NotifySettingModel
    {

        public int NotifyId { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string Remark { get; set; }
        public string Memo { get; set; }
        public string ClassName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifyTime { get; set; }
    }
}
