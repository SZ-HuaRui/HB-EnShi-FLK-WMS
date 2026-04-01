using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    public interface ICriticalService
    {
        /// <summary>
        /// 外部-越库-立库，产生紧急补料单 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-越库-立库，产生紧急补料单  (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "model必填值：OutOrderCode 出库单单据，CriticalDataList 紧急补料数量",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CriticalService/CreateCritical")]
        public WebJsonInfo CreateCritical(CriticalInput.CriticalModel model);
    }
}
