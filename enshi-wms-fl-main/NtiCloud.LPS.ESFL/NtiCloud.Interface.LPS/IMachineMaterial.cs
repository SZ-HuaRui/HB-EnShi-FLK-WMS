using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;

namespace NtiCloud.Interface.WMS
{
    public interface IMachineMaterialService
    {
        #region 机台要料
        /// <summary>
        /// 根据机台站台产生物料出任务，通过机台和站台获取区段、生产牌号，进而获取物料。根据物料，调用货位分配算法，得到货位，产生从该货位出物料的任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "机台要料",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/CreateOutBoundByMachineDock")]
        WebJsonInfo CreateOutBoundByMachineDock(MachineMaterialInput.MachineMaterialModel model);
        #endregion

        #region 机台要料任务强制完成
        /// <summary>
        /// 根据任务id，强制完成机台要料任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "机台要料任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishMachineMaterial")]
        WebJsonInfo FinishMachineMaterial(MachineMaterialInput.FinishMachineMaterialModel model);
        #endregion

        #region 机台要料任务部分完成
        /// <summary>
        /// 根据任务id，部分完成机台要料任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "机台要料任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/PartionFinishMachineMaterial")]
        WebJsonInfo PartionFinishMachineMaterial(MachineMaterialInput.PartionFinishMachineMaterialModel model);
        #endregion

        #region 机台要料任务完成
        /// <summary>
        /// 根据任务id，完成机台要料任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "机台要料任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishMaterial")]
        WebJsonInfo FinishMaterial(MachineMaterialInput.FinishMaterialModel model);
        #endregion

        #region 机台要料任务回收
        /// <summary>
        /// 根据任务id，机台要料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "机台要料任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecycleMachineMaterial")]
        WebJsonInfo RecycleMachineMaterial(MachineMaterialInput.RecycleMachineMaterialModel model);
        #endregion

        #region 退余料+要料
        /// <summary>
        /// 退余料+要料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退余料+要料",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、details余料明细（json格式）、hasAGV:是否产生AGV任务、IsNeedCallPallet:是否需要再次要料（1:需要；0:不需要）",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ClientReturnPallet")]
        WebJsonInfo ClientReturnPallet(MachineMaterialInput.ClientReturnPalletModel model);
        #endregion

        #region 退余料任务强制完成
        /// <summary>
        /// 退余料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退余料任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishClientReturnPallet")]
        WebJsonInfo FinishClientReturnPallet(MachineMaterialInput.FinishClientReturnPalletModel model);
        #endregion

        #region 退余料任务部分完成
        /// <summary>
        /// 退余料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退余料任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/PartionFinishClientReturnPallet")]
        WebJsonInfo PartionFinishClientReturnPallet(MachineMaterialInput.PartionFinishClientReturnPalletModel model);
        #endregion

        #region 退余料任务完成
        /// <summary>
        /// 退余料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退余料任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishReturnPalletTask")]
        WebJsonInfo FinishReturnPalletTask(MachineMaterialInput.FinishReturnPalletTaskModel model);
        #endregion

        #region 退余料任务回收
        /// <summary>
        /// 退余料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退余料任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecycleClientReturnPallet")]
        WebJsonInfo RecycleClientReturnPallet(MachineMaterialInput.RecycleClientReturnPalletModel model);
        #endregion

