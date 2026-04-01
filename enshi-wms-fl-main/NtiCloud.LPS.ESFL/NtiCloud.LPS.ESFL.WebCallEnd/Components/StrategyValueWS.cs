using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface StrategyValueWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        string ReadStrategy(string EquipmentNumber, string UserName, string Password, string WarehouseID, string Key, string Default);
        [System.ServiceModel.OperationContract()]
        void WriteStrategy(string EquipmentNumber, string UserName, string Password, string WarehouseID, string Key, string Value);
        [System.ServiceModel.OperationContract()]
        void DeleteStrategy(string EquipmentNumber, string UserName, string Password, string WarehouseID, string Key);

        [System.ServiceModel.OperationContract()]
        DataSet SelectRegionSelect(string EquipmentNumber, string UserName, string Password, string WarehouseID, string FormName, string TypeCode);
        [System.ServiceModel.OperationContract()]
        string GetRegionSelect(string EquipmentNumber, string UserName, string Password, string WarehouseID, string Name, string FormName, string TypeCode);
        [System.ServiceModel.OperationContract()]
        int InsertRegionSelect(string EquipmentNumber, string UserName, string Password, string WarehouseID, string Name, string FormName, string TypeCode, string Value);
        [System.ServiceModel.OperationContract()]
        int DeleteRegionSelect(string EquipmentNumber, string UserName, string Password, string WarehouseID, string Name, string FormName, string TypeCode);
   
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
