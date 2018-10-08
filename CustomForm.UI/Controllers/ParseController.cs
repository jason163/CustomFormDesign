using CustomForm.DA;
using CustomForm.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace CustomForm.UI.Controllers
{
    /// <summary>
    /// 表单模板服务
    /// </summary>
    public class ParseController : Controller
    {
        /// <summary>
        /// 模板保存
        /// </summary>
        /// <param name="formInfo"></param>
        /// <returns></returns>
        public JsonResult Save(CustomFormInfo formInfo)
        {
            ParseForm form = JsonConvert.DeserializeObject<ParseForm>(formInfo.ParseForm);

            CustomTableInfo customTableInfo = new CustomTableInfo();
            // 表单数据库名
            customTableInfo.DBName = Constant.CustomFormDBName;

            FormMaster master = new FormMaster();
            master.SysNo = int.Parse(formInfo.Formid);
            if(master.SysNo > 1)
            {
                master = FormMasterDA.LoadFormMaster(int.Parse(formInfo.Formid));
            }
            else
            {
                master.FormCode = string.Format("dbo.{0}", formInfo.FormCode); //"dbo.customertable1";
                master.FormName = formInfo.FormName;// "testForm";
                master.FormDesc = formInfo.FormDesc;
            }
            // 表名
            customTableInfo.TableName = master.FormCode;
            master.FormType = 0;
            master.Template = form.Template;
            master.ParseTemplate = form.Parse;
            master.ControllerTemplate = JsonConvert.SerializeObject(form.Data);

            if (master.SysNo < 1)
            {
                // 加载默认列
                customTableInfo.InitDefaultColumns();

                foreach (var control in form.AddFileds)
                {
                    FieldInfo field = new FieldInfo();
                    field.Name = control.Title;
                    field.Type = "NVARCHAR(20)";
                    field.Value = control.Value;
                    field.Prority = 2;
                    customTableInfo.Columns.Add(field);
                }
                // 控件数
                master.FieldCount = form.AddFileds.Count;
                FormMasterDA.InsertFormMaster(master);

                FormDA.CreateFormTable(customTableInfo);
            }
            else
            {
                // 验证
                DataTable dt = TableValidateDA.QueryTableColumns(Constant.CustomFormDBName, customTableInfo.TableName);
                List<string> columns = new List<string>();
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        columns.Add(dr[0].ToString());
                    }
                }
                bool isExist = false;
                string sameTitles = string.Empty;
                form.AddFileds.ForEach(p =>
                {
                    if (columns.Contains(p.Title))
                    {
                        isExist = true;
                        sameTitles += p.Title + "|";
                    }
                });
                if (isExist)
                {
                    return Json(new { Code = -1, Data = new object(), Msg = "数据库中已存在相同字段:" + sameTitles });
                }

                customTableInfo.ClearDefaultColumns();
                
                master.EditDate = DateTime.Now;
                master.EditUserName = "update";
                // 更新控件数
                master.FieldCount = master.FieldCount + form.AddFileds.Count;
                FormMasterDA.UpdateFormMaster(master);

                if(form.AddFileds!=null&&form.AddFileds.Count > 0)
                {
                    foreach (var control in form.AddFileds)
                    {
                        FieldInfo field = new FieldInfo();
                        field.Name = control.Title;
                        field.Type = "NVARCHAR(20)";
                        field.Prority = 2;
                        customTableInfo.Columns.Add(field);
                    }
                    FormDA.UpdateFormTable(customTableInfo);
                }
                
            }

            return Json(new object());
        }

        /// <summary>
        /// 加载模板
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public JsonResult LoadTemplate(int id)
        {
            var template = FormMasterDA.LoadFormMaster(id);
            DynamicTableInfo tableInfo = new DynamicTableInfo();
            tableInfo.TableName = string.Format("dbo.{0}",template.FormCode);
            tableInfo.ColumnAndValues = new System.Collections.Generic.Dictionary<string, object>();
            tableInfo.ColumnAndValues.Add("FormMasterSysNo", 2);
            tableInfo.ColumnAndValues.Add("CustomerSysNo", 1);
            DataTable dt = FormDA.LoadFormData(tableInfo);

            string html = CustomFormUtils.UnparseForm(template, dt);

            return Json(new { Code = 0, Data = template.Template, Msg = string.Empty });
        }
    }
}
