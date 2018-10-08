using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForm
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class BizException : Exception
    {
        public BizException(string msg) : base(msg)
        {

        }
    }
}
