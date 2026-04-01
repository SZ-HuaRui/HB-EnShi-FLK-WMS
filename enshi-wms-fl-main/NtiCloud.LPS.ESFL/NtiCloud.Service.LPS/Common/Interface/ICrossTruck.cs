using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface ICrossTruck
    {
        /// <summary>
        /// 外部-越库-立库，波次后库存不足则创建越库单
        /// </summary>
        /// <param name="OutOrderCode">出库单单据</param>
        /// <returns></returns>
        public WebJsonInfo CreateCrossTruck(string OutOrderCode);
    }
}
