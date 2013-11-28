using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlexCel.Core;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;

namespace WebSite
{
    public abstract class ExcelInterpreter
    {
        private string _excelPath;
        private string _sheetName;
        private Dictionary<string, int> _columnMapperDictionary = new Dictionary<string, int>();

        public ExcelInterpreter(string excelPath, string sheetName)
        {
            _excelPath = excelPath;
            _sheetName = sheetName;
        }


        #region 与Excel文件交互

        private IEnumerable<CellValue> ReadExcel()
        {
            using (ExcelReader reader = new ExcelReader(_excelPath))
            {
                return reader.Read(_sheetName);
            }
        }

        private void AddErrorCommentToExcel(List<CellError> cellErrors)
        {
            using (ExcelWriter writer = new ExcelWriter(_excelPath, _sheetName))
            {
                writer.ClearAllComment();
                foreach (CellError cellError in cellErrors)
                {
                    writer.AddCommentToCell(cellError.RowIndex, cellError.ColIndex, "", cellError.ErrorInfo);
                }
            }
        }

        private void AddErrorCommentToExcel(CellError cellError)
        {
            using (ExcelWriter writer = new ExcelWriter(_excelPath, _sheetName))
            {
                writer.ClearAllComment();
                writer.AddCommentToCell(cellError.RowIndex, cellError.ColIndex, "", cellError.ErrorInfo);
            }
        }

        #endregion


        #region 正确性检查

        protected abstract List<CellError> ValidateRecordCorrectness(IEnumerable<CellValue> rowCells, int rowIndex, out Dictionary<string, object> columnsValueDictionary);

        #endregion


        #region 重复性检查

        protected string[] PickupRepeateElementsFromSelf(string[] allElements)
        {
            string[] distinctElements = allElements.Distinct().ToArray();
            return distinctElements.Where(e => allElements.Count(a => a == e) > 1).ToArray();
        }

        protected string[] PickupRepeateElementsFromAnother(string[] myElements, string[] anotherElements)
        {
            string[] distinctElements = myElements.Distinct().ToArray();
            return distinctElements.Where(e => anotherElements.Contains(e)).ToArray();
        }

        protected virtual List<CellError> ValidateRecordRepetitiveness(IEnumerable<CellValue> dataCells, List<Dictionary<string, object>> recordList)
        {
            return new List<CellError>();
        }

        protected void CreateRepeativeCellErrors(int colIndex, IEnumerable<int> rowIndexCollection, string errorInfo, ref List<CellError> cellErrors)
        {
            foreach (int rowIndex in rowIndexCollection)
            {
                CellError cellError = cellErrors.FirstOrDefault(c => c.ColIndex == colIndex && c.RowIndex == rowIndex);
                if (cellError != null)
                {
                    cellError.ErrorInfo += "\n" + errorInfo;
                }
                else
                {
                    cellErrors.Add(new CellError(rowIndex, colIndex, errorInfo));
                }
            }
        }

        #endregion


        #region 解读数据的意义

        protected abstract string[] GetAllColumns();

        protected abstract string[] GetAllDateTimeColumns();

        private bool IsDateTimeColumn(string columnName)
        {
            string[] g_dateTimeColumns = GetAllDateTimeColumns();
            return g_dateTimeColumns.Contains(columnName);
        }

        private bool MapExcelByColumnCells(IEnumerable<CellValue> columnCells)
        {
            // 1.检查字段是否齐全
            string[] g_allColumns = GetAllColumns();
            string[] columnNameArray = columnCells.Select(c => Convert.ToString(c.Value)).ToArray();
            bool lackColumns = g_allColumns.Any(c => !columnNameArray.Contains(c));
            if (lackColumns)
            {
                return false;
            }

            // 2.用定义好的 "columnName" 去映射Excel文件中的 "colIndex" 
            foreach (CellValue columnCell in columnCells)
            {
                string columnName = Convert.ToString(columnCell.Value);
                if (g_allColumns.Contains(columnName))
                {
                    _columnMapperDictionary.Add(columnName, columnCell.Col);
                }
            }

            return true;
        }

