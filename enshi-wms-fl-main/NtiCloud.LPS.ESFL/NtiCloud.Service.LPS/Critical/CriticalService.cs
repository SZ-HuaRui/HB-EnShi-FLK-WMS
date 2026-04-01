using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.BaseData;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Critical
{
    public class CriticalService : ICriticalService
    {
        public CriticalService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            BaseDataService baseDataService = new BaseDataService(_serviceProvider, logger, configuration, dbFactory);
        }

        #region 外部-越库-立库，新增紧急补料
        /// <summary>
        /// 外部-越库-立库，新增紧急补料
        /// </summary>
        /// <param name="model">model必填值：
        /// OutOrderCode 出库单单据,CriticalDataList 紧急补料数量
        /// </param>
        /// <returns></returns>
        public WebJsonInfo CreateCritical(CriticalInput.CriticalModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.OutOrderCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "出库单不能为空";
                info.isSucceed = false;
                return info;
            }

            if(model.OutOrderCode == null || model.CriticalDataList.Count()<1 || model.CriticalDataList.Sum(o=>o.outNum)==0)
            {
                info.errcode = 500;
                info.errmsg = "补料数量不能为0";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<Critical>().CreateCritical(model.OutOrderCode,model.CriticalDataList);

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