        #region 人工领料确认
        /// <summary>
        /// 人工领料确认
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工领料确认",
                     inputDescription: "pickReturn_htid:人工领料单据主档id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ConfirmPersonReceiveMateriel")]
        WebJsonInfo ConfirmPersonReceiveMateriel(MachineMaterialInput.ConfirmPersonReceiveMaterielModel model);
        #endregion

        #region 人工领料完成
        /// <summary>
        /// 人工领料完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工领料完成",
                     inputDescription: "pickReturn_htid:人工领料单据主档id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishPersonReceiveMateriel")]
        WebJsonInfo FinishPersonReceiveMateriel(MachineMaterialInput.FinishPersonReceiveMaterielModel model);
        #endregion

        #region 人工领料任务强制完成
        /// <summary>
        /// 人工领料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工领料任务强制完成",
                     inputDescription: "pickReturn_htid:人工领料单据主档id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishPersonReceiveMaterielTask")]
        WebJsonInfo FinishPersonReceiveMaterielTask(MachineMaterialInput.FinishPersonReceiveMaterielTaskModel model);
        #endregion

        #region 人工领料任务回收
        /// <summary>
        /// 人工领料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工领料任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecyclePersonReceiveMaterielTask")]
        WebJsonInfo RecyclePersonReceiveMaterielTask(MachineMaterialInput.RecyclePersonReceiveMaterielTaskModel model);
        #endregion

        #region 人工退料确认
        /// <summary>
        /// 人工退料确认
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工退料确认",
                     inputDescription: "pickReturn_htid:人工退料单据主档id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ConfirmPersonReturnMateriel")]
        WebJsonInfo ConfirmPersonReturnMateriel(MachineMaterialInput.ConfirmPersonReturnMaterielModel model);
        #endregion

        #region 人工退料完成
        /// <summary>
        /// 人工退料完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工退料完成",
                     inputDescription: "pickReturn_htid:人工退料单据主档id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishPersonReturnMateriel")]
        WebJsonInfo FinishPersonReturnMateriel(MachineMaterialInput.FinishPersonReturnMaterielModel model);
        #endregion

        #region 人工退料任务强制完成
        /// <summary>
        /// 人工退料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工退料任务强制完成",
                     inputDescription: "TaskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishPersonReturnMaterielTask")]
        WebJsonInfo FinishPersonReturnMaterielTask(MachineMaterialInput.FinishPersonReturnMaterielTaskModel model);
        #endregion

        #region 人工退料任务回收
        /// <summary>
        /// 人工退料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "人工退料任务回收",
                     inputDescription: "TaskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecyclePersonReturnMaterielTask")]
        WebJsonInfo RecyclePersonReturnMaterielTask(MachineMaterialInput.RecyclePersonReturnMaterielTaskModel model);
        #endregion

        #region 退空托盘+要料
        /// <summary>
        /// 退空托盘+要料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退空托盘+要料",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ReturnNullAndCallPallet")]
        WebJsonInfo ReturnNullAndCallPallet(MachineMaterialInput.ReturnNullAndCallPalletModel model);
        #endregion

        #region 退空托盘
        /// <summary>
        /// 退空托盘
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退空托盘",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ClientReturnNullPallet")]
        WebJsonInfo ClientReturnNullPallet(MachineMaterialInput.ClientReturnNullPalletModel model);
        #endregion

        #region 退空托盘任务强制完成
        /// <summary>
        /// 退空托盘任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退空托盘任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishClientReturnNullPallet")]
        WebJsonInfo FinishClientReturnNullPallet(MachineMaterialInput.FinishClientReturnNullPalletModel model);
        #endregion

        #region 退空托盘任务回收
        /// <summary>
        /// 退空托盘任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退空托盘任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecycleClientReturnNullPallet")]
        WebJsonInfo RecycleClientReturnNullPallet(MachineMaterialInput.RecycleClientReturnNullPalletModel model);
        #endregion

        #region 退空托盘任务部分完成
        /// <summary>
        /// 退空托盘任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退空托盘任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/PartionFinishReturnNullPallet")]
        WebJsonInfo PartionFinishReturnNullPallet(MachineMaterialInput.PartionFinishReturnNullPalletModel model);
        #endregion

        #region 退空托盘任务完成
        /// <summary>
        /// 退空托盘任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退空托盘任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishReturnNullPallet")]
        WebJsonInfo FinishReturnNullPallet(MachineMaterialInput.FinishReturnNullPalletModel model);
        #endregion

        #region 退坏料
        /// <summary>
        /// 退坏料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退坏料",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、details坏料明细（json格式）、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ClientReturnBadPallet")]
        WebJsonInfo ClientReturnBadPallet(MachineMaterialInput.ClientReturnBadPalletModel model);
        #endregion

        #region 退坏料任务强制完成
        /// <summary>
        /// 退坏料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退坏料任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishClientReturnBadPallet")]
        WebJsonInfo FinishClientReturnBadPallet(MachineMaterialInput.FinishClientReturnBadPalletModel model);
        #endregion

        #region 退坏料任务部分完成
        /// <summary>
        /// 退坏料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退坏料任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/PartionFinishReturnBadPallet")]
        WebJsonInfo PartionFinishReturnBadPallet(MachineMaterialInput.PartionFinishReturnBadPalletModel model);
        #endregion

        #region 退坏料任务完成
        /// <summary>
        /// 退坏料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退坏料任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishReturnBadPallet")]
        WebJsonInfo FinishReturnBadPallet(MachineMaterialInput.FinishReturnBadPalletModel model);
        #endregion

        #region 退坏料任务回收
        /// <summary>
        /// 退坏料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退坏料任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecycleClientReturnBadPallet")]
        WebJsonInfo RecycleClientReturnBadPallet(MachineMaterialInput.RecycleClientReturnBadPalletModel model);
        #endregion

        #region 退废料
        /// <summary>
        /// 退废料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退废料",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ReturnWasteMaterials")]
        WebJsonInfo ReturnWasteMaterials(MachineMaterialInput.ReturnWasteMaterialsModel model);
        #endregion

        #region 退废料任务强制完成
        /// <summary>
        /// 退废料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退废料任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishReturnWasteMaterials")]
        WebJsonInfo FinishReturnWasteMaterials(MachineMaterialInput.FinishReturnWasteMaterialsModel model);
        #endregion

        #region 退废料任务回收
        /// <summary>
        /// 退废料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退废料任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecycleReturnWasteMaterials")]
        WebJsonInfo RecycleReturnWasteMaterials(MachineMaterialInput.RecycleReturnWasteMaterialsModel model);
        #endregion

        #region 退废料任务部分完成
        /// <summary>
        /// 退废料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退废料任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/PartionFinishReturnWasteMaterials")]
        WebJsonInfo PartionFinishReturnWasteMaterials(MachineMaterialInput.PartionFinishReturnWasteMaterialsModel model);
        #endregion

        #region 退废料任务完成
        /// <summary>
        /// 退废料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "退废料任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishWasteMaterialsTask")]
        WebJsonInfo FinishWasteMaterialsTask(MachineMaterialInput.FinishWasteMaterialsModel model);
        #endregion

        #region 批量退料
        /// <summary>
        /// 批量退料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "批量退料",
                     inputDescription: "machine:机台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、details坏料明细（json格式）、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ChangeCard")]
        WebJsonInfo ChangeCard(MachineMaterialInput.ChangeCardModel model);
        #endregion

        #region 交班
        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "交班",
                     inputDescription: "machine:机台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、details交班时余料明细（json格式，ReturnItems包含项：容器明细containerDetailId、物料itemId、物料名称itemName、要料时物料的数量planQty、交班数量qty、批次lotNo、供应商ownerId）",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ReturnMaterielToNext")]
        WebJsonInfo ReturnMaterielToNext(MachineMaterialInput.ReturnMaterielToNextModel model);
        #endregion

        #region 接班
        /// <summary>
        /// 接班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "接班",
                     inputDescription: "machine:机台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、details余料明细（json格式）",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ReturnMaterielReturnBill")]
        WebJsonInfo ReturnMaterielReturnBill(MachineMaterialInput.ReturnMaterielReturnBillModel model);
        #endregion

        #region 接班前修改物料数量
        /// <summary>
        /// 接班前修改物料数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "接班前修改物料数量",
                     inputDescription: "machine:机台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、details余料明细（json格式）",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/EditMaterielBillModel")]
        WebJsonInfo EditMaterielBillModel(MachineMaterialInput.EditMaterielBillModel model);
        #endregion

        #region 根据货位获取执行任务的堆垛机
        /// <summary>
        /// 根据货位获取执行任务的堆垛机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "根据货位获取执行任务的堆垛机",
                     inputDescription: "Row:行号、LocationId:货位Id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/GetEquipmentByLoc")]
        WebJsonInfo GetEquipmentByLoc(MachineMaterialInput.GetEquipmentByLocModel model);
        #endregion

        #region 成品转移
        /// <summary>
        /// 成品转移
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "成品转移",
                     inputDescription: "machine:机台;dock:站台、productPlanOutId:工单id信息、shiftId:班次、teamId:班组、hasAGV:是否产生AGV任务",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/ProductTransfer")]
        WebJsonInfo ProductTransfer(MachineMaterialInput.ProductTransferModel model);
        #endregion

        #region 成品转移任务完成
        /// <summary>
        /// 成品转移任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "成品转移任务完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishTransfer")]
        WebJsonInfo FinishTransfer(MachineMaterialInput.FinishTransferModel model);
        #endregion

        #region 成品转移任务部分完成
        /// <summary>
        /// 成品转移任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "成品转移任务部分完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/PartionFinishProductTransferTask")]
        WebJsonInfo PartionFinishProductTransferTask(MachineMaterialInput.PartionFinishProductTransferTaskModel model);
        #endregion

        #region 成品转移任务强制完成
        /// <summary>
        /// 成品转移任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "成品转移任务强制完成",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/FinishProductTransferTask")]
        WebJsonInfo FinishProductTransferTask(MachineMaterialInput.FinishProductTransferTaskModel model);
        #endregion

        #region 成品转移任务回收
        /// <summary>
        /// 成品转移任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "成品转移任务回收",
                     inputDescription: "taskTid:任务id",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/MachineMaterialService/RecycleProductTransferTask")]
        WebJsonInfo RecycleProductTransferTask(MachineMaterialInput.RecycleProductTransferTaskModel model);
        #endregion
    }
}
