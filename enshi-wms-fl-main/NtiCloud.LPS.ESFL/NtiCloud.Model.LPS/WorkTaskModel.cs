using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class WorkTaskModel
    {

        public class RecycleTaskParam
        {
            public string TaskNoOrIds { get; set; }
        }
        public class FinishTasksParam
        {
            public string TaskNoOrIds { get; set; }
            public string WarehouseCode { get; set; }
            public string ClientId { get; set; }

        }
        public class FinishOneTaskParam
        {
            public string TaskNoOrID { get; set; }
            public decimal Qty { get; set; }
        }
        public class ReplaceItemsParam
        {
            public string TaskNoOrIds { get; set; }
        }
        public class CancelItemReplacementParam
        {
            public string ReplacementIds { get; set; }
        }
        public class ConfirmItemReplacementParam
        {
            public string ReplacementIds { get; set; }
        }
        public class AddItemReplacementParam
        {
            public string ItemBarcode { get; set; }
            public string Reason { get; set; }
            public string AbnormalWeight { get; set; }
        }
        public class ModifyReplacementParam
        {
            public string ReplacementID { get; set; }
            public string LocationNoOrID { get; set; }
            public string Reason { get; set; }
            public string Memo { get; set; }
            public string AbnormalWeight { get; set; }
        }
        public class StartReplaceParam
        {
            public string ReplacementIds { get; set; }
        }
        public class FinishReplaceParam
        {
            public string ReplacementIds { get; set; }
        }
        /// <summary>
        /// 入库申请参数
        /// </summary>
        public class ApplyTaskInParam
        {
            public ApplyTaskInParam()
            {
                this.lstSN = new List<ApplyTaskInDetail>();

            } 
            public string ClientId { get; set; }
            /// <summary>
            /// 仓库代码
            /// </summary>
            public string WarehouseCode { get; set; }
            /// <summary>
            /// 区域id
            /// </summary>
            public string ZoneID { get; set; }
            /// <summary>
            /// 入库类型：空托盘申请入库、实托盘申请入库、生产申请入库
            /// </summary>
            public ApplyInboundType ApplyType { get; set; }
            /// <summary>
            /// 申请入库容器号
            /// </summary>
            public string ContainerNo { get; set; }

            /// <summary>
            /// 申请站台号
            /// </summary>
            public string ApplyStationCode { get; set; }

            /// <summary>
            /// 容器长度
            /// </summary>
            public decimal? Length { get; set; }
            /// <summary>
            /// 容器宽度
            /// </summary>
            public decimal? Width { get; set; }
            /// <summary>
            /// 容器高度
            /// </summary>
            public decimal? Height { get; set; }
            /// <summary>
            /// 容器重量
            /// </summary>
            public decimal? Weight { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public decimal? Qty { get; set; }
            /// <summary>
            /// 条码信息
            /// </summary>
            public List<ApplyTaskInDetail> lstSN { get; set; }

            /// <summary>
            /// 容器中的条码信息
            /// </summary>
            public class ApplyTaskInDetail
            {
                /// <summary>
                /// 箱码、序列号
                /// </summary>
                public string SNCode { get; set; }
                /// <summary>
                /// SN数量
                /// </summary>
                public decimal? SNQty { get; set; }
                /// <summary>
                /// 位置
                /// </summary>
                public string Position { get; set; }
            }
            /// <summary>
            /// 申请入库类型
            /// </summary>
            public enum ApplyInboundType
            {
                /// <summary>
                /// 空托盘
                /// </summary>
                [Description("空托盘")]
                EmptyContainer,
                /// <summary>
                /// 实托盘
                /// </summary>
                [Description("实托盘")]
                TallyedContainer,
                /// <summary>
                /// 生产入库
                /// </summary>
                [Description("生产入库")]
                ProductContainer,
                /// <summary>
                /// 余料回库
                /// </summary>
                [Description("余料回库")]
                ContainerPickBack,
            }
        }

        public class CallEmptyContainerParam
        {
            public string ClientId { get; set; } 
            /// <summary>
            /// 仓库代码
            /// </summary>
            public string WarehouseCode { get; set; }
            /// <summary>
            /// 申请站台号
            /// </summary>
            public string ApplyStationCode { get; set; }
            /// <summary>
            /// 托盘规格
            /// </summary>
            public int? Width { get; set; }
        }

        public class EquipmentParamIn {
            public string ClientId { get; set; }
            /// <summary>
            /// 仓库代码
            /// </summary>
            public string WarehouseCode { get; set; }
            /// <summary>
            /// 设备编码
            /// </summary>
            public string EquipmentCode { get; set; }
            /// <summary>
            /// 设备状态
            /// </summary>
            public string EquipmentStatus { get; set; }
        }

        /// <summary>
        /// 货位二次分配请求参数
        /// </summary>
        public class SecondDistribuLocRequest
        {
            /// <summary>
            /// 消息ID
            /// </summary>
            public string Msg_Id { get; set; }
            /// <summary>
            /// 消息标记
            /// </summary>
            public string Msg_Mark { get; set; }
            /// <summary>
            /// 调用时间
            /// </summary>
            public string Msg_Time { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string Msg_User { get; set; }
            /// <summary>
            /// 密码
            /// </summary>
            public string Msg_UserPwd { get; set; }
            /// <summary>
            /// 二次分配
            /// </summary>
            public string Msg_Method { get; set; }
            /// <summary>
            /// 数据组
            /// </summary>
            public List<Msg_Paramslist> Msg_Params { get; set; }

            /// <summary>
            /// 数据组
            /// </summary>
            public class Msg_Paramslist
            {
                /// <summary>
                /// 仓库ID
                /// </summary>
                public string WarehouseID { get; set; }
                /// <summary>
                /// 区域ID
                /// </summary>
                public string ZoneID { get; set; }
                /// <summary>
                /// 任务号
                /// </summary>
                public string TaskNum { get; set; }
            }
        }

        /// <summary>
        /// 货位二次分配响应参数
        /// </summary>
        public class SecondDistribuLocResponse
        {
            /// <summary>
            /// 响应数据
            /// </summary>
            public Msg_Paramslist Msg_Params { get; set; }

            /// <summary>
            /// 响应体model
            /// </summary>
            public class Msg_Paramslist
            {
                /// <summary>
                /// 仓库ID
                /// </summary>
                public string WarehouseId { get; set; }
                /// <summary>
                /// 区域ID
                /// </summary>
                public string ZoneId { get; set; }
                /// <summary>
                /// 任务号
                /// </summary>
                public string TaskNum { get; set; }
                /// <summary>
                /// 目标地址
                /// </summary>
                public string TargetAddress { get; set; }
            }
        }
        /// <summary>
        /// SN操作日志
        /// </summary>
        public class SNLogIn {
            /// <summary>
            /// 仓库代码
            /// </summary>
            public string WarehouseCode { get; set; }
            /// <summary>
            /// 客户ID
            /// </summary>
            public string ClientId { get; set; }
            /// <summary>
            /// SN号
            /// </summary>
            public string sn { get; set; }
            /// <summary>
            /// 原位置ID
            /// </summary>
            public string fromLocationId { get; set; }
            /// <summary>
            /// 原位置编号
            /// </summary>
            public string fromLocationCode { get; set; }
            /// <summary>
            /// 原位置名称
            /// </summary>
            public string fromLocationName { get; set; }
            /// <summary>
            /// 新位置ID
            /// </summary>
            public string toLocationId { get; set; }
            /// <summary>
            /// 新位置编号
            /// </summary>
            public string toLocationCode { get; set; }
            /// <summary>
            /// 新位置名称
            /// </summary>
            public string toLocationName { get; set; }
            /// <summary>
            /// 变更类型
            /// </summary>
            public string action { get; set; }
            /// <summary>
            /// 变更原因
            /// </summary>
            public string reason { get; set; }

            /// <summary>
            /// 对应单据号
            /// </summary>
            public string docno { get; set; }
            /// <summary>
            /// 对应任务号
            /// </summary>
            public string taskno { get; set; }
        }
    }
}
