using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite
{
    /// <summary>
    /// 运行时环境变量
    /// </summary>
    public class RuntimeVarible
    {
        /// <summary>
        /// 路由信息
        /// </summary>
        public RouteInfo CurrentRouteInfo
        {
            get
            {
                HttpRequest httpRequest = HttpContext.Current.Request;
                return new RouteInfo
                {
                    Controller = httpRequest.RequestContext.RouteData.Values["controller"].ToString(),
                    Action = httpRequest.RequestContext.RouteData.Values["action"].ToString(),
                    QueryString = HttpUtility.UrlDecode(httpRequest.QueryString.ToString())
                };
            }
        }

        /// <summary>
        /// 当前位置（必须与siteMap一致）
        /// </summary>
        public string CurrentUrl
        {
            get
            {
                return string.Format("/{0}/{1}", CurrentRouteInfo.Controller, CurrentRouteInfo.Action);
            }
        }

        public string CurrentActivity
        {
            get { return "当前Action所属于的活动"; }
        }
    }
}