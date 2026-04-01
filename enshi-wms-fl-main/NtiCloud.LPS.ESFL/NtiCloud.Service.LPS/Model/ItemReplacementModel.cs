using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class ItemReplacementModel
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
        /// 物料ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 物料条码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 原生产批次
        /// </summary>
        public string ProductionLotNo { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// 任务号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 货位ID
        /// </summary>
        public string LocationID { get; set; }

        /// <summary>
        /// 货位编号
        /// </summary>
        public string LocationNo { get; set; }

        /// <summary>
        /// 出库单ID
        /// </summary>
        public string OutboundHeadID { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundNo { get; set; }

        /// <summary>
        /// 统计数量
        /// </summary>
        public decimal? StatQty { get; set; }

        /// <summary>
        /// 统计数量单位
        /// </summary>
        public string UomStatID { get; set; }

        /// <summary>
        /// 新物料ID
        /// </summary>
        public string NewItemID { get; set; }

        /// <summary>
        /// 新物料编码
        /// </summary>
        public string NewItemCode { get; set; }

        /// <summary>
        /// 新物料名称
        /// </summary>
        public string NewItemName { get; set; }

        /// <summary>
        /// 新物料条码
        /// </summary>
        public string NewBarcode { get; set; }

        /// <summary>
        /// 新任务ID
        /// </summary>
        public string NewTaskID { get; set; }

        /// <summary>
        /// 新任务号
        /// </summary>
        public string NewTaskNo { get; set; }

        /// <summary>
        /// 新货位ID
        /// </summary>
        public string NewLocationID { get; set; }

        /// <summary>
        /// 新货位编号
        /// </summary>
        public string NewLocationNo { get; set; }

        /// <summary>
        /// 新数量
        /// </summary>
        public decimal? NewStatQty { get; set; }

        /// <summary>
        /// 入库单ID
        /// </summary>
        public string InboundHeadID { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string InboundNo { get; set; }

        /// <summary>
        /// 备料批次
        /// </summary>
        public string PrepLotNo { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 异常数量(kg)
        /// </summary>
        public decimal? AbnormalStatQty { get; set; }

        /// <summary>
        /// 替换状态
        /// </summary>
        public string ReplacementStatus { get; set; }

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
        /// 状态，默认值为'1'。'0'：已删除'1'：未删除
        /// </summary>
        public string ActStatus { get; set; }

    }

    [Serializable]
    public class ItemReplacementModelDB
    {

        /// <summary>
        /// 主键ID
        /// </summary>
        public string t_id { get; set; }

        /// <summary>
        /// 集团ID
        /// </summary>
        public string client_id { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string org_id { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string warehouse_id { get; set; }

        /// <summary>
        /// 物料ID
        /// </summary>
        public string item_id { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string item_code { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }

        /// <summary>
        /// 物料条码
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// 原生产批次
        /// </summary>
        public string production_lot_no { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public string task_id { get; set; }

        /// <summary>
        /// 任务号
        /// </summary>
        public string task_no { get; set; }

        /// <summary>
        /// 货位ID
        /// </summary>
        public string location_id { get; set; }

        /// <summary>
        /// 货位编号
        /// </summary>
        public string location_no { get; set; }

        /// <summary>
        /// 出库单ID
        /// </summary>
        public string outbound_h_id { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string outbound_no { get; set; }

        /// <summary>
        /// 统计数量
        /// </summary>
        public decimal? stat_qty { get; set; }

        /// <summary>
        /// 统计数量单位
        /// </summary>
        public string uom_stat_id { get; set; }

        /// <summary>
        /// 新物料ID
        /// </summary>
        public string new_item_id { get; set; }

        /// <summary>
        /// 新物料编码
        /// </summary>
        public string new_item_code { get; set; }

        /// <summary>
        /// 新物料名称
        /// </summary>
        public string new_item_name { get; set; }

        /// <summary>
        /// 新物料条码
        /// </summary>
        public string new_barcode { get; set; }

        /// <summary>
        /// 新任务ID
        /// </summary>
        public string new_task_id { get; set; }

        /// <summary>
        /// 新任务号
        /// </summary>
        public string new_task_no { get; set; }

        /// <summary>
        /// 新货位ID
        /// </summary>
        public string new_location_id { get; set; }

        /// <summary>
        /// 新货位编号
        /// </summary>
        public string new_location_no { get; set; }

        /// <summary>
        /// 新数量
        /// </summary>
        public decimal? new_stat_qty { get; set; }

        /// <summary>
        /// 入库单ID
        /// </summary>
        public string inbound_h_id { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string inbound_no { get; set; }

        /// <summary>
        /// 备料批次
        /// </summary>
        public string prep_lot_no { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 替换单号
        /// </summary>
        public string replacement_no { get; set; }

        /// <summary>
        /// 异常数量(kg)
        /// </summary>
        public decimal? abnormal_stat_qty { get; set; }

        /// <summary>
        /// 替换状态
        /// </summary>
        public string replacement_status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_date { get; set; }

        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public string created_by { get; set; }

        /// <summary>
        /// 操作人IP 
        /// </summary>
        public string modify_ip { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? modify_date { get; set; }

        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public string last_modified_by { get; set; }

        /// <summary>
        /// 状态，默认值为'1'。'0'：已删除'1'：未删除
        /// </summary>
        public string act_status { get; set; }
        /// <summary>
        /// 替换补充条码(醇化库补发条码)
        /// </summary>
        public string resendbarcode { get; set; }

    }


    public class ItemReplacementOutModel
    {

        /// <summary>
        /// 烟包替换主键ＩＤ
        /// </summary>
        public string t_id { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string item_code { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 烟包重量
        /// </summary>
        public decimal? spec { get; set; }
        /// <summary>
        /// 被替换烟包对应的容器ID
        /// </summary>
        public string container_id { get; set; }
        /// <summary>
        /// 被替换烟包对应的容器明细ID
        /// </summary>
        public string container_d_id { get; set; }
        /// <summary>
        /// 霉变日期
        /// </summary>
        public DateTime? create_date { get; set; }

    }
}