        private string ConvertToDateTime(string date)
        {
            if (date == string.Empty)
            {
                return string.Empty;
            }

            if (date.Contains('年'))
            {
                date = date.Replace('年', '.');
            }
            if (date.Contains("月"))
            {
                date = date.Replace('月', '.');
            }
            if (date.Contains("日"))
            {
                date = date.Replace('日', '.');
            }
            if (date.Contains("-"))
            {
                date = date.Replace('-', '.');
            }
            if (date.Contains("。"))
            {
                date = date.Replace("。", ".");
            }
            if (date.Contains("/"))
            {
                date = date.Replace("/", ".");
            }
            if (date.Contains("\\"))
            {
                date = date.Replace("\\", ".");
            }
            if (date.Contains("、"))
            {
                date = date.Replace("、", ".");
            }

            date = date.Trim('.');
            if (date.Contains("."))
            {
                string[] result = date.Split(new char[] { '.' });
                if (result.Count() == 2)
                {
                    string newdate = result[0] + "/" + result[1] + "/" + "1";
                    return newdate;
                }
                return ConvertStringToShortDateTimeString(date);
            }
            else
            {
                if (!IsDigit(date))
                    return date;

                int days = int.Parse(date);

                if (days > 0 && days < 3000)
                {
                    return days.ToString(CultureInfo.InvariantCulture) + "/1/1";
                }
                if (days >= -657435.0 && days <= 2958466.0)
                {
                    return DateTime.FromOADate(days).ToShortDateString();
                }
                else
                {
                    return days.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private string ConvertStringToShortDateTimeString(string date)
        {
            DateTime resultDateTime;
            if (DateTime.TryParse(date, out resultDateTime))
            {
                return DateTime.Parse(date).ToShortDateString();
            }

            return date;
        }

        private bool IsDigit(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return false;

            foreach (char c in dateString)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        protected int GetColumnIndexByName(string columnName)
        {
            if (_columnMapperDictionary.Keys.Contains(columnName))
            {
                return _columnMapperDictionary[columnName];
            }

            throw new KeyNotFoundException(columnName);
        }

        protected object GetColumnValueFromCells(string columnName, IEnumerable<CellValue> rowCells)
        {
            int colIndex = GetColumnIndexByName(columnName);
            CellValue cellValue = rowCells.FirstOrDefault(c => c.Col == colIndex);

            string value = cellValue != null ? Convert.ToString(cellValue.Value) : string.Empty;
            if (IsDateTimeColumn(columnName))
            {
                value = ConvertToDateTime(Convert.ToString(value));
            }

            return value;
        }

        protected Dictionary<string, object> GetRecordFromCells(IEnumerable<CellValue> rowCells)
        {
            Dictionary<string, object> propertyDictionary = new Dictionary<string, object>();

            string[] g_allColumns = GetAllColumns();
            foreach (string columnName in g_allColumns)
            {
                object columnValue = GetColumnValueFromCells(columnName, rowCells);
                propertyDictionary.Add(columnName, columnValue);
            }

            return propertyDictionary;
        }

        public List<T> GetEntityList<T>(Func<PropertyInfo, string> mapPropertyToColumnTitle, bool strictMode = true) where T : class, new()
        {
            List<T> entityList = new List<T>();

            // 1.读取单元格内容
            IEnumerable<CellValue> cells = ReadExcel();
            int minRow = cells.Min(c => c.Row);
            int maxRow = cells.Max(c => c.Row);
            int minCol = cells.Min(c => c.Col);
            IEnumerable<CellValue> columnCells = cells.Where(c => c.Row == minRow);
            IEnumerable<CellValue> dataCells = cells.Where(c => c.Row > minRow);

            // 2.模版格式验证
            bool mapSuccess = MapExcelByColumnCells(columnCells);
            if (!mapSuccess)
            {
                AddErrorCommentToExcel(new CellError(minRow + 1, 1, "文件模板不正确"));
                return new List<T>();
            }
            else if (maxRow == minRow)
            {
                AddErrorCommentToExcel(new CellError(minRow + 1, 1, "文件中没有记录"));
                return new List<T>();
            }

            // 3.数据正确性验证
            List<Dictionary<string, object>> recordList = new List<Dictionary<string, object>>();
            List<CellError> cellErrors = new List<CellError>();
            for (int row = minRow + 1; row <= maxRow; row++)     // 对每行记录进行正确性验证
            {
                IEnumerable<CellValue> rowCells = dataCells.Where(c => c.Row == row);
                Dictionary<string, object> columnsValueDictionary;
                List<CellError> cellErrorsInRow = ValidateRecordCorrectness(rowCells, row, out columnsValueDictionary);

                if (cellErrorsInRow.Count > 0)
                {
                    cellErrors.AddRange(cellErrorsInRow);
                }
                else
                {
                    recordList.Add(columnsValueDictionary);
                }
            }

            // 4.数据重复性验证
            List<CellError> repeativeErrors = ValidateRecordRepetitiveness(dataCells, recordList);
            if (repeativeErrors.Count > 0)
            {
                cellErrors.AddRange(repeativeErrors);
            }

            // 5.将错误信息标记回文件
            if (cellErrors.Count > 0)
            {
                AddErrorCommentToExcel(cellErrors);

                if (strictMode)
                {
                    return new List<T>();   // 严格模式要求文件中不能有任何错误
                }
            }

            // 6.将记录转换为实体
            List<T> modelList = new List<T>();
            PropertyInfo[] propertyArray = typeof(T).GetProperties().Where(p => p.CanWrite && !string.IsNullOrEmpty(mapPropertyToColumnTitle(p))).ToArray();
            foreach (Dictionary<string, object> record in recordList)
            {
                object model = Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo property in propertyArray)
                {
                    string columnName = mapPropertyToColumnTitle(property);
                    object value = record[columnName];

                    object propertyValue;
                    if (property.PropertyType == typeof(Guid))
                    {
                        propertyValue = new Guid(value.ToString());
                    }
                    else
                    {
                        propertyValue = Convert.ChangeType(value, property.PropertyType);
                    }
                    property.SetValue(model, propertyValue, null);
                }
                modelList.Add(model as T);
            }

            return modelList;
        }

        public List<T> GetEntityList<T>(bool strictMode = true) where T : class, new()
        {
            Func<PropertyInfo, string> MapPropertyToColumnTitle = property => MetadataReader.GetDisplayName(property);

            return GetEntityList<T>(MapPropertyToColumnTitle, strictMode);
        }

        #endregion

    }

    public class CellError
    {
        public CellError(int rowIndex, int colIndex)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }

        public CellError(int rowIndex, int colIndex, string errorInfo)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
            ErrorInfo = errorInfo;
        }

        public int RowIndex { get; private set; }

        public int ColIndex { get; private set; }

        public string ErrorInfo { get; set; }
    }
}