using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    public class DockN
    {
        public String WarehouseID;
        public String DockID;
        public String DockNumber;
        public String TypeCode;
        public Decimal Length;
        public Decimal Width;
        public Decimal Height;
        public String UserID;
    }
    [Serializable]
    public class DockInfo : DockBase
    {
        public string MachineNumber;    //机台号
        public string MachineType;      //机台类型
        //public int HasLGV;              //是否产生LGV任务 
        public string MachineName;
        public string TaskNumber;       //任务号

        //public string ContainerID;
        public string ModelCode;    //区段
        //public string ContainerBarCode;

        //public string ProcessBOMTypeID;

        public string PalletNumber;

        public string ItemSource;
        //public string BillID;
        //public bool IsExcuting = false;
    }
    [Serializable]
    public class DockBase
    {
        public string DockNumber;   //站台位
        public string DockName;     //站台位名
        public string MachineDockID;    //站台位ID
        public string DockStatus;       //站台位状态
    }
}
