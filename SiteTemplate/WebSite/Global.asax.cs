using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace WebSite
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Demo", action = "AuthCode", id = UrlParameter.Optional } // 参数默认值
            );

        }

        protected void Application_Start()
        {
            // 仅需支持Razor一种视图模式即可
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            // 模型绑定
            ModelBinders.Binders.Add(typeof(UserContext), new UserContextBinder());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }


        void Application_BeginRequest(object sender, EventArgs e)
        {
            string session_param_name = "ASPSESSID";
            string session_cookie_name = "ASP.NET_SessionId";
            if (HttpContext.Current.Request.Form[session_param_name] != null)
            {
                UpdateCookie(session_cookie_name, HttpContext.Current.Request.Form[session_param_name]);
            }
            else if (HttpContext.Current.Request.QueryString[session_param_name] != null)
            {
                UpdateCookie(session_cookie_name, HttpContext.Current.Request.QueryString[session_param_name]);
            }

            string auth_param_name = "AUTHID";
            string auth_cookie_name = FormsAuthentication.FormsCookieName;
            if (HttpContext.Current.Request.Form[auth_param_name] != null)
            {
                UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
            }
            else if (HttpContext.Current.Request.QueryString[auth_param_name] != null)
            {
                UpdateCookie(auth_cookie_name, HttpContext.Current.Request.QueryString[auth_param_name]);
            }
        }

        private void UpdateCookie(string cookie_name, string cookie_value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookie_name);
            if (null == cookie)
            {
                cookie = new HttpCookie(cookie_name);
            }
            cookie.Value = cookie_value;
            HttpContext.Current.Request.Cookies.Set(cookie);//重新设定请求中的cookie值，将服务器端的session值赋值给它
        }
    }
}