using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class WaveResultModel
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
        /// 波次头ID
        /// </summary>
        public string WaveHeadID { get; set; }

        /// <summary>
        /// 优先拣选面
        /// </summary>
        public string PriorGroup { get; set; }

        /// <summary>
        /// 拣选面代码
        /// </summary>
        public string LocgpCode { get; set; }

        /// <summary>
        /// 拣选面ID
        /// </summary>
        public string LocgpID { get; set; }

        /// <summary>
        /// 第一拣选面规格数
        /// </summary>
        public decimal? P1ItemCount { get; set; }

        /// <summary>
        /// 第一拣选面数量
        /// </summary>
        public decimal? P1Qty { get; set; }

        /// <summary>
        /// 第一拣选面规格数
        /// </summary>
        public decimal? P2ItemCount { get; set; }

        /// <summary>
        /// 第二拣选面数量
        /// </summary>
        public decimal? P2Qty { get; set; }

        /// <summary>
        /// 其它区域规格数
        /// </summary>
        public decimal? OtherItemCount { get; set; }

        /// <summary>
        /// 其它区域数量
        /// </summary>
        public decimal? OtherQty { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建人账号
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
        /// 最后修改人账号
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string ActStatus { get; set; }

    }
}
