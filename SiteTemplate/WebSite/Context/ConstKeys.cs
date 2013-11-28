using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite
{
    /// <summary>
    /// 键名集合
    /// (所有键值对，“对应的键”全部在这里定义)
    /// </summary>
    public class ConstKeys
    {
        // Application
        public const string Application_NavigationNode = "NavigationNode";

        // Session
        public const string Session_UserContext = "UserContext";
        public const string Session_AuthCode = "AuthCode";

        // Cookie
        public const string Cookie_Notice = "Notice";

        // TempData
        public const string TempData_ErrorMsg = "ErrorMsg";
        public const string TempData_AlertMsg = "AlertMsg";
    }
}