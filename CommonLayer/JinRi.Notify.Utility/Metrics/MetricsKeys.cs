using System;

namespace JinRi.Notify.Utility
{
    public class MetricsKeys
    {
        #region 接收消息

        public const string ReceiverService = "JinRi.Notify.ReceiverService.ReceiveMessage";
        public const string ReceiveHandler = "JinRi.Notify.ReceiverService.ReceiveHandler";

        #endregion

        #region RabbitMQ

        public const string RabbitMQ_Publish = "JinRi.Notify.RabbitMQ.Publish";
        public const string RabbitMQ_Subscribe = "JinRi.Notify.RabbitMQ.Subscribe";

        #endregion

        #region 生成消息

        public const string BuilderService_Build = "JinRi.Notify.BuilderService.Build";
        public const string BuilderService_DirectReceive = "JinRi.Notify.BuilderService.DirectReceive";

        #endregion

        #region 发送消息

        public const string SenderService_Receive = "JinRi.Notify.SenderService.Receive";
        public const string SenderService_Callback = "JinRi.Notify.SenderService.Callback";
        public const string SenderService_Send = "JinRi.Notify.SenderService.Send";

        #endregion

        #region 重扫服务

        public const string RedoService = "JinRi.Notify.RedoService";

        #endregion

        #region Cache

        public const string Cache_Error = "JinRi.Notify.Cache.Error";

        #endregion
    }
}
