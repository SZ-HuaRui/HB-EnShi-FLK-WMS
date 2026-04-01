using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class OutboundModel
    {
        public class AddDetailByBomParam
        {
            public string OrderNoOrID { get; set; }
            public string BomID { get; set; }
            public decimal Times { get; set; }
        }
        public class PickOutParam
        {
            public string OrderID { get; set; }
            public string ItemID { get; set; }
            public string LocationNo { get; set; }
            public string ContainerBarcode { get; set; }
            public decimal Qty { get; set; }
        }

        public class PreallocateInventoryParam
        {
            public string OrderNoOrID { get; set; }
        }
        public class CancelInventoryPreallocationParam
        {
            public string OrderNoOrID { get; set; }
        }
        public class ConfirmPreallocationParam
        {
            public string OrderNoOrID { get; set; }
            public string ExecutionMode { get; set; }
            public string PreparationArea { get; set; }
        }
        public class CancelConfirmPreallocationParam
        {
            public string OrderNoOrID { get; set; }
        }
        public class CloseOrderParam
        {
            public string OrderNoOrID { get; set; }
        }
        public class CreateWaveParam
        {
            public string WaveNo { get; set; }
            public string OrderIds { get; set; }
        }
        public class AppendWaveParam
        {
            public string WaveHeadID { get; set; }
            public string OrderIds { get; set; }
        }
        public class SplitWaveDetailParam
        {
            public string GroupNo { get; set; }
            public decimal Num { get; set; }
            public string DetailIds { get; set; }
        }
        public class MergeWaveDetailParam
        {
            public string GroupNo { get; set; }
            public string DetailIds { get; set; }
        }
        public class AddOutboundTaskParam
        {
            public string OrderDetailID { get; set; }
            public string LocationNoOrID { get; set; }
            public decimal Qty { get; set; }
        }
        public class ModifyOutboundTaskParam
        {
            public string TaskNoOrID { get; set; }
            public decimal Qty { get; set; }
            public string LocationID { get; set; }
        }
        public class ModifyOutboundTaskItemParam
        {
            public string TaskItemID { get; set; }
            public decimal Qty { get; set; }
            public string LocationID { get; set; }
        }
        public class ModifyWaveTaskParam
        {
            public string TaskNoOrID { get; set; }
            public decimal Qty { get; set; }
            public string LocationID { get; set; }
        }
        public class ComputeWaveParam
        {
            public string WaveHeadNoOrID { get; set; }
            public string Type { get; set; }
        }
        public class ComputeWaveDetailParam
        {
            public string DetailID { get; set; }
            public string Room { get; set; }
        }
        public class PrecomputeWaveParam
        {
            public string WaveHeadNoOrID { get; set; }
            public string Target { get; set; }
            public string LocationGroup { get; set; }
        }
        public class ReleaseWaveParam
        {
            public string WaveHeadNoOrID { get; set; }
            public string StockUpLocID { get; set; }
        }
        public class CancelReleaseParam
        {
            public string WaveHeadNoOrID { get; set; }
        }
        public class DeleteWaveParam
        {
            public string WaveHeadNoOrID { get; set; }
        }
        public class FinishWaveParam
        {
            public string WaveHeadNoOrID { get; set; }
        }
        public class RemoveWaveParam
        {
            public string WaveHeadNoOrID { get; set; }
        }
        public class RemoveWaveDetailsParam
        {
            public string DetailIds { get; set; }
        }
        public class RemoveWaveTasksParam
        {
            public string WaveTaskIds { get; set; }
        }
        public class RemoveOutboundTasksParam
        {
            public string TaskIds { get; set; }
        }
        public class RemoveOutboundTaskItemsParam
        {
            public string TaskItemIds { get; set; }
        }
        public class CancelPrellocationParam
        {
            public string WaveHeadNoOrID { get; set; }
        }
        public class SaveBarcodeNumberParam
        {
            public string WaveNo { get; set; }
            public string JsonItemList { get; set; }
        }
        public class AddWaveTaskParam
        {
            public string TaskNo { get; set; }
            public string GroupNo { get; set; }
            public string SeqNo { get; set; }
            public string Priority { get; set; }
            public string WaveDetailID { get; set; }
            public decimal Qty { get; set; }
            public string LocationNoOrID { get; set; }
        }
        public class TransferOwnershipParam
        {
            public string OrderNoOrID { get; set; }
        }
        public class CreateShippingOrderParam
        {
            public string Json { get; set; }
        }
    }
}
