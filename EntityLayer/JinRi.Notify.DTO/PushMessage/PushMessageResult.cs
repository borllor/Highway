using System;
using System.Linq;
using System.Runtime.Serialization;

using JinRi.Notify.Model;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [Serializable]
    public class PushMessageResult : BaseResult
    {
        [DataMember]
        public string PushId { get; set; }

        public object ReferObject { get; set; }

        [DataMember]
        public PushResultEnum PushStatus { get; set; }
    }
}
