using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using FlexCel.Core;
using FlexCel.XlsAdapter;

namespace WebSite
{
    /// <summary>
    /// Excel写入器
    /// </summary>
    public class ExcelWriter : IDisposable
    {
        private XlsFile _xls = new XlsFile(true);
        private string _filePath;
        private string _sheetName;
        private string _savePath;

        public ExcelWriter(string filePath, string sheetName, string savePath)
        {
            _filePath = filePath;
            _sheetName = sheetName;
            _savePath = savePath;

            _xls = new XlsFile(true);
            _xls.Open(_filePath);
            _xls.ActiveSheetByName = _sheetName;
        }

        public ExcelWriter(string filePath, string sheetName)
            : this(filePath, sheetName, filePath)
        {

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
            SetDefaultBackground(row, col);

            // 处理批注内容过长
            if (comment.Length > 45)
            {
                TCommentProperties CommentProps = new TCommentProperties();
                CommentProps.Anchor = new TClientAnchor(false, TFlxAnchorType.DontMoveAndDontResize, 7, 113, 5, 240, 11, 198, 9, 16);
                _xls.SetCommentProperties(row, col, CommentProps);
            }

            // 设置批注
            _xls.SetComment(row, col, GetCommentStyle(comment));
        }

        /// <summary>
        /// 给单元格清除标注
        /// </summary>
        /// <param name="row">单元格所在行</param>
        /// <param name="col">单元格所在列</param>
        public void ClearCommentInCell(int row, int col)
        {
            if (_xls.GetComment(row, col).Length > 0)
            {
                // 设置单元格背景色
                SetDefaultBackground(row, col);

                // 清除标注内容
                _xls.SetComment(row, col, string.Empty);
            }
        }

        public void ClearAllComment()
        {
            int rowCount = _xls.RowCount;
            int colCount = _xls.ColCount;

            for (int rowIndex = 1; rowIndex <= rowCount; rowIndex++)
            {
                for (int colIndex = 1; colIndex <= colCount; colIndex++)
                {
                    ClearCommentInCell(rowIndex, colIndex);
                }
            }
        }

        /// <summary>
        /// 为T类型，创建报表模版
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="startRow">“列标题”的开始行</param>
        /// <param name="startColumn">“列标题”的开始列</param>
        /// <param name="mapPropertyToColumnTitle">对象属性和列标题的映射关系</param>
        public void CreateExcelReportTemlate<T>(int startRow, int startColumn, Func<PropertyInfo, string> mapPropertyToColumnTitle) where T : class,new()
        {
            // 属性集合
            int columnCount = 0;
            int titleRowIndex = startRow;
            int dataRowIndex = titleRowIndex + 1;
            PropertyInfo[] propertyArray = GetPropertyArrayToMap<T>(mapPropertyToColumnTitle);
            foreach (PropertyInfo property in propertyArray)
            {
                columnCount++;

                object[] attributes = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                string propertyName = property.Name;
                string displayName = ((DisplayNameAttribute)attributes[0]).DisplayName;

                // 设置列标题
                int titleColumnIndex = columnCount + (startColumn - 1);
                SetTitleCell(titleRowIndex, titleColumnIndex, displayName);

                // 设置属性绑定标记
                int dataColumnIndex = titleColumnIndex;
                SetDataBindCell(dataRowIndex, dataColumnIndex, propertyName);
            }

            // 定义名称区域
            char[] excelColumn = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            string 数据表区域 = string.Format("='{0}'!${2}${1}:${3}${1}", _sheetName, startRow + 1, excelColumn[startColumn - 1], excelColumn[columnCount - 1]);
            _xls.SetNamedRange(new TXlsNamedRange("__数据表__", 0, 0, 数据表区域));

            string 打印区域 = string.Format("='{0}'!$A$1:${2}${1}", _sheetName, startRow + 3, excelColumn[columnCount]);
            _xls.SetNamedRange(new TXlsNamedRange(TXlsNamedRange.GetInternalName(InternalNameRange.Print_Area), 1, 32, 打印区域));
        }

