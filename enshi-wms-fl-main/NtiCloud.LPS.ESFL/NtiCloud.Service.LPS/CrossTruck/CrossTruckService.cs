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

namespace NtiCloud.Service.WMS.CrossTruck
{
    public class CrossTruckService: ICrossTruckService
    {
        public CrossTruckService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            BaseDataService baseDataService = new BaseDataService(_serviceProvider, logger, configuration, dbFactory);
        }

        #region 外部-越库-立库，波次后库存不足则创建越库单
        /// <summary>
        /// 外部-越库-立库，波次后库存不足则创建越库单
        /// </summary>
        /// <param name="model">model必填值：
        /// OutOrderCode 出库单单据
        /// </param>
        /// <returns></returns>
        public WebJsonInfo CreateCrossTruck(CrossTruckInput.CrossTruckModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.OutOrderCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "出库单不能为空";
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CrossTruck>().CreateCrossTruck(model.OutOrderCode);

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
        #endregion 
    }
}
