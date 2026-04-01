using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IContainer
    {
        IContainer AddDetailFinanceQty(string detailID, decimal qty, decimal? oldQty = null);

        IContainer AddDetailQty(string detailID, decimal qty, decimal? oldQty = null);

        IContainer AddDetailInQty(string detailID, decimal qty, decimal? oldQty = null);

        IContainer ClearDetail(string detailID);

        IContainer DeleteDetail(string detailID);
        IContainer InsertDetail(DataRow row);
        IContainer InsertHead(DataRow row);
        IContainer DeleteDetails(string containerId);

        DataRow GetHead(string headIDOrBarcode);
        DataRow GetHeadAll(string headIDOrBarcode);
        DataRow GetContainerH(string containerHID);

        DataRow GetContainerByBarcode(string barcode);

        DataRow[] GetHeadsByLocation(string locationID);

        DataRow[] GetDetailByHeadID(string headID);

        DataRow GetDetail(string detailID);

        //DataRow GetDetail(string headID, string itemID);

        //DataRow GetDetail(string headID, string itemID, string batchNo);

        DataRow GetDetail(string headID, string itemID,
            string batchNo = null, string inboundID = null);

        DataRow GetDetailByLocation(string locID, string itemID,
            string batchNo = null, string inboundID = null);

        DataRow[] GetDetailsAll(string containerID);

        DataRow[] GetDetails(string headID, string itemID);

        DataRow[] GetDetails(string headID, string itemID = null, string batchNo = null, string inboundID = null);

        DataRow[] GetDetailsByLocation(string locationID, string itemID = null);

        //DataRow[] GetInventoryDetails(string locationID, string itemID = null);
        DataRow[] GetInventoryDetails(string locationID,
            string itemID = null, string batchNo = null);

        DataRow[] GetInventoryDetailsByContainer(string containerID);

        DataRow[] GetInventoryDetailsByLocation(string locationID);

        DataRow[] GetInventoryDetails(DataRow locRow);

        decimal GetInventory(string containerID, string itemID);

        decimal GetInventoryByLocation(string locationID, string itemID);

        DataRow[] Get4StockingOut(ContainerDetailModel detailModel);

        IContainer Insert(ContainerHeadModel model);

        IContainer Insert(ContainerDetailModel model);

        IContainer UpdateDetailCountingState(string containerDetailID, string countingState);

        IContainer UpdateDetailCountingStateByContainerID(string containerID, string countingState);

        /// <summary>
        /// 修改容器明细物料数量
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="qty">物料数量</param>
        /// <returns></returns>
        IContainer UpdateDetailQty(string containerDetailID, decimal qty);

        /// <summary>
        /// 修改容器明细待出量
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="outQty">待出量</param>
        /// <returns></returns>
        IContainer UpdateDetailOutQty(string containerDetailID, decimal outQty);

        /// <summary>
        /// 修改容器明细待入量
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="inQty">待入量</param>
        /// <returns></returns>
        IContainer UpdateDetailInQty(string containerDetailID, decimal inQty);

        /// <summary>
        /// 修改容器明细质检状态
        /// </summary>
        /// <param name="containerDetailID">容器明细id</param>
        /// <param name="qcStatus">质检状态</param>
        /// <returns></returns>
        IContainer UpdateDetailQCStatus(string containerDetailID, string qcStatus);

        /// <summary>
        /// 修改容器主档是否允许呼叫
        /// </summary>
        /// <param name="containerHeadID">容器id</param>
        /// <param name="isAllowCalled">是否允许呼叫，0：不允许，1：允许</param>
        /// <returns></returns>
        IContainer UpdateHeadIsAllowCalled(string containerHeadID, string isAllowCalled);

        IContainer UpdateHeadActStatus(string id, string actStatus, string oldActStatus = null);

        IContainer UpdateHeadIsOut(string id, string value, string oldValue = null);

        IContainer UpdateDetail(DataRow row);

        /// <summary>
        /// 修改容器明细删除标记
        /// </summary>
        /// <param name="detailID">容器明细id</param>
        /// <param name="actStatus">删除标记（1表示不删除；0表示删除）</param>
        /// <returns></returns>
        IContainer UpdateDetailActStatus(string detailID, string actStatus);

        /// <summary>
        /// 修改容器明细首次入库时间
        /// </summary>
        /// <param name="headID">容器主档id</param>
        /// <param name="dt">首次入库时间</param>
        /// <returns></returns>
        IContainer UpdateDetailFirstInDate(string headID, DateTime dt);
        /// <summary>
        /// 修改容器主档首次入库时间
        /// </summary>
        /// <param name="headID">容器主档id</param>
        /// <param name="dt">首次入库时间</param>
        /// <returns></returns>
        IContainer UpdateContainerHFirstInDate(string headID, DateTime dt);

        IContainer UpdateDetailOwnership(string containerID, string shipperID, string orderNo);

        IContainer UpdateHeadCheckState(string headID, string checkState);

        IContainer UpdateHeadCountingState(string headID, string countingState);

        IContainer UpdateHeadLocation(string headID, string locationID, string oldID = "");


        IContainer UpdateHeadPackingType(string headID, string packingType, string oldPackingType = null);

        IContainer TakeItemOut(string containerID, string itemBarcodes);

        IContainer AddDetailOutQty(string detailID, decimal outQty, decimal? oldQty = null);

        IContainer AddContainerOutQty(string containerID, decimal outQty, decimal? oldQty = null);

        decimal PreparePutaway(ContainerDetailModel detailModel,
            decimal leftQty, decimal locationCapacity);

        void CancelPutawayPreparation(string containerDetailID, decimal qty);

        IContainer Prepare4StockingOut(string containerDetailID, decimal qty);

        IContainer Prepare4StockingOut(string locationID, ContainerDetailModel detail);

        IContainer CancelStockingOut(string containerDetailID, decimal qty);

        IContainer StockIn(DataRow ctnDtlRow, decimal qty, decimal financeQty);

        IContainer StockOut(DataRow ctnDtlRow, decimal qty, decimal financeQty);


        public WebJsonInfo AddContainerH(IDBContextProvider dbW, UserLoginInfo user, ContainerModel ctn);
        public WebJsonInfo DeleteContainerH(IDBContextProvider dbW, UserLoginInfo user, string ctnid);
        public bool DeleteHead(string ctnid);
        public WebJsonInfo AddContainerD(IDBContextProvider dbW, UserLoginInfo user, ContainerDModel ctndetail);
        public WebJsonInfo DeleteContainerD(IDBContextProvider dbW, UserLoginInfo user, string ctndetailid);
        DataRow GetOrNewVirtualHead(DataRow locationRow);
        /// <summary>
        /// 判断货位在容器表中是否存在
        /// </summary>
        /// <param name="dbR"></param>
        /// <param name="user"></param>
        /// <param name="locationcode"></param>
        /// <returns></returns>
        public bool CheckLocationExis(IDBContextProvider dbR, UserLoginInfo user, string locationcode);
        /// <summary>
        /// 获取货位对应的容器ID
        /// </summary>
        /// <param name="dbR"></param>
        /// <param name="user"></param>
        /// <param name="locationcode"></param>
        /// <returns></returns>
        public string getContainerIdByLocation(string locationcode);
        /// <summary>
        /// 更新收货容器的修改人信息
        /// </summary>
        /// <param name="dbW"></param>
        /// <param name="user"></param>
        /// <param name="ctnid"></param>
        /// <returns></returns>
        public WebJsonInfo UpdateContainerModifyInfo(IDBContextProvider dbW, UserLoginInfo user, string ctnid);

        /// <summary>
        /// 检查容器明细中该物料+批次是否存在
        /// </summary>
        /// <param name="ctnId">容器ID</param>
        /// <param name="itemid">物料编码ID</param>
        /// <param name="wmsLot">WMS批次</param>
        /// <returns></returns>
        public bool CheckContainerDetailExists(IDBContextProvider dbR, UserLoginInfo user, string ctnId, string itemid, string wmsLot);

        /// <summary>
        /// 获取容器行ID
        /// </summary>
        /// <param name="dbR"></param>
        /// <param name="user"></param>
        /// <param name="ctnId"></param>
        /// <param name="itemid"></param>
        /// <param name="wmsLot"></param>
        /// <returns></returns>
        public string getContainerDetailId(IDBContextProvider dbR, UserLoginInfo user, string ctnId, string itemid, string wmsLot);

        /// <summary>
        /// 长生WMS批次
        /// </summary>
        /// <param name="ctndetail"></param>
        /// <returns></returns>
        public string GenerateWmsLot(ContainerDModel ctndetail);

        public bool UpdateContainerLocation(string containerid, string locationid);
        /// <summary>
        /// 更新容器满载标记
        /// </summary>
        /// <param name="containerid">容器ID</param>
        /// <param name="isFull">满载标记</param>
        /// <returns></returns>
        public bool UpdateContainerIsFull(string containerid, string isFull);

        /// <summary>
        /// 增加带入量
        /// </summary>
        /// <param name="dbW"></param>
        /// <param name="user"></param>
        /// <param name="ctndetailid"></param>
        /// <param name="addQty"></param>
        /// <returns></returns>
        public WebJsonInfo AddInQty(IDBContextProvider dbW, UserLoginInfo user, string ctndetailid, decimal addQty);

    }
}
