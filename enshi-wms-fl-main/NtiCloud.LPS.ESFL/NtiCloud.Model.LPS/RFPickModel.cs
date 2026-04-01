using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class RFPickInput
    {
        public class RFPickModel
        {
            /// <summary>
            /// 根据托盘查找该托盘需要拣选的出库单(立库[波次/非波次])
            /// </summary>
            public string ContainerCode { get; set; }
            /// <summary>
            /// 上一项、下一项的页面(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public int Page { get; set; }
            /// <summary>
            /// 用户选择的出库单号(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public string Order { get; set; }

            /// <summary>
            /// 任务明细ID(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public string TaskDetailID { get; set; }

            /// <summary>
            /// 空托盘条码(立库[波次/非波次])
            /// </summary>
            public string OrderContainerCode { get; set; }
            /// <summary>
            /// 拣选模式，1为默认，2为交换托盘(立库[波次/非波次])
            /// </summary>
            public string PickType { get; set; }
            /// <summary>
            /// 已拣选数量(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public int ReadyPickNum { get; set; }
            /// <summary>
            /// 拣选数量(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public int PickNum { get; set; }
            /// <summary>
            /// 本次应拣数量(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public int NeedPick { get; set; }
            /// <summary>
            /// 拆箱时多余部分的数量
            /// </summary>
            public int MoreNum { get; set; }
            /// <summary>
            /// 来源单据明细ID(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public string RefDocDID { get; set; }
            /// <summary>
            /// 来源单据ID(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public string RefDocID { get; set; }

            /// <summary>
            /// 波次明细ID(立库[波次]、平库[波次])
            /// </summary>
            public string RefWaveDID { get; set; }
            /// <summary>
            /// 波次ID(立库[波次]、平库[波次])
            /// </summary>
            public string RefWaveID { get; set; }

            /// <summary>
            /// WMS出库单号(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public string DocNo { get; set; }

            /// <summary>
            /// 拣选的BN列表(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public IList<string> BNList { get; set; }

            /// <summary>
            /// 扫箱码时的箱码(立库[波次/非波次]、平库[波次/非波次])
            /// </summary>
            public string BNStr { get; set; }
            /// <summary>
            /// 货位编码(平库[波次/非波次])
            /// </summary>
            public string LocationCode { get; set; }

            /// <summary>
            /// 回库时WCS提供的站台地址
            /// </summary>
            public string SourceAddress { get; set; }
        }
    }
}
