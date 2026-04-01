using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.BaseData;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace NtiCloud.Service.WMS.RFPick
{
    public class RFPickService : IRFPickService
    {

        public RFPickService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            BaseDataService baseDataService = new BaseDataService(_serviceProvider, logger, configuration, dbFactory);
        }


        // RF拣选-立库

        #region 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode</param>
        /// <returns></returns>
        public WebJsonInfo LK_ScanContainerCode(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.ContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "容器编码不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().LK_ScanContainerCode(model.ContainerCode);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）</param>
        /// <returns></returns>
        public WebJsonInfo LK_ContainerPickInfo(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.ContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "容器编码不能为空";
                info.isSucceed = false;
                return info;
            }

            if (string.IsNullOrWhiteSpace(model.Order))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "用户未选择单据";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().LK_ContainerPickInfo(model.ContainerCode, model.Order,model.Page);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
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
        public WebJsonInfo LK_PickSubmit(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.TaskDetailID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "任务明细不能为空";
                info.isSucceed = false;
                return info;
            }

            if (string.IsNullOrWhiteSpace(model.PickType))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "拣选模式不能为空";
                info.isSucceed = false;
                return info;
            }

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.OrderContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "目标容器不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().LK_PickSubmit(model.TaskDetailID, model.PickType, model.OrderContainerCode,model.ReadyPickNum, model.PickNum, model.NeedPick, model.BNList);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode</param>
        /// <returns></returns>
        public WebJsonInfo LK_PickBack(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.ContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "容器编码不能为空";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().LK_PickBack(model.ContainerCode,model.SourceAddress);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的订单容器编码-OrderContainerCode,当前站台地址-SourceAddress</param>
        /// <returns></returns>
        public WebJsonInfo LK_OrderContainerBack(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.OrderContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "订单容器编码不能为空";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().LK_OrderContainerBack(model.OrderContainerCode, model.SourceAddress);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion
        // RF拣选-平库

        #region 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo PK_GetOutBoundInfo()
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().PK_GetOutBoundInfo();

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的用户选择的单据-Order</param>
        /// <returns></returns>
        public WebJsonInfo PK_PickLocationInfo(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数

            if (string.IsNullOrWhiteSpace(model.Order))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "用户未选择单据";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().PK_PickLocationInfo(model.Order);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的用户选择的单据-Order</param>
        /// <returns></returns>
        public WebJsonInfo PK_PickContainerInfo(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数

            if (string.IsNullOrWhiteSpace(model.Order))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "用户未选择单据";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().PK_PickContainerInfo(model.Order);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion


        #region 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的货位编码-LocationCode或容器编码-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）</param>
        /// <returns></returns>
        public WebJsonInfo PK_ContainerPickInfo(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.LocationCode) && string.IsNullOrWhiteSpace(model.ContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "货位/容器编码不能为空";
                info.isSucceed = false;
                return info;
            }

            if (string.IsNullOrWhiteSpace(model.Order))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "用户未选择单据";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().PK_ContainerPickInfo(model.LocationCode,model.ContainerCode,model.Order,model.Page);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
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
        public WebJsonInfo PK_PickSubmit(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.TaskDetailID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "任务明细ID不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().PK_PickSubmit(model.TaskDetailID,model.ReadyPickNum, model.PickNum, model.NeedPick,model.BNList);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion


        // RF拣选-[立库/平库]

        #region 外部-拣选-[立库/平库]，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-BNStr</param>
        /// <returns></returns>
        public WebJsonInfo BNInfo(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.BNStr))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "箱码不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().BNInfo(model.BNStr);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-拣选-[立库/平库]，拆箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拆箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的箱码应拣选数量-NeedPick，箱码拣选多余时多余的数量-MoreNum，箱码编号-BNStr</param>
        /// <returns></returns>
        public WebJsonInfo BNSpilt(RFPickInput.RFPickModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.BNStr))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "箱码不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<RFPick>().BNSpilt(model.NeedPick,model.MoreNum,model.BNStr);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

    }
}
