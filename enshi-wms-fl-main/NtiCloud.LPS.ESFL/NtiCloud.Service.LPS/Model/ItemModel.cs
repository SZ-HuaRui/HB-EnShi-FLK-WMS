using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Model
{
    public class ItemModel:BaseModel
    {
        /// <summary>
        /// 物料编码，唯一编码
        /// </summary>
        public string item_code { get; set; }
        /// <summary>
        /// 物料编码2
        /// </summary>
        public string item_code2 { get; set; }
        /// <summary>
        /// 物料编码3
        /// </summary>
        public string item_code3 { get; set; }
        /// <summary>
        /// 物料编码4
        /// </summary>
        public string item_code4 { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 物料别名
        /// </summary>
        public string item_alias { get; set; }
        /// <summary>
        /// 物料描述
        /// </summary>
        public string item_description { get; set; }
        /// <summary>
        /// 入库时是否需要质检
        /// </summary>
        public string need_qc { get; set; }
        /// <summary>
        /// 物料初始入库检验状态
        /// </summary>
        public string qc_status { get; set; }
        /// <summary>
        /// 物料启用，1启用，0禁用
        /// </summary>
        public string is_enabled { get; set; }
        /// <summary>
        /// 小数位数，财务单位的精度
        /// </summary>
        public int? scale { get; set; }
        /// <summary>
        /// 进位值
        /// </summary>
        public int? round_value { get; set; }
        /// <summary>
        /// 默认容器载量
        /// </summary>
        public decimal? max_container_qty { get; set; }
        /// <summary>
        /// 码盘方式（机械手）
        /// </summary>
        public string pallet_stacking_mode { get; set; }
        /// <summary>
        /// 种类ID
        /// </summary>
        public string catalog_id { get; set; }
        /// <summary>
        /// ABC分类
        /// </summary>
        public string abc { get; set; }
        /// <summary>
        /// 批次编码规则ID，用于生成仓库用批次属性
        /// </summary>
        public string lot_rule_id { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string origin { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string item_grade { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? given_year { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        public decimal? item_length { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public decimal? item_width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public decimal? item_height { get; set; }
        /// <summary>
        /// 尺寸单位ID
        /// </summary>
        public string size_uom_id { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        public decimal? net_weight { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? gross_weight { get; set; }
        /// <summary>
        /// 皮重
        /// </summary>
        public decimal? tare { get; set; }
        /// <summary>
        /// 重量计量单位ID
        /// </summary>
        public string weight_uom_id { get; set; }
        /// <summary>
        /// 默认库存数量单位
        /// </summary>
        public string default_pack_uom_id { get; set; }
        /// <summary>
        /// 默认库存统计单位
        /// </summary>
        public string default_stat_uom_id { get; set; }
        /// <summary>
        /// 默认物流数量到默认统计数量的转换率，与规格联用计算
        /// </summary>
        public decimal? rate { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string pic_path { get; set; }
        /// <summary>
        /// 默认规格
        /// </summary>
        public string spec { get; set; }
        /// <summary>
        /// 标签，用英文逗号分开
        /// </summary>
        public string tags { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 安全库存，0表示无安全库存。
        /// </summary>
        public decimal? safety_stock_qty { get; set; }
        /// <summary>
        /// 是否管理物料标签
        /// </summary>
        public string is_control_barcode { get; set; }
        /// <summary>
        /// 最少在库时间，单位分钟
        /// </summary>
        public int aging_time { get; set; }
        /// <summary>
        /// 最长在库时间，单位分钟
        /// </summary>
        public int? max_aging_time { get; set; }
        /// <summary>
        /// 是否控制有效期
        /// </summary>
        public string is_control_expiry_date { get; set; }
        /// <summary>
        /// 有效期控制类型
        /// </summary>
        public string expiry_date_control_type { get; set; }
        /// <summary>
        /// 有效期天数
        /// </summary>
        public int? expiry_days { get; set; }
        /// <summary>
        /// 最大有效期天数。
        /// </summary>
        public int? max_expiry_days { get; set; }
        /// <summary>
        /// 失效期预警天数
        /// </summary>
        public int? alert_days { get; set; }
        /// <summary>
        /// 入库有效期
        /// </summary>
        public int? stock_in_expiry_minutes { get; set; }
        /// <summary>
        /// 优先出库时间
        /// </summary>
        public int? out_first_days { get; set; }
        /// <summary>
        /// 出库有效期天数
        /// </summary>
        public int? stock_out_expiry_days { get; set; }
        /// <summary>
        /// 货主ID
        /// </summary>
        public string owner_id { get; set; }
      
    }
}
