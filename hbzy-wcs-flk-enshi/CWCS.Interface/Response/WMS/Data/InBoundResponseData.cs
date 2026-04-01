using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Response.WMS
{
    public class InBoundResponseData
    {
        public string WarehouseId { get; set; }  
        public string ZoneId { get; set; }
        public string TaskNum { get; set; }
        public string TaskStatus { get; set; }
        public string TaskType { get; set; }
        public string ContainerBarcode { get; set; }
        public string Wave_ID { get; set; }
        public string GroupNum { get; set; }
        public string SeqNum { get; set; }
        public string SourceAddress { get; set; }
        public string TargetAddress { get; set; }
        public string EquipmentNum { get; set; }
        public string LedText { get; set; }
        //增加字段，中间站台
        public string MiddleStation { get; set; }
    }
}
