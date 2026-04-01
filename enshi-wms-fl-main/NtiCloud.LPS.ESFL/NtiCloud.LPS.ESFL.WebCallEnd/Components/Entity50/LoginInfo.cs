using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.Entity50
{
    [Serializable()]
    public class LoginInfo
    {
        public String UserID { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String EquipmentNumber { get; set; }
        public String WarehouseID { get; set; }
        public String WarehouseText { get; set; }
        
        public override string ToString()
        {
            return "UserID:" + UserID + ",UserName:" + UserName + ",WarehouseText:" + WarehouseText;
        }
    }

    [Serializable()]
    public class Result
    {
        public Result()
        { }
        public Result(Int32 res, String message,String PK)
        {
            this.Res = res;
            this.Message = message;
            this.PK = PK;
        }
        public Int32 Res
        {
            set;
            get;
        }
        public String PK
        {
            get;
            set;
        }
        public String Message { get; set; }
    }

  
}
