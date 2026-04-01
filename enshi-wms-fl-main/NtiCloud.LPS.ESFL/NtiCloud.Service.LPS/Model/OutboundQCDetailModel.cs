using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class OutboundQCDetailModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 集团公司ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgID { get; set; }

        /// <summary>
        /// 所属仓库ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public string ZoneID { get; set; }

        /// <summary>
        /// 出库质检单ID
        /// </summary>
        public string QCOutHeadID { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 库位ID
        /// </summary>
        public string LocationID { get; set; }

        /// <summary>
        /// 统计数量
        /// </summary>
        public decimal? FinanceQty { get; set; }

        /// <summary>
        /// 统计单位
        /// </summary>
        public string UomFinanceID { get; set; }

        /// <summary>
        /// 物流数量
        /// </summary>
        public decimal? PlanQty { get; set; }

        /// <summary>
        /// 物流单位
        /// </summary>
        public string UomQtyID { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 单包统计数量
        /// </summary>
        public decimal? FinanceQtyPcs { get; set; }

        /// <summary>
        /// 实际数量
        /// </summary>
        public decimal? ActualQty { get; set; }

        /// <summary>
        /// 实际统计数量
        /// </summary>
        public decimal? ActualFinanceQty { get; set; }

        /// <summary>
        /// 外观(使用字典,APPEARANCE_OUT)
        /// </summary>
        public string Appearance { get; set; }

        /// <summary>
        /// 水份
        /// </summary>
        public string Humidity { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// 霉变
        /// </summary>
        public string Mildew { get; set; }

        /// <summary>
        /// 虫情
        /// </summary>
        public string Pest { get; set; }

        /// <summary>
        /// 压油
        /// </summary>
        public string Oilpressure { get; set; }

        /// <summary>
        /// 颜色程度
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 板结
        /// </summary>
        public string Compaction { get; set; }

        /// <summary>
        /// 质检结果
        /// </summary>
        public string CheckResult { get; set; }

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

        /// <summary>
        /// 出库单明细ID
        /// </summary>
        public string OutboundDetailID { get; set; }

    }
}
