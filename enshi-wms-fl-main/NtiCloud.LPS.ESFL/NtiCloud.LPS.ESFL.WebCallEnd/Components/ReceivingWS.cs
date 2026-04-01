using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface ReceivingWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        int InsertReceivingHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingNumber, string SchemeID, string SchemeNumber, string SourceBillNumber, string CommonID, string CommonNumber, string Mode, string LotID, string LotNumber, string BOMCode, string ShipperID, string Deliveryman, DateTime ReceivingDate, string UserID, string SourceNumber, string DestinNumber, string DockID, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectReceivingHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceReceivingHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string StateCode);
        [System.ServiceModel.OperationContract()]
        int CountReceivingHead_ByLotNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LotNumber);
        [System.ServiceModel.OperationContract()]
        int UpdateReceivingHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, string SchemeID, string SchemeNumber, string SourceBillNumber, string CommonID, string CommonNumber, string Mode, string LotID, string LotNumber, string BOMCode, string ShipperID, string Deliveryman, DateTime ReceivingDate, string UserID, string DockID, string DestinNumber);
        [System.ServiceModel.OperationContract()]
        int DeleteReceivingHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, string ReceivingNumber);

        [System.ServiceModel.OperationContract()]
        int InsertReceivingDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, string DetailID, string ItemID, decimal SchemeQty, decimal ActualQty, string Memo, string Field1, string Field2, string Supplier, string LotNumber);

        [System.ServiceModel.OperationContract()]
        DataSet SelectReceivingDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID);
        [System.ServiceModel.OperationContract()]
        int UpdateReceivingDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, string DetailID, string ItemID, decimal SchemeQty, decimal ActualQty, string Memo, string LotNumber, string Supplier);
        [System.ServiceModel.OperationContract()]
        int DeleteReceivingDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, string DetailID);


        //TODO:存放到高架库区
        //[System.ServiceModel.OperationContract()]
        //WorkFlowResult Confirm(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, bool bHaveCross, string ZoneCode);

        //TODO:存放到暂存区
        [System.ServiceModel.OperationContract()]
        WorkFlowResult NormalConfirm(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, ContainerEntity ContainerInfo, string ZoneID, bool bHaveCross);

        [System.ServiceModel.OperationContract()]
        bool CheckHaveCrossHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceReceivingBom(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Itemid);

        //TODO:通过配盘收货，根据配盘单号找物料
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceReceivingBomItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Bomid);

        #region 5.0

        [System.ServiceModel.OperationContract()]
        String InsertReceiving(WMSInterface.EntityDefination.ReceivingHead head, List<WMSInterface.EntityDefination.ReceivingDetail> details, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        Int32 UpdateReceiving(String ReceivingID, WMSInterface.EntityDefination.ReceivingHead head, List<WMSInterface.EntityDefination.ReceivingDetail> details, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        Int32 DeleteReceiving(String ReceivingID, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        DataSet GetReceivingInfoByConditions(WMSInterface.Entity50.LoginInfo login, WMSInterface.EntityDefination.ReceivingQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        Collection<WMSInterface.EntityDefination.ReceivingInfo> GetReceivingInfo(WMSInterface.EntityDefination.ReceivingQueryConditionBase Conditions, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        DataSet dsGetReceivingInfo(WMSInterface.Entity50.LoginInfo login, WMSInterface.EntityDefination.ReceivingQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        DataTable GetReceivingHeadInfo(WMSInterface.Entity50.LoginInfo login, string receivingId);

        [System.ServiceModel.OperationContract()]
        DataTable GetReceivingDetail(WMSInterface.Entity50.LoginInfo login, string receivingId);
        
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ConfirmReceiving(WMSInterface.Entity50.LoginInfo login, string ReceivingID, ContainerEntity ContainerInfo, string ZoneID, bool HaveCross);

        [System.ServiceModel.OperationContract()]
        bool ExcuteToNormal(WMSInterface.Entity50.LoginInfo login, string receivingId, string moveNumber, DataTable dt, out string msg);
        
        [System.ServiceModel.OperationContract()]
        DataSet SelectProductBOMDetailByBomId(WMSInterface.Entity50.LoginInfo login, string BomID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectItemInfoByBomId(string warehouseid, string bomid);

        [System.ServiceModel.OperationContract()]
        Int32 ForceFinishReceiving(string WarehouseID, string ReceivingID);
        #endregion

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
