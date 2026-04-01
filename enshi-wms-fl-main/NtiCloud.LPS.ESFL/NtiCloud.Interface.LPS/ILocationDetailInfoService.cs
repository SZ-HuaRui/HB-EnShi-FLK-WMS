using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    public interface ILocationDetailInfoService
    {
        /// <summary>
        /// 外部-库存概况-立库，获取库存概况
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-库存概况-立库，获取库存概况",
                     inputDescription: "model必填值：Layer 层",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationDetailInfoService/GetLocationDetailInfo")]
        public WebJsonInfo GetLocationDetailInfo(LocationDetailInfoModelInput.LocationDetailInfoModel model);

        /// <summary>
        /// 外部-库存概况-立库，获取区域信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-库存概况-立库，获取区域信息",
                     inputDescription: "无参数",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationDetailInfoService/GetZoneInfo")]
        public WebJsonInfo GetZoneInfo();

    }
}
