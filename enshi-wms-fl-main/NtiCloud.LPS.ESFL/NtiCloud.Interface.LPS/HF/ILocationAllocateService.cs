using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Interface.WMS.HF
{
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    [Description("货位分配","","")]
    public interface ILocationAllocateService
    {
        /// <summary>
        /// 保存出库单信息
        /// </summary>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        [Description("保存出库单信息","","")]
        public WebJsonInfo UpdateOutOrder(Model.WMS.EditOutboundIn outboundInfo );

        /// <summary>
        /// 保存并确认出库单
        /// </summary>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        [Description("保存并确认出库单", "", "")]
        public WebJsonInfo SaveAndConfirmOutbound(Model.WMS.EditOutboundIn outboundInfo);

        /// <summary>
        /// 删除出库单
        /// </summary>
        /// <param name="lstParams"></param>
        /// <returns></删除出库单>
        [Description("删除出库单", "", "")]
        public WebJsonInfo DeleteOutboundOrder(string order_no);

        /// <summary>
        /// 删除出库单明细
        /// </summary>
        /// <param name="lstParams"></param>
        /// <returns></删除出库单>
        [Description("删除出库单明细", "", "")]
        public WebJsonInfo DeleteOutboundOrderDetail(string orderdetailid);
        
        /// <summary>
        /// 取消货位分配
        /// </summary>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        [Description("取消货位分配", "", "")]
        public WebJsonInfo UnconfirmOutboundOrder(string order_no);

        /// <summary>
        /// 确认货位分配
        /// </summary> 
        [Description("确认货位分配", "", "")]
        public WebJsonInfo ConfirmLocationInfo2(Model.WMS.ConfirmLocationIn orderinfo);

        /// <summary>
        /// 出库单拆分：将有码与无码拆分成两个出库单
        /// </summary>
        ///<param name="order_no">出库单号</param> 
        [Description("出库单拆分：将有码与无码拆分成两个出库单", "", "")]
        public WebJsonInfo SplitOutbound(string order_no);

        /// <summary>
        /// 取消出库
        /// </summary>
        ///<param name="order_no">出库单号</param> 
        [Description("取消出库", "", "")]
        public WebJsonInfo UndoOutbound(string order_no);


        /// <summary>
        /// 同步ERP
        /// </summary>
        ///<param name="order_no">出库单号</param> 
        [Description("同步ERP", "", "")]
        public WebJsonInfo SyncData(List<NtiCloud.Model.WMS.SysErpIn> lstOrderno);

        /// <summary>
        /// 撤销同步ERP
        /// </summary>
        ///<param name="order_no">出库单号</param> 
        [Description("撤销同步ERP", "", "")]
        public WebJsonInfo DeleteSyncOrder(List<NtiCloud.Model.WMS.SysErpIn> lstOrderno);
        /// <summary>
        /// 按出库单生成条码
        /// </summary>
        /// <param name="lstOrderno"></param>
        /// <returns></returns>
        [Description("按出库单生成条码", "出库单号", "")]
        public WebJsonInfo MakeNewBoxNumberByOrder(string order_no);

        /// <summary>
        /// 按出库单+项次生成条码
        /// </summary>
        /// <param name="lstOrderno"></param>
        /// <returns></returns>
        [Description("按出库单+项次生成条码", "", "")]        
        public WebJsonInfo MakeNewBoxNumberByOrderItem(Model.WMS.MakeNewbarckdeIn applyInfo);

        /// <summary>
        /// 按出库单明细ID生成条码
        /// </summary>
        /// <param name="lstOrderno"></param>
        /// <returns></returns>
        [Description("按出库单明细ID生成条码", "", "")]
        public WebJsonInfo MakeNewBoxNumberByOrderDetailId(Model.WMS.MakeNewbarckde2In applyInfo);

        /// <summary>
        /// 多笔出库单明细ID批量生成条码
        /// </summary>
        /// <param name="lsyApplyInfo">多笔出库单明细ID批量生成条码</param>
        /// <returns></returns>
        [Description("多笔出库单明细ID批量生成条码", "", "")]
        public WebJsonInfo MakeNewBoxNumberBydetailIds(List<Model.WMS.MakeNewbarckde2In> lsyApplyInfo);


        /// <summary>
        /// 更新出库单打印状态
        /// </summary>
        /// <param name="order_nos"></param>
        /// <returns></returns>
        [Description("更新出库单打印状态", "出库单号：多个用,号分割", "")]
        public WebJsonInfo UpdatePrintStatus(string order_nos);


        /// <summary>
        /// 生成分组打印数据并插入到临时表
        /// </summary>
        /// <param name="searchinfo"></param>
        /// <returns></returns>
        [Description("生成分组打印数据并插入到临时表", "", "打印批次号")]
        public WebJsonInfo GenerateDetailGroupData(Model.WMS.OutboundDetailGroupIn searchinfo);

        /// <summary>
        /// 撤销已扫描数据重新扫描
        /// </summary>
        /// <param name="searchinfo"></param>
        /// <returns></returns>
        [Description("撤销已扫描数据重新扫描", "", "")]
        public WebJsonInfo RestoreScanedBarcode(Model.WMS.RestoreScanedBarcodeIn orderdetail);

      

    }
}
