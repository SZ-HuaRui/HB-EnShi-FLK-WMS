using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface PreInBoundWS : _KeepAlive
    {

        [System.ServiceModel.OperationContract()]
        int InsertPreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupNumber, string SequenceNumber, int Priority, string ReceivingID, string ReceivingNumber, string ContainerID, string ContainerBarCode, string ContainerTypeID, string SchemeID, string SchemeNumber, string SourceBillNumber, string CommonID, string CommonNumber, string LotID, string LotNumber, string BOMCode, string ProcessBOMID, string ProcessBOMNumber, Decimal Qty, string ReceivingLotNumber, string[] ContainerDetail_ItemID, string[] ContainerDetail_ItemBarCode1, string[] ContainerDetail_ItemBarCode2, string[] ContainerDetail_ItemPosition, decimal[] ContainerDetail_Qty, string[] ContainerDetail_LotNumber,string ZoneID,string TypeCode,DateTime FirstInTime,System.Collections.Hashtable OtherParameters);

        [System.ServiceModel.OperationContract()]
        string InsertPreInBoundNew(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupNumber, string SequenceNumber, int Priority, string ReceivingID, string ReceivingNumber, string ContainerID, string ContainerBarCode, string ContainerTypeID, string SchemeID, string SchemeNumber, string SourceBillNumber, string CommonID, string CommonNumber, string LotID, string LotNumber, string BOMCode, string ProcessBOMID, string ProcessBOMNumber, Decimal Qty, string ReceivingLotNumber, string[] ContainerDetail_ItemID, string[] ContainerDetail_ItemBarCode1, string[] ContainerDetail_ItemBarCode2, string[] ContainerDetail_ItemPosition, decimal[] ContainerDetail_Qty, string[] ContainerDetail_LotNumber, string ZoneID, string TypeCode, DateTime FirstInTime, System.Collections.Hashtable OtherParameters);


        [System.ServiceModel.OperationContract()]
        int UpdatePreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID, int Priority, string ContainerBarCode);

        [System.ServiceModel.OperationContract()]
        int DeletePreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID, bool DeleteContainerDetail);

        [System.ServiceModel.OperationContract()]
        int DeleteFinishedPreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID);
       
        [System.ServiceModel.OperationContract()]
        DataSet SelectPreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
       
        [System.ServiceModel.OperationContract()]
        DataSet SelectFinishedPreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
       
        [System.ServiceModel.OperationContract()]
        DataSet ShowContainerDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerID);
       
        [System.ServiceModel.OperationContract()]
        int ConfirmPreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID, string WorkFlowHeadID, string ItemID);

        [System.ServiceModel.OperationContract()]
        int ConfirmPreInBoundWithLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID, string WorkFlowHeadID, string ItemID, string locationId);

        [System.ServiceModel.OperationContract()]
        int ChangeItemBarCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerDetailID, string BarCode1, string BarCode2);

        //By PC 2009/10/26
        [System.ServiceModel.OperationContract()]
        DataSet SelectPreInBoundByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlCondition);

        //By PC 2009/11/5
        [System.ServiceModel.OperationContract()]
        DataSet SelectFinishedPreInBoundByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlCondition);

        //[System.ServiceModel.OperationContract()]
        //DataSet GetPreInBoundLocation(string EquipmentNumber, string UserID, string Pwd, string WarehouseID, string ZoneID);

        [System.ServiceModel.OperationContract()]
        DataSet CountNormalInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID,string ItemID);


        [System.ServiceModel.OperationContract()]
        DataSet GetPreInBoundLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);

        [System.ServiceModel.OperationContract()]
        Result InsertPreInBoundN(LoginInfo loginInfo, PreInBoundN entity, List<ContainerDetailN> details);

        [System.ServiceModel.OperationContract()]
        Result UpdatePreInBoundN(LoginInfo loginInfo, PreInBoundN entity, List<ContainerDetailN> details);

        [System.ServiceModel.OperationContract()]
        Result DeletePreInBoundN(LoginInfo loginInfo, PreInBoundN entity);
        
        [System.ServiceModel.OperationContract()]
        DataTable SelectPreInBoundN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        int ConfirmPreInBoundN(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID, string WorkFlowHeadID, string ItemID);

        [System.ServiceModel.OperationContract()]
        int ConfirmPreInBoundWithLocationN(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PreInBoundID, string WorkFlowHeadID, string ItemID, string locationId);


        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
