using JinRi.Notify.Business;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using JinRi.Notify.Web.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Filters
{
    /// <summary>
    /// 统一登录验证过滤器
    /// </summary>
    public class CheckLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //添加SkipCheckLoginAttribute跳过验证
            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false))
            {
                return;
            }

            WebUserModel userMod = BaseController.WebUser;
            if (userMod == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ContentResult result = new ContentResult();
                    result.Content = JsonConvert.SerializeObject(new { rows= new string[0], total = 0, Success = false, Msg = "站点已登出，请重新登录!" });
                    filterContext.Result = result;
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Account/Login");
                }
            }
        }
    }
}