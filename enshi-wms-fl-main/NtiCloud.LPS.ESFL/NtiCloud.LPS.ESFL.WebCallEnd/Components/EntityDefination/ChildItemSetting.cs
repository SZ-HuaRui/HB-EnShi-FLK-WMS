using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class ChildItemSetting
    {
        public String WarehouseId;
        public String ChildId;       
        public String ChildName;
        public String Description;
        public String ItemId;
        public String StateCode;
        public String UserId;
        public DateTime CreateTime;
        public String Field1;
        public String Field2;
        public String Field3;

        public override string ToString()
        {
            return String.Format("ChildID：{0} 细牌号名称：{1} 描述：{2} 品规Id：{3} 状态：{4} ", ChildId, ChildName,Description, ItemId, StateCode);
        }
       
    }

    [Serializable()]
    public class ChildItemSetting_View
    {
        public String WarehouseId;
        public String ChildId;
        public String ChildName;
        public String Description;
        public String ItemId;
        public String ItemName;
        public String ItemCode;
        public String StateCode;
        public String UserId;
        public DateTime CreateTime;
        public String Field1;
        public String Field2;
        public String Field3;


    }
}
