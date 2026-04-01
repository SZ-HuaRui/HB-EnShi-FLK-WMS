using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IWCSTaskService
    {
 
        /// <summary>
        /// （强制）完成任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("完成任务", "任务ID", "")]
        WebJsonInfo FinishTasks(WorkTaskModel.FinishTasksParam p);
         
        /// <summary>
        /// 申请产生任务
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        [Description("入库申请", "", "返回任务信息")]
        WebJsonInfo ApplyInTask(WorkTaskModel.ApplyTaskInParam applyInfo);  

        /// <summary>
        /// 任务部分完成
        /// <br>针对下架任务，释放原货位</br>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("任务部分完成", "任务ID", "是否成功")]
        WebJsonInfo PartionCompleteTask(WorkTaskModel.FinishTasksParam p);
        /// <summary>
        /// 呼叫空托盘
        /// </summary>
        /// <param name="p">呼叫参数</param>
        /// <returns></returns>
        [Description("呼叫空托盘出库", "", "")]
        WebJsonInfo CalloutEmptyContainer(WorkTaskModel.CallEmptyContainerParam p);


        /// <summary>
        /// 同步设备状态
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("同步设备状态", "EquipmentStatus_OK/EquipmentStatus_Error", "")]
        WebJsonInfo SyncEquipmentStatus(WorkTaskModel.EquipmentParamIn param);

        /// <summary>
        /// 更改巷道
        /// <br>更改入库任务巷道</br>
        /// </summary>
        /// <param name="p">任务单号</param>
        /// <returns></returns>
        [Description("更改巷道:更改入库任务巷道", "任务单号", "新的任务信息")]
        WebJsonInfo ChangeInboundChannel(WorkTaskModel.FinishTasksParam taskInfo);

        WebJsonInfo ScanBarcodeIntoInv(string containercode, string batch);

        /// <summary>
        /// WCS增加SN日志 
        /// </summary>
        /// <param name="logInfo">日志信息</param>
        /// <returns></returns>
        [Description("WCS增加SN日志", "任务单号", "新的任务信息")]
        WebJsonInfo WcsAddSNLog(WorkTaskModel.SNLogIn logInfo);
    }
}
