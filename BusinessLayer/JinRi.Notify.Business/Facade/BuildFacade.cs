using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;


namespace JinRi.Notify.Business
{
    public class BuildFacade
    {
        private static readonly ParallelSubscribeBusiness _subscribeBus = new ParallelSubscribeBusiness();
        private static readonly BuildMessageBusiness _buildBus = new BuildMessageBusiness();

        public void Build()
        {
            _subscribeBus.Parallel();
        }

        public void CloseRabbitMQBus()
        {
            _subscribeBus.CloseRabbitMQBus();
        }

        public NotifyMessageResult Receive(NotifyMessage message)
        {
            return _buildBus.Receive(message);
        }

        public NotifyMessageResult ReceiveList(List<NotifyMessage> list)
        {
            return _buildBus.ReceiveList(list);
        }

        public void FlushPool()
        {
            _buildBus.FlushPool();
        }
    }
}
