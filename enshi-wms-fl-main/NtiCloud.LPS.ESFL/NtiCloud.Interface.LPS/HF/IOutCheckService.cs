using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS.HF
{
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    [Description("出库质检","","")]
    public  interface IOutCheckService
    {
        /// <summary>
        /// 新增质检信息
        /// </summary>
        /// <param name="OutCheckInfo"></param>
        /// <returns></returns>
        [Description("新增质检信息", "","")]
        WebJsonInfo InsertOutCheck(NtiCloud.Model.WMS.AddOutCheckHeaderIn OutCheckInfo);

        /// <summary>
        /// 确认质检结果
        /// </summary>
        /// <param name="checkorder">质检单号</param>
        /// <returns></returns>
        [Description("确认质检结果", "", "")]
        WebJsonInfo ConfirmOutCheck(string checkorder); 

    }
}