        /// <summary>
        /// 为T类型，创建报表模版
        /// 默认映射关系："对象属性的DisplayName" -> "列标题"
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="startRow">“列标题”的开始行（默认为第二行）</param>
        /// <param name="startColumn">“列标题”的开始列（默认为第一列）</param>
        public void CreateExcelReportTemlate<T>(int startRow = 2, int startColumn = 1) where T : class,new()
        {
            Func<PropertyInfo, string> MapPropertyToColumnTitle = property =>
            {
                object[] attributes = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                return attributes.Count() > 0
                    ? ((DisplayNameAttribute)attributes[0]).DisplayName
                    : string.Empty;
            };

            CreateExcelReportTemlate<T>(startRow, startColumn, MapPropertyToColumnTitle);
        }

        private PropertyInfo[] GetPropertyArrayToMap<T>(Func<PropertyInfo, string> mapPropertyToColumnTitle)
        {
            return typeof(T).GetProperties().Where(p => p.CanRead && !string.IsNullOrEmpty(mapPropertyToColumnTitle(p))).ToArray();
        }

        private TRichString GetCommentStyle(string comment)
        {
            TRTFRun[] Runs;
            Runs = new TRTFRun[2];
            Runs[0].FirstChar = 0;
            TFlxFont fnt;
            fnt = _xls.GetDefaultFont;
            fnt.Size20 = 180;
            fnt.Color = TExcelColor.Automatic;
            fnt.Style = TFlxFontStyles.Bold;
            fnt.Family = 0;
            Runs[0].FontIndex = _xls.AddFont(fnt);
            Runs[1].FirstChar = comment.Length;
            fnt = _xls.GetDefaultFont;
            Runs[1].FontIndex = _xls.AddFont(fnt);
            //TRTFRun[] Runs = new TRTFRun[3];
            //Runs[0].FirstChar = 0;
            //TFlxFont fnt = _xls.GetDefaultFont;
            //fnt.Size20 = 180;
            //fnt.Color = TExcelColor.Automatic;
            //fnt.Style = TFlxFontStyles.Bold;
            //fnt.Family = 3;
            //fnt.CharSet = 134;
            //fnt.Scheme = TFontScheme.None;
            //Runs[0].FontIndex = _xls.AddFont(fnt);
            //Runs[1].FirstChar = 7;
            //fnt = _xls.GetDefaultFont;
            //fnt.Size20 = 180;
            //fnt.Color = TExcelColor.Automatic;
            //fnt.Family = 3;
            //fnt.CharSet = 134;
            //fnt.Scheme = TFontScheme.None;
            //Runs[1].FontIndex = _xls.AddFont(fnt);
            //Runs[2].FirstChar = 13;
            //fnt = _xls.GetDefaultFont;
            //Runs[2].FontIndex = _xls.AddFont(fnt);

            return new TRichString(comment, Runs, _xls);
        }
        private void SetDefaultBackground(int row, int col)
        {
            TFlxFormat fmt = _xls.GetCellVisibleFormatDef(row, col);
            fmt.FillPattern.Pattern = TFlxPatternStyle.None;
            fmt.FillPattern.FgColor = TExcelColor.Automatic;
            _xls.SetCellFormat(row, col, _xls.AddFormat(fmt));
        }

        private void SetTitleCell(int row, int col, string diaplayName)
        {
            SetCellFormat(row, col, true, cellColor: 0xC0C0C0, fontColor: 0xFFFF00);
            _xls.SetCellValue(row, col, diaplayName);
        }
        private void SetDataBindCell(int row, int col, string propertyName)
        {
            SetCellFormat(row, col, false, fontColor: 0xFF0000);
            _xls.SetCellValue(row, col, string.Format("<#数据表.{0}>", propertyName));
        }
        private void SetCellFormat(int row, int col, bool isFontBold, bool isAlignCenter = true, int fontColor = -1, int cellColor = -1)
        {
            TFlxFormat fmt = _xls.GetCellVisibleFormatDef(row, col);

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

            _xls.SetCellFormat(row, col, _xls.AddFormat(fmt));
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
                    _xls.SelectCell(1, 1, false);
                    _xls.Save(_savePath);
                    _xls = null;
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