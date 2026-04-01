using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IOutboundOrder
    {
        void AllocateInventory(string orderNoOrID);

        void PreallocateInventory(string orderNoOrID);

        void CancelInventoryPreallocation(string orderNoOrID);

        void ConfirmPreallocation(string orderNoOrID, out WebJsonInfo info);

        IOutboundOrder AddOutboundTask(string orderDetailID, string locationNoOrID, decimal qty);

        DataRow GetOrder(string orderNoOrID);

        DataRow GetDetail(string detailID);

        DataRow[] GetDetails(string orderID);

        DataRow[] GetOrderHeads(string orderIds);

        DataRow[] GetBomDetail(string bomId);

        DataRow[] GetDetailNotFinished(string orderID);

        IOutboundOrder UpdateOrderHeadState(string orderIds, string state, string oldState = null);

        IOutboundOrder AddDetailProcessedQty(string detailID, decimal processedQty, decimal? oldQty = null);

        IOutboundOrder TransferOwnership(string orderNoOrID);

        IOutboundOrder CreateShippingOrder(string json);

        IOutboundOrder FinishDetail(string headID, string detailID, decimal qty);

        IOutboundOrder FinishOrderWhenTasksFinishedAndQtyFinished(string orderNoOrID);

        IOutboundOrder ModifyOutboundTask(string taskNoOrID, decimal qty, string locationID);

        IOutboundOrder RemoveOutboundTasks(string taskIds);

        IOutboundOrder RemoveOutboundTaskItems(string taskItemIds);

    }
}
