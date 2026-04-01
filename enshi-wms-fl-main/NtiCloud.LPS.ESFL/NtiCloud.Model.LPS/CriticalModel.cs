using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class CriticalInput
    {
        public class CriticalModel
        {
            //紧急补料原单编码
            public string OutOrderCode { get; set; }
            /// <summary>
            /// 紧急补料数量
            /// </summary>
            public IList<CriticalData> CriticalDataList { get; set; }
        }

        public class CriticalData
        {
            /// <summary>
            /// 原单据明细ID
            /// </summary>
            public string outDetailID { get; set; }
            /// <summary>
            /// 补料数量
            /// </summary>
           public decimal outNum { get; set; }
        }
    }
}
