using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Model
{
    public class ProdPlanInModel:BaseModel
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string equipment_id { get; set; }
        /// <summary>
        /// 计划单号
        /// </summary>
        public string plan_no { get; set; }
        /// <summary>
        /// 来源单号
        /// </summary>
        public string ref_doc_no { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string item_id { get; set; }
        /// <summary>
        /// 产品编码
        /// </summary>
        public string item_code { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 生产批次
        /// </summary>
        public string lot_no { get; set; }
        /// <summary>
        /// 仓库批次
        /// </summary>
        public string wms_lot_no { get; set; }
        /// <summary>
        /// 班次ID
        /// </summary>
        public string shift_id { get; set; }
        /// <summary>
        /// 班组
        /// </summary>
        public string team { get; set; }
        /// <summary>
        /// 有无箱码
        /// </summary>
        public string exist_barcode { get; set; }
        /// <summary>
        /// 来源设备ID，对成品库来说是封箱机ID
        /// </summary>
        public string src_equipment_id { get; set; }
        /// <summary>
        /// 货主ID
        /// </summary>
        public string owner_id { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public decimal? plan_qty { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public decimal? finished_qty { get; set; }
        /// <summary>
        /// 物流单位
        /// </summary>
        public string uom_qty_id { get; set; }
        /// <summary>
        /// 计划统计数量
        /// </summary>
        public decimal? plan_stat_qty { get; set; }
        /// <summary>
        /// 完成统计数量
        /// </summary>
        public decimal? finished_stat_qty { get; set; }
        /// <summary>
        /// 统计数量单位
        /// </summary>
        public string uom_stat_id { get; set; }

        /// <summary>
        /// 是否启用数量控制：1：启用，0：不启用
        /// </summary>
        public string is_qty_controlled { get; set; }
        /// <summary>
        /// 是否允许结束计划
        /// </summary>
        public string allow_end { get; set; }
        /// <summary>
        /// 是否自动创建
        /// </summary>
        public string is_auto_created { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public string plan_status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? start_time { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? end_time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; } 
    }
}
