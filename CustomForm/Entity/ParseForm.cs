using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomForm.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomFormInfo
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 表单ID
        /// </summary>
        public string Formid { get; set; }

        public string FormCode { get; set; }

        public string FormName { get; set; }

        public string FormDesc { get; set; }

        /// <summary>
        /// 表单内容
        /// </summary>
        public string ParseForm { get; set; }
    }

    public class ParseForm
    {
        /// <summary>
        /// 总字段数
        /// </summary>
        public int FieldCount { get; set; }

        /// <summary>
        /// 完整html
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 控件替换为{data_1}的html
        /// </summary>
        public string Parse { get; set; }

        

        /// <summary>
        /// 所有控件
        /// </summary>
        public List<ControllerInfo> Data { get; set; }

        /// <summary>
        /// 新增控件列表
        /// </summary>
        public List<ControllerInfo> AddFileds
        {
            get
            {
                return this.Data.Where(p => p.IsNew == 1).ToList();
            }
        }

    }

    public class ControllerInfo : FiledInfo
    {
        /// <summary>
        /// 是否新增加控件
        /// </summary>
        public int IsNew { get; set; }

        public string Style { get; set; }

        /// <summary>
        /// 控件HTML
        /// </summary>
        public string Content { get; set; }

        #region Select&CheckBox
        public string Selected { get; set; }
        public string Checked { get; set; }
        #endregion
    }

    /// <summary>
    /// 表单字段信息
    /// </summary>
    public class FiledInfo
    {
        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// 使用此字段作为列名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 替换名
        /// </summary>
        public string parse_name { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public string LeipiPlugins { get; set; }
    }
}