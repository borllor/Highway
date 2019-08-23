using System;
using System.Web;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Utility
{
    public class HttpException : Exception
    {
        public HttpException(string message)
            : base(message)
        {

        }

        public HttpException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
