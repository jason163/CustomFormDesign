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
    public class FormController : Controller
    {
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
        /// 真实表单详情
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
        /// 表单数据维护
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
            foreach(var item in dic)
            {
                info.ColumnAndValues.Add(item.Key, item.Value);
            }
            info.ColumnAndValues.Add("FormMasterSysNo", dataInfo.TemplateID);
            info.ColumnAndValues.Add("CustomerSysNo", dataInfo.CustomerSysNo);
            info.ColumnAndValues.Add("Status", 1);
            
            
            if(dataInfo.FormID > 0)
            {
                info.ColumnAndValues.Add("EditUser", 1);
                info.ColumnAndValues.Add("EditUserName", "update");
                info.ColumnAndValues.Add("EditDate", DateTime.Now);

               // FormDA.(info);
            }
            else
            {
                info.ColumnAndValues.Add("InUser", 1);
                info.ColumnAndValues.Add("InUserName", "add");
                info.ColumnAndValues.Add("InDate", DateTime.Now);

                FormDA.SaveFormData(info);
            }

            

            return Json(new { Code = 0, Data = 1, Msg = "保存成功" });
        }
    }
}