using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    /// <summary>
    /// 请求更新目标地址
    /// </summary>
    public class WMSUpdateTargetAddressRequest : WMSRequestBase<UpdateTargetAddressParam>, IWMSRequest<UpdateTargetAddressParam>
    {
        public WMSUpdateTargetAddressRequest()
        {
            Msg_Params.Add(new UpdateTargetAddressParam());
        }
    }
}
