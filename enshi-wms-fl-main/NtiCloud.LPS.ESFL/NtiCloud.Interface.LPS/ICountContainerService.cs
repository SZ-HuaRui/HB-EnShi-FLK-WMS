using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    public interface ICountContainerService
    {
        /// <summary>
        /// 外部-盘点-立库，产生盘点任务 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Description(description: "外部-盘点-立库，产生盘点任务 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "model必填值：CountHeadID 盘点单ID",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/GenCountTask")]
        public WebJsonInfo GenCountTask(CountContainerInput.CountContainerModel model);

        /// <summary>
        /// 外部-盘点-立库，审核盘点通过或不通过 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Description(description: "外部-盘点-立库，审核盘点通过或不通过 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "model必填值：CountHeadID 盘点单ID，Audit 盘点结果是否通过(0不通过、1通过)",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/PassCountAudit")]
        public WebJsonInfo PassCountAudit(CountContainerInput.CountContainerModel model);

        /// <summary>
        /// 外部-盘点-立库，删除盘点明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-盘点-立库，删除盘点明细",
        inputDescription: "model必填值：CountDetailID 盘点明细ID",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/DelCountDetail")]
        public WebJsonInfo DelCountDetail(CountContainerInput.CountContainerModel model);


        [Description(description: "外部-盘点-立库，添加盘点明细",
                inputDescription: "model必填值：CountDetailID 盘点明细ID",
                             outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/AddCountDetail")]
        public WebJsonInfo AddCountDetail(CountContainerInput.CountContainerModel model);

        [Description(description: "外部-盘点-立库，录入盘点数量",
        inputDescription: "model必填值：CountDetailID 盘点明细ID,IsConErr 是否托盘条码错误(0无错，1错误), Remark 备注, QtyCount 盘点数量",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/ConfirmCountQty")]
        public WebJsonInfo ConfirmCountQty(CountContainerInput.CountContainerModel model);

        [Description(description: "外部-盘点-立库，提交盘点结果",
        inputDescription: "model必填值：CountDetailID 盘点明细ID",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/SubmitCountAudit")]
        public WebJsonInfo SubmitCountAudit(CountContainerInput.CountContainerModel model);
        
        [Description(description: "外部-盘点-立库，获取盘点容器的所有库存信息",
        inputDescription: "model必填值：ContainerCode 盘点托盘条码",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/CountGetConInfo")]
        public WebJsonInfo CountGetConInfo(CountContainerInput.CountContainerModel model);

        [Description(description: "外部-盘点-立库，RF快速盘点，提交产生盘点回库任务",
        inputDescription: "model必填值：ContainerCode 盘点托盘条码,CountHeadID-盘点单ID",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/CountContainerService/RFSingleGenInCountTask")]
        public WebJsonInfo RFSingleGenInCountTask(CountContainerInput.CountContainerModel model);

    }
}
