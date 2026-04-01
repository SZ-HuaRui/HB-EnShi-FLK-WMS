using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class CONUSEDMATERHEAD
    {
        public String ConusedmaterheadId;
        public String SchemeId;
        public String MachineNumber;
        public String RunsGroup;
        public Int32 CheckField;
        public String ItemId;
        public DateTime CreateTime;
        public String Field1;
        public String Field2;
        public String Field3;
        public String Field4;
        public String Field5;
        //public override string ToString()
        //{
        //    return String.Format("订单号：{0} 顺序号：{1} 原始单据号：{2} 来源单据：{3}/{4} 目的地：{5}/{6}/{7} 批次：{8}/{9} BOM：{10} 类型：{11} 货主：{12} 发货时间：{13} 计划：{14} 承运人：{15}牌号ID：{16}牌号名：{17}",
        //        OrderNumber, OrderSequence, SourceBillNumber, CommonId, CommonNumber, TargetAddressId, TargetAddressType, TargetAddressText, LotId, LotNumber, BomCode, TypeCode, ShipperId, DeliverDate, SchemeId, DeliveryMan, ItemID, ItemName);
        //}
    }
}