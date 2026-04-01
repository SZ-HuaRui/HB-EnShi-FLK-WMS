
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface PreOutBoundWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        DataSet GetPreOutBoundLocation(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID);


        [System.ServiceModel.OperationContract()]
        DataSet GetPreOutBoundLocation_SqlQuery(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID, string Sql);
        [System.ServiceModel.OperationContract()]
        DataSet GetPreOutBoundByConditionQuery(string EquipmentNumber, string UserID, string Password, string WarehouseID, string Sql);

        [System.ServiceModel.OperationContract()]
        DataSet GetPreOutBoundByID(string EquipmentNumber, string UserID, string Password, string WarehouseID, string PreOutBoundID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectPreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        int DeletePreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID, string PreOutBoundID);
        [System.ServiceModel.OperationContract()]
        int UpdatePreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID, string PreOutBoundID, int Priority, string Destination);

        [System.ServiceModel.OperationContract()]
        int ChangeItemBarCode(string EquipmentNumber, string UserName, string Password, string WarehouseID, string ContainerDetailID, string BarCode1, string BarCode2);
        [System.ServiceModel.OperationContract()]
        DataSet ShowContainerDetail(string EquipmentNumber, string UserName, string Password, string WarehouseID, string ContainerID);

        [System.ServiceModel.OperationContract()]
        string InsertPreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID, string GroupNumber, string SequenceNumber, int Priority, string ContainerID, string ContainerBarCode, string ContainerTypeID, string SchemeID, string SchemeNumber, string SourceBillNumber, string LotID, string LotNumber, string BomCode, string CommonID, string CommonNumber, string PreOutBoundMode, decimal Qty, string ReceivingLotNumber, string ZoneID, string Destination, string LocationNumber, string LocationID, string DestinNumber, string TargetType,string BOMID,string ModelCode);


        [System.ServiceModel.OperationContract()]
        int ConfirmPreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID, string ZoneID, string PreOutBoundID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectFinishedPreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID);


        [System.ServiceModel.OperationContract()]
        int DeleteFinishedPreOutBound(string EquipmentNumber, string UserName, string Password, string WarehouseID, string PreOutBoundID);

        [System.ServiceModel.OperationContract()]
        DataSet GetPreOutBoundLocation_Heap(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID, string ReceivingLotNumber, Int32 CheckStayTime);

        [System.ServiceModel.OperationContract()]
        DataSet GetAllReceivingLotNumber(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID);

        [System.ServiceModel.OperationContract()]
        bool CheckCanPreOutBound(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID, string LocationID);

        [System.ServiceModel.OperationContract()]
        bool ConfirmCheckOutBound(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID, string LocationID);

        [System.ServiceModel.OperationContract()]
        bool CheckContainerStateOutBound(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ContainerID);

        [System.ServiceModel.OperationContract()]
        bool ChangeReceivingLotNumber(string equipmentNumber, string userID, string pwd, string warehouseID, List<String> locationIds, string receivingLotNumber);
        [System.ServiceModel.OperationContract()]
        bool GetPreOutBoundEquipmentState(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string LocationID);

        #region EmergencyOutBound By Jeffrey
        [System.ServiceModel.OperationContract()]
        DataSet SelectOrderHead(string equipmentNumber, string userId, string userPassword, string warehouseId);

        [System.ServiceModel.OperationContract()]
        DataSet SelectOrderDetail(string equipmentNumber, string userId, string userPassword, string warehouseId, String orderId);
        [System.ServiceModel.OperationContract()]
        int Confirm(string equipmentNumber, string userId, string userPassword, string warehouseId, String detailId, Int32 priority, Decimal qty, String targetAddressId, String destination, String reason, String memo);

        #endregion
        #region EmergencyOutBoundFromBuffer By PC
        [System.ServiceModel.OperationContract()]
        int ConfirmEmergencyOutBoundFromBuffer(string equipmentNumber, string userId, string password, string warehouseId, Int32 priority, Decimal qty, string reason, string memo, string itemID);
        #endregion

        #region Code By Tarrant
        [System.ServiceModel.OperationContract()]
        DataTable GetBOM(string warehouseid, string ContainerID);

        [System.ServiceModel.OperationContract()]
        int UpdateBOMOfContainer(string warehouseid, string ContainerIDString, string BOMID, string ModelCode);

        [System.ServiceModel.OperationContract()]
        DataTable GetOrderHeadN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        Result ConfirmN(LoginInfo loginInfo,List<EmergencyOutBoundN> Details);

        [System.ServiceModel.OperationContract()]
        DataTable GetOrderDetailN(LoginInfo loginInfo, String OrderID);

        [System.ServiceModel.OperationContract()]
        DataTable GetEquipmentForEmergencyOutBoundN(LoginInfo loinInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetOrderDetail(LoginInfo loginInfo, string detailID);
        #endregion

        [System.ServiceModel.OperationContract()]
        bool CheckPreOutBoundEx(string WarehouseID, string ContainerID);

        [System.ServiceModel.OperationContract()]
        WorkFlowResult ConfirmPreOutBoundDetail(string warehouseId, string preOutBoundId, DataTable dtDetail, string user);

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
