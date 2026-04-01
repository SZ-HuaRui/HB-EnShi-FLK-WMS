using System;
using System.Collections.Generic;
using System.Text;
using NtiCloud.Interface.Core; 
using NtiCloud.Interface.Core; 
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using NtiCloud.Service.WMS.Common;
using Microsoft.Extensions.Logging;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Interface.WMS;
using NtiCloud.Service.WMS.Tasking;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Inbound;
using System.Linq;

namespace NtiCloud.Service.WMS.Inbound

{
    public class PutawayService : IPutaway
    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private readonly IKernelCachingProvider _redis;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<PutawayService> _logger;
        public PutawayService(IServiceProvider _serviceProvider, IConfiguration configuration, Func<string,
        IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _redis = serviceProvider.GetService<KernelRedisProvider>();
            _configuration = configuration;
            _dbFactory = dbFactory;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);
        }
        #endregion

        /// <summary>
        /// 上架单货位预分配
        /// </summary>
        /// <param name="putawayid">上架单信息</param>
        /// <returns></returns>
        public WebJsonInfo AllocateStorageBin(PutawayModelInput putawayinfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "上架单货位预分配";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(putawayinfo);
            logModel.CommonName = "/api/wms/Putaway/AllocateStorageBin";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            { 
                re = new Putaway(_dbFactory(Context.UserInfo.UserDbConnW.dbType)).AllocateStorageBin(putawayinfo);
                logModel.LogText = "上架单货位预分配:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -1;
                re.errmsg = "异常:" + ex.Message;
                re.isSucceed = false;
                re.json = "";
                logModel.LogText = "上架单货位预分配:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:"+Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
       
        /// <summary>
        /// 保存人工调整后的货位分配信息
        /// </summary>
        /// <param name="modifiedList"></param>
        /// <returns></returns>
        public WebJsonInfo ModifyAllocateStorageBin(List<ModifyAllocLocationIn> modifiedList)
        {
            WebJsonInfo re= new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "手工调整上架单货位预分配";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(modifiedList);
            logModel.CommonName = "/api/wms/Putaway/ModifyAllocateStorageBin";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                Putaway pa = new Putaway(_dbFactory(Context.UserInfo.UserDbConnW.dbType));
                re= pa.ModifyAllocateStorageBin(modifiedList);
                DBHelper.EndTransaction(true);
                logModel.LogText = "手工调整上架单货位预分配:成功";
            }
            catch (Exception ex) { 
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "手工调整上架单货位预分配:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:"+Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 确认货位分配信息，将任务更新为可执行
        /// </summary>
        /// <param name="modifiedList"></param>
        /// <returns></returns>
        public WebJsonInfo ConfirmAllocationLocation(PutawayModelInput putawayid)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "确认货位分配";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(putawayid);
            logModel.CommonName = "/api/wms/Putaway/ConfirmAllocationLocation";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                Putaway pa = new Putaway(_dbFactory(Context.UserInfo.UserDbConnW.dbType));
                re = pa.ConfirmAllocationLocation(putawayid.PutawayId);
                DBHelper.EndTransaction(true);
                logModel.LogText = "确认货位分配:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "确认货位分配:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 完成上架单：将未完成的任务全部完成并增加库存
        /// </summary>
        /// <param name="putawayid"></param>
        /// <returns></returns>
        public WebJsonInfo CompletePutaway(PutawayModelInput putawayid)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "手工完成上架单";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(putawayid);
            logModel.CommonName = "/api/wms/Putaway/CompletePutaway";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                Putaway putaway = new Putaway(_dbFactory(Context.UserInfo.UserDbConnW.dbType)); 
                putaway.CompletePutaway(putawayid.PutawayId); 
                DBHelper.EndTransaction(true);
                logModel.LogText = "手工完成上架单:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "手工完成上架单:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re; 
        }

        /// <summary>
        /// 关闭上架单:将未完成的任务全部完成不增加库存
        /// </summary>
        /// <param name="putawayid">上架单ID</param>
        /// <returns></returns>
        public WebJsonInfo ClosePutaway(PutawayModelInput putawayid)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "手工关闭上架单";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(putawayid);
            logModel.CommonName = "/api/wms/Putaway/ClosePutaway";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                Putaway putaway = new Putaway(_dbFactory(Context.UserInfo.UserDbConnW.dbType));
                putaway.ClosePutaway(putawayid.PutawayId);
                DBHelper.EndTransaction(true);
                logModel.LogText = "手工关闭上架单:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "手工关闭上架单:失败";
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re; 
        }
        /// <summary>
        /// 获取所有待执行任务
        /// </summary>
        /// <param name="taskdingin"></param>
        /// <returns></returns>
        public WebJsonInfo GetTaskListFordoing(TaskdoingIn taskdingin)
        {
            WebJsonInfo re = new WebJsonInfo();
            try
            {

                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
               List<TaskItemModel> lstItem= task.getTaskItemsForDoing(taskdingin.TaskId); ;
                if (lstItem.Count > 0)
                {
                    re.errcode = 0;
                    re.isSucceed = true;
                    re.json = lstItem;                    
                }
                else {
                    re.errcode = 0;
                    re.isSucceed = false;
                    re.json = lstItem;
                    re.errmsg = "没有查询到待执行任务";
                }
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace); 
            }
            return re;
        }
        /// <summary>
        /// 获取任务指定任务下的待执行明细
        /// </summary>
        /// <param name="taskdingin"></param>
        /// <returns></returns>
        public WebJsonInfo GetTaskDetail(TaskdoingIn taskdingin)
        {
            WebJsonInfo re = new WebJsonInfo();
            try
            {

                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
                List<TaskItemModel> lstRe= task.getTaskItemsForDoing(taskdingin.TaskId);
                if (lstRe.Count > 0)
                {
                    re.errcode = 0;
                    re.isSucceed = true;
                    re.json = lstRe;
                   
                }
                else
                {
                    re.errcode = 0;
                    re.isSucceed = false;
                    re.json = lstRe;
                    re.errmsg = "没有查询到待执行任务明细";
                }
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
            }
            return re;
        }
        /// <summary>
        /// 按物料上架到指定货位
        /// </summary>
        /// <param name="putawayToLocation"></param>
        /// <returns></returns>
        public WebJsonInfo DoPutawayToLocation(PutawayToLocation putawayToLocation)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "RF上架";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(putawayToLocation);
            logModel.CommonName = "/api/wms/Putaway/DoPutawayToLocation";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                Putaway putaway = new Putaway();
                re= putaway.DoPutawayToLocation(putawayToLocation, new Putaway.CheckPutawayComplete(putaway.CheckPutawayDone)); 
                DBHelper.EndTransaction(true);
                logModel.LogText = "RF上架:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "RF上架:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 查询可执行任务信息
        /// </summary>
        /// <param name="taskdingin">根据参考单号、货位等</param>
        /// <returns></returns>
        public WebJsonInfo GetLocationByItemCode(TaskdoingIn taskdingin)
        {

            WebJsonInfo re = new WebJsonInfo();
            try
            {

                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
              DataRow[] tsks=  task.GetWorkTasks(taskdingin);
                if (tsks.Count() > 0)
                {
                    re.isSucceed = true;
                    re.json = tsks;
                    re.errcode = 0;
                    re.errmsg = "";
                }
                else {
                    re.isSucceed = false;
                    re.json = tsks;
                    re.errcode = -200;
                    re.errmsg = "没有可执行任务";
                }
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
            }
            return re;
        }

        /// <summary>
        /// 根据物料获取上架任务
        /// </summary>
        /// <param name="taskdingin"></param>
        /// <returns></returns>
        public WebJsonInfo GetTaskListFordoing_RF(TaskdoingIn taskdingin)
        {
            WebJsonInfo re = new WebJsonInfo();
            try
            {
                Context.WarehouseID = taskdingin.WarehouseId;
                NtiCloud.Service.WMS.Tasking.WorkTask task = new NtiCloud.Service.WMS.Tasking.WorkTask();
                List<TaskItemForRfModel> tsks = task.getTaskItemsForDoingRF(taskdingin.WarehouseId, taskdingin.ItemCode); 
                if (tsks.Count() > 0)
                { 
                    re.isSucceed = true;
                    re.json = tsks;
                    re.errcode = 0;
                    re.errmsg = "";
                }
                else
                {
                    re.isSucceed = false;
                    re.json = tsks;
                    re.errcode = -200;
                    re.errmsg = "没有可执行任务";
                }
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
            }
            return re;
        }

        /// <summary>
        /// 扫描上架单号，获取待执行的上架单明细列表
        /// </summary>
        /// <param name="putawayinfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetPutawayItemListForDoing(PutawayDirectIn putawayinfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            try
            {
                Putaway putaway = new Putaway();
                List<PutawayDetailModel> tsks = putaway.GetPutawayDetailForDoing(putawayinfo.PutawayNo);
                foreach (PutawayDetailModel m in tsks) {
                    m.production_date_str = m.production_date ==null? "" : ((DateTime)m.production_date).ToString("yyyy-MM-dd");
                }

                re.isSucceed = true;
                re.json = tsks;
                re.errcode = 0;
                re.errmsg = "";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
            }
            return re;
        }

        /// <summary>
        /// 扫描上架单号与物料号，查询出详细的待上架信息
        /// </summary>
        /// <param name="putawayinfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetPutawayItemDetailsForDoing(PutawayDirectIn putawayinfo)
        {
            WebJsonInfo re = new WebJsonInfo();
            try
            {
                Putaway putaway = new Putaway();
                List<PutawayDetailModel> tsks = putaway.GetPutawayDetailForDoing(putawayinfo.PutawayNo, putawayinfo.ItemCode);
                foreach (PutawayDetailModel m in tsks)
                {
                    m.production_date_str = m.production_date == null ? "" : ((DateTime)m.production_date).ToString("yyyy-mm-dd");
                }
                re.isSucceed = true;
                re.json = tsks;
                re.errcode = 0;
                re.errmsg = "";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
            }
            return re;
        }


        /// <summary>
        /// 上架单直接上架提交（无上架任务直接根据上架明细上架）
        /// </summary>
        /// <param name="putawayinfo"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitPutawayWithoutTask(PutawayDirectDoneIn putawayinfo) {
            WebJsonInfo re = new WebJsonInfo(); 
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "RF上架";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(putawayinfo);
            logModel.CommonName = "/api/wms/Putaway/SubmitPutawayWithoutTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();

                Putaway putaway = new Putaway();
                re = putaway.SubmitPutawayWithoutTask(putawayinfo); 
                DBHelper.EndTransaction(true);
                logModel.LogText = "RF上架:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "RF上架:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        /// <summary>
        /// 获取托盘待上架信息
        /// </summary>
        /// <param name="putawayinfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetPalletInfoForIn(string containerNo) 
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel()
            {
                ModifyIP = Context.UserInfo.LoginIP,
                LogText = $"入库前获取托盘信息",
                LogTextLong = $"参数：containerNo:{containerNo}",
                TypeCode = "入库前获取托盘信息",
                CommonName = "/api/wms/PutawayService/GetPalletInfoForIn",
                CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info,
                CreatedBy = Context.UserInfo.UserId,
            };
            try
            {
                DBHelper.BeginTransaction();
                Putaway putaway = new Putaway();
                re = putaway.GetPalletInfoForIn(containerNo);
                DBHelper.EndTransaction(true);
                logModel.LogText += ":成功";
            }
            catch (Exception ex)
            {
                re = new WebJsonInfo();
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText += "失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:"+Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }

        /// <summary>
        /// 容器/托盘上架到货位
        /// </summary>
        /// <param name="containerId">容器ID</param>
        /// <param name="toLocation">目标货位</param>
        /// <returns></returns>
        public WebJsonInfo PutawayPalletToLocation(string containerId, string toLocation) {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "RF上架";
            logModel.LogTextLong = "请求参数:{containerId:"+ containerId + ",toLocation:"+ toLocation + "}";
            logModel.CommonName = "/api/wms/Putaway/PutawayPalletToLocation";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction(); 
                Putaway putaway = new Putaway();
                re = putaway.PutawayPalletToLocation(containerId, toLocation);
                DBHelper.EndTransaction(true);
                logModel.LogText = "RF上架:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "RF上架:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
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
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                Putaway putaway = new Putaway(); 
                var ctn = putaway.ScanBarcodeIntoInv(containercode, batch);
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
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 获取烟包替换任务
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo GetItemReplaceList()
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "RF获取烟包替换任务";
            logModel.LogTextLong = "请求参数:{}";
            logModel.CommonName = "/api/wms/Putaway/GetItemReplaceList";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                ItemReplacement replace = new ItemReplacement();
                var ctn = replace.GetItemReplaceList();
                re.isSucceed = true;
                re.json = ctn;
                DBHelper.EndTransaction(true);
                logModel.LogText = "RF获取烟包替换任务:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "RF获取烟包替换任务:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
        /// <summary>
        /// 提交烟包替换
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitReplace(ItemReplaceInModel info)
        {
            WebJsonInfo re = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "RF提交烟包替换";
            logModel.LogTextLong = "请求参数:"+Newtonsoft.Json.JsonConvert.SerializeObject(info);
            logModel.CommonName = "/api/wms/Putaway/SubmitReplace";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            logModel.CreatedBy = Context.UserInfo.UserId;
            try
            {
                DBHelper.BeginTransaction();
                ItemReplacement replace = new ItemReplacement();
                var ctn = replace.SubmitReplace(info);
                re.isSucceed = true;
                re.json = ctn;
                DBHelper.EndTransaction(true);
                logModel.LogText = "RF提交烟包替换:成功";
            }
            catch (Exception ex)
            {
                re.errcode = -200;
                re.isSucceed = false;
                re.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
                logModel.LogText = "RF提交烟包替换:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(re);
            UserLog.Add(logModel);
            return re;
        }
    }
  
}
