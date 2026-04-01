using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NtiCloud.Interface.WMS
{
    /// <summary>
    /// WMS库-工作任务相关服务
    /// </summary>
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IWorkTaskService 
    {
        /// <summary>
        /// 回收任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("回收任务", "任务ID", "")]
        WebJsonInfo RecycleTask(WorkTaskModel.RecycleTaskParam p);

        /// <summary>
        /// （强制）完成任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("完成任务", "任务ID", "")]
        WebJsonInfo FinishTasks(WorkTaskModel.FinishTasksParam p);

        /// <summary>
        /// （强制）完成任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo FinishOneTask(WorkTaskModel.FinishOneTaskParam p);

        /// <summary>
        /// 申请产生任务
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        [Description("入库申请", "", "返回任务信息")]
        WebJsonInfo ApplyInTask(WorkTaskModel.ApplyTaskInParam applyInfo);
        /// <summary>
        /// 任务强制执行中
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("任务执行中", "", "")]
        WebJsonInfo ForceTaskExcuting(WorkTaskModel.FinishTasksParam p);

        /// <summary>
        /// 强制未执行
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("强制未执行", "", "")]
        WebJsonInfo ForceTaskNotExcute(WorkTaskModel.FinishTasksParam p);

        /// <summary>
        /// 任务部分完成
        /// <br>针对下架任务，释放原货位</br>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("任务部分完成","任务ID","是否成功")]
        WebJsonInfo PartionCompleteTask(WorkTaskModel.FinishTasksParam p);
        /// <summary>
        /// 呼叫空托盘
        /// </summary>
        /// <param name="p">呼叫参数</param>
        /// <returns></returns>
        [Description("呼叫空托盘出库", "", "")]
        WebJsonInfo CalloutEmptyContainer(WorkTaskModel.CallEmptyContainerParam p);
        
        /// <summary>
        /// 替换物料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("替换物料", "", "")]
        WebJsonInfo ReplaceItems(WorkTaskModel.ReplaceItemsParam p);

        /// <summary>
        /// 取消替换物料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("取消替换物料", "", "")]
        WebJsonInfo CancelItemReplacement(WorkTaskModel.CancelItemReplacementParam p);

        /// <summary>
        /// 确认替换物料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("确认替换物料", "", "")]
        WebJsonInfo ConfirmItemReplacement(WorkTaskModel.ConfirmItemReplacementParam p);
        /// <summary>
        /// 增加替换物料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("增加替换物料", "", "")]
        WebJsonInfo AddItemReplacement(WorkTaskModel.AddItemReplacementParam p);
        /// <summary>
        /// 修改替换物料货位
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("修改替换物料货位", "", "")]
        WebJsonInfo ModifyReplacement(WorkTaskModel.ModifyReplacementParam p);

        /// <summary>
        /// 启动替换 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("启动替换", "", "")]
        WebJsonInfo StartReplace(WorkTaskModel.StartReplaceParam p);
        /// <summary>
        /// 确认入库，替换完成
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("确认入库，替换完成", "", "")]
        WebJsonInfo FinishReplace(WorkTaskModel.FinishReplaceParam p);

        ///// <summary>
        ///// 货位二次分配
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //[Description("货位二次分配", "", "")]
        //WebJsonInfo SecondDistributionLocation(WorkTaskModel.SecondDistribuLocRequest p);

        /// <summary>
        /// 入库货位二次分配
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("入库货位二次分配", "", "")]
        WebJsonInfo InBoundSecondDistributionLocation(WorkTaskModel.SecondDistribuLocRequest p);

        /// <summary>
        /// 出库货位二次分配
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [Description("出库货位二次分配", "", "")]
        WebJsonInfo OutBoundSecondDistributionLocation(WorkTaskModel.SecondDistribuLocRequest p);
        /// <summary>
        /// 手工下发(重发)任务给WCS
        /// </summary>
        /// <param name="taskids"></param>
        /// <returns></returns>
        [Description("手工下发(重发)任务给WCS", "任务ID：多个用,号分割", "")]
        WebJsonInfo SendTaskToWcs(string taskids);


    }
}
 