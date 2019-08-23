using JinRi.Notify.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckLoginFilter());
        }
    }
}