using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlexCel.XlsAdapter;
using System.Reflection;
using System.ComponentModel;
using FlexCel.Core;
using System.Drawing;

namespace WebSite
{
    /// <summary>
    /// Excel写入器
    /// </summary>
    public class ExcelWriter : IDisposable
    {
        private XlsFile xls;

        public string FilePhysicalPath { get; private set; }
        public string SheetName { get; set; }
        public string SavePath { get; private set; }

        public ExcelWriter(string filePhysicalPath, string sheetName)
        {
            FilePhysicalPath = filePhysicalPath;
            SheetName = sheetName;
            SavePath = filePhysicalPath;

            xls = new XlsFile(true);
            xls.Open(FilePhysicalPath);
            xls.ActiveSheetByName = SheetName;
        }
        public ExcelWriter(string filePhysicalPath, string sheetName, string savePath)
        {
            FilePhysicalPath = filePhysicalPath;
            SheetName = sheetName;
            SavePath = savePath;

            xls = new XlsFile(true);
            xls.Open(FilePhysicalPath);
            xls.ActiveSheetByName = SheetName;
        }


        /// <summary>
        /// 给单元格添加标注
        /// </summary>
        /// <param name="row">单元格所在行</param>
        /// <param name="col">单元格所在列</param>
        /// <param name="comment">标注</param>
        public void AddCommentToCell(int row, int col, string commentTitle, string commentDetail)
        {
            string comment = string.Format("{0}\n{1}\n", commentTitle, commentDetail);

            // 设置单元格背景色
            TFlxFormat fmt = xls.GetCellVisibleFormatDef(row, col);
            fmt.FillPattern.Pattern = TFlxPatternStyle.Solid;
            fmt.FillPattern.FgColor = Color.FromArgb(0xFF9999);
            xls.SetCellFormat(row, col, xls.AddFormat(fmt));

            // 设置批注
            TRTFRun[] Runs = new TRTFRun[3];
            Runs[0].FirstChar = 0;
            TFlxFont fnt = xls.GetDefaultFont;
            fnt.Size20 = 180;
            fnt.Color = TExcelColor.Automatic;
            fnt.Style = TFlxFontStyles.Bold;
            fnt.Family = 3;
            fnt.CharSet = 134;
            fnt.Scheme = TFontScheme.None;
            Runs[0].FontIndex = xls.AddFont(fnt);
            Runs[1].FirstChar = 7;
            fnt = xls.GetDefaultFont;
            fnt.Size20 = 180;
            fnt.Color = TExcelColor.Automatic;
            fnt.Family = 3;
            fnt.CharSet = 134;
            fnt.Scheme = TFontScheme.None;
            Runs[1].FontIndex = xls.AddFont(fnt);
            Runs[2].FirstChar = 13;
            fnt = xls.GetDefaultFont;
            Runs[2].FontIndex = xls.AddFont(fnt);
            xls.SetComment(row, col, new TRichString(comment, Runs, xls));
        }
        
        /// <summary>
        /// 给单元格清除标注
        /// </summary>
        /// <param name="row">单元格所在行</param>
        /// <param name="col">单元格所在列</param>
        public void ClearCommentInCell(int row, int col)
        {
            // 设置单元格背景色
            TFlxFormat fmt = xls.GetCellVisibleFormatDef(row, col);
            fmt.FillPattern.Pattern = TFlxPatternStyle.None;
            fmt.FillPattern.FgColor = TExcelColor.Automatic;
            xls.SetCellFormat(row, col, xls.AddFormat(fmt));

            // 清除标注内容
            xls.SetComment(row, col, string.Empty);
        }

