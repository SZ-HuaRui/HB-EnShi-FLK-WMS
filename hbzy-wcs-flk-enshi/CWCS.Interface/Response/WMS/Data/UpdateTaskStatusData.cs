using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Response.WMS
{
    public class UpdateTaskStatusData
    {
        public string WarehouseId { get; set; }
        public string ZoneId { get; set; }
        public string TaskNum { get; set; }
        public string ContainerBarcode { get; set; }
    }
}
