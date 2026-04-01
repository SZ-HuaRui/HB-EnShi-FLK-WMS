using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    /// <summary>
    /// 生产领料退料主表
    /// </summary>
    public class PickReturnHModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Tid { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgID { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string WarehouseID { get; set; }
        /// <summary>
        /// 单编号
        /// </summary>
        public string PickReturnNo { get; set; }
        /// <summary>
        /// 单类型:人工退料、人工领料
        /// </summary>
        public string PickReturnType { get; set; }
        /// <summary>
        /// 生产工单单号
        /// </summary>
        public string JobNo { get; set; }
        /// <summary>
        /// 机台ID，关联设备表
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 站台id，关联设备表
        /// </summary>
        public string DockId { get; set; }
        /// <summary>
        /// 站台名称
        /// </summary>
        public string DockName { get; set; }
        /// <summary>
        /// (牌号)产品id
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// (牌号)产品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 容器主档ID
        /// </summary>
        public string ContainerId { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        public string ShiftId { get; set; }
        /// <summary>
        /// 班组
        /// </summary>
        public string TeamId { get; set; }
        /// <summary>
        /// 退料单据中的日期
        /// </summary>
        public DateTime? PickReturnDate { get; set; }
        /// <summary>
        /// 退料路线，1、车间退暂存区单据；2、暂存区退平库
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 参考单号，来源单号
        /// </summary>
        public string RefDocNo { get; set; }
        /// <summary>
        /// 源地址ID
        /// </summary>
        public string SrcAddrId { get; set; }
        /// <summary>
        /// 源地址类型
        /// </summary>
        public string SrcAddrType { get; set; }
        /// <summary>
        /// 源地址
        /// </summary>
        public string SrcAddrText { get; set; }
        /// <summary>
        /// 目标地址ID
        /// </summary>
        public string DestAddrId { get; set; }
        /// <summary>
        /// 目标地址类型
        /// </summary>
        public string DestAddrType { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string DestAddrText { get; set; }
        /// <summary>
        /// 生产线
        /// </summary>
        public string ProductionLine { get; set; }
        /// <summary>
        /// 是否需要手持终端扫码(0：不需要；1：需要)
        /// </summary>
        public string IsRf { get; set; }
        /// <summary>
        /// 退料原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 单据状态，新建、确认、完成
        /// </summary>
        public string PickReturnStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 操作人IP 
        /// </summary>
        public string ModifyIP { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string ActStatus { get; set; }
    }
    /// <summary>
    /// 生产领料退料明细表
    /// </summary>
    public class PickReturnDModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Tid { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgID { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string WarehouseID { get; set; }
        /// <summary>
        /// 领料退料单ID
        /// </summary>
        public string PickReturnId { get; set; }  
        /// <summary>
        /// 容器明细id
        /// </summary>
        public string ContainerDtid { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string LotNo { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string Suppliy { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 数量单位
        /// </summary>
        public string UomQtyId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// 操作人IP 
        /// </summary>
        public string ModifyIP { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public string LastModifiedBy { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string ActStatus { get; set; }
    }
}
