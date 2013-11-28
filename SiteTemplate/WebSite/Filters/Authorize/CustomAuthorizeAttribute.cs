using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WebSite
{
    /// <summary>
    /// 自定义授权逻辑
    /// </summary>
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // 1.不需要对ChildAction进行权限检查
            if (filterContext.IsChildAction)
            {
                return;
            }

            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// 授权逻辑
        /// </summary>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // 1.Session过期，不予授权
            if (httpContext == null || httpContext.Session[ConstKeys.Session_UserContext] == null)
            {
                return false;
            }

            // 2.判断当前活动是否在用户可访问的活动集合中
            string currentActivity = SysContext.RuntimeVarible.CurrentActivity;
            //IEnumerable<string> activityNames = userContext.ValidActivityNames;
            //if (!activityNames.Contains(SysContext.CurrentActivityName))
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 无权限处理
        /// </summary>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // Ajax须返回JsonResult
                filterContext.Result = new JsonResult
                {
                    Data = new { Error = "未授权", LogOnUrl = FormsAuthentication.LoginUrl },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                // 默认显示登录页面，也可以自定义
                //filterContext.Result = new ViewResult { ViewName = MVC.Shared.Views.Error };
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}