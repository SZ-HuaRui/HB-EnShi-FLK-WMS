using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Model
{
    public class WarehouseModel:BaseModel
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string warehouse_name { get; set; }
        /// <summary>
        /// 仓库代码
        /// </summary>
        public string warehouse_code { get; set; }
        /// <summary>
        /// 仓库描述
        /// </summary>
        public string warehouse_desc { get; set; }
        /// <summary>
        /// 仓库性质
        /// </summary>
        public string warehouse_property { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string postcode { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public string warehouse_type { get; set; } 

    }
}
