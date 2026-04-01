using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class PutawayModelInput
    {

        /// <summary>
        /// 上架单ID
        /// </summary>
        public string PutawayId { get; set; }
        /// <summary>
        /// 上架单号
        /// </summary>
        public string PutawayNo { get; set; }
        /// <summary>
        /// 上架单明细ID
        /// </summary>
        public string PutawayDetailId { get; set; }
        /// <summary>
        /// 货主
        /// </summary>
        public string owner_id { get; set; }
    }
    /// <summary>
    /// 分配货位需要传入的参数信息
    /// </summary>
    public class AllocateLocationIn {
        public string warehouserid { get; set; }
        public string itemid { get; set; }
        public string itemcode { get; set; }

        public string itemName { get; set; }
        public string owner_id { get; set; }
        public string supplier_id { get; set; }
        public string lotno { get; set; }
        public string wmslotno { get; set; }
        public string proucedate { get; set; }

        public decimal qty { get; set; }

        public string ref_doc_type { get; set; }

        public string ref_doc_id { get; set; }

        public string ref_doc_no { get; set; }
        public string ref_doc_d_id { get; set; }
    }
    /// <summary>
    /// 分配的单个货位信息
    /// </summary>
    public class AllocateLocationOut {
        public bool isSuccess { get; set; }
        public string locationId { get; set; }

        public string locationCode { get; set; }

        public string containerNo { get; set; }
        public decimal allocateQty { get; set; }
    }


    public class ModifyAllocLocationIn
    {
        /// <summary>
        /// 操作类型:新增/删除/修改
        /// </summary>
        public string actionType { get; set; }
        /// <summary>
        /// 上架单ID
        /// </summary>
        public string inbound_id { get; set; }
        /// <summary>
        /// 上架单明细ID
        /// </summary>
        public string inbound_detail_id { get; set; }
        /// <summary>
        /// 容器id
        /// </summary>
        public string container_id { get; set; }
        /// <summary>
        /// 容器明细ID
        /// </summary>
        public string container_d_id { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public string task_id { get; set; }
        /// <summary>
        /// 任务明细ID
        /// </summary>
        public string task_item_id { get; set; }
        /// <summary>
        /// 目标货位
        /// </summary>
        public string dest_addr_text { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public decimal? task_plan_qty { get; set; }


    }

    /// <summary>
    /// 查询待执行任务输入参数模型
    /// </summary>
    public class TaskdoingIn {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string WarehouseId { get; set; }
        /// <summary>
        /// 任务来源单号
        /// </summary>
        public string RefDocno { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// 任务行ID
        /// </summary>
        public string TaskItemId { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 货位
        /// </summary>
        public string LocationCode { get; set; }

        /// <summary>
        /// 任务执行人
        /// </summary>
        public string ExcuteBy { get; set; }
    }

    /// <summary>
    /// 上架到货位（有扫描箱码）
    /// </summary>
    public class PutawayToLocation {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 任务行ID
        /// </summary>
        public string TaskItemId { get; set; }
        /// <summary>
        /// 上架货位
        /// </summary>
        public string ToLocationCode { get; set; }
        /// <summary>
        /// 上架数量
        /// </summary>
        public decimal InQty { get; set; }
        /// <summary>
        /// 扫描的箱码SN
        /// </summary>
        public List<SNModel> LstSN { get; set; }

    }
    public class SNModel{
        public string SN { get; set; }
        public decimal? Qty { get; set; }
    }

    /// <summary>
    /// 直接上架
    /// </summary>
    public class PutawayDirectIn { 
        /// <summary>
        /// 上架单
        /// </summary>
        public string PutawayNo { get; set; } 
      
        /// <summary>
        /// 物料编码
        /// </summary>
        public string ItemCode { get; set; }
       
         
    }

    public class PutawayDirectDoneIn
    { 
        /// <summary>
        /// 上架单
        /// </summary>
        public string PutawayNo { get; set; }

        /// <summary>
        /// 上架单ID
        /// </summary>
        public string PutawayId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string ItemCode { get; set; } 
        /// <summary>
        /// 上架单明细ID
        /// </summary>
        public string PutawayDetailId { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>

        public DateTime? ProduceDate { get; set; }
        /// <summary>
        /// 批次
        /// </summary>

        public string LotNo { get; set; }

 
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 上架货位
        /// </summary>
        public string ToLocation { get; set; }
    }

    /// <summary>
    /// 霉变烟包替换入参
    /// </summary>
    public class ItemReplaceInModel
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

        /// <summary>
        /// 替换后的新烟包条码
        /// </summary>
        public string new_container_code { get; set; }
    }



}
