using System;
using System.Web;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// FirstRequestInitialization实现
    /// </summary>
    public static class FirstRequestInitialization
    {
        private static bool s_InitializedAlready = false;
        private static Object s_lock = new Object();

        /// <summary>
        /// 应用程序初始化
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="initAction">initAction</param>
        public static void Initialize(HttpContext context, Action<HttpContext> initAction)
        {
            if (s_InitializedAlready)
            {
                return;
            }
            lock (s_lock)
            {
                if (s_InitializedAlready)
                {
                    return;
                }
                initAction(context);
                s_InitializedAlready = true;
            }
        }
    }
}
