using System;
using System.Collections;
using System.Text;
using System.Data ;
namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface SingleItemsWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        Hashtable GetShiper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        /// 取指定供应商生产的物料种类
        [System.ServiceModel.OperationContract()]
        Hashtable GetCatalogsByShiper(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ShiperID);


        /// 取指定供应商、物料种类的物料信息。
        [System.ServiceModel.OperationContract()]
        Hashtable GetItemsByShiperCatalog(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string shiperID, string CatalogID);


        // 单品种物料收货确认
        [System.ServiceModel.OperationContract()]
        int ReceivingSure(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingID, Hashtable ItemQtyList);

        //RF收货单信息获取
        [System.ServiceModel.OperationContract()]
        DataSet GetReceives(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceivingNumber);
        //RF收货单详细信息获取
        [System.ServiceModel.OperationContract()]
        DataSet GetReceiveMsg(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ReceiveID);
        /// 单品种物料操作日志
        [System.ServiceModel.OperationContract()]
        int WriteLog(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string InventoryID, string shiperName, string TypeCode, string ItemID, decimal Qty, string Description);

        // 获得物料单位名称
        [System.ServiceModel.OperationContract()]
        string GetItemUnidName(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        /// 取物料库存
        [System.ServiceModel.OperationContract()]
        DataSet GetItemInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);


        /// 删除指定批次的物料信息
        [System.ServiceModel.OperationContract()]
        int DeleteItemInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LotNumber);

        [System.ServiceModel.OperationContract()]
        IList GetLotNumberlist(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CatalogID, string shiperID);


        /// 确认退货， 主表删除记录；从表操作日志。
        [System.ServiceModel.OperationContract()]
        int ReturnConfirm(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ShipperID, string LotNumber, string TypeCode, string Description);

        [System.ServiceModel.OperationContract()]
        string GetBillNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string BillName);
        
        [System.ServiceModel.OperationContract()]
        DataSet SelectItemInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        int UpdateItemInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LotNumber, decimal totalQty, decimal usedQty);

        [System.ServiceModel.OperationContract()]
        int ModifyItemInventory(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LotNumber, decimal AddTotalQty, decimal UsedQty);
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

    }
}
