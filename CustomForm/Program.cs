using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm
{
    class Program
    {
        static void Main(string[] args)
        {
            DynamicTableInfo info = new DynamicTableInfo();
            info.TableName = "test.dbo.demo";
            info.ColumnAndValues = new Dictionary<string, object>();
            info.ColumnAndValues.Add("Name", "test");
            FormDA.SaveFormData(info);

            var data = FormDA.GetDynamicData();
            DynamicInfoManager.GenernateDynamicInstance();
            Console.ReadKey();
        }
    }
}
