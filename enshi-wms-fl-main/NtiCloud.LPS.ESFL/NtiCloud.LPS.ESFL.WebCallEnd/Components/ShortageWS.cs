using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface ShortageWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        DataSet SelectShortageHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, bool ShowFinished);

        [System.ServiceModel.OperationContract()]
        DataSet SelectShortageDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ShortageID);

        [System.ServiceModel.OperationContract()]
        String CreateReceiving(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, List<string> ShortageIDs);

        [System.ServiceModel.OperationContract()]
        String CreateCross(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, List<string> ShortageIDs);

        [System.ServiceModel.OperationContract()]
        string DeleteShortage(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ShortageIDs);
    }
}
