using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.DTO
{
    [DataContract]
    [Serializable]
    public class BeatMessage
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        [DataMember]
        public string HostIP { get; set; }
    }
}
