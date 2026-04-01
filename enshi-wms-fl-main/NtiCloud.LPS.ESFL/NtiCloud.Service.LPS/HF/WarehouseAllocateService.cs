using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.HF
{
   public  class WarehouseAllocateService : NtiCloud.Interface.WMS.HF.IWarehouseAllocateService
    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<WarehouseAllocateService> _logger;
        public WarehouseAllocateService(IServiceProvider _serviceProvider, IConfiguration configuration, Func<string,
        IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _configuration = configuration;
            _dbFactory = dbFactory;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);

        }
        #endregion

        /// <summary>
        /// 保存仓库分配信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebJsonInfo SaveWarehouseEditInfo(EditWarhouseIn model)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (string.IsNullOrWhiteSpace(model.order_no))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "必须传单据号码";
                return info;
            } 
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<WarehouseAllocate>().EditOutboundPlan(model);
                info.isSucceed = true;
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
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }

        /// <summary>
        /// 确认仓库分配信息
        /// </summary>
        /// <param name="confirminfo"></param>
        /// <returns></returns>
        public WebJsonInfo MakesureWarehouseAllocate(ConfirmWarehouseIn confirminfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (string.IsNullOrWhiteSpace(confirminfo.order_no))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "必须传单据号码";
                return info;
            }
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<WarehouseAllocate>().MakesureWarehouseAllocate(confirminfo);
                info.isSucceed = true;
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
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }

        /// <summary>
        /// 取消仓库分配确认
        /// </summary>
        /// <param name="confirminfo"></param>
        /// <returns></returns>
        public WebJsonInfo RollBackWarehouseAllocate(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (string.IsNullOrWhiteSpace(order_no))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "必须传单据号码";
                return info;
            }
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().AdjustOrder("P_BackOutboundPlan@hfchk_link", order_no);
                info.isSucceed = true;
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
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }


        /// <summary>
        /// 删除出库计划
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public WebJsonInfo DeleteOutboundPlan(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (string.IsNullOrWhiteSpace(order_no))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "必须传单据号码";
                return info;
            }
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<WarehouseAllocate>().DeleteOutboundPlan(order_no);
                info.isSucceed = true;
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
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        /// <summary>
        /// 删除出库计划明细
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public WebJsonInfo DeleteOutboundPlanDetail(string order_no,decimal? orderdetail_id)
        {
            WebJsonInfo info = new WebJsonInfo();             
            try
            {
                DBHelper.BeginTransaction();
                if (orderdetail_id == null) {
                    throw new Exception("订单项不能为空");
                }
                bool re = WmsFactory.Make<WarehouseAllocate>().DeleteOutboundPlanDetail(order_no,orderdetail_id.ToString());
                info.isSucceed = true;
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
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        /// <summary>
        /// 确认仓库分配信息
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public WebJsonInfo MakesureWarehouseAllocate2(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (string.IsNullOrWhiteSpace(order_no))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "必须传单据号码";
                return info;
            }
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<WarehouseAllocate>().MakesureWarehouseAllocate2(order_no);
                info.isSucceed = true;
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
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        /// <summary>
        /// 同步配方
        /// </summary>
        /// <param name="order_nos"></param>
        /// <returns></returns>
        public WebJsonInfo SyncPrescription(List<SyncPrescriptionParam> ordernos)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (ordernos==null ||ordernos.Count==0)
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "必须传单据号码";
                return info;
            }
            try
            {
                DBHelper.BeginTransaction();
                string order_nos = "";
                foreach (SyncPrescriptionParam o in ordernos) {
                    order_nos += o.order_no + ",";
                }
                 WmsFactory.Make<WarehouseAllocate>().syncPrescription(order_nos);
                info.isSucceed = true;
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
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
    }
}
