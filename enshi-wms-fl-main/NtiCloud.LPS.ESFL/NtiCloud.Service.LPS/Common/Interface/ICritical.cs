using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface ICritical
    {
        /// <summary>
        /// 外部-越库-立库，产生紧急补料单
        /// </summary>
        /// <param name="OutOrderCode">出库单单据</param>
        /// <param name="CriticalDataList">出库数量</param>
        /// <returns></returns>
        public WebJsonInfo CreateCritical(string OutOrderCode,IList<CriticalInput.CriticalData> CriticalDataList);
    }
}
