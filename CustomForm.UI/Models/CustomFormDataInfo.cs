﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomForm.UI.Models
{
    /// <summary>
    /// 保存自定义表单实体
    /// </summary>
    public class CustomFormDataInfo
    {
        /// <summary>
        /// 表单模板
        /// </summary>
        public int TemplateID { get; set; }

        /// <summary>
        /// 表单用户
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 自定义表单数据
        /// </summary>
        public dynamic Data { get; set; }
    }
}