using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IRFPick
    {
        #region 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode</param>
        /// <returns></returns>
        WebJsonInfo LK_ScanContainerCode(string ContainerCode);
        #endregion

        #region 外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）</param>
        /// <returns></returns>
        WebJsonInfo LK_ContainerPickInfo(string ContainerCode, string Order, int Page);
        #endregion

        #region 外部-拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">model必填值：
        /// TaskDetailID 任务明细ID
        /// PickType     拣选模式  (1为默认，2为交换托盘)
        /// OrderContainerCode  空托盘条码
        /// PickNum      拣选数量
        /// NeedPick     本次应拣数量
        /// BNList       箱码(有箱码则填，无箱码不填)
        /// </param>
        /// <returns></returns>
        WebJsonInfo LK_PickSubmit(string TaskDetailID, string PickType, string OrderContainerCode, int readyPickNum, int PickNum, int NeedPick, IList<string> BNList);
        #endregion


        // RF拣选-平库

        #region 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <returns></returns>
        WebJsonInfo PK_GetOutBoundInfo();
        #endregion

        #region 外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的用户选择的单据-Order</param>
        /// <returns></returns>
        WebJsonInfo PK_PickLocationInfo(string Order);
        #endregion

        #region 外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的用户选择的单据-Order</param>
        /// <returns></returns>
        WebJsonInfo PK_PickContainerInfo(string Order);
        #endregion


        #region 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的货位编码-LocationCode或容器编码-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）</param>
        /// <returns></returns>
        WebJsonInfo PK_ContainerPickInfo(string LocationCode, string ContainerCode, string Order, int Page);
        #endregion

        #region 外部-拣选-平库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的货位编码-LocationCode</param>
        /// <returns></returns>
        WebJsonInfo LK_PickBack(string LocationCode,string SourceAddress);
        #endregion

        #region 外部-拣选-平库，订单托盘回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，订单托盘回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的订单容器编码-OrderContainerCode,当前站台地址-SourceAddress</param>
        /// <returns></returns>
        WebJsonInfo LK_OrderContainerBack(string OrderContainerCode, string SourceAddress);
        #endregion

        #region 外部-拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">model必填值：
        /// TaskDetailID 任务明细ID
        /// PickNum      拣选数量
        /// NeedPick     本次应拣数量
        /// BNList       箱码(有箱码则填，无箱码不填)
        /// </param>
        /// <returns></returns>
        WebJsonInfo PK_PickSubmit(string TaskDetailID, int readyPickNum, int PickNum, int NeedPick, IList<string> BNList);
        #endregion


        // RF拣选-[立库/平库]

        #region 外部-拣选-[立库/平库]，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-BNStr</param>
        /// <returns></returns>
        WebJsonInfo BNInfo(string BNStr);
        #endregion

        #region 外部-拣选-[立库/平库]，拆分箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拆分箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="NeedPick">传入model箱码应拣选数量-NeedPick</param>
        /// <param name="MoreNum">传入model箱码拣选多余时多余的数量-MoreNum</param>
        /// <param name="BNStr">传入model的箱码编号-BNStr</param>
        /// <returns></returns>
        WebJsonInfo BNSpilt(int NeedPick, int MoreNum, string BNStr);
        #endregion
    }
}
