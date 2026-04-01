using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class Container_View
    {
        public String WarehouseId;
        public String ContainerId;
        public String ContainerBarCode1;
        public String ContainerBarCode2;
        public String ContainerType;
        public String BomId;
        public String ModelCode;
        public String LotNumber;
        public Int32 FullPallet;
        public Decimal Qty;
        public String Code;
        public String Version;
        public DateTime? CreateDate;
        public DateTime? ModifyDate;
        public String Description;
        public String CheckState;
        public String CheckState_Description;
        public String Source;
        public String ReturnUserId;
        public String Destination;
        public DateTime? FirstInTime;
        public String Owner;
        public DateTime? RunTime;
        public Decimal Length;
        public Decimal Width;
        public Decimal Height;
        public Decimal Weight;
        public Decimal ContainerWeight;
        public Decimal Field5;
        public Collection<ContainerDetail_View> Details = new Collection<ContainerDetail_View>();
    }

    [Serializable()]
    public class ContainerDetail_View
    {
        public String ContainerDetailId;
        public String ItemId;
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Description;
        public Decimal ItemId_ItemUnitRate;
        public String ItemId_ItemUnitName;
        public String ItemId_SameCode;//Add by wzy 2010/06/24
        public String ItemBarCode1;
        public String ItemBarCode2;
        public String ItemPosition;
        public Decimal Qty;
        public String LotNumber;
        public String CheckState;
        public String CheckState_Description;
        public Int32 FullStatus;
        public Decimal Odd1;
        public Decimal Odd2;
        public Decimal Odd3;
        public String OutboundState;
        public DateTime? OutboundDate;
    }
}
