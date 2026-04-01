using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface ILocationAllocator
    {
        string AllocateInventory(string warehouseID, string itemID, string withBarcode);

        string AllocateInventory(InventoryAllocationParam param);

        string AllocateInventory(Dictionary<string, object> inputParams,
            string definedRule = null, string definedParam = null);

        string AllocateLocation(Dictionary<string, object> inputParams,
            string definedRule = null, string definedParam = null);

        string AllocateLocation(string warehouseID,
            string itemID, string batchNo);

        string AllocateLocation(
            string itemID, string batchNo, string packingType, string lastLocationNo);

        string AllocateLocation(string warehouseID, string itemID,
            string typeCode, string workFlowHeadID, string checkState);

        string AllocateLocation(string warehouseID, string itemID,
            string typeCode, string workFlowHeadID, string checkState, 
            string ownerID, string isFullPallet);

        decimal GetTimeWindow(string warehouseID,
            string itemID, string typeCode, string workFlowHeadID);
    }

    public class InventoryAllocationParam
    {
        public string DocHeadID { get; set; }

        public string DocDetailID { get; set; }

        public string WarehouseID { get; set; }

        public string ItemID { get; set; }

        public string BarcodeFlag { get; set; }

        public string BatchNo { get; set; }

        public string LastLocation { get; set; }

        public string PartialFlag { get; set; }

        public string ZoneID { get; set; }

        public string LocationGroupID1 { get; set; }

        public string LocationGroupID2 { get; set; }

        public int SaftyPeriod { get; set; }
    }
}
