using CWCS.Interface.Request.WMS;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    public class WMSInBoundRequest : WMSRequestBase<InBoundRequestParams>,IWMSRequest<InBoundRequestParams>
    {
        public WMSInBoundRequest()
        {
            Msg_Params.Add(new InBoundRequestParams());
        }
    }
}
