using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request.WMS
{
    /// <summary>
    /// 更新目标地址参数
    /// </summary>
    public class UpdateTargetAddressParam
    {
        public string WarehouseID { get; set; }
        public string ZoneID { get; set; }
        //WMS任务号
        public string TaskNum { get; set; }
       
    }
}
