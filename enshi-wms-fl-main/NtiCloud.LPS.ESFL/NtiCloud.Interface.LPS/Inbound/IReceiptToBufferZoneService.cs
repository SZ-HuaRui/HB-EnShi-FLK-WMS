using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Inbound;

namespace NtiCloud.Interface.WMS.Inbound
{
    [Authorization]
    public interface IReceiptToBufferZoneService
    {
        [Description(description: "收货到暂存区",
                     inputDescription: "",
                     outputDescription: "")]
        [Post("/api/wms/ReceiptToBufferZone/ToBufferZone")]
        WebJsonInfo ToBufferZone(string receiptId);
    }
}
