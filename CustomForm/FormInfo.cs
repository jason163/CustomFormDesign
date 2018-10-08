using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm
{
    /// <summary>
    /// 动态表实体
    /// </summary>
    public class DynamicTableInfo
    {
        public DynamicTableInfo()
        {
            this.ColumnAndValues = new Dictionary<string, object>();
        }
        public int SysNo { get; set; }
        public string DBName { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// 动态表所属模板编号
        /// </summary>
        public int FormMasterSysNo
        {
            get
            {
                int.TryParse(ColumnAndValues["FormMasterSysNo"].ToString(), out int formSysNo);
                return formSysNo;
            }
        }
        /// <summary>
        /// 动态表拥有者
        /// </summary>
        public int CustomerSysNo
        {
            get
            {
                int.TryParse(ColumnAndValues["CustomerSysNo"].ToString(), out int customerSysNo);
                return customerSysNo;
            }
        }

        public Dictionary<string,object> ColumnAndValues { get; set; }

    }

   
}
