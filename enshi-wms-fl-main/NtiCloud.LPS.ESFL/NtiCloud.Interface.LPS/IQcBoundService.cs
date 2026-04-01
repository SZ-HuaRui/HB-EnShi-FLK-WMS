using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IQcBoundService
    {
        /// <summary>
        /// 确认质检
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "确认质检",
         inputDescription: @"{ qc_h_id:主档id ,
                               type:'业务类型'}",
         outputDescription: "")]
        WebJsonInfo ConfirmQcRunByType(dynamic param);

        /// <summary>
        /// 删除质检明细
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "删除质检明细",
         inputDescription: @"{ qc_h_id:'质检主档id',
                               t_id:'质检明细id'
                              }",
         outputDescription: "")]
        WebJsonInfo DelQcDetail(dynamic param);

        /// <summary>
        /// RF调用质检产生任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "RF调用质检产生入库任务",
         inputDescription: @"{ containerCode:'托盘条码'
                             }",
         outputDescription: "")]
        WebJsonInfo StartInQcTask(string containerCode);

        /// <summary>
        /// RF调用质检产生任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "RF调用质检产生任务",
         inputDescription: @"{ containerCode:'托盘条码'
                               items:'物料列表'
                             }",
         outputDescription: "")]
        WebJsonInfo StartInQcTaskWithItem(string containerCode, dynamic items);

        /// <summary>
        /// WCS调用质检回库任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "WCS调用质检回库任务",
         inputDescription: @"{ containerCode:'托盘条码'
                             }",
         outputDescription: "")]
        WebJsonInfo RunInQcTaskByWcs(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo);

        /// <summary>
        /// 获取质检单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "获取质检单信息",
         inputDescription: @"{ barcode:'托盘条码'
                             }",
         outputDescription: "")]
        WebJsonInfo GetQcInfo(string barcode);

        /// <summary>
        /// 更新质检结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "更新质检结果",
         inputDescription: @"{ t_id:'质检主档id',
                               check_result:'质检结果' }",
         outputDescription: "")]
        WebJsonInfo UpdateQcResult(dynamic param);

        /// <summary>
        /// 确认质检结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "确认质检结果",
         inputDescription: @"{ qc_h_id:'质检主档id' }",
         outputDescription: "")]
        WebJsonInfo ConfirmQcResult(dynamic param);

        /// <summary>
        /// 审核质检结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "审核质检结果",
         inputDescription: @"{ qc_h_id:'质检主档id' 
                               qc_doc_status:'质检状态'
                             }",
         outputDescription: "")]
        WebJsonInfo CheckQcResult(dynamic param);
    }
}
