using System;
using System.Data;

namespace JinRi.Notify.Entity
{
    public class HandleLogEntity
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Ikey { get; set; }
        public string Username { get; set; }
        public DateTime LogTime { get; set; }
        public string ClientIP { get; set; }
        public string ServerIP { get; set; }
        public string Module { get; set; }
        public string Keyword { get; set; }
        public string LogType { get; set; }
        public string Content { get; set; }
    }
}
