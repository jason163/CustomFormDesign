using CustomForm.DA;
using CustomForm.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomForm.UI.Controllers
{
    /// <summary>
    /// 表单服务
    /// </summary>
    public class FormController : Controller
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
            if (master.SysNo > 1)
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

                if (form.AddFileds != null && form.AddFileds.Count > 0)
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
        /// 表单模板详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var template = FormMasterDA.LoadFormMaster(id.Value);
                ViewBag.Template = template.Template;
                ViewBag.FormID = template.SysNo;
                ViewBag.FieldCount = template.FieldCount;
            }
            else
            {
                ViewBag.Template = string.Empty;
                ViewBag.FormID = 0;
                ViewBag.FieldCount = 0;
            }

            return View();
        }

        /// <summary>
        /// 表单模板预览
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="type"></param>
        /// <param name="formid"></param>
        /// <param name="parse_form"></param>
        /// <param name="design_content"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Preview(int fields,string type,int formid,string parse_form,string design_content)
        {
            ViewBag.TempHtml = design_content.Replace("-|}","").Replace("{|-", "");
            return View();
        }

        /// <summary>
        /// 动态表单详情
        /// </summary>
        /// <param name="id">真实表单编号</param>
        /// <returns></returns>
        public ActionResult Maintain(int id)
        {
            int.TryParse(Request.QueryString["tmp"], out int templateSysNo);

            var template = FormMasterDA.LoadFormMaster(templateSysNo);

            DynamicTableInfo tableInfo = new DynamicTableInfo();
            tableInfo.DBName = Constant.CustomFormDBName;
            tableInfo.TableName = template.FormCode;
            tableInfo.SysNo = id;
            DataTable dt = FormDA.LoadFormBySysNo(tableInfo);

            string html = CustomFormUtils.UnparseForm(template, dt);

            ViewBag.Template = html.Replace("-|}", "").Replace("{|-", "");
            ViewBag.FormID = tableInfo.SysNo;
            ViewBag.TemplateID = template.SysNo;
            return View();
        }

        /// <summary>
        /// 动态表单数据维护
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <returns></returns>
        public JsonResult SaveCustomForm(CustomFormDataInfo dataInfo)
        {
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataInfo.Data);
            // 
            var templateInfo = FormMasterDA.LoadFormMaster(dataInfo.TemplateID);

            DynamicTableInfo info = new DynamicTableInfo();
            info.TableName = templateInfo.FormCode;
            info.DBName = Constant.CustomFormDBName;
            foreach(var item in dic)
            {
                info.ColumnAndValues.Add(item.Key, item.Value);
            }
            info.ColumnAndValues.Add("CustomerSysNo", dataInfo.CustomerSysNo);
            info.ColumnAndValues.Add("Status", 1);            
            
            if(dataInfo.FormID > 0)
            {
                info.SysNo = dataInfo.FormID;
                info.ColumnAndValues.Add("EditUser", 1);
                info.ColumnAndValues.Add("EditUserName", "update");
                info.ColumnAndValues.Add("EditDate", DateTime.Now);
                FormDA.UpdateFormData(info);
               // FormDA.(info);
            }
            else
            {
                info.ColumnAndValues.Add("FormMasterSysNo", dataInfo.TemplateID);
                info.ColumnAndValues.Add("InUser", 1);
                info.ColumnAndValues.Add("InUserName", "add");
                info.ColumnAndValues.Add("InDate", DateTime.Now);

                FormDA.SaveFormData(info);
            }

            

            return Json(new { Code = 0, Data = 1, Msg = "保存成功" });
        }
    }
}