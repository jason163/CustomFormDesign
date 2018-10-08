using CustomForm.Entity;
using Nesoft.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.DA
{
    /// <summary>
    /// 动态表DA
    /// </summary>
    public class FormDA
    {
        public static List<dynamic> GetDynamicData()
        {
            System.Dynamic.ExpandoObject dfd = new System.Dynamic.ExpandoObject();
            
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadData");
            var dt = cmd.ExecuteDataTable();

            return dt.ToExPando();
        }

        #region 动态表结构维护
        /// <summary>
        /// 创建表单数据库结构
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static int CreateFormTable(CustomTableInfo tableInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateFormTable");
            cmd.SetParameterValue("@DBName", tableInfo.DBName);
            cmd.SetParameterValue("@TableName", tableInfo.TableName);

            StringBuilder columns = new StringBuilder();
            foreach (var field in tableInfo.Columns.OrderBy(p => p.Prority).ToList())
            {
                if (field.IsKey)
                {
                    columns.AppendFormat(string.Format(" {0} {1} IDENTITY(1,1) NOT NULL ,", field.Name, field.Type));
                }
                else
                {
                    columns.AppendLine(string.Format(" {0} {1} NULL ,", field.Name, field.Type));
                    if (field.IsIndex)
                    {
                        cmd.SetParameterValue("@IndexColumn", field.Name);
                    }
                }

            }
            string strColumns = columns.ToString();
            strColumns = strColumns.Substring(0, strColumns.Length - 1);
            cmd.SetParameterValue("@Columns", strColumns);

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// 更新表单数据库结构
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static int UpdateFormTable(CustomTableInfo tableInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateFormTable");
            cmd.SetParameterValue("@DBName", tableInfo.DBName);
            cmd.SetParameterValue("@TableName", tableInfo.TableName);

            StringBuilder columns = new StringBuilder();
            foreach (var field in tableInfo.Columns)
            {
                columns.AppendFormat(string.Format(" {0} {1},", field.Name, field.Type));
            }
            string strColumns = columns.ToString();
            strColumns = strColumns.Substring(0, strColumns.Length - 1);
            cmd.SetParameterValue("@AddColumns", strColumns);

            return cmd.ExecuteNonQuery();

        }
        #endregion


        #region 动态表数据维护
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="formInfo"></param>
        /// <returns></returns>
        public static int SaveFormData(DynamicTableInfo formInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaveFormData");
            cmd.SetParameterValue("@TableName", string.Format("{0}", formInfo.TableName));
            List<string> cols = new List<string>();
            List<object> vals = new List<object>();
            foreach (var item in formInfo.ColumnAndValues)
            {
                cols.Add(item.Key);
                if (int.TryParse(item.Value.ToString(), out int val))
                {
                    vals.Add(val);
                }
                else
                {
                    vals.Add(string.Format("'{0}'", item.Value));
                }
            }
            cmd.SetParameterValue("@Columns", string.Join(",", cols));

            cmd.SetParameterValue("@ColumnValues", string.Join(",", vals));

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// 更新动态表数据
        /// </summary>
        /// <param name="formInfo"></param>
        /// <returns></returns>
        public static int UpdateFormData(DynamicTableInfo formInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateFormData");
            cmd.SetParameterValue("@DBName", string.Format("{0}", formInfo.DBName));
            cmd.SetParameterValue("@TableName", string.Format("{0}", formInfo.TableName));
            List<string> cols = new List<string>();
            foreach (var item in formInfo.ColumnAndValues)
            {
                cols.Add(string.Format(" {0}='{1}'", item.Key.Trim(), item.Value.ToString().Trim()));
            }
            cmd.SetParameterValue("@ColumnValues", string.Join(",", cols));

            cmd.SetParameterValue("@SysNo", formInfo.SysNo);

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// 加载表单数据
        /// </summary>
        /// <param name="formInfo"></param>
        /// <returns></returns>
        public static DataTable LoadFormData(DynamicTableInfo formInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadFormData");
            cmd.SetParameterValue("@DBName", string.Format("{0}", formInfo.DBName));
            cmd.SetParameterValue("@TableName", string.Format("{0}", formInfo.TableName));

            cmd.SetParameterValue("@FormMasterSysNo", formInfo.FormMasterSysNo);

            cmd.SetParameterValue("@CustomerSysNo", formInfo.CustomerSysNo);

            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// 根据表单系统编号加载动态数据
        /// </summary>
        /// <param name="formInfo"></param>
        /// <returns></returns>
        public static DataTable LoadFormBySysNo(DynamicTableInfo formInfo)
        {
            //LoadFormDataBySysNo
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadFormDataBySysNo");
            cmd.SetParameterValue("@DBName", string.Format("{0}", formInfo.DBName));
            cmd.SetParameterValue("@TableName", string.Format("{0}", formInfo.TableName));

            cmd.SetParameterValue("@SysNo", formInfo.SysNo);

            return cmd.ExecuteDataTable();
        }
        #endregion


    }
}
