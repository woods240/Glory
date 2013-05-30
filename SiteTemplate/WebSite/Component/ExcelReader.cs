using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlexCel.XlsAdapter;
using FlexCel.Core;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace WebSite
{
    /// <summary>
    /// Excel读取器
    /// </summary>
    public class ExcelReader : IDisposable
    {
        public string FilePhysicalPath { get; private set; }
        public string SheetName { get; private set; }
        public bool HasTitle { get; private set; }

        private XlsFile xls;

        public IEnumerable<CellValue> CellValueCollection { get; private set; }
        public int MinRowIndex { get { return CellValueCollection.Min(c => c.Row); } }
        public int MaxRowIndex { get { return CellValueCollection.Max(c => c.Row); } }
        public int MinColIndex { get { return CellValueCollection.Min(c => c.Col); } }
        public int MaxColIndex { get { return CellValueCollection.Max(c => c.Col); } }
        public int ColumnRowIndex { get { return HasTitle ? (MinRowIndex + 1) : MinRowIndex; } }
        public int DataRowIndex { get { return ColumnRowIndex + 1; } }

        public ExcelReader(string filePhysicalPath, string sheetName, bool hasTitle)
        {
            FilePhysicalPath = filePhysicalPath;
            SheetName = sheetName;
            HasTitle = hasTitle;

            xls = new XlsFile(false);
            xls.Open(FilePhysicalPath);
            ChangeSheet(SheetName);
        }

        /// <summary>
        /// 切换标签页
        /// </summary>
        public void ChangeSheet(string sheetName)
        {
            SheetName = sheetName;
            xls.ActiveSheetByName = SheetName;
            CellValueCollection = xls.AsEnumerable();
        }

        /// <summary>
        /// 读取报表标题（通常在[第一行，第一列]）
        /// </summary>
        /// <returns>报表标题</returns>
        public string GetExcelTitle()
        {
            if (!HasTitle) return string.Empty;

            int titleRowIndex = MinRowIndex; // 标题行
            return CellValueCollection.FirstOrDefault(c => c.Row == MinRowIndex && c.Col == MinColIndex).Value.ToString();
        }

        /// <summary>
        /// 将Excel中的“数据表”部分，读取到泛型集合中
        /// </summary>
        /// <typeparam name="T">集合元素的类型</typeparam>
        /// <returns>泛型集合</returns>
        public List<T> Read<T>() where T : class, new()
        {
            // 属性集合
            IEnumerable<PropertyInfo> propertyArray = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(DisplayNameAttribute), true).Length > 0);

            // 列标题集合
            List<CellValue> columnTitleCells = CellValueCollection.Where(c => c.Row == ColumnRowIndex).ToList();

            // 遍历数据行
            List<T> modelList = new List<T>();
            for (int rowIndex = DataRowIndex; rowIndex <= MaxRowIndex; rowIndex++)
            {
                object model = Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo property in propertyArray)
                {
                    SetPropertyValue(model, property, rowIndex, columnTitleCells);
                }
                modelList.Add(model as T);
            }

            return modelList;
        }

        private void SetPropertyValue(object model, PropertyInfo property, int rowIndex, List<CellValue> columnTitleCells)
        {
            // DisplayName -> columnTitle -> columnIndex
            object[] attributes = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            string displayName = ((DisplayNameAttribute)attributes[0]).DisplayName;
            CellValue cell = columnTitleCells.FirstOrDefault(c => c.Value.ToString().Equals(displayName));
            if (cell == null) return;
            int colIndex = cell.Col;

            // 查找对应单元格的数据
            object value = CellValueCollection.FirstOrDefault(c => c.Row == rowIndex && c.Col == colIndex).Value;

            // 格式转换
            object propertyValue = null;
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
                    CellValueCollection = null;
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