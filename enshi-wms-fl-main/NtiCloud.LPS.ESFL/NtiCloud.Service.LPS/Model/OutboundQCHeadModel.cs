using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class OutboundQCHeadModel
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
        /// 质检单号
        /// </summary>
        public string QcoutDocNo { get; set; }

        /// <summary>
        /// 出库单ID
        /// </summary>
        public string OutboundHid { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// 出库质检状态
        /// </summary>
        public string QcoutStatus { get; set; }

        /// <summary>
        /// 提交状态(0,未提交，1,已提交 )
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 统计数量
        /// </summary>
        public decimal? FinanceQty { get; set; }

        /// <summary>
        /// 统计单位
        /// </summary>
        public string UomFinanceid { get; set; }

        /// <summary>
        /// 物流数量
        /// </summary>
        public decimal? Qty { get; set; }

        /// <summary>
        /// 物流单位
        /// </summary>
        public string UomQtyid { get; set; }

        /// <summary>
        /// 经办人ID
        /// </summary>
        public string HandlingUserID { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public string HandlingDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

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
        /// 是否系统创建,0:自动创建，1：手动创建 
        /// </summary>
        public string IsAutoCreated { get; set; }
    }
}
