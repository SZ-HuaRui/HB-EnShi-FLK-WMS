using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;

namespace NtiCloud.Interface.WMS
{
    public interface IRetrievalService
    {
        /// <summary>
        /// 下架单提交下架
        /// </summary>
        /// <param name="model">传入model的下架单ID-RetrievalID</param>
        /// <returns></returns>
        [Description(description: "下架单开始下架",
        inputDescription: "传入model的下架单ID-RetrievalID",
            outputDescription: "")]
        [Post("/api/wms/RetrievalService/RetrievalDownCommit")]
        WebJsonInfo RetrievalDownCommit(RetrievalModel model);

        /// <summary>
        /// 完成下架任务(WCS使用)
        /// </summary>
        /// <param name="model">传入model的下架任务主档ID-TaskHID</param>
        /// <returns></returns>
        [Description(description: "完成下架任务(WCS使用)",
        inputDescription: "传入model的下架任务主档ID-TaskHID",
            outputDescription: "")]
        [Post("/api/wms/RetrievalService/RetrievalDownTaskOver")]
        WebJsonInfo RetrievalDownTaskOver(RetrievalModel model);

        /// <summary>
        /// 删除下架单(明细或主档)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "删除下架单(明细或主档)",
        inputDescription: "传入model的下架单ID-RetrievalID或下架单明细ID-DetailID",
            outputDescription: "")]
        [Post("/api/wms/RetrievalService/DelRetrievalDetail")]
        WebJsonInfo DelRetrievalDetail(RetrievalModel model);

        /// <summary>
        /// 添加下架明细
        /// </summary>
        /// <param name="model">传入model的 下架单ID-RetrievalID,选择的容器ID列表-containerIDList</param>
        /// <returns></returns>
        [Description(description: "添加下架明细",
        inputDescription: "传入model的 下架单ID-RetrievalID,选择的容器ID列表-containerIDList",
            outputDescription: "")]
        [Post("/api/wms/RetrievalService/RetrievalSaveDetail")]
        WebJsonInfo RetrievalSaveDetail(RetrievalModel model);
    }
}
