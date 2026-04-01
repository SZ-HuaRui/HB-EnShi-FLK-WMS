using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class OrderDetail
    {
        public String OrderItemSequence;
        public String ItemId;
        public Decimal Qty;
        public String Memo;

        public override string ToString()
        {
            return String.Format("顺序：{0} 物料：{1} 数量（含单位）：{2} 备注：{3}", OrderItemSequence, ItemId, Qty, Memo);
        }
    }

    [Serializable()]
    public class OrderDetail_View
    {
        public String OrderItemSequence;
        public String DetailId;
        public String ItemId;
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Description;
        public Decimal Qty;
        public Decimal ItemId_ItemUnitRate;
        public String ItemId_ItemUnitName;
        public String Memo;
        public Decimal ProcessedQty;
        public String UserId;
        public DateTime CreateTime;
        public String ItemId_SameCode;//ADD by WZY 
    }

    [Serializable()]
    public class OrderDetail_View_Ex : OrderDetail_View
    {
        public string Paihao;
        public string XiPaihao;
        public Decimal TotalQty;//此牌号的总数量
    }
}
