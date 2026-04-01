using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class Move
    {
        public String WarehouseId;
        public String MoveId;
        public String SourceAddressId;
        public String TargetAddressId;
        public String UserId;
        public DateTime CreateTime;
        public DateTime UpdateTime;
        
    }
}
