using JinRi.Notify.Business;
using JinRi.Notify.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    public class DistributedCacheController : Controller
    {
        //
        // GET: /DistributedCache/

        public ActionResult Index(string key)
        {
            Dictionary<string, string> allCache = DistributedCache.GetAllCache();
            if (allCache == null)
            {
                allCache = new Dictionary<string, string>();
            }
            if (!string.IsNullOrEmpty(key))
            {
                ViewBag.AllCache = allCache.Where(t => t.Key.Contains(key));
            }
            else
            {
                ViewBag.AllCache = allCache;
            }
            return View();
        }

        public ActionResult Lookup(string cacheKey, string args)
        {
            string key = string.Format(cacheKey, args);
            var value = GetCacheValue(key);
            return Content(value);
        }

        public ActionResult Delete(string cacheKey, string args)
        {
            var message = new ReturnMessage { Success = false, Msg = "操作失败!" };
            try
            {
                string key = string.Format(cacheKey, args);
                message.Success = DistributedCache.Delete(key);
                message.Msg = GetCacheValue(key);
            }
            catch (Exception)
            {
            }
            return Content(JsonConvert.SerializeObject(message));
        }

        private string GetCacheValue(string key)
        {
            var value = DistributedCache.Get(key);
            return DistributedCache.FormatCacheValue(value);
        }

    }
}
