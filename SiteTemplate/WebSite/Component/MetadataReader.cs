using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel;

namespace WebSite
{
    /// <summary>
    /// 元数据读取器
    /// </summary>
    public class MetadataReader
    {
        public static string GetDisplayName(PropertyInfo property)
        {
            object[] displayAttribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (displayAttribute.Count() > 0)
            {
                return ((DisplayNameAttribute)displayAttribute[0]).DisplayName;
            }

            return string.Empty;
        }

    }
}