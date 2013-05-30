using System.Web.Mvc;
using System.Diagnostics;

namespace WebSite
{
    /// <summary>
    /// 监视Result的性能
    /// </summary>
    public class ResultPerformanceAttribute : FilterAttribute, IResultFilter
    {
        private Stopwatch timer;

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            timer = Stopwatch.StartNew();
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            timer.Stop();

            if (filterContext.Result is ViewResultBase)
            {
                string view = ((ViewResultBase)filterContext.Result).ViewName;
                string exacuteTimeLog = string.Format("\r\n View: {0}   ElapsedTime: {1}ms \r\n", view, timer.Elapsed.TotalMilliseconds);

                SysContext.Logger.ViewPerformanceLogger.Trace(exacuteTimeLog);
            }
        }
    }
}