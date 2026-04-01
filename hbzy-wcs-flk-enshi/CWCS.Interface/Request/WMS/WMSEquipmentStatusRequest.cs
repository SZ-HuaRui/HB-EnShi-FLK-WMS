using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    public class WMSEquipmentStatusRequest : WMSRequestBase<EquipmentStatusParam>, IWMSRequest<EquipmentStatusParam>
    {
        public WMSEquipmentStatusRequest()
        {
            Msg_Params.Add(new EquipmentStatusParam());
        }

    }
}
