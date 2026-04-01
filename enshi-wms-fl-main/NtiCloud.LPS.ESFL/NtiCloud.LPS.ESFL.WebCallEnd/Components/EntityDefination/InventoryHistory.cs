using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class InventoryHistoryHead_Edit
    {
        public String Description;
        public String TypeCode;

        public override string ToString()
        {
            return String.Format("类型：{0} 描述：{1}", TypeCode, Description);
        }
    }

    [Serializable()]
    public class InventoryHistoryHead
    {
        public String HistoryId;
        public DateTime CreateTime;
        public String Description;
        public String UserId;
        public String TypeCode;
        public String TypeText;
    }

    [Serializable()]
    public class InventoryHistoryDetail
    {
        public String Id;
        public String Text;
        public String Value;
        public String TypeText;
        public String TypeCode;
        public Decimal StatisticValue1;
        public String StatisticUnit1;
        public Decimal StatisticValue2;
        public String StatisticUnit2;
        public Decimal InventoryValue;
        public Decimal UnitRate;
        public String UnitText;
        public String EditText;
        public String ItemCode;
        public String ItemSameCode;  //唯一码
    }

    [Serializable()]
    public class InventoryHistoryDetail_Edit
    {
        public Decimal InventoryValueWithRate;
        public Decimal StatisticValue1;
        public String StatisticUnit1;
        public Decimal StatisticValue2;
        public String StatisticUnit2;

        public override string ToString()
        {
            return String.Format("值(带单位转换率)：{0:0.###} 统计值1：{1:0.###} {2} 统计值2：{3:0.###} {4}",
                InventoryValueWithRate, StatisticValue1, StatisticUnit1, StatisticValue2, StatisticUnit2);
        }
    }
}
