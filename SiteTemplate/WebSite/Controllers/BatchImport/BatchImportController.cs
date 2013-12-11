using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Component;

namespace WebSite.Controllers
{
    public abstract class BatchImportController<T> : Controller where T : class, new()
    {
        /// <summary>
        /// 定义批量处理页面
        /// </summary>
        /// <param name="viewModel">批量处理的数据模型</param>
        /// <returns>呈现“批量处理页面”</returns>
        public ActionResult Index(BatchImportViewModel viewModel)
        {
            return View(viewModel);
        }

        /// <summary>
        /// Excel数据处理
        /// /* 上传状态码约定 （根据实际情况，自行修改）
        ///     状态码         含义
        ///       0         "上传失败"
        ///       1         "模版格式错误"
        ///       2         "批量导入失败"
        ///       3         "批量导入成功"
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import(string templateDownloadName, string sheetName)
        {
            // 1.保存上传文件
            HttpPostedFileBase file = Request.Files[0];
            if (file == null)
            {
                return Json(new { StatusCode = 0 }, JsonRequestBehavior.AllowGet);
            }
            string filePhysicalPath = SysContext.UploadFileManager.AddUploadFile(file);

            // 2.读取上传文件
            ExcelInterpreter excelInterpreter = GetExcelInterpreter(filePhysicalPath, sheetName);
            List<T> entityList = excelInterpreter.GetEntityList<T>();
            if (entityList.Count == 0)
            {
                return Json(
                    new
                    {
                        StatusCode = 1,
                        ErrorFilePath = SysContext.CommonService.GetDownloadLink(filePhysicalPath, templateDownloadName)
                    }
                    , JsonRequestBehavior.AllowGet);
            }

            // 3.批量处理
            int successRecordCount = BatchOperate(entityList);
            if (successRecordCount == 0)
            {
                return Json(new { StatusCode = 2 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { StatusCode = 3, SuccessRecordCount = successRecordCount }, JsonRequestBehavior.AllowGet);
        }

        protected abstract ExcelInterpreter GetExcelInterpreter(string excelPath, string sheetName);

        protected abstract int BatchOperate(List<T> entityList);
    }

}
