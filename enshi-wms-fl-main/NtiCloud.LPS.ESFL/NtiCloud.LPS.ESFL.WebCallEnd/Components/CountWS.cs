using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using WMSInterface.EntityDefination;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface CountWS : _KeepAlive
    {


        #region Chioce 
        [System.ServiceModel.OperationContract()]

        System.Collections.ObjectModel.Collection<WMSInterface.EntityDefination.CountHeadData> ChioceCountHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SQLComm);


        [System.ServiceModel.OperationContract()]

        DataSet ChioceCountDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID, string SQLComm);

        [System.ServiceModel.OperationContract()]

        DataSet ChioceCountContainer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string NSQLComm, string OSQLComm);

        [System.ServiceModel.OperationContract()]

        DataSet ChioceContainerDetailServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContianerID);

        [System.ServiceModel.OperationContract()]
        bool ModifyCountYYServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string Reason, string Opstr);

        [System.ServiceModel.OperationContract()]
        int CheckTask(string WarehouseID, bool CountOrQC);

        #endregion





        [System.ServiceModel.OperationContract()]

        string CountNew_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode, string Reason);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult Count_Del_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult Count_Direct_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID, string ContainerID, string Reason);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult Count_OutBound_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID, string ContainerID, string TargetAddressID, string TargetAddressText, string TargetAddressType, string Reason);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult CountDetail_Del_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID);



        [System.ServiceModel.OperationContract()]

        WorkFlowResult Count_Fininshed_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult Count_ConfirmFininshed_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult Count_DelFininshed_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountID);

        [System.ServiceModel.OperationContract()]
        WorkFlowResult ConConninfo_Update_Version(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string ContainerDetailID,  bool tagrr, string Version);
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ConConninfo_TZUpdate_Version(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string ContainerDetailID, bool tagrr, string Version);
        [System.ServiceModel.OperationContract()]

        WorkFlowResult ConDetail_Ins_TZWork(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string itemid, string lotnumber, string ItemBarCode, decimal qty, bool tagrr);

        [System.ServiceModel.OperationContract()]
        WorkFlowResult ConDetail_Del_TZWork(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string ContainerDetailID, string ItemID, bool tagrr);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult ConDetail_Update_TZWork(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string ContainerDetailID, string ItemID, string ItemBarCode, string LotNumber, decimal Qty, bool tagrr);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult ConDetail_Ins_CTZWork(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string itemid, string lotnumber, string ItemBarCode, decimal qty, bool tagrr);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult ConDetail_Del_CTZWork(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string ContainerDetailID, string ItemID, bool tagrr);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult ConDetail_Update_CTZWork(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CountDetailID, string ContainerID, string ContainerDetailID, string ItemID, string ItemBarCode, string LotNumber, decimal Qty, bool tagrr);



        [System.ServiceModel.OperationContract()]

        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        DataSet GetPlanTypeAndState(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet GetItemsOrBomData(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, bool SingleItem);
        [System.ServiceModel.OperationContract()]
        string GetCountPlanNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        bool InsertCountPlanData(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, CountPlanEntity planEntity);
        [System.ServiceModel.OperationContract()]
        DataSet GetPlanDetailData(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string CountPlanID);


        [System.ServiceModel.OperationContract()]
        DataTable GetLocationInfoN(LoginInfo loginInfo, Dictionary<String, String> Params, bool withCountID);
        [System.ServiceModel.OperationContract()]
        DataTable GetCountHeadN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetCountDetailN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetContainerDetailN(LoginInfo loginInfo, Dictionary<String, String> Params);
    }
}
