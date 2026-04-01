using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface TaskWS : _KeepAlive
    {

        [System.ServiceModel.OperationContract()]
        DataSet SelectTask(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, bool ShowAll);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceTask(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet GetLocationInfoByTaskNumber(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string TaskNumber);

        [System.ServiceModel.OperationContract()]
        String Rollback(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);


        [System.ServiceModel.OperationContract()]
        int UpdateTask_ForceNotProcess(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);
        [System.ServiceModel.OperationContract()]
        int UpdateTask_Force_Pause(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);
        [System.ServiceModel.OperationContract()]
        int UpdateTask_Force_WaitingExecute(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);
        [System.ServiceModel.OperationContract()]
        int UpdateTask_Force_Executing(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);

        [System.ServiceModel.OperationContract()]
        int UpdateTask_Force_FinishedTask(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);

        [System.ServiceModel.OperationContract()]
        DataSet SelectFinishedTask(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, String condition);


        [System.ServiceModel.OperationContract()]

        DataSet SelectFinishedFLTask(string equipmentNumber, string userId, string userPassword, string warehouseId, String condition);

        [System.ServiceModel.OperationContract()]

        DataSet SelectFinishedOtherTask(string equipmentNumber, string userId, string userPassword, string warehouseId, String condition);

        [System.ServiceModel.OperationContract()]
        DateTime GetServerTime();

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        DataTable GetFinishedTaskFL(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetFinishedTaskNotFL(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);
    }
}
