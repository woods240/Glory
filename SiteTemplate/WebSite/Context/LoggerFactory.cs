using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace WebSite
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class LoggerFactory
    {
        /// <summary>
        /// Action性能日志记录器
        /// </summary>
        public Logger ActionPerformanceLogger
        {
            get { return LogManager.GetLogger("ActionPerformanceLogger"); }
        }

        /// <summary>
        /// View性能日志记录器
        /// </summary>
        public Logger ViewPerformanceLogger
        {
            get { return LogManager.GetLogger("ViewPerformanceLogger"); }
        }

        /// <summary>
        /// 异常日志记录器
        /// </summary>
        public Logger ExceptionLogger
        {
            get { return LogManager.GetLogger("ExceptionLogger"); }
        }
    }
}