using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.Entity
{
    public class FormMaster
    {
        public FormMaster()
        {
            Status = 1;
            InUser = 0;
            InUserName = "test";
            InDate = DateTime.Now;
        }

        /// <summary>
        /// 表单编号：表单编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 表单分类：如：按科室分类
        /// </summary>
        public int FormType { get; set; }

        /// <summary>
        /// 菜单父级编号：
        /// </summary>
        public int ParentSysNo { get; set; }

        /// <summary>
        /// 表单Code：用于动态生成表单数据库表
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 表单名称：表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表单描述：表单描述
        /// </summary>
        public string FormDesc { get; set; }

        /// <summary>
        /// 表单模板HTML：
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 控件替换的表单模板：控件替换为{data_1}的html
        /// </summary>
        public string ParseTemplate { get; set; }

        /// <summary>
        /// //控件HTML列表
        /// </summary>
        public string ControllerTemplate { get; set; }

        /// <summary>
        /// 模板控件数
        /// </summary>
        public int FieldCount
        {
            get;set;
        }

        /// <summary>
        /// 表单优先级：表单优先级，ParentSysNo不为空时才有用
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 是否叶子节点：是否叶子节点
        /// </summary>
        public int IsLeaf { get; set; }

        /// <summary>
        /// 表单状态：表单状态 无效 -1；有效 1；
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建人编号：
        /// </summary>
        public int InUser { get; set; }

        /// <summary>
        /// 创建人：
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间：
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 创建时间：
        /// </summary>
        public string InDateStr { get { return InDate.ToString("yyyy-MM-dd HH:mm:ss"); } }

        /// <summary>
        /// 编辑人编号：
        /// </summary>
        public int? EditUser { get; set; }

        /// <summary>
        /// 编辑人：
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 编辑时间：
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 编辑时间：
        /// </summary>
        public string EditDateStr { get { return EditDate.HasValue ? EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }
    }
}
