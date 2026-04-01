using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Inbound;

namespace NtiCloud.Interface.WMS.Inbound
{
    /// <summary>
    /// RF配盘收货（适用辅料库）
    /// </summary>
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IComposeContainerService
    {
        [Description(description: "RF配盘收货（适用辅料库）",
                     inputDescription: "",
                     outputDescription: "")]
        WebJsonInfo Compose(ComposeContainerInput.ComposeModel model);

        [Description(description: "获取所有生产牌号",
                     inputDescription: "仓库ID",
                     outputDescription: "所有生产牌号")]


        WebJsonInfo GetZone();

        [Description(description: "获取区域",
                     inputDescription: "",
                     outputDescription: "区域清单")]
        WebJsonInfo GetItem(string warehouseId);

        [Description(description: "根据牌号获取bom清单头",
                     inputDescription: "牌号ID",
                     outputDescription: "bom清单头")]
        WebJsonInfo GetBomHeaderByItem(string itemId);
        
        [Description(description: "根据牌号获取bom清单明细",
                     inputDescription: "bom清单头ID",
                     outputDescription: "bom清单明细")]
        WebJsonInfo GetBomDetailByItem(string bomHeaderId);
        
        [Description(description: "根据物料ID，从暂存区获取供应商",
                     inputDescription: "物料ID",
                     outputDescription: "供应商")]
        WebJsonInfo GetItemSupplier(string itemId);
        
        [Description(description: "根据物料ID，从暂存区获取批次号",
                     inputDescription: "物料ID",
                     outputDescription: "批次号")]
        WebJsonInfo GetItemLot(string itemId);
    }
}
