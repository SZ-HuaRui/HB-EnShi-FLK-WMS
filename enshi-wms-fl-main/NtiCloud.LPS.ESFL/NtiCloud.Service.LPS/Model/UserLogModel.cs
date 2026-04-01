using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class UserLogModel
    {

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 集团公司ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string OrgID { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 日志说明
        /// </summary>
        public string LogText { get; set; }

        /// <summary>
        /// 长日志文本
        /// </summary>
        public string LogTextLong { get; set; }

        /// <summary>
        /// 对象名称
        /// </summary>
        public string CommonName { get; set; }

        /// <summary>
        /// 严重等级
        /// </summary>
        public decimal? CriticalLevel { get; set; }

        /// <summary>
        /// 客户号
        /// </summary>
        public string ClientNumber { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 日志号
        /// </summary>
        public string LogNo { get; set; }

        /// <summary>
        /// 父日志号
        /// </summary>
        public string ParrentLogNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建人帐号
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 操作人IP
        /// </summary>
        public string ModifyIP { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 最后修改人帐号
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string ActStatus { get; set; }

    }
}
