using System.Web;
using System.Web.Mvc;

namespace WebSite
{
    /// <summary>
    /// 路由信息
    /// </summary>
    public class RouteInfo
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string QueryString { get; set; }
    }
}