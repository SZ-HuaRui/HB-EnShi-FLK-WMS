using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    public abstract class WMSRequestBase<T> where T:class
    {
        public string Msg_Id { get; set; } = Guid.NewGuid().ToString().Replace("-","");
        public string Msg_Mark { get; set; } = "WCS->WMS";
        public string Msg_Time { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public string Msg_User { get; set; } = "wmsjk";
        public string Msg_UserPwd { get; set; } = "Nti56.com";
        public string Msg_Method { get; set; } = "";
        public List<T> Msg_Params { get; set; } = new List<T>();
    }
}
