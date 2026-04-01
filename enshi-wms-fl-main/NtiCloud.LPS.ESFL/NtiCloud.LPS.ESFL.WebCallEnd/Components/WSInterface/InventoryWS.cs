using System;
using System.Collections.Generic;
using System.Text;
using WMSInterface.EntityDefination;
using System.Collections.ObjectModel;
using System.Data;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface InventoryWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        String InventoryHistory_Create(String equipmentNumber, String userId, String userPassword, String warehouseId, InventoryHistoryHead_Edit entity);
        [System.ServiceModel.OperationContract()]
        String InventoryHistory_Update(String equipmentNumber, String userId, String userPassword, String warehouseId, String historyId, InventoryHistoryHead_Edit entity);
        [System.ServiceModel.OperationContract()]
        String InventoryHistory_Delete(String equipmentNumber, String userId, String userPassword, String warehouseId, String historyId);
        [System.ServiceModel.OperationContract()]
        Collection<InventoryHistoryHead> InventoryHistory_SelectHead(String equipmentNumber, String userId, String userPassword, String warehouseId);
        [System.ServiceModel.OperationContract()]
        Collection<InventoryHistoryDetail> InventoryHistory_SelectDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String historyId);
        [System.ServiceModel.OperationContract()]
        String InventoryHistory_EditDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String historyDetailId, InventoryHistoryDetail_Edit entity);

        [System.ServiceModel.OperationContract()]
        System.Data.DataTable InventoryWarning(string WarehouseID);     


        [System.ServiceModel.OperationContract()]
        DataTable GetInventoryInfo(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetItemSpecInventory(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLotInventory(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLocations(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetZone(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLocaitonCount(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable InventoryWarning_FL(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable TimeOutInventoryView(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable UnSafeInventoryView(LoginInfo loginInfo, Dictionary<String, String> Params, bool low);

        //by add liucui
        [System.ServiceModel.OperationContract()]
        System.Data.DataTable InventoryAlarm(string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataTable UnSafeInventoryAlarmView(LoginInfo loginInfo, Dictionary<String, String> Params, bool low);
        //by add liucui
       
    
    }
}
