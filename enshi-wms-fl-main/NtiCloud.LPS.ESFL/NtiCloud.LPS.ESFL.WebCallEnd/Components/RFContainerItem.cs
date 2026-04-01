using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface
{
    [Serializable]
    public class RFContainerItem
    {

        #region Model
        private string _billId;
        private string _warehouseid;
        private string _containerbarcode;
        private string _containerid;
        private string _bomid;
        private string _modelcode;
        private int _fullPallet;
        private DateTime _firstInTime;

        public DateTime FirstInTime
        {
            get { return _firstInTime; }
            set { _firstInTime = value; }
        }

        public int FullPallet
        {
            get { return _fullPallet; }
            set { _fullPallet = value; }
        }
        private List<Item_Struct> _itemList = new List<Item_Struct>();

        public List<Item_Struct> ItemList
        {
            get { return _itemList; }
        }

        /// <summary>
        /// 添加容器内的ITEM数据
        /// </summary>
        /// <param name="itemID">物料ID</param>
        /// <param name="itemName">物料名称</param>
        /// <param name="qty">数量</param>
        /// <param name="itemCode">物料编号</param>
        /// <param name="lotNumber">批次号</param>
        /// <param name="odd1">零头1</param>
        /// <param name="odd2">零头2</param>
        /// <param name="odd3">零头3</param>
        public void AddContainerDetail_ItemData(string itemID, string itemName, decimal qty, string itemCode, string lotNumber, decimal odd1, decimal odd2, decimal odd3, bool lockLotNumber)
        {
            Item_Struct item = new Item_Struct();
            item.itemID = itemID;
            item.itemName = itemName;
            item.itemCode = itemCode;
            item.qty = qty;
            item.lotnumber = lotNumber;
            item.odd1 = odd1;
            item.odd2 = odd2;
            item.odd3 = odd3;
            item.lockLotNumber = lockLotNumber;

            _itemList.Add(item);
        }

        /// <summary>
        /// 添加容器内的ITEM数据（有坏料原因）
        /// </summary>
        /// <param name="itemID">物料ID</param>
        /// <param name="itemName">物料名称</param>
        /// <param name="qty">数量</param>
        /// <param name="itemCode">物料编号</param>
        /// <param name="lotNumber">批次号</param>
        /// <param name="odd1">零头1</param>
        /// <param name="odd2">零头2</param>
        /// <param name="odd3">零头3</param>
        /// /// <param name="reasons">坏料原因</param>
        public void AddBadContainerDetail_ItemData(string itemID, string itemName, decimal qty, string itemCode, string lotNumber, decimal odd1, decimal odd2, decimal odd3, bool lockLotNumber, string reasons)
        {
            Item_Struct item = new Item_Struct();
            item.itemID = itemID;
            item.itemName = itemName;
            item.itemCode = itemCode;
            item.qty = qty;
            item.lotnumber = lotNumber;
            item.odd1 = odd1;
            item.odd2 = odd2;
            item.odd3 = odd3;
            item.lockLotNumber = lockLotNumber;
            item.reasons = reasons;

            _itemList.Add(item);
        }

        /// <summary>
        /// 添加容器内的ITEM数据
        /// </summary>
        /// <param name="containerDetailID">托盘明细ID</param>
        /// <param name="itemID">物料ID</param>
        /// <param name="itemName">物料名称</param>
        /// <param name="qty">数量</param>
        /// <param name="itemCode">物料编号</param>
        /// <param name="lotNumber">批次号</param>
        /// <param name="odd1">零头1</param>
        /// <param name="odd2">零头2</param>
        /// <param name="odd3">零头3</param>
        public void AddContainerDetail_ItemData(string containerDetailID, string itemID, string itemName, decimal qty, string itemCode, string lotNumber, decimal odd1, decimal odd2, decimal odd3, bool lockLotNumber)
        {
            Item_Struct item = new Item_Struct();
            item.containerDetailID = containerDetailID;
            item.itemID = itemID;
            item.itemName = itemName;
            item.itemCode = itemCode;
            item.qty = qty;
            item.lotnumber = lotNumber;
            item.odd1 = odd1;
            item.odd2 = odd2;
            item.odd3 = odd3;
            item.lockLotNumber = lockLotNumber;

            _itemList.Add(item);
        }

        public string BillID
        {
            get { return _billId; }
            set { _billId = value; }
        }

        public string Containerbarcode
        {
            get { return _containerbarcode; }
            set { _containerbarcode = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WAREHOUSEID
        {
            set { _warehouseid = value; }
            get { return _warehouseid; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CONTAINERID
        {
            set { _containerid = value; }
            get { return _containerid; }
        }

        public string Bomid
        {
            get { return _bomid; }
            set { _bomid = value; }
        }

        public string Modelcode
        {
            get { return _modelcode; }
            set { _modelcode = value; }
        }

        [Serializable]
        public struct Item_Struct
        {
            public string containerDetailID;
            public string itemID;
            public string itemName;
            public string itemCode;
            public decimal qty;
            public string lotnumber;
            public decimal odd1;
            public decimal odd2;
            public decimal odd3;
            public bool lockLotNumber;
            public string reasons;
        }

        #endregion Model
    }
}
