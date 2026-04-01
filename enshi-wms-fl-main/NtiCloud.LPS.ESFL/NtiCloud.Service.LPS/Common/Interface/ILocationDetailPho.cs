using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface ILocationDetailPho
    {
        /// <summary>
        /// 创建快照
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo CreateDetailPho();
    }
}
