using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IWave
    {
        IWave AddDetailFinishedQty(string detailID, decimal qty, decimal? oldQty = null);

        IWave AddDetailProcessedQty(string detailID, decimal qty, decimal? oldQty = null);

        IWave AddWaveTask(string taskNo, string groupNo, string seqNo,
            string priority, string waveDetailID, decimal qty, string locationNoOrID);

        decimal AddWaveTask(WorkTaskModel task, decimal leftQty);

        List<WorkTaskItemModel> AllocateItems(DataRow[] detailRows, DataRow[] inventoryRows);

        IWave CancelRelease(string waveHeadNoOrID);

        IWave CreateWave(string waveNo, string orderIds);

        IWave AppendWave(string waveHeadID, string orderIds);

        IWave SplitWaveDetail(string groupNo, decimal num, string detailIds);

        IWave MergeWaveDetail(string groupNo, string detailIds);

        IWave ModifyWaveTask(string taskNoOrID, decimal qty, string locationID);

        IWave CancelPreallocation(string waveHeadNoOrID);

        IWave ComputeWave(string headNoOrID, string type);

        IWave ComputeWaveDetail(string detailID, string room);

        IWave PrecomputeWave(string headNoOrID, string target, string locationGroup);

        IWave ReleaseWave(string headNoOrID, string stockUpLocID, out WebJsonInfo info);

        IWave DeleteWave(string waveHeadNoOrID);

        IWave FinishDetail(string detailID, decimal qty);

        IWave FinishDetail(DataRow detailRow, decimal qty);

        IWave FinishWave(string waveHeadNoOrID);

        IWave FinishWaveWhenTasksFinished(string waveNoOrID);

        IWave RemoveWave(string waveHeadNoOrID);

        IWave RemoveWaveDetails(string detailIds);

        IWave RemoveWaveTasks(string taskIds);

        IWave SaveBarcodeNumber(string waveNo, string jsonItemList);

        decimal GenTasksForOutbound(DataRow[] detailRows, DataRow detailRow, 
            //Address dest, DataRow locRow, ref decimal seqNo, decimal lot = 0);
            Address dest, DataRow locRow, ref decimal seqNo);

        DataRow GetWaveDetail(string detailID);

        DataRow[] GetWaveDetails(string detailIds);

        DataRow[] GetDetailsByOrderDetailID(string outboundDetailID);

        DataRow[] GetDetailsByOrderID(string outboundHeadID);

        DataRow GetHeadByOrderID(string outboundHeadID);

        DataRow GetWaveHead(string waveNoOrID);

        DataRow GetWaveTask(string waveTaskNoOrID);

        DataRow[] GetWaveTasksByHeadID(string headID);

        IWave UpdateWaveTaskState(string waveTaskNo, string state);

        IWave AddHeadLeftTaskQty(string headNoOrID, decimal qty, decimal? oldQty = null);

        IWave UpdateHeadStatus(string headNoOrID, string state, string oldState = null);

        IWave UpdateWaveType(string headNoOrID, string waveType, string oldType = null);

        IWave InsertWaveTask(DataRow waveDetailRow, string taskType,
            string locationID, string groupNo, decimal planQty);

        IWave InsertWaveTask(DataRow waveDetailRow, string taskType,
            string locationID, string groupNo, decimal planQty, string taskNo);

        IWave InsertWaveTask(WaveTaskModel model);

        decimal ComputeOutQty(decimal availQty, decimal leftQty);

        string ComputeTaskType(decimal availQty, decimal outQty);

        IWave RestoreEditableStatus(string waveHeadID);

        IWave ResetWave(string waveHeadID);
    }
}
