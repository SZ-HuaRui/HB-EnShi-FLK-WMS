using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IPickingZone
    {
        decimal GetInventory(string itemID);

        decimal HandlePickingZoneAfterAllocation(DataRow waveDetailRow,
            DataRow rackInventoryDetail, string groupNo, decimal leftQty);

        decimal HandlePickingZoneBeforeAllocation(
            DataRow waveDetailRow, string groupNo, decimal leftQty);
    }
}
