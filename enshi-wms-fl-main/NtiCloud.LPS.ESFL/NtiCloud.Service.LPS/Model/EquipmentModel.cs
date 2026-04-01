using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Model
{
   public class EquipmentModel:BaseModel
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string zone_id { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string equipment_code { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string equipment_name { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string equipment_status { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string equipment_type { get; set; }
        /// <summary>
        /// 设备用途
        /// </summary>
        public string equipment_usage { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public string is_enabled { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; } 
    }
}
