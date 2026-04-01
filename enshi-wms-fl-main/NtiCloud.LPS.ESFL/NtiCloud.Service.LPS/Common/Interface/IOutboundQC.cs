using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IOutboundQC
    {
        IOutboundQC Insert(OutboundQCHeadModel model);

        IOutboundQC Insert(OutboundQCDetailModel model);
    }
}
