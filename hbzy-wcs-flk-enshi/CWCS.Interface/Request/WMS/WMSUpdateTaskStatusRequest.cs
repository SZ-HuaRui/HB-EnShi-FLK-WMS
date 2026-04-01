using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    public class WMSUpdateTaskStatusRequest : WMSRequestBase<UpdateTaskStatusParam>, IWMSRequest<UpdateTaskStatusParam>
    {
        public WMSUpdateTaskStatusRequest()
        {
            Msg_Params.Add(new UpdateTaskStatusParam());
        }
       
    }
}
