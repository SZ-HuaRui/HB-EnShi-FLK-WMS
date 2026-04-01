using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class WorkTaskItemModel
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
        /// 主任务ID
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 计划数量
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
        /// 数量单位
        /// </summary>
        public string UomQtyID { get; set; }

        /// <summary>
        /// 完成统计数量
        /// </summary>
        public decimal? FinishedStatQty { get; set; }

        /// <summary>
        /// 统计数量单位
        /// </summary>
        public string UomStatID { get; set; }

        /// <summary>
        /// 容器明细ID
        /// </summary>
        public string ContainerDetailID { get; set; }

        /// <summary>
        /// 来源单据类型
        /// </summary>
        public string RefDocType { get; set; }

        /// <summary>
        /// 来源单ID
        /// </summary>
        public string RefDocID { get; set; }

        /// <summary>
        /// 来源单明细ID
        /// </summary>
        public string RefDocDetailID { get; set; }

        /// <summary>
        /// 来源单号
        /// </summary>
        public string RefDocNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 工作状态
        /// </summary>
        public string ProcessStatus { get; set; }

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
