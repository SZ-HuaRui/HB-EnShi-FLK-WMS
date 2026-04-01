using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Response.WMS
{
    public class UpdateTargetAddressData
    {
        public string WarehouseId { get; set; }
        public string ZoneId { get; set; }
        public string TaskNum { get; set; }
        public string TargetAddress { get; set; }
    }
}
