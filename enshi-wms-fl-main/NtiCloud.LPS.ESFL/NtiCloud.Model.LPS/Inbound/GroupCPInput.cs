using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS.Inbound
{
    public class GroupCPInput
    {
        /// <summary>
        /// 成品组盘
        /// </summary>
        public class GroupCPModel
        {
            /// <summary>
            /// 入库站台
            /// </summary>
            public string DockNumber { get; set; }

            /// <summary>
            /// 容器条码
            /// </summary>
            public string ContainerBarcode { get; set; }

            /// <summary>
            /// 件烟码列表
            /// </summary>
            public List<string> BarcodeList { get; set; }

            /// <summary>
            /// 组盘数量
            /// </summary>
            public int Qty { get; set; }

            /// <summary>
            /// 牌号id
            /// </summary>
            public string ItemId { get; set; }

            /// <summary>
            /// 牌号编码
            /// </summary>
            public string ItemCode { get; set; }

            /// <summary>
            /// 牌号名称
            /// </summary>
            public string ItemName { get; set; }

            /// <summary>
            /// 单位id
            /// </summary>
            public string UomId { get; set; }

            /// <summary>
            /// 是否整盘
            /// </summary>
            public int IsFull { get; set; }
        }
    }
}
