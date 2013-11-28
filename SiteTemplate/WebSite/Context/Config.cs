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
    /// (只在应用启动时读取一次，修改配置后需要重启)
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 是否发布模式
        /// </summary>
        public bool IsReleaseMode
        {
            get
            {
                if (!_isReleaseMode.HasValue)
                {
                    _isReleaseMode = ConfigurationManager.AppSettings["Release"].Equals("true");
                }

                return _isReleaseMode.Value;
            }
        }
        private bool? _isReleaseMode;

        /// <summary>
        /// 启用性能日志
        /// </summary>
        public bool EnablePerformanceLog
        {
            get {
                if (!_enablePerformanceLog.HasValue)
                {
                    _enablePerformanceLog = ConfigurationManager.AppSettings["EnablePerformanceLog"].Equals("true");
                }

                return _enablePerformanceLog.Value;
            }
        }
        private bool? _enablePerformanceLog;

        /// <summary>
        /// Excel报表模版目录（物理）
        /// </summary>
        public string ReportTemplateDirectory_Physical
        {
            get
            {
                if (string.IsNullOrEmpty(_reportTemplateDirectory_Physical))
                {
                    _reportTemplateDirectory_Physical = ConfigurationManager.AppSettings["ReportTemplateDirectory_Physical"];
                    if (string.IsNullOrEmpty(_reportTemplateDirectory_Physical))
                    {
                        _reportTemplateDirectory_Physical = HttpContext.Current.Server.MapPath("/Content/ReportTemplate/");
                    }
                }

                if (!Directory.Exists(_reportTemplateDirectory_Physical))
                {
                    Directory.CreateDirectory(_reportTemplateDirectory_Physical);
                }

                return _reportTemplateDirectory_Physical;
            }
        }
        private string _reportTemplateDirectory_Physical;

        /// <summary>
        /// 临时文件目录（物理）
        /// </summary>
        public string TempDirectory_Physical
        {
            get
            {
                if (string.IsNullOrEmpty(_tempDirectory_Physical))
                {
                    _tempDirectory_Physical = ConfigurationManager.AppSettings["TempDirectory_Physical"];
                    if (string.IsNullOrEmpty(_tempDirectory_Physical))
                    {
                        _tempDirectory_Physical = HttpContext.Current.Server.MapPath("/Content/Temp/");
                    }
                }

                if (!Directory.Exists(_tempDirectory_Physical))
                {
                    Directory.CreateDirectory(_tempDirectory_Physical);
                }

                return _tempDirectory_Physical;
            }
        }
        private string _tempDirectory_Physical;

        /// <summary>
        /// 上传文件目录（物理）
        /// </summary>
        public string UploadDirectory_Physical
        {
            get
            {
                if (string.IsNullOrEmpty(_uploadDirectory_Physical))
                {
                    _uploadDirectory_Physical = ConfigurationManager.AppSettings["UploadDirectory_Physical"];
                    if (string.IsNullOrEmpty(_uploadDirectory_Physical))
                    {
                        _uploadDirectory_Physical = HttpContext.Current.Server.MapPath("/Content/Upload/");
                    }
                }

                if (!Directory.Exists(_uploadDirectory_Physical))
                {
                    Directory.CreateDirectory(_uploadDirectory_Physical);
                }

                return _uploadDirectory_Physical;
            }
        }
        private string _uploadDirectory_Physical;
    }
}