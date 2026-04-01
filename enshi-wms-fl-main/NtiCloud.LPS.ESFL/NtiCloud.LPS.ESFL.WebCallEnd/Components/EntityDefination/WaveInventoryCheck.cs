using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class WaveInventoryCheck
    {
        public String ItemId;
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Description;
        public Decimal ItemId_UnitRate;
        public String ItemId_UnitName;

        public Decimal Required;
        public Decimal OutsideRequirement;

        public Decimal UsableWithoutOutside;
        public Decimal Outside;
        public Decimal OutsideIncoming;

        public Decimal FillingRequired;
        public Decimal FillingQty;
        public Decimal PickingQty;
    }
}
