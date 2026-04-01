using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface ILocationDetailInfo
    {
        /// <summary>
        /// 外部-库存概况-立库，获取指定区域指定行的库存货位信息
        /// </summary>
        /// <param name="location_row">行</param>
        /// <returns></returns>
        public WebJsonInfo GetLocationDetailInfo(string location_row, string zoneid,string itemname, string itemcode, string lot);

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo GetZoneInfo();
    }
}
