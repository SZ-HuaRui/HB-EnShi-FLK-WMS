using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    public class UpdateTaskStatusParam
    {
        public string WarehouseID { get; set; }
        public string ZoneID { get; set; }
        //WMS任务号
        public string TaskNum { get; set; }
        //1,完成，3部分完成
        public string TaskStatus { get; set; }

     
    }
}
