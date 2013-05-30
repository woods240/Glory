using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite
{
    public class LinkViewModel
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public LinkTarget Target { get; set; }
        public LinkState State { get; set; }
    }

    public enum LinkState
    {
        common = 0,     // 正常
        active,         // 已选择
        disabled        // 禁用
    }
    public enum LinkTarget
    {
        _blank = 0,     // 新页面
        _self           // 本窗口
    }
}