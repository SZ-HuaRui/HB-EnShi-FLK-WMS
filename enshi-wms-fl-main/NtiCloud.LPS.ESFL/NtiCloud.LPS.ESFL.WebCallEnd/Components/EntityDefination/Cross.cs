using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class CrossDetail
    {
        public String WarehouseID;
        public String CrossDetailID;
        public String CrossID;
        public String ItemID;
        public String LotNumber;
        public String ReceivingID;
        public Decimal Qty;
        public Decimal CrossQty;
        public String StateCode;
        public String UserID;
        public String ItemSupplier;
    }
}
