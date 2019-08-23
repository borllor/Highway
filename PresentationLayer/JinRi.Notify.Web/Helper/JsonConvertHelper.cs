using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JinRi.Notify.Web.Helper
{
    public static class JsonConvertHelper
    {
        public static IsoDateTimeConverter IsoDateTimeConverter
        {
            get
            {
                return new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            }
        }
    }
}