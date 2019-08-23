using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [KnownTypeAttribute(typeof(PushCallbackResult))]
    [KnownTypeAttribute(typeof(PushMessageResult))]
    [KnownTypeAttribute(typeof(NotifyMessageResult))]
    public abstract class BaseResult : IExtensibleDataObject
    {
        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string ErrMsg { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
