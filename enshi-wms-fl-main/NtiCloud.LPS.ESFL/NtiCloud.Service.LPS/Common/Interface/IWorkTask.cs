using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IWorkTask
    {
        List<WorkTaskItemModel> AllocateItems(DataRow[] taskItemRows, DataRow[] inventoryRows);

        WorkTaskModel AddOutboundTask(WorkTaskModel task,
            ContainerDetailModel detailModel, ref decimal leftQty);

        void AddTaskItem(DataRow[] taskRows, WorkTaskItemModel taskItem);

        bool ExistsFinishedTaskInRefDoc(string srcDocNo);

        bool ExistsUnfinishedTaskInRefDoc(string docType, string docNoOrID);

        bool ExistsUnfinishedTaskAtDestAddress(Address addr);

        IWorkTask DeleteWorkTask(string taskNoOrID);

        /// <summary>
        /// 根据货位及库存确定任务路径目的
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        RoutePurpose DetermineRoutePurpose(DataRow locRow, DataRow[] inventoryRows);

        DataRow GetMainDockByTasks(string docNo);

        DataRow Get(string taskNoOrID);

        TaskHeaderModel GetTaskHeader(string taskid);

        DataRow[] GetByEquipmentID(string equipIds);

        DataRow[] GetByOrderNoOrID(string orderNoOrID);

        DataRow[] GetDependentTasks(DataRow task);

        DataRow[] GetTaskItems(string taskItemIds);

        DataRow[] GetTaskItemsBy(string taskID, string actStatus = "1",bool errorWhenNotFound = true);
        
        DataRow GetWorkTask(string taskNoOrID);

        DataRow[] GetWorkTasksWithProcess(string taskNoOrIds);

        DataRow[] GetWorkTasks(string taskIds);

        DataRow[] GetBy(string refDocID, string srcAddrType, string srcAddrID);

        DataRow[] GetByContainer(string containerBarcode, string taskType = "N/A");

        DataRow[] GetByRefDocDetailID(string refDocDetailID);

        DataRow[] GetByRefDocNoOrID(string refDocType, string docNo);

        DataRow[] GetByRefDocID(string docID);

        DataRow[] GetBySrcAddr(string srcAddrType, string srcAddrID);
        DataTable GetTableByRefDocID(string docID);

        IWorkTask InsertTask(DataRow waveTaskRow, string id = null);

        IWorkTask Insert(WorkTaskModel model);

        IWorkTask InsertFinishedTask(DataRow row);
        /// <summary>
        /// 根据任务实体类，插入任务历史表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IWorkTask InsertFinishedTask(WorkTaskModel model);

        IWorkTask UpdateActStatus(string id, string actStatus, string oldActStatus = null);

        IWorkTask InsertHistory(string taskId);

        IWorkTask TaskItemInsertHistory(string taskId);

        IWorkTask DeleteWorkTaskItem(string taskitemid);

        /// <summary>
        /// 修改任务物料表中的软删除标记
        /// </summary>
        /// <param name="id">任务物料表t_id</param>
        /// <param name="actStatus">软删除标记</param>
        /// <returns></returns>
        IWorkTask UpdateTaskItemActStatus(string id, string actStatus);

        IWorkTask UpdateTaskAndItemActStatus(string taskID, string status);

        IWorkTask ForceTaskExecuting(string warehouseID, string taskID);

        bool ForceTaskExecuting( string taskID);

        IWorkTask ForceTaskFinished(string warehouseID, string taskID, decimal? qty = null);

        IWorkTask FinishWorkTasks(string taskNoOrIds);

        IWorkTask FinishOneTask(string taskNoOrID, decimal qty);

        IWorkTask RecycleTasks(string taskNoOrIds);

        IWorkTask RecycleTasks(DataRow[] taskRows);

        IWorkTask RecycleTask(DataRow taskRow);

        bool IsFinished(DataRow taskRow);

        bool IsFinished(DataRow[] taskRows);

        bool RefDocIsStarted(string srcDocType, string srcDocNo);

        bool UpdateTaskComplete(string taskid);

        /// <summary>
        /// 将完成的任务插入历史表（包括任务物料明细）
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        IWorkTask MoveToHistory(string taskId);
        /// <summary>
        /// 根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
        /// </summary>
        /// <param name="taskid">任务物料id</param>
        /// <returns></returns>
        bool UpdateTaskItemCompleteAndQty(string taskid);

        IWorkTask RemoveWaveTasks(string taskNoOrIds);

        IWorkTask RemoveWaveTasks(DataRow[] taskRows);

        IWorkTask RemoveTaskItem(DataRow taskItemRow);

        IWorkTask RevertWaveTypeTask(DataRow taskRow);

        decimal SaveTask(DataRow[] route, WorkTaskModel workTask, List<WorkTaskItemModel> taskItemList);

        decimal SaveTask(RoutePurpose purpose, WorkTaskModel task, List<WorkTaskItemModel> taskItemList);

        IWorkTask StartTask(string taskID);

        IWorkTask StartTask(DataRow taskRow);

        IWorkTask UpdateContainerDetailID(string id, string containerDetailID, string oldContainerDetailID = null);

        IWorkTask UpdateContainerBarcode(string id, string containerBarcode, string oldContainerBarcode = null);

        IWorkTask UpdateOperationQty(string id, decimal operationQty, decimal? oldOperationQty = null);

        IWorkTask UpdateFinanceQty(string id, decimal financeQty, decimal? oldFinanceQty = null);

        IWorkTask UpdateStorageZoneID(string id, string storageZoneID, string oldStorageZoneID = null);

        IWorkTask UpdateSource(string taskID, string sourceType, string sourceID, string sourceText);

        IWorkTask UpdateTarget(string taskID, string targetType, string targetID, string targetText);

        IWorkTask UpdateIsExecutable(string id, string isExecutable, string oldIsExecutable = null);

        IWorkTask UpdateTaskItemStatus(string id, string status, string oldStatus = null);
        /// <summary>
        /// 更新任务明细是否有效状态
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="actstatus">有效标识:1有效，0无效</param>
        /// <returns></returns>
        IWorkTask UpdateTaskItemActStatusByTask(string taskId, string actstatus);

        IWorkTask UpdateTaskStatus(string id, string taskStatus, string oldTaskStatus = null);
    }
}
