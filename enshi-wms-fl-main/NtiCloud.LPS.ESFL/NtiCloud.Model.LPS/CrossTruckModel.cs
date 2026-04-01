using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class CrossTruckInput
    {
        public class CrossTruckModel
        {
            //出库单据波次不足时，调用新建单据方法
            public string OutOrderCode { get; set; }
        }
    }
}
