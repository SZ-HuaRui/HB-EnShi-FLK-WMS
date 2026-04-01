using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class WaveDetailModel
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
        /// 波次主表ID
        /// </summary>
        public string WaveHeadID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OutboundHeadID { get; set; }

        /// <summary>
        /// 订单明细ID
        /// </summary>
        public string OutboundDetailID { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 计划数量，为物流单位数量
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
        /// 订单出库顺序
        /// </summary>
        public string OrderSeq { get; set; }

        /// <summary>
        /// 组顺序号
        /// </summary>
        public string GroupSeq { get; set; }

        /// <summary>
        /// 组顺序号
        /// </summary>
        public string DetailSeq { get; set; }

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
