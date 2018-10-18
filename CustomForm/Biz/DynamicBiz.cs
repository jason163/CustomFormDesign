using CustomForm.DA;
using CustomForm.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.Biz
{
    public static class DynamicBiz
    {
        public static void SaveDynamicTableInfo(List<PropertyDBInfo> properties)
        {
            properties.ForEach(property => {
                DynamicTableInfoDA.InsertTableColumns(property);
            });
        }

        public static List<PropertyDBInfo> GenerateByCustomTableInfo(CustomTableInfo customTableInfo)
        {
            List<PropertyDBInfo> rst = new List<PropertyDBInfo>();

            customTableInfo.Columns.ForEach(p => {
                PropertyDBInfo item = new PropertyDBInfo();
                item.DBName = customTableInfo.DBName;
                item.TableName = customTableInfo.TableName;
                item.ColumnName = p.Name;
                item.DataType = p.Type;
                item.ColumnDesc = p.Value;
                item.Schema = customTableInfo.Schema;

                rst.Add(item);
            });

            return rst;
        }

    }
}
