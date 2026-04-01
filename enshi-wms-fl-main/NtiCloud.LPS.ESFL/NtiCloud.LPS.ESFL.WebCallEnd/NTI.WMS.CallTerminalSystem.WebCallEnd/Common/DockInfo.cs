using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    [Serializable]
    public class DockInfo : DockBase
    {
        /// <summary>
        /// 机台id
        /// </summary>
        public string machineId;
        /// <summary>
        /// 机台编码
        /// </summary>
        public string machineCode;
        /// <summary>
        /// 区段id
        /// </summary>
        public string sectionId;
        /// <summary>
        /// 区段编码（界面上显示托盘信息）
        /// </summary>
        public string sectionCode;
        /// <summary>
        /// 机台号
        /// </summary>
        public string MachineNumber;    
        /// <summary>
        /// 机台类型
        /// </summary>
        public string MachineType;      
        /// <summary>
        /// 机台名称
        /// </summary>
        public string MachineName;
        /// <summary>
        /// 任务号
        /// </summary>
        public string TaskNumber;
        /// <summary>
        /// 区段code
        /// </summary>
        public string ModelCode;
        public string PalletNumber;

        public string ItemSource;
    }

    [Serializable]
    public class DockBase
    {
        /// <summary>
        /// 站台编码(界面上显示的站台)
        /// </summary>
        public string DockNumber;
        /// <summary>
        /// 站台名
        /// </summary>
        public string DockName;
        /// <summary>
        /// 站台ID
        /// </summary>
        public string MachineDockID;
        /// <summary>
        /// 站台状态
        /// </summary>
        public string DockStatus;
    }

    public class Warehouse
    {
        public string ID;
        public string Name;
    }

    public class User
    {
        public string UserID;
        public string UserName;
        public string role;
        public string EquipmentNum;
    }

    public class Product
    {
        public string ProductNumber;
        public string ProductName;
    }

    public enum ReturnResult
    {
        None,
        Succeed,
        Fail
    }
}