using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.WMS.HF;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.HF
{
    public class OutCheckService : IOutCheckService

    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<OutCheckService> _logger;
        public OutCheckService(IServiceProvider _serviceProvider, IConfiguration configuration, Func<string,
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
        /// 新增质检信息
        /// </summary>
        /// <param name="OutCheckInfo"></param>
        /// <returns></returns>
        public WebJsonInfo InsertOutCheck(AddOutCheckHeaderIn OutCheckInfo)
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                if (string.IsNullOrEmpty(OutCheckInfo.order_no) || string.IsNullOrEmpty(OutCheckInfo.service_type) || OutCheckInfo.lstDetail == null || OutCheckInfo.lstDetail.Count == 0)
                {
                    throw new Exception("参数错误.");
                }

                DBHelper.BeginTransaction();
                WmsFactory.Make<OutCheck>().InsertOutCheck(OutCheckInfo);
                info.errcode = 0;
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
        /// 确认质检单
        /// </summary>
        /// <param name="checkorder">质检单号</param>
        /// <returns></returns>
        public WebJsonInfo ConfirmOutCheck(string checkorder)
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                if (string.IsNullOrEmpty(checkorder))
                {
                    throw new Exception("参数错误.");
                }

                DBHelper.BeginTransaction();
                WmsFactory.Make<OutCheck>().UpdCheckConfirm(checkorder);
                info.errcode = 0;
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
