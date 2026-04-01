using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Response.WMS
{
    public class WMSResponseBase<T>
    {
        public int errcode { get; set; }
        //结果描述
        public string errmsg { get; set; }
        //成功标志
        public bool isSucceed { get; set; } = false;
        public T json { get; set; }
    }
}
