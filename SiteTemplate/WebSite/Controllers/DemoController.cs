using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using FlexCel.Core;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Web.Security;

namespace WebSite.Controllers
{
    public class DemoController : Controller
    {
        [ActionPerformance]
        [ResultPerformance]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AuthCode()
        {
            return View();
        }
        public ActionResult CreateExcelReportTemlate()
        {
            return View();
        }
        public ActionResult Export()
        {
            return View();
        }
        public ActionResult DynamicExport()
        {
            return View();
        }
        public ActionResult AddCommentToExcel()
        {
            return View();
        }
        public ActionResult ClearCommentInExcel()
        {
            return View();
        }
        public ActionResult ReadExcel()
        {
            return View();
        }
        public ActionResult ReadSiteMap()
        {
            return View();
        }
        public ActionResult ReadUserNavigation()
        {
            return View();
        }
        public ActionResult TreeMenu()
        {
            return View();
        }
        public ActionResult DropDownMenu()
        {
            return View();
        }
        public ActionResult UpgradeLog()
        {
            return View();
        }
        public ActionResult ProgressBar()
        {
            return View();
        }
        public ActionResult DataGrid()
        {
            return View();
        }
        public ActionResult BatchImport()
        {
            return View();
        }


        #region 例子

        public ActionResult AuthCodeDemo()
        {
            return View();
        }

        public ActionResult CreateExcelReportTemlateDemo()
        {
            // 1.创建报表模版
            string filePhysicalPath = SysContext.Config.ReportTemplateDirectory_Physical + "测试模板.xlsx";
            string savePhysicalPath = SysContext.Config.TempDirectory_Physical + SysContext.CommonService.CreateUniqueNameForFile(filePhysicalPath);
            using (ExcelWriter excelWriter = new ExcelWriter(filePhysicalPath, "生成报表模版", savePhysicalPath))
            {
                excelWriter.CreateExcelReportTemlate<ExcelDataViewModel>();
            }

            // 2.展现Excel文件
            return View("~/Views/Shared/Controls/ExcelReportView.cshtml", new ExcelDisplayViewModel
            {
                Title = "创建Excel报表模版 Demo",
                FilePhysicalPath = savePhysicalPath,
                SheetName = "生成报表模版"
            });
        }

        public ActionResult ExportDemo()
        {
            // 1.获取数据源
            DataSet dataSource = new DataSet();
            DataTable dt = new DataTable("数据表");
            dt.Columns.Add("姓名");
            DataRow dr = dt.NewRow();
            dr["姓名"] = "胡明明";
            dt.Rows.Add(dr);
            dataSource.Tables.Add(dt);

            // 2.生成报表
            string templatePath = SysContext.Config.ReportTemplateDirectory_Physical + "Excel报表模版Demo.xlsx";
            ExcelReport excelReport = new ExcelReport(templatePath);
            excelReport.CreateExcelFile(dataSource);

            // 3.展现报表
            string reportName = "Excel普通报表 Demo";
            return View("~/Views/Shared/Controls/ExcelReportView.cshtml", new ExcelDisplayViewModel
            {
                Title = reportName,
                FilePhysicalPath = excelReport.FilePhysicalPath,
                BodyEndJavascript = "alert('" + reportName + "');",
                BodyStartHtml = string.Format("<a href=\"{0}\" target=\"_blank\">开始下载</a>",
                        SysContext.CommonService.GetDownloadLink(excelReport.FilePhysicalPath, reportName)),
            });
        }

        public ActionResult DynamicExportDemo()
        {
            // 1.获取数据源
            DataSet dataSource = GetDataSourceForDynamicExportDemo();

            // 2.生成报表
            string templatePath = SysContext.Config.ReportTemplateDirectory_Physical + "Excel动态报表模版Demo.xlsx";
            ExcelReport_Dynamic<DynamicExcelDataViewModel> dynamicReport = new ExcelReport_Dynamic<DynamicExcelDataViewModel>(templatePath, "动态模版页", 3, 1);
            dynamicReport.CreateExcelFile(dataSource);

            // 3.展现报表
            string reportName = "Excel动态报表 Demo";
            return View("~/Views/Shared/Controls/ExcelReportView.cshtml", new ExcelDisplayViewModel
            {
                Title = reportName,
                FilePhysicalPath = dynamicReport.FilePhysicalPath,
                BodyEndJavascript = "alert('" + reportName + "');",
                BodyStartHtml = string.Format("<a href=\"{0}\" target=\"_blank\">开始下载</a>",
                        SysContext.CommonService.GetDownloadLink(dynamicReport.FilePhysicalPath, reportName)),
            });
        }
        private DataSet GetDataSourceForDynamicExportDemo()
        {
            DataSet dataSource = new DataSet();

            // 标题表
            DataTable 标题表 = new DataTable("标题表");
            标题表.Columns.Add("标题");
            标题表.Columns.Add("副标题");
            DataRow dr = 标题表.NewRow();
            dr["标题"] = "动态报表" + typeof(DynamicExcelDataViewModel).Name;
            dr["副标题"] = "动态模版：" + typeof(ExcelDataViewModel).Name;
            标题表.Rows.Add(dr);
            dataSource.Tables.Add(标题表);

            // 数据表
            using (ExcelReader excelReader = new ExcelReader(SysContext.Config.ReportTemplateDirectory_Physical + "测试模板.xlsx", true))
            {
                List<DynamicExcelDataViewModel> modelList = excelReader.Read<DynamicExcelDataViewModel>("哈哈");
                DataTable 数据表 = FormatConverter.ListToDataTable<DynamicExcelDataViewModel>(
                    modelList,
                    property => property.Name);
                数据表.TableName = "数据表";
                dataSource.Tables.Add(数据表);
            }

            return dataSource;
        }

