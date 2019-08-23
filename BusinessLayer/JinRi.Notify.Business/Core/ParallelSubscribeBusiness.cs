using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.Frame;
using JinRi.Notify.ServiceModel;
using Newtonsoft.Json;
using JinRi.Notify.Utility;


namespace JinRi.Notify.Business
{
    public class ParallelSubscribeBusiness
    {
        private Dictionary<string, object[]> m_threadDic = new Dictionary<string, object[]>();
        private Dictionary<MessagePriorityEnum, RabbitMQBus> m_rabbitBusDic = new Dictionary<MessagePriorityEnum, RabbitMQBus>();
        private static readonly BuildMessageBusiness m_buildBus = new BuildMessageBusiness();
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(ParallelSubscribeBusiness));

        public void Parallel()
        {
            if (BuilderServiceSetting.SystemStatus != SystemStatusEnum.Initializing &&
                BuilderServiceSetting.SystemStatus != SystemStatusEnum.Stopping &&
                BuilderServiceSetting.SystemStatus != SystemStatusEnum.Stopped)
            {
                if (m_rabbitBusDic.Count == 0)
                {
                    BuilderServiceSetting.SystemStatus = SystemStatusEnum.Initializing;

                    List<BuilderServiceSetting.ParallelSubscribeSetting> list = BuilderServiceSetting.ParallelSubscribeSettingList;
                    Array enumArr = Enum.GetValues(typeof(MessagePriorityEnum));
                    if (list != null && list.Count > 0)
                    {
                        MessagePriorityEnum priority;
                        List<MessagePriorityEnum> priorityList = new List<MessagePriorityEnum>();
                        foreach (BuilderServiceSetting.ParallelSubscribeSetting setting in list)
                        {
                            if (Enum.TryParse<MessagePriorityEnum>(setting.MessagePriority, out priority))
                            {
                                priorityList.Add(priority);
                            }
                        }
                        enumArr = priorityList.ToArray();
                    }
                    foreach (MessagePriorityEnum e in enumArr)
                    {
                        if (e != MessagePriorityEnum.None)
                        {
                            Process.Debug("并行生成消息", "Parallel", string.Format("消息优先级：【{0}】", e.ToString()), "");
                            RabbitMQBus bus = new RabbitMQBus();
                            m_rabbitBusDic.Add(e, bus);
                            Subscribe(bus, e);
                        }
                    }
                    BuilderServiceSetting.SystemStatus = SystemStatusEnum.Initialized;
                }
                BuilderServiceSetting.SystemStatus = SystemStatusEnum.Running;
            }
        }

        public void CloseRabbitMQBus()
        {
            if (m_rabbitBusDic != null && m_rabbitBusDic.Count > 0)
            {
                foreach (KeyValuePair<MessagePriorityEnum, RabbitMQBus> kv in m_rabbitBusDic)
                {
                    kv.Value.Close();
                }
            }
        }

        private void Subscribe(RabbitMQBus bus, MessagePriorityEnum priority)
        {
            Process.Debug("并行生成消息", "Subscribe", string.Format("开始订阅，消息优先级：【{0}】", priority.ToString()), "");
            RabbitMQBusiness.Instance.Subscribe(bus, priority, m_buildBus.Build);
        }
    }
}