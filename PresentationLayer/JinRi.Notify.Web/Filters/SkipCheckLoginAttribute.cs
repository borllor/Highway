using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JinRi.Notify.Web.Filters
{
    /// <summary>
    /// 跳过登录验证Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SkipCheckLoginAttribute : Attribute
    {
    }
}