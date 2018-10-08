using CustomForm.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm
{
    public class CustomFormUtils
    {
        /// <summary>
        /// 根据控件输入的值和控件模板反向解析HTML
        /// </summary>
        /// <param name="formMaster">模板</param>
        /// <param name="formData">控件值</param>
        /// <returns>反向解析生成的HTML</returns>
        public static string UnparseForm(FormMaster formMaster,DataTable formData)
        {
            // 如果动态表中没有相应数据，则直接返回HTML模板
            if (formData == null || formData.Rows == null || formData.Rows.Count < 1)
                return formMaster.Template;

            Dictionary<string, string> formDic = new Dictionary<string, string>();
            foreach (DataColumn column in formData.Columns)
            {
                formDic.Add(column.ColumnName, formData.Rows[0][column.ColumnName].ToString());
            }

            string content_parse = formMaster.ParseTemplate;
            // 表格样式
            content_parse = content_parse.Replace("<table", "table class=\"table table - bordered\"");

            List<ControllerInfo> controllers = JsonConvert.DeserializeObject<List<ControllerInfo>>(formMaster.ControllerTemplate);

            Dictionary<string, string> tempController = new Dictionary<string, string>();

            Parallel.ForEach(controllers, (item) => {
                //对每个控件进行处理
                // 控件HTML内容
                string content = item.Content;
                // 控件名称
                string controllerName = item.Title;
                // 控件对应的值
                string controllerValue = formDic[controllerName];
                switch(item.LeipiPlugins)
                {
                    case "text":
                        content = string.Format("<input type=\"text\" value=\"{0}\" title=\"{1}\"  name=\"{1}\"  style=\"{2}\"/>", formDic[controllerName],controllerName,item.Style);
                        break;
                    case "textarea":
                        content = string.Format("<textarea  name=\"{0}\" title=\"{0}\" id=\"{0}\" value=\"{1}\" style=\"{2}\">{1}</textarea>", controllerName, formDic[controllerName], item.Style);
                        break;
                    case "select":
                        // 是否设置默认值
                        if (item.Selected.Equals("selected"))
                        {
                            content = content.Replace("selected=\"selected\"", "");
                        }
                        // 重新定默认值
                        content = content.Replace(string.Format("value=\"{0}\"", formDic[controllerName].Trim()), string.Format("value=\"{0}\" selected=\"selected\"", formDic[controllerName].Trim()));
                        break;
                    case "checkboxs":
                        if (item.Checked.Equals("checked", StringComparison.CurrentCultureIgnoreCase))
                        {
                            content = content.Replace("checked=\"checked\"", "");
                        }
                        string[] array = item.Value.Split(new char[] { ',' });
                        for(int i = 0; i < array.Length; i++)
                        {
                            // 重新定默认值
                            content = content.Replace(string.Format("value=\"{0}\"", formDic[controllerName].Trim()), string.Format("value=\"{0}\" selected=\"selected\"", formDic[controllerName].Trim()));
                        }
                        // 重新定默认值
                        content = content.Replace(string.Format("value=\"{0}\"", formDic[controllerName].Trim()), string.Format("value=\"{0}\" selected=\"selected\"", formDic[controllerName].Trim()));
                        break;
                }
                //k:data_1;v:content
                tempController.Add(item.Name, content);
                
            });
            
            // 替换占位符
            foreach(var item in controllers)
            {
                content_parse = content_parse.Replace(string.Format("{0}", "{"+item.Name+"}"), tempController[item.Name]);
            }

            return content_parse;

        }
    }
}
