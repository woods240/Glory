using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite
{
    /// <summary>
    /// 键名集合
    /// </summary>
    public class ConstKeys
    {
        // Application
        public string Application_NavigationNode
        {
            get { return "NavigationNode"; }
        }

        // Session
        public string Session_UserContext
        {
            get { return "UserContext"; }
        }
        public string Session_AuthCode
        {
            get { return "AuthCode"; }
        }

        public string Cookie_Notice
        {
            get { return "Notice"; }
        }

        // TempData
        public string TempData_ErrorMsg
        {
            get { return "ErrorMsg"; }
        }
        public string TempData_AlertMsg
        {
            get { return "AlertMsg"; }
        }
    }
}