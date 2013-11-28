using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

namespace WebSite.Controllers
{
    public class CommonServiceController : Controller
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePhysicalPath">文件的虚拟路径</param>
        /// <param name="downloadName">下载文件名</param>
        public FilePathResult DownloadFile(string filePhysicalPath, string downloadName)
        {
            string extension = Path.GetExtension(filePhysicalPath);
            downloadName = HttpUtility.UrlPathEncode(downloadName + extension);

            if (Request.Browser.Browser == "IE")
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=" + downloadName);
            }

            return File(filePhysicalPath, MimeHelper.GetMIMETypeForFile(extension), downloadName);
        }

        /// <summary>
        /// 获取随机验证码
        /// </summary>
        /// <returns>验证码图片</returns>
        public FileContentResult GetRandomAuthCode()
        {
            AuthCode auCode = new AuthCode(4);
            string strCode = auCode.GetRandomCode();
            Session[ConstKeys.Session_AuthCode] = strCode;
            byte[] imageByte = auCode.CreateJpegImage(strCode);

            return File(imageByte, @"image/jpeg");
        }

        /// <summary>
        /// 系统公告
        /// </summary>
        /// <returns>弹出“系统公告”窗口</returns>
        public ActionResult Notice()
        {
            // 1.获取服务器端公告标识
            string noticeFolder = Server.MapPath("~/Content/Notice");
            string serverNoticeFile = Directory.GetFiles(noticeFolder, "Notice_*.txt", SearchOption.TopDirectoryOnly).OrderBy(f => System.IO.File.GetCreationTime(f)).FirstOrDefault();
            if (string.IsNullOrEmpty(serverNoticeFile))
            {
                // 不显示公告
                return null;
            }

            // 2.获取客户端公告标识
            string clientNoticeFile = Request.Cookies[ConstKeys.Cookie_Notice] != null ? Request.Cookies[ConstKeys.Cookie_Notice].Value : string.Empty;
            if (serverNoticeFile == clientNoticeFile)
            {
                return null;
            }

            // 3.显示公告 
            NoticeViewModel model = new NoticeViewModel();
            model.NoticeTime = System.IO.File.GetCreationTime(serverNoticeFile);
            model.NoticeContent = System.IO.File.ReadAllText(serverNoticeFile);

            // 4.更新客户端公告标识
            HttpCookie noticeCookie = new HttpCookie(ConstKeys.Cookie_Notice, serverNoticeFile);
            noticeCookie.HttpOnly = true;
            noticeCookie.Expires = DateTime.Now.AddDays(3); // 公告保留时间
            Response.Cookies.Add(noticeCookie);

            return PartialView(model);
        }

        /// <summary>
        /// 升级日志
        /// </summary>
        /// <returns>升级日志部分页面</returns>
        public ActionResult UpgradeLog()
        {
            // 搜索以 "UpgradeLog_" 开头的 xml文件，降序排列
            string upgradeLogFolder = Server.MapPath("~/Content/UpgradeLog");
            string[] logFiles = Directory.GetFiles(upgradeLogFolder, "UpgradeLog_*.xml", SearchOption.TopDirectoryOnly).OrderByDescending(f => System.IO.File.GetCreationTime(f)).ToArray();

            return PartialView(logFiles);
        }


    }
}
