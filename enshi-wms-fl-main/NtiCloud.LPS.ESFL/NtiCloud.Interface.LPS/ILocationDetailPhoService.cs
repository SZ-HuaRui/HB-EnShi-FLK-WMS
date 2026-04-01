using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    public interface ILocationDetailPhoService
    {

        /// <summary>
        /// 创建快照
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-创建快照",
                     inputDescription: "无参数",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/LocationDetailPhoService/CreateDetailPho")]
        public WebJsonInfo CreateDetailPho();

    }
}
