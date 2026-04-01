using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IReceipt
    {
        void AddDetailFinishedQty(string detailID, decimal qty, decimal? oldQty = null);

        void AllocateLocation(string receiptNoOrID);

        void CancelAllocateLocation(string receiptNoOrID);

        void ConfirmAllocation(string receiptNoOrID);

        void AddInboundTask(string receiptID,
            string itemID, decimal qty, string locationID);

        void ModifyInboundTask(string taskID, decimal qty, string locationID);

        void RemoveAllocation(string taskNoOrID);

        void FinishReceivingDoc(string receiptNoOrID);

        void FinishDetail(string docNo, string detailID, decimal qty);

        void FinishReceiptWhenTasksFinished(string docNo);

        DataRow GetHead(string headNoOrID);

        DataRow GetDetail(string detailID);

        DateTime? NormalizeDate(object date);
    }
}
