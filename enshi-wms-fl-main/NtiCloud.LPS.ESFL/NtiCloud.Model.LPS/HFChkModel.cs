using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NtiCloud.Model.WMS
{
    /// <summary>
    /// 编辑仓库分配输入参数
    /// </summary>
    public class EditWarhouseIn
    {
        /// <summary>
        /// 操作类型：新增、删除、修改
        /// </summary>
        public OperationType operation_type { get; set; }
        /// <summary>
        /// 单据ID
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 产品代码
        /// </summary>
        public string productcode { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string productname { get; set; }
        /// <summary>
        /// 单据类型
        /// </summary>
        public string order_type { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 批次数
        /// </summary>
        public string batchnum { get; set; }

        /// <summary>
        /// 单据明细信息
        /// </summary>
        public List<EditWarhouseDetailIn> lstDetail { get; set; }
    }
    public class EditWarhouseDetailIn
    {
        /// <summary>
        /// 操作类型：新增、删除、修改
        /// </summary>
        public OperationType operation_type { get; set; }
        /// <summary>
        /// 单据ID
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 明细ID
        /// </summary>
        public string orderdetail_id { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemcode { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal? weight { get; set; }
        /// <summary>
        /// 件数
        /// </summary>
        public decimal? qty { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public decimal? specification { get; set; }
        /// <summary>
        /// 收货仓库
        /// </summary>
        public string rcvwarehouseid { get; set; }
        /// <summary>
        /// 发料仓库
        /// </summary>
        public string warehouseid { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }

    public enum OperationType
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        add,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        update,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        delete
    }
    /// <summary>
    /// 确认仓库分配输入参数
    /// </summary>
    public class ConfirmWarehouseIn
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string orgid { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string order_type { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }

        public List<ConfirmWarehouseDetailIn> lstDetail { get; set; }
    }

    public class ConfirmWarehouseDetailIn
    {
        /// <summary>
        /// 单据行ID
        /// </summary>
        public string orderdetail_id { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public decimal qty { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal weight { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string specifications_kg { get; set; }

        /// <summary>
        /// 发货仓库ID
        /// </summary>
        public string warehouseid { get; set; }

        /// <summary>
        /// 收货仓库ID
        /// </summary>
        public string rcvwarehouseid { get; set; }
    }
    /// <summary>
    /// 编辑出库单输入参数
    /// </summary>
    public class EditOutboundIn {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string orgid { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 出库单ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 出库仓库ID
        /// </summary>
        public string warehouseid { get; set; }
        /// <summary>
        /// 收货仓库ID
        /// </summary>
        public string arr_warehouseid { get; set; }
        /// <summary>
        /// 去向类型
        /// </summary>
        public string flow_type { get; set; }
        /// <summary>
        /// 去向
        /// </summary>
        public string transfers { get; set; }
        /// <summary>
        /// 去向代码
        /// </summary>
        public string transferscode { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public string settlement_type { get; set; }
        /// <summary>
        /// 结算单位
        /// </summary>
        public string chargeunit { get; set; }
        /// <summary>
        /// 结算单位代码
        /// </summary>
        public string chargeunitcode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 退货原因
        /// </summary>
        public string returnreason { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }
        /// <summary>
        /// 是否有条码
        /// </summary>
        public string isbarcode { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string productname { get; set; }
        /// <summary>
        /// 产品编码
        /// </summary>
        public string productcode { get; set; }
        /// <summary>
        /// 批次数
        /// </summary>
        public string batchnum { get; set; }
        /// <summary>
        /// 调入工厂
        /// </summary>
        public string rcvorgid { get; set; }
        /// <summary>
        /// 销售合同号
        /// </summary>
        public string vcontractno { get; set; }
        /// <summary>
        /// 出库单明细
        /// </summary>
        public List<EditOutboundDetailIn> lstDetail { get; set; }

    }
    public class EditOutboundDetailIn {
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string outbounddetail_id { get; set; }
        /// <summary>
        /// 出库明细ID？
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string specifications_kg { get; set; }
        /// <summary>
        /// 实际重量
        /// </summary>
        public string actualweight { get; set; }
        /// <summary>
        /// 实际数量
        /// </summary>
        public string actualqty { get; set; }
        /// <summary>
        /// 货位ID
        /// </summary>
        public string loactionid { get; set; }
        /// <summary>
        /// 仓耗重量
        /// </summary>
        public string lossweight { get; set; }
        /// <summary>
        /// 仓耗数量
        /// </summary>
        public string lossqty { get; set; }
    }

    /// <summary>
    /// 出库同步ERP参数
    /// </summary>
    public class SysErpIn {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }
    }
    /// <summary>
    /// 同步配方参数
    /// </summary>
    public class SyncPrescriptionParam {
        /// <summary>
        /// 订单号
        /// </summary>
        public string order_no { get; set; }
    }
    /// <summary>
    /// 新增出库质检单头参数
    /// </summary>
    public class AddOutCheckHeaderIn {
        /// <summary>
        /// 组织ID
        /// </summary>
        public string orgid { get; set; }
        /// <summary>
        /// 质检单号
        /// </summary>
        public string checkno { get; set; }
        /// <summary>
        /// 参考单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string create_by { get; set; }

        /// <summary>
        /// 创建人时间:yyyy-MM-dd
        /// </summary>
        public string create_date { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        public List<AddOutCheckDetailIn> lstDetail { get; set; }
    }
    public class AddOutCheckDetailIn
    {
        /// <summary>
        /// 单据项次
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }
        /// <summary>
        /// 单包重量
        /// </summary>
        public decimal specifications_kg { get; set; }

        /// <summary>
        /// 出库重量
        /// </summary>
        public decimal weight { get; set; }
        /// <summary>
        ///出库件数
        /// </summary>
        public decimal qty { get; set; }

        /// <summary>
        /// 抽检重量
        /// </summary>
        public decimal checkweight { get; set; }
        /// <summary>
        ///抽检件数
        /// </summary>
        public decimal checkqty { get; set; }
        /// <summary>
        /// 烟包外观
        /// </summary>
        public string appearance { get; set; }
        /// <summary>
        /// 烟叶水分
        /// </summary>
        public string humidity { get; set; }
        /// <summary>
        /// 包芯温度
        /// </summary>
        public string temperature { get; set; }
        /// <summary>
        /// 烟叶霉变
        /// </summary>
        public string mildew { get; set; }
        /// <summary>
        /// 烟叶虫情
        /// </summary>
        public string pest { get; set; }
        /// <summary>
        /// 压油
        /// </summary>
        public string oilpressure { get; set; }
        /// <summary>
        /// 板结
        /// </summary>
        public string compaction { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 质检结果
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 货位ID
        /// </summary>
        public string locationid { get; set; }
    }

    public class ConfirmLocationIn {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }

        /// <summary>
        /// 总批次数
        /// </summary>
        public decimal batchnum { get; set; }

        /// <summary>
        /// 已执行批次数
        /// </summary>
        public decimal batchexnum { get; set; }

        /// <summary>
        /// 本次执行批次数
        /// </summary>
        public decimal thisexnum { get; set; }

        /// <summary>
        /// 有无条码
        /// </summary>
        public string isbarcode { get; set; }
    }

    public class UserInfo
    {

        private string _id;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _account;
        /// <summary>
        /// 帐号
        /// </summary>
        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }

        private string _password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _chinesename;
        /// <summary>
        /// 中文名
        /// </summary>
        public string ChineseName
        {
            get { return _chinesename; }
            set { _chinesename = value; }
        }

        private string _englishname;
        /// <summary>
        /// 英文名
        /// </summary>
        public string EnglishName
        {
            get { return _englishname; }
            set { _englishname = value; }
        }

        private string _sex;
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        private DateTime _birthday;
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday
        {
            get { return _birthday; }
            set { _birthday = value; }
        }

        private string _mobile;
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        private string _tel;
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get { return _tel; }
            set { _tel = value; }
        }

        private string _email;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
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

        private DateTime _createdate;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get { return _createdate; }
            set { _createdate = value; }
        }

        private decimal _status;
        /// <summary>
        /// 状态
        /// </summary>
        public decimal Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _orgid;
        /// <summary>
        /// 组织Id
        /// </summary>
        public string OrgId
        {
            get { return _orgid; }
            set { _orgid = value; }
        }

        private string _warehousecode;
        /// <summary>
        /// 仓库编码,各个编码用英文逗号分开
        /// </summary>
        public string WarehouseCode
        {
            get { return _warehousecode; }
            set { _warehousecode = value; }
        }

        private string _enterpriseid;
        /// <summary>
        /// 复烤厂Id
        /// </summary>
        public string EnterpriseID
        {
            get { return _enterpriseid; }
            set { _enterpriseid = value; }
        }

        //冗余字段
        private string _orgname;
        /// <summary>
        /// 企业名称
        /// </summary>
        public string OrgName
        {
            get { return _orgname; }
            set { _orgname = value; }
        }


        private decimal _errortimes;
        /// <summary>
        /// 登录错误次数
        /// </summary>
        public decimal ErrorTimes
        {
            get { return _errortimes; }
            set { _errortimes = value; }
        }
    }
    public class SynOutboundTaskIn {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string WarehouseId { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string Floor { get; set; }
        /// <summary>
        /// 仓间
        /// </summary>
        public string Room { get; set; }
        /// <summary>
        /// 货位
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 来源单号
        /// </summary>
        public string ref_order_no { get; set; }
    }
    /// <summary>
    /// 拣料提交参数
    /// </summary>
    public class SubmitOutboudDetailIn {
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string OutboundOrderDetailId { get; set; }
        /// <summary>
        /// 本次提交数量
        /// </summary>
        public decimal TotalGW { get; set; }
        /// <summary>
        /// 提交的条码信息
        /// </summary>
        public List<PickedBarcode> lstDetails { set; get; }
    }
    public class PickedBarcode
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Specis { get; set; }
    }
    public class OutbounddetailBarcodeOut {
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string itembarcode { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal weight { get; set; }
    }
    public class MergebarcodeIn
    {
        /// <summary>
        /// 合并之后的新条码
        /// </summary>
        public string NewBarcode { get; set; }
        /// <summary>
        /// 合并之后的旧条码列表
        /// </summary>
        public List<string> OldBarcodes { get; set; }
    }
    public class MergebarcodeNewIn
    {
        /// <summary>
        /// 合包批次流水号
        /// </summary>
        public string MergeBatchId { get; set; }
        /// <summary>
        /// 合并之后的旧条码列表
        /// </summary>
        public List<BarcodeMergeInfo> OldBarcodes { get; set; }
    }
    public class MergebarcodeOut
    {
        /// <summary>
        /// 合包批次流水号
        /// </summary>
        public string MergeBatchId { get; set; }
        /// <summary>
        /// 是否处理成功
        /// </summary>
        public bool IsSuccess{get;set;}
        /// <summary>
        /// 处理失败时显示失败原因
        /// </summary>
        public string msg { get; set; }     
    }
    public class BarcodeMergeInfo
    {
        /// <summary>
        /// 烟包条码
        /// </summary>
        public string itembarcode { get; set; } 

        /// <summary>
        /// 烟包重量
        /// </summary>
        public decimal speci { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string orderdetailid { get; set; } 

    }
    public class BarcodeInfo {
        /// <summary>
        /// 烟包条码
        /// </summary>
        public string itembarcode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }

        /// <summary>
        /// 烟包重量
        /// </summary>
        public string weight { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string orderdetailid { get; set; }
        /// <summary>
        /// 来源单号(如：出库计划单号)
        /// </summary>
        public string ref_orderno { get; set; }

    }

    public class ApplyNewbarcodein {
        /// <summary>
        /// 是否全部打印
        /// </summary>
        public int printall { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemcode { get; set; }
        /// <summary>
        /// 包规
        /// </summary>
        public decimal itemspeci { get; set; }
        /// <summary>
        /// 烟包个数
        /// </summary>
        public int qty { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 出库单项次
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 出库单明细ID
        /// </summary>
        public string orderdetailid { get; set; }
    }
    /// <summary>
    /// 出库货位分配分组报表数据参数
    /// </summary>
    public class OutboundDetailGroupIn {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string service_type { get; set; }
        /// <summary>
        /// 发货仓库id
        /// </summary>
        public string warehouseid { get; set; }
        /// <summary>
        /// 收货仓库id
        /// </summary>
        public string arr_warehouseid { get; set; }
        /// <summary>
        /// 收货工厂id
        /// </summary>
        public string rcvorgid { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string datefrom { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string dateto { get; set; }
        /// <summary>
        /// 来源单号
        /// </summary>
        public string ref_orderno { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string productname { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string username { get; set; }
    }

    public class BarcodeOut {
        /// <summary>
        /// 烟包条码主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 烟包条码
        /// </summary>
        public string itembarcode { get; set; }
        /// <summary>
        /// 烟包重量
        /// </summary>
        public decimal? weight { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemcode { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        public string itemname { get; set; }
        /// <summary>
        /// 容器id
        /// </summary>
        public string containerid { get; set; }
        /// <summary>
        /// State#当前状态( 新增=-1， 待检=0，可用=1，已出库=2)
        /// </summary>
        public int? state { get; set; }
        /// <summary>
        /// 含水率
        /// </summary>
        public decimal? warterrate { get; set; }

    }
    public class SplitDetail {
        /// <summary>
        /// 拆分重量
        /// </summary>
        public decimal? SplitGw { get; set; }
        /// <summary>
        /// 拆分件数
        /// </summary>
        public decimal? SplitQty { get; set; }
    }
    public class BarcodeSplitIn: BarcodeOut
    { 
        /// <summary>
        /// 拆分明细
        /// </summary>
      public List<SplitDetail> lstSplitDetail { get; set; }
    }

    public class MakeNewbarckdeIn {
        /// <summary>
        /// 订单号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 项次
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public decimal? speci { get; set; }
        /// <summary>
        /// 份数
        /// </summary>
        public int copies { get; set; }
    }

    public class MakeNewbarckde2In
    {        
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string orderdetailid { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public decimal? speci { get; set; }
        /// <summary>
        /// 份数
        /// </summary>
        public int copies { get; set; }
    }
    public class RestoreScanedBarcodeIn { 
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OrderNo { get; set; } 
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string OrderDetailId { get; set; }
    }
    /// <summary>
    /// 合包入参
    /// </summary>
    public class CombineCtnIn { 
        /// <summary>
        /// 备料单号
        /// </summary>
        public string PlanNo { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutOrderNo { get; set; }
        /// <summary>
        /// 出库明细ID
        /// </summary>
        public string OutDetailId { get; set; }
    }
}
    