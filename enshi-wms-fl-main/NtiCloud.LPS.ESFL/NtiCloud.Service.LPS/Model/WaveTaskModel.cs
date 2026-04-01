using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class WaveTaskModel
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
        /// 仓库ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 波次ID
        /// </summary>
        public string WaveHeadID { get; set; }

        /// <summary>
        /// 波次明细ID
        /// </summary>
        public string WaveDetailID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OutboundHeadID { get; set; }

        /// <summary>
        /// 订单明细ID
        /// </summary>
        public string OutboundDetailID { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskTypeCode { get; set; }

        /// <summary>
        /// 任务执行者类型
        /// </summary>
        public string TaskExecTypeCode { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 执行设备ID
        /// </summary>
        public string EquipmentID { get; set; }

        /// <summary>
        /// 组编号
        /// </summary>
        public string GroupNo { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 优先级，正常优先级由100开始，从小到大，数字小的更为优先
        /// </summary>
        public decimal? Priority { get; set; }

        /// <summary>
        /// 容器ID
        /// </summary>
        public string ContainerID { get; set; }

        /// <summary>
        /// 容器条码
        /// </summary>
        public string ContainerBarcode { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 操作数量
        /// </summary>
        public decimal? OperationQty { get; set; }

        /// <summary>
        /// 托盘数量
        /// </summary>
        public decimal? PalletQty { get; set; }
        /// <summary>
        /// 状态编码
        /// </summary>
        public string StateCode { get; set; }

        /// <summary>
        /// 货位ID
        /// </summary>
        public string LocationID { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// BOM编码
        /// </summary>
        public string BomCode { get; set; }

        /// <summary>
        /// 来源地址ID
        /// </summary>
        public string SourceID { get; set; }

        /// <summary>
        /// 来源地址类型
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 来源地址
        /// </summary>
        public string SourceText { get; set; }

        /// <summary>
        /// 目标地址ID
        /// </summary>
        public string TargetID { get; set; }

        /// <summary>
        /// 目标地址类型
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public string TargetText { get; set; }

        /// <summary>
        /// 是否可执行
        /// </summary>
        public decimal? Executable { get; set; }

        /// <summary>
        /// 依赖任务号
        /// </summary>
        public string DependenceTaskNo { get; set; }

        /// <summary>
        /// LED内容
        /// </summary>
        public string LedText { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 修改人IP
        /// </summary>
        public string ModifyIP { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// 记录状态
        /// </summary>
        public string ActStatus { get; set; }
    }
}
