using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS.Inbound
{
    [Authorization]
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IReceiptPlanService
    {
        [Description(description: "计划执行",
                     inputDescription: "计划ID",
                     outputDescription: "")]
        WebJsonInfo Excute(string planId);
        
        [Description(description: "计划暂停",
                     inputDescription: "计划ID",
                     outputDescription: "")]
        WebJsonInfo Pause(string planId);

        [Description(description: "计划完成",
                     inputDescription: "计划ID",
                     outputDescription: "")]
        WebJsonInfo Finish(string planId);
    }
}
