using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    [Serializable]
    public class LocationN
    {
//        :WarehouseID,:ZoneID,:LocationID,:LocationNumber,:LocationLine,:LocationColumn,:LocationLayer,:LocationDepth,:LocationFlag,
//:Length,:Width,:Height,null,'',0,'',null,'','',null,null,'','',null,'',:ParentID,:LocationState,:UserID,:CreateTime,:Field1,:Field2,
//:Field3,:Field4,:Field5
        public String WarehouseID;
        public String LocationID;
        public String ZoneID;
        public String LocationNumber;
        public Int32 LocationLine;
        public Int32 LocationColumn;
        public Int32 LocationLayer;
        public Int32 LocationDepth;
        public String LocationFlag;
        public Decimal Length;
        public Decimal Width;
        public Decimal Height;
        public String ParentID;
        public String LocationState;
        public String UserID;
        public String Field1;
        public String Field2;
        public String Field3;
        public String Field4;
        public String Field5;
    }
}
