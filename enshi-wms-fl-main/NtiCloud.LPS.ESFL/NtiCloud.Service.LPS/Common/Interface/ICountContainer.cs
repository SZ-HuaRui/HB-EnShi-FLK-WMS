using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface ICountContainer
    {
        /// <summary>
        /// 外部-盘点-立库，波次后库存不足则创建越库单
        /// </summary>
        /// <param name="OutOrderCode">出库单单据</param>
        /// <returns></returns>
        public WebJsonInfo GenCountTask(string CountHeadID);

        /// <summary>
        /// 外部-盘点-立库，盘点审核通过或不通过
        /// </summary>
        /// <param name="CountHeadID"></param>
        /// <param name="Audit"></param>
        /// <returns></returns>
        public WebJsonInfo PassCountAudit(string CountHeadID, string Audit);

        /// <summary>
        /// 删除盘点明细
        /// </summary>
        /// <param name="countDetailID">盘点明细ID</param>
        /// <returns></returns>
        public WebJsonInfo DelCountDetail(string countDetailID);
    }
}
