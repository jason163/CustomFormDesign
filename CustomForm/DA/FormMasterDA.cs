using CustomForm.Entity;
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
    /// 表单模板维护
    /// </summary>
    public class FormMasterDA
    {
        #region 表单模板维护
        /// <summary>
        /// 创建FormMaster信息
        /// </summary>
        public static int InsertFormMaster(FormMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertFormMaster");
            cmd.SetParameterValue<FormMaster>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }
        /// <summary>
        /// 更新FormMaster信息
        /// </summary>
        public static int UpdateFormMaster(FormMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateFormMaster");
            cmd.SetParameterValue<FormMaster>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }
        /// <summary>
        /// 获取单个FormMaster信息
        /// </summary>
        public static FormMaster LoadFormMaster(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadFormMaster");
            cmd.SetParameterValue("@SysNo", sysNo);
            FormMaster result = cmd.ExecuteEntity<FormMaster>();
            return result;
        }
        #endregion

    }
}
