using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using WMSInterface.EntityDefination;
using System.Collections.ObjectModel;
using WMSInterface.Entity50;
using NTIVirtualMachine.Component.Attributes;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface InitializeWS : _KeepAlive
    {
        #region Warehouse
        [System.ServiceModel.OperationContract()]
        string InsertWarehouse(string EquipmentNumber, string UserName, string UserPassword, string Code, string Name, string Address, string Postcode, String TypeCode, string SequenceNumber, string WarehouseKey, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceWarehouse();
        [System.ServiceModel.OperationContract()]
        DataSet SelectWarehouse(string EquipmentNumber, string UserName, string UserPassword, string MyWarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectWarehouseByID(string EquipmentNumber, string UserName, string UserPassword, string MyWarehouseID);

        [System.ServiceModel.OperationContract()]
        int UpdateWarehouse(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Code, string Name, string Address, string Postcode, String TypeCode, string SequenceNumber, string WarehouseKey);
        [System.ServiceModel.OperationContract()]
        int DeleteWarehouse(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        #endregion

        #region Dock
        [System.ServiceModel.OperationContract()]
        int InsertDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Number, string Type, decimal Length, decimal Width, decimal Height, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode);
        [System.ServiceModel.OperationContract()]
        DataSet SelectDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectDock_WithDescription(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string DockID, string Number, string Type, decimal Length, decimal Width, decimal Height);
        [System.ServiceModel.OperationContract()]
        int DeleteDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string DockID);

        [System.ServiceModel.OperationContract()]
        Result InsertDockN( LoginInfo loginInfo,DockN entity);

        [System.ServiceModel.OperationContract()]
        Result DeleteDockN(LoginInfo loginInfo, DockN entity);

        [System.ServiceModel.OperationContract()]
        Result UpdateDockN(LoginInfo loginInfo, DockN entity);

        [System.ServiceModel.OperationContract()]
        DataTable SelectDockN(LoginInfo loginInfo, Dictionary<String,String> Params);

        #endregion

        #region Type
        [System.ServiceModel.OperationContract()]
        int InsertType(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code, string Description, string Flag, string DefaultDescription, string Field1, string Field2, string Field3, string Field4, string Field5, string SequenceNumber, string DefaultSequenceNumber);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceType(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Flag); 
        [System.ServiceModel.OperationContract()]
        DataSet SelectType(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);

        [System.ServiceModel.OperationContract()]
        DataSet SelectTypeByCondition(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string SqlCondition);
        [System.ServiceModel.OperationContract()]
        DataSet SelectTypeByFlag(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Flag);
        [System.ServiceModel.OperationContract()]
        int UpdateType(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code, string Description, string SequenceNumber);
        [System.ServiceModel.OperationContract()]
        int Update2Type(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code, string Description, string Flag, string DefaultDescription, string SequenceNumber, string DefaultSequenceNumber);
        [System.ServiceModel.OperationContract()]
        int RestoreType(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);
        [System.ServiceModel.OperationContract()]
        int DeleteType(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code);
        #endregion

        #region State
        [System.ServiceModel.OperationContract()]
        int InsertState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code, string Description, string Flag, string DefaultDescription, string Field1, string Field2, string Field3, string Field4, string Field5, string SequenceNumber, string DefaultSequenceNumber);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Flag);

        [System.ServiceModel.OperationContract()]
        DataSet SelectStateByCondition(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string SqlCondition);

        [System.ServiceModel.OperationContract()]
        DataSet SelectState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);
        [System.ServiceModel.OperationContract()]
        //DataSet SelectStateByFlag(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Flag);
        //[System.ServiceModel.OperationContract()]
        int UpdateState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code, string Description, string SequenceNumber);
        [System.ServiceModel.OperationContract()]
        int Update2State(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code, string Description, string Flag, string DefaultDescription, string SequenceNumber, string DefaultSequenceNumber);
        [System.ServiceModel.OperationContract()]
        int RestoreState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);
        [System.ServiceModel.OperationContract()]
        int DeleteState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Code);
        #endregion

        #region Equipment
        [System.ServiceModel.OperationContract()]
        int InsertEquipment(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Number, string Description, decimal Length, decimal Width, decimal Height, string StateCode, string TypeCode, string WCSTypeCode, string ZoneID, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceEquipment(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceEquipmentWithoutEquipment(string WarehouseID, string TypeCode);
        [System.ServiceModel.OperationContract()]
        DataSet SelectEquipment(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectEquipment_WithDescription(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateEquipment(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EquipmentID, string Number, string Description, decimal Length, decimal Width, decimal Height, string StateCode, string TypeCode, string WCSTypeCode, string ZoneID);
        [System.ServiceModel.OperationContract()]
        int UpdateEquipmentState(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string[] Number, string[] StateCode);
        [System.ServiceModel.OperationContract()]
        int DeleteEquipment(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EquipmentID);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceEquipmentWithZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string Flag);
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineEquipmentType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EquipmentID);

        [System.ServiceModel.OperationContract()]
        DataTable GetEquipmentN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        Result InsertEquipmentN(LoginInfo loginInfo, EquipmentN entity);

        [System.ServiceModel.OperationContract()]
        Result UpdateEquipmentN(LoginInfo loginInfo, EquipmentN entity);

        [System.ServiceModel.OperationContract()]
        Result DeleteEquipmentN(LoginInfo loginInfo, EquipmentN entity);

        #endregion

        #region EquipmentLine
        [System.ServiceModel.OperationContract()]
        int InsertEquipmentLine(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string EquipmentID, int Line, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceEquipmentLine(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, int Line);
        [System.ServiceModel.OperationContract()]
        DataSet SelectEquipmentLine(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);
        [System.ServiceModel.OperationContract()]
        int DeleteEquipmentLine(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string EquipmentID, int Line);
        [System.ServiceModel.OperationContract()]
        DataSet GetEquipmentLineByZoneID(string equipmentNumber, string userName, string userPassword, string warehouseId, string zoneId);

        [System.ServiceModel.OperationContract()]
        int UpdateEquipmentLine(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string EquipmentID, int OldLine, int NewLine);

        [System.ServiceModel.OperationContract()]
        Result InsertEquipmentLineN(LoginInfo loginInfo, EquipmentLineN entity);

        [System.ServiceModel.OperationContract()]
        Result UpdateEquipmentLineN(LoginInfo loginInfo, EquipmentLineN entity);

        [System.ServiceModel.OperationContract()]
        Result DeleteEquipmentLineN(LoginInfo loginInfo, EquipmentLineN entity);

        [System.ServiceModel.OperationContract()]
        DataTable SelectEquipmentLineN(LoginInfo loginInfo, Dictionary<string, string> Params);
        #endregion

        #region Zone
        [System.ServiceModel.OperationContract()]
        string InsertZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Code, string Name, string Description, string TypeCode, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        
  //      [MethodDesc("Strategy.Initialize.Zone.SelectZone", "选择区域")]
        [System.ServiceModel.OperationContract()]
        DataSet SelectZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
      
   //     [MethodDesc("Strategy.Initialize.Zone.UpdateZone", "修改区域")]
          [System.ServiceModel.OperationContract()]
        int UpdateZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string Code, string Name, string Description, string TypeCode);
        
    //    [MethodDesc("Strategy.Initialize.Zone.DeleteZone", "删除区域")]
        [System.ServiceModel.OperationContract()]
        int DeleteZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);

        //ADD By XSJ
        [System.ServiceModel.OperationContract()]
        DataSet SelectZoneByID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string ZoneID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectZoneByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SqlCondition);
        //ADD BY WZY2009/06/04
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceZone_ChoiceWithType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceZone_ChoiceWithType2(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //RF获取车间
        [System.ServiceModel.OperationContract()]
        DataSet SelectZoneWithWarehouseID(string WarehouseID);

        //只获取暂存区
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceZoneNormal(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataTable GetZoneN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);
        #endregion

        #region ItemCatalog
        [System.ServiceModel.OperationContract()]
        String InsertItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.ItemCatalog entity);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.ItemCatalog entity, String catalogId);
        [System.ServiceModel.OperationContract()]
        Int32 DeleteItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId, String catalogId);
        [System.ServiceModel.OperationContract()]
        Collection<ItemCatalog_View> SelectItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId);
 [System.ServiceModel.OperationContract()]
        Collection<ItemCatalog_View> SelectEnabledItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId);

        [System.ServiceModel.OperationContract()]
        Collection<ItemCatalog_View> SelectOneItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId, String catalogId);
        [System.ServiceModel.OperationContract()]
        Collection<ItemCatalog_View> ChoiceItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId);
        [System.ServiceModel.OperationContract()]
        bool CheckSameCatalogCodeWhenNew(String equipmentNumber, String userId, String userPassword, String WarehouseId, String CatalogCode);

        [System.ServiceModel.OperationContract()]
        bool CheckSameCatalogCodeWhenUpdate(String equipmentNumber, String userId, String userPassword, String WarehouseId, String CatalogCode, String CatalogId);
        

        [System.ServiceModel.OperationContract()]
        DataSet DsSelectOneItemCatalog(String equipmentNumber, String userId, String userPassword, String warehouseId, String catalogId);
        
        #endregion

        #region Location
        [System.ServiceModel.OperationContract()]
        int InsertLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string[] LocationNumber, Int32[] LocationLine, Int32[] LocationColumn, Int32[] LocationLayer, Int32[] LocationDepth, string LocationFlag, decimal Length, decimal Width, decimal Height, string ParentID, string LocationState, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);
        [System.ServiceModel.OperationContract()]
        int UpdateLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string LocationID, decimal Length, decimal Width, decimal Height, string ParentID, string LocationFlag);
        [System.ServiceModel.OperationContract()]
        int UpdateLocationZone(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string OldZoneID, string NewZoneID, string LocationID, string LocationState);
        [System.ServiceModel.OperationContract()]
        int DeleteLocation(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string LocationID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectLocation_RepeatContainerBarCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectLocation_RepeatItemBarCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);

        [System.ServiceModel.OperationContract()]
        Int32 UpdateLocationState(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string LocationID, String OldStateCode, String NewStateCode);

        #region 5.0
        [System.ServiceModel.OperationContract()]
        DataTable GetLocationsN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        Result InsertLocationN(LoginInfo loginInfo, LocationN location);

        [System.ServiceModel.OperationContract()]
        Result InsertLocationRangeN(LoginInfo loginInfo,string FormatString, LocationN location, Dictionary<string, int> Params);

        [System.ServiceModel.OperationContract()]
        Result UpdateLocationN(LoginInfo loginInfo, LocationN location);

        [System.ServiceModel.OperationContract()]
        Result UpdateLocationRangeN(LoginInfo loginInfo, Dictionary<String, String> Params, List<String> LocationsID);

        #endregion

        #endregion

        #region ItemPrice
        [System.ServiceModel.OperationContract()]
        int InsertItemPrice(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, decimal Price, string StateCode, string Description, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemPrice(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateItemPrice(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PriceID, decimal Price, string StateCode, string Description);
        [System.ServiceModel.OperationContract()]
        DataSet SelectItemPrice(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int DeleteItemPrice(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string PriceID);
        #endregion

        #region Shipper
        [System.ServiceModel.OperationContract()]
        string InsertShipper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode, string Name, string Description, string Telephone, string Fax, string Address, string Postcode, string Contract, string ReportSample, string StateCode, string Grade, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceShipper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string Shipper);
        [System.ServiceModel.OperationContract()]
        DataSet SelectShipper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
 
        [System.ServiceModel.OperationContract()]
        int UpdateShipper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ShipperID, string TypeCode, string Name, string Description, string Telephone, string Fax, string Address, string Postcode, string Contract, string ReportSample, string StateCode, string Grade);
        [System.ServiceModel.OperationContract()]
        int DeleteShipper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ShipperID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectShipperByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition);

        [System.ServiceModel.OperationContract()]
        DataSet SelectShipperByItemID(string WarehouseID, string ItemID);
        #endregion

        #region UnitGroup
        [System.ServiceModel.OperationContract()]
        string InsertUnitGroup(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Name, string Description, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceUnitGroup(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectUnitGroup(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateUnitGroup(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupID, string Name, string Description);
        [System.ServiceModel.OperationContract()]
        int DeleteUnitGroup(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectUnitGroupByGroupID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string GroupID);
        #endregion

        #region ItemUnit
        [System.ServiceModel.OperationContract()]
        int InsertItemUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupID, string CName, string EName, decimal Rate, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectItemUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupID);
        [System.ServiceModel.OperationContract()]
        int UpdateItemUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupID, string UnitID, string CName, string EName, decimal Rate, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        int DeleteItemUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string GroupID, string UnitID);
        [System.ServiceModel.OperationContract()]
        EntityDefination.ItemUnit GetItemUnitByItemID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemUnitWithItemCode(string WarehouseID, string Code);
        #endregion

        #region Item
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItem_WithUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItem_WithType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode);
        [System.ServiceModel.OperationContract()]
        DataSet SelectItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
       
        [System.ServiceModel.OperationContract()]
        int DeleteItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemNameByItemID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemByParentID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ParentID, bool AllOtherWarehouseItem);

        [System.ServiceModel.OperationContract()]
        bool CheckItemSameCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,  string SameCode,string extraCondition);

        [System.ServiceModel.OperationContract()]
        bool CheckSameMarkCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Marknumber, string DetailMarkNumber, string extraCondition);

        [System.ServiceModel.OperationContract()]
        bool CheckItemisHaveChild(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        Stack<string> ChoiceItemByParentIDForDelete(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceContainerItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        Item_Short Item_ReadByCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Code);
        [System.ServiceModel.OperationContract()]
        DataSet Item_ReadByItemID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);
        [System.ServiceModel.OperationContract()]
        DataSet Item_ReadByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition);
        [System.ServiceModel.OperationContract()]
        DataSet SelectItemwithSqlCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition,string ItemTypecode);

        [System.ServiceModel.OperationContract()]
        DataSet SelectItemManageSqlCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition, string ItemTypecode);

        [System.ServiceModel.OperationContract()]
        DataSet SelectProductItemwithSqlCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition);

        //Add By PC 2009/12/7
        //Change By PC 2010/01/21
        [System.ServiceModel.OperationContract()]
        DataSet Item_ReadByZone(string EquipmentID, string UserName, string UserPassword, string WarehouseID, string Zoneid, string sqlCondition);

        //Add By PC 2010/01/05
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemButGroup(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID);
        //Add By Jeffrey
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemData(string EquipmentID, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet GetItemInfoByConditions(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.ITEM.ITEMQueryConditionBase Conditions);
        [System.ServiceModel.OperationContract()]        
        String InsertItem(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.ITEM entity);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateItem(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.ITEM entity, String itemId);

         [System.ServiceModel.OperationContract()]
        DataTable GetItemContainerInfo(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);
        #endregion

        #region Inventory
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceInventory(string WarehouseID, string[] ItemID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceOutsideInventory(string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, bool bSelectAll);
        [System.ServiceModel.OperationContract()]
        int UpdateInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID, string OPCode, decimal Value);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceInventoryUseContainerID(string WarehouseID, string ContainerID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectNormalInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);
        [System.ServiceModel.OperationContract()]
        int UpdateNormalInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerID, decimal Value);

        [System.ServiceModel.OperationContract()]
        DataTable GetInventoryN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        Result UpdateInventoryN(LoginInfo loginInfo, InventoryN entity);

        [System.ServiceModel.OperationContract()]
        Result UpdateNormalInventoryN(LoginInfo loginInfo, string itemId, decimal tempAreaQty);

        //BY LIUCUI ADD 按配盘库存预警
        [System.ServiceModel.OperationContract()]
        DataTable GetContainerInventoryN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        Result UpdateContainerInventoryN(LoginInfo loginInfo, ContainerInventoryN entity);

        //BY LIUCUI ADD 按配盘库存预警

        #endregion

        #region Bill
        [System.ServiceModel.OperationContract()]
        int InsertBill(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode, string Name, string Description, string BillNumberMask, long BillNumber, int BillNumberLength, string BillNumberDateMask, bool BillNumberDateMode, string LastDate, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectBill(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectBillByName(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Name);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceBill(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateBill(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode, string Name, string Description, string BillNumberMask, long BillNumber, int BillNumberLength, string BillNumberDateMask, bool BillNumberDateMode, string LastDate);
        [System.ServiceModel.OperationContract()]
        int DeleteBill(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Name);
        #endregion

        #region BillNumber
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceBillNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BillName);
        [System.ServiceModel.OperationContract()]
        string GetBillNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BillName);
        #endregion

        #region Runs
        //[System.ServiceModel.OperationContract()]
        //int InsertRuns(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode, string RunsName, int SignStartTime, int SignEndTime, int DateAdd);
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceRuns(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectRuns(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //[System.ServiceModel.OperationContract()]
        //int UpdateRuns(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RunsID, string TypeCode, string RunsName, int SignStartTime, int SignEndTime, int DateAdd);
        //[System.ServiceModel.OperationContract()]
        //int DeleteRuns(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RunsID);
        #endregion

        #region  ContainerType
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceContainerType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceOneContainerType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerTypeID);
        //[System.ServiceModel.OperationContract()]
        //int UpdateContainerType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerTypeID, string ContainerTypeName, string Description);
        //[System.ServiceModel.OperationContract()]
        //int InsertContainerType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerTypeID, string ContainerTypeName, string Description);
        //[System.ServiceModel.OperationContract()]
        //int DeleteContainerType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerTypeID);
        #endregion

        #region CombinationType
        //Remove
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceCombinationType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID);
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectCombinationType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //[System.ServiceModel.OperationContract()]
        //int UpdateCombinationType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID, string TypeName, string Description, string UnitID, decimal Qty, string Remarks);
        //[System.ServiceModel.OperationContract()]
        //int InsertCombinationType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID, string CombinationTypeID, string TypeName, string Description, string UnitID, decimal Qty, string Remarks);
        //[System.ServiceModel.OperationContract()]
        //int DeleteCombinationType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID, string CombinationTypeID);
        #endregion

        #region CombinationDetail
        //Remove
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectCombinationDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID);
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceCombinationDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID);
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceOneCombinationDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID, string CatalogID);
        //[System.ServiceModel.OperationContract()]
        //int UpdateCombinationDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID, string CatalogID, decimal Qty, string Remarks, DateTime CreateDate);
        //[System.ServiceModel.OperationContract()]
        //int InsertCombinationDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID, string CatalogID, decimal Qty, string Remarks, DateTime CreateDate);
        //[System.ServiceModel.OperationContract()]
        //int DeleteCombinationDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID, string CatalogID);
        #endregion

        #region MachineProductPlan
        [System.ServiceModel.OperationContract()]
        DataTable GetLotNumber(LoginInfo loginInfo, string itemid);

        [System.ServiceModel.OperationContract()]
        DataSet GetWCSFillBinPlan(LoginInfo loginInfo, string SqlString);

        [System.ServiceModel.OperationContract()]
        string InsertWCSFillBinPlan(LoginInfo loginInfo,MachineProductPlan entity);

        [System.ServiceModel.OperationContract()]
        int UpdateWCSFillBinPlan(LoginInfo loginInfo, MachineProductPlan entity);

        [System.ServiceModel.OperationContract()]
        int DeleteWCSFillBinPlan(LoginInfo loginInfo, MachineProductPlan entity);

        [System.ServiceModel.OperationContract()]
        Int32 CheckSchemeTask(LoginInfo loginInfo, MachineProductPlan entity);

        //更新机台生产牌号信息
        [System.ServiceModel.OperationContract()]
        int UpdateMachineProductPlan(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineEquipmentID, string SchemeID, string ItemID, string MachineNumber, int Qty, string LotNumber, string Version);

        //完成机台生产牌号信息
        [System.ServiceModel.OperationContract()]
        int FinishMachineProductPlan(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineNumber, string SchemeID, string ItemID);

        //获取机台生产计划及设备信息
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineProductPlanAndEquipmentInfo(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet GetMachineProductPlanAndEquipmentByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string SqlString);

        //获取牌号物料信息
        [System.ServiceModel.OperationContract()]
        DataSet GetProductItemInfo(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemTypeCode);

        [System.ServiceModel.OperationContract()]
        int DeleteMachineProductPlan(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineEquipmentID, string MachineNumber);
        ////更改计划状态
        //[System.ServiceModel.OperationContract()]
        //int UpdateSchemeState(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string SchemeID,string OperateType);

        //获取所有新的计划信息
        [System.ServiceModel.OperationContract()]
        DataSet GetSchemeInfo(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        //检查机台是否有正在使用中的站台位
        [System.ServiceModel.OperationContract()]
        int Check_HasUsingState(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineNumber);

        #endregion

        #region MachineProductPlanHis
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceMachineProductPlanHis(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineEquipmentID);
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectMachineProductPlanHis(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //[System.ServiceModel.OperationContract()]
        //int InsertMachineProductPlanHis(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineEquipmentID, string SchemeID, string ItemID, string StateCode, DateTime CreateDate, DateTime StartDate, DateTime EndDate, decimal Qty, string Memo);
        #endregion

        #region  MachineDock
        [System.ServiceModel.OperationContract()]
        DataSet SelectMachineDock_ProductData(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectDistinctMachineDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceMachineDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode);
        [System.ServiceModel.OperationContract()]
        DataSet SelectMachineDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateMachineDockCurTaskNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineNo, string DockNumber, string CurPalletTaskNumber);
        [System.ServiceModel.OperationContract()]
        int UpdateMachineDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineNumber, string DockNumber, string DockName, string DockStatus, string ModelCode, string ZoneID, int ItemSource, string CurContainerID, string CurContainerBarcode, string CurBOMID, string PerDockNumber);
        [System.ServiceModel.OperationContract()]
        int InsertMachineDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineNumber, string MachineType, string DockNumber, string DockName, string DockStatus, string ModelCode, string CurpalletNumber, string CurtaskNumber, string Curcontainerid, string curbillid, string CurBOMid, string Zoneid, string ZoneCode, string Modelid, int ItemSource, string Userid, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        int DeleteMachineDock(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string MachineNumber, string DockNumber);
        //根据机台编号获取生产牌名
        [System.ServiceModel.OperationContract()]
        DataSet SelectItemNameWithMachineCode(string WarehouseID, string MachineCode);
        //根据仓库、车间、机台编号获取站台位信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectDockWithMachineCode(string WarehouseID, string WorkShop, string MachineCode);
        //根据仓库、机台编号、站台位获取状态信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectStateWithDockNumber(string WarehouseID, string MachineCode, string DockNumber);

        //根据仓库、机台编号、站台位获取区段信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectModelCodeWithDockNumber(string WarehouseID, string MachineCode, string DockNumber);

        //根据仓库、ContainerID获取ContainerDetail信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectContainerDetailWithContainerID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerID);

        //检查站台编号是否重复
        [System.ServiceModel.OperationContract()]
        bool CheckExistDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string DockNumber);

        //设置机台数据
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineDockDataSetting(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string DockNumber);

        //获取班次数据 QWH 2010/08/02
        [System.ServiceModel.OperationContract()]
        DataSet GetRunsData(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);
        
        [System.ServiceModel.OperationContract()]
        WMSInterface.Entity50.Result InsertMachineDockN(WMSInterface.Entity50.LoginInfo loginInfo, WMSInterface.Entity50.MachineDockN machineDock);

        [System.ServiceModel.OperationContract()]
        WMSInterface.Entity50.Result UpdateMachineDockN(WMSInterface.Entity50.LoginInfo loginInfo, WMSInterface.Entity50.MachineDockN machineDock);

        [System.ServiceModel.OperationContract()]
        DataTable GetDockN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetMachineDockN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable SelectMachineDockN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);

        #endregion

        #region ContainerInventory
        [System.ServiceModel.OperationContract()]
        DataSet SelectContainerInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, bool is_SelectAllState);

        [System.ServiceModel.OperationContract()]
        DataTable SelectContainerInventoryN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataSet SelectContainerInventoryByItemID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceContainerInventory(string WarehouseID, string[] ContainerTypeID);
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceOutsideCombinationInventory(string WarehouseID);
        [System.ServiceModel.OperationContract()]
        int UpdateContainerInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string OPCode, decimal Value);
        #endregion

        #region ProcessBOMType

        // add by cc 0819
        [System.ServiceModel.OperationContract()]
        DataSet GetProductionBOMDetailID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string SEQUENCENUMBER);
        //添加生产BOM物料明细时判断是否已存在
        [System.ServiceModel.OperationContract()]
        bool CheckExistsItem(string WAREHOUSEID, string DETAILID, string ITEMID,string _type);
        //根据DetailID 获取生产BOMDetialItem的数量，用于控制上下顺序按钮的显示
        [System.ServiceModel.OperationContract()]
        int GetRowCoutByDetailID(string WAREHOUSEID, string DETAILID);
        //生产BOM上下顺序调整
        [System.ServiceModel.OperationContract()]
        bool SortProductionBOMDetailItem(string WAREHOUSEID, string DETAILID, string ITEMID, int Direction);

        [System.ServiceModel.OperationContract()]
        DataSet GetProcessBOMID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMCode);
        //根据产品名称得到配盘单信息
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceProcessBOMType_ByItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID,string strItemName, string EquipmentTypeCode, int DisplayAll);
        [System.ServiceModel.OperationContract()]
        DataSet SelectProcessBOMType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, int DisplayAll);
        //新增加工BOM主表
        [System.ServiceModel.OperationContract()]
        int UpdateProcessBOMType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BomID, string ItemID, string Priority, string BOM1Status, string BOM2Status, string Description, string BOMCode, decimal TotalQty, string Field1,string EquipmentType,string txtVersion);
        //修改生产BOM主表
        [System.ServiceModel.OperationContract()]
        int UpdateProductionBOMType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string ItemID, string ItemName, string BOM1Status, string BOMCode, decimal TotalQty, string Version);
        //新增加工BOM主表
        [System.ServiceModel.OperationContract()]
        int InsertProcessBOMType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMNumber, string ItemID, string ItemName, string Priority, string BOM1Status, string BOM2Status, string Description, string BOMCode, string StateCode, decimal TotalQty, string Version, string ModelCode, string BOMType, string EquipmentTypeCode, string Field1, string Field2, string Field3, string Field4, string Field5);
        //新增生产BOM主表
        [System.ServiceModel.OperationContract()]
        string InsertProductionBOMType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMNumber, string ItemID, string ItemName, string Priority, string BOM1Status, string BOM2Status, string Description, string BOMCode, string StateCode, decimal TotalQty, string Version, string ModelCode, string BOMType, string EquipmentTypeCode, string Field1, string Field2, string Field3, string Field4, string Field5);

        //新增生产BOM主表，同时增加明细表,by xsj
        [System.ServiceModel.OperationContract()]
        string InsertProductionBOMMainAndDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMNumber, string ItemID, string ItemName, string Priority, string BOM1Status, string BOM2Status, string Description, string BOMCode, string StateCode, decimal TotalQty, string Version, string ModelCode, string BOMType, string EquipmentTypeCode, string Field1, string Field2, string Field3, string Field4, string Field5,List< WMSInterface.EntityDefination.ProductionBOMDetailSingle> DetailList);

        //更新生产BOM主表，同时更新明细表
         [System.ServiceModel.OperationContract()]
        int UpdateProductionBOMMainAndDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string ItemID, string ItemName, string BOM1Status, string BOMCode, decimal TotalQty, string Version, List<WMSInterface.EntityDefination.ProductionBOMDetailSingle> DetailList);

         //删除生产BOM主表，同时删除明细表
         [System.ServiceModel.OperationContract()]
         int DeleteProductBOMMainAndDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BomID, string ItemID, string BOMCode);

        [System.ServiceModel.OperationContract()] //删除配盘单 (片烟库 生产BOM) & (辅料库 加工BOM) 共用
        int DeleteProcessBOMType(int type,string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BomID, string ItemID, string BOMCode);
       
        
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectProcessBOMType_WithBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectProcessBOMType_WithItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        //根据产品ID得到产品单位
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceProcessBOMTypeUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);
        //获得存在的版本信息
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceProcessBOMTypeVersion(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID, string EquipmentTypeCode, string ModelCode);
        //获取配盘单所有信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectAllBOMCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, int BOMType);

        //通过BOMID获取配盘单信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectBOMCodeByBOMID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, int BOMType,string BOMID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectAllBOMCodeNotStop(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, int BOMType);

        [System.ServiceModel.OperationContract()]
        int WriterLog(string EquipmentNumber, string UserName, string warehouseid, string BOMCode, string NewBOMCodeState, string OldBOMCodeState, string NewPriority, string OldPriority, string NewVersion, string OldVersion);
        //检查配盘单库存
        [System.ServiceModel.OperationContract()]
        bool CheckBOMInventory(string EquipmentNumber, string UserName, string UserPassWord, string warehouseid, string BomID, string ModelID, string ModeCode, string Version);

        [System.ServiceModel.OperationContract()]
        DataSet SelectProductBOMTypeByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition);
        #endregion

        #region ProcessBOMDetail
        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceProcessBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProcessBOMTypeID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectProcessBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID);
        
        //修改加工BOM明细以及加工Bom 修改物料明细
        [System.ServiceModel.OperationContract()]
        int UpdateProcessBOMDetail(int type,string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BomID, string DETAILID, string OldItemID, decimal Qty, string Remark, string Field1,string name,string newItemid);
        //新增加工BOM明细
        [System.ServiceModel.OperationContract()]
        int InsertProcessBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, int LevelNumber, string ItemID, string ItemName, decimal Qty, decimal QtyPercent, string Remark, string Field1, string Field2, string Field3);

        //2010-01-18 yyl修改
        [System.ServiceModel.OperationContract()]
        int InsertProcess_BOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, SelectItem item);

        //新增生产BOM明细
        [System.ServiceModel.OperationContract()]
        int InsertProductionBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string DetailID, string ItemID, string ItemName, decimal Qty, decimal QtyPercent, string Remark, string SequenctNumber, string Field1, string Field2, string Field3, int InsertType);
        //删除生产BOM明细
        [System.ServiceModel.OperationContract()]
        int DeleteProductionBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WAREHOUSEID, string DETAILID, string ITEMID);

        //修改生产BOM明细
        [System.ServiceModel.OperationContract()]
        int UpdateBOMDetailItem(string EquipmentNumber, string UserName, string UserPassword, string WAREHOUSEID, string DETAILID, string ITEMID, decimal QTY, string REMARK);

        [System.ServiceModel.OperationContract()]
        int DeleteProcessBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BomID, string DETAILID, string ItemID);

        //查询首选物料
        [System.ServiceModel.OperationContract()]
        DataSet SelectMasterBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID);
        //查询备选物料
        [System.ServiceModel.OperationContract()]
        DataSet SelectExtraBOMDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string MasterItem);


        #endregion

        #region Section
        [System.ServiceModel.OperationContract()]
        int InsertSection(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Code, string Description, string Status, string Remark, string UserID, DateTime CreateTime, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceSection(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string Status);
        [System.ServiceModel.OperationContract()]
        DataSet SelectSection(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);

        [System.ServiceModel.OperationContract()]
        int UpdateSection(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Code, string Description, string Status, string Remark);
        //[System.ServiceModel.OperationContract()]
        //int RestoreSection(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);
        [System.ServiceModel.OperationContract()]
        int DeleteSection(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Code);

        #region 5.0
        [System.ServiceModel.OperationContract()]
        DataTable GetModelN(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        Result InsertModelN(WMSInterface.Entity50.LoginInfo loginInfo,ModelN entity);

        [System.ServiceModel.OperationContract()]
        Result UpdateModelN(WMSInterface.Entity50.LoginInfo loginInfo, ModelN entity);

        [System.ServiceModel.OperationContract()]
        Result DeleteModelN(WMSInterface.Entity50.LoginInfo loginInfo, ModelN entity);
        #endregion
        #endregion

        #region BOMSetting
        //根据配盘单号获得所有产品名称
        [System.ServiceModel.OperationContract()]
        DataSet SelectProductwithBOMID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID);
        //根据配盘单号获得所有机台型号
        [System.ServiceModel.OperationContract()]
        DataSet SelectEquipmentTypewithBOMID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID);

        //插入产品名称或者机台型号
        [System.ServiceModel.OperationContract()]
        int InsertBOMSetting(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string BOMNumber, string ModelID, string ModelCode, string ItemID, string ItemName, string TypeCode, string TypeDescription, string UserID, DateTime CreateTime, string Field1, string Field2, string Field3, string Field4, string Field5);
        //删除机台型号记录
        [System.ServiceModel.OperationContract()]
        int DeleteBOMSetting(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string TypeCode);
        //判断要删除的牌号是否是默认牌号
        [System.ServiceModel.OperationContract()]
        DataSet ItemisDefault(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMNumber, string ItemID);
        //判断要删除的机台型号是否是默认机台型号
        [System.ServiceModel.OperationContract()]
        DataSet EquipmentTypeisDefault(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMNumber, string EquipmentTypeCode);

        //删除产品记录
        [System.ServiceModel.OperationContract()]
        int DeleteBOMSettingItem(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BOMID, string ItemID);
        #endregion

        #region CombinationCount
        //获取可用于配盘的配盘单数据
        [System.ServiceModel.OperationContract()]
        DataSet GetNotStopStateBOMByItemID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        //获取BOM明细
        [System.ServiceModel.OperationContract()]
        DataSet GetBOMDetailByBomID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BomID);
        #endregion

        #region WorkflowFile
        [System.ServiceModel.OperationContract()]
        int InsertWorkflowFile(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string FileName, string FileValue);

        [System.ServiceModel.OperationContract()]
        int UpdateWorkflowFile(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string FileName, string FileValue);

        [System.ServiceModel.OperationContract()]
        string SelectWorkflowFile(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string FileName);

        [System.ServiceModel.OperationContract()]
        DataSet SelectStrategyDataByFlag(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string Flag);
        #endregion

        #region InventoryOverView
        [System.ServiceModel.OperationContract()]
        DataSet SelectEquipmentAndLocationLineDataByZoneID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectLocationAndItemDataByZoneIDAndEquipmentID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ZoneID, string EquipmentID);

        #endregion InventoryOverView

        #region Move
        [System.ServiceModel.OperationContract()]
        DataSet GetAllLocationByZoneID(string EquipmentNumber, string UserName, string UserPassword, String warehouseId, String zoneId, String locationState, String equipmentId);
        #endregion

        #region LotNumber Inventory
        [System.ServiceModel.OperationContract()]
        DataSet SelectLotInventory(string equipmentNumber, string userId, string userPassword, string warehouseId,string type);//新增类型  peq 2011/9/4


        #endregion

        #region SelectInventoryInfo
        [System.ServiceModel.OperationContract()]
        DataSet SelectInventoryInfo(String equipmentNumber, String userId, String userPassword, String warehouseId, String locationNumber, String code, String name, String zoneId, String equipmentId, String bomCode);
        #endregion

        #region ChildItemSetting
        [System.ServiceModel.OperationContract()]
        String InsertChildItemSetting(string equipmentNumber, String userId, string userPassword, string warehouseId, WMSInterface.EntityDefination.ChildItemSetting entity);

        [System.ServiceModel.OperationContract()]
        Int32 UpdateChildItemSetting(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.ChildItemSetting entity, String childId);

        [System.ServiceModel.OperationContract()]
        Int32 DeleteChildItemSetting(String equipmentNumber, String userId, String userPassword, String warehouseId, String childId);

        [System.ServiceModel.OperationContract()]
        Collection<ChildItemSetting_View> SelectChildItemSetting(String equipmentNumber, String userId, String userPassword, String warehouseId);

        [System.ServiceModel.OperationContract()]
        Collection<ChildItemSetting_View> ChoiceChildItemSetting(String equipmentNumber, String userId, String userPassword, String warehouseId,String itemId);

        [System.ServiceModel.OperationContract()]
        Boolean CheckChildItemSetting(string equipmentNumber, String userId, string userPassword, string warehouseId, String itemId,String childName);
        #endregion

        #region StatisticUnit
        [System.ServiceModel.OperationContract()]
        String InsertStatisticUnit(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.StatisticUnit entity);
        [System.ServiceModel.OperationContract()]
        String UpdateStatisticUnit(String equipmentNumber, String userId, String userPassword, String warehouseId, String id, WMSInterface.EntityDefination.StatisticUnit entity);
        [System.ServiceModel.OperationContract()]
        String DeleteStatisticUnit(String equipmentNumber, String userId, String userPassword, String warehouseId, String id);
        [System.ServiceModel.OperationContract()]
        String DeleteStatisticUnitRecord(String equipmentNumber, String userId, String userPassword, String warehouseId, String id);
        [System.ServiceModel.OperationContract()]
        Collection<StatisticUnit_View> SelectStatisticUnit(String equipmentNumber, String userId, String userPassword, String warehouseId, StatisticUnitQueryConditionBase condition);

        #endregion

        #region StatisticValue
        [System.ServiceModel.OperationContract()]
        String InsertStatisticValue(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.EntityDefination.StatisticValue entity);
        [System.ServiceModel.OperationContract()]
        String UpdateStatisticValue(String equipmentNumber, String userId, String userPassword, String warehouseId, String id, WMSInterface.EntityDefination.StatisticValue entity);
        [System.ServiceModel.OperationContract()]
        String DeleteStatisticValue(String equipmentNumber, String userId, String userPassword, String warehouseId, String id);
        [System.ServiceModel.OperationContract()]
        String DeleteStatisticValueRecord(String equipmentNumber, String userId, String userPassword, String warehouseId, String id);
        [System.ServiceModel.OperationContract()]
        Collection<StatisticValue_View> SelectStatisticValue(String equipmentNumber, String userId, String userPassword, String warehouseId, StatisticValueQueryConditionBase condition);

        [System.ServiceModel.OperationContract()]
        DataTable GetUnit(string warehouseid);


        #endregion

        #region GetUserInfo
        [System.ServiceModel.OperationContract()]
         DataSet GetUserInfoByLoginId(string warehouseID, string loginId);

        [System.ServiceModel.OperationContract()]
         DataSet GetUserInfoByUserId(string warehouseID, string UserID);
        
        #endregion

        #region OverView

        [System.ServiceModel.OperationContract()]
        DataTable GetLocations(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string strWhere);

        #endregion

        #region SilkQc
        [System.ServiceModel.OperationContract()]
        int InsertSilkQc(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string LotNumber, string ItemCode, string CheckState, string ItemId);

        //[System.ServiceModel.OperationContract()]
        //DataSet ChoiceCheckState(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string StateFlag);

        [System.ServiceModel.OperationContract()]
        DataSet SelectSilkQc(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword);

        [System.ServiceModel.OperationContract()]
        int UpdateSilkQc(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string LotNumber, string ItemCode, string CheckState, string ItemId);

        [System.ServiceModel.OperationContract()]
        int UpdateContainer(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string LotNumber, string ItemCode, string CheckState, string ItemId);
        #endregion SilkQc

        #region 配盘单
        [System.ServiceModel.OperationContract()]
        DataSet GetBomInfoByConditions(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.BomQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        Collection<BOMInfo> GetBomInfo(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.BomQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        DataSet dsGetBomInfo(string EquipmentNumber, string UserID, string UserPassword, string WarehouseID, WMSInterface.EntityDefination.BomQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        String InsertBom(string equipmentNumber, string userId, string userPassword, string warehouseId, WMSInterface.EntityDefination.BOMHeadAndDetails entity);

        [System.ServiceModel.OperationContract()]
        Int32 UpdateBom(string equipmentNumber, string userId, string userPassword, string warehouseId, WMSInterface.EntityDefination.BOMHeadAndDetails entity, string BomId);

        [System.ServiceModel.OperationContract()]
        Int32 UpdateBomOtherInfo(string equipmentNumber, string userId, string userPassword, string warehouseId, WMSInterface.EntityDefination.BOMHeadAndDetails entity, string BomId);

        [System.ServiceModel.OperationContract()]
        Int32 DeleteBOM(string equipmentNumber, string userId, string userPassword, string warehouseId, string BomId);

        [System.ServiceModel.OperationContract()]
        bool CheckExitsBomCode(string equipmentNumber, string userId, string userPassword, string warehouseId, string bomCode);
        [System.ServiceModel.OperationContract()]
        bool CheckBomIdUsed(string equipmentNumber, string userId, string userPassword, string warehouseId, string bomId);

        [System.ServiceModel.OperationContract()]
        DataTable GetProductBOMComputeDetailN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable CheckBomItemInventory(string warehouseId, string itemId);

        #endregion

        [System.ServiceModel.OperationContract()]
        int updateContainerDetail(string lotnumber, string itemid, decimal qty, string warehouseid);

        //供应商和物料关联
        [System.ServiceModel.OperationContract()]
        string InsertItemShipper(ItemShipperEntity ItemShipper);

        [System.ServiceModel.OperationContract()]
        string UpdateItemShipper(ItemShipperEntity ItemShipper);

        [System.ServiceModel.OperationContract()]
        int DeleteItemShipper(string WarehouseId, string RelationId);

        [System.ServiceModel.OperationContract()]
        DataSet QueryItemShipper(string StartTime, string EndTime,
            ItemShipperEntity ItemShipper, string Conditions);

        [System.ServiceModel.OperationContract()]
        DataSet QueryByItemIdAndShipperId(string WarehouseId, string ItemId, string ShipperId);

        [System.ServiceModel.OperationContract()]
        DataSet QueryRelationById(string WarehouseId, string RelationId);

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        DataSet GetRunsWithoutConditions(string warehouseID, string equipmentNumber, string userName, string userPassword);

        [System.ServiceModel.OperationContract()]
        DataSet GetRunsByConditions(string warehouseID, string equipmentNumber, string userName, string userPassword, string conditions);

        [System.ServiceModel.OperationContract()]
        DataSet GetRunsByRunsId(string warehouseID, string equipmentNumber, string userName, string userPassword, string runsId);

        [System.ServiceModel.OperationContract()]
        string InsertRuns(string equipmentNumber, string userName, string userPassword, string warehouseId, string typeCode, string runsName, string signstarttime, string signendtime, string userId, string lotnumber, string date);

        [System.ServiceModel.OperationContract()]
        string UpdateRuns(string equipmentNumber, string userName, string userPassword, string warehouseId, string typeCode, string runsName, string signstarttime, string signendtime, string userId, string runsId, string lotnumber, string date, string runsCode, string runsCodeName);

        [System.ServiceModel.OperationContract()]
        bool DeleteRuns(string equipmentNumber, string userName, string userPassword, string warehouseId, string runsId);

        [System.ServiceModel.OperationContract()]
        DataTable GetMachineInfo(string warehouseId, string machineId);

        [System.ServiceModel.OperationContract()]
        DataTable GetMachines(string warehouseId);

        [System.ServiceModel.OperationContract()]
        DataTable GetMachineModels(string warehouseId, string machineNumber);

        [System.ServiceModel.OperationContract()]
        DataTable GetMachineModelShippers(string warehouseId, string machineNumber, string modelCode);

        [System.ServiceModel.OperationContract()]
        DataTable GetMachineShipperSet(string warehouseId, string machineNumber, string modelCode);

        [System.ServiceModel.OperationContract()]
        bool UpdateMachineShipper(string warehouseId, string machineNumber, string modelCode, string shipperIds, string userId, out string msg);

        [System.ServiceModel.OperationContract()]
        DataTable GetDockSetting(string warehouseId);

        [System.ServiceModel.OperationContract()]
        bool UpdateDockSetting(string warehouseId, string dockNumber, string modelCode, string user);
    }
}
