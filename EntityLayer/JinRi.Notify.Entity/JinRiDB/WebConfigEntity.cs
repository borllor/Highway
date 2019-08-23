using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Entity
{
    [Serializable]
    public class WebConfigEntity
    {
        /// <summary>
        /// key
        /// </summary>
        public String SettingKey { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        public String SettingValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        public DateTime Date { get; set; }
    }
}
