using NtiCloud.Model.WMS.Model;
using System.Collections.Generic;


namespace NtiCloud.Model.WMS.Inbound
{
    public class ComposeContainerInput
    {
        public class ComposeModel
        {
            public string BomId { get; set; }
            public string ContainerBarcode { get; set; }
            public string ZoneId { get; set; } = string.Empty;
            public List<ContainerDetail> ContainerDetailList { get; set; }
        }
    }
}
