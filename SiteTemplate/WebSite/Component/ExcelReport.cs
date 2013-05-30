using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using FlexCel.Report;
using System.IO;

namespace WebSite
{
    /// <summary>
    /// 普通报表
    /// </summary>
    public class ExcelReport
    {
        public string TemplatePath { get; protected set; }      // 模版路径
        public string FilePhysicalPath { get; private set; }    // 报表生成后的物理路径

        public ExcelReport(string templatePath)
        {
            TemplatePath = templatePath;
        }
        

        /// <summary>
        /// 生成报表
        /// </summary>
        public void CreateExcelFile(DataSet dataSource)
        {
            string fileName = SysContext.CommonService.CreateUniqueNameForFile(TemplatePath);
            string physicalDirectory = SysContext.Config.TempDirectory_Physical;
            string fileSavePath = physicalDirectory + "\\" + fileName;

            FlexCelReport cellReport = new FlexCelReport(true);
            cellReport.AddTable(dataSource);
            using (FlexCelReport ordersReport = cellReport)
            {
                ordersReport.SetValue("Date", DateTime.Now);
                ordersReport.Run(TemplatePath, fileSavePath);
            }

            FilePhysicalPath = fileSavePath;
        }

    }
}