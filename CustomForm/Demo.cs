using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm
{
    /// <summary>
    /// 动态创建实体
    /// </summary>
    public class DynamicInfoManager
    {
        public static dynamic GenernateDynamicInstance()
        {
            dynamic expando = new System.Dynamic.ExpandoObject(); //动态类型字段 可读可写

            List<string> fieldList = new List<string>() { "Name", "Age", "Sex" }; //From config or db

            dynamic dobj = new System.Dynamic.ExpandoObject();

            var dic = (IDictionary<string, object>)dobj;
            foreach (var fieldItem in fieldList)
            {
                dic[fieldItem] = "set " + fieldItem + " value"; /*实现类似js里的 动态添加属性的功能*/

            }

            Console.WriteLine($"ID:{dobj.Name}");
            return expando;
        }
    }
}
