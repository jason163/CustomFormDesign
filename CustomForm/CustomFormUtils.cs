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
    /// <summary>
    /// 自定义表单解析工具类
    /// </summary>
    public class CustomFormUtils
    {
        /// <summary>
        /// 解析HTML模板
        /// </summary>
        /// <param name="formMaster"></param>
        /// <returns></returns>
        public static string ParseForm(ParseForm parseForm)
        {
            string content_parse = parseForm.Parse;
            // 表格样式
            content_parse = content_parse.Replace("<table", "table class=\"table table - bordered\"");
            //List<ControllerInfo> controllers = JsonConvert.DeserializeObject<List<ControllerInfo>>(parseForm.Data);
            List<ControllerInfo> controllers = parseForm.Data;
            Dictionary<string, string> tempController = new Dictionary<string, string>();
            Parallel.ForEach(controllers, item => {
                //对每个控件进行处理
                // 控件HTML内容
                string content = item.Content;
                // 控件名称
                string controllerName = item.Title;
                switch (item.LeipiPlugins)
                {
                    case "text":
                    case "textarea":
                    case "select":
                    case "checkboxs":
                    case "radios":
                        return;
                    case "listctrl":
                        content = ParseListCtrl(null, item);
                        break;
                        
                }
                tempController.Add(string.IsNullOrEmpty(item.parse_name) ? item.Name : item.parse_name, content);
            });

            // 替换占位符
            foreach (var item in controllers)
            {
                string key = string.IsNullOrEmpty(item.parse_name) ? item.Name : item.parse_name;
                content_parse = content_parse.Replace(string.Format("{0}", "{" + key + "}"), tempController[key]);
            }

            return content_parse;
        }

        /// <summary>
        /// 根据控件输入的值和控件模板反向解析HTML
        /// </summary>
        /// <param name="formMaster">模板</param>
        /// <param name="formData">控件值</param>
        /// <returns>反向解析生成的HTML</returns>
        public static string UnparseForm(FormMaster formMaster,DataTable formData)
        {
            string content_parse = formMaster.ParseTemplate;
            // 表格样式
            content_parse = content_parse.Replace("<table", "table class=\"table table - bordered\"");
            List<ControllerInfo> controllers = JsonConvert.DeserializeObject<List<ControllerInfo>>(formMaster.ControllerTemplate);


            // 如果动态表中没有相应数据，则直接返回HTML模板
            if (formData == null || formData.Rows == null || formData.Rows.Count < 1)
                return formMaster.Template;

            Dictionary<string, string> formDic = new Dictionary<string, string>();
            foreach (DataColumn column in formData.Columns)
            {
                formDic.Add(column.ColumnName, formData.Rows[0][column.ColumnName].ToString());
            }

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
                    case "listctrl":
                        content = ParseListCtrl(formData, item);
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

        /// <summary>
        /// 逆向解析ListCtrl
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="controllerInfo"></param>
        private static string ParseListCtrl(DataTable dataTable, ControllerInfo controllerInfo)
        {
            //编辑数据
            //$def_value[$key] = !empty($value['value']) ? unserialize($value['value']) : '';
            string orgTitle = controllerInfo.OrgTitle.TrimEnd('`');
            string orgColType = controllerInfo.OrgColType.TrimEnd('`');
            string orgUnit = controllerInfo.OrgUnit.TrimEnd('`');
            string orgSum = controllerInfo.OrgSum.TrimEnd('`');
            string orgColValue = controllerInfo.OrgColValue.TrimEnd('`');
            string orgWidth = controllerInfo.OrgWidth.TrimEnd('`');

            List<string> lstTitle = orgTitle.Split(new char[] { '`' }).ToList();
            List<string> lstColType = orgColType.Split(new char[] { '`' }).ToList();
            List<string> lstColSum = orgSum.Split(new char[] { '`' }).ToList();
            List<string> lstColVal = orgColValue.Split(new char[] { '`' }).ToList();
            List<string> lstColUnit = orgUnit.Split(new char[] { '`' }).ToList();

            return ViewListCtrl(controllerInfo, dataTable, lstTitle, lstColType, lstColSum, lstColVal, lstColUnit);
        }

        private static string ViewListCtrl(ControllerInfo controllerInfo, DataTable dataTable,
            List<string> lstTitle, List<string> lstColType, List<string> lstColSum, List<string> lstColVal,List<string> orgUnit)
        {
            
            string strHead = string.Empty;
            string strBody = string.Empty;
            string strFooter = string.Empty;

            for(int i=0;i<lstTitle.Count;i++)
            {
                // thead
                strHead += (string.Format("<th>{0}</th>", lstTitle[i]));
                // tbody
                strBody += ("<td></td>");
                // 需要合计
                if (lstColSum.Count > 0)
                {
                    strFooter += string.Format("<td>合计：{0}</td>", orgUnit[i]);
                }
                else
                {
                    strFooter += "<td></td>";
                }
            }

            // 有编辑值时，还原table
            StringBuilder strBodyFooterTR = new StringBuilder();

            StringBuilder tempHtml = new StringBuilder();
            tempHtml.Append(string.Format("<table cellspacing=\"0\" class=\"table table-bordered table-condensed\" style=\"{0}\"", controllerInfo.Style));
            tempHtml.Append("<thead>");
            tempHtml.Append(string.Format("<tr><th colspan=\"{0}\">{1}</th></tr>",lstTitle.Count(),controllerInfo.Title));
            // 头部
            string header = string.Empty;
            for (int i = 0; i < lstTitle.Count; i++)
            {
                header += (string.Format("<th name=\"{0}\">{0}</th>", lstTitle[i]));
            }
            tempHtml.Append(string.Format("<tr>{0}</tr>", header));
            tempHtml.Append("</thead>");
            if(dataTable != null && dataTable.Rows != null)
            {
                // 按行来渲染
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    tempHtml.Append("<tr class=\"template\">");
                    // 按列来渲染
                    for (int j = 0; j < lstTitle.Count; j++)
                    {
                        tempHtml.Append(string.Format("<td><input class=\"input-medium\" " +
                            "type=\"text\" name=\"data_1[0][]\" value=\"{0}\"></td>", dataTable.Rows[i][lstTitle[j]]));
                    }
                    tempHtml.Append("</tr>");
                }
            }
            
            // table内容
            tempHtml.Append("<tbody>");          
            

            tempHtml.Append("</tbody>");

            tempHtml.Append("</table>");


            return tempHtml.ToString();

        }
    }
}
