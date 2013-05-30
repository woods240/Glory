using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace WebSite
{
    /// <summary>
    /// 格式转换
    /// </summary>
    public static class FormatConverter
    {

        public static DataTable ListToDataTable<T>(IEnumerable<T> list, Func<PropertyInfo, bool> propertyFilter, bool useDisplayName = false) where T : class, new()
        {
            // 筛选泛型类的属性
            PropertyInfo[] propertyArray = typeof(T).GetProperties();
            IEnumerable<PropertyInfo> requiredProperties = propertyArray.Where(propertyFilter);

            // 构造Table的结构
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo property in requiredProperties)
            {
                string columnName = property.Name;
                if (useDisplayName)
                {
                    string displayName = MetadataReader.GetDisplayName(property);
                    if (!string.IsNullOrEmpty(displayName))
                    {
                        columnName = displayName;
                    }
                }

                dataTable.Columns.Add(columnName, property.PropertyType);
            }

            // 向Table添加数据
            foreach (T model in list)
            {
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);

                foreach (PropertyInfo property in requiredProperties)
                {
                    string columnName = property.Name;
                    if (useDisplayName)
                    {
                        string displayName = MetadataReader.GetDisplayName(property);
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            columnName = displayName;
                        }
                    }

                    object propertyValue = property.GetValue(model, null);
                    row[columnName] = propertyValue;
                }
            }

            return dataTable;
        }
    
    
    }
}