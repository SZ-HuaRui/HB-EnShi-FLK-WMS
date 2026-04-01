using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class WaveTask_View
    {
        public String WaveId;
        public String WaveDetailId;
        public String TaskTypeCode;
        public String TaskTypeCode_Description;
        public String TaskExecutorTypeCode;
        public String TaskExecutorTypeCode_Description;
        public String TaskNumber;
        public String ExecutiveEquipmentId;
        public String ExecutiveEquipmentId_Number;
        public String GroupNumber;
        public String SequenceNumber;
        public Int32 Priority;
        public String ContainerId;
        public String ContainerBarCode;
        public String ItemId;
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Description;
        public Decimal ItemId_UnitRate;
        public String ItemId_UnitName;
        public Decimal OperationQty;
        public String SourceAddressId;
        public String SourceAddressType;
        public String SourceAddressType_Description;
        public String SourceAddressText;
        public String TargetAddressId;
        public String TargetAddressType;
        public String TargetAddressType_Description;
        public String TargetAddressText;
        public Int32 Executable;
        public String DependenceTaskNumber;
        public String LEDText;
    }

    [Serializable()]
    public enum WaveTaskTypeForInsert
    {
        Outbound, Fill, Pick
    }
}
