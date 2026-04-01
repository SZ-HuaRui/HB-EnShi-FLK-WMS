using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.BaseData;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.CountContainer
{
    public class LocationDetailPhoService : ILocationDetailPhoService
    {
        public LocationDetailPhoService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            BaseDataService baseDataService = new BaseDataService(_serviceProvider, logger, configuration, dbFactory);
        }

        #region 外部-库存概况-立库，获取库存概况
        /// <summary>
        /// 外部-库存概况-立库，获取库存概况
        /// </summary>
        /// <param name="model">model必填值：
        /// Layer 层数
        /// </param>
        /// <returns></returns>
        public WebJsonInfo CreateDetailPho()
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<LocationDetailPho.LocationDetailPho>().CreateDetailPho();

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
        #endregion 



    }
}
