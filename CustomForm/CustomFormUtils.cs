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
                    case "radios":
                        StringBuilder tmp = new StringBuilder();
                        //"<span leipiplugins=\"radios\" name=\"data_3\" title=\"Sex\"><input type=\"radio\" name=\"data_3\" value=\"男\"  />男&nbsp;<input type=\"radio\" name=\"data_3\" value=\"女\"  />女&nbsp;</span>"
                        tmp.Append(string.Format("<span leipiplugins=\"{0}\" name=\"{1}\" title=\"{2}\">", item.LeipiPlugins, item.Name, item.Title));
                        string[] radiosArr = item.Value.Split(new char[] { ',' });
                        for (int i = 0; i < radiosArr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(radiosArr[i]) || radiosArr[i] == "")
                                continue;
                            tmp.Append(string.Format("<input type=\"radio\" name=\"{0}\" value=\"{1}\" {2} />{1}&nbsp;",item.Name, radiosArr[i], radiosArr[i].Equals(formDic[controllerName].Trim())?"checked":""));
                        }
                        tmp.Append("</span>");
                        content = tmp.ToString();
                        break;
                    case "checkboxs":
                        StringBuilder ckStr = new StringBuilder();
                        //"<span leipiplugins=\"checkboxs\" title=\"Favirote\"><input type=\"checkbox\" name=\"data_6\" value=\"A\"  />A&nbsp;<input type=\"checkbox\" name=\"data_7\" value=\"B\"  />B&nbsp;</span>"
                        ckStr.Append(string.Format("<span leipiplugins=\"{0}\" name=\"{1}\" title=\"{2}\">", item.LeipiPlugins, item.Name, item.Title));
                        string[] cksArr = item.Value.Split(new char[] { ',' });
                        for (int i = 0; i < cksArr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(cksArr[i]) || cksArr[i] == "")
                                continue;
                            bool isChecked = false;
                            if(formDic[controllerName].Split(new char[] { ',' }).Contains(cksArr[i]))
                            {
                                isChecked = true;
                            }
                            ckStr.Append(string.Format("<input type=\"checkbox\" name=\"{0}\" value=\"{1}\" {2} />{1}&nbsp;", item.Name, cksArr[i], isChecked ? "checked" : ""));
                        }
                        ckStr.Append("</span>");
                        content = ckStr.ToString();
                        break;
                }
                
                //k:data_1;v:content
                tempController.Add(string.IsNullOrEmpty(item.parse_name)?item.Name:item.parse_name, content);
                
            });
            
            // 替换占位符
            foreach(var item in controllers)
            {
                string key = string.IsNullOrEmpty(item.parse_name) ? item.Name : item.parse_name;
                content_parse = content_parse.Replace(string.Format("{0}", "{"+ key + "}"), tempController[key]);
            }

            return content_parse;

        }
    }
}
