using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.Equipment.Rfid
{
    public class RfidEquipment
    {
        public string EquipmentNo { get; set; }
        public EquipmentPosition EquipmentPosition { get; set; }
        public IRfid Rfid { get; set; }
        public Action<string> NotifyStringEvent { get; set; }

        public RfidEquipment(string equipmentNo,EquipmentPosition position,IRfid rfid)
        {
            EquipmentNo = equipmentNo;
            EquipmentPosition = position;
            Rfid = rfid;
            rfid.NotifyStringMessage = NotifyStringMessage;
        }

        public void NotifyStringMessage(string message)
        {
            message += $"设备号：{EquipmentNo}。";
            NotifyStringEvent?.Invoke(message);
        }

        public RfidEquipment()
        {

        }
    }

    public enum EquipmentPosition:int
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
        LeftRight = 4,
        UpDown = 5,
        NoDifference = 6
    }
}
