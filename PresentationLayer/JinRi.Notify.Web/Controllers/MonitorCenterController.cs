using JinRi.Notify.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    [SkipCheckLoginAttribute]
    public class MonitorCenterController : Controller
    {
        //
        // GET: /MonitorCenter/

        public ActionResult Index(string metricsKey)
        {
            ViewBag.MetricsKey = metricsKey;
            return View();
        }

    }
}
