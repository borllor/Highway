using System;
using System.Web;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JinRi.Notify.Utility
{
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        {

        }

        public BusinessException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
