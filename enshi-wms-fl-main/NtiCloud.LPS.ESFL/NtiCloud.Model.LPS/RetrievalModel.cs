using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Model.WMS
{
    public class RetrievalModel
    {
        /// <summary>
        /// 下架单ID
        /// </summary>
        public string RetrievalID { get; set; }
        
        /// <summary>
        /// 下架主档任务ID
        /// </summary>
        public string TaskHID { get; set; }

        /// <summary>
        /// 下架单明细ID
        /// </summary>
        public string DetailID { get; set; }

        /// <summary>
        /// 添加下架明细的托盘ID
        /// </summary>
        public string[] containerIDList { get; set; }
    }
}
