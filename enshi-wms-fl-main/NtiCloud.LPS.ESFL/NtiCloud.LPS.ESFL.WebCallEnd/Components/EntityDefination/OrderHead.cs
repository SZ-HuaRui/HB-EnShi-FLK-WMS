using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class OrderHead
    {
        public String OrderSequence;
        public String OrderNumber;
        public String SourceBillNumber;
        public String CommonId;
        public String CommonNumber;
        public String TargetAddressId;
        public String TargetAddressType;
        public String TargetAddressText;
        public Boolean TargetAddressChangeable;
        public String LotId;
        public String LotNumber;
        public String BomCode;
        public String TypeCode;
        public String ShipperId;
        public DateTime DeliverDate;
        public String SchemeId;
        public String DeliveryMan;
        public String ItemID;
        public String ItemName;
        public String OrderId;

        public override string ToString()
        {
            return String.Format("订单号：{0} 顺序号：{1} 原始单据号：{2} 来源单据：{3}/{4} 目的地：{5}/{6}/{7} 批次：{8}/{9} BOM：{10} 类型：{11} 货主：{12} 发货时间：{13} 计划：{14} 承运人：{15}牌号ID：{16}牌号名：{17}",
                OrderNumber, OrderSequence, SourceBillNumber, CommonId, CommonNumber, TargetAddressId, TargetAddressType, TargetAddressText, LotId, LotNumber, BomCode, TypeCode, ShipperId, DeliverDate, SchemeId, DeliveryMan,ItemID,ItemName);
        }
    }

    [Serializable()]
    public class OrderHead_View
    {
        public String OrderId;
        public String OrderSequence;
        public String OrderNumber;
        public String SourceBillNumber;
        public String CommonId;
        public String CommonNumber;
        public String TargetAddressId;
        public String TargetAddressType;
        public String TargetAddressType_Text;
        public String TargetAddressText;
        public Boolean TargetAddressChangeable;
        public String LotId;
        public String LotNumber;
        public String BomCode;
        public DateTime CreateDate_DateTime;
        public String TypeCode;
        public String TypeCode_Text;
        public String ShipperId;
        public String ShipperId_Text;
        public String StateCode;
        public String StateCode_Text;
        public String UserId;
        public DateTime DeliverDate_DateTime;
        public String SchemeId;
        public String DeliveryMan;
        public String ItemID;
        public String ItemName;

        
    }

    [Serializable()]
    public class OrderHead_View_ForWave : OrderHead_View
    {
        public Decimal TotalQtyWithoutRate;
        public Decimal TotalProcessedQtyWithoutRate;
        public String ShipperId_Grade;
        public String ShipperId_Address;


    }

    [Serializable()]
    public class OrderHead_View_Ex : OrderHead_View
    {
        public bool IsStandard;


    }

    [Serializable()]
    public class OrderHead_Ex : OrderHead
    {
        public List<OrderDetail> Details = new List<OrderDetail>();
    }


}
