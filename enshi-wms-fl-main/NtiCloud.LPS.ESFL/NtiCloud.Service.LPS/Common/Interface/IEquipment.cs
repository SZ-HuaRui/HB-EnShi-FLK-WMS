using NtiCloud.Service.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IEquipment
    {
        DataRow GetEquipment(string equipNoOrID);

        DataRow[] GetEquipmentByStatus(string equipType, string equipStatus);

        DataRow GetStackByLocation(string locationNoOrID);

        DataRow[] GetPickingStation();

        bool IsOutboundStation(Address dest);
    }
}
