using System;
using System.Linq;
using System.Runtime.Serialization;

using JinRi.Notify.Model;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [Serializable]
    public class NotifyMessageResult : BaseResult
    {
        [DataMember]
        public string MessageId { get; set; }
    }
}
