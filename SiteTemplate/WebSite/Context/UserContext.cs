using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace WebSite
{
    public class UserContext
    {
        public UserContext()
        {


            Navigation = GetNavigation();
        }

        public Navigation Navigation { get; private set; }


        #region 私有方法

        private Navigation GetNavigation()
        {
            // 1.拷贝全局导航节点
            XmlNode rootNode = SysContext.NavigationNode;

            // 2.根据用户权限，筛选可以使用的导航节点（Demo）
            rootNode.Iterate(node =>
            {
                bool deleteNode = node.GetAttribute("description").Equals("3");
                if (deleteNode) { node.RemoveSelf(); }
                return deleteNode;
            });

            // 3.用筛选出的节点树片段，构造导航
            return new Navigation(rootNode);
        }

        #endregion
    }
}