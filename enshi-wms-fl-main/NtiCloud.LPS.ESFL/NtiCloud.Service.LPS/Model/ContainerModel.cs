using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    public class ContainerModel
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
        /// 货位ID
        /// </summary>
        public string location_id { get; set; }
        /// <summary>
        /// 容器条码
        /// </summary>
        public string container_barcode { get; set; }
        /// <summary>
        /// 是否满载标记，1，满载，0，非满载
        /// </summary>
        public string is_full { get; set; }
        /// <summary>
        /// 容器类别，虚拟，实际
        /// </summary>
        public string container_type { get; set; }
        /// <summary>
        /// 容器种类ID
        /// </summary>
        public string container_catalog_id { get; set; }
        /// <summary>
        /// 物料清单ID，生产工单号
        /// </summary>
        public string bom_id { get; set; }
        /// <summary>
        /// 原机台ID，机台退回后登记
        /// </summary>
        public string equipment_id { get; set; }
        /// <summary>
        /// 是否允许呼叫，0：不允许，1：允许
        /// </summary>
        public string allow_called { get; set; }
        /// <summary>
        /// 首次入库时间（按托盘管理的时候用）
        /// </summary>
        public DateTime first_in_date { get; set; }
        /// <summary>
        /// 标签码（RFID信息或板标签等）
        /// </summary>
        public string label_code { get; set; }
        /// <summary>
        /// 货主ID
        /// </summary>
        public string owner_id { get; set; }
        /// <summary>
        /// 0，未入，1，已 入，立体库用
        /// </summary>
        public string is_in { get; set; }
        /// <summary>
        /// 执行入库的操作人
        /// </summary>
        public string take_in_by { get; set; }
        /// <summary>
        /// 0，未出，1，已出，立体库用
        /// </summary>
        public string is_out { get; set; }
        /// <summary>
        /// 执行出库的操作人
        /// </summary>
        public string take_out_by { get; set; }
        /// <summary>
        /// 是否存放多种物料
        /// </summary>
        public string is_mixed_item { get; set; }
        /// <summary>
        /// 是否存放多个批次的物料
        /// </summary>
        public string is_mixed_lot { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_date { get; set; }
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
        public DateTime modify_date { get; set; }
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public string last_modified_by { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string act_status { get; set; }
    }

    public class ContainerDModel {
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
        /// 容器ID
        /// </summary>
        public string container_id { get; set; }
        /// <summary>
        /// 入库单ID
        /// </summary>
        public string inbound_h_id { get; set; }
        /// <summary>
        /// 入库明细ID
        /// </summary>
        public string inbound_d_id { get; set; }
        /// <summary>
        /// 备料单ID
        /// </summary>
        public string outbound_h_id { get; set; }
        /// <summary>
        /// 子容器编号
        /// </summary>
        public string sub_container_no { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string supplier_id { get; set; }
        /// <summary>
        /// 物料清单ID
        /// </summary>
        public string bom_id { get; set; }
        /// <summary>
        /// 货主ID
        /// </summary>
        public string owner_id { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string item_id { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string lot_no { get; set; }
        /// <summary>
        /// 批次序号
        /// </summary>
        public string lot_seq { get; set; }
        /// <summary>
        /// 货主批次
        /// </summary>
        public string owner_lot_no { get; set; }
        /// <summary>
        /// 库存批次
        /// </summary>
        public string wms_lot_no { get; set; }
        /// <summary>
        /// 有无箱码:0/1
        /// </summary>
        public string exist_barcode { get; set; }
        /// <summary>
        /// 是否整包:0/1
        /// </summary>
        public string is_full { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? given_year { get; set; }
        /// <summary>
        /// 收货工厂
        /// </summary>
        public string sub_factory { get; set; }
        /// <summary>
        /// 收货仓库
        /// </summary>
        public string sub_inventory { get; set; }
        /// <summary>
        /// 收货货位
        /// </summary>
        public string sub_location { get; set; }
        /// <summary>
        /// 质检状态
        /// </summary>
        public string qc_status { get; set; }
        /// <summary>
        /// 库存量
        /// </summary>
        public decimal? qty { get; set; }
        /// <summary>
        /// 统计库存量
        /// </summary>
        public decimal? stat_qty { get; set; }
        /// <summary>
        /// 待入量
        /// </summary>
        public decimal? in_qty { get; set; }
        /// <summary>
        /// 待出量
        /// </summary>
        public decimal? out_qty { get; set; }
        /// <summary>
        /// 锁定量
        /// </summary>
        public decimal? lock_qty { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string spec { get; set; }
        /// <summary>
        /// 物流单位
        /// </summary>
        public string uom_qty_id { get; set; }
        /// <summary>
        /// 统计单位
        /// </summary>
        public string uom_stat_id { get; set; }
        /// <summary>
        /// 转换比率
        /// </summary>
        public decimal? ratio { get; set; }
        /// <summary>
        /// 首次入库时间
        /// </summary>
        public DateTime? first_in_date { get; set; }
        /// <summary>
        /// 出库开始时间
        /// </summary>
        public DateTime? start_out_date { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? production_date { get; set; }
        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? exp_date { get; set; }
        /// <summary>
        /// 出库预警日期
        /// </summary>
        public DateTime? warning_date { get; set; }
        /// <summary>
        /// 优先出库日期
        /// </summary>
        public DateTime? first_out_date { get; set; }
        /// <summary>
        /// 出库截止日期
        /// </summary>
        public DateTime? out_end_date { get; set; }
        /// <summary>
        /// 可使用日期
        /// </summary>
        public DateTime? use_by_date { get; set; }
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
        /// 状态:0/1
        /// </summary>
        public string act_status { get; set; }
    }


    public class ContainerComponent {
        public ContainerModel container { get; set; }

        public List<ContainerDModel> containerdetails { get; set; }
    }
    /// <summary>
    /// 条码追溯表
    /// </summary>
    public class Container_SN_D {
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
        /// 序列码主档ID
        /// </summary>
        public string sn_id { get; set; }
        /// <summary>
        /// 源库位ID
        /// </summary>
        public string from_location_id { get; set; }
        /// <summary>
        /// 源货位号
        /// </summary>
        public string from_location_no { get; set; }
        /// <summary>
        /// 源货位名称
        /// </summary>
        public string from_location_name { get; set; }
        /// <summary>
        /// 目的库位ID
        /// </summary>
        public string to_location_id { get; set; }
        /// <summary>
        /// 目的货位号
        /// </summary>
        public string to_location_no { get; set; }
        /// <summary>
        /// 目的货位名称
        /// </summary>
        public string to_location_name { get; set; }
        /// <summary>
        /// 变动动作
        /// </summary>
        public string action { get; set; }
        /// <summary>
        /// 库存变化原因
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string doc_no { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public string task_no { get; set; }
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
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? qty { get; set; }
    }

    /// <summary>
    /// 条码表
    /// </summary>
    public class Container_SN : NtiCloud.Service.LPS.Model.BaseModel {
        /// <summary>
        /// SN码
        /// </summary>
        public string item_sn { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string item_id { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 容器ID
        /// </summary>
        public string container_id { get; set; }
        /// <summary>
        /// 容器编号
        /// </summary>
        public string container_no { get; set; }
        /// <summary>
        /// 容器明细ID
        /// </summary>
        public string container_d_id { get; set; }
        /// <summary>
        /// 货位ID
        /// </summary>
        public string location_id { get; set; }
        /// <summary>
        /// 货位名称
        /// </summary>
        public string location_name { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string zone_id { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string zone_name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? qty { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string spec { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string lot_no { get; set; }
        /// <summary>
        /// 仓库批次
        /// </summary>
        public string wms_lot_no { get; set; }
        /// <summary>
        /// 质检状态
        /// </summary>
        public string qc_status { get; set; }
        /// <summary>
        /// 存储状态，1：已存储，0：未存储
        /// </summary>
        public string store_status { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 统计数量1
        /// </summary>
        public decimal? stat_qty1 { get; set; }
        /// <summary>
        /// 统计数量2
        /// </summary>
        public decimal? stat_qty2 { get; set; }
        /// <summary>
        /// 批次属性
        /// </summary>
        public string lot_att { get; set; }
        /// <summary>
        /// 过程属性
        /// </summary>
        public string procedure_att { get; set; }
        /// <summary>
        /// 养护属性
        /// </summary>
        public string maintenance_att { get; set; }
        /// <summary>
        /// 首次入库时间
        /// </summary>
        public DateTime? first_in_date { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        public decimal? net_weight { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? gross_weight { get; set; }
        /// <summary>
        /// 含水率
        /// </summary>
        public decimal? water_rate { get; set; } 
    }

}
