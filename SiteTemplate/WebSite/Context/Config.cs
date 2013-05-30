using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

namespace WebSite
{
    /// <summary>
    /// 系统配置(Web.config)
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 是否发布模式
        /// </summary>
        public bool IsReleaseMode
        {
            get { return ConfigurationManager.AppSettings["Release"].Equals("true"); }
        }

        /// <summary>
        /// 启用性能日志
        /// </summary>
        public bool EnablePerformanceLog
        {
            get { return ConfigurationManager.AppSettings["EnablePerformanceLog"].Equals("true"); }
        }

        /// <summary>
        /// 上传目录（虚拟）
        /// </summary>
        public string UploadDirectory_Virtual
        {
            get { return ConfigurationManager.AppSettings["UploadDirectory_Virtual"]; }
        }

        /// <summary>
        /// Excel报表模版目录（虚拟）
        /// </summary>
        public string ReportTemplateDirectory_Virtual
        {
            get { return ConfigurationManager.AppSettings["ReportTemplateDirectory_Virtual"]; }
        }

        /// <summary>
        /// 临时文件目录（虚拟）
        /// </summary>
        public string TempDirectory_Virtual
        {
            get { return ConfigurationManager.AppSettings["TempDirectory_Virtual"]; }
        }

        /// <summary>
        /// 上传目录（物理）
        /// </summary>
        public string UploadDirectory_Physical
        {
            get
            {
                string uploadDirectory_Physical = HttpContext.Current.Server.MapPath(UploadDirectory_Virtual);
                if (!Directory.Exists(uploadDirectory_Physical))
                {
                    Directory.CreateDirectory(uploadDirectory_Physical);
                }

                return uploadDirectory_Physical;
            }
        }

        /// <summary>
        /// Excel报表模版目录（物理）
        /// </summary>
        public string ReportTemplateDirectory_Physical
        {
            get
            {
                string reportTemplateDirectory_Physical = HttpContext.Current.Server.MapPath(ReportTemplateDirectory_Virtual);
                if (!Directory.Exists(reportTemplateDirectory_Physical))
                {
                    Directory.CreateDirectory(reportTemplateDirectory_Physical);
                }

                return reportTemplateDirectory_Physical;
            }
        }

        /// <summary>
        /// 临时文件目录（物理）
        /// </summary>
        public string TempDirectory_Physical
        {
            get
            {
                string tempDirectory_Physical = HttpContext.Current.Server.MapPath(TempDirectory_Virtual);
                if (!Directory.Exists(tempDirectory_Physical))
                {
                    Directory.CreateDirectory(tempDirectory_Physical);
                }

                return tempDirectory_Physical;
            }
        }
    }
}