using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    /// <summary>
    /// 任务信息
    /// </summary>
    public class TaskHeaderModel
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
        /// 区域ID
        /// </summary>
        public string zone_id { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string task_no { get; set; }
        /// <summary>
        /// 依赖任务号
        /// </summary>
        public string dependent_task_no { get; set; }
        /// <summary>
        /// 任务类型编码
        /// </summary>
        public string task_type { get; set; }
        /// <summary>
        /// 任务执行人
        /// </summary>
        public string executed_by { get; set; }
        /// <summary>
        /// 执行设备ID
        /// </summary>
        public string equipment_id { get; set; }
        /// <summary>
        /// 执行者类型
        /// </summary>
        public string executor_type { get; set; }
        /// <summary>
        /// 实际执行设备ID
        /// </summary>
        public string actual_equipment_id { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int seq_no { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int priority { get; set; }
        /// <summary>
        /// 容器ID
        /// </summary>
        public string container_id { get; set; }
        /// <summary>
        /// 容器条码
        /// </summary>
        public string container_barcode { get; set; }
        /// <summary>
        /// 来源单据类型
        /// </summary>
        public string ref_doc_type { get; set; }
        /// <summary>
        /// 来源单ID
        /// </summary>
        public string ref_doc_id { get; set; }
        /// <summary>
        /// 来源单明细ID
        /// </summary>
        public string ref_doc_d_id { get; set; }
        /// <summary>
        /// 来源单号
        /// </summary>
        public string ref_doc_no { get; set; }
        /// <summary>
        /// 来源地址ID
        /// </summary>
        public string src_addr_id { get; set; }
        /// <summary>
        /// 来源地址类型
        /// </summary>
        public string src_addr_type { get; set; }
        /// <summary>
        /// 来源地址
        /// </summary>
        public string src_addr_text { get; set; }
        /// <summary>
        /// 目标地址ID
        /// </summary>
        public string dest_addr_id { get; set; }
        /// <summary>
        /// 目标地址类型
        /// </summary>
        public string dest_addr_type { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string dest_addr_text { get; set; }
        /// <summary>
        /// 实际目标地址ID
        /// </summary>
        public string actual_dest_addr_id { get; set; }
        /// <summary>
        /// 实际目标地址
        /// </summary>
        public string actual_dest_addr_text { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string task_status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 任务是否可执行
        /// </summary>
        public string is_executable { get; set; }
        /// <summary>
        /// 优先级修改时间
        /// </summary>
        public DateTime? priority_change_time { get; set; }
        /// <summary>
        /// LED显示内容
        /// </summary>
        public string led_text { get; set; }
        /// <summary>
        /// 是否存在LGV
        /// </summary>
        public string exist_lgv { get; set; }
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
        /// 任务明细
        /// </summary>
        public List<TaskItemModel> lstTaskItems { get; set; }
    }

    /// <summary>
    /// 任务物料信息
    /// </summary>
    public class TaskItemModel {
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
        /// 主任务ID
        /// </summary>
        public string task_id { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string item_id { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public decimal? plan_qty { get; set; }
        /// <summary>
        /// 已处理数量
        /// </summary>
        public decimal? processed_qty { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public decimal? finished_qty { get; set; }
        /// <summary>
        /// 数量单位
        /// </summary>
        public string uom_qty_id { get; set; }
        /// <summary>
        /// 完成统计数量
        /// </summary>
        public decimal finished_stat_qty { get; set; }
        /// <summary>
        /// 统计数量单位
        /// </summary>
        public string uom_stat_id { get; set; }
        /// <summary>
        /// 容器明细ID
        /// </summary>
        public string container_d_id { get; set; }
        /// <summary>
        /// 来源单据类型
        /// </summary>
        public string ref_doc_type { get; set; }
        /// <summary>
        /// 来源单ID
        /// </summary>
        public string ref_doc_id { get; set; }
        /// <summary>
        /// 来源单明细ID
        /// </summary>
        public string ref_doc_d_id { get; set; }
        /// <summary>
        /// 来源单号
        /// </summary>
        public string ref_doc_no { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 工作状态
        /// </summary>
        public string process_status { get; set; }
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
        /// 同物料分组
        /// </summary>
        public string group_num { get; set; }
    }


    public class TaskItemForRfModel : TaskItemModel { 
        /// <summary>
        /// 货位
        /// </summary>
        public string to_location { get; set; }
        public decimal? waiting_qty { get; set; }
    }
}
