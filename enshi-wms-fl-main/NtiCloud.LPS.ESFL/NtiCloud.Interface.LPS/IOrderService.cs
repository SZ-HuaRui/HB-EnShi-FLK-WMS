using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;

namespace NtiCloud.Interface.WMS
{
    /// <summary>
    /// 出库订单服务
    /// </summary>
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IOrderService
    {
        /// <summary>
        /// 通过配方单创建单据明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description(description: "通过配方单创建单据明细", inputDescription: "", outputDescription: "")]
        WebJsonInfo AddDetailByBom(OutboundModel.AddDetailByBomParam p);
        /// <summary>
        ///  捡出
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="itemID"></param>
        /// <param name="locationNo"></param>
        /// <param name="containerBarcode"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        [Description(description: "捡出",
        inputDescription: "",
            outputDescription: "")]
        WebJsonInfo PickOut(OutboundModel.PickOutParam p);
        /// <summary>
        /// 分配出库库存
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="appCode"></param>
        /// <param name="clientID"></param>
        /// <param name="orderNoOrID"></param>
        /// <returns></returns>>().
        //[Authorization(AuthType = AuthorizationType.JWT)]
        //WebJsonInfo AllocateInventory(OutboundModel.AllocateInventoryParam p);
        /// <summary>
        /// 预分配出库库存
        /// </summary>
        /// <param name="orderNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo PreallocateInventory(OutboundModel.PreallocateInventoryParam p);

        /// <summary>
        /// 取消订单库存预分配
        /// </summary>
        /// <param name="orderNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo CancelInventoryPreallocation(OutboundModel.CancelInventoryPreallocationParam p);

        /// <summary>
        /// 确认订单库存预分配
        /// </summary>
        /// <param name="orderNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo ConfirmPreallocation(OutboundModel.ConfirmPreallocationParam p);

        /// <summary>
        /// 取消确认
        /// </summary>
        /// <param name="orderNoOrID"></param>
        /// <returns></returns>
        [Description(description: "取消预分配确认",
        inputDescription: "订单号或订单ID",
            outputDescription: "错误信息")]
        WebJsonInfo CancelConfirmPreallocation(OutboundModel.CancelConfirmPreallocationParam p);

        /// <summary>
        /// 创建波次
        /// </summary>
        /// <param name="waveNo"></param>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        WebJsonInfo CreateWave(OutboundModel.CreateWaveParam p);

        /// <summary>
        /// 追加波次
        /// </summary>
        /// <param name="waveHeadID"></param>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        WebJsonInfo AppendWave(OutboundModel.AppendWaveParam p);

        /// <summary>
        /// 将指定的波次明细拆分为新的一组
        /// </summary>
        /// <param name="groupNo"></param>
        /// <param name="num"></param>
        /// <param name="detailIds"></param>
        /// <returns></returns>
        WebJsonInfo SplitWaveDetail(OutboundModel.SplitWaveDetailParam p);

        /// <summary>
        /// 将指定的波次明细合并到指定组
        /// </summary>
        /// <param name="groupNo"></param>
        /// <param name="detailIds"></param>
        /// <returns></returns>
        WebJsonInfo MergeWaveDetail(OutboundModel.MergeWaveDetailParam p);

        /// <summary>
        /// 手工预分配出库任务
        /// </summary>
        /// <param name="orderDetailID"></param>
        /// <param name="locationNoOrID"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        WebJsonInfo AddOutboundTask(OutboundModel.AddOutboundTaskParam p);

        /// <summary>
        /// 修改出库任务
        /// </summary>
        /// <param name="taskNoOrID"></param>
        /// <param name="qty"></param>
        /// <param name="locationID"></param>
        /// <returns></returns>
        WebJsonInfo ModifyOutboundTask(OutboundModel.ModifyOutboundTaskParam p);

        /// <summary>
        /// 修改从出库单生成的任务明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo ModifyOutboundTaskItem(OutboundModel.ModifyOutboundTaskItemParam p);

        /// <summary>
        /// 关闭出库订单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo CloseOrder(OutboundModel.CloseOrderParam p);

        /// <summary>
        /// 修改波次任务
        /// </summary>
        /// <param name="taskNoOrID"></param>
        /// <param name="qty"></param>
        /// <param name="locationID"></param>
        /// <returns></returns>
        WebJsonInfo ModifyWaveTask(OutboundModel.ModifyWaveTaskParam p);

        /// <summary>
        /// 波次计算
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <param name="type">1-删除波次任务后进行波次运算 2-不删除波次任务，对增量进行波次运算</param>
        /// <returns></returns>
        WebJsonInfo ComputeWave(OutboundModel.ComputeWaveParam p);

        /// <summary>
        /// 对波次明细按仓间预分配货位
        /// </summary>
        /// <param name="detailID"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        WebJsonInfo ComputeWaveDetail(OutboundModel.ComputeWaveDetailParam p);

        /// <summary>
        /// 预计算波次
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <param name="target"></param>
        /// <param name="locationGroup"></param>
        /// <returns></returns>
        WebJsonInfo PrecomputeWave(OutboundModel.PrecomputeWaveParam p);

        /// <summary>
        /// 释放波次 
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <param name="stockUpLocID"></param>
        /// <returns></returns>
        //WebJsonInfo ReleaseWave(OutboundModel.ReleaseWaveParam p);
        WebJsonInfo ReleaseWave(OutboundModel.ReleaseWaveParam p);

        /// <summary>
        /// 取消释放波次
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo CancelRelease(OutboundModel.CancelReleaseParam p);

        /// <summary>
        /// 仅删除波次
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo DeleteWave(OutboundModel.DeleteWaveParam p);

        /// <summary>
        /// 完成波次
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo FinishWave(OutboundModel.FinishWaveParam p);

        /// <summary>
        /// 删除波次，从删除波次任务到删除波次的一条龙服务
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo RemoveWave(OutboundModel.RemoveWaveParam p);

        /// <summary>
        /// 删除波次明细
        /// </summary>
        /// <param name="detailIds">多个ID用逗号分割</param>
        /// <returns></returns>
        WebJsonInfo RemoveWaveDetails(OutboundModel.RemoveWaveDetailsParam p);

        /// <summary>
        /// 删除波次任务
        /// </summary>
        /// <param name="waveTaskIds">多个ID用逗号分割</param>
        /// <returns></returns>
        WebJsonInfo RemoveWaveTasks(OutboundModel.RemoveWaveTasksParam p);

        /// <summary>
        /// 删除出库任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo RemoveOutboundTasks(OutboundModel.RemoveOutboundTasksParam p);

        /// <summary>
        /// 删除出库任务明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo RemoveOutboundTaskItems(OutboundModel.RemoveOutboundTaskItemsParam p);

        /// <summary>
        /// 取消预分配
        /// </summary>
        /// <param name="waveHeadNoOrID"></param>
        WebJsonInfo CancelPrellocation(OutboundModel.CancelPrellocationParam p);

        /// <summary>
        /// 保存码段
        /// </summary>
        /// <param name="waveNo"></param>
        /// <param name="jsonItemList"></param>
        /// <returns></returns>
        WebJsonInfo SaveBarcodeNumber(OutboundModel.SaveBarcodeNumberParam p);

        /// <summary>
        /// 人工指定货位并创建波次任务
        /// </summary>
        /// <param name="taskNo"></param>
        /// <param name="groupNo"></param>
        /// <param name="seqNo"></param>
        /// <param name="priority"></param>
        /// <param name="waveDetailID"></param>
        /// <param name="qty"></param>
        /// <param name="locationNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo AddWaveTask(OutboundModel.AddWaveTaskParam p);

        /// <summary>
        /// 货权转移
        /// </summary>
        /// <param name="orderNoOrID"></param>
        /// <returns></returns>
        WebJsonInfo TransferOwnership(OutboundModel.TransferOwnershipParam p);

        /// <summary>
        /// 创建提货单
        /// </summary>
        /// <param name="json">DataSet结构的参数字符串</param>
        /// <returns></returns>
        WebJsonInfo CreateShippingOrder(OutboundModel.CreateShippingOrderParam p);

        /// <summary>
        /// 出库测试
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        WebJsonInfo OutboundTest(OutboundModel.CreateShippingOrderParam p);

    }
}
