using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using WMSInterface.EntityDefination;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface OrderWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        Collection<OrderHead_View_Ex> SelectOrderHead(String equipmentNumber, String userId, String userPassword, String warehouseId, SelectStateScope stateScope);


        [System.ServiceModel.OperationContract()]
        String InsertOrderHead(String equipmentNumber, String userId, String userPassword, String warehouseId, OrderHead entity);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateOrderHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId, OrderHead entity);
        [System.ServiceModel.OperationContract()]
        Int32 DeleteOrderHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId);
        [System.ServiceModel.OperationContract()]
        OrderHead_View SelectOneOrderHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId);


        [System.ServiceModel.OperationContract()]
        Collection<OrderDetail_View_Ex> SelectOrderDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId);
        [System.ServiceModel.OperationContract()]
        String InsertOrderDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId, OrderDetail entity);
        [System.ServiceModel.OperationContract()]
        Int32 DeleteOrderDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId, String detailId);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateOrderDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId, String detailId, OrderDetail entity);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateOrderDetailQty(String equipmentNumber, String userId, String userPassword, String warehouseId, String orderId, String detailId, Decimal qty, String memo);

        [System.ServiceModel.OperationContract()]
        List<ProductionBOM> GetProductionBOMHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String itemId);
        [System.ServiceModel.OperationContract()]
        List<ProductionBOMDetail> GetProductionBOMDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String bomId);
        [System.ServiceModel.OperationContract()]
        String ProcessProductionBOM(String equipmentNumber, String userId, String userPassword, String warehouseId, OrderHead order, String bomId, List<ProductionBOMSelectedItem> itemIds, String flag, Decimal qty);
        [System.ServiceModel.OperationContract()]
        List<ItemInventory> GetBOMRelatedInventory(String equipmentNumber, String userId, String userPassword, String warehouseId, String bomId);
        [System.ServiceModel.OperationContract()]
        List<Item_Short> GetBOMPreparedItem(String equipmentNumber, String userId, String userPassword, String warehouseId);

        //[System.ServiceModel.OperationContract()]
        //Int32 UpdateWholeOrderDetail(String equipmentNumber, String userId, String userPassword, String orderId, Collection<OrderDetail> orderDetails);
        //[System.ServiceModel.OperationContract()]
        //Int32 UpdateOrderDetailQty(String equipmentNumber, String userId, String userPassword, String orderId, String detailId, Decimal newQty);


        //获取订单数据 By PC 2009/12/14
        [System.ServiceModel.OperationContract()]
        DataSet GetOrderHeadInfo(string equipmentNumber, string userId, string userPassword, string warehouseId, WMSInterface.SelectStateScope stateScope);

        [System.ServiceModel.OperationContract()]
        bool ChangeOrderDetail(string equipmentNumber, string userId, string userPassword, string warehouseId,string OrderID,string oldDetailID, List<WMSInterface.EntityDefination.OrderDetail_View_Ex> OrderDetails);

        [System.ServiceModel.OperationContract()]
        string OrderSplit(string equipmentNumber, string userId, string userPassword, string warehouseId, WMSInterface.EntityDefination.OrderHead_Ex order,string oldOrderID, string[] detailsid);

        [System.ServiceModel.OperationContract()]
        string OrderToCross(string equipmentNumber, string userId, string userPassword, string warehouseId, string oldOrderID, string CrossBillName);


        #region 5.0
        [System.ServiceModel.OperationContract()]
        Result InsertOrder(WMSInterface.EntityDefination.OrderHead head, List<WMSInterface.EntityDefination.OrderDetail> details, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        Result UpdateOrder(string OrderId, WMSInterface.EntityDefination.OrderHead head, List<WMSInterface.EntityDefination.OrderDetail> details, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        Result DeleteOrder(String OrderID, WMSInterface.Entity50.LoginInfo login);
        [System.ServiceModel.OperationContract()]
        DataTable GetOrderInfo(string viewName, LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        String CheckInventory(Collection<WMSInterface.EntityDefination.WaveInventoryCheck> entities, int StrategyValue_Outbound, LoginInfo login);

        [System.ServiceModel.OperationContract()]
        Result FinishedOrder(string orderId, LoginInfo login);
        #endregion
  
    }
}