        /// <summary>
        /// 创建 [Excel报表模版] 中的“数据表”部分
        /// </summary>
        /// <typeparam name="T">要绑定的集合元素的类型</typeparam>
        /// <param name="startRow">“数据表”的开始行（从1开始）</param>
        /// <param name="startColumn">“数据表”的开始列（从1开始）</param>
        public void CreateExcelReportTemlate<T>(int startRow = 2, int startColumn = 1) where T : class,new()
        {
            // 属性集合
            int columnCount = 0;
            PropertyInfo[] propertyArray = typeof(T).GetProperties();
            foreach (PropertyInfo property in propertyArray)
            {
                object[] attributes = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (attributes.Length == 0) continue;

                columnCount++;
                string propertyName = property.Name;
                string displayName = ((DisplayNameAttribute)attributes[0]).DisplayName;

                // 设置列标题
                int titleRowIndex = startRow;
                int titleColumnIndex = columnCount + (startColumn - 1);
                SetTitleCell(titleRowIndex, titleColumnIndex, displayName);

                // 设置属性绑定标记
                int dataRowIndex = titleRowIndex + 1;
                int dataColumnIndex = titleColumnIndex;
                SetDataBindCell(dataRowIndex, dataColumnIndex, propertyName);
            }

            // 定义名称区域
            char[] excelColumn = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            string 数据表区域 = string.Format("='{0}'!${2}${1}:${3}${1}", SheetName, startRow + 1, excelColumn[startColumn - 1], excelColumn[columnCount - 1]);
            xls.SetNamedRange(new TXlsNamedRange("__数据表__", 0, 0, 数据表区域));

            string 打印区域 = string.Format("='{0}'!$A$1:${2}${1}", SheetName, startRow + 3, excelColumn[columnCount]);
            xls.SetNamedRange(new TXlsNamedRange(TXlsNamedRange.GetInternalName(InternalNameRange.Print_Area), 1, 32, 打印区域));
        }

        private void SetTitleCell(int row, int col, string diaplayName)
        {
            SetCellFormat(row, col, true, cellColor: 0xC0C0C0, fontColor: 0xFFFF00);
            xls.SetCellValue(row, col, diaplayName);
        }
        private void SetDataBindCell(int row, int col, string propertyName)
        {
            SetCellFormat(row, col, false, fontColor: 0xFF0000);
            xls.SetCellValue(row, col, string.Format("<#数据表.{0}>", propertyName));
        }
        private void SetCellFormat(int row, int col, bool isFontBold, bool isAlignCenter = true, int fontColor = -1, int cellColor = -1)
        {
            TFlxFormat fmt = xls.GetCellVisibleFormatDef(row, col);

            // 字体
            fmt.Font.Family = 3;
            fmt.Font.Size20 = 200;
            fmt.Font.CharSet = 134;
            fmt.Font.Style = isFontBold ? TFlxFontStyles.Bold : TFlxFontStyles.None;
            fmt.Font.Color = fontColor == -1 ? TExcelColor.Automatic : TExcelColor.FromArgb(fontColor);
            //fmt.Font.Scheme = TFontScheme.None;

            // 边框
            fmt.Borders.Left.Style = TFlxBorderStyle.Thin;
            fmt.Borders.Left.Color = TExcelColor.Automatic;
            fmt.Borders.Right.Style = TFlxBorderStyle.Thin;
            fmt.Borders.Right.Color = TExcelColor.Automatic;
            fmt.Borders.Top.Style = TFlxBorderStyle.Thin;
            fmt.Borders.Top.Color = TExcelColor.Automatic;
            fmt.Borders.Bottom.Style = TFlxBorderStyle.Thin;
            fmt.Borders.Bottom.Color = TExcelColor.Automatic;

            // 位置
            if (isAlignCenter)
            {
                fmt.HAlignment = THFlxAlignment.center;
                fmt.VAlignment = TVFlxAlignment.center;
            }

            // 填充
            fmt.WrapText = true;
            fmt.FillPattern.Pattern = cellColor == -1 ? TFlxPatternStyle.None : TFlxPatternStyle.Solid;
            fmt.FillPattern.FgColor = cellColor == -1 ? TExcelColor.Automatic : Color.FromArgb(cellColor);

            xls.SetCellFormat(row, col, xls.AddFormat(fmt));
        }


        #region IDisposable 成员

        ~ExcelWriter()
        {
            Dispose(false);
        }
        private bool _disposed = false;  // 资源是否已释放  
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源  
                    xls.SelectCell(1, 1, false);
                    xls.Save(SavePath);
                    xls = null;
                }
                // 释放非托管资源  
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  // 托管资源已释放，不需要CLR再调用Finalize方法  
        }

        #endregion
    }
}