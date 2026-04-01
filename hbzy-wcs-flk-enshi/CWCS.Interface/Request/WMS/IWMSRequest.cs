using CWCS.Interface.Request.WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    public interface IWMSRequest<T>:IRequest
    {
        //消息Id
        string Msg_Id { get; set; }
        //"WCS->WMS"
        string Msg_Mark { get; set; }

        string Msg_Time { get; set; }
        //用户名
        string Msg_User { get; set; }
        //密码
        string Msg_UserPwd { get; set; }
        //空托盘入库
        string Msg_Method { get; set; }

        List<T> Msg_Params { get; set; }

    }
}
