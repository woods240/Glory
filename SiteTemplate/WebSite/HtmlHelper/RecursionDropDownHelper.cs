using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Text;

namespace System.Web.Mvc
{
    /// <summary>
    /// 递归下拉菜单
    /// </summary>
    public static class RecursionDropDownHelper
    {
        /// <summary>
        /// 递归下拉菜单
        /// </summary>
        public static MvcHtmlString RecursionDropDown(this HtmlHelper htmlHelper, XmlNode rootNode)
        {
            StringBuilder sb = new StringBuilder();
            CreateDropDownMenForChildNodes(rootNode, sb);

            return MvcHtmlString.Create(sb.ToString());
        }

        private static void CreateDropDownMenForChildNodes(XmlNode node, StringBuilder sb)
        {
            //sb.Append("<ul class=\"dropdown-menu\" role=\"menu\" aria-labelledby=\"dropdownMenu\">");
            sb.Append("<ul class=\"dropdown-menu\">");
            foreach (XmlNode childNode in node.ChildNodes)
            {
                string linkText = childNode.GetAttribute("title");
                string linkUrl = childNode.GetAttribute("url");

                if (childNode.HasChildNodes)
                {
                    sb.Append(string.Format("<li class=\"dropdown-submenu\"><a href=\"#\">{0}</a>", linkText));
                    CreateDropDownMenForChildNodes(childNode, sb);
                    sb.Append("</li>");
                }
                else
                {
                    sb.Append(string.Format("<li><a href=\"{1}\">{0}</a></li>", linkText, linkUrl));
                }
            }
            sb.Append("</ul>");
        }
    }
}