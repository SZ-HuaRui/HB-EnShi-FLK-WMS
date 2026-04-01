using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.CallEnd;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Model.WMS.Inbound;

namespace NtiCloud.Service.WMS.CallEnd
{
    public class MachineMaterialService : IMachineMaterialService
    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private readonly IKernelCachingProvider _redis;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<MachineMaterialService> _logger;
        public MachineMaterialService(IServiceProvider _serviceProvider, ILogger<MachineMaterialService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
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

        #region 机台要料
        /// <summary>
        /// 根据机台站台产生物料出任务，通过机台和站台获取区段、生产牌号，进而获取物料。根据物料，调用货位分配算法，得到货位，产生从该货位出物料的任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.CreateOutBoundByMachineDock(MachineMaterialInput.MachineMaterialModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            #endregion

            //return WmsHelper.Do(new UserLogModel()
            //{
            //    LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}要料",
            //    LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
            //    TypeCode = "机台要料",
            //    CommonName = "api/wms/MachineMaterialService/CreateOutBoundByMachineDock",
            //}, () => new MachineMaterialBusinessProce().CreateOutBoundByMachineDock(model));


            WebJsonInfo info = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}要料",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "机台要料",
                CommonName = "api/wms/MachineMaterialService/CreateOutBoundByMachineDock",
            }, () => new MachineMaterialBusinessProce().CreateOutBoundByMachineDock(model));

            string planno = model.Planno == null ? "" : model.Planno;

            if (info.errcode != 0)
            {
                //info.json = "{"+ "\"planno\":" + "\""+planno + "\""+"}";
                info.json = new { planno= model.Planno } ;
                //info.json = JsonConvert.SerializeObject(model);
            }

            //MachineMaterialInput.MachineMaterialModel rt = JsonConvert.DeserializeObject<MachineMaterialInput.MachineMaterialModel>(info.json.ToString());

            return info;


            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='机台要料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().CreateOutBoundByMachineDock(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;

        }
        #endregion

        #region 机台要料任务强制完成
        /// <summary>
        /// 机台要料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishMachineMaterial(MachineMaterialInput.FinishMachineMaterialModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"要料任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "要料任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishMachineMaterial",
            }, () => new MachineMaterialBusinessProce().FinishMachineMaterialModel(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='机台要料任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishMachineMaterialModel(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 机台要料任务回收
        /// <summary>
        /// 机台要料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebJsonInfo RecycleMachineMaterial(MachineMaterialInput.RecycleMachineMaterialModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"要料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "要料任务回收",
                CommonName = "api/wms/MachineMaterialService/RecycleMachineMaterial",
            }, () => new MachineMaterialBusinessProce().RecycleMachineMaterial(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='机台要料任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecycleMachineMaterial(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 机台要料任务部分完成
        /// <summary>
        /// 机台要料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.PartionFinishMachineMaterial(MachineMaterialInput.PartionFinishMachineMaterialModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"机台要料任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "机台要料任务部分完成",
                CommonName = "api/wms/MachineMaterialService/PartionFinishMachineMaterial",
            }, () => new MachineMaterialBusinessProce().PartionFinishMachineMaterial(model));
        }
        #endregion

        #region 机台要料任务完成
        /// <summary>
        /// 机台要料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishMaterial(MachineMaterialInput.FinishMaterialModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"机台要料任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "机台要料任务完成",
                CommonName = "api/wms/MachineMaterialService/FinishMaterial",
            }, () => new MachineMaterialBusinessProce().FinishMaterial(model));
        } 
        #endregion

        #region 退余料+要料
        /// <summary>
        /// 退余料+要料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ClientReturnPallet(MachineMaterialInput.ClientReturnPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Details))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "退余料明细不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.IsNeedCallPallet))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "是否需要再次要料不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.IsCanBeCallOut))
            {
                return new WebJsonInfo { errcode = 1, isSucceed = false, errmsg = "是否允许被再次呼出不可为空", json = null };
            }
            #endregion
            string msg = $"{DateTime.Now}\r\n  type='退余料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            LogHelper.LogInformation(msg);
            return new MachineMaterialBusinessProce().ClientReturnPallet(model);
        }
        #endregion

        #region 退余料任务强制完成
        /// <summary>
        /// 退余料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishClientReturnPallet(MachineMaterialInput.FinishClientReturnPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退余料任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退余料任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishClientReturnPallet",
            }, () => new MachineMaterialBusinessProce().FinishClientReturnPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退余料任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishClientReturnPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退余料任务回收
        /// <summary>
        /// 退余料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.RecycleClientReturnPallet(MachineMaterialInput.RecycleClientReturnPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退余料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退余料任务回收",
                CommonName = "api/wms/MachineMaterialService/RecycleClientReturnPallet",
            }, () => new MachineMaterialBusinessProce().RecycleClientReturnPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退余料任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecycleClientReturnPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }

        #endregion

        #region 退余料任务部分完成
        /// <summary>
        /// 退余料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.PartionFinishClientReturnPallet(MachineMaterialInput.PartionFinishClientReturnPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退余料任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退余料任务部分完成",
                CommonName = "api/wms/MachineMaterialService/PartionFinishClientReturnPallet",
            }, () => new MachineMaterialBusinessProce().PartionFinishClientReturnPallet(model));
        }
        #endregion

        #region 退余料任务完成
        /// <summary>
        /// 退余料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishReturnPalletTask(MachineMaterialInput.FinishReturnPalletTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退余料任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退余料任务完成",
                CommonName = "api/wms/MachineMaterialService/FinishReturnPalletTask",
            }, () => new MachineMaterialBusinessProce().FinishReturnPalletTask(model));
        } 
        #endregion

        #region 人工领料确认
        /// <summary>
        /// 人工领料确认
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ConfirmPersonReceiveMateriel(MachineMaterialInput.ConfirmPersonReceiveMaterielModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.PickReturn_htid))
            {
                throw new WmsException(500, "人工领料单据id不能为空");
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工领料单{model.PickReturn_htid}确认",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工领料确认",
                CommonName = "api/wms/MachineMaterialService/ConfirmPersonReceiveMateriel",
            }, () => new MachineMaterialBusinessProce().ConfirmPersonReceiveMateriel(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='确认人工领料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ConfirmPersonReceiveMateriel(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 人工领料完成
        /// <summary>
        /// 人工领料完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishPersonReceiveMateriel(MachineMaterialInput.FinishPersonReceiveMaterielModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.PickReturn_htid))
            {
                throw new WmsException(500, "人工领料单据id不能为空");
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工领料单{model.PickReturn_htid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工领料完成",
                CommonName = "api/wms/MachineMaterialService/FinishPersonReceiveMateriel",
            }, () => new MachineMaterialBusinessProce().FinishPersonReceiveMateriel(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='完成人工领料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishPersonReceiveMateriel(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }

        #endregion

        #region 人工领料任务强制完成
        /// <summary>
        /// 人工领料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishPersonReceiveMaterielTask(MachineMaterialInput.FinishPersonReceiveMaterielTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工领料任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工领料任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishPersonReceiveMaterielTask",
            }, () => new MachineMaterialBusinessProce().FinishPersonReceiveMaterielTask(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='人工领料任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishPersonReceiveMaterielTask(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 人工领料任务回收
        /// <summary>
        /// 人工领料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.RecyclePersonReceiveMaterielTask(MachineMaterialInput.RecyclePersonReceiveMaterielTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工领料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工领料任务回收",
                CommonName = "api/wms/MachineMaterialService/RecyclePersonReceiveMaterielTask",
            }, () => new MachineMaterialBusinessProce().RecyclePersonReceiveMaterielTask(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='人工领料任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecyclePersonReceiveMaterielTask(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 人工退料确认
        /// <summary>
        /// 人工退料确认
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ConfirmPersonReturnMateriel(MachineMaterialInput.ConfirmPersonReturnMaterielModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.PickReturn_htid))
            {
                throw new WmsException(500, "人工退料单据id不能为空");
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工退料单{model.PickReturn_htid}确认",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工退料确认",
                CommonName = "api/wms/MachineMaterialService/ConfirmPersonReturnMateriel",
            }, () => new MachineMaterialBusinessProce().ConfirmPersonReturnMateriel(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='确认人工退料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ConfirmPersonReturnMateriel(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 人工退料完成
        /// <summary>
        /// 人工退料完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishPersonReturnMateriel(MachineMaterialInput.FinishPersonReturnMaterielModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.PickReturn_htid))
            {
                throw new WmsException(500, "人工退料单据id不能为空");
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工退料单{model.PickReturn_htid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工退料完成",
                CommonName = "api/wms/MachineMaterialService/FinishPersonReturnMateriel",
            }, () => new MachineMaterialBusinessProce().FinishPersonReturnMateriel(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='完成人工退料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishPersonReturnMateriel(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }

        #endregion

        #region 人工退料任务强制完成
        /// <summary>
        /// 人工退料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishPersonReturnMaterielTask(MachineMaterialInput.FinishPersonReturnMaterielTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工退料任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工退料任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishPersonReturnMaterielTask",
            }, () => new MachineMaterialBusinessProce().FinishPersonReturnMaterielTask(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='人工领料任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishPersonReturnMaterielTask(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 人工退料任务回收
        /// <summary>
        /// 人工退料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.RecyclePersonReturnMaterielTask(MachineMaterialInput.RecyclePersonReturnMaterielTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"人工退料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "人工退料任务回收",
                CommonName = "api/wms/MachineMaterialService/RecyclePersonReturnMaterielTask",
            }, () => new MachineMaterialBusinessProce().RecyclePersonReturnMaterielTask(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='人工退料任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecyclePersonReturnMaterielTask(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退空托盘+要料
        /// <summary>
        /// 退空托盘+要料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ReturnNullAndCallPallet(MachineMaterialInput.ReturnNullAndCallPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            #endregion

            //return WmsHelper.Do(new UserLogModel()
            //{
            //    LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}退空托盘+要料",
            //    LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
            //    TypeCode = "退空托盘+要料",
            //    CommonName = "api/wms/MachineMaterialService/ReturnNullAndCallPallet",
            //}, () => new MachineMaterialBusinessProce().ReturnNullAndCallPallet(model));

            return new MachineMaterialBusinessProce().ReturnNullAndCallPallet(model);
        } 
        #endregion

        #region 退空托盘
        /// <summary>
        /// 退空托盘
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ClientReturnNullPallet(MachineMaterialInput.ClientReturnNullPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}退空托盘",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘",
                CommonName = "api/wms/MachineMaterialService/ClientReturnNullPallet",
            }, () => new MachineMaterialBusinessProce().ClientReturnNullPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退空托盘'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ClientReturnNullPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退空托盘任务强制完成
        /// <summary>
        /// 退空托盘任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishClientReturnNullPallet(MachineMaterialInput.FinishClientReturnNullPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退空托盘任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishClientReturnNullPallet",
            }, () => new MachineMaterialBusinessProce().FinishClientReturnNullPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退空托盘任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishClientReturnNullPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退空托盘任务回收
        /// <summary>
        /// 退空托盘任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebJsonInfo RecycleClientReturnNullPallet(MachineMaterialInput.RecycleClientReturnNullPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退空托盘任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘任务回收",
                CommonName = "api/wms/MachineMaterialService/RecycleClientReturnNullPallet",
            }, () => new MachineMaterialBusinessProce().RecycleClientReturnNullPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退空托盘任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecycleClientReturnNullPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退空托盘任务部分完成
        /// <summary>
        /// 退空托盘任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.PartionFinishReturnNullPallet(MachineMaterialInput.PartionFinishReturnNullPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退空托盘任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘任务部分完成",
                CommonName = "api/wms/MachineMaterialService/PartionFinishReturnNullPallet",
            }, () => new MachineMaterialBusinessProce().PartionFinishReturnNullPallet(model));
        }
        #endregion

        #region 退空托盘任务完成
        /// <summary>
        /// 退空托盘任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishReturnNullPallet(MachineMaterialInput.FinishReturnNullPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退空托盘任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘任务完成",
                CommonName = "api/wms/MachineMaterialService/FinishReturnNullPallet",
            }, () => new MachineMaterialBusinessProce().FinishReturnNullPallet(model));
        } 
        #endregion

        #region 退坏料
        /// <summary>
        /// 退坏料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ClientReturnBadPallet(MachineMaterialInput.ClientReturnBadPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Details))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "退坏料明细不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            //if (string.IsNullOrWhiteSpace(model.TargetDockId))
            //{
            //    return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "退坏料的目的站台不可为空", json = null };
            //}
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}退坏料",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退坏料",
                CommonName = "api/wms/MachineMaterialService/ClientReturnBadPallet",
            }, () => new MachineMaterialBusinessProce().ClientReturnBadPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退坏料'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ClientReturnBadPallet(model);

            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退坏料任务强制完成
        /// <summary>
        /// 退坏料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishClientReturnBadPallet(MachineMaterialInput.FinishClientReturnBadPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退坏料任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退坏料任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishClientReturnBadPallet",
            }, () => new MachineMaterialBusinessProce().FinishClientReturnBadPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退坏料任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishClientReturnBadPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退坏料任务回收
        /// <summary>
        /// 退坏料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebJsonInfo RecycleClientReturnBadPallet(MachineMaterialInput.RecycleClientReturnBadPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退坏料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退坏料任务回收",
                CommonName = "api/wms/MachineMaterialService/RecycleClientReturnBadPallet",
            }, () => new MachineMaterialBusinessProce().RecycleClientReturnBadPallet(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退坏料任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecycleClientReturnBadPallet(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退坏料任务部分完成
        /// <summary>
        /// 退坏料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.PartionFinishReturnBadPallet(MachineMaterialInput.PartionFinishReturnBadPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退坏料任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退坏料任务部分完成",
                CommonName = "api/wms/MachineMaterialService/PartionFinishReturnBadPallet",
            }, () => new MachineMaterialBusinessProce().PartionFinishReturnBadPallet(model));
        }
        #endregion

        #region 退坏料任务完成
        /// <summary>
        /// 退坏料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishReturnBadPallet(MachineMaterialInput.FinishReturnBadPalletModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退坏料任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退坏料任务完成",
                CommonName = "api/wms/MachineMaterialService/FinishReturnBadPallet",
            }, () => new MachineMaterialBusinessProce().FinishReturnBadPallet(model));
        } 
        #endregion

        #region 退废料
        /// <summary>
        /// 退废料
        /// </summary>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ReturnWasteMaterials(MachineMaterialInput.ReturnWasteMaterialsModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            #endregion

            string errmsg = "OK";
            WebJsonInfo retInfo = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}退废料",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退废料",
                CommonName = "api/wms/MachineMaterialService/ReturnWasteMaterials",
            }, () =>
            {
                new MachineMaterialBusinessProce().ReturnWasteMaterials(model, out WebJsonInfo info);
                if (info.errmsg != errmsg)
                {
                    errmsg = "退废料任务已产生，但发送报文到WCS有错误发生：" + info.errmsg;
                }
            });
            //retInfo.errmsg = errmsg;
            return retInfo;

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    new MachineMaterialBusinessProce().ReturnWasteMaterials(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退废料任务强制完成
        /// <summary>
        /// 退废料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishReturnWasteMaterials(MachineMaterialInput.FinishReturnWasteMaterialsModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退废料任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退废料任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishReturnWasteMaterials",
            }, () => new MachineMaterialBusinessProce().FinishReturnWasteMaterials(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退废料任务强制完成'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().FinishReturnWasteMaterials(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退废料任务回收
        /// <summary>
        /// 退废料任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.RecycleReturnWasteMaterials(MachineMaterialInput.RecycleReturnWasteMaterialsModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退废料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退废料任务回收",
                CommonName = "api/wms/MachineMaterialService/RecycleReturnWasteMaterials",
            }, () => new MachineMaterialBusinessProce().RecycleReturnWasteMaterials(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='退废料任务回收'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().RecycleReturnWasteMaterials(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 退废料任务部分完成
        /// <summary>
        /// 退废料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.PartionFinishReturnWasteMaterials(MachineMaterialInput.PartionFinishReturnWasteMaterialsModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退废料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退废料任务回收",
                CommonName = "api/wms/MachineMaterialService/FinishWasteMaterialsTask",
            }, () => new MachineMaterialBusinessProce().PartionFinishReturnWasteMaterials(model));
        }
        #endregion


        #region 退废料任务完成
        /// <summary>
        /// 退废料任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishWasteMaterialsTask(MachineMaterialInput.FinishWasteMaterialsModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"退废料任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退废料任务回收",
                CommonName = "api/wms/MachineMaterialService/FinishWasteMaterialsTask",
            }, () => new MachineMaterialBusinessProce().FinishWasteMaterialsTask(model));
        } 
        #endregion

        #region 批量退料
        /// <summary>
        /// 批量退料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ChangeCard(MachineMaterialInput.ChangeCardModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Details))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "余料明细不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"机台{model.Machine}批量退料",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "批量退料",
                CommonName = "api/wms/MachineMaterialService/ChangeCard",
            }, () => new MachineMaterialBusinessProce().ChangeCard(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='换牌'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ChangeCard(model);

            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }
        #endregion

        #region 交班
        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ReturnMaterielToNext(MachineMaterialInput.ReturnMaterielToNextModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.IsChangePersonReceive))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否交接人工领料数据不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Details))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "交班时余料明细不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},班次{model.ShiftId},班组{model.TeamId}交班",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "交班",
                CommonName = "api/wms/MachineMaterialService/ReturnMaterielToNext",
            }, () => new MachineMaterialBusinessProce().ReturnMaterielToNext(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='交班'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ReturnMaterielToNext(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }

        #endregion

        #region 接班
        /// <summary>
        /// 接班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ReturnMaterielReturnBill(MachineMaterialInput.ReturnMaterielReturnBillModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.IsChangePersonReceive))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否交接人工领料数据", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Details))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "接班时余料明细不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},班次{model.ShiftId},班组{model.TeamId}接班",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "接班",
                CommonName = "api/wms/MachineMaterialService/ReturnMaterielReturnBill",
            }, () => new MachineMaterialBusinessProce().ReturnMaterielReturnBill(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='接班'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().ReturnMaterielReturnBill(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }

        #endregion

        #region 接班前修改物料数量
        /// <summary>
        /// 接班前修改物料数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.EditMaterielBillModel(MachineMaterialInput.EditMaterielBillModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Details))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "余料明细不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},班次{model.ShiftId},班组{model.TeamId}接班前确认物料数量",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "接班前确认物料数量",
                CommonName = "api/wms/MachineMaterialService/EditMaterielBillModel",
            }, () => new MachineMaterialBusinessProce().EditMaterielBillModel(model));

            //WebJsonInfo info = new WebJsonInfo();
            //try
            //{
            //    DBHelper.BeginTransaction();
            //    string msg = $"{DateTime.Now}\r\n  type='接班前修改物料数量'\r\n  model={JsonConvert.SerializeObject(model)}\r\n";
            //    LogHelper.LogInformation(msg);
            //    new MachineMaterialBusinessProce().EditMaterielBillModel(model);
            //    DBHelper.EndTransaction();
            //}
            //catch (Exception ex)
            //{
            //    info.errcode = 500;
            //    info.errmsg = ex.Message;
            //    info.isSucceed = false;
            //    LogHelper.LogError(ex.Message);
            //    LogHelper.LogError(ex.StackTrace);
            //    DBHelper.EndTransaction(false);
            //}
            //return info;
        }

        #endregion

        #region 根据货位获取执行任务的堆垛机入参
        WebJsonInfo IMachineMaterialService.GetEquipmentByLoc(MachineMaterialInput.GetEquipmentByLocModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Row))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "行号不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.LocationId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "货位id不可为空", json = null };
            }
            #endregion

            //return WmsHelper.Do(new UserLogModel()
            //{
            //    LogText = $"行号：{model.Row},货位id：{model.LocationId}",
            //    LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
            //    TypeCode = "机台要料",
            //    CommonName = "api/wms/MachineMaterialService/GetEquipmentByLoc",
            //}, () => new MachineMaterialBusinessProce().GetEquipmentByLoc(model));


            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "根据货位获取执行任务的堆垛机";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(model);
            logModel.CommonName = "api/wms/MachineMaterialService/GetEquipmentByLoc";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                re = new MachineMaterialBusinessProce().GetEquipmentByLoc(model);
                DBHelper.EndTransaction(true);
                re.isSucceed = true;
                logModel.LogText = "获取堆垛机:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "获取堆垛机:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        #endregion

        #region 成品转移
        /// <summary>
        /// 成品转移
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.ProductTransfer(MachineMaterialInput.ProductTransferModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.Machine))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "机台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.Dock))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "站台id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ProductPlanOutId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "生产工单id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.ShiftId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班次id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.TeamId))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "班组id不可为空", json = null };
            }
            if (string.IsNullOrWhiteSpace(model.HasAGV))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "是否产生AGV任务不可为空", json = null };
            }
            #endregion

            string errmsg = "OK";
            WebJsonInfo retInfo = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock}成品转移",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "成品转移",
                CommonName = "api/wms/MachineMaterialService/ProductTransfer",
            }, () =>
            {
                new MachineMaterialBusinessProce().ProductTransfer(model, out WebJsonInfo info);
                if (info.errmsg != errmsg)
                {
                    errmsg = "成品转移任务已产生，但发送报文到WCS有错误发生：" + info.errmsg;
                }
            });
            //retInfo.errmsg = errmsg;
            return retInfo;
        }
        #endregion

        #region 成品转移任务完成
        /// <summary>
        /// 成品转移任务完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishTransfer(MachineMaterialInput.FinishTransferModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"成品转移任务{model.TaskTid}完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "成品转移任务完成",
                CommonName = "api/wms/MachineMaterialService/FinishTransfer",
            }, () => new MachineMaterialBusinessProce().FinishTransfer(model));
        }
        #endregion

        #region 成品转移任务部分完成
        /// <summary>
        /// 成品转移任务部分完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.PartionFinishProductTransferTask(MachineMaterialInput.PartionFinishProductTransferTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"成品转移任务{model.TaskTid}部分完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "成品转移任务部分完成",
                CommonName = "api/wms/MachineMaterialService/PartionFinishProductTransferTask",
            }, () => new MachineMaterialBusinessProce().PartionFinishProductTransferTask(model));
        }
        #endregion

        #region 成品转移任务强制完成
        /// <summary>
        /// 成品转移任务强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.FinishProductTransferTask(MachineMaterialInput.FinishProductTransferTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"成品转移任务{model.TaskTid}强制完成",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "成品转移任务强制完成",
                CommonName = "api/wms/MachineMaterialService/FinishProductTransferTask",
            }, () => new MachineMaterialBusinessProce().FinishProductTransferTask(model));
        }
        #endregion

        #region 成品转移任务回收
        /// <summary>
        /// 成品转移任务回收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        WebJsonInfo IMachineMaterialService.RecycleProductTransferTask(MachineMaterialInput.RecycleProductTransferTaskModel model)
        {
            #region 入参判空
            if (string.IsNullOrWhiteSpace(model.TaskTid))
            {
                return new WebJsonInfo { errcode = 0, isSucceed = false, errmsg = "任务id不可为空", json = null };
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"成品转移任务{model.TaskTid}回收",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "成品转移任务回收",
                CommonName = "api/wms/MachineMaterialService/RecycleProductTransferTask",
            }, () => new MachineMaterialBusinessProce().RecycleProductTransferTask(model));
        }

        #endregion

    }
}
