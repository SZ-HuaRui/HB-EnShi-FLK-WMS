using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    #region 地址类型
    /// <summary>
    /// 地址类型
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// 存储区
        /// </summary>
        [Description("存储区")]
        AddressType_Zone,

        /// <summary>
        /// 月台
        /// </summary>
        [Description("月台")]
        AddressType_Dock,

        /// <summary>
        /// 机台站台
        /// </summary>
        [Description("机台站台")]
        AddressType_MachineStation,

        /// <summary>
        /// 机械手
        /// </summary>
        [Description("机械手")]
        AddressType_Robot,

        /// <summary>
        /// 线上理货区
        /// </summary>
        [Description("线上理货区")]
        AddressType_OnlinePicking,

        /// <summary>
        /// 线下理货区
        /// </summary>
        [Description("线下理货区")]
        AddressType_Picking,

        /// <summary>
        /// 货位
        /// </summary>
        [Description("货位")]
        AddressType_Location,

        /// <summary>
        /// 输送机站台
        /// </summary>
        [Description("输送机站台")]
        AddressType_Station,
    }
    #endregion

    #region 容器类别
    /// <summary>
    /// 容器类别
    /// </summary>
    public enum ContainerType
    {
        /// <summary>
        /// 实际容器
        /// </summary>
        [Description("实际容器")]
        ContainerType_Actual,
        /// <summary>
        /// 虚拟容器
        /// </summary>
        [Description("虚拟容器")]
        ContainerType_Virtual,
    }
    #endregion
    public enum PlanStatus
    {
        [Description("执行中")]
        PlanStatus_Executing,
        [Description("暂停")]
        PlanStatus_Pausing,
        [Description("完成")]
        PlanStatus_Finished
    }
    #region 日志级别
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum CriticalLevel
    {
        /// <summary>
        /// 信息
        /// </summary>
        [Description("信息")]
        CriticalLevel_Info,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        CriticalLevel_Error,
    }
    #endregion

    #region 单据类型
    /// <summary>
    /// 单据类型
    /// </summary>
    public enum DocType
    {
        /// <summary>
        /// 上架单
        /// </summary>
        [Description("上架单")]
        DocType_Inbound,
        /// <summary>
        /// 空托盘组入库
        /// </summary>
        [Description("空托盘组入库")]
        DocType_EmptyContainerIn,

        /// <summary>
        /// 空托盘组出库
        /// </summary>
        [Description("空托盘组出库")]
        DocType_EmptyContainerOut,
        /// <summary>
        /// 实托盘组入库
        /// </summary>
        [Description("实托盘组入库")]
        DocType_ContainerIn,

        /// <summary>
        /// 入库生产计划
        /// </summary>
        [Description("入库生产计划")]
        DocType_ProductPlanIn,

        /// <summary>
        /// 出库生产计划
        /// </summary>
        [Description("出库生产计划")]
        DocType_ProductPlanOut,

        /// <summary>
        /// 出库订单
        /// </summary>
        [Description("出库订单")]
        DocType_Order,

        /// <summary>
        /// 收货单
        /// </summary>
        [Description("收货单")]
        DocType_Receiving,

        /// <summary>
        /// 波次
        /// </summary>
        [Description("波次")]
        DocType_Wave,

        /// <summary>
        /// 车间呼叫出库
        /// </summary>
        [Description("车间呼叫出库")]
        DocType_CallTerminal,
        /// <summary>
        /// 人工领料
        /// </summary>
        [Description("人工领料")]
        DocType_PersonReceiveMateriel,
        /// <summary>
        /// 人工退料
        /// </summary>
        [Description("人工退料")]
        DocType_PersonReturnMateriel,
        /// <summary>
        /// 机台退坏料
        /// </summary>
        [Description("机台退坏料")]
        DocType_ReturnBadPallet,
        /// <summary>
        /// 交班
        /// </summary>
        [Description("交班")]
        DocType_ReturnMaterielToNext,
        /// <summary>
        /// 接班
        /// </summary>
        [Description("接班")]
        DocType_ReturnMaterielReturnBill,
        /// <summary>
        /// 退余料
        /// </summary>
        [Description("退余料")]
        DocType_ClientReturnPallet,
        /// <summary>
        /// 堆垛机执行退余料
        /// </summary>
        [Description("堆垛机执行退余料")]
        DocType_ClientReturnPalletByDD,
        /// <summary>
        /// 退空托盘
        /// </summary>
        [Description("退空托盘")]
        DocType_ClientReturnNullPallet,

        /// <summary>
        /// 退废料
        /// </summary>
        [Description("退废料")]
        DocType_ClientReturnWaste,
        /// <summary>
        /// 质检
        /// </summary>
        [Description("质检")]
        DocType_Qc,
        /// <summary>
        /// 配盘
        /// </summary>
        DocType_ReceiptCompose,
        /// <summary>
        /// 组盘
        /// </summary>
        DocType_ReceiptGroup,
        /// <summary>
        /// 入库暂存区
        /// </summary>
        DocType_ReceiptToBufferZone,
        /// <summary>
        /// 库位调整
        /// </summary>
        DocType_Adjustment,
        /// <summary>
        /// 自动库位调整
        /// </summary>
        DocType_AutoAdjustment,
        /// <summary>
        /// 下架
        /// </summary>
        DocType_Retrieval,
        /// <summary>
        /// 盘点
        /// </summary>
        DocType_Count,
        /// <summary>
        /// 拣选余料回库
        /// </summary>
        DocType_PickSurpluBack,
        /// <summary>
        /// 拣选订单箱回库
        /// </summary>
        DocType_PickOrderBack,
        /// <summary>
        /// 成品转移
        /// </summary>
        DocType_ProductTransfer
    }
    #endregion

    #region 任务执行者类型，对应task表中executed_by
    /// <summary>
    /// 货位存储状态
    /// </summary>
    public enum TaskExecutor
    {
        /// <summary>
        /// WCS
        /// </summary>
        [Description("WCS")]
        TaskExecutor_WCS,
        /// <summary>
        /// WMS
        /// </summary>
        [Description("WMS")]
        TaskExecutor_WMS,
        /// <summary>
        /// 手持设备RF
        /// </summary>
        [Description("手持设备RF")]
        TaskExecutor_RF,
    }
    #endregion

    #region 货位存储状态
    /// <summary>
    /// 货位存储状态
    /// </summary>
    public enum LocationStatus
    {

        /// <summary>
        /// 空
        /// </summary>
        [Description("空")]
        LocationStatus_Empty,

        /// <summary>
        /// 已存储
        /// </summary>
        [Description("已存储")]
        LocationStatus_Stored,

        /// <summary>
        /// 待入库
        /// </summary>
        [Description("待入库")]
        LocationStatus_In,

        /// <summary>
        /// 待出库
        /// </summary>
        [Description("待出库")]
        LocationStatus_Out,
        /// <summary>
        /// 预分配
        /// </summary>
        [Description("预分配")]
        LocationStatus_Plan,
        /// <summary>
        /// 已满
        /// </summary>
        [Description("已满")]
        LocationStatus_Full,
        /// <summary>
        /// 部分存储
        /// </summary>
        [Description("部分存储")]
        LocationStatus_Partial
    }
    #endregion

    #region 货位类型
    /// <summary>
    /// 货位类型
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// 地面平仓
        /// </summary>
        [Description("地面平仓")]
        LocationType_FlatStock,
        /// <summary>
        /// 密集库货位
        /// </summary>
        [Description("密集库货位")]
        LocationType_Compact,
        /// <summary>
        /// 立体库货位
        /// </summary>
        [Description("立体库货位")]
        LocationType_3DRack,
        /// <summary>
        /// 输送线
        /// </summary>
        [Description("输送线")]
        LocationType_Conveyor
    }
    #endregion

    #region 货位标识
    /// <summary>
    /// 货位标识
    /// </summary>
    public enum LocationFlag
    {
        /// <summary>
        /// 可用
        /// </summary>
        [Description("可用")]
        LocationFlag_Usable,
        /// <summary>
        /// 可入不可出
        /// </summary>
        [Description("可入不可出")]
        LocationFlag_InOnly,
        /// <summary>
        /// 可出不可入
        /// </summary>
        [Description("可出不可入")]
        LocationFlag_OutOnly,
        /// <summary>
        /// 行出库锁定
        /// </summary>
        [Description("行出库锁定")]
        LocationFlag_RowLock,

        /// <summary>
        /// 保留
        /// </summary>
        [Description("保留")]
        LocationFlag_Hold,

        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        LocationFlag_Forbidden

    }
    #endregion

    #region 货位用途
    /// <summary>
    /// 货位用途
    /// </summary>
    public enum LocationUse
    {
        /// <summary>
        ///  拆装货位
        /// </summary>
        [Description(" 拆装货位")]
        LocationUse_Split,
        /// <summary>
        ///  存储货位
        /// </summary>
        [Description(" 存储货位")]
        LocationUse_Stock,
        /// <summary>
        ///  整装货位
        /// </summary>
        [Description(" 整装货位")]
        LocationUse_Package,
        /// <summary>
        ///  散装货位
        /// </summary>
        [Description(" 散装货位")]
        LocationUse_Scattered,
        /// <summary>
        ///  收货理货区
        /// </summary>
        [Description(" 收货理货区")]
        LocationUse_InboundBuffer,
        /// <summary>
        ///  补货理货区
        /// </summary>
        [Description(" 补货理货区")]
        LocationUse_ReplenishmentBuffer,
        /// <summary>
        ///  发货理货区
        /// </summary>
        [Description(" 发货理货区")]
        LocationUse_OutboundBuffer,
        /// <summary>
        ///  备货位
        /// </summary>
        [Description(" 备货位")]
        LocationUse_Backup,
        /// <summary>
        ///  平库货位
        /// </summary>
        [Description(" 平库货位")]
        LocationUse_FlatStock,
        /// <summary>
        ///  密集货位
        /// </summary>
        [Description(" 密集货位")]
        LocationUse_RackStock,
        /// <summary>
        ///  码段虚拟货位
        /// </summary>
        [Description(" 码段虚拟货位")]
        LocationUse_Barcode
    }
    #endregion
    public enum InDocStatus
    {
        /// </summary>
        [Description("新建")]
        InDocStatus_New,

        [Description(" 执行中")]
        InDocStatus_Executing,

        [Description("完成")]
        InDocStatus_Finished,

    }
    #region 日志类型
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 操作日志
        /// </summary>
        [Description("操作日志")]
        LogType_Operation,
        /// <summary>
        /// 接口日志
        /// </summary>
        [Description("接口日志")]
        LogType_Interface,
    }
    #endregion
    #region 出库单状态
    /// <summary>
    /// 出库单状态
    /// </summary>
    public enum OutDocStatus
    {
        /// <summary>
        ///  执行中
        /// </summary>
        [Description(" 执行中")]
        OutDocStatus_Executing,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        OutDocStatus_Finished,

        /// <summary>
        /// 已分配
        /// </summary>
        [Description("已分配")]
        OutDocStatus_Allocated,

        /// <summary>
        /// 已备料
        /// </summary>
        [Description("已备料")]
        OutDocStatus_Prepared,

        /// <summary>
        /// 拣选中
        /// </summary>
        [Description("拣选中")]
        OutDocStatus_Picking,

        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        OutDocStatus_New,

        /// <summary>
        /// 预分配
        /// </summary>
        [Description("预分配")]
        OutDocStatus_Preallocation,

        /// <summary>
        /// 已生成波次
        /// </summary>
        [Description("已生成波次")]
        OutDocStatus_InWave,
    }
    #endregion

    #region 上架单状态
    /// <summary>
    /// 上架单状态
    /// </summary>
    public enum PutawayStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Putaway_Create,
        /// <summary>
        /// 货位分配中
        /// </summary>
        [Description("货位分配中")]
        Putaway_Allocate,
        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        Putaway_Confirm,
        /// <summary>
        /// 上架中
        /// </summary>
        [Description("上架中")]
        Putaway_Doing,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Putaway_Done
    }
    #endregion

    #region 下架单状态
    /// <summary>
    /// 下架单状态
    /// </summary>
    public enum RetrievalState
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        RetOutboundStatus_Create,
        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        RetOutboundStatus_Execting,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        RetOutboundStatus_Over,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        RetOutboundStatus_Cancel
    }
    #endregion

    public enum CountState
    {
        /// <summary>
        /// 未执行
        /// </summary>
        [Description("未执行")]
        CountDocStatus_NotExecute,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        CountDocStatus_Executed,

        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        CountDocStatus_Executing,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        CountDocStatus_UnAudit,

        /// <summary>
        /// 已确定
        /// </summary>
        [Description("待确定")]
        CountDocStatus_Confirmed
    }

    #region 货位调整单状态
    /// <summary>
    /// 货位调整单状态
    /// </summary>
    public enum AdjustState
    {
        /// <summary>
        /// 未执行
        /// </summary>
        [Description("未执行")]
        AdjustState_NotExecute,
        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        AdjustState_Executing,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        AdjustState_Finished,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        AdjustState_Cancel
    }
    #endregion

    #region 领料退料类型
    /// <summary>
    /// 领料退料类型
    /// </summary>
    public enum PickReturnType
    {
        /// <summary>
        /// 机台要料
        /// </summary>
        [Description("机台要料")]
        PickReturnType_YL,
        /// <summary>
        /// 退余料
        /// </summary>
        [Description("退余料")]
        PickReturnType_TYL,
        /// <summary>
        /// 人工领料
        /// </summary>
        [Description("人工领料")]
        PickReturnType_RGLL,
        /// <summary>
        /// 人工退料
        /// </summary>
        [Description("人工退料")]
        PickReturnType_RGTL,
        /// <summary>
        /// 退坏料
        /// </summary>
        [Description("退坏料")]
        PickReturnType_THL
    } 
    #endregion

    #region 领料退料单据状态
    /// <summary>
    /// 领料退料单据状态
    /// </summary>
    public enum PickReturnStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        PickReturnStatus_New,
        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        PickReturnStatus_Confirmed,
        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        PickReturnStatus_Finish
    } 
    #endregion

    #region 生产工单单据状态
    /// <summary>
    /// 生产工单单据状态
    /// </summary>
    public enum PlanOutStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        [Description("未开始")]
        PlanOutStatus_NotExecute,
        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        PlanOutStatus_Executing,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        PlanOutStatus_Finished
    }
    #endregion

    #region 设备状态
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum EquipmentStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        EquipmentStatus_OK,
        /// <summary>
        /// 使用中
        /// </summary>
        [Description("使用中")]
        EquipmentStatus_Using,
        /// <summary>
        /// 故障
        /// </summary>
        [Description("故障")]
        EquipmentStatus_Error
    }
    #endregion

    #region 设备用途
    /// <summary>
    /// 设备用途
    /// </summary>
    public enum EquipmentUsage
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        EquipmentUsage_Inbound,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        EquipmentUsage_Outbound,
        /// <summary>
        /// 拣选
        /// </summary>
        [Description("拣选")]
        EquipmentUsage_Picking,
        /// <summary>
        /// 盘点
        /// </summary>
        [Description("盘点")]
        EquipmentUsage_Counting,
        /// <summary>
        /// 退空托盘
        /// </summary>
        [Description("退空托盘(退空托盘的目的站台用途)")]
        EquipmentUsage_ReturnNullPallet,
        /// <summary>
        /// 退废料
        /// </summary>
        [Description("退废料(退废料的目的站台用途)")]
        EquipmentUsage_ReturnWasteMaterials,
        /// <summary>
        /// 成品转移
        /// </summary>
        [Description("成品转移(成品转移的目的站台用途)")]
        EquipmentUsage_Prouduct,
        /// <summary>
        /// 质检
        /// </summary>
        [Description("质检")]
        EquipmentUsage_QC,
        /// <summary>
        /// 手包机
        /// </summary>
        [Description("手包机")]
        EquipmentUsage_Hand,
        /// <summary>
        /// 辅料暂存
        /// </summary>
        [Description("辅料暂存区专用")]
        EquipmentUsage_SuppleMaterial
    }
    #endregion

    /*
    #region 工作状态
    /// <summary>
    /// 工作状态
    /// </summary>
    public enum ProcessStatus
    {
        [Description("新建")]
        ProcessStatus_New,
        [Description("作业中")]
        ProcessStatus_Doing,
        [Description("已完成")]
        ProcessStatus_Done

    }
    #endregion
    */
    #region 物料替换原因
    /// <summary>
    /// 物料替换原因
    /// </summary>
    public enum ReplacementReason
    {

        /// <summary>
        /// 设备故障
        /// </summary>
        [Description("设备故障")]
        ReplacementReason_DeviceFault,

        /// <summary>
        /// 霉变
        /// </summary>
        [Description("霉变")]
        ReplacementReason_MoldyItem,
    }
    #endregion

    #region 物料替换状态
    /// <summary>
    /// 物料替换状态
    /// </summary>
    public enum ReplacementStatus
    {

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        ReplacementStatus_Finished,

        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        ReplacementStatus_Confirmed,

        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        ReplacementStatus_New,

        /// <summary>
        /// 已启动
        /// </summary>
        [Description("已启动")]
        ReplacementStatus_Start,
    }
    #endregion

    #region 路径目的
    /// <summary>
    /// 路径目的
    /// </summary>
    public enum RoutePurpose
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        RoutePurpose_Inbound,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        RoutePurpose_Outbound,

        /// <summary>
        /// 拣选
        /// </summary>
        [Description("拣选")]
        RoutePurpose_Picking,

        /// <summary>
        /// 盘点入库
        /// </summary>
        [Description("盘点入库")]
        RoutePurpose_CountIn,

        /// <summary>
        /// 盘点出库
        /// </summary>
        [Description("盘点出库")]
        RoutePurpose_CountOut,

        /// <summary>
        /// 下架
        /// </summary>
        [Description("下架")]
        RoutePurpose_Retrieval

    }
    #endregion

    #region 任务状态
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        TaskStatus_Create,
        /// <summary>
        /// 未执行
        /// </summary>
        [Description("未执行")]
        TaskStatus_NotExecute,
        /// <summary>
        /// 待执行
        /// </summary>
        [Description("待执行")]
        TaskStatus_WaitingExecute,
        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        TaskStatus_Executing,
        /// <summary>
        /// 部分完成
        /// </summary>
        [Description("部分完成")]
        TaskStatus_PartialFinished,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        TaskStatus_Finished,
        /// <summary>
        /// 替换中
        /// </summary>
        [Description("替换中")]
        TaskStatus_Replacing,
        /// <summary>
        /// 已取消
        /// </summary>
        //[Description("已取消")]
        //TaskStatus_Cancel
    }
    #endregion

    #region 任务明细工作处理状态
    /// <summary>
    /// 任务明细工作处理状态
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        ProcessStatus_New,
        /// <summary>
        /// 作业中
        /// </summary>
        [Description("作业中")]
        ProcessStatus_Doing,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        ProcessStatus_Done
    }
    #endregion

    #region 任务类型
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// 上架任务
        /// </summary>
        [Description("上架任务")]
        TaskType_InBound,
        /// <summary>
        /// 出库任务
        /// </summary>
        [Description("出库任务")]
        TaskType_OutBound,

        /// <summary>
        /// 拣选任务
        /// </summary>
        [Description("拣选任务")]
        TaskType_Picking,

        /// <summary>
        /// 移库任务
        /// </summary>
        [Description("移库任务")]
        TaskType_Move,

        /// <summary>
        /// 补货任务
        /// </summary>
        [Description("补货任务")]
        TaskType_Replenishment,

        /// <summary>
        /// 质检出库
        /// </summary>
        [Description("质检出库")]
        TaskType_CheckOut,

        /// <summary>
        /// 质检入库
        /// </summary>
        [Description("质检入库")]
        TaskType_CheckIn,
    }
    #endregion

    #region 计量单位类型
    /// <summary>
    /// 计量单位类型
    /// </summary>
    public enum UomType
    {
        /// <summary>
        /// 物流单位
        /// </summary>
        [Description("物流单位")]
        UomType_Pack,

        /// <summary>
        /// 统计单位
        /// </summary>
        [Description("统计单位")]
        UomType_Stat,
    }
    #endregion

    #region 波次创建方式
    /// <summary>
    /// 波次创建方式
    /// </summary>
    public enum WaveCreationMode
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        WaveCreationMode_Normal,

        /// <summary>
        /// 按配方单创建
        /// </summary>
        [Description("按配方单创建")]
        WaveCreationMode_ByBOM,
    }
    #endregion

    #region 波次状态
    /// <summary>
    /// 波次状态
    /// </summary>
    public enum WaveStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        WaveStatus_New,

        /// <summary>
        /// 可编辑
        /// </summary>
        [Description("可编辑")]
        WaveStatus_Editable,

        /// <summary>
        /// 计算中
        /// </summary>
        [Description("计算中")]
        WaveStatus_Computing,

        /// <summary>
        /// 已释放
        /// </summary>
        [Description("已释放")]
        WaveStatus_Released,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        WaveStatus_Finished,
    }
    #endregion

    #region 波次类型
    /// <summary>
    /// 波次类型
    /// </summary>
    public enum WaveType
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        WaveType_Normal,

        /// <summary>
        /// 从订单自动创建
        /// </summary>
        [Description("从订单自动创建")]
        WaveType_AutoFromOrder,
    }
    #endregion

    public enum SNStoreStatus
    {
        /// <summary>
        /// 未入库
        /// </summary>
        [Description("未入库")]
        StoreStatus_In,
        /// <summary>
        /// 已入库
        /// </summary>
        [Description("已入库")]
        StoreStatus_Storage,
        /// <summary>
        /// 已出库
        /// </summary>
        [Description("已出库")]
        StoreStatus_Out
    }

    public enum BOMStatus
    {
        [Description("在用")]
        BOMStatus_Using
    }

    #region 物料有效期控制类型
    /// <summary>
    /// 物料有效期控制类型
    /// </summary>
    public enum ExpiryDateControlType
    {
        /// <summary>
        /// 按入库日期
        /// </summary>
        [Description("按入库日期")]
        ExpiryDateControlType_ByIn,
        /// <summary>
        /// 按生产日期
        /// </summary>
        [Description("按生产日期")]
        ExpiryDateControlType_ByProduce,
        /// <summary>
        /// 有效期控制类型
        /// </summary>
        [Description("有效期控制类型")]
        ExpiryDateControlType_ByLosed
    }
    #endregion

    #region 质检
    /// <summary>
    /// 质检单状态
    /// </summary>
    public enum QCDocStatus
    {
        /// <summary>
        /// 未执行
        /// </summary>
        [Description("未执行")]
        QCDocStatus_NotExecute,

        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        QCDocStatus_Executing,

        /// <summary>
        /// 待确认
        /// </summary>
        [Description("待确认")]
        QCDocStatus_Confirmed,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        QCDocStatus_UnAudit,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        QCDocStatus_Executed
    }
    /// <summary>
    /// 出库单业务类型
    /// </summary>
    public enum OutServiceType
    {
        /// <summary>
        /// 生产领用出库
        /// </summary>
        [Description("生产领用出库")]
        OutServiceType_Production,

        /// <summary>
        /// 取样出库
        /// </summary>
        [Description("取样出库")]
        OutServiceType_Sampling,

        /// <summary>
        /// 联营加工出库 
        /// </summary>
        [Description("联营加工出库")]
        OutServiceType_JointProcessing,

        /// <summary>
        /// 销售出库 
        /// </summary>
        [Description("销售出库")]
        OutServiceType_Sale,

        /// <summary>
        /// 紧急补料 
        /// </summary>
        [Description("紧急补料")]
        OutServiceType_Critical,
    }

    /// <summary>
    /// 质检单状态
    /// </summary>
    public enum QCServiceType
    {
        /// <summary>
        /// 库内质检
        /// </summary>
        [Description("库内质检")]
        ServiceType_InSide,

        /// <summary>
        /// 按托盘在库质检
        /// </summary>
        [Description("按托盘在库质检")]
        ServiceType_OutSide,
    }

    /// <summary>
    /// 物料质检状态
    /// </summary>
    public enum QcStatus
    {
        /// <summary>
        /// 待检
        /// </summary>
        [Description("待检")]
        QcStatus_CheckWaiting,

        /// <summary>
        /// 质检中
        /// </summary>
        [Description("质检中")]
        QcStatus_Checking,

        /// <summary>
        /// 合格
        /// </summary>
        [Description("合格")]
        QcStatus_CheckPassed,

        /// <summary>
        /// 免检
        /// </summary>
        [Description("免检")]
        QcStatus_CheckAvoid,

        /// <summary>
        /// 不合格
        /// </summary>
        [Description("不合格")]
        QcStatus_CheckFailed
    }

    /// <summary>
    /// 质检结果
    /// </summary>
    public enum QCCheckResult
    {
        /// <summary>
        /// 合格
        /// </summary>
        [Description("合格")]
        CheckResult_Passed,

        /// <summary>
        /// 免检
        /// </summary>
        [Description("免检")]
        CheckResult_Avoid,

        /// <summary>
        /// 不合格
        /// </summary>
        [Description("不合格")]
        CheckResult_Failed,

        /// <summary>
        /// 已质检
        /// </summary>
        [Description("已质检")]
        QCDocStatus_Checked
    }

    #endregion

    #region 工艺段状态
    /// <summary>
    /// 工艺段状态
    /// </summary>
    public enum SectionStatus
    {
        /// <summary>
        /// 使用
        /// </summary>
        [Description("使用")]
        SectionStatus_Use,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        SectionStatus_Unuse
    }
    #endregion

    #region 设备类型
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum EquipmentType
    {
        /// <summary>
        /// 机台
        /// </summary>
        [Description("机台")]
        EquipmentType_Machine,
        /// <summary>
        /// 机台站台
        /// </summary>
        [Description("机台站台")]
        EquipmentType_MachineStation,
        /// <summary>
        /// RF
        /// </summary>
        [Description("RF")]
        EquipmentType_RF,
        /// <summary>
        /// 夹抱车
        /// </summary>
        [Description("夹抱车")]
        EquipmentType_ClampCar,
        /// <summary>
        /// 通道门
        /// </summary>
        [Description("通道门")]
        EquipmentType_AccessDoor,
        /// <summary>
        /// 输送机站台
        /// </summary>
        [Description("输送机站台")]
        EquipmentType_Station,
        /// <summary>
        /// AGV
        /// </summary>
        [Description("AGV")]
        EquipmentType_AGV,
        /// <summary>
        /// 堆垛机
        /// </summary>
        [Description("堆垛机")]
        EquipmentType_AS

    } 
    #endregion
}