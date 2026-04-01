using System;
using System.Collections.Generic;
using System.Text;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;

namespace NtiCloud.Interface.WMS
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface ILocationAdjust
    {
        /// <summary>
        /// 货位调整单，确认生成任务
        /// </summary>
        /// <param name="LocationAdjustH"></param>
        /// <returns></returns>
        [Description(description: "新建完毕的货位调整单，检查数据，明细与主档的有码和无码调整是否一致，移入库位的容量",
                     inputDescription: "adjust_no:调整单号",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        WebJsonInfo ConfirmLocationAdjust(string adjustno);
    }
}
