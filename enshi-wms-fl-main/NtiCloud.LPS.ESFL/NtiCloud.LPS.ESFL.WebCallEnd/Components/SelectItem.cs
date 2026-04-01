using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WMSInterface
{
    [Serializable]
    public class SelectItem
    {
        private string _itemID;
        private string _itemName;
        private decimal _qty;
        private decimal _Rate;
        private string _itemcode; //物料编码
        private string _Description; //描述
        private string _Bomid;   
        private int _levelnumber;
        private decimal _QtyPercent;
        private string _Field1;
        private string _Field2;
        private string _Field3;

        #region 定义属性
        public string itemid
        {
            set { _itemID = value; }
            get { return _itemID; }
        }

        public string itemname
        {
            set { _itemName = value; }
            get { return _itemName; }
        }

        public decimal qty
        {
            set { _qty = value; }
            get { return _qty; }
        }

        public decimal rate
        {
            set { _Rate = value; }
            get { return _Rate; }
        }

        public string itemcode
        {
            set { _itemcode = value; }
            get { return _itemcode;  }
        }

        public string Description
        {
            set { _Description=value;}
            get { return _Description;}
        }

        public string Bomid
        {
            set { _Bomid = value; }
            get { return _Bomid; }
        }

        public int levelnumber
        {
            set { _levelnumber = value; }
            get { return _levelnumber; }
        }

        public decimal QtyPercent
        {
            set { _QtyPercent = value; }
            get { return _QtyPercent; }
        }

        public string Field1
        {
            set { _Field1 = value; }
            get { return _Field1; }
        }

        public string Field12
        {
            set { _Field2 = value; }
            get { return _Field2; }
        }

        public string Field3
        {
            set { _Field3 = value; }
            get { return _Field3; }
        }

        private List<BomItem_Struct> _ListItem = new List<BomItem_Struct>();

        public List<BomItem_Struct> ListItem
        {
            get { return _ListItem; }
        }
        #endregion
       
        public void Bom_ItemInsert(string BOMID, int LevelNumber, string ItemID, string ItemName, decimal Qty, decimal sQtyPercent, string Remark, string sField1, string sField2, string sField3)
        {
            BomItem_Struct C_Item = new BomItem_Struct();
            C_Item.Bomid = BOMID;
            C_Item.levelnumber = LevelNumber;
            C_Item.itemID = ItemID;
            C_Item.itemName = ItemName;
            C_Item.qty = Qty;
            C_Item.QtyPercent = sQtyPercent;
            C_Item.Description = Remark;
            C_Item.Field1 = sField1;
            C_Item.Field2 = sField2;
            C_Item.Field3 = sField3;

            _ListItem.Add(C_Item);
        }

        public  void Bom_Item_Insert(string bomid,string ItemID, string ItemName, string Remark)
        {
            BomItem_Struct C_Item = new BomItem_Struct();

            C_Item.Bomid = bomid;
            C_Item.itemID = ItemID;
            C_Item.itemName = ItemName;           
            C_Item.Description = Remark;
           

            _ListItem.Add(C_Item);
        }



        [Serializable]
        public struct BomItem_Struct
        {
            public string itemID;
            public string itemName;
            public string itemcode;
            public string Description;
            public decimal qty;
            public decimal Rate;
            public string Bomid;
            public int levelnumber;
            public decimal QtyPercent;
            public string Field1;
            public string Field2;
            public string Field3;
        }
    }
}
