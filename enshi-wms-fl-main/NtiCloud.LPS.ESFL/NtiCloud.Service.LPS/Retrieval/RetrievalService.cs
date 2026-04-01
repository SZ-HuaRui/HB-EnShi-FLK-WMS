using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.BaseData;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NtiCloud.Model.WMS.Model;
using Newtonsoft.Json;

namespace NtiCloud.Service.WMS.Retrieval
{
    public class RetrievalService: IRetrievalService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly UserMode userMode;
        private readonly IDBContextProvider dbProvider;
        private readonly UserLoginInfo userLoginInfo;

        public RetrievalService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            BaseDataService baseDataService = new BaseDataService(_serviceProvider, logger, configuration, dbFactory);
            serviceProvider = _serviceProvider;
            userMode = serviceProvider.GetService<IUserService>().GetToken();
            userLoginInfo = userMode.userLoginInfo;
            dbProvider = dbFactory(userLoginInfo.UserDbConnR.dbType);
        }


        #region 下架单提交下架
        /// <summary>
        /// 下架单提交下架
        /// </summary>
        /// <param name="model">传入model的下架单ID-RetrievalID</param>
        /// <returns></returns>
        public WebJsonInfo RetrievalDownCommit(RetrievalModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.RetrievalID))
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "下架单ID不能为空";
                Console.WriteLine(info.errmsg);
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<Retrieval>().RetrievalDownCommit(model.RetrievalID);

                Log log = new Log()
                {
                    Message = "执行下架成功",
                    Action = "/api/wms/RetrievalService/RetrievalDownCommit",
                    Parameter = JsonConvert.SerializeObject(model),
                    Result = JsonConvert.SerializeObject(info),
                    Type = "执行下架"
                };

                WmsLog.Write(serviceProvider, dbProvider, userMode, log);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                if (info.errcode == 0)
                {
                    info.errcode = 500;
                    info.isSucceed = false;
                }
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 下架任务完成
        /// <summary>
        /// 下架任务完成(WCS调用)
        /// </summary>
        /// <param name="model">传入model的任务主档ID：TaskHID</param>
        /// <returns></returns>
        public WebJsonInfo RetrievalDownTaskOver(RetrievalModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.TaskHID))
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "下架任务不能为空";
                Console.WriteLine(info.errmsg);
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<Retrieval>().RetrievalDownTaskOver(model.TaskHID);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                if (info.errcode == 0)
                {
                    info.errcode = 500;
                    info.isSucceed = false;
                }
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 删除下架单(明细或主档)
        /// <summary>
        /// 删除下架单(明细或主档)
        /// </summary>
        /// <param name="model">传入model的下架单ID-RetrievalID或下架单明细ID-DetailID</param>
        /// <returns></returns>
        public WebJsonInfo DelRetrievalDetail(RetrievalModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.RetrievalID) && string.IsNullOrEmpty(model.DetailID))
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "未选择要删除的数据";
                Console.WriteLine(info.errmsg);
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<Retrieval>().DelRetrievalDetail(model.RetrievalID,model.DetailID);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                if (info.errcode == 0)
                {
                    info.errcode = 500;
                    info.isSucceed = false;
                }
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 添加下架明细
        /// <summary>
        /// 添加下架明细
        /// </summary>
        /// <param name="model">传入model的 下架单ID-RetrievalID,选择的容器ID列表-containerIDList</param>
        /// <returns></returns>
        public WebJsonInfo RetrievalSaveDetail(RetrievalModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(model.RetrievalID) || model.containerIDList==null)
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "未选择要添加的数据";
                Console.WriteLine(info.errmsg);
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<Retrieval>().RetrievalSaveDetail(model.RetrievalID, model.containerIDList);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                if (info.errcode == 0)
                {
                    info.errcode = 500;
                    info.isSucceed = false;
                }
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion


    }
}
