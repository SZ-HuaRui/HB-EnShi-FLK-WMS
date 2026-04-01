using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Inbound;

namespace NtiCloud.Interface.WMS.Inbound
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IInboundService
    {
        [Description(description: "片烟收货",
                    inputDescription: "",
                    outputDescription: "")]
        WebJsonInfo PYInbound(string inputJson);
    }
}
