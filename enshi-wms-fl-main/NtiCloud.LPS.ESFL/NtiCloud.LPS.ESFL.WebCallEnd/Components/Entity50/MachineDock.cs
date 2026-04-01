using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    [Serializable()]
    public class MachineDockN
    {
        public String WarehouseID;
        public String MachineDockID;
        public String MachineNumber;
        public String MachineType;
        public String DockNumber;
        public String DockName;
        public String DockStatus;
        public String ModelCode;
        public String ZoneID;
        public String ModelID;
        public Int32 ItemSource;
        /// <summary>
        /// 对应machinedock.field1
        /// </summary>
        public String PerDockNumber;//field1
        public String UserID;
        public String MachineId;
    }
}
