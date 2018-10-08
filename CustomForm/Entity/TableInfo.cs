using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.Entity
{
    /// <summary>
    /// 自定义表信息
    /// </summary>
    public class CustomTableInfo
    {
        public CustomTableInfo()
        {
            Columns = new List<FieldInfo>();
            

        }
        /// <summary>
        /// 加载默认字段
        /// </summary>
        public void InitDefaultColumns()
        {
            // 默认字段
            Columns.Add(new FieldInfo { Name = "SysNo", IsKey = true, Type = "INT" });

            Columns.Add(new FieldInfo { Name = "FormMasterSysNo",Type = "INT",IsIndex=true });
            // 表单拥有者
            Columns.Add(new FieldInfo { Name = "CustomerSysNo", Type = "INT", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "Status", Type = "INT", Prority = 10 });
            // 表单维护者
            Columns.Add(new FieldInfo { Name = "InUser", Type = "INT", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "InUserName", Type = "NVARCHAR(20)", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "InDate", Type = "DATETIME", Prority = 10 });
            // 表单编辑者
            Columns.Add(new FieldInfo { Name = "EditUser", Type = "INT", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "EditUserName", Type = "NVARCHAR(20)", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "EditDate", Type = "DATETIME", Prority = 10 });
        }
        /// <summary>
        /// 清空默认列
        /// </summary>
        public void ClearDefaultColumns()
        {
            this.Columns = new List<FieldInfo>();
        }

        public int SysNo { get; set; }
        public String DBName { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// 创建时就是所有列；更新是为新增列
        /// </summary>
        public List<FieldInfo> Columns { get; set; }
    }

    /// <summary>
    /// 字段信息
    /// </summary>
    public class FieldInfo
    {
        private bool _isKey = false;
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsKey
        {
            get { return _isKey; }
            set { _isKey = value; }
        }

        private bool _isIndex = false;
        /// <summary>
        /// 是否索引字段
        /// </summary>
        public bool IsIndex
        {
            get { return _isIndex; }
            set { _isIndex = value; }
        }

        public string Name { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// NVARCHAR(200)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 字段顺序
        /// </summary>
        public int Prority { get; set; }
    }
}
