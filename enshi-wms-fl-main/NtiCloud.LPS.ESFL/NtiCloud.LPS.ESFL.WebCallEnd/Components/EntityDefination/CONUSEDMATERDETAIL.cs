using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class CONUSEDMATERDETAIL
    {
        public String ConusedmaterdetailId;
        public String ConusedmaterheadId;
        public String ItemId;
        public String ItemCode;
        public Decimal CallQty;
        public Decimal ReturnQty;
        public Decimal AcceptQty;
        public Decimal HandoutQty;
        public Decimal StatisticalQty;
        public DateTime CreateTime;
        public String ShipperId;
        public String Field1;
        public String Field2;
        public String Field3;
        public String Field4;
        public String Field5;
        public DateTime ConfirmTime;
    }
}