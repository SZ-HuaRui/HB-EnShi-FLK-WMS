using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface LocationWS : _KeepAlive
    {
        #region Rule
        [System.ServiceModel.OperationContract()]
        int InsertRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RuleName, string RuleScript, string Description, int SequenceNumber, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        int UpdateRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RuleID, string RuleName, string RuleScript, string Description, int SequenceNumber);
        [System.ServiceModel.OperationContract()]
        DataSet SelectRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int DeleteRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RuleID);
        [System.ServiceModel.OperationContract()]
        DataSet GetAllSP();

        #endregion

        #region LocationZone
        [System.ServiceModel.OperationContract()]
        int InsertLocationZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID, int ZoneSort, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectLocationZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID);
        [System.ServiceModel.OperationContract()]
        int DeleteLocationZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID);
        #endregion

        #region LocationZoneSQL
        [System.ServiceModel.OperationContract()]
        int InsertLocationZoneSQL(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID, string SQL, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectLocationZoneSQL(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID);
        [System.ServiceModel.OperationContract()]
        int DeleteLocationZoneSQL(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID);
        #endregion

        #region LocationZoneSQLRule
        [System.ServiceModel.OperationContract()]
        int InsertLocationZoneSQLRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID, string RuleID,string RuleScript,Int32 SequenceNumber, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectLocationZoneSQLRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID);
        [System.ServiceModel.OperationContract()]
        int DeleteLocationZoneSQLRule(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, string ZoneID, string RuleID);
        #endregion

        #region LocationRules
        [System.ServiceModel.OperationContract()]
        int InsertLocationRules(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, bool IsStoreInEveryZone, string WorkFlowHeadID, string TypeCode, string ItemID, string Field1, string Field2, string Field3, string Field4, string Field5, Boolean TimeSortEnabled, Int32 TimeSortSpan);
        [System.ServiceModel.OperationContract()]
        int UpdateLocationRules(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID, bool IsStoreInEveryZone, string WorkFlowHeadID, string TypeCode, string ItemID, Boolean TimeSortEnabled, Int32 TimeSortSpan);
        [System.ServiceModel.OperationContract()]
        DataSet SelectLocationRules(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode);
        [System.ServiceModel.OperationContract()]
        int DeleteLocationRules(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LocationRulesID);
        #endregion
         

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
