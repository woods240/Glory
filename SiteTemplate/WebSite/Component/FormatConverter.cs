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

        public static DataTable ListToDataTable<T>(IEnumerable<T> list, Func<PropertyInfo, string> mapPropertyToColumnTitle) where T : class, new()
        {
            // 筛选泛型类的属性
            PropertyInfo[] requiredProperties = typeof(T).GetProperties().Where(p => p.CanRead && !string.IsNullOrEmpty(mapPropertyToColumnTitle(p))).ToArray();
            Dictionary<PropertyInfo, string> propertyToColumnTitleDictionary = new Dictionary<PropertyInfo, string>();

            // 构造Table的结构
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo property in requiredProperties)
            {
                string columnName = mapPropertyToColumnTitle(property);
                dataTable.Columns.Add(columnName, property.PropertyType);
                propertyToColumnTitleDictionary.Add(property, columnName);
            }

            // 向Table添加数据
            foreach (T model in list)
            {
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);

                foreach (PropertyInfo property in requiredProperties)
                {
                    string columnName = propertyToColumnTitleDictionary[property];
                    object propertyValue = property.GetValue(model, null);
                    row[columnName] = propertyValue;
                }
            }

            return dataTable;
        }


    }
}