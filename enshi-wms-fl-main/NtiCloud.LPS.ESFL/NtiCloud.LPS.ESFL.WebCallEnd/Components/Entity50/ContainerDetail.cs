using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    [Serializable]
    public class ContainerDetailN
    {
        public String WarehouseID;
        public String ContainerDetailID;
        public String ContainerID;
        public String ItemID;
        public Decimal Qty;
        public String LotNumber;
        public String CheckState;
        public String FullStatus;
        public String UserID;
        public String ItemPosition;
        public String ODD1;
        public String ODD2;
        public String ODD3; 
        public String Field1;
        public String Field2;
        public String Field3;
        public String ItemName;
        public String ItemBarcode1;
        public String ItemBarcode2;
        public Decimal EachPalletFillQty;
        public String Itemsupplier;
        public Decimal ItemRate;
    }
}
