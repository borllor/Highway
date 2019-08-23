using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    public class BaseController : Controller
    {
        public static string AccessKey
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                {
                    HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["accesskey"];
                    if (cookie != null)
                    {
                        return cookie.Value;
                    }
                    
                }
                return string.Empty;
            }
        }

        public static string ClientIP
        {
            get
            {
                return JinRi.Notify.Frame.ClientHelper.GetClientIP();
            }
        }

        public static WebUserModel WebUser
        {
            get
            {
                JinRi.Notify.Model.WebUserModel userMod = null;
                if (System.Web.HttpContext.Current != null)
                {
                    if (System.Web.HttpContext.Current.Session[CacheKeys.WebUserCacheKey] != null)
                    {
                        userMod = System.Web.HttpContext.Current.Session[CacheKeys.WebUserCacheKey] as JinRi.Notify.Model.WebUserModel;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(AccessKey))
                        {
                            userMod = JinRi.Notify.Business.DistributedCache.Get(string.Format(JinRi.Notify.Utility.CacheKeys.UserSessionKey_Arg1, AccessKey)) as JinRi.Notify.Model.WebUserModel;
                        }
                    }
                }
                return userMod;
            }
        }

        public static string UserName
        {
            get
            {
                return WebUser == null ? string.Empty : WebUser.UserName;
            }
        }
    }
}
