using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebSite
{
    /// <summary>
    /// 动态报表
    /// </summary>
    public class ExcelReport_Dynamic<T> : ExcelReport where T : class, new()
    {
        public ExcelReport_Dynamic(string dynamicTemplatePath, string sheetName, int startRow, int startColumn)
            : base(null)
        {
            base.TemplatePath = GetTemplatePath(dynamicTemplatePath, sheetName, startRow, startColumn);
        }


        /// <summary>
        /// 获取静态模版，如果不存在，就使用T来创建
        /// </summary>
        private string GetTemplatePath(string dynamicTemplatePath, string sheetName, int startRow, int startColumn)
        {
            string suffix = Path.GetExtension(dynamicTemplatePath);
            int suffixIndex = dynamicTemplatePath.LastIndexOf(suffix);
            string templatePath = dynamicTemplatePath.Insert(suffixIndex, "_" + typeof(T).Name);

            // 如果模版在约定的目录中能找到，就不用再动态创建
            if (!File.Exists(templatePath))
            {
                // 修改动态模版，写入T的属性，将其变成静态模版
                using (ExcelWriter excelWriter = new ExcelWriter(dynamicTemplatePath, sheetName, templatePath))
                {
                    excelWriter.CreateExcelReportTemlate<T>(startRow, startColumn);
                }
            }

            return templatePath;
        }
    }
}