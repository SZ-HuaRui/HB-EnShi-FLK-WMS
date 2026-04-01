using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
//wms Nti56com
namespace NtiCloud.Interface.WMS
{

    public interface IRFPickService
    {
        #region RF拣选-[立库]
        /// <summary>
        /// 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的托盘号-ContainerCode",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/LK_ScanContainerCode")]
        WebJsonInfo LK_ScanContainerCode(RFPickInput.RFPickModel model);

        /// <summary>
        /// 拣选-立库-无箱码 ，选定下拉单据后，根据托盘号+出库单号+页码 获取需要拣选的单行明细数据(波次出库、无波次出库 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的托盘号-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/LK_ContainerPickInfo")]
        WebJsonInfo LK_ContainerPickInfo(RFPickInput.RFPickModel model);

        /// <summary>
        /// 外部-拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "model必填值：TaskDetailID-任务明细ID,PickType-拣选模式(1为默认，2为交换托盘),OrderContainerCode-空托盘条码,PickNum-拣选数量,NeedPick-本次应拣数量,BNList-箱码(有箱码则填，无箱码不填)",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/LK_PickSubmit")]
        WebJsonInfo LK_PickSubmit(RFPickInput.RFPickModel model);

        /// <summary>
        /// 拣选-立库-无箱码 ，拣选回库(波次出库、无波次出库 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的托盘号-ContainerCode",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/LK_PickBack")]
        WebJsonInfo LK_PickBack(RFPickInput.RFPickModel model);

        /// <summary>
        /// 拣选-立库-无箱码 ，订单托盘回库(波次出库、无波次出库 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-立库，订单托盘回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的订单容器编码-OrderContainerCode,当前站台地址-SourceAddress",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/LK_OrderContainerBack")]
        WebJsonInfo LK_OrderContainerBack(RFPickInput.RFPickModel model);
        #endregion

        #region RF拣选-[平库]

        /// <summary>
        /// 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "无传入参数",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/PK_GetOutBoundInfo")]
        WebJsonInfo PK_GetOutBoundInfo();


        /// <summary>
        /// 外部-拣选-平库，获取需要拣选的出库单货位列表(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位列表(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的用户选择的单据-Order",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/PK_PickLocationInfo")]
        WebJsonInfo PK_PickLocationInfo(RFPickInput.RFPickModel model);

        /// <summary>
        /// 外部-拣选-平库，获取需要拣选的出库单容器列表(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器列表(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的用户选择的单据-Order",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/PK_PickContainerInfo")]
        WebJsonInfo PK_PickContainerInfo(RFPickInput.RFPickModel model);

        /// <summary>
        /// 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的货位编码-LocationCode或容器编码-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/PK_ContainerPickInfo")]
        WebJsonInfo PK_ContainerPickInfo(RFPickInput.RFPickModel model);

        /// <summary>
        /// 外部-拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "model必填值：TaskDetailID-任务明细ID,PickNum-拣选数量,NeedPick-本次应拣数量,BNList-箱码(有箱码则填，无箱码不填)",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/PK_PickSubmit")]
        WebJsonInfo PK_PickSubmit(RFPickInput.RFPickModel model);

        #endregion

        #region RF拣选-[立库/平库]

        /// <summary>
        /// 外部-拣选-[平库/立库]，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description(description: "外部-拣选-[平库/立库]，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的托盘号-BNStr",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/BNInfo")]
        WebJsonInfo BNInfo(RFPickInput.RFPickModel model);

        #endregion

        #region 外部-拣选-[立库/平库]，拆箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拆箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-BNStr</param>
        /// <returns></returns>
        [Description(description: "外部-拣选-[平库/立库]，拆箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)",
                     inputDescription: "传入model的箱码应拣选数量-NeedPick，箱码拣选多余时多余的数量-MoreNum，箱码编号-BNStr",
                     outputDescription: "errcode 为 0 表示成功，否则失败")]
        [Post("/api/wms/RFPickService/BNSpilt")]
        WebJsonInfo BNSpilt(RFPickInput.RFPickModel model);
        #endregion
    }
}
