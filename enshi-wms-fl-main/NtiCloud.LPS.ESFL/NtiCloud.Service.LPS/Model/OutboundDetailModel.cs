using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class OutboundDetailModel
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
        /// 出库单ID
        /// </summary>
        public string OutboundHeadID { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public decimal? GivenYear { get; set; }

        /// <summary>
        /// 模块批次号
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 计划批次号
        /// </summary>
        public string OwnerLotNo { get; set; }

        /// <summary>
        /// 批次数
        /// </summary>
        public decimal? Batches { get; set; }

        /// <summary>
        /// 有无条码
        /// </summary>
        public string ExistBarcode { get; set; }

        /// <summary>
        /// 整包标识
        /// </summary>
        public string IsFull { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public decimal? SeqNo { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 计划物流数量
        /// </summary>
        public decimal? PlanQty { get; set; }

        /// <summary>
        /// 已处理数量
        /// </summary>
        public decimal? ProcessedQty { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public decimal? FinishedQty { get; set; }

        /// <summary>
        /// 物流单位
        /// </summary>
        public string UomQtyID { get; set; }

        /// <summary>
        /// 计划统计数量
        /// </summary>
        public decimal? PlanStatQty { get; set; }

        /// <summary>
        /// 已处理统计数量
        /// </summary>
        public decimal? ProcessedStatQty { get; set; }

        /// <summary>
        /// 完成统计数量
        /// </summary>
        public decimal? FinishedStatQty { get; set; }

        /// <summary>
        /// 统计数量单位
        /// </summary>
        public string UomStatID { get; set; }

        /// <summary>
        /// 来源工厂，发货工厂
        /// </summary>
        public string SrcFactory { get; set; }

        /// <summary>
        /// 来源仓库，发货仓库
        /// </summary>
        public string SrcInventory { get; set; }

        /// <summary>
        /// 来源货位，发货货位
        /// </summary>
        public string SrcLocation { get; set; }

        /// <summary>
        /// 目的工厂，收货工厂
        /// </summary>
        public string DestFactory { get; set; }

        /// <summary>
        /// 目的仓库，收货仓库
        /// </summary>
        public string DestInventory { get; set; }

        /// <summary>
        /// 目的货位，收货货位
        /// </summary>
        public string DestLocation { get; set; }

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
