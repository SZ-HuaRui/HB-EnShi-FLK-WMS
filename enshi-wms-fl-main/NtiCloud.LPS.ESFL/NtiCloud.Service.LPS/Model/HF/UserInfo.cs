using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model.HF
{
   
    /// <summary>
    ///组织
    /// </summary>
    public class Organization
    {


        private string _orgid;
        /// <summary>
        /// 组织ID（GUID）
        /// </summary>
        public string OrgId
        {
            get { return _orgid; }
            set { _orgid = value; }
        }

        private string _code;
        /// <summary>
        /// 组织编码
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _name;
        /// <summary>
        /// 组织名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private DateTime _createtime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }

        private string _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

    }


    public class Warehouse
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string orgid { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string warehouseid { get; set; }
        /// <summary>
        /// 仓库代码
        /// </summary>
        public string warehousecode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string name { get; set; } 
    }

    public class Location {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationNumber { get; set; }
        public string WarehouseID { get; set; }
        public string House { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public string Local { get; set; }
        public string OrgId { get; set; }
        public string WarehouseName { get; set; }
        public string OrgName { get; set; }
        public string ModifyuserName { get; set; } 
    }

    public class SysType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
        public string SequenceNumber { get; set; }
        public string DefaultDescription { get; set; }
    }

    public class OutboundTaskPlanList
    {
        /// <summary>
        /// 出库计划单号
        /// </summary>
        public string plan_order_no { get; set; }
        /// <summary>
        /// 牌号
        /// </summary>
        public string productname { get; set; }
        /// <summary>
        /// 批次数
        /// </summary>
        public decimal batchnum { get; set; }
        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime? createdate { get; set; }
        /// <summary>
        /// 任务俩表
        /// </summary>
        public List<OutboundTaskList> lstTaskList { get; set; }
        /// <summary>
        /// 出库计划已扫描的出库条码
        /// </summary>
        public List<NtiCloud.Model.WMS.OutbounddetailBarcodeOut> lstShippedBarcodeList { get; set; }

    }
    
    public class OutboundTaskList
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string orgid { get; set; }
        public string orgcode { get; set; }
        public string orgname { get; set; }
        public string warehousecode { get; set; }
        public string warename { get; set; }
        public string locationnumber { get; set; }
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string id { get; set; }
       /// <summary>
       /// 出库单号
       /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 出库项次
        /// </summary>
        public string order_id { get; set; }
        public string warehouseid { get; set; } 

        public decimal? qty { get; set; }
        public decimal? weight { get; set; }
        public decimal? spec { get; set; }
        public string itemname { get; set; }
        public string itemcode { get; set; }
        public string itemid { get; set; }
        public string loactionid { get; set; }
        public string locationid { get; set; }
        public string locationname { get; set; }
        /// <summary>
        /// 任务数量
        /// </summary>
        public decimal? actualqty { get; set; }
        /// <summary>
        /// 任务重量
        /// </summary>
        public decimal? actualweight { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string rf_status { get; set; }
        /// <summary>
        /// 已拣选数量
        /// </summary>
        public decimal? rf_qty { get; set; }
        /// <summary>
        /// 已拣选重量
        /// </summary>
        public decimal? rf_weight { get; set; }
        /// <summary>
        /// 待执行数量
        /// </summary>
        public decimal? surplusqty { get; set; } 
        /// <summary>
        /// 已扫描烟包数
        /// </summary>
        public int? scanedqty { get; set; }
        /// <summary>
        /// 条码列表
        /// </summary>
        public List<NtiCloud.Model.WMS.OutbounddetailBarcodeOut> lstBarcodes { get; set; }
    }
    /// <summary>
    /// 楼层
    /// </summary>
    public class Floor { 
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string warehouseid { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string warehousename { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string floor { get; set; }
    }
    /// <summary>
    /// 仓间
    /// </summary>
    public class FloorRoom
    {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string warehouseid { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string warehousename { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 仓间
        /// </summary>
        public string room { get; set; }
    }
}
