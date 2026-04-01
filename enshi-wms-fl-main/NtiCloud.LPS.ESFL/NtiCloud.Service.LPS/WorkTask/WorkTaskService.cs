using Microsoft.Extensions.Logging;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Tasking;
using NtiCloud.Service.WMS.Common;
using System;
using Newtonsoft.Json;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;

namespace NtiCloud.Service.WMS.Tasking
{
    public class WorkTaskService : WmsObject, IWorkTaskService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<WorkTaskService> _logger;

        public WorkTaskService(IServiceProvider provider,
            ILogger<WorkTaskService> logger,
            Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            _logger = logger;
            _dbFactory = dbFactory;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);
        }
        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public WebJsonInfo FinishTasks(WorkTaskModel.FinishTasksParam p)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "完成任务";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/FinishTasks";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            //using KernelDataBase kernelDataBase = new KernelDataBase();
            try
            {
                // 检查参数
                if (string.IsNullOrWhiteSpace(p.TaskNoOrIds))
                {
                    throw new Exception("参数不能为空，停止处理");
                }

                DBHelper.BeginTransaction();
                WmsFactory.Make<WorkTask>().FinishWorkTasks(p.TaskNoOrIds);
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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
                logModel.LogText = "完成任务:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public WebJsonInfo FinishOneTask(WorkTaskModel.FinishOneTaskParam p)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "完成任务";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/FinishOneTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            //using KernelDataBase kernelDataBase = new KernelDataBase();
            try
            { 
                DBHelper.BeginTransaction();
                WmsFactory.Make<WorkTask>().FinishOneTask(p.TaskNoOrID, p.Qty);
                DBHelper.EndTransaction();
                info.isSucceed = true;
                logModel.LogText = "完成任务:成功";
                //DBHelper.EndTransaction(false);
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
                logModel.LogText = "完成任务:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogText += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }

        /// <summary>
        /// 回收任务
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public WebJsonInfo RecycleTask(WorkTaskModel.RecycleTaskParam p)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "回收任务";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/RecycleTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;           

            //using KernelDataBase kernelDataBase = new KernelDataBase();
            try
            { // 检查参数
                if (string.IsNullOrWhiteSpace(p.TaskNoOrIds))
                {  
                    throw new Exception("参数不能为空，停止处理"); 
                }
                DBHelper.BeginTransaction();
                WmsFactory.Make<WorkTask>().RecycleTasks(p.TaskNoOrIds);
                DBHelper.EndTransaction();
                info.isSucceed = true;
                logModel.LogText = "回收任务:成功";
                //DBHelper.EndTransaction(false);
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
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
                info.isSucceed = false;
                logModel.LogText = "回收任务:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogText += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;

        }

        /// <summary>
        /// 入库申请
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public WebJsonInfo ApplyInTask(WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "入库申请";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(applyInfo);
            logModel.CommonName = "/api/wms/WorkTask/ApplyInTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();
                re = task.ApplyTask(applyInfo);
                DBHelper.EndTransaction(true);
                re.isSucceed = true;
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
                logModel.CriticalLevel =(decimal) CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;

        }
        /// <summary>
        /// 任务强制执行中
        /// </summary>
        /// <param name="p">任务id，多个任务id用,号隔开</param>
        /// <returns></returns>
        public WebJsonInfo ForceTaskExcuting(WorkTaskModel.FinishTasksParam p)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "任务强制执行中";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/ForceTaskExcuting";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();
                if (string.IsNullOrEmpty(p.TaskNoOrIds)) {
                    throw new Exception("请提供需要强制执行中的任务ID");
                }
                string[] taskIds = p.TaskNoOrIds.Split(',');
                foreach (string tid in taskIds)
                {
                    bool isSuccess = task.ForceTaskExecuting(tid);
                    if (!isSuccess) {
                        throw new Exception("任务:" + tid + "强制执行中失败");
                    }
                }
                DBHelper.EndTransaction(true);
                re.isSucceed = true;
                logModel.LogText = "任务强制执行中:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "任务强制执行中:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        /// <summary>
        /// 强制未执行
        /// </summary>
        /// <param name="p">任务id，多个任务id用,号隔开</param>
        /// <returns></returns>
        public WebJsonInfo ForceTaskNotExcute(WorkTaskModel.FinishTasksParam p)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "强制未执行";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/ForceTaskNotExcute";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();
                if (string.IsNullOrEmpty(p.TaskNoOrIds))
                {
                    throw new Exception("请提供需要强制执行中的任务ID");
                }
                string[] taskIds = p.TaskNoOrIds.Split(',');
                foreach (string tid in taskIds)
                {
                    bool isSuccess = task.ForceTaskNotExecute(tid);
                    if (!isSuccess)
                    {
                        throw new Exception("任务:" + tid + "强制未执行失败");
                    }
                }
                DBHelper.EndTransaction(true);
                re.isSucceed = true;
                logModel.LogText = "强制未执行：成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "强制未执行：失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            return re;
        }

        /// <summary>
        /// 任务部分完成
        /// <br>针对下架任务，释放原货位</br>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public WebJsonInfo PartionCompleteTask(WorkTaskModel.FinishTasksParam p) {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "任务部分完成";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/PartionCompleteTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();

                string[] taskIds = p.TaskNoOrIds.Split(',');
                foreach (string tid in taskIds)
                {
                    bool isSuccess = task.PartionCompleteTask(tid);
                    if (!isSuccess)
                    {
                        throw new Exception("任务:" + tid + "部分完成失败");
                    }
                }
                DBHelper.EndTransaction(true);
                re.isSucceed = true;
                logModel.LogText = "任务部分完成:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "任务部分完成:失败";
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
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "呼叫空托盘";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(applyInfo);
            logModel.CommonName = "/api/wms/WorkTask/CalloutEmptyContainer";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();
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

        public WebJsonInfo ReplaceItems(WorkTaskModel.ReplaceItemsParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"替换物料{p.TaskNoOrIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "替换物料",
                CommonName = "api/wms/WorkTask/ReplaceItems",
            },
            () => new ItemReplacement().ReplaceItems(p.TaskNoOrIds));
        }

        public WebJsonInfo CancelItemReplacement(WorkTaskModel.CancelItemReplacementParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"取消替换物料{p.ReplacementIds.ToString()}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "取消替换物料",
                CommonName = "api/wms/WorkTask/CancelItemReplacement",
            },
            () => new ItemReplacement().CancelItemReplacement(p.ReplacementIds.ToString()));
        }

        public WebJsonInfo ConfirmItemReplacement(WorkTaskModel.ConfirmItemReplacementParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"确认替换物料{p.ReplacementIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "确认替换物料",
                CommonName = "api/wms/WorkTask/ConfirmItemReplacement",
            },
            () => new ItemReplacement().ConfirmItemReplacement(p.ReplacementIds));
        }

        public WebJsonInfo AddItemReplacement(WorkTaskModel.AddItemReplacementParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"增加替换物料{p.ItemBarcode}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "增加替换物料",
                CommonName = "api/wms/WorkTask/AddItemReplacement",
            },
            () => new ItemReplacement().AddItemReplacement(p));
        }

        public WebJsonInfo ModifyReplacement(WorkTaskModel.ModifyReplacementParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"修改替换物料货位{p.LocationNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "修改替换物料",
                CommonName = "api/wms/WorkTask/ModifyReplacement",
            },
            () => new ItemReplacement().ModifyReplacement(p));
        }
      
        public WebJsonInfo StartReplace(WorkTaskModel.StartReplaceParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"启动替换物料{p.ReplacementIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "启动替换物料",
                CommonName = "api/wms/WorkTask/StartReplace",
            },
            () => new ItemReplacement().StartReplace(p.ReplacementIds));
        }
        public WebJsonInfo FinishReplace(WorkTaskModel.FinishReplaceParam p)
        {
            return WmsHelper.Do(new Model.UserLogModel()
            {
                LogText = $"启动替换物料确认入库{p.ReplacementIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "启动替换物料确认入库",
                CommonName = "api/wms/WorkTask/FinishReplace",
            },
            () => new ItemReplacement().FinishReplace(p.ReplacementIds));
        }

        ///// <summary>
        ///// 货位二次分配
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //WebJsonInfo IWorkTaskService.SecondDistributionLocation(WorkTaskModel.SecondDistribuLocRequest model)
        //{
        //    WebJsonInfo re = new WebJsonInfo();
        //    if (model.Msg_Params == null || model.Msg_Params.Count <= 0)
        //    {
        //        re.errcode = -200;
        //        re.isSucceed = false;
        //        re.errmsg = "请求体Msg_Params不能为空";
        //        re.json = null;
        //        return re;
        //    }
        //    if (string.IsNullOrEmpty(model.Msg_Params[0].WarehouseID))
        //    {
        //        re.errcode = -200;
        //        re.isSucceed = false;
        //        re.errmsg = "仓库id不能为空";
        //        re.json = null;
        //        return re;
        //    }
        //    if (string.IsNullOrEmpty(model.Msg_Params[0].ZoneID))
        //    {
        //        re.errcode = -200;
        //        re.isSucceed = false;
        //        re.errmsg = "区域id不能为空";
        //        re.json = null;
        //        return re;
        //    }
        //    if (string.IsNullOrEmpty(model.Msg_Params[0].TaskNum))
        //    {
        //        re.errcode = -200;
        //        re.isSucceed = false;
        //        re.errmsg = "任务号不能为空";
        //        re.json = null;
        //        return re;
        //    }
        //    if (string.Equals(model.Msg_Params[0].ZoneID, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
        //    {//二楼
        //        re = OutBoundSecondDistributionLocation(model);
        //    }
        //    else
        //    {//一楼

        //    }
        //    Model.UserLogModel logModel = new Model.UserLogModel();
        //    logModel.ModifyIP = Context.UserInfo.LoginIP;
        //    logModel.WarehouseID = Context.WarehouseID;
        //    logModel.TypeCode = "入库货位二次分配";
        //    logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(model);
        //    logModel.CommonName = "/api/wms/WorkTask/SecondDistributionLocation";
        //    logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
        //    logModel.CreatedBy = Context.UserInfo.UserId;
        //    try
        //    {
        //        DBHelper.BeginTransaction();
        //        WorkTask task = new WorkTask();
        //        WorkTaskModel.SecondDistribuLocResponse sp=task.SecondDistributionLocation(model);
        //        re.json = sp.Msg_Params;
        //        re.isSucceed = true;
        //        DBHelper.EndTransaction(true);
        //        logModel.LogText = "入库货位二次分配:成功";
        //    }
        //    catch (Exception ex)
        //    {
        //        re.errcode = -200;
        //        re.isSucceed = false;
        //        re.errmsg = ex.Message;
        //        LogHelper.LogError(ex.Message);
        //        LogHelper.LogError(ex.StackTrace);
        //        DBHelper.EndTransaction(false);
        //        logModel.LogText = "入库货位二次分配:失败";
        //        logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
        //    }
        //    logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
        //    UserLog.Add(logModel);
        //    return re;
        //}

        /// <summary>
        /// 出库货位二次分配
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo IWorkTaskService.OutBoundSecondDistributionLocation(WorkTaskModel.SecondDistribuLocRequest p)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "出库货位二次分配";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/OutBoundSecondDistributionLocation";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();
                WorkTaskModel.SecondDistribuLocResponse sp = task.OutBoundSecondDistributionLocation(p);
                re.json = sp.Msg_Params;// Newtonsoft.Json.JsonConvert.SerializeObject(sp.Msg_Params);
                re.isSucceed = true;
                DBHelper.EndTransaction(true);
                logModel.LogText = "出库货位二次分配:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "出库货位二次分配:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        /// <summary>
        /// 入库货位二次分配
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        WebJsonInfo IWorkTaskService.InBoundSecondDistributionLocation(WorkTaskModel.SecondDistribuLocRequest p)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "入库货位二次分配";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(p);
            logModel.CommonName = "/api/wms/WorkTask/InBoundSecondDistributionLocation";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                WorkTask task = new WorkTask();
                WorkTaskModel.SecondDistribuLocResponse sp = task.InBoundSecondDistributionLocation(p);
                re.json = sp.Msg_Params;//Newtonsoft.Json.JsonConvert.SerializeObject(sp.Msg_Params);
                re.isSucceed = true;
                DBHelper.EndTransaction(true);
                logModel.LogText = "入库货位二次分配:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "入库货位二次分配:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 手工下发(重发)任务给WCS
        /// </summary>
        /// <param name="taskids"></param>
        /// <returns></returns>
        public WebJsonInfo SendTaskToWcs(string taskids)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.WarehouseID = Context.WarehouseID;
            logModel.TypeCode = "手工下发(重发)任务给WCS";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(new { taskids= taskids });
            logModel.CommonName = "/api/wms/WorkTask/SendTaskToWcs";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                string[] ids = taskids.Split(",");
                re =WorkTaskInterface.Send(new System.Collections.Generic.List<string>(ids));
                if (re.errcode == 0) {
                    WorkTask task = new WorkTask();
                    foreach (string tid in ids)
                    {
                        task.UpdateTaskStatus(tid,
                            Base.TaskStatus.TaskStatus_Executing.ToString(),
                            Base.TaskStatus.TaskStatus_NotExecute.ToString()
                            );
                    }
                }
                DBHelper.EndTransaction(true);
                logModel.LogText = "手工下发(重发)任务给WCS:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "手工下发(重发)任务给WCS:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
    }
}
