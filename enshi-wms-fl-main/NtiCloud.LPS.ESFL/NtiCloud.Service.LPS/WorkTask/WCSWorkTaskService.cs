using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Expressions;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Base;
using NtiCloud.Service.LPS.Inbound;
using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Tasking
{
    class WCSWorkTaskService : IWCSTaskService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<WCSWorkTaskService> _logger;
        public WCSWorkTaskService(IServiceProvider provider,
        ILogger<WCSWorkTaskService> logger,
        Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            _logger = logger;
            _dbFactory = dbFactory; 
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory); 
        }


        public WebJsonInfo FinishTasks(WorkTaskModel.FinishTasksParam p)
        {
            WebJsonInfo info = new WebJsonInfo();
             
            //using KernelDataBase kernelDataBase = new KernelDataBase(); 
            Model.UserLogModel logModel = new Model.UserLogModel();            
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "完成任务";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WCSTask/FinishTasks";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;

            try
            {
                initclient(p.WarehouseCode, p.ClientId);
                logModel.WarehouseID = Context.WarehouseID;
                // 检查参数
                if (string.IsNullOrWhiteSpace(p.TaskNoOrIds))
                { 
                    throw new Exception( "参数不能为空，停止处理");
                }                
                DBHelper.BeginTransaction();

                WmsFactory.Make<NtiCloud.Service.WMS.Tasking.WorkTask>().FinishWorkTasks(p.TaskNoOrIds);

                DBHelper.EndTransaction();
                info.isSucceed = true;
                logModel.LogText = "完成任务:成功";
                
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
                    info.errcode = -200;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);

                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
                logModel.LogText = "完成任务:失败";
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);

            return info;
        }

        private void initclient(string warehousecode,string clientid)
        {
            
            Context.ClientID = clientid; 
            Context.User = serviceProvider.GetService<IUserService>().GetUserMode();
            Context.DataSource = warehousecode + ".";
            WarehouseModel wm = new Warehouse().GetWarehouseId(warehousecode);
            if (wm != null) {
                Context.WarehouseID = wm.t_id;
            }
        }

        /// <summary>
        /// 入库申请
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public WebJsonInfo ApplyInTask(WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            LogHelper.LogInformation("入库申请");
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();            
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "入库申请";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(applyInfo)+".at"+DateTime.Now.ToString("HH:mm:ss.fff");
            logModel.CommonName = "/api/wms/WCSTask/ApplyInTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                LogHelper.LogInformation("initclient...");
                initclient(applyInfo.WarehouseCode, applyInfo.ClientId);
                LogHelper.LogInformation("initclient end");
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
                re = task.ApplyTask(applyInfo);
                LogHelper.LogInformation("ApplyTask end");
                DBHelper.EndTransaction(true);
                logModel.LogText = "入库申请:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "入库申请:失败";
                logModel.CriticalLevel= (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re) + ".at" + DateTime.Now.ToString("HH:mm:ss.fff");
            UserLog.Add(logModel);
            return re;

        } 

        /// <summary>
        /// 任务部分完成
        /// <br>针对下架任务，释放原货位</br>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public WebJsonInfo PartionCompleteTask(WorkTaskModel.FinishTasksParam p)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();            
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "部分完成";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WCSTask/PartionCompleteTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                initclient(p.WarehouseCode, p.ClientId);
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();

                string[] taskIds = p.TaskNoOrIds.Split(',');
                foreach (string tno in taskIds)
                {
                    bool isSuccess = task.PartionCompleteTask(tno);
                    if (!isSuccess)
                    {
                        throw new Exception("任务:" + tno + "部分完成失败");
                    }
                }
                DBHelper.EndTransaction(true);
                logModel.LogText = "部分完成:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "部分完成:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        /// <summary>
        /// 呼叫空托盘
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public WebJsonInfo CalloutEmptyContainer(WorkTaskModel.CallEmptyContainerParam applyInfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();            
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "呼叫空托盘";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(applyInfo);
            logModel.CommonName = "/api/wms/WCSTask/CalloutEmptyContainer";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                initclient(applyInfo.WarehouseCode, applyInfo.ClientId);
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
                re.json = task.CallEmptyContainer(applyInfo);
                re.isSucceed = true;
                DBHelper.EndTransaction(true);
                logModel.LogText = "呼叫空托盘:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "呼叫空托盘:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 同步设备状态
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo SyncEquipmentStatus(WorkTaskModel.EquipmentParamIn param)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();            
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "同步设备状态";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(param);
            logModel.CommonName = "/api/wms/WCSTask/SyncEquipmentStatus";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                initclient(param.WarehouseCode, param.ClientId);
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                 
                   new Equipment().SynEquipmentStatus(param.EquipmentCode,param.EquipmentStatus);
                re.isSucceed = true;
                DBHelper.EndTransaction(true);
                logModel.LogText = "同步设备状态:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "同步设备状态:失败";
            }
            logModel.LogTextLong += ";返回:"+Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        public WebJsonInfo ScanBarcodeIntoInv(string containercode, string batch)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "RF采集托盘";
            logModel.LogTextLong = "请求参数:{containercode:" + containercode + ",batch:" + batch + "}";
            logModel.CommonName = "/api/wms/Putaway/ScanBarcodeIntoInv";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                initclient("WMS", "1010203040506070809");
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                var ctn = new Putaway().ScanBarcodeIntoInv(containercode, batch);
                re.isSucceed = true;
                re.json = ctn;
                DBHelper.EndTransaction(true);
                logModel.LogText = "RF采集托盘:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "RF采集托盘:失败";
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 更改入库巷道
        /// </summary>
        /// <param name="taskInfo">任务单号</param>
        /// <returns></returns>
        public WebJsonInfo ChangeInboundChannel(WorkTaskModel.FinishTasksParam taskInfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "更改入库巷道";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(taskInfo);
            logModel.CommonName = "/api/wms/WCSTask/ChangeInboundChannel";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                initclient(taskInfo.WarehouseCode, taskInfo.ClientId);
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
                re.json = task.ChangeInboundChannel(taskInfo.TaskNoOrIds);
                re.errcode = 0;
                re.isSucceed = true;
                DBHelper.EndTransaction(true);
                logModel.LogText = "更改入库巷道:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "更改入库巷道:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        /// <summary>
        /// WCS增加SN日志
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public WebJsonInfo WcsAddSNLog(WorkTaskModel.SNLogIn logInfo) {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "WCS增加SN日志";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(logInfo);
            logModel.CommonName = "/api/wms/WCSTask/WcsAddSNLog";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                initclient(logInfo.WarehouseCode, logInfo.ClientId);
                logModel.WarehouseID = Context.WarehouseID;
                DBHelper.BeginTransaction();
                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
                re.isSucceed = task.AddSNLog(logInfo.sn, logInfo);
                re.errcode = 0;
                DBHelper.EndTransaction(true);
                logModel.LogText = "WCS增加SN日志:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "WCS增加SN日志:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
    }
}
