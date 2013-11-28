using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FlexCel.Core;
using FlexCel.XlsAdapter;

namespace WebSite
{
    /// <summary>
    /// Excel读取器
    /// </summary>
    public class ExcelReader : IDisposable
    {
        private XlsFile _xls = new XlsFile(false);
        private string _filePath;
        private bool _hasTitle;

        public ExcelReader(string filePath, bool hasTitle = false)
        {
            _filePath = filePath;
            _hasTitle = hasTitle;
        }

        /// <summary>
        /// 读取标签页的内容
        /// </summary>
        public IEnumerable<CellValue> Read(string sheetName)
        {
            _xls.Open(_filePath);
            _xls.ActiveSheetByName = sheetName;

            return _xls.ToList();
        }

        /// <summary>
        /// 将Excel中的“数据表”部分，反序列化成对象集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="sheetName">标签页名称</param>
        /// <param name="mapPropertyToColumnTitle">对象属性和列标题的映射关系</param>
        /// <returns>对象集合</returns>
        public List<T> Read<T>(string sheetName, Func<PropertyInfo, string> mapPropertyToColumnTitle) where T : class, new()
        {
            // 1.读取Excel单元格内容
            IEnumerable<CellValue> cells = Read(sheetName);
            int minRowIndex = cells.Min(c => c.Row);
            int maxRowIndex = cells.Max(c => c.Row);
            int minColIndex = cells.Min(c => c.Col);
            int maxColIndex = cells.Max(c => c.Col);
            int columnRowIndex = _hasTitle ? (minRowIndex + 1) : minRowIndex;
            int minDataRowIndex = columnRowIndex + 1;

            // 2.确定类中的哪些属性将被映射
            PropertyInfo[] propertyArray = GetPropertyArrayToMap<T>(mapPropertyToColumnTitle);

            // 3.建立 [属性,Excel列号] 的映射关系
            CellValue[] columnTitleCellsArray = cells.Where(c => c.Row == columnRowIndex).ToArray();
            Dictionary<string, int> columnTitleMapperDictionary = GetColumnTitleToColDictionary(columnTitleCellsArray);
            Dictionary<PropertyInfo, int> mapperDictionary = GetPropertyToColDictionary(propertyArray, columnTitleMapperDictionary, mapPropertyToColumnTitle);

            // 4.遍历数据行,将每一行数据映射为一个对象
            IGrouping<int, CellValue>[] recordCellsArray = cells.GroupBy(c => c.Row).Where(g => g.Key >= minDataRowIndex).ToArray();
            List<T> modelList = new List<T>();
            foreach (IGrouping<int, CellValue> recordCells in recordCellsArray)
            {
                object model = Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo property in propertyArray)
                {
                    CellValue cell = recordCells.FirstOrDefault(c => c.Col == mapperDictionary[property]);
                    object value = cell == null ? null : cell.Value;

                    SetPropertyValue(model, property, value);
                }
                modelList.Add(model as T);
            }

            return modelList;
        }

        /// <summary>
        /// 将Excel中的“数据表”部分，反序列化成对象集合
        /// 默认映射关系："对象属性的DisplayName" -> "列标题"
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="sheetName">标签页名称</param>
        /// <returns>对象集合</returns>
        public List<T> Read<T>(string sheetName) where T : class, new()
        {
            Func<PropertyInfo, string> MapPropertyToColumnTitle = property => MetadataReader.GetDisplayName(property);

            return Read<T>(sheetName, MapPropertyToColumnTitle);
        }

        private PropertyInfo[] GetPropertyArrayToMap<T>(Func<PropertyInfo, string> mapPropertyToColumnTitle)
        {
            return typeof(T).GetProperties().Where(p => p.CanWrite && !string.IsNullOrEmpty(mapPropertyToColumnTitle(p))).ToArray();
        }
        private void SetPropertyValue(object model, PropertyInfo property, object value)
        {
            object propertyValue;
            if (property.PropertyType == typeof(Guid))
            {
                propertyValue = value == null ? Guid.Empty : new Guid(value.ToString());
            }
            else
            {
                propertyValue = Convert.ChangeType(value, property.PropertyType);
            }
            property.SetValue(model, propertyValue, null);
        }

        private Dictionary<PropertyInfo, int> GetPropertyToColDictionary(PropertyInfo[] propertyArray, Dictionary<string, int> columnTitleMapperDictionary, Func<PropertyInfo, string> MapPropertyToColumnTitle)
        {
            Dictionary<PropertyInfo, int> mapperDictionary = new Dictionary<PropertyInfo, int>();

            foreach (PropertyInfo property in propertyArray)
            {
                string columnTitle = MapPropertyToColumnTitle(property);

                int col = 0;
                if (columnTitleMapperDictionary.Keys.Contains(columnTitle))
                {
                    col = columnTitleMapperDictionary[columnTitle];
                }
                mapperDictionary.Add(property, col);
            }

            return mapperDictionary;
        }
        private Dictionary<string, int> GetColumnTitleToColDictionary(CellValue[] columnTitleCellsArray)
        {
            Dictionary<string, int> columnTitleMapperDictionary = new Dictionary<string, int>();

            foreach (CellValue cell in columnTitleCellsArray)
            {
                if (cell.Value != null)
                {
                    columnTitleMapperDictionary.Add(cell.Value.ToString().Trim(), cell.Col);
                }
            }

            return columnTitleMapperDictionary;
        }


        #region IDisposable 成员

        ~ExcelReader()
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
                    _filePath = null;
                }

                // 释放非托管资源 
                _xls = null;

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