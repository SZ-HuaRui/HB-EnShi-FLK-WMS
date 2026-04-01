using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class LocationDetailInfoModelInput
    {
        public class LocationDetailInfoModel
        {
            /// <summary>
            /// 层
            /// </summary>
            public string Layer { get; set; }
            /// <summary>
            /// 区域ID
            /// </summary>
            public string ZoneID { get; set; }

            /// <summary>
            /// 行
            /// </summary>
            public string Row { get; set; }

            /// <summary>
            /// 物料名称
            /// </summary>
            public string ItemName { get; set; }

            /// <summary>
            /// 物料编码
            /// </summary>
            public string ItemCode { get; set; }

            /// <summary>
            /// 批次
            /// </summary>
            public string Lot { get; set; }

            /// <summary>
            /// 起始货位
            /// </summary>
            public string FromLocation { get; set; }
            /// <summary>
            /// 目标货位
            /// </summary>
            public string ToLocation { get; set; }
        }
    }

    /// <summary>
    /// 库位信息
    /// </summary>
    public class LocationCell
    {
        /// <summary>
        /// 货位编码
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// 列(货架)
        /// </summary>
        public string location_column { get; set; }
        /// <summary>
        /// 行(列)
        /// </summary>
        public string location_row { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        public string location_layer { get; set; }

        /// <summary>
        /// 深
        /// </summary>
        public string deep { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemcode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemname { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string lot { get; set; }

        public string index_no { get; set; }
        public string index_col { get; set; }
        /// <summary>
        /// 存储状态 （Empty-空，Partial-部分存储，Full-满）
        /// </summary>
        public string stock_sts { get; set; }

        /// <summary>
        /// 是否可用 （Enable-可用，Disable-禁用）
        /// </summary>
        public string useable_sts { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public string remain_qty { get; set; }
    }

    /// <summary>
    /// 区域信息
    /// </summary>
    public class ZoneInfo
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string ZoneID { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 父区域
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 该区域所有层
        /// </summary>
        public IList<string> LayerList { get; set; }

        /// <summary>
        /// 该区域所有行
        /// </summary>
        public IList<string> RowList { get; set; }

        /// <summary>
        /// 子区域
        /// </summary>
        public List<ZoneInfo> ChildZone { get; set; }
    }
}
