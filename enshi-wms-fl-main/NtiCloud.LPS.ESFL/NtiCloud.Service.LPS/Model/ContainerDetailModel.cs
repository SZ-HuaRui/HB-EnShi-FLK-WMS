using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class ContainerDetailModel
    {

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

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
        /// 容器ID
        /// </summary>
        public string ContainerID { get; set; }

        /// <summary>
        /// 入库单ID
        /// </summary>
        public string InboundHeadID { get; set; }

        /// <summary>
        /// 备料单ID
        /// </summary>
        public string OutboundHeadID { get; set; }

        /// <summary>
        /// 子容器编号
        /// </summary>
        public string SubContainerNo { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 物料清单ID
        /// </summary>
        public string BomID { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 货主批次
        /// </summary>
        public string OwnerLotNo { get; set; }

        /// <summary>
        /// 库存批次
        /// </summary>
        public string WmsLotNo { get; set; }

        /// <summary>
        /// 有无箱码
        /// </summary>
        public string ExistBarcode { get; set; }

        /// <summary>
        /// 是否整包
        /// </summary>
        public string IsFull { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public decimal? GivenYear { get; set; }

        /// <summary>
        /// 收货工厂
        /// </summary>
        public string SubFactory { get; set; }

        /// <summary>
        /// 收货仓库
        /// </summary>
        public string SubInventory { get; set; }

        /// <summary>
        /// 收货货位
        /// </summary>
        public string SubLocation { get; set; }

        /// <summary>
        /// 质检状态
        /// </summary>
        public string QCStatus { get; set; }

        /// <summary>
        /// 库存量
        /// </summary>
        public decimal? Qty { get; set; }

        /// <summary>
        /// 统计库存量
        /// </summary>
        public decimal? StatQty { get; set; }

        /// <summary>
        /// 待入量
        /// </summary>
        public decimal? InQty { get; set; }

        /// <summary>
        /// 待出量
        /// </summary>
        public decimal? OutQty { get; set; }

        /// <summary>
        /// 锁定量
        /// </summary>
        public decimal? LockQty { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 物流单位
        /// </summary>
        public string UomQtyID { get; set; }

        /// <summary>
        /// 统计单位
        /// </summary>
        public string UomStatID { get; set; }

        /// <summary>
        /// 转换比率
        /// </summary>
        public decimal? Ratio { get; set; }

        /// <summary>
        /// 首次入库时间
        /// </summary>
        public DateTime? FirstInDate { get; set; }

        /// <summary>
        /// 出库开始时间
        /// </summary>
        public DateTime? StartOutDate { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ProductionDate { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? ExpDate { get; set; }

        /// <summary>
        /// 出库预警日期
        /// </summary>
        public DateTime? WarningDate { get; set; }

        /// <summary>
        /// 优先出库日期
        /// </summary>
        public DateTime? FirstOutDate { get; set; }

        /// <summary>
        /// 出库截止日期
        /// </summary>
        public DateTime? OutEndDate { get; set; }

        /// <summary>
        /// 可使用日期
        /// </summary>
        public DateTime? UseByDate { get; set; }

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
