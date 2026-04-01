using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Model
{/// <summary>
/// 公共模型，将公共字段放在此模型
/// </summary>
   public abstract class BaseModel
    {/// <summary>
     /// 主键ID
     /// </summary>
        public string t_id { get; set; }
        /// <summary>
        /// 集团公司ID
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 组织ID
        /// </summary>
        public string org_id { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public string warehouse_id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_date { get; set; }
        /// <summary>
        /// 创建人帐号 
        /// </summary>
        public string created_by { get; set; }
        /// <summary>
        /// 操作人IP
        /// </summary>
        public string modify_ip { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? modify_date { get; set; }
        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public string last_modified_by { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string act_status { get; set; }
    }
}
