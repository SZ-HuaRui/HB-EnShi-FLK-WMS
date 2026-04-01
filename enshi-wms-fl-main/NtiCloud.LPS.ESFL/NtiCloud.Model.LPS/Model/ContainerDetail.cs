namespace NtiCloud.Model.WMS.Model
{
    public class ContainerDetail
    {
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? Qty { get; set; } = 0;
        public string UomId { get; set; }
        public string SupplierId { get; set; }
        public string Lot { get; set; }
        public decimal? Rate { get; set; } = 0;
        public string ItemLable { get; set; } = string.Empty;
    }
}
