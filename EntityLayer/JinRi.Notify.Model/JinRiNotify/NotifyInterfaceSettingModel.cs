using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JinRi.Notify.Entity;

namespace JinRi.Notify.Model
{
    public class NotifyInterfaceSettingModel
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public int SettingId { get; set; }
        /// <summary>
        /// 应用编号
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 消息类型名称（E文）
        /// </summary>
        public string MessageTypeEName { get; set; }

        /// <summary>
        /// 消息类型名称（中文）
        /// </summary>
        public string MessageTypeName { get; set; }
        /// <summary>
        /// 消息接口处理地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 请求方法 POST/GET
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 推送次数阀值
        /// </summary>
        public int PushLimitCount { get; set; }
        /// <summary>
        /// 失败推送规则0,5,15
        /// </summary>
        public string PushInternalRule { get; set; }

        public List<int> m_pushInternalRuleList;
        public List<int> PushInternalRuleList
        {
            get
            {
                if (m_pushInternalRuleList == null)
                {
                    List<int> tmpList = new List<int>();
                    string[] ruleArr = PushInternalRule.Split(new char[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string r in ruleArr)
                    {
                        int tmp = 0;
                        if (int.TryParse(r, out tmp))
                        {
                            tmpList.Add(tmp);
                        }
                    }
                    m_pushInternalRuleList = tmpList;
                }
                return m_pushInternalRuleList;
            }
        }
        /// <summary>
        /// 字符编码
        /// </summary>
        public string Encoding { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最新创建时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int Status { get; set; }

        public NotifyInterfaceSettingModel()
        {
        }
    }
}
