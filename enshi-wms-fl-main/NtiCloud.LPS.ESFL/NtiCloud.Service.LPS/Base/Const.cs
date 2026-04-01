using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Services.WMS.Base
{
    /* 根据规范，以下方式不再使用，规范指定的使用方式是Enum
    public struct BarcodeFlag
    {
        public const string None = "ExistBarcode_None";         // 无码
        public const string Exist = "ExistBarcode_Exist";       // 有码
    }

    public struct StockStatus
    {
        public const string Empty = "StockStatus_Empty";        // 空
        public const string Partial = "StockStatus_Partial";    // 未放满
        public const string Full = "StockStatus_Full";          // 已放满
    }

    public struct TaskType
    {
        public const string Inbound = "TaskType_InBound";     // 入库
        public const string Outbound = "TaskType_OutBound";   // 出库
        public const string QCOut = "TaskType_CheckOut";      // 质检出库
        public const string QCIn = "TaskType_CheckIn";        // 质检入库
        public const string Filling = "Task_Filling";         // 补货
        public const string Picking = "Task_Picking";         // 拣选
        public const string StockUp = "TaskType_Out4StockUp"; // 备货
        public const string Barcode = "TaskType_Barcode";     // 码段
    }

    public struct WaveStatus
    {
        public const string New = "WaveStatus_New";             // 新建
        public const string Editable = "WaveStatus_Editable";   // 可编辑
        public const string Computing = "WaveStatus_Computing"; // 计算中
        public const string Released = "WaveStatus_Released";   // 已释放
    }

    public struct ProcessStatus
    {
        public const string New = "ProcessStatus_New";          // 新建
    }

    public struct UomType
    {
        public const string Pack = "UomType_Pack";
        public const string Stat = "UomType_Stat";
    }
    */
    public struct YN
    {
        public const string Yes = "1";
        public const string No = "0";
    }
    public struct ErrMsg
    {
        public const string ParamIsNull = "参数不能为空，停止处理";
    }
    /// <summary>
    /// 容器库存表
    /// </summary>
    public struct cd
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 容器ID
        /// </summary>
        public const string ContainerID = "container_id";
        /// <summary>
        /// 入库单ID
        /// </summary>
        public const string InboundHeadID = "inbound_h_id";
        /// <summary>
        /// 备料单ID
        /// </summary>
        public const string OutboundHeadID = "outbound_h_id";
        /// <summary>
        /// 子容器编号
        /// </summary>
        public const string SubContainerNo = "sub_container_no";
        /// <summary>
        /// 供应商ID
        /// </summary>
        public const string SupplierID = "supplier_id";
        /// <summary>
        /// 物料清单ID
        /// </summary>
        public const string BomID = "bom_id";
        /// <summary>
        /// 货主ID
        /// </summary>
        public const string OwnerID = "owner_id";
        /// <summary>
        /// 物料ID
        /// </summary>
        public const string ItemID = "item_id";
        /// <summary>
        /// 物料名称
        /// </summary>
        public const string ItemName = "item_name";
        /// <summary>
        /// 批次
        /// </summary>
        public const string LotNo = "lot_no";
        /// <summary>
        /// 货主批次
        /// </summary>
        public const string OwnerLotNo = "owner_lot_no";
        /// <summary>
        /// 库存批次
        /// </summary>
        public const string WmsLotNo = "wms_lot_no";
        /// <summary>
        /// 有无箱码
        /// </summary>
        public const string ExistBarcode = "exist_barcode";
        /// <summary>
        /// 是否整包
        /// </summary>
        public const string IsFull = "is_full";
        /// <summary>
        /// 年份
        /// </summary>
        public const string GivenYear = "given_year";
        /// <summary>
        /// 收货工厂
        /// </summary>
        public const string SubFactory = "sub_factory";
        /// <summary>
        /// 收货仓库
        /// </summary>
        public const string SubInventory = "sub_inventory";
        /// <summary>
        /// 收货货位
        /// </summary>
        public const string SubLocation = "sub_location";
        /// <summary>
        /// 质检状态
        /// </summary>
        public const string QCStatus = "qc_status";
        /// <summary>
        /// 库存量
        /// </summary>
        public const string Qty = "qty";
        /// <summary>
        /// 统计库存量
        /// </summary>
        public const string StatQty = "stat_qty";
        /// <summary>
        /// 待入量
        /// </summary>
        public const string InQty = "in_qty";
        /// <summary>
        /// 待出量
        /// </summary>
        public const string OutQty = "out_qty";
        /// <summary>
        /// 锁定量
        /// </summary>
        public const string LockQty = "lock_qty";
        /// <summary>
        /// 规格
        /// </summary>
        public const string Spec = "spec";
        /// <summary>
        /// 物流单位
        /// </summary>
        public const string UomQtyID = "uom_qty_id";
        /// <summary>
        /// 统计单位
        /// </summary>
        public const string UomStatID = "uom_stat_id";
        /// <summary>
        /// 转换比率
        /// </summary>
        public const string Ratio = "ratio";
        /// <summary>
        /// 首次入库时间
        /// </summary>
        public const string FirstInDate = "first_in_date";
        /// <summary>
        /// 出库开始时间
        /// </summary>
        public const string StartOutDate = "start_out_date";
        /// <summary>
        /// 生产日期
        /// </summary>
        public const string ProductionDate = "production_date";
        /// <summary>
        /// 过期日期
        /// </summary>
        public const string ExpDate = "exp_date";
        /// <summary>
        /// 出库预警日期
        /// </summary>
        public const string WarningDate = "warning_date";
        /// <summary>
        /// 优先出库日期
        /// </summary>
        public const string FirstOutDate = "first_out_date";
        /// <summary>
        /// 出库截止日期
        /// </summary>
        public const string OutEndDate = "out_end_date";
        /// <summary>
        /// 可使用日期
        /// </summary>
        public const string UseByDate = "use_by_date";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }


    /// <summary>
    /// 容器的使用信息
    /// </summary>
    public struct ch
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 货位ID
        /// </summary>
        public const string LocationID = "location_id";
        /// <summary>
        /// 容器条码
        /// </summary>
        public const string ContainerBarcode = "container_barcode";
        /// <summary>
        /// 是否满载标记，1，满载，0，非满载
        /// </summary>
        public const string IsFull = "is_full";
        /// <summary>
        /// 容器类别，虚拟，实际
        /// </summary>
        public const string ContainerType = "container_type";
        /// <summary>
        /// 容器种类ID
        /// </summary>
        public const string ContainerCatalogID = "container_catalog_id";
        /// <summary>
        /// 物料清单ID，生产工单号
        /// </summary>
        public const string BomID = "bom_id";
        /// <summary>
        /// 原机台ID，机台退回后登记
        /// </summary>
        public const string EquipmentID = "equipment_id";
        /// <summary>
        /// 是否允许呼叫，0：不允许，1：允许
        /// </summary>
        public const string AllowCalled = "allow_called";
        /// <summary>
        /// 首次入库时间（按托盘管理的时候用）
        /// </summary>
        public const string FirstInDate = "first_in_date";
        /// <summary>
        /// 标签码（RFID信息或板标签等）
        /// </summary>
        public const string LabelCode = "label_code";
        /// <summary>
        /// 货主ID
        /// </summary>
        public const string OwnerID = "owner_id";
        /// <summary>
        /// 0，未入，1，已 入，立体库用
        /// </summary>
        public const string IsIn = "is_in";
        /// <summary>
        /// 执行入库的操作人
        /// </summary>
        public const string TakeInBy = "take_in_by";
        /// <summary>
        /// 0，未出，1，已出，立体库用
        /// </summary>
        public const string IsOut = "is_out";
        /// <summary>
        /// 执行出库的操作人
        /// </summary>
        public const string TakeOutBy = "take_out_by";
        /// <summary>
        /// 是否存放多种物料
        /// </summary>
        public const string IsMixedItem = "is_mixed_item";
        /// <summary>
        /// 是否存放多个批次的物料
        /// </summary>
        public const string IsMixedLot = "is_mixed_lot";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Memo = "memo";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 货位表
    /// </summary>
    public struct l
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团公司ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 货位号
        /// </summary>
        public const string LocationNo = "location_no";
        /// <summary>
        /// 货位编码
        /// </summary>
        public const string LocationCode = "location_code";
        /// <summary>
        /// 货位名称
        /// </summary>
        public const string LocationName = "location_name";
        /// <summary>
        /// 区域ID
        /// </summary>
        public const string ZoneID = "zone_id";
        /// <summary>
        /// 区域ID2
        /// </summary>
        public const string ZoneId2 = "zone_id2";
        /// <summary>
        /// 区域ID3
        /// </summary>
        public const string ZoneId3 = "zone_id3";
        /// <summary>
        /// 区域ID4
        /// </summary>
        public const string ZoneId4 = "zone_id4";
        /// <summary>
        /// 行
        /// </summary>
        public const string LocationRow = "location_row";
        /// <summary>
        /// 列
        /// </summary>
        public const string LocationColumn = "location_column";
        /// <summary>
        /// 层
        /// </summary>
        public const string LocationLayer = "location_layer";
        /// <summary>
        /// 深度
        /// </summary>
        public const string LocationDepth = "location_depth";
        /// <summary>
        /// 楼层
        /// </summary>
        public const string FloorNo = "floor_no";
        /// <summary>
        /// 仓间
        /// </summary>
        public const string RoomNo = "room_no";
        /// <summary>
        /// 单元号
        /// </summary>
        public const string UnitNo = "unit_no";
        /// <summary>
        /// 标签
        /// </summary>
        public const string Tags = "tags";
        /// <summary>
        /// 货位类型
        /// </summary>
        public const string LocationType = "location_type";
        /// <summary>
        /// 货位状态
        /// </summary>
        public const string LocationStatus = "location_status";
        /// <summary>
        /// 货位标识
        /// </summary>
        public const string LocationFlag = "location_flag";
        /// <summary>
        /// 工作状态
        /// </summary>
        public const string ProcessStatus = "process_status";
        /// <summary>
        /// 货位用途
        /// </summary>
        public const string Usage = "usage";
        /// <summary>
        /// 货主ID
        /// </summary>
        public const string OwnerID = "owner_id";
        /// <summary>
        /// 容器种类ID
        /// </summary>
        public const string ContainerCatalogID = "container_catalog_id";
        /// <summary>
        /// 是否允许混装物料
        /// </summary>
        public const string IsMixItem = "is_mix_item";
        /// <summary>
        /// 是否允许混装批次
        /// </summary>
        public const string IsMixLot = "is_mix_lot";
        /// <summary>
        /// 是否允许等级混放
        /// </summary>
        public const string IsMixGrade = "is_mix_grade";
        /// <summary>
        /// 长
        /// </summary>
        public const string Length = "length";
        /// <summary>
        /// 宽
        /// </summary>
        public const string Width = "width";
        /// <summary>
        /// 高
        /// </summary>
        public const string Height = "height";
        /// <summary>
        /// 尺寸单位ID
        /// </summary>
        public const string UomID = "uom_id";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string SeqNo = "seq_no";
        /// <summary>
        /// 体积限制
        /// </summary>
        public const string VolumeLimit = "volume_limit";
        /// <summary>
        /// 承重限制
        /// </summary>
        public const string WeightLimit = "weight_limit";
        /// <summary>
        /// 数量限制
        /// </summary>
        public const string QtyLimit = "qty_limit";
        /// <summary>
        /// 容器数量限制
        /// </summary>
        public const string ContainerLimit = "container_limit";
        /// <summary>
        /// 数量单位ID
        /// </summary>
        public const string UomQtyid = "uom_qtyid";
        /// <summary>
        /// 重量单位
        /// </summary>
        public const string Weightid = "weightid";
        /// <summary>
        /// 描述
        /// </summary>
        public const string Description = "description";
        /// <summary>
        /// X座标
        /// </summary>
        public const string Xcoord = "xcoord";
        /// <summary>
        /// Y座标
        /// </summary>
        public const string Ycoord = "ycoord";
        /// <summary>
        /// Z座标
        /// </summary>
        public const string Zcoord = "zcoord";
        /// <summary>
        /// X像素
        /// </summary>
        public const string Xdistance = "xdistance";
        /// <summary>
        /// Y像素
        /// </summary>
        public const string Ydistance = "ydistance";
        /// <summary>
        /// 入库日期
        /// </summary>
        public const string InboundDate = "inbound_date";
        /// <summary>
        /// 垛型
        /// </summary>
        public const string PackingType = "packing_type";
        /// <summary>
        /// 货位状态变动原因
        /// </summary>
        public const string StatusReason = "status_reason";
        /// <summary>
        /// 货位标识变动原因
        /// </summary>
        public const string FlagReason = "flag_reason";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 出库单主档
    /// </summary>
    public struct oh
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 单号
        /// </summary>
        public const string DocNo = "doc_no";
        /// <summary>
        /// 参考单号
        /// </summary>
        public const string RefDocNo = "ref_doc_no";
        /// <summary>
        /// 单据类型
        /// </summary>
        public const string OutboundType = "outbound_type";
        /// <summary>
        /// 业务类型
        /// </summary>
        public const string ServiceType = "service_type";
        /// <summary>
        /// 单据状态
        /// </summary>
        public const string OutboundStatus = "outbound_status";
        /// <summary>
        /// BOM编码
        /// </summary>
        public const string BomCode = "bom_code";
        /// <summary>
        /// 生产批次
        /// </summary>
        public const string ProdLotNo = "prod_lot_no";
        /// <summary>
        /// 货主ID
        /// </summary>
        public const string OwnerID = "owner_id";
        /// <summary>
        /// 来源系统
        /// </summary>
        public const string SourceSystem = "source_system";
        /// <summary>
        /// 波次创建方式
        /// </summary>
        public const string WaveCreationMode = "wave_creation_mode";
        /// <summary>
        /// 总数量
        /// </summary>
        public const string TotalQty = "total_qty";
        /// <summary>
        /// 总统计数量
        /// </summary>
        public const string TotalStatQty = "total_stat_qty";
        /// <summary>
        /// 物流单位
        /// </summary>
        public const string UomQtyID = "uom_qty_id";
        /// <summary>
        /// 统计单位
        /// </summary>
        public const string UomStatID = "uom_stat_id";
        /// <summary>
        /// 预计发货时间
        /// </summary>
        public const string PlanDeliveryDate = "plan_delivery_date";
        /// <summary>
        /// 结算类型
        /// </summary>
        public const string SettlementType = "settlement_type";
        /// <summary>
        /// 结算部门/单位
        /// </summary>
        public const string SettlementDept = "settlement_dept";
        /// <summary>
        /// 收货部门/单位
        /// </summary>
        public const string ReceivingDept = "receiving_dept";
        /// <summary>
        /// 发货部门/单位
        /// </summary>
        public const string DeliveryDept = "delivery_dept";
        /// <summary>
        /// 来源工厂，发货工厂
        /// </summary>
        public const string SrcFactory = "src_factory";
        /// <summary>
        /// 来源仓库，发货仓库
        /// </summary>
        public const string SrcInventory = "src_inventory";
        /// <summary>
        /// 来源货位，发货货位
        /// </summary>
        public const string SrcLocation = "src_location";
        /// <summary>
        /// 目的工厂，收货工厂
        /// </summary>
        public const string DestFactory = "dest_factory";
        /// <summary>
        /// 目的仓库，收货仓库
        /// </summary>
        public const string DestInventory = "dest_inventory";
        /// <summary>
        /// 目的货位，收货货位
        /// </summary>
        public const string DestLocation = "dest_location";
        /// <summary>
        /// 司机
        /// </summary>
        public const string Driver = "driver";
        /// <summary>
        /// 司机电话
        /// </summary>
        public const string DriverTel = "driver_tel";
        /// <summary>
        /// 车牌号
        /// </summary>
        public const string LicensePlate = "license_plate";
        /// <summary>
        /// 检查结果
        /// </summary>
        public const string QCResult = "qc_result";
        /// <summary>
        /// 运单号
        /// </summary>
        public const string TrackingNo = "tracking_no";
        /// <summary>
        /// 合同
        /// </summary>
        public const string Contract = "contract";
        /// <summary>
        /// 订单顺序
        /// </summary>
        public const string SeqNo = "seq_no";
        /// <summary>
        /// 承运商ID
        /// </summary>
        public const string ShipperID = "shipper_id";
        /// <summary>
        /// 备货目标区域
        /// </summary>
        public const string PreparationArea = "preparation_area";
        /// <summary>
        /// 拣选地址
        /// </summary>
        public const string EquipmentID = "equipment_id";
        /// <summary>
        /// 目标地址ID
        /// </summary>
        public const string DestAddrID = "dest_addr_id";
        /// <summary>
        /// 目标地址类型
        /// </summary>
        public const string DestAddrType = "dest_addr_type";
        /// <summary>
        /// 目标地址内容
        /// </summary>
        public const string DestAddrText = "dest_addr_text";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Memo = "memo";
        /// <summary>
        /// 计划批次号
        /// </summary>
        public const string PrepProdOrder = "prep_prod_order";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 出库单的细项
    /// </summary>
    public struct od
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 出库单ID
        /// </summary>
        public const string OutboundHeadID = "outbound_h_id";
        /// <summary>
        /// 物料ID
        /// </summary>
        public const string ItemID = "item_id";
        /// <summary>
        /// 物料名称
        /// </summary>
        public const string ItemName = "item_name";
        /// <summary>
        /// 规格
        /// </summary>
        public const string Spec = "spec";
        /// <summary>
        /// 年份
        /// </summary>
        public const string GivenYear = "given_year";
        /// <summary>
        /// 批次号
        /// </summary>
        public const string LotNo = "lot_no";
        /// <summary>
        /// 计划批次号
        /// </summary>
        public const string OwnerLotNo = "owner_lot_no";
        /// <summary>
        /// 批次数
        /// </summary>
        public const string Batches = "batches";
        /// <summary>
        /// 有无条码
        /// </summary>
        public const string ExistBarcode = "exist_barcode";
        /// <summary>
        /// 整包标识
        /// </summary>
        public const string IsFull = "is_full";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string SeqNo = "seq_no";
        /// <summary>
        /// 货主ID
        /// </summary>
        public const string OwnerID = "owner_id";
        /// <summary>
        /// 计划物流数量
        /// </summary>
        public const string PlanQty = "plan_qty";
        /// <summary>
        /// 已处理数量
        /// </summary>
        public const string ProcessedQty = "processed_qty";
        /// <summary>
        /// 完成数量
        /// </summary>
        public const string FinishedQty = "finished_qty";
        /// <summary>
        /// 物流单位
        /// </summary>
        public const string UomQtyID = "uom_qty_id";
        /// <summary>
        /// 计划统计数量
        /// </summary>
        public const string PlanStatQty = "plan_stat_qty";
        /// <summary>
        /// 已处理统计数量
        /// </summary>
        public const string ProcessedStatQty = "processed_stat_qty";
        /// <summary>
        /// 完成统计数量
        /// </summary>
        public const string FinishedStatQty = "finished_stat_qty";
        /// <summary>
        /// 统计数量单位
        /// </summary>
        public const string UomStatID = "uom_stat_id";
        /// <summary>
        /// 来源工厂，发货工厂
        /// </summary>
        public const string SrcFactory = "src_factory";
        /// <summary>
        /// 来源仓库，发货仓库
        /// </summary>
        public const string SrcInventory = "src_inventory";
        /// <summary>
        /// 来源货位，发货货位
        /// </summary>
        public const string SrcLocation = "src_location";
        /// <summary>
        /// 目的工厂，收货工厂
        /// </summary>
        public const string DestFactory = "dest_factory";
        /// <summary>
        /// 目的仓库，收货仓库
        /// </summary>
        public const string DestInventory = "dest_inventory";
        /// <summary>
        /// 目的货位，收货货位
        /// </summary>
        public const string DestLocation = "dest_location";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Memo = "memo";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }
    /// <summary>
    /// 波次明细表
    /// </summary>
    public struct wd
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 波次主表ID
        /// </summary>
        public const string WaveHeadID = "wave_h_id";
        /// <summary>
        /// 订单ID
        /// </summary>
        public const string OutboundHeadID = "outbound_h_id";
        /// <summary>
        /// 订单明细ID
        /// </summary>
        public const string OutboundDetailID = "outbound_d_id";
        /// <summary>
        /// 物料ID
        /// </summary>
        public const string ItemID = "item_id";
        /// <summary>
        /// 计划数量，为物流单位数量
        /// </summary>
        public const string PlanQty = "plan_qty";
        /// <summary>
        /// 已处理数量
        /// </summary>
        public const string ProcessedQty = "processed_qty";
        /// <summary>
        /// 完成数量
        /// </summary>
        public const string FinishedQty = "finished_qty";
        /// <summary>
        /// 物流单位
        /// </summary>
        public const string UomQtyID = "uom_qty_id";
        /// <summary>
        /// 计划统计数量
        /// </summary>
        public const string PlanStatQty = "plan_stat_qty";
        /// <summary>
        /// 已处理统计数量
        /// </summary>
        public const string ProcessedStatQty = "processed_stat_qty";
        /// <summary>
        /// 完成统计数量
        /// </summary>
        public const string FinishedStatQty = "finished_stat_qty";
        /// <summary>
        /// 统计数量单位
        /// </summary>
        public const string UomStatID = "uom_stat_id";
        /// <summary>
        /// 订单出库顺序
        /// </summary>
        public const string OrderSeq = "order_seq";
        /// <summary>
        /// 组顺序号
        /// </summary>
        public const string GroupSeq = "group_seq";
        /// <summary>
        /// 组顺序号
        /// </summary>
        public const string DetailSeq = "detail_seq";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }


    /// <summary>
    /// 
    /// </summary>
    public struct wh
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 波次号
        /// </summary>
        public const string WaveNo = "wave_no";
        /// <summary>
        /// 波次释放者
        /// </summary>
        public const string ReleasedBy = "released_by";
        /// <summary>
        /// 释放时间
        /// </summary>
        public const string ReleaseDate = "release_date";
        /// <summary>
        /// 波次状态
        /// </summary>
        public const string WaveStatus = "wave_status";
        /// <summary>
        /// 波次类型
        /// </summary>
        public const string WaveType = "wave_type";
        /// <summary>
        /// 订单数量
        /// </summary>
        public const string OrderQty = "order_qty";
        /// <summary>
        /// 剩余任务数
        /// </summary>
        public const string LeftTaskQty = "left_task_qty";
        /// <summary>
        /// 波次完成时间
        /// </summary>
        public const string FinishDate = "finish_date";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 任务表
    /// </summary>
    public struct t
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 区域ID
        /// </summary>
        public const string ZoneID = "zone_id";
        /// <summary>
        /// 任务编号
        /// </summary>
        public const string TaskNo = "task_no";
        /// <summary>
        /// 依赖任务号
        /// </summary>
        public const string DependentTaskNo = "dependent_task_no";
        /// <summary>
        /// 任务类型编码
        /// </summary>
        public const string TaskType = "task_type";
        /// <summary>
        /// 任务执行人
        /// </summary>
        public const string ExecutedBy = "executed_by";
        /// <summary>
        /// 执行设备ID
        /// </summary>
        public const string EquipmentID = "equipment_id";
        /// <summary>
        /// 执行设备类型
        /// </summary>
        public const string ExecutorType = "executor_type";
        /// <summary>
        /// 实际执行设备ID
        /// </summary>
        public const string ActualEquipmentID = "actual_equipment_id";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string SeqNo = "seq_no";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string Priority = "priority";
        /// <summary>
        /// 容器ID
        /// </summary>
        public const string ContainerID = "container_id";
        /// <summary>
        /// 容器条码
        /// </summary>
        public const string ContainerBarcode = "container_barcode";
        /// <summary>
        /// 来源单据类型
        /// </summary>
        public const string RefDocType = "ref_doc_type";
        /// <summary>
        /// 来源单ID
        /// </summary>
        public const string RefDocID = "ref_doc_id";
        /// <summary>
        /// 来源单明细ID
        /// </summary>
        public const string RefDocDetailID = "ref_doc_d_id";
        /// <summary>
        /// 来源单号
        /// </summary>
        public const string RefDocNo = "ref_doc_no";
        /// <summary>
        /// 来源地址ID
        /// </summary>
        public const string SrcAddrID = "src_addr_id";
        /// <summary>
        /// 来源地址类型
        /// </summary>
        public const string SrcAddrType = "src_addr_type";
        /// <summary>
        /// 来源地址
        /// </summary>
        public const string SrcAddrText = "src_addr_text";
        /// <summary>
        /// 目标地址ID
        /// </summary>
        public const string DestAddrID = "dest_addr_id";
        /// <summary>
        /// 目标地址类型
        /// </summary>
        public const string DestAddrType = "dest_addr_type";
        /// <summary>
        /// 目标地址
        /// </summary>
        public const string DestAddrText = "dest_addr_text";
        /// <summary>
        /// 实际目标地址ID
        /// </summary>
        public const string ActualDestAddrID = "actual_dest_addr_id";
        /// <summary>
        /// 实际目标地址
        /// </summary>
        public const string ActualDestAddrText = "actual_dest_addr_text";
        /// <summary>
        /// 任务状态
        /// </summary>
        public const string TaskStatus = "task_status";
        /// <summary>
        /// 开始时间
        /// </summary>
        public const string StartDate = "start_date";
        /// <summary>
        /// 结束时间
        /// </summary>
        public const string EndDate = "end_date";
        /// <summary>
        /// 任务是否可执行
        /// </summary>
        public const string IsExecutable = "is_executable";
        /// <summary>
        /// 优先级修改时间
        /// </summary>
        public const string PriorityChangeTime = "priority_change_time";
        /// <summary>
        /// LED显示内容
        /// </summary>
        public const string LedText = "led_text";
        /// <summary>
        /// 是否存在LGV
        /// </summary>
        public const string ExistLgv = "exist_lgv";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 任务物料表
    /// </summary>
    public struct ti
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 主任务ID
        /// </summary>
        public const string TaskID = "task_id";
        /// <summary>
        /// 物料ID
        /// </summary>
        public const string ItemID = "item_id";
        /// <summary>
        /// 物料名称
        /// </summary>
        public const string ItemName = "item_name";
        /// <summary>
        /// 计划数量
        /// </summary>
        public const string PlanQty = "plan_qty";
        /// <summary>
        /// 已处理数量
        /// </summary>
        public const string ProcessedQty = "processed_qty";
        /// <summary>
        /// 完成数量
        /// </summary>
        public const string FinishedQty = "finished_qty";
        /// <summary>
        /// 数量单位
        /// </summary>
        public const string UomQtyID = "uom_qty_id";
        /// <summary>
        /// 完成统计数量
        /// </summary>
        public const string FinishedStatQty = "finished_stat_qty";
        /// <summary>
        /// 统计数量单位
        /// </summary>
        public const string UomStatID = "uom_stat_id";
        /// <summary>
        /// 容器明细ID
        /// </summary>
        public const string ContainerDetailID = "container_d_id";
        /// <summary>
        /// 来源单据类型
        /// </summary>
        public const string RefDocType = "ref_doc_type";
        /// <summary>
        /// 来源单ID
        /// </summary>
        public const string RefDocID = "ref_doc_id";
        /// <summary>
        /// 来源单明细ID
        /// </summary>
        public const string RefDocDetailID = "ref_doc_d_id";
        /// <summary>
        /// 来源单号
        /// </summary>
        public const string RefDocNo = "ref_doc_no";
        /// <summary>
        /// 开始时间
        /// </summary>
        public const string StartDate = "start_date";
        /// <summary>
        /// 结束时间
        /// </summary>
        public const string EndDate = "end_date";
        /// <summary>
        /// 工作状态
        /// </summary>
        public const string ProcessStatus = "process_status";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 路径表
    /// </summary>
    public struct r
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团公司ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 源地址类型
        /// </summary>
        public const string SrcAddrType = "src_addr_type";
        /// <summary>
        /// 源地址ID
        /// </summary>
        public const string SrcAddrID = "src_addr_id";
        /// <summary>
        /// 源地址
        /// </summary>
        public const string SrcAddrText = "src_addr_text";
        /// <summary>
        /// 目标地址类型
        /// </summary>
        public const string DestAddrType = "dest_addr_type";
        /// <summary>
        /// 目标地址ID
        /// </summary>
        public const string DestAddrID = "dest_addr_id";
        /// <summary>
        /// 目标地址
        /// </summary>
        public const string DestAddrText = "dest_addr_text";
        /// <summary>
        /// 目的
        /// </summary>
        public const string Purpose = "purpose";
        /// <summary>
        /// 生成的任务类型
        /// </summary>
        public const string TaskType = "task_type";
        /// <summary>
        /// 生成的任务执行者类型
        /// </summary>
        public const string ExecutorType = "executor_type";
        /// <summary>
        /// 是否默认
        /// </summary>
        public const string IsDefault = "is_default";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string SeqNo = "seq_no";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Memo = "memo";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 物料清单主表
    /// </summary>
    public struct bh
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 配方/盘代码
        /// </summary>
        public const string BomCode = "bom_code";
        /// <summary>
        /// 配方/盘单名称
        /// </summary>
        public const string BomName = "bom_name";
        /// <summary>
        /// 成品ID
        /// </summary>
        public const string ItemID = "item_id";
        /// <summary>
        /// 成品名称
        /// </summary>
        public const string ItemName = "item_name";
        /// <summary>
        /// 机台型号
        /// </summary>
        public const string EquipmentTypeCode = "equipment_type_code";
        /// <summary>
        /// BOM类型
        /// </summary>
        public const string BomType = "bom_type";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string Priority = "priority";
        /// <summary>
        /// 所属配方单ID
        /// </summary>
        public const string BomID = "bom_id";
        /// <summary>
        /// 是否可呼叫
        /// </summary>
        public const string CanBECalled = "can_be_called";
        /// <summary>
        /// BOM状态
        /// </summary>
        public const string BomStatus = "bom_status";
        /// <summary>
        /// 工艺段
        /// </summary>
        public const string ProdSection = "prod_section";
        /// <summary>
        /// 配方单数量
        /// </summary>
        public const string TotalQty = "total_qty";
        /// <summary>
        /// 版本
        /// </summary>
        public const string BomVersion = "bom_version";
        /// <summary>
        /// 描述
        /// </summary>
        public const string BomDescription = "bom_description";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }

    /// <summary>
    /// 物料清单明细表
    /// </summary>
    public struct bd
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// BOMID
        /// </summary>
        public const string BomHeadID = "bom_h_id";
        /// <summary>
        /// 被替代物料明细ID
        /// </summary>
        public const string BomDetailID = "bom_d_id";
        /// <summary>
        /// 加工顺序
        /// </summary>
        public const string SeqNo = "seq_no";
        /// <summary>
        /// 替代顺序层级
        /// </summary>
        public const string LevelNo = "level_no";
        /// <summary>
        /// 物料ID
        /// </summary>
        public const string ItemID = "item_id";
        /// <summary>
        /// 物料编码
        /// </summary>
        public const string ItemCode = "item_code";
        /// <summary>
        /// 物料名称
        /// </summary>
        public const string ItemName = "item_name";
        /// <summary>
        /// 数量
        /// </summary>
        public const string Qty = "qty";
        /// <summary>
        /// 数量单位
        /// </summary>
        public const string UomQtyID = "uom_qty_id";
        /// <summary>
        /// 替换百分比
        /// </summary>
        public const string QtyPercent = "qty_percent";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Memo = "memo";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }
    /// <summary>
    /// 物料表
    /// </summary>
    public struct i
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public const string ID = "t_id";
        /// <summary>
        /// 集团公司ID
        /// </summary>
        public const string ClientID = "client_id";
        /// <summary>
        /// 组织ID
        /// </summary>
        public const string OrgID = "org_id";
        /// <summary>
        /// 仓库ID
        /// </summary>
        public const string WarehouseID = "warehouse_id";
        /// <summary>
        /// 物料编码，唯一编码
        /// </summary>
        public const string ItemCode = "item_code";
        /// <summary>
        /// 物料编码2
        /// </summary>
        public const string ItemCode2 = "item_code2";
        /// <summary>
        /// 物料编码3
        /// </summary>
        public const string ItemCode3 = "item_code3";
        /// <summary>
        /// 物料编码4
        /// </summary>
        public const string ItemCode4 = "item_code4";
        /// <summary>
        /// 条码
        /// </summary>
        public const string Barcode = "barcode";
        /// <summary>
        /// 物料名称
        /// </summary>
        public const string ItemName = "item_name";
        /// <summary>
        /// 物料别名
        /// </summary>
        public const string ItemAlias = "item_alias";
        /// <summary>
        /// 物料描述
        /// </summary>
        public const string ItemDescription = "item_description";
        /// <summary>
        /// 入库时是否需要质检
        /// </summary>
        public const string NeedQC = "need_qc";
        /// <summary>
        /// 物料初始入库检验状态
        /// </summary>
        public const string QCStatus = "qc_status";
        /// <summary>
        /// 物料启用，1启用，0禁用
        /// </summary>
        public const string IsEnabled = "is_enabled";
        /// <summary>
        /// 小数位数，财务单位的精度
        /// </summary>
        public const string Scale = "scale";
        /// <summary>
        /// 进位值
        /// </summary>
        public const string RoundValue = "round_value";
        /// <summary>
        /// 默认容器载量
        /// </summary>
        public const string MaxContainerQty = "max_container_qty";
        /// <summary>
        /// 码盘方式（机械手）
        /// </summary>
        public const string PalletStackingMode = "pallet_stacking_mode";
        /// <summary>
        /// 种类ID
        /// </summary>
        public const string CatalogID = "catalog_id";
        /// <summary>
        /// ABC分类
        /// </summary>
        public const string Abc = "abc";
        /// <summary>
        /// 批次编码规则ID，用于生成仓库用批次属性
        /// </summary>
        public const string LotRuleID = "lot_rule_id";
        /// <summary>
        /// 产地
        /// </summary>
        public const string Origin = "origin";
        /// <summary>
        /// 等级
        /// </summary>
        public const string ItemGrade = "item_grade";
        /// <summary>
        /// 年份
        /// </summary>
        public const string GivenYear = "given_year";
        /// <summary>
        /// 长
        /// </summary>
        public const string ItemLength = "item_length";
        /// <summary>
        /// 宽
        /// </summary>
        public const string ItemWidth = "item_width";
        /// <summary>
        /// 高
        /// </summary>
        public const string ItemHeight = "item_height";
        /// <summary>
        /// 尺寸单位ID
        /// </summary>
        public const string SizeUomID = "size_uom_id";
        /// <summary>
        /// 净重
        /// </summary>
        public const string NetWeight = "net_weight";
        /// <summary>
        /// 毛重
        /// </summary>
        public const string GrossWeight = "gross_weight";
        /// <summary>
        /// 皮重
        /// </summary>
        public const string Tare = "tare";
        /// <summary>
        /// 重量计量单位ID
        /// </summary>
        public const string WeightUomID = "weight_uom_id";
        /// <summary>
        /// 默认库存数量单位
        /// </summary>
        public const string DefaultPackUomID = "default_pack_uom_id";
        /// <summary>
        /// 默认库存统计单位
        /// </summary>
        public const string DefaultStatUomID = "default_stat_uom_id";
        /// <summary>
        /// 默认物流数量到默认统计数量的转换率，与规格联用计算
        /// </summary>
        public const string Rate = "rate";
        /// <summary>
        /// 图片路径
        /// </summary>
        public const string PicPath = "pic_path";
        /// <summary>
        /// 默认规格
        /// </summary>
        public const string Spec = "spec";
        /// <summary>
        /// 标签，用英文逗号分开
        /// </summary>
        public const string Tags = "tags";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Memo = "memo";
        /// <summary>
        /// 安全库存，0表示无安全库存。
        /// </summary>
        public const string SafetyStockQty = "safety_stock_qty";
        /// <summary>
        /// 是否管理物料标签
        /// </summary>
        public const string IsControlBarcode = "is_control_barcode";
        /// <summary>
        /// 最少在库时间，单位分钟
        /// </summary>
        public const string AgingTime = "aging_time";
        /// <summary>
        /// 最长在库时间，单位分钟
        /// </summary>
        public const string MaxAgingTime = "max_aging_time";
        /// <summary>
        /// 是否控制有效期
        /// </summary>
        public const string IsControlExpiryDate = "is_control_expiry_date";
        /// <summary>
        /// 有效期控制类型
        /// </summary>
        public const string ExpiryDateControlType = "expiry_date_control_type";
        /// <summary>
        /// 有效期天数
        /// </summary>
        public const string ExpiryDays = "expiry_days";
        /// <summary>
        /// 最大有效期天数。
        /// </summary>
        public const string MaxExpiryDays = "max_expiry_days";
        /// <summary>
        /// 失效期预警天数
        /// </summary>
        public const string AlertDays = "alert_days";
        /// <summary>
        /// 入库有效期
        /// </summary>
        public const string StockInExpiryMinutes = "stock_in_expiry_minutes";
        /// <summary>
        /// 优先出库时间
        /// </summary>
        public const string OutFirstDays = "out_first_days";
        /// <summary>
        /// 出库有效期天数
        /// </summary>
        public const string StockOutExpiryDays = "stock_out_expiry_days";
        /// <summary>
        /// 货主ID
        /// </summary>
        public const string OwnerID = "owner_id";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreateDate = "create_date";
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public const string CreatedBy = "created_by";
        /// <summary>
        /// 操作人IP
        /// </summary>
        public const string ModifyIP = "modify_ip";
        /// <summary>
        /// 编辑时间
        /// </summary>
        public const string ModifyDate = "modify_date";
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public const string LastModifiedBy = "last_modified_by";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ActStatus = "act_status";
    }


}
