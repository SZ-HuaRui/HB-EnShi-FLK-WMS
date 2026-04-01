using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{/// <summary>
/// 货位
/// </summary>
    public class LocationModel
    {/// <summary>
     /// 主键ID
     /// </summary>
        public string t_id { get; set; }
        /// <summary>
        /// 集团公司ID
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
        /// 货位号
        /// </summary>
        public string location_no { get; set; }
        /// <summary>
        /// 货位编码
        /// </summary>
        public string location_code { get; set; }
        /// <summary>
        /// 货位名称
        /// </summary>
        public string location_name { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string zone_id { get; set; }
        /// <summary>
        /// 区域ID2
        /// </summary>
        public string zone_id2 { get; set; }
        /// <summary>
        /// 区域ID3
        /// </summary>
        public string zone_id3 { get; set; }
        /// <summary>
        /// 区域ID4
        /// </summary>
        public string zone_id4 { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        public int? location_row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public int? location_column { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        public int? location_layer { get; set; }
        /// <summary>
        /// 深度
        /// </summary>
        public int? location_depth { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string floor_no { get; set; }
        /// <summary>
        /// 仓间
        /// </summary>
        public string room_no { get; set; }
        /// <summary>
        /// 单元号
        /// </summary>
        public string unit_no { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string tags { get; set; }
        /// <summary>
        /// 货位类型
        /// </summary>
        public string location_type { get; set; }
        /// <summary>
        /// 货位状态
        /// </summary>
        public string location_status { get; set; }
        /// <summary>
        /// 货位标识
        /// </summary>
        public string location_flag { get; set; }
        /// <summary>
        /// 工作状态
        /// </summary>
        public string process_status { get; set; }
        /// <summary>
        /// 货位用途
        /// </summary>
        public string usage { get; set; }
        /// <summary>
        /// 货主ID
        /// </summary>
        public string owner_id { get; set; }
        /// <summary>
        /// 容器种类ID
        /// </summary>
        public string container_catalog_id { get; set; }
        /// <summary>
        /// 是否允许混装物料
        /// </summary>
        public string is_mix_item { get; set; }
        /// <summary>
        /// 是否允许混装批次
        /// </summary>
        public string is_mix_lot { get; set; }
        /// <summary>
        /// 是否允许等级混放
        /// </summary>
        public string is_mix_grade { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        public decimal? length { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public decimal? width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public decimal? height { get; set; }
        /// <summary>
        /// 尺寸单位ID
        /// </summary>
        public string uom_id { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public decimal? seq_no { get; set; }
        /// <summary>
        /// 体积限制
        /// </summary>
        public decimal? volume_limit { get; set; }
        /// <summary>
        /// 承重限制
        /// </summary>
        public decimal? weight_limit { get; set; }
        /// <summary>
        /// 数量限制
        /// </summary>
        public decimal? qty_limit { get; set; }
        /// <summary>
        /// CONTAINER_LIMIT
        /// </summary>
        public decimal? container_limit { get; set; }
        /// <summary>
        /// 数量单位ID
        /// </summary>
        public string uom_qtyid { get; set; }
        /// <summary>
        /// 重量单位
        /// </summary>
        public string weightid { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// X座标
        /// </summary>
        public int? xcoord { get; set; }
        /// <summary>
        /// Y座标
        /// </summary>
        public int? ycoord { get; set; }
        /// <summary>
        /// Z座标
        /// </summary>
        public int? zcoord { get; set; }
        /// <summary>
        /// X像素
        /// </summary>
        public int? xdistance { get; set; }
        /// <summary>
        /// Y像素
        /// </summary>
        public int? ydistance { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime? inbound_date { get; set; }
        /// <summary>
        /// 垛型
        /// </summary>
        public string packing_type { get; set; }
        /// <summary>
        /// 货位状态变动原因
        /// </summary>
        public string status_reason { get; set; }
        /// <summary>
        /// 货位标识变动原因
        /// </summary>
        public string flag_reason { get; set; }
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
        /// 状态
        /// </summary>
        public string act_status { get; set; }

        public string locationformat { get; set; }
    }
}
