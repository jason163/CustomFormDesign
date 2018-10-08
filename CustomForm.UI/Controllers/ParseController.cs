using CustomForm.DA;
using CustomForm.Entity;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.Mvc;

namespace CustomForm.UI.Controllers
{
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
