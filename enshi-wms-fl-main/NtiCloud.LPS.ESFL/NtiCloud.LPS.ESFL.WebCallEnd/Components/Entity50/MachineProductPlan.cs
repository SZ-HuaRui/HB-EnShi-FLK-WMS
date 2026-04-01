using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    [Serializable]
    public class MachineProductPlan
    {
        public string warehouseid;
        public string machineequipmentid;
        public string machinenumber;
        public string schemeid;
        public string itemid;
        public string statecode;
        public DateTime createdate;
        public DateTime startdate;
        public DateTime enddate;
        public decimal qty;
        public decimal qty1;
        public decimal qty2;
        public string memo;
        public string userid;
        public DateTime createtime;
        public string field1;
        public string field2;
        public string field3;
        public string field4;
        public string field5;
        public string lotNumber;
        public string version;
        public string ItemCode;
        public string schemeNumber;
    }
}
