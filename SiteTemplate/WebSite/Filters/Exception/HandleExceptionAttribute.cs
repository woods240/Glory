using System;
using System.Web.Mvc;

namespace WebSite
{
    /// <summary>
    /// 自定义异常处理逻辑
    /// </summary>
    public class HandleExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;

            // 1.记录异常日志（面向开发人员）
            SysContext.Logger.ExceptionLogger.ErrorException("系统异常", ex);

            // 2.实施补救措施（替代方案、应急预案等）
            // TODO

            // 3.展示错误页面：默认为~/Views/Shared/Error.cshtml，由HandleErrorAttribute捕获（面向用户）
            if (ex is NullReferenceException)
            {
                //filterContext.Result = new ViewResult { ViewName = MVC.Home.Views.Index };
            }
            else if (ex is HttpAntiForgeryException)
            {
                filterContext.Result = new RedirectResult("/Authentication/LogOn");
            }
            // ...
        }
    }
}