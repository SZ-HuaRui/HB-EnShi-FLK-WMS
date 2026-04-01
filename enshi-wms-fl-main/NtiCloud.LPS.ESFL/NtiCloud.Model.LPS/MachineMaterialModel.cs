using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    /// <summary>
    /// 车间管理
    /// </summary>
    public class MachineMaterialInput
    {
        #region 机台要料入参
        /// <summary>
        /// 机台要料入参
        /// </summary>
        public class MachineMaterialModel
        {
            /// <summary>
            /// mes叫料号
            /// </summary>
            public string Planno { get; set; }
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
        }
        #endregion

        #region 机台要料任务强制完成入参
        /// <summary>
        /// 机台要料任务强制完成入参
        /// </summary>
        public class FinishMachineMaterialModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 机台要料任务部分完成入参
        /// <summary>
        /// 机台要料任务部分完成入参
        /// </summary>
        public class PartionFinishMachineMaterialModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 机台要料任务完成入参
        /// <summary>
        /// 机台要料任务完成入参
        /// </summary>
        public class FinishMaterialModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 机台要料任务回收入参
        /// <summary>
        /// 机台要料任务回收入参
        /// </summary>
        public class RecycleMachineMaterialModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退余料入参
        /// <summary>
        /// 退余料入参
        /// </summary>
        public class ClientReturnPalletModel
        {
            /// <summary>
            /// mes叫料号
            /// </summary>
            public string Planno { get; set; }
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 余料明细（json格式）
            /// </summary>
            public string Details { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
            /// <summary>
            /// 是否需要再次要料（1:需要；0:不需要）
            /// </summary>
            public string IsNeedCallPallet { get; set; }
            /// <summary>
            /// 是否允许被再次呼出（1:允许；0:不允许）
            /// </summary>
            public string IsCanBeCallOut { get; set; }
        }
        #endregion

        #region 退余料任务强制完成入参
        /// <summary>
        /// 退余料任务强制完成入参
        /// </summary>
        public class FinishClientReturnPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退余料任务回收入参
        /// <summary>
        /// 退余料任务回收入参
        /// </summary>
        public class RecycleClientReturnPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退余料任务部分完成入参
        /// <summary>
        /// 退余料任务部分完成入参
        /// </summary>
        public class PartionFinishClientReturnPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退余料任务完成入参
        /// <summary>
        /// 退余料任务完成入参
        /// </summary>
        public class FinishReturnPalletTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 人工领料确认入参
        /// <summary>
        /// 人工领料确认入参
        /// </summary>
        public class ConfirmPersonReceiveMaterielModel
        {
            /// <summary>
            /// 人工领料主档id
            /// </summary>
            public string PickReturn_htid { get; set; }
        }
        #endregion

        #region 人工领料完成入参
        /// <summary>
        /// 人工领料确认入参
        /// </summary>
        public class FinishPersonReceiveMaterielModel
        {
            /// <summary>
            /// 人工领料主档id
            /// </summary>
            public string PickReturn_htid { get; set; }
        }
        #endregion

        #region 人工领料任务强制完成入参
        /// <summary>
        /// 人工领料任务强制完成入参
        /// </summary>
        public class FinishPersonReceiveMaterielTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 人工领料任务回收入参
        /// <summary>
        /// 人工领料任务回收入参
        /// </summary>
        public class RecyclePersonReceiveMaterielTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 人工退料确认入参
        /// <summary>
        /// 人工退料确认入参
        /// </summary>
        public class ConfirmPersonReturnMaterielModel
        {
            /// <summary>
            /// 人工退料主档id
            /// </summary>
            public string PickReturn_htid { get; set; }
        }
        #endregion

        #region 人工退料完成入参
        /// <summary>
        /// 人工退料完成入参
        /// </summary>
        public class FinishPersonReturnMaterielModel
        {
            /// <summary>
            /// 人工退料主档id
            /// </summary>
            public string PickReturn_htid { get; set; }
        }
        #endregion

        #region 人工退料任务强制完成入参
        /// <summary>
        /// 人工退料任务强制完成入参
        /// </summary>
        public class FinishPersonReturnMaterielTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 人工退料任务回收入参
        /// <summary>
        /// 人工退料任务回收入参
        /// </summary>
        public class RecyclePersonReturnMaterielTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 换牌入参
        /// <summary>
        /// 换牌入参
        /// </summary>
        public class ChangeCardModel
        {
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 余料明细（json格式）
            /// </summary>
            public string Details { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
            /// <summary>
            /// 是否允许被再次呼出（1:允许；0:不允许）
            /// </summary>
            public string IsCanBeCallOut { get; set; }
        }
        #endregion

        #region 交班入参
        /// <summary>
        /// 交班入参
        /// </summary>
        public class ReturnMaterielToNextModel
        {
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否交接人工领料数据（1:交；0:不交）
            /// </summary>
            public string IsChangePersonReceive { get; set; }
            /// <summary>
            /// 余料明细（json格式，ReturnItems包含项：容器明细containerDetailId、物料itemId、物料名称itemName、要料时物料的数量planQty、交班数量qty、批次lotNo、供应商ownerId）
            /// </summary>
            public string Details { get; set; }
        }
        #endregion

        #region 接班入参
        /// <summary>
        /// 接班入参
        /// </summary>
        public class ReturnMaterielReturnBillModel
        {
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否交接人工领料数据（1:交；0:不交）
            /// </summary>
            public string IsChangePersonReceive { get; set; }
            /// <summary>
            /// 余料明细（json格式）
            /// </summary>
            public string Details { get; set; }
        }
        #endregion

        #region 接班前修改物料数量入参
        /// <summary>
        /// 接班前修改物料数量入参
        /// </summary>
        public class EditMaterielBillModel
        {
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 余料明细（json格式）
            /// </summary>
            public string Details { get; set; }
        }
        #endregion

        #region 退空托盘+要料入参
        /// <summary>
        /// 退空托盘+要料入参
        /// </summary>
        public class ReturnNullAndCallPalletModel
        {
            /// <summary>
            /// mes叫料号
            /// </summary>
            public string Planno { get; set; }
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
        }
        #endregion

        #region 退空托盘入参
        /// <summary>
        /// 退空托盘入参
        /// </summary>
        public class ClientReturnNullPalletModel
        {
            /// <summary>
            /// mes叫料号
            /// </summary>
            public string Planno { get; set; }
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
        }
        #endregion

        #region 退空盘任务强制完成入参
        /// <summary>
        /// 退空盘任务强制完成入参
        /// </summary>
        public class FinishClientReturnNullPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退空盘任务回收入参
        /// <summary>
        /// 退空盘任务回收入参
        /// </summary>
        public class RecycleClientReturnNullPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退空盘任务部分完成入参
        /// <summary>
        /// 退空盘任务部分完成入参
        /// </summary>
        public class PartionFinishReturnNullPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退空盘任务完成入参
        /// <summary>
        /// 退空盘任务完成入参
        /// </summary>
        public class FinishReturnNullPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退坏料入参
        /// <summary>
        /// 退坏料入参
        /// </summary>
        public class ClientReturnBadPalletModel
        {
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 坏料明细（json格式）
            /// </summary>
            public string Details { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
            ///// <summary>
            ///// 目的站台id
            ///// </summary>
            //public string TargetDockId { get; set; }
        }
        #endregion

        #region 退坏料任务强制完成入参
        /// <summary>
        /// 退坏料任务强制完成入参
        /// </summary>
        public class FinishClientReturnBadPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退坏料任务部分完成入参
        /// <summary>
        /// 退坏料任务部分完成入参
        /// </summary>
        public class PartionFinishReturnBadPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退坏料任务完成入参
        /// <summary>
        /// 退坏料任务完成入参
        /// </summary>
        public class FinishReturnBadPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退坏料任务回收入参
        /// <summary>
        /// 退坏料任务回收入参
        /// </summary>
        public class RecycleClientReturnBadPalletModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退废料入参
        /// <summary>
        /// 退废料入参
        /// </summary>
        public class ReturnWasteMaterialsModel
        {
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
        }
        #endregion

        #region 退废料任务强制完成入参
        /// <summary>
        /// 退废料任务强制完成入参
        /// </summary>
        public class FinishReturnWasteMaterialsModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退废料任务回收入参
        /// <summary>
        /// 退废料任务回收入参
        /// </summary>
        public class RecycleReturnWasteMaterialsModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退废料任务部分完成入参
        /// <summary>
        /// 退废料任务部分完成入参
        /// </summary>
        public class PartionFinishReturnWasteMaterialsModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 退废料任务完成入参
        /// <summary>
        /// 退废料任务完成入参
        /// </summary>
        public class FinishWasteMaterialsModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 根据货位获取堆垛机入参
        /// <summary>
        /// 根据货位获取堆垛机入参
        /// </summary>
        public class GetEquipmentByLocModel
        {
            /// <summary>
            /// 行号
            /// </summary>
            public string Row { get; set; }

            /// <summary>
            /// 货位Id
            /// </summary>
            public string LocationId { get; set; }
        }
        #endregion

        #region 成品转移入参
        /// <summary>
        /// 成品转移入参
        /// </summary>
        public class ProductTransferModel
        {
            /// <summary>
            /// mes叫料号
            /// </summary>
            public string Planno { get; set; }
            /// <summary>
            /// 机台
            /// </summary>
            public string Machine { get; set; }
            /// <summary>
            /// 站台
            /// </summary>
            public string Dock { get; set; }
            /// <summary>
            /// 生产工单id
            /// </summary>
            public string ProductPlanOutId { get; set; }
            /// <summary>
            /// 班次
            /// </summary>
            public string ShiftId { get; set; }
            /// <summary>
            /// 班组
            /// </summary>
            public string TeamId { get; set; }
            /// <summary>
            /// 是否产生AGV任务
            /// </summary>
            public string HasAGV { get; set; }
        }
        #endregion

        #region 成品转移任务强制完成入参
        /// <summary>
        /// 成品转移任务强制完成入参
        /// </summary>
        public class FinishProductTransferTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 成品转移任务部分完成入参
        /// <summary>
        /// 成品转移任务强制完成入参
        /// </summary>
        public class PartionFinishProductTransferTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 成品转移任务完成入参
        /// <summary>
        /// 成品转移任务完成入参
        /// </summary>
        public class FinishTransferModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 成品转移任务回收入参
        /// <summary>
        /// 成品转移任务回收入参
        /// </summary>
        public class RecycleProductTransferTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion
    }
}
