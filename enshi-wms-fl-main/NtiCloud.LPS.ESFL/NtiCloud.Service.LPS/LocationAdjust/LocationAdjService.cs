using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using NtiCloud.Interface.WMS;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Kernel.Core;
using System.Runtime.InteropServices.WindowsRuntime;
using NtiCloud.Service.WMS.LocationAdjust;
using NtiCloud.Service.WMS.Model;
using Newtonsoft.Json;

namespace NtiCloud.Service.WMS.LocationAdjust
{
    public class LocationAdjService : ILocationAdjustService
    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private readonly IKernelCachingProvider _redis;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<LocationAdjService> _logger;
        public LocationAdjService(IServiceProvider _serviceProvider, ILogger<LocationAdjService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _redis = serviceProvider.GetService<KernelRedisProvider>();
            _configuration = configuration;
            _dbFactory = dbFactory;
            _logger = logger;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);
        }
        #endregion
        /// <summary>
        /// 货位调整单，确认生成任务
        /// </summary>
        /// <param name="LocationAdjustH"></param>
        /// <returns></returns>
        public WebJsonInfo ConfirmLocationAdjust(string adjustno)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).ConfirmLocationAdjust(adjustno);
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 货位调整RF页面加载移库单号
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo GetAdjustNo()
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {

                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                DataTable table = new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).GetAdjustNo();

                info.json = DBHelper.ConvertTableToObject<LocationAdjustModel.LocationAdjustH>(table);
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 通过移库单号获取移库单明细物料项
        /// </summary>
        /// <param name="AdjustNo"></param>
        /// <returns></returns>
        public WebJsonInfo GetItembyAdjustNo(string AdjustNo)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                DataTable table = new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).GetItembyAdjustNo(AdjustNo);
                info.json = table;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 货位调整直接移库，通过容器号获取明细物料
        /// </summary>
        /// <param name="sContainerCode"></param>
        /// <returns></returns>
        public WebJsonInfo GetContainerItem(string sLocationcode, string sContainerCode)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                DataTable table = new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).GetContainerItem(sLocationcode, sContainerCode);

                info.json = DBHelper.ConvertTableToObject<NtiCloud.Model.WMS.Model.ContainerDetail>(table); ;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }

        public WebJsonInfo GetLocationItem(string sLocationcode)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                DataTable table = new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).GetLocationItem(sLocationcode);

                info.json = DBHelper.ConvertTableToObject<ContainerDModel>(table); ;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 检查入库货位是否有容量
        /// </summary>
        /// <param name="sLocationcode"></param>
        /// <returns></returns>
        public WebJsonInfo CheckToInLocation(string sLocationcode)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                 new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).CheckToInLocation(sLocationcode);
                
                //info.json = DBHelper.ConvertTableToObject<NtiCloud.Model.WMS.Model.ContainerDetail>(table); ;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 检查移入容量的合法性
        /// </summary>
        /// <param name="sContainercode"></param>
        /// <returns></returns>
        public WebJsonInfo CheckToInContainer(string scontainerNo, string swms_lot, string sitemid)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).CheckToInContainer(scontainerNo, swms_lot, sitemid);

                //info.json = DBHelper.ConvertTableToObject<NtiCloud.Model.WMS.Model.ContainerDetail>(table); ;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 有调整单，无码调整，确认保存数据
        /// </summary>
        /// <param name="locationD"></param>
        /// <returns></returns>
        public WebJsonInfo ConfirmAdjustDocRFNoneSN(LocationAdjustModel.LocationAdjustD locationD)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).ConfirmAdjustDocRFNoneSN(locationD);

                //info.json = DBHelper.ConvertTableToObject<NtiCloud.Model.WMS.Model.ContainerDetail>(table); ;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
        /// <summary>
        /// 无调整单，无码调整，确认保存数据
        /// </summary>
        /// <param name="locationD"></param>
        /// <returns></returns>
        public WebJsonInfo ConfirmAdjustNDocRFNoneSN(LocationAdjustModel.LocationAdjustD locationD)
        {

            WebJsonInfo info = new WebJsonInfo();
            try
            {
                LogHelper.Init(_logger);
                Context.Init(serviceProvider, _dbFactory);
                DBHelper.BeginTransaction();
                new LocationAdj(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).ConfirmAdjustNDocRFNoneSN(locationD);

                //info.json = DBHelper.ConvertTableToObject<NtiCloud.Model.WMS.Model.ContainerDetail>(table); ;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                info.errcode = 500;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;

        }

        #region 库位调整
        /// <summary>
        /// 库位调整
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.LocationadJust(LocationAdjustModel.LocationadJustModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.LocationadJustHTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "库位调整单据id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"单据id{model.LocationadJustHTid}，进行库位调整",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "库位调整",
                CommonName = "api/wms/LocationAdjService/LocationadJust",
            }, () => new LocationAdj().LocationadJust(model));

        }
        #endregion

        #region 创建整行库位调整明细
        WebJsonInfo ILocationAdjustService.CreateLocationadJustDetail(LocationAdjustModel.CreateLocationadJustDetail model)
        {
            #region 入参判空
            //if (string.IsNullOrWhiteSpace(model.LocationadJustHTid))
            //{
            //    return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "库位调整单据id不可为空", json = null };
            //}
            if (string.IsNullOrWhiteSpace(model.Equipment_id))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "执行设备不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Inline))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "移入行号不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Outline))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "移出行号不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Zone_id))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "区域不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Adjust_no))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "单号不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"创建整行库位调整明细",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "创建整行库位调整明细",
                CommonName = "api/wms/LocationAdjService/CreateLocationadJustDetail",
            }, () => new LocationAdj().CreateLocationadJustDetail(model));

        }
        #endregion

        #region 编辑整行库位调整主档
        WebJsonInfo ILocationAdjustService.EditLocationadJustHead(LocationAdjustModel.EditLocationadJustHead model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.LocationadJustHTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "库位调整单据id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Equipment_id))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "执行设备不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Inline))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "移入行号不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Outline))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "移出行号不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Zone_id))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "区域不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"单据id{model.LocationadJustHTid}，编辑整行库位调整主档",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "编辑整行库位调整主档",
                CommonName = "api/wms/LocationAdjService/EditLocationadJustHead",
            }, () => new LocationAdj().EditLocationadJustHead(model));

        }
        #endregion

        #region 库位调整任务完成
        /// <summary>
        /// 库位调整任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.FinishLocationadJust(LocationAdjustModel.FinishLocationadJustModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "库位调整任务完成",
                CommonName = "api/wms/LocationAdjService/FinishLocationadJust",
            }, () => new LocationAdj().FinishLocationadJust(model));
        }
        #endregion

        #region 库位调整任务部分完成
        /// <summary>
        /// 库位调整任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.PartionFinishLocationadJustTask(LocationAdjustModel.PartionFinishLocationadJustTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "库位调整任务部分完成",
                CommonName = "api/wms/LocationAdjService/PartionFinishLocationadJustTask",
            }, () => new LocationAdj().PartionFinishLocationadJustTask(model));
        }
        #endregion

        #region 库位调整任务强制完成
        /// <summary>
        /// 库位调整任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.ForceFinishLocationadJustTask(LocationAdjustModel.ForceFinishLocationadJustTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "库位调整任务强制完成",
                CommonName = "api/wms/LocationAdjService/ForceFinishLocationadJustTask",
            }, () => new LocationAdj().ForceFinishLocationadJustTask(model));
        }
        #endregion

        #region 库位调整任务回收
        /// <summary>
        /// 库位调整任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.RecycleLocationadJustTask(LocationAdjustModel.RecycleLocationadJustTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "库位调整任务回收",
                CommonName = "api/wms/LocationAdjService/RecycleLocationadJustTask",
            }, () => new LocationAdj().RecycleLocationadJustTask(model));
        }
        #endregion

        #region 自动库位调整
        /// <summary>
        /// 自动库位调整
        /// </summary>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.AutoLocationadJust(LocationAdjustModel.AutoLocationadJustModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.ZoneId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "区域id不可为空", json = null };
            }
            #endregion
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"区域{model.ZoneId},进行自动库位调整",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "自动库位调整",
                CommonName = "api/wms/LocationAdjService/AutoLocationadJust",
            }, () => new LocationAdj().AutoLocationadJust(model));
        }
        #endregion

        #region 自动库位调整任务完成
        /// <summary>
        /// 自动库位调整任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.FinishAutoLocationadJust(LocationAdjustModel.FinishAutoLocationadJustModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "自动库位调整任务完成",
                CommonName = "api/wms/LocationAdjService/FinishAutoLocationadJust",
            }, () => new LocationAdj().FinishAutoLocationadJust(model));
        }
        #endregion

        #region 自动库位调整任务部分完成
        /// <summary>
        /// 自动库位调整任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.PartionFinishAutoLocationadJustTask(LocationAdjustModel.PartionFinishAutoLocationadJustTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "自动库位调整任务部分完成",
                CommonName = "api/wms/LocationAdjService/PartionFinishAutoLocationadJustTask",
            }, () => new LocationAdj().PartionFinishAutoLocationadJustTask(model));
        }
        #endregion

        #region 自动库位调整任务强制完成
        /// <summary>
        /// 自动库位调整任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.ForceFinishAutoLocationadJustTask(LocationAdjustModel.ForceFinishAutoLocationadJustTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "自动库位调整任务强制完成",
                CommonName = "api/wms/LocationAdjService/ForceFinishAutoLocationadJustTask",
            }, () => new LocationAdj().ForceFinishAutoLocationadJustTask(model));
        }
        #endregion

        #region 自动库位调整任务回收
        /// <summary>
        /// 自动库位调整任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo ILocationAdjustService.RecycleAutoLocationadJustTask(LocationAdjustModel.RecycleAutoLocationadJustTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "自动库位调整任务回收",
                CommonName = "api/wms/LocationAdjService/RecycleAutoLocationadJustTask",
            }, () => new LocationAdj().RecycleAutoLocationadJustTask(model));
        }
        #endregion
    }
}
