using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.Entity
{
    /// <summary>
    /// 自定义表结构信息
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
            Columns.Add(new FieldInfo { Name = "SysNo", Value="系统编号", IsKey = true, Type = "INT" });

            Columns.Add(new FieldInfo { Name = "FormMasterSysNo", Value = "模板系统编号", Type = "INT",IsIndex=true });
            // 表单拥有者
            Columns.Add(new FieldInfo { Name = "CustomerSysNo", Value = "拥有者编号", Type = "INT", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "Status", Value = "状态", Type = "INT", Prority = 10 });
            // 表单维护者
            Columns.Add(new FieldInfo { Name = "InUser", Value = "创建人", Type = "INT", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "InUserName", Value = "创建人", Type = "NVARCHAR(20)", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "InDate", Value = "创建时间", Type = "DATETIME", Prority = 10 });
            // 表单编辑者
            Columns.Add(new FieldInfo { Name = "EditUser", Value = "编辑人", Type = "INT", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "EditUserName", Value = "编辑人", Type = "NVARCHAR(20)", Prority = 10 });
            Columns.Add(new FieldInfo { Name = "EditDate", Value = "编辑时间", Type = "DATETIME", Prority = 10 });
        }
        /// <summary>
        /// 清空默认列
        /// </summary>
        public void ClearDefaultColumns()
        {
            this.Columns = new List<FieldInfo>();
        }

        public int SysNo { get; set; }
        public string DBName { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// 创建时就是所有列；更新是为新增列
        /// </summary>
        public List<FieldInfo> Columns { get; set; }
    }

    /// <summary>
    /// 表字段信息
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

    /// <summary>
    /// 动态表数据库结构信息
    /// </summary>
    public class PropertyDBInfo
    {

        /// <summary>
        /// 编号：编号
        /// </summary>
        public int SysNo { get; set; }

        
        /// <summary>
        /// Schema名称：属性所在Schema名称
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 数据库名称：属性所在数据库名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 表名称：属性所在表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名：属性编号
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列名描述：属性描述
        /// </summary>
        public string ColumnDesc { get; set; }

        /// <summary>
        /// 数据类型：int,varchar
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 字符数据最大长度：字符数据最大长度
        /// </summary>
        public int? CharMAXLength { get; set; }
    }
}
