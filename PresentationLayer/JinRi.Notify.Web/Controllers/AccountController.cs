using JinRi.Notify.Business;
using JinRi.Notify.Business.Common;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using JinRi.Notify.Web.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    [SkipCheckLogin]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            var users = ConfigCenterBusiness.WebUser;
            string accesskey = Guid.NewGuid().ToString();
            var user = users.Where(t => t.UserName.Equals(userName) && t.PassWord.Equals(password)).FirstOrDefault();
            if (user == null)
            {
                ViewData["result"] = "用户名或密码错误";
                return View();
            }
            Session[CacheKeys.WebUserCacheKey] = user;
            DistributedCache.Set(string.Format(CacheKeys.UserSessionKey_Arg1, accesskey), user, DateTime.Now.AddHours(8));
            HttpCookie cookie = new HttpCookie("accesskey", accesskey);
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }
        
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session[CacheKeys.WebUserCacheKey] = null;
            if (!string.IsNullOrWhiteSpace(AccessKey))
            {
                DistributedCache.Delete(string.Format(CacheKeys.UserSessionKey_Arg1, AccessKey));
            }
            return RedirectToAction("Login");
        }

       
    }
}
