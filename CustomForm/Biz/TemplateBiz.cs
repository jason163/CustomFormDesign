using CustomForm.DA;
using CustomForm.Entity;
using Nesoft.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm.Biz
{
    /// <summary>
    /// 自定义表单模板业务
    /// </summary>
    public class TemplateBiz
    {
        /// <summary>
        /// 模板维护
        /// </summary>
        /// <param name="formInfo"></param>
        public static void Maintain(CustomFormInfo formInfo)
        {
            ParseForm form = JsonConvert.DeserializeObject<ParseForm>(formInfo.ParseForm);
            //string html = CustomFormUtils.ParseForm(form);
            if (int.Parse(formInfo.Formid) > 1)
            {
                
                UpdateTemplate(formInfo, form);
            }
            else
            {
                SaveTemplate(formInfo, form);
            }
        }

        /// <summary>
        /// 保存模板和动态表结构
        /// </summary>
        /// <param name="formInfo"></param>
        /// <param name="form"></param>
        private static void SaveTemplate(CustomFormInfo formInfo, ParseForm form)
        {
            // 模板实体
            FormMaster master = new FormMaster();
            master.FormCode = string.Format("dbo.{0}", formInfo.FormCode); 
            master.FormName = formInfo.FormName;
            master.FormDesc = formInfo.FormDesc;
            BuildTemplate(form, master);
            // 控件数
            master.FieldCount = form.AddFileds.Count;

            //动态表实体
            CustomTableInfo customTableInfo = new CustomTableInfo();
            // 动态表数据库名
            customTableInfo.DBName = Constant.CustomFormDBName;
            // 动态表表名
            customTableInfo.TableName = master.FormCode;
            // 加载动态表默认列
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
            List<PropertyDBInfo> infos = DynamicBiz.GenerateByCustomTableInfo(customTableInfo);
            using (ITransaction ts = TransactionManager.Create())
            {
                FormMasterDA.InsertFormMaster(master);
                FormDA.CreateFormTable(customTableInfo);
                DynamicBiz.SaveDynamicTableInfo(infos);

                ts.Complete();
            }
            

        }

        /// <summary>
        /// 更新模板和动态表结构
        /// </summary>
        /// <param name="formInfo"></param>
        /// <param name="form"></param>
        private static void UpdateTemplate(CustomFormInfo formInfo, ParseForm form)
        {
            
            // 模板实体
            FormMaster master = new FormMaster();
            master = FormMasterDA.LoadFormMaster(int.Parse(formInfo.Formid));
            master.EditDate = DateTime.Now;
            master.EditUserName = "update";
            BuildTemplate(form, master);
            // 更新控件数
            master.FieldCount = master.FieldCount + form.AddFileds.Count;

            //动态表实体
            CustomTableInfo customTableInfo = new CustomTableInfo();
            customTableInfo.ClearDefaultColumns();
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
            }
            

            using (var ts = TransactionManager.Create())
            {
                FormMasterDA.UpdateFormMaster(master);
                if (customTableInfo.Columns.Count > 0)
                {
                    // 有新增控件才验证是否有相同字段
                    ValidateDynamicTableStruct(customTableInfo, form);
                    FormDA.UpdateFormTable(customTableInfo);
                }

                ts.Complete();
            }
                

        }

        /// <summary>
        /// 验证动态表结构
        /// </summary>
        /// <param name="customTableInfo"></param>
        /// <param name="form"></param>
        /// <returns>异常信息</returns>
        private static void ValidateDynamicTableStruct(CustomTableInfo customTableInfo,ParseForm form)
        {
            // 验证
            DataTable dt = DynamicTableInfoDA.QueryTableColumns(Constant.CustomFormDBName, customTableInfo.TableName);
            List<string> columns = new List<string>();
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    columns.Add(dr[0].ToString());
                }
            }
            List<string> sames = new List<string>();
            form.AddFileds.ForEach(p =>
            {
                if (columns.Contains(p.Title))
                {
                    sames.Add(p.Title);
                }
            });
            if (sames.Count > 0)
            {
                throw new BizException(string.Format("数据库中已经存在以下字段:\r\n{0}", string.Join(",", sames)));
            }
        }

        private static void BuildTemplate(ParseForm form, FormMaster master)
        {
            master.FormType = 0;
            master.Template = form.Template;
            master.ParseTemplate = form.Parse;
            master.ControllerTemplate = JsonConvert.SerializeObject(form.Data);
        }
    }
}
