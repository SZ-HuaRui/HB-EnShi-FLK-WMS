using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class WaveDetail : List<WaveDetailItem>
    { }

    [Serializable()]
    public class WaveDetailItem
    {
        public String OrderDetailId;
        public Decimal InWaveQty;
    }

    [Serializable()]
    public class WaveDetail_View
    {
        public String WaveHeadId;
        public String OrderSequence;
        public String DetailSequence;
        public String WaveDetailId;
        public String OrderId;
        public String OrderId_Number;
        public String OrderId_TargetAddressId;
        public String OrderId_TargetAddressType;
        public String OrderId_TargetAddressType_Text;
        public String OrderId_TargetAddressText;
        public Boolean OrderId_TargetAddressChangeable;
        public String OrderId_LotId;
        public String OrderId_LotNumber;
        public String OrderId_BomCode;
        public String OrderId_TypeCode;
        public String OrderId_TypeCode_Text;
        public String OrderDetailId;
        public Decimal OrderDetailId_NotProcessedQty;
        public String ItemId;
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Samecode;//add by wzy
        public String ItemId_Description;
        public Decimal InWaveQty;
        public Decimal InWaveProcessedQty;
        public Decimal FinishedQty;
        public Decimal ItemId_ItemUnitRate;
        public String ItemId_ItemUnitName;
        public String UserId;
        public DateTime CreateTime;
        public String OrderHead_Sequence;
        public String OrderDetail_Sequence;
        public Decimal FillRequested;
    }
}
