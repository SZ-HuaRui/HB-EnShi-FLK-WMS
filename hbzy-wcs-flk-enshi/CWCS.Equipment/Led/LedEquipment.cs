using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Led
{
    public class LedEquipment
    {
        public string EquipmentNo { get; set; }
        public ILed Led { get; set; }
        public LedEquipment(string equipmentNo,ILed led)
        {
            EquipmentNo = equipmentNo;
            Led = led;
        }
        public LedEquipment()
        {

        }
    }
}
