using System;
using System.Collections.Generic;
using System.Text;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;

namespace NtiCloud.Interface.WMS
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface ILocationAdjustService
    {
        /// <summary>
        /// 货位调整单，确认生成任务
        /// </summary>
        /// <param name="LocationAdjustH"></param>
        /// <returns></returns>
        //[Description(description: "新建完毕的货位调整单，检查数据，明细与主档的有码和无码调整是否一致，移入库位的容量",
        //             inputDescription: "adjust_no:调整单号",
        //             outputDescription: "errcode 为 0 表示成功，否则失败")]
        //WebJsonInfo ConfirmLocationAdjust(string adjustno);
        /// <summary>
        /// 获取货位调整单
        /// </summary>
        /// <returns></returns>
        [Description(description: "获取货位调整单",
                     inputDescription: " ",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo GetAdjustNo();
        /// <summary>
        /// 获取调整单物料明细
        /// </summary>
        /// <param name="adjustno"></param>
        /// <returns></returns>
        //[Description(description: "获取调整单物料明细",
        //             inputDescription: "adjust_no:调整单号",
        //             outputDescription: "errcode 为 0 表示成功，否则失败")]
        //WebJsonInfo GetItembyAdjustNo(string adjustno);

        [Description(description: "货位调整直接移库，通过容器号获取明细物料",
                     inputDescription: "LocationCode:货位编号，ContainerCode:容器编号",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo GetContainerItem(string Locationcode, string ContainerCode);

        [Description(description: "平库库位调整，获取明细物料",
                     inputDescription: "LocationCode:货位编号",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo GetLocationItem(string Locationcode);

        [Description(description: "货位移库，检查移入库位的合法性",
                     inputDescription: "LocationCode:移入库位编号",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo CheckToInLocation(string Locationcode);

        [Description(description: "有单据无码调整，确认保存数据",
                    inputDescription: "LocationAdjustD:调整单明细Model",
                    outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo ConfirmAdjustDocRFNoneSN(LocationAdjustModel.LocationAdjustD LocationDM);

        [Description(description: "无单据无码调整，确认保存数据",
                    inputDescription: "LocationAdjustD:调整单明细Model",
                    outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo ConfirmAdjustNDocRFNoneSN(LocationAdjustModel.LocationAdjustD LocationDM);

        #region 库位调整
        /// <summary>
        /// 库位调整
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "库位调整",
                     inputDescription: "LocationadJustHTid:单据id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/LocationadJust")]
        WebJsonInfo LocationadJust(LocationAdjustModel.LocationadJustModel model);
        #endregion

        #region 创建整行库位调整明细
        /// <summary>
        /// 创建整行库位调整明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "创建整行库位调整明细",
                     inputDescription: "LocationadJustHTid:单据id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/CreateLocationadJustDetail")]
        WebJsonInfo CreateLocationadJustDetail(LocationAdjustModel.CreateLocationadJustDetail model);
        #endregion

        #region 编辑整行库位调整明细
        /// <summary>
        /// 编辑整行库位调整明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "编辑整行库位调整明细",
                     inputDescription: "LocationadJustHTid:单据id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/EditLocationadJustHead")]
        WebJsonInfo EditLocationadJustHead(LocationAdjustModel.EditLocationadJustHead model);
        #endregion

        #region 库位调整任务完成
        /// <summary>
        /// 库位调整任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "库位调整任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/FinishLocationadJust")]
        WebJsonInfo FinishLocationadJust(LocationAdjustModel.FinishLocationadJustModel model);
        #endregion

        #region 库位调整任务部分完成
        /// <summary>
        /// 库位调整任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "库位调整任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/PartionFinishLocationadJustTask")]
        WebJsonInfo PartionFinishLocationadJustTask(LocationAdjustModel.PartionFinishLocationadJustTaskModel model);
        #endregion

        #region 库位调整任务强制完成
        /// <summary>
        /// 库位调整任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "库位调整任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/ForceFinishLocationadJustTask")]
        WebJsonInfo ForceFinishLocationadJustTask(LocationAdjustModel.ForceFinishLocationadJustTaskModel model);
        #endregion

        #region 库位调整任务回收
        /// <summary>
        /// 库位调整任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "库位调整任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/RecycleLocationadJustTask")]
        WebJsonInfo RecycleLocationadJustTask(LocationAdjustModel.RecycleLocationadJustTaskModel model);
        #endregion


        #region 自动库位调整
        /// <summary>
        /// 自动库位调整
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "自动库位调整",
                     inputDescription: "",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/AutoLocationadJust")]
        WebJsonInfo AutoLocationadJust(LocationAdjustModel.AutoLocationadJustModel model);
        #endregion

        #region 自动库位调整任务完成
        /// <summary>
        /// 自动库位调整任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "自动库位调整任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/FinishAutoLocationadJust")]
        WebJsonInfo FinishAutoLocationadJust(LocationAdjustModel.FinishAutoLocationadJustModel model);
        #endregion

        #region 自动库位调整任务部分完成
        /// <summary>
        /// 自动库位调整任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "自动库位调整任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/PartionFinishAutoLocationadJustTask")]
        WebJsonInfo PartionFinishAutoLocationadJustTask(LocationAdjustModel.PartionFinishAutoLocationadJustTaskModel model);
        #endregion

        #region 自动库位调整任务强制完成
        /// <summary>
        /// 自动库位调整任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "自动库位调整任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/ForceFinishAutoLocationadJustTask")]
        WebJsonInfo ForceFinishAutoLocationadJustTask(LocationAdjustModel.ForceFinishAutoLocationadJustTaskModel model);
        #endregion

        #region 自动库位调整任务回收
        /// <summary>
        /// 自动库位调整任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "自动库位调整任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationAdjService/RecycleAutoLocationadJustTask")]
        WebJsonInfo RecycleAutoLocationadJustTask(LocationAdjustModel.RecycleAutoLocationadJustTaskModel model);
        #endregion
    }
}
