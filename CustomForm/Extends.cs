using Nesoft.Utility.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm
{
    public static class Extends
    {
        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        /// Snagged from http://theburningmonk.com/2011/05/idictionarystring-object-to-expandoobject-extension-method/
        /// </summary>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            dynamic expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }

        /// <summary>
        /// Extension method that turns a datatable to a list of ExpandoObject
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<dynamic> ToExPando(this DataTable dt)
        {
            List<dynamic> list = new List<dynamic>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                foreach (DataColumn column in dt.Columns)
                {
                    dic.Add(column.ColumnName, dr[column.ColumnName].ToString());
                }
                dynamic temp = dic.ToExpando();
                list.Add(temp);
            }
            return list;
        }

        /// <summary>
        /// 扩展DataCommand
        /// </summary>
        /// <param name="dataCommand"></param>
        /// <returns>返回List<dynamic></returns>
        public static List<dynamic> ToExpando(this DataCommand dataCommand)
        {
            List<dynamic> rst = new List<dynamic>();
            DataTable dt = dataCommand.ExecuteDataTable();
            if(dt.Rows != null && dt.Rows.Count > 0)
            {
                return dt.ToExPando();
            }
            return rst;
        }
    }
}
