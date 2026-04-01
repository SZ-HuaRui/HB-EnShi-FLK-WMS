using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Model
{
    /// <summary>
    /// 烟包替换
    /// </summary>
   public class ItemReplaceModel
    {
        /// <summary>
        /// 烟包替换主键ＩＤ
        /// </summary>
        public string t_id { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string item_code { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 烟包重量
        /// </summary>
        public decimal? spec { get; set; }
        /// <summary>
        /// 被替换烟包对应的容器ID
        /// </summary>
        public string container_id { get; set; }
        /// <summary>
        /// 被替换烟包对应的容器明细ID
        /// </summary>
        public string container_d_id { get; set; }
        /// <summary>
        /// 霉变日期
        /// </summary>
        public DateTime? create_date { get; set; }
    }


}
