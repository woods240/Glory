using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WebSite
{
    /// <summary>
    /// 公共服务
    /// </summary>
    public class CommonService
    {
        /// <summary>
        /// 生成“指定文件”的下载链接
        /// </summary>
        /// <param name="filePhysicalPath">文件的物理路径</param>
        /// <param name="downloadName">下载名称</param>
        /// <returns>url</returns>
        public string GetDownloadLink(string filePhysicalPath, string downloadName)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            return url.Action("DownloadFile", "CommonService", new { filePhysicalPath = filePhysicalPath, downloadName = downloadName });
        }

        /// <summary>
        /// 生成“随机验证码图片”的下载链接
        /// </summary>
        /// <returns>url</returns>
        public string GetAuthCodeImageSrc()
        {
            return "/CommonService/GetRandomAuthCode?date=" + DateTime.Now.Ticks;
        }

        /// <summary>
        /// 为文件创建唯一的新名称
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string CreateUniqueNameForFile(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            return string.Format("{0}_{1}{2}", name, Guid.NewGuid(), extension);
        }

        /// <summary>
        /// 验证“客户端输入的验证码”与“本次session中最新生成的验证码”是否一致
        /// </summary>
        /// <param name="authCode">客户端输入的验证码</param>
        /// <returns>是否一致</returns>
        public bool ValidateAuthCode(string authCode)
        {
            if (HttpContext.Current.Session[ConstKeys.Session_AuthCode] != null && HttpContext.Current.Session[ConstKeys.Session_AuthCode].ToString() == authCode)
            {
                return true;
            }

            return false;
        }
    }
}