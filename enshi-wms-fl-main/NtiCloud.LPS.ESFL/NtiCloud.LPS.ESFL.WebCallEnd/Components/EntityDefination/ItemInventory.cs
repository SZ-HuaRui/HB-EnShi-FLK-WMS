using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class ItemInventory
    {
        public String ItemId;

        public Decimal UsableWithoutOutside;
        public Decimal Outside;
        public Decimal OutsideIncoming;
    }
}
