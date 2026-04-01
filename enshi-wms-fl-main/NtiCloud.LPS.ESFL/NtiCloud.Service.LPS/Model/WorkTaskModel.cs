using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class WorkTaskModel
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
        /// 区域ID
        /// </summary>
        public string ZoneID { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 依赖任务号
        /// </summary>
        public string DependentTaskNo { get; set; }

        /// <summary>
        /// 任务类型编码
        /// </summary>
        public string TaskType { get; set; }

        /// <summary>
        /// 任务执行人
        /// </summary>
        public string ExecutedBy { get; set; }

        /// <summary>
        /// 执行设备ID
        /// </summary>
        public string EquipmentID { get; set; }

        /// <summary>
        /// 执行者类型
        /// </summary>
        public string ExecutorType { get; set; }

        /// <summary>
        /// 实际执行设备ID
        /// </summary>
        public string ActualEquipmentID { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>
        public decimal? SeqNo { get; set; }

        /// <summary>
        /// 优先级
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
        /// 来源地址ID
        /// </summary>
        public string SrcAddrID { get; set; }

        /// <summary>
        /// 来源地址类型
        /// </summary>
        public string SrcAddrType { get; set; }

        /// <summary>
        /// 来源地址
        /// </summary>
        public string SrcAddrText { get; set; }

        /// <summary>
        /// 目标地址ID
        /// </summary>
        public string DestAddrID { get; set; }

        /// <summary>
        /// 目标地址类型
        /// </summary>
        public string DestAddrType { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public string DestAddrText { get; set; }

        /// <summary>
        /// 实际目标地址ID
        /// </summary>
        public string ActualDestAddrID { get; set; }

        /// <summary>
        /// 实际目标地址
        /// </summary>
        public string ActualDestAddrText { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public string TaskStatus { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 任务是否可执行
        /// </summary>
        public string IsExecutable { get; set; }

        /// <summary>
        /// 优先级修改时间
        /// </summary>
        public DateTime? PriorityChangeTime { get; set; }

        /// <summary>
        /// LED显示内容
        /// </summary>
        public string LedText { get; set; }

        /// <summary>
        /// 是否存在LGV
        /// </summary>
        public string ExistLgv { get; set; }

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