        public ActionResult AddCommentToExcelDemo()
        {
            // 1.添加批注
            string filePhysicalPath = SysContext.Config.ReportTemplateDirectory_Physical + "测试模板.xlsx";
            using (ExcelWriter excelWriter = new ExcelWriter(filePhysicalPath, "哈哈"))
            {
                excelWriter.AddCommentToCell(3, 1, "数据格式错误", "必须是GUID");
                excelWriter.AddCommentToCell(3, 8, "数据格式错误", "必须是数字");
            }

            // 2.展现Excel文件
            return View("~/Views/Shared/Controls/ExcelReportView.cshtml", new ExcelDisplayViewModel
            {
                Title = "Excel添加批注 Demo",
                FilePhysicalPath = filePhysicalPath,
                SheetName = "哈哈"
            });
        }

        public ActionResult ClearCommentInExcelDemo()
        {
            // 1.清除批注
            string filePhysicalPath = SysContext.Config.ReportTemplateDirectory_Physical + "测试模板.xlsx";
            using (ExcelWriter excelWriter = new ExcelWriter(filePhysicalPath, "哈哈"))
            {
                excelWriter.ClearCommentInCell(3, 8);
            }

            // 2.展现Excel文件
            return View("~/Views/Shared/Controls/ExcelReportView.cshtml", new ExcelDisplayViewModel
            {
                Title = "Excel清除批注 Demo",
                FilePhysicalPath = filePhysicalPath,
                SheetName = "哈哈"
            });
        }

        public ActionResult ReadExcelDemo()
        {
            List<ExcelDataViewModel> modelList = null;

            string filePhysicalPath = SysContext.Config.ReportTemplateDirectory_Physical + "测试模板.xlsx";
            string sheetName = "哈哈";
            bool hasTitle = true;
            using (ExcelReader excelReader = new ExcelReader(filePhysicalPath, hasTitle))
            {
                modelList = excelReader.Read<ExcelDataViewModel>(sheetName);
            }

            DataTable dataTable = FormatConverter.ListToDataTable<ExcelDataViewModel>(
                modelList,
                property => MetadataReader.GetDisplayName(property));
            return View("~/Views/Shared/Controls/Table_DataTable_Partial.cshtml", dataTable);

            //return View("~/Views/Shared/Controls/Table_IEnumerable_Partial.cshtml", modelList);
        }

        public ActionResult ReadSiteMapDemo()
        {
            // 1.读取站点地图
            XmlNode rootNode = SysContext.NavigationNode;

            // 2.修改Xml节点
            foreach (XmlNode node in rootNode.GetChildNodes("siteMapNode", "title", "验证码组件"))
            {
                node.RemoveSelf();
            }

            // 3.将修改后的Xml节点，保存到文件，并显示在浏览器中
            string filePath = SysContext.Config.TempDirectory_Physical + SysContext.CommonService.CreateUniqueNameForFile("siteMapDemo.xml");
            rootNode.Save(filePath);
            return File(filePath, MimeHelper.GetMIMETypeForFile(".xml"));
        }

        public ActionResult ReadUserNavigationDemo()
        {
            UserContext userContext = new UserContext();

            string filePath = SysContext.Config.TempDirectory_Physical + SysContext.CommonService.CreateUniqueNameForFile("siteMapDemo.xml");
            userContext.Navigation.RootNode.Save(filePath);
            return File(filePath, MimeHelper.GetMIMETypeForFile(".xml"));
        }

        public ActionResult BatchImportDemo()
        {
            BatchImportViewModel batchImportViewModel = new BatchImportViewModel();
            batchImportViewModel.Title = "批量导入Demo";
            batchImportViewModel.TemplatePath = Server.MapPath("/Content/ImportTemplate/批量导入模版.xlsx");
            batchImportViewModel.TemplateDownloadName = "批量导入模版Demo";
            batchImportViewModel.SheetName = "批量导入数据Demo";
            batchImportViewModel.UploadSettings = new UploadSettingsViewModel("file_upload",
                "/BatchImportDemo/Import",
                new
                {
                    templateDownloadName = batchImportViewModel.TemplateDownloadName,
                    sheetName = batchImportViewModel.SheetName,
                    ASPSESSID = Session.SessionID,
                    AUTHID = Request.Cookies[FormsAuthentication.FormsCookieName] != null
                        ? Request.Cookies[FormsAuthentication.FormsCookieName].Value
                        : string.Empty
                }
            );
            
            return View(batchImportViewModel);
        }

        #endregion

        #region MockData

        public class ExcelDataViewModel
        {
            [DisplayName("设备ID")]
            public Guid EquipmentId { get; set; }

            [DisplayName("编号")]
            public string SN { get; set; }

            [DisplayName("设备名称")]
            public string Name { get; set; }

            [DisplayName("规格要求")]
            public string Specification { get; set; }

            [DisplayName("单位")]
            public string Unit { get; set; }

            [DisplayName("标配数量")]
            public string Standard { get; set; }

            [DisplayName("新增数量")]
            public int Amount { get; set; }

            [DisplayName("单价（元）")]
            public decimal Price { get; set; }
        }
        public class DynamicExcelDataViewModel : ExcelDataViewModel
        {
            [DisplayName("备注")]
            public string Description { get; set; }
        }

        #endregion
    }
}
