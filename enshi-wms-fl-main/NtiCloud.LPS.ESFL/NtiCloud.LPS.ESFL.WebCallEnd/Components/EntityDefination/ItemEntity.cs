using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WMSInterface.EntityDefination
{
    public class ItemEntity
    {
        private string _ItemID;
        private string _ItemCode;
        private string _ItemName;
        private decimal _Itemqty;
        private string _ItemLotNumber;

        public string ItemID
        {
            set { _ItemID = value; }
            get { return _ItemID; }
        }

        public string ItemCode
        {
            set { _ItemCode = value; }
            get { return _ItemCode; }
        }

        public string ItemName
        {
            set { _ItemName = value; }
            get { return _ItemName; }
        }

        public decimal Itemqty
        {
            set { _Itemqty = value; }
            get { return _Itemqty; }
        }

        public string ItemLotNumber
        {
            set { _ItemLotNumber = value; }
            get { return _ItemLotNumber; }
        }

        private List<Item_Struct> _ListItem = new List<Item_Struct>();

        public List<Item_Struct> ListItem
        {
            get { return _ListItem; }
        }

        
        public void Cross_Item(string itemid, string itemcode,string ItemName, decimal itemQty, string itemlotnumber)
        {
            Item_Struct StItem = new Item_Struct();

            StItem.itemID = itemid;
            StItem.itemcode = itemcode;
            StItem.itemName = ItemName;
            StItem.qty = itemQty;
            StItem.lotnumber = itemlotnumber;

            _ListItem.Add(StItem);
        }

        [Serializable]
        public struct Item_Struct
        {
            public string itemID;
            public string itemcode;
            public string itemName;
            public decimal qty;
            public string lotnumber;
        }
    }

    //public class InsertItem
    //{
    //    public string ItemId; //物料ID
    //    public string ItemCode;//物料编码
    //    public string ItemName;//物料名称
    //    public Decimal ItemQty;//数量
    //    public string ItemLotNumber;//批次号
    //}
}
