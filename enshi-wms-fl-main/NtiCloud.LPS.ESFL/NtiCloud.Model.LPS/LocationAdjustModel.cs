using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    /// <summary>
    /// 货位整理
    /// </summary>
    public class LocationAdjustModel
    {

        public class LocationAdjustH
        {
            public string adjust_no { get; set; }
            /// <summary>
            /// Client ID
            /// </summary>
            public string Client_id { get; set; }
            /// <summary>
            /// Org ID
            /// </summary>
            public string Org_id { get; set; }
            /// <summary>
            /// 调整主单ID
            /// </summary>        
            public string AdjustID { get; set; }
            /// <summary>
            /// Warehouse ID
            /// </summary>
            public string Warehouse_id { get; set; }

            /// <summary>
            /// 区域ID
            /// </summary>
            public string Zone_id { get; set; }

            /// <summary>
            ///  有码标识
            /// </summary>
            public bool IsExistsBarcode { get; set; }
        }

        public class LocationAdjustD 
        {           
            /// <summary>
            /// 调整主单ID
            /// </summary>        
            public string Adjust_hid { get; set; }
            
            /// <summary>
            /// 货位整理明细ID
            /// </summary>
            public string Adjustdetailid { get; set; }
            /// <summary>
            /// 移出货位ID
            /// </summary>
            public string Location_out_id { get; set; }
            /// <summary>
            /// 移出货位编码
            /// </summary>
            public string Location_out_code { get; set; }
            /// <summary>
            /// 移入货位ID
            /// </summary>
            public string Location_in_id { get; set; }
            /// <summary>
            /// 移入货位编码
            /// </summary>
            public string Location_in_code { get; set; }
            /// <summary>
            /// 库存容器ID
            /// </summary>
            public string Out_container_id { get; set; }
            /// <summary>
            /// 库存容器明细ID
            /// </summary>
            public string Container_d_id { get; set; }
            /// <summary>
            /// 货位调整明细状态
            /// </summary>
            public string Adjust_detail_state { get; set; }
            /// <summary>
            /// 移出物料
            /// </summary>
            public string Item_id { get; set; }
            /// <summary>
            /// 物料名称
            /// </summary>
            public string Item_name { get; set; }
            /// <summary>
            /// 调整数量
            /// </summary>           

            public decimal? Qty { get; set; }
            /// <summary>
            /// 调整物料单位
            /// </summary>
            public string Uom_qty_id { get; set; }
            /// <summary>
            /// 调整统计数量S
            /// </summary>
            public decimal? Sta_Qty { get; set; }
            /// <summary>
            /// 调整统计单位
            /// </summary>
            public string Uom_stat_id { get; set; }
            /// <summary>
            /// 供应商ID
            /// </summary>
            public string Supplier_id { get; set; }
           
            /// <summary>
            /// WMS批次号
            /// </summary>
            public string Wms_Lot_No { get; set; }
            /// <summary>
            /// 批次，生产批次或物资批次
            /// </summary>
            public string Lot_No { get; set; }
            /// <summary>
            /// 规格
            /// </summary>
            public string Speci { get; set; }
        }



        #region 库位调整入参
        /// <summary>
        /// 库位调整入参
        /// </summary>
        public class LocationadJustModel
        {
            /// <summary>
            /// 单据id
            /// </summary>
            public string LocationadJustHTid { get; set; }
        }
        #endregion

        #region 创建整行库位调整明细入参
        /// <summary>
        /// 创建整行库位调整明细入参
        /// </summary>
        public class CreateLocationadJustDetail
        {
            /// <summary>
            /// 单号
            /// </summary>
            public string Adjust_no { get; set; }
            /// <summary>
            /// 移库原因
            /// </summary>
            public string Adjust_reason { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Memo { get; set; }
            /// <summary>
            /// 执行设备id
            /// </summary>
            public string Equipment_id { get; set; }
            /// <summary>
            /// 区域id
            /// </summary>
            public string Zone_id { get; set; }
            /// <summary>
            /// 移出行号
            /// </summary>
            public string Outline { get; set; }
            /// <summary>
            /// 移入行号
            /// </summary>
            public string Inline { get; set; }
        }
        #endregion

        #region 编辑整行库位调整主档入参
        /// <summary>
        /// 编辑整行库位调整主档入参
        /// </summary>
        public class EditLocationadJustHead
        {
            /// <summary>
            /// 主档id
            /// </summary>
            public string LocationadJustHTid { get; set; }
            /// <summary>
            /// 移库原因
            /// </summary>
            public string Adjust_reason { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Memo { get; set; }
            /// <summary>
            /// 执行设备id
            /// </summary>
            public string Equipment_id { get; set; }
            /// <summary>
            /// 区域id
            /// </summary>
            public string Zone_id { get; set; }
            /// <summary>
            /// 移出行号
            /// </summary>
            public string Outline { get; set; }
            /// <summary>
            /// 移入行号
            /// </summary>
            public string Inline { get; set; }
        }
        #endregion

        #region 库位调整任务强制完成入参
        /// <summary>
        /// 库位调整任务强制完成入参
        /// </summary>
        public class ForceFinishLocationadJustTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 库位调整任务部分完成入参
        /// <summary>
        /// 库位调整任务强制完成入参
        /// </summary>
        public class PartionFinishLocationadJustTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 库位调整任务完成入参
        /// <summary>
        /// 库位调整任务完成入参
        /// </summary>
        public class FinishLocationadJustModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 库位调整任务回收入参
        /// <summary>
        /// 库位调整任务回收入参
        /// </summary>
        public class RecycleLocationadJustTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion


        #region 自动库位调整入参
        /// <summary>
        /// 自动库位调整入参
        /// </summary>
        public class AutoLocationadJustModel
        {
            /// <summary>
            /// 区域id
            /// </summary>
            public string ZoneId { get; set; }
        }
        #endregion

        #region 自动库位调整任务强制完成入参
        /// <summary>
        /// 自动库位调整任务强制完成入参
        /// </summary>
        public class ForceFinishAutoLocationadJustTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 自动库位调整任务部分完成入参
        /// <summary>
        /// 自动库位调整任务强制完成入参
        /// </summary>
        public class PartionFinishAutoLocationadJustTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 自动库位调整任务完成入参
        /// <summary>
        /// 自动库位调整任务完成入参
        /// </summary>
        public class FinishAutoLocationadJustModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

        #region 自动库位调整任务回收入参
        /// <summary>
        /// 自动库位调整任务回收入参
        /// </summary>
        public class RecycleAutoLocationadJustTaskModel
        {
            /// <summary>
            /// 任务id
            /// </summary>
            public string TaskTid { get; set; }
        }
        #endregion

    }

}
