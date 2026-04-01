using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System.Collections.Generic;

namespace NtiCloud.Interface.WMS
{
    public interface IPutaway
    {
        [Description(description: "上架单货位预分配",
                     inputDescription: "putawayid为上架单ID",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/AllocateStorageBin")]
        WebJsonInfo AllocateStorageBin(PutawayModelInput putawayid);

        [Description(description: "手工调整货位预分配信息",
                    inputDescription: "新增/修改/删除的货位分配信息",
                    outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/ModifyAllocateStorageBin")]
        WebJsonInfo ModifyAllocateStorageBin(List<ModifyAllocLocationIn> modifiedList);

        [Description(description: "确认货位预分配信息",
                  inputDescription: "上架单ID",
                  outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/ConfirmAllocationLocation")]
        WebJsonInfo ConfirmAllocationLocation(PutawayModelInput putawayid);

        [Description(description: "完成上架单：将未完成的任务全部完成并增加库存",
                  inputDescription: "上架单ID",
                  outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/CompletePutaway")]
        WebJsonInfo CompletePutaway(PutawayModelInput putawayid);

        [Description(description: "关闭上架单:将未完成的任务全部完成不增加库存",
             inputDescription: "上架单ID",
             outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/ClosePutaway")]
        WebJsonInfo ClosePutaway(PutawayModelInput putawayid);

        #region 车载/RF相关服务
        [Description(description: "获取待执行的任务清单",
            inputDescription: "任务相关查询字段",
            outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/GetTaskListFordoing")]
        WebJsonInfo GetTaskListFordoing(TaskdoingIn taskdingin);

        [Description(description: "RF获取待执行的任务清单",
            inputDescription: "任务相关查询字段",
            outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/GetTaskListFordoing_RF")]
        WebJsonInfo GetTaskListFordoing_RF(TaskdoingIn taskdingin);

        [Description(description: "根据任务ID获取未完成的任务明细",
           inputDescription: "任务ID",
           outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/GetTaskDetail")]
        WebJsonInfo GetTaskDetail(TaskdoingIn taskdingin);

        [Description(description: "上架任务执行，更新任务数量、库存量等",
          inputDescription: "实际上架信息",
          outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/DoPutawayToLocation")]
        WebJsonInfo DoPutawayToLocation(PutawayToLocation putawayToLocation);


        [Description(description: "扫描上架单号，货位待执行的物料明细",
          inputDescription: "上架单号",
          outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/GetPutawayItemListForDoing")]
        WebJsonInfo GetPutawayItemListForDoing(PutawayDirectIn putawayinfo);


        [Description(description: "扫描上架单号与物料号，查询出详细的待上架信息",
          inputDescription: "上架单号和物料编码",
          outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/GetPutawayItemDetailsForDoing")]
        WebJsonInfo GetPutawayItemDetailsForDoing(PutawayDirectIn putawayinfo);


        [Description(description: "上架单直接上架提交（无上架任务直接根据上架明细上架）",
        inputDescription: "上架信息",
        outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/SubmitPutawayWithoutTask")]
        WebJsonInfo SubmitPutawayWithoutTask(PutawayDirectDoneIn putawayinfo);


        [Description(description: "获取待上架的托盘信息",
        inputDescription: "托盘单号",
        outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/GetPalletInfoForIn")]
        WebJsonInfo GetPalletInfoForIn(string containerNo);

        [Description(description: "更新托盘上架货位",
        inputDescription: "容器ID，目标货位号",
        outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/PutawayService/PutawayPalletToLocation")]
        WebJsonInfo PutawayPalletToLocation(string containerId, string toLocation);

       
        [Post("/api/wms/PutawayService/ScanBarcodeIntoInv")]
        WebJsonInfo ScanBarcodeIntoInv(string containercode, string batch);

        [Description(description: "获取霉变替换任务清单",
        inputDescription: "",
        outputDescription: "")]
        [Post("/api/wms/PutawayService/GetItemReplaceList")]
        WebJsonInfo GetItemReplaceList();

        [Description(description: "提交烟包替换",
        inputDescription: "",
        outputDescription: "")]
        [Post("/api/wms/PutawayService/SubmitReplace")]
        WebJsonInfo SubmitReplace(ItemReplaceInModel info);
        #endregion

    }
}
