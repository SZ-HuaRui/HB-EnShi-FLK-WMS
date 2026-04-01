using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Entity
{
    public class UpdateEquipmentStatusModel
    {
        public string WarehouseId { get; set; } = " ";
        public string ZoneId { get; set; } = " ";
        public string EquipmentNo { get; set; } = " ";
        public string EquipmentStatus { get; set; } = " ";
        public string ErrorCode { get; set; } = " ";
        public string ErrorDescription { get; set; } = " ";

    }
}
