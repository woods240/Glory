using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite
{
    public class ExcelDisplayViewModel
    {
        public string Title { get; set; }                   // 页面标题
        public string FilePhysicalPath { get; set; }        // Excel文件路径
        public string SheetName { get; set; }               // 标签页名称
        public string BodyStartHtml { get; set; }           // 在body的开始添加html
        public string BodyEndJavascript { get; set; }       // 在body的结束添加javascript
    }
}