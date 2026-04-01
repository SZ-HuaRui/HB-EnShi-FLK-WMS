using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface ICrossWS :_KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();


        [System.ServiceModel.OperationContract()]
        DataSet SelectCrossHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        /// <summary>
        /// 通过条件查询越库单。5.0版本，by xsj
        /// </summary>
        [System.ServiceModel.OperationContract()]
        DataSet SelectCrossHeadByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string SqlCondition);

        [System.ServiceModel.OperationContract()]
        int InsertCrossHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Crossnumber, string SourceaddressID, string Sourceaddresstype, string Sourceaddresstext, string SourcebillID, string Sourcebillnumber, string Sourcebilltype, string TargetaddressID, string Targetaddresstype, string Targetaddresstext
            );

        [System.ServiceModel.OperationContract()]
        string InsertCrossHeadAndDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Crossnumber, string SourceaddressID, string Sourceaddresstype, string Sourceaddresstext, string SourcebillID, string Sourcebillnumber, string Sourcebilltype, string TargetaddressID, string Targetaddresstype, string Targetaddresstext,string CrossHeadMode,string ProductItemid, List<WMSInterface.EntityDefination.CrossDetail> DetailList
            );

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceCrossDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string CrossID);

        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceCrossDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);



        [System.ServiceModel.OperationContract()]
        DataSet ChoiceFinishedCrossDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID);

        [System.ServiceModel.OperationContract()]
        int UpdateCrossHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID, string SourceaddressID, string Sourceaddresstype, string Sourceaddresstext, string SourcebillID, string Sourcebillnumber, string Sourcebilltype, string TargetaddressID, string Targetaddresstype, string Targetaddresstext,string CrossHeadMode,string ProductItemid);

        [System.ServiceModel.OperationContract()]
        int UpdateCrossHeadAndDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID, string SourceaddressID, string Sourceaddresstype, string Sourceaddresstext, string SourcebillID, string Sourcebillnumber, string Sourcebilltype, string TargetaddressID, string Targetaddresstype, string Targetaddresstext, string CrossHeadMode, string ProductItemid,List<WMSInterface.EntityDefination.CrossDetail> DetailList);

        [System.ServiceModel.OperationContract()]
        bool CheckCrossState(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID);

        [System.ServiceModel.OperationContract()]
        int DeleteCross(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID);

        [System.ServiceModel.OperationContract()]
        int InsertCrossDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID,string ItemID,string ReceivingID,decimal Qty);

        [System.ServiceModel.OperationContract()]
        int DeleteCrossDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID, string CrossDetailID);

        [System.ServiceModel.OperationContract()]
        int CrossConfirm(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectFinishedCrossHead(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        int DeleteFinishedCross(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID);
        [System.ServiceModel.OperationContract()]
        int UpdateCrossDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CrossID, string CrossDetailid, decimal QTY, string Itemid,string Lotnumber,string Supplier);



        [System.ServiceModel.OperationContract()]
        bool CrossTurnBackToOrder(string equipmentNumber, string userId, string userPassword, string warehouseId, string crossid);
        //add by xyj
        [System.ServiceModel.OperationContract()]
        DataSet GetCrossItemInfoByConditions(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.ITEM.ITEMQueryConditionBase Conditions);




        
    }
}
