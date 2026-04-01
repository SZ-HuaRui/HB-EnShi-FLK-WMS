using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Interface.Request
{
    public class EquipmentStatusParam
    {
        public string WarehouseID { get; set; }
        public string ZoneID { get; set; }
        //设备类型
        public string StaType { get; set; }
        //设备状态
        public string StaStatus { get; set; }
        //故障代码
        public string STA_Errorcode { get; set; }
        //故障信息
        public string STA_Errorinfo { get; set; }
        //设备编号
        public string EquipmentNo { get; set; }

    }
}
