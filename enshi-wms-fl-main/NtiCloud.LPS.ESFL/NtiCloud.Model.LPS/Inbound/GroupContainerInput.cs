namespace NtiCloud.Model.WMS.Inbound
{
    public class GroupContainerInput
    {
        public class GroupModel 
        {
            public string ReceiptId { get; set; }
            public string ItemId { get; set; }
            public string ItemName { get; set; }
            public decimal? Qty { get; set; } = 0;
            public string UomId { get; set; }
            //托盘载量
            public decimal? ContainerCapacity { get; set; } = 0;
            public string SupplierId { get; set; }
            public string Lot { get; set; }
            public string QCStatus { get; set; }
            public string ContainerBarcode { get; set; }
            public string ItemLabel { get; set; }
            /// <summary>
            /// 统计单位
            /// </summary>
            public string StaUomId { get; set; } 
            /// <summary>
            /// 规格
            /// </summary>
            public decimal? Spec { get; set; }
            /// <summary>
            /// 转换率
            /// </summary>
            public decimal? Ratio { get; set; }
        }
    }
}
