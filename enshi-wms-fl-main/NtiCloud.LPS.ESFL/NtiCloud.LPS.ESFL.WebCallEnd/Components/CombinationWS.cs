using System;
using System.Collections;
using System.Text;
using System.Data;

namespace WMSInterface
{

    [System.ServiceModel.ServiceContract ]
    public interface CombinationWS : _KeepAlive
    {
        #region  配盘
        /// <summary>
        ///  取品规列表
        /// </summary>
         [System.ServiceModel.OperationContract()]
         Hashtable GetTobaccolist(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
         
         /// <summary>
         ///  取某一品规的模式列表
         /// </summary>
         [System.ServiceModel.OperationContract()]
         Hashtable GetCombinationType(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID);

         /// <summary>
         ///  取某一模式的物料Item列表
         /// </summary>
         [System.ServiceModel.OperationContract()]
         DataSet GetCatalogListByCombinationID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID);

         /// <summary>
         /// 根据配盘模式，CatalogID，取Item 的 批次信息
         /// </summary>
         [System.ServiceModel.OperationContract()]
         Hashtable GetItemLotInfo(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID, string CatalogID);

         /// <summary>
         /// 根据ItemID 取Item 的 批次信息
         /// </summary>
         [System.ServiceModel.OperationContract()]
         Hashtable GetItemLotNumberInfo(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        /// <summary>
        ///  配盘确认
        /// </summary>
         [System.ServiceModel.OperationContract()]
         int CombinatePalletSure(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerBarCode, string ProductID, string CombinationTypeID, string[] ItemIDs, decimal[] Qtys,string[] LotNums, decimal[] LotNumQtys, bool SureInBound);
       
        /// <summary>
        /// 通过物料种类CatalogID，批次号，取物料ID
        /// </summary>
         [System.ServiceModel.OperationContract()]
         string GetItemIDByLotNumber(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CatalogID, string LotNumber);
        #endregion 

         #region  配盘计算
         /// <summary>
         /// 通过产品ID，计算出配盘的基本数量(以默认的生产量为一个单位)
         /// </summary>
         [System.ServiceModel.OperationContract()]
         Hashtable GetCombinationTypeBaseQtyByProduct(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID);


         /// <summary>
         /// 通过产品ID，换算比率，计算出配盘数量 (以默认的生产量为一个单位)
         /// </summary>
         [System.ServiceModel.OperationContract()]
         Hashtable GetCombinationTypeQtyByProduct(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ProductID, decimal rate);

        #endregion

         [System.ServiceModel.OperationContract()]
         bool CheckIsExistPreInBound(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerBarCode, string StateCode);

         /// <summary>
         /// 通过ID，取Item信息
         /// </summary>
         [System.ServiceModel.OperationContract()]
         DataSet GetItemByID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);
         [System.ServiceModel.OperationContract()]
          DataSet GetCatalogUnit(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
       
        [System.ServiceModel.OperationContract()]
         int CreateInBoundTask(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerBarCode, string WorkFlowHeadTypeCode);

        [System.ServiceModel.OperationContract()]
        DataSet GetCatalogsQtyByCombinationTypeID(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string CombinationTypeID, string CatalogID);

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

    }
}
