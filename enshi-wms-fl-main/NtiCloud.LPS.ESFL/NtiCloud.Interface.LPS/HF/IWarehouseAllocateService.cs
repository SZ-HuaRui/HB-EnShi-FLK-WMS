using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS.HF
{
    [Authorization]    
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    [Description("仓库分配","","")]
    public  interface IWarehouseAllocateService
    {
        /// <summary>
        /// 保存仓库分配明细信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("保存仓库分配明细信息", "", "")]
        WebJsonInfo SaveWarehouseEditInfo(Model.WMS.EditWarhouseIn p);

        /// <summary>
        /// 确认仓库分配信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("确认仓库分配信息", "", "")]
        WebJsonInfo MakesureWarehouseAllocate(Model.WMS.ConfirmWarehouseIn p);
        /// <summary>
        /// 确认仓库分配信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("确认仓库分配信息", "", "")]
        WebJsonInfo MakesureWarehouseAllocate2(string order_no);

        /// <summary>
        /// 取消仓库分配确认
        /// </summary>
        /// <param name="confirminfo"></param>
        /// <returns></returns>
        [Description("取消仓库分配确认", "", "")]
        public WebJsonInfo RollBackWarehouseAllocate(string order_no);
        /// <summary>
        /// 删除出库计划
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        [Description("删除出库计划", "", "")]
        WebJsonInfo DeleteOutboundPlan(string order_no);
        /// <summary>
        /// 删除出库计划明细
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        [Description("删除出库计划明细", "", "")]
        public WebJsonInfo DeleteOutboundPlanDetail(string order_no, decimal? orderdetail_id);
        /// <summary>
        /// 同步配方
        /// </summary>
        /// <param name="ordernos"></param>
        /// <returns></returns>
        [Description("同步配方", "", "")]
        WebJsonInfo SyncPrescription(List<Model.WMS.SyncPrescriptionParam> ordernos);
        
    }
}
