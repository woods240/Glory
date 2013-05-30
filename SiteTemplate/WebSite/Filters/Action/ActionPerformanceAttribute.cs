using System.Web.Mvc;
using System.Diagnostics;

namespace WebSite
{
    /// <summary>
    /// 监视Action的性能
    /// </summary>
    public class ActionPerformanceAttribute : FilterAttribute, IActionFilter
    {
        private Stopwatch timer;

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            timer = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            timer.Stop();

            string exacuteTimeLog = string.Format("\r\n Controller:{0} Action:{1} QueryString:{2}   ElapsedTime: {3}ms \r\n",
                SysContext.RuntimeVarible.CurrentRouteInfo.Controller,
                SysContext.RuntimeVarible.CurrentRouteInfo.Action,
                SysContext.RuntimeVarible.CurrentRouteInfo.QueryString,
                timer.Elapsed.TotalMilliseconds);

            SysContext.Logger.ActionPerformanceLogger.Trace(exacuteTimeLog);
        }
    }
}