using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
    public class UomModel
    {
        public string UomID { get; set; }
        public decimal Qty { get; set; }
    }

    public class UomConverterModel
    {
        public string DefaultPackUomID { get; set; } = string.Empty;
        public string DefaultStatUomID { get; set; } = string.Empty;
        public decimal DefaultPackRate { get; set; } = 1m;
        public decimal DefaultStatRate { get; set; } = 1m;
        public decimal Rate { get; set; } = 1m;
    }

}
