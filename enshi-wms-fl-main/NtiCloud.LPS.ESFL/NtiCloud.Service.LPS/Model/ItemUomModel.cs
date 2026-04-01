using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class ItemUomModel
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
        /// 物料ID，为空表示用物料种类来定义单位
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 物料种类ID，为空表示用标签来定义单位
        /// </summary>
        public string ItemCatalogID { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 标准单位ID
        /// </summary>
        public string UomID { get; set; }

        /// <summary>
        /// 下级单位ID
        /// </summary>
        public string ChildID { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UomName { get; set; }

        /// <summary>
        /// 单位描述
        /// </summary>
        public string UomDesc { get; set; }

        /// <summary>
        /// 单位种类，同标准单位表
        /// </summary>
        public string UomCategory { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string PackName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public string IsVisible { get; set; }

        /// <summary>
        /// 单位类型，物流/统计
        /// </summary>
        public string UomType { get; set; }

        /// <summary>
        /// 是否默认单位
        /// </summary>
        public string IsDefault { get; set; }

        /// <summary>
        /// 是否最小出入库数量单位
        /// </summary>
        public string IsMinIOPackUom { get; set; }

        /// <summary>
        /// 是否最小单位
        /// </summary>
        public string IsMinUom { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>
        public decimal? SeqNo { get; set; }

        /// <summary>
        /// 转换率，相对于最小单位的转换率，当前数量单位如果已经是最小数量单位，则本字段恒定为1
        /// </summary>
        public decimal? Rate { get; set; }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        public string IsDisabled { get; set; }

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
