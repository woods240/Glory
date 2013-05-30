using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using FlexCel.Report;
using NLog;
using System.Web.Mvc;

namespace WebSite
{
    /// <summary>
    /// 系统环境上下文
    /// </summary>
    public static class SysContext
    {
        // 键名集合
        public static readonly ConstKeys ConstKeys = new ConstKeys();

        // 系统配置
        public static readonly Config Config = new Config();

        // 日志记录器
        public static readonly LoggerFactory Logger = new LoggerFactory();

        // 公共服务
        public static readonly CommonService CommonService = new CommonService();

        // 环境变量 - 运行时
        public static readonly RuntimeVarible RuntimeVarible = new RuntimeVarible();

        // 导航节点
        public static XmlNode NavigationNode
        {
            get
            {
                XmlNode navigationNode = HttpContext.Current.Application[ConstKeys.Application_NavigationNode] as XmlNode;
                if (navigationNode == null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Content/UserSiteMap/Web.sitemap"));
                    navigationNode = xmlDoc.SelectSingleNode("/siteMap/siteMapNode");
                    HttpContext.Current.Application[ConstKeys.Application_NavigationNode] = navigationNode;
                }

                return navigationNode.Clone();
            }
        }
    }
}