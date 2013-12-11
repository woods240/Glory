using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlexCel.Core;

namespace WebSite.Controllers.BatchImport
{
    /// <summary>
    /// 批量处理Demo
    /// </summary>
    public class BatchImportDemoController : BatchImportController<DemoController.ExcelDataViewModel>
    {
        protected override ExcelInterpreter GetExcelInterpreter(string excelPath, string sheetName)
        {
            return new DemoExcelInterpreter(excelPath, sheetName);
        }

        protected override int BatchOperate(List<DemoController.ExcelDataViewModel> entityList)
        {
            return entityList.Count;
        }

        private class DemoExcelInterpreter : ExcelInterpreter
        {
            public DemoExcelInterpreter(string excelPath, string sheetName)
                : base(excelPath, sheetName)
            {

            }

            protected override List<CellError> ValidateRecordCorrectness(IEnumerable<CellValue> rowCells, int rowIndex, out Dictionary<string, object> columnsValueDictionary)
            {
                List<CellError> cellErrors = new List<CellError>();
                columnsValueDictionary = GetRecordFromCells(rowCells);

                foreach (KeyValuePair<string, object> columnValue in columnsValueDictionary)
                {
                    string columnName = columnValue.Key;
                    string errorInfo = ValidateColumnValue(columnName, columnsValueDictionary);

                    if (!string.IsNullOrEmpty(errorInfo))
                    {
                        int colIndex = GetColumnIndexByName(columnName);
                        cellErrors.Add(new CellError(rowIndex, colIndex, errorInfo));
                    }
                }

                return cellErrors;
            }


            private string ValidateColumnValue(string columnName, Dictionary<string, object> propertyDictionary)
            {
                string errorInfo = string.Empty;
                object value = propertyDictionary[columnName];
                switch (columnName)
                {
                    case DemoImportTemplateColumns.设备ID:
                        #region 验证 设备ID
                        string 设备ID = Convert.ToString(value);
                        if (string.IsNullOrEmpty(设备ID))
                        {
                            errorInfo = "*  '设备ID'不能为空";
                        }
                        else if (设备ID.Replace("-", "").Length != 32)
                        {
                            errorInfo = "*  '设备ID'应包含32位数字";
                        }
                        #endregion
                        break;

                    default:
                        break;
                }

                return errorInfo;
            }


            protected override string[] GetAllColumns()
            {
                return DemoImportTemplateColumns.AllColumns;
            }

            protected override string[] GetAllDateTimeColumns()
            {
                return DemoImportTemplateColumns.DateTimeColumns;
            }

            private class DemoImportTemplateColumns
            {
                public const string 设备ID = "设备ID";
                public const string 编号 = "编号";
                public const string 设备名称 = "设备名称";
                public const string 规格要求 = "规格要求";
                public const string 单位 = "单位";
                public const string 标配数量 = "标配数量";
                public const string 新增数量 = "新增数量";
                public const string 单价 = "单价（元）";

                public static readonly string[] AllColumns = new string[] { 
                    设备ID, 编号,设备名称,规格要求,单位,标配数量,新增数量,单价
                };

                public static readonly string[] DateTimeColumns = new string[] { };

            }
        }
    }
}
