using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Inbound;
using System;

namespace NtiCloud.Interface.WMS.Inbound
{
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IGroupContainerService
    {
        [Description(description: "RF组盘收货（适用辅料库）",
                     inputDescription: "",
                     outputDescription: "")]
        WebJsonInfo Group(GroupContainerInput.GroupModel model);

        [Description(description: "获取收货单信息",
                     inputDescription: "出库ID",
                     outputDescription: "收货单信息")]
        WebJsonInfo GetReceiptNo(string warehouseId);

        [Description(description: "根据收货单获取货品信息",
                     inputDescription: "收货单ID",
                     outputDescription: "货品信息")]
        WebJsonInfo GetItemsByReceiptId(string receiptId);
        
        [Description(description: "根据货品ID获取货品信息",
                     inputDescription: "收货单ID，货品ID",
                     outputDescription: "货品信息")]
        WebJsonInfo GetItemByItemId(string receiptId,string itemId);

        //[Description(description: "根据物料标签获取物料信息",
        //             inputDescription: "收货单ID",
        //             outputDescription: "标签信息")]
        //WebJsonInfo GetReceiptDetailByItemLabel(string receiptId,string label,Func<string,string> func);

        /// <summary>
        /// 成品组盘入库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "成品组盘入库（扫件烟码）",
                     inputDescription: "",
                     outputDescription: "")]
        WebJsonInfo GroupProduct(GroupCPInput.GroupCPModel model);

        /// <summary>
        /// 获取成品牌号
        /// </summary>
        /// <returns></returns>
        [Description(description: "获取成品牌号",
                     inputDescription: "",
                     outputDescription: "")]
        WebJsonInfo GetProductList();
    }
}
