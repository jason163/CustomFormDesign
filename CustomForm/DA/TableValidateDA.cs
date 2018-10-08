using Nesoft.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.DA
{
    /// <summary>
    /// 系统表DA
    /// </summary>
    public class TableValidateDA
    {
        /// <summary>
        /// 根据数据名和表名查询列集合
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable QueryTableColumns(string dbName,string tableName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryTableColumns");
            cmd.SetParameterValue("@DBName", dbName);
            cmd.SetParameterValue("@TableName", tableName);
            return cmd.ExecuteDataTable();
        }
    }
}
