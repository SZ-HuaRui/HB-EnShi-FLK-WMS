using System;
using System.Collections.Generic;

using System.Text;

namespace NTI.WMS.BR.RFManager
{
    [Serializable]
    public class CrossItem
    {         
         private string _itemID;
         private string _itemName;
         private decimal _qty;
         private decimal _Rate;

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

        private List<CorssItem_Struct> _ListItem = new List<CorssItem_Struct>();

        public List<CorssItem_Struct> ListItem
        {
            get { return _ListItem;}
        }

        

       
        public void Cross_Item(string itemid,string ItemName,decimal Qty,decimal Rate)
        {
            CorssItem_Struct C_Item = new CorssItem_Struct();

            C_Item.itemID = itemid;
            C_Item.itemName = ItemName;
            C_Item.qty = Qty;
            C_Item.Rate = Rate;

            _ListItem.Add(C_Item);
      }

        [Serializable]
        public struct CorssItem_Struct
        {
            public string itemID;
            public string itemName;
            public decimal qty;
            public decimal Rate;
        }
    }

}
