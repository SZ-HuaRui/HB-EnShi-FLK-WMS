using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface LanewaySelectWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        DataSet GetLanewaySelectSetting(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EquipmentNumber"></param>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <param name="WarehouseID"></param>
        /// <param name="EquipmentID"></param>
        /// <param name="ItemID"></param>     
        /// <returns></returns>
        [System.ServiceModel.OperationContract()]
        int ChangeLanewaySelectSetting(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EquipmentID, string ItemID, string ItemCode, string Field1, string Field2, string Field3, string Field4, string Field5);

        [System.ServiceModel.OperationContract()]
        DataSet GetLaneWayScheme(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        int SetUsedLanewaySelectScheme(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EquipmentID, string ItemID, string ItemCode);


        [System.ServiceModel.OperationContract()]
        int NotUsedLanewaySelectScheme(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EquipmentID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectLaneWayScheme(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet GetLanewaySelectSettingByLineWayId(string equipmentId, string userId, string password, string warehouseId,string field3,string field1,string field2);

    }
}
