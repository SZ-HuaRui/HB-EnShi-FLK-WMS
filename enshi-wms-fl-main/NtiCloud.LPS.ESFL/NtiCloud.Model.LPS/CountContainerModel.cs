using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class CountContainerInput
    {
        public class CountContainerModel
        {
            /// <summary>
            /// 盘点单号ID
            /// </summary>
            public string CountHeadID { get; set; }

            /// <summary>
            /// 审核结果(0不通过，1通过)
            /// </summary>
            public string Audit { get; set; }

            /// <summary>
            /// 盘点明细ID
            /// </summary>
            public string CountDetailID { get; set; }

            /// <summary>
            /// 添加盘点明细IDList
            /// </summary>
            public List<string> DetailIDList { get; set; }

            /// <summary>
            /// 是否容器条码错误
            /// </summary>
            public string IsConErr { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; }

            /// <summary>
            /// 盘点数量
            /// </summary>
            public decimal QtyCount { get; set; }

            /// <summary>
            /// 盘点扫描的条码
            /// </summary>
            public string ContainerCode { get; set; }
        }

        /// <summary>
        /// 盘点提交的数据
        /// </summary>
        public class CountContainerDetail
        {
            public string DetailID { get; set; }
            public string ItemName { get; set; }

            public string ItemCode { get; set; }

            /// <summary>
            /// 原数量
            /// </summary>
            public string Qty { get; set; }

            /// <summary>
            /// 盘点单号
            /// </summary>
            public string CountCode { get; set; }

            public string CountHeadID { get; set; }
        }
    }
}
