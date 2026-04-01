using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Response.WMS
{
    public interface IWMSResponse<T>:IResponse
    {
        //结果编码
        int errcode { get; set; }
        //结果描述
        string errmsg { get; set; }
        //成功标志
        bool isSucceed { get; set; }
        //接口反馈的具体内容
        T json { get; set; } 
    }
}
