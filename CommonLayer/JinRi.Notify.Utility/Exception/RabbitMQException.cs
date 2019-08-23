using System;
using System.Web;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Utility
{
    public class RabbitMQException : Exception
    {
        public RabbitMQException(string message)
            : base(message)
        {

        }

        public RabbitMQException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
