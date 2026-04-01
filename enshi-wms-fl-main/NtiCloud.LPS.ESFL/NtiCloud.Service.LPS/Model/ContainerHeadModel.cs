using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    [Serializable]
    public class ContainerHeadModel
    {

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 集团ID
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
        /// 货位ID
        /// </summary>
        public string LocationID { get; set; }

        /// <summary>
        /// 容器条码
        /// </summary>
        public string ContainerBarcode { get; set; }

        /// <summary>
        /// 是否满载标记，1，满载，0，非满载
        /// </summary>
        public string IsFull { get; set; }

        /// <summary>
        /// 容器类别，虚拟，实际
        /// </summary>
        public string ContainerType { get; set; }

        /// <summary>
        /// 容器种类ID
        /// </summary>
        public string ContainerCatalogID { get; set; }

        /// <summary>
        /// 物料清单ID，生产工单号
        /// </summary>
        public string BomID { get; set; }

        /// <summary>
        /// 原机台ID，机台退回后登记
        /// </summary>
        public string EquipmentID { get; set; }

        /// <summary>
        /// 是否允许呼叫，0：不允许，1：允许
        /// </summary>
        public string AllowCalled { get; set; }

        /// <summary>
        /// 首次入库时间（按托盘管理的时候用）
        /// </summary>
        public DateTime? FirstInDate { get; set; }

        /// <summary>
        /// 标签码（RFID信息或板标签等）
        /// </summary>
        public string LabelCode { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 0，未入，1，已 入，立体库用
        /// </summary>
        public string IsIn { get; set; }

        /// <summary>
        /// 执行入库的操作人
        /// </summary>
        public string TakeInBy { get; set; }

        /// <summary>
        /// 0，未出，1，已出，立体库用
        /// </summary>
        public string IsOut { get; set; }

        /// <summary>
        /// 执行出库的操作人
        /// </summary>
        public string TakeOutBy { get; set; }

        /// <summary>
        /// 是否存放多种物料
        /// </summary>
        public string IsMixedItem { get; set; }

        /// <summary>
        /// 是否存放多个批次的物料
        /// </summary>
        public string IsMixedLot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

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
