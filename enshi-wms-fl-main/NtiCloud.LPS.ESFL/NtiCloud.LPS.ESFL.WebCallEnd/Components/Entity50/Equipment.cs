using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    [Serializable]
    public class EquipmentN
    {
        //WarehouseID,EquipmentID,EquipmentNumber,Description,Length,Width,Height,
        //    StateCode,TypeCode,WCSTypeCode,ZoneID,ContainerInfoID,ContainerInfoBarCode,Qty,UserID,CreateTime,Field1,Field2,Field3,Field4,Field5
        public String WarehouseID;
        public String EquipmentID;
        public String EquipmentNumber;
        public String Description;
        public String Length;
        public String Width;
        public String Height;
        public String StateCode;
        public String TypeCode;
        public String WCSTypeCode;
        public String ZoneID;
        public String ContainerInfoID;
        public String ContainerInfoBarCode;
        public String Qty;
        public String UserID; 
    }

    [Serializable]
    public class EquipmentLineN
    {
        public String WarehouseID;
        public String EquipmentLineID;
        public String ZoneID;
        public String EquipmentID;
        public String Line;
        public String UserID; 
    }
}
