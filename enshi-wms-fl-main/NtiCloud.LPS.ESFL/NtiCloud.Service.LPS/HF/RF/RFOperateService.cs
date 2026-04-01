
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.WMS.HF;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.HF.RF;
using System;
using System.Collections.Generic;
using System.Data; 
using System.Text;

namespace NtiCloud.Service.WMS.HF.RF
{
    class RFOperateService : IRFOperateService
    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<RFOperateService> _logger;
        public RFOperateService(IServiceProvider _serviceProvider, IConfiguration configuration, Func<string,
        IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _configuration = configuration;
            _dbFactory = dbFactory;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);
            Context.DataSource = "WMS.";
            Context.WarehouseID = "57274b9c55464753b8bd7a0df96421f2";
            Context.ClientID = "1010203040506070809";

        }
        #endregion

        /// <summary>
        ///登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public WebJsonInfo Login(string account, string pwd)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF登录";
            logModel.LogTextLong = "请求参数:{account:" + account + ",pwd:" + pwd + "}";
            logModel.CommonName = "/api/wms/RFOperate/Login";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                UserInfo user = WmsFactory.Make<RFOperation>().Login(account, pwd);
                info.isSucceed = true;
                info.json = user;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF登录:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF登录:失败";
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
                logModel.LogText = "醇化库RF登录:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;

            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetWarehouseList(UserInfo userinfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取仓库列表";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetWarehouseList";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().GetWarehouseList(userinfo);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取仓库列表:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取仓库列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
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
                logModel.LogText = "醇化库RF获取仓库列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetLocationList(UserInfo userinfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取货位列表";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetLocationList";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().getLocationList(userinfo);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取货位列表:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取货位列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
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
                logModel.LogText = "醇化库RF获取货位列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }

        /// <summary>
        /// 获取基础数据
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public WebJsonInfo GetSysTypeList(string flag)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().getSysType(flag);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
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
        /// 获取任务列表
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetOutboundTaskList(UserInfo userinfo, SynOutboundTaskIn searchInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取任务列表";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetOutboundTaskList";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().getOutboundTaskList(userinfo,
                    searchInfo.WarehouseId, searchInfo.Floor, searchInfo.Room, searchInfo.ref_order_no, true);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取任务列表:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取任务列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
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
                logModel.LogText = "醇化库RF获取任务列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 获取任务列表:不获取条码
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public WebJsonInfo GetOutboundTaskListNew(UserInfo userinfo, SynOutboundTaskIn searchInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取任务列表";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetOutboundTaskListNew";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().getOutboundTaskList(userinfo,
                    searchInfo.WarehouseId, searchInfo.Floor, searchInfo.Room, searchInfo.ref_order_no, false);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取任务列表:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取任务列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
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
                logModel.LogText = "醇化库RF获取任务列表:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }


        /// <summary>
        /// 获取出库货位条码明细
        /// </summary>
        /// <param name="outboundOrdedetailid"></param>
        /// <returns></returns>
        public WebJsonInfo GetOutboundTaskDetail(string outboundOrdedetailid)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取出库货位条码明细";
            logModel.LogTextLong = "请求参数:{outboundOrdedetailid:" + outboundOrdedetailid + "}";
            logModel.CommonName = "/api/wms/RFOperate/GetOutboundTaskList";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().GetoutboundTaskDetail(outboundOrdedetailid);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取出库货位条码明细:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取出库货位条码明细:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
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
                logModel.LogText = "醇化库RF获取出库货位条码明细:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }

        public WebJsonInfo GetWarehouseFloor(string warehouseId)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().GetFloorByWarehouse(warehouseId);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
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

        public WebJsonInfo GetRoomByFloor(string warehouseId, string floor)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                DBHelper.BeginTransaction();
                var wmList = WmsFactory.Make<RFOperation>().GetRoomByFloor(warehouseId, floor);
                info.isSucceed = true;
                info.json = wmList;
                DBHelper.EndTransaction();
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
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
        /// 醇化库RF出库扫码提交
        /// </summary>
        /// <param name="pickedInfo"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitOutboundTask(SubmitOutboudDetailIn pickedInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF出库扫码提交";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(pickedInfo);
            logModel.CommonName = "/api/wms/WCSTask/SubmitOutboundTask";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                if (pickedInfo.lstDetails == null || pickedInfo.lstDetails.Count == 0)
                {
                    throw new Exception("提交的条码清单为空");
                }
                bool re = WmsFactory.Make<RFOperation>().UpOutTaskInfoNew(pickedInfo);
                info.isSucceed = true;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF出库扫码提交:成功";
            }
            catch (CommitWhenException ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                DBHelper.EndTransaction();
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
                logModel.LogText = "完成任务:失败";
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
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
                logModel.LogText = "醇化库RF出库扫码提交:失败";
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 醇化库RF批量出库扫码提交
        /// </summary>
        /// <param name="pickedInfos"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitOutboundTaskList(List<SubmitOutboudDetailIn> pickedInfos)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF批量出库扫码提交";
            logModel.LogTextLong = "SubmitOutboundTaskList:" + Newtonsoft.Json.JsonConvert.SerializeObject(pickedInfos);
            logModel.CommonName = "/api/wms/RFOperate/SubmitOutboundTaskList";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                Dictionary<string, object> erroDict = new Dictionary<string, object>();
                foreach (SubmitOutboudDetailIn onepick in pickedInfos)
                {
                    try
                    {
                        DBHelper.BeginTransaction();
                        var wmList = WmsFactory.Make<RFOperation>().UpOutTaskInfoNew(onepick);
                        DBHelper.EndTransaction();
                    }
                    catch (CommitWhenException ex)
                    {
                        if (erroDict.ContainsKey(onepick.OutboundOrderDetailId))
                        {
                            erroDict[onepick.OutboundOrderDetailId] = erroDict[onepick.OutboundOrderDetailId] + ";" + ex.Message;
                        }
                        else
                        {
                            erroDict.Add(onepick.OutboundOrderDetailId, ex.Message);
                        }
                        DBHelper.EndTransaction();
                    }
                    catch (Exception ex)
                    {
                        if (erroDict.ContainsKey(onepick.OutboundOrderDetailId))
                        {
                            erroDict[onepick.OutboundOrderDetailId] = erroDict[onepick.OutboundOrderDetailId] + ";" + ex.Message;
                        }
                        else
                        {
                            erroDict.Add(onepick.OutboundOrderDetailId, ex.Message);
                        }
                        DBHelper.EndTransaction(false);
                    }
                }
                info.isSucceed = true;
                info.json = erroDict;
                logModel.LogText = "醇化库RF批量出库扫码提交:成功";
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
                logModel.LogText = "醇化库RF批量出库扫码提交:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 按条码返回处理结果
        /// </summary>
        /// <param name="pickedInfos"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitOutboundTaskList3(List<SubmitOutboudDetailIn> pickedInfos)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF批量出库扫码提交";
            logModel.LogTextLong = "SubmitOutboundTaskList3:" + Newtonsoft.Json.JsonConvert.SerializeObject(pickedInfos);
            logModel.CommonName = "/api/wms/RFOperate/SubmitOutboundTaskList3";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                System.Collections.ArrayList lstRe = new System.Collections.ArrayList(); 
                foreach (SubmitOutboudDetailIn onepick in pickedInfos)
                {
                    try
                    {
                        DBHelper.BeginTransaction();
                        System.Collections.ArrayList re = WmsFactory.Make<RFOperation>().UpOutTaskInfoNew2(onepick); 
                        lstRe.Add(new { OutboundOrderDetailId = onepick.OutboundOrderDetailId, re = re });
                        DBHelper.EndTransaction();
                    }
                    catch (CommitWhenException ex)
                    {  
                        DBHelper.EndTransaction();
                    }
                    catch (Exception ex)
                    { 
                        DBHelper.EndTransaction(false);
                    }
                }
                info.isSucceed = true;
                info.json = lstRe;
                logModel.LogText = "醇化库RF批量出库扫码提交:成功";
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
                logModel.LogText = "醇化库RF批量出库扫码提交:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }


        /// <summary>
        /// 后台分配条码对应的任务行
        /// </summary>
        /// <param name="pickedInfos"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitOutboundTaskList2(List<SubmitOutboudDetailIn> pickedInfos)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF批量出库扫码提交";
            logModel.LogTextLong = "SubmitOutboundTaskList2:" + Newtonsoft.Json.JsonConvert.SerializeObject(pickedInfos);
            logModel.CommonName = "/api/wms/RFOperate/SubmitOutboundTaskList2";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                Dictionary<string, object> erroDict = new Dictionary<string, object>();
                RFOperation rfops = WmsFactory.Make<RFOperation>();
                foreach (SubmitOutboudDetailIn onepick in pickedInfos)
                {

                    foreach (PickedBarcode pb in onepick.lstDetails)
                    {
                        try
                        {
                            SubmitOutboudDetailIn newPick = rfops.AllocateBarcodeToOutboundDetailId(onepick.OutboundOrderDetailId, pb.Barcode);
                            if (newPick != null)
                            {
                                DBHelper.BeginTransaction();
                                var wmList = WmsFactory.Make<RFOperation>().UpOutTaskInfoNew(newPick);
                                DBHelper.EndTransaction();
                            }
                            else
                            {
                                throw new Exception(pb.Barcode + "没有分配到待扫描任务");
                            }
                        }
                        catch (CommitWhenException ex)
                        {
                            if (erroDict.ContainsKey(onepick.OutboundOrderDetailId))
                            {
                                erroDict[onepick.OutboundOrderDetailId] = erroDict[onepick.OutboundOrderDetailId] + ";" + ex.Message;
                            }
                            else
                            {
                                erroDict.Add(onepick.OutboundOrderDetailId, ex.Message);
                            }
                            DBHelper.EndTransaction();
                        }
                        catch (Exception ex)
                        {
                            if (erroDict.ContainsKey(onepick.OutboundOrderDetailId))
                            {
                                erroDict[onepick.OutboundOrderDetailId] = erroDict[onepick.OutboundOrderDetailId] + ";" + ex.Message;
                            }
                            else
                            {
                                erroDict.Add(onepick.OutboundOrderDetailId, ex.Message);
                            }
                            DBHelper.EndTransaction(false);
                        }
                    }
                }
                info.isSucceed = true;
                info.json = erroDict;
                logModel.LogText = "醇化库RF批量出库扫码提交:成功";
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
                logModel.LogText = "醇化库RF批量出库扫码提交:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }


        /// <summary>
        /// 获取已出库的条码信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public WebJsonInfo GetShippedbarcode(string barcode)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                DBHelper.BeginTransaction();

                BarcodeInfo barcodeinfo = WmsFactory.Make<RFOperation>().GetShippedBarcode(barcode);
                if (barcodeinfo == null)
                {
                    throw new Exception("没获取到此条码出库信息,请扫描已出库的条码");
                }
                info.json = barcodeinfo;
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
        /// 执行出库合包
        /// </summary>
        /// <param name="mergeBarcode"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitMergedCarcode(MergebarcodeIn mergeBarcode)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF出库合包";
            logModel.LogTextLong = "醇化库RF出库合包:" + Newtonsoft.Json.JsonConvert.SerializeObject(mergeBarcode);
            logModel.CommonName = "/api/wms/RFOperate/SubmitMergedCarcode";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();

                bool re = WmsFactory.Make<RFOperation>().CombineShippedBarcode(mergeBarcode);
                if (re == null)
                {
                    throw new Exception("保存合包记录出错");
                }
                info.isSucceed = true;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF出库合包:成功";
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
                logModel.LogText = "醇化库RF出库合包:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        } 
        /// <summary>
        /// 按出库单生成烟包条码
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public WebJsonInfo MakeNewBoxNumberByOrder(string orderno)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                DBHelper.BeginTransaction();
                string re = WmsFactory.Make<LocationAllocate>().MakeNewBoxNumberByOrder(orderno);
                info.isSucceed = true;
                info.json = new { PrintBatchId = re };
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
        /// 获取在库烟包条码信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public WebJsonInfo GetInvBarCode(string barcode)
        {

            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            barcode = barcode.Trim();
            logModel.TypeCode = "醇化库RF获取条码信息";
            logModel.LogTextLong = "醇化库RF出库合包:{barcode:" + barcode + "}";
            logModel.CommonName = "/api/wms/RFOperate/GetInvBarCode";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                BarcodeOut re = WmsFactory.Make<RFOperation>().GetInvBarcodeInfo(barcode);
                info.isSucceed = true;
                info.json = re;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取条码信息:成功";
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
                logModel.LogText = "醇化库RF获取条码信息:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 提交拆分信息，并生成对应的拆分条码
        /// </summary>
        /// <param name="splitInfo"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitSplitInfo(BarcodeSplitIn splitInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF拆包";
            logModel.LogTextLong = "醇化库RF出库合包:" + Newtonsoft.Json.JsonConvert.SerializeObject(splitInfo);
            logModel.CommonName = "/api/wms/RFOperate/SubmitSplitInfo";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<RFOperation>().SubmitSplitInfo(splitInfo);
                info.isSucceed = true;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF拆包:成功";
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
                logModel.LogText = "醇化库RF拆包:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Error;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 批量提交拆分
        /// </summary>
        /// <param name="splitInfo"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitSplitInfos(List<BarcodeSplitIn> splitInfos)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF拆包批量提交";
            logModel.LogTextLong = "醇化库RF出库合包:" + Newtonsoft.Json.JsonConvert.SerializeObject(splitInfos);
            logModel.CommonName = "/api/wms/RFOperate/SubmitSplitInfos";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<RFOperation>().SubmitSplitInfos(splitInfos);
                info.isSucceed = true;
                DBHelper.EndTransaction();
                logModel.TypeCode = "醇化库RF拆包批量提交:成功";
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
                logModel.TypeCode = "醇化库RF拆包批量提交:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }

        /// <summary>
        /// 按备料出库计划单维度获取的待出库任务:整料任务
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        WebJsonInfo IRFOperateService.GetTasklistThrowPlanNo(UserInfo userinfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取的待出库任务";
            logModel.LogTextLong = "醇化库RF获取的待出库任务:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetTasklistThrowPlanNo";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                List<Model.HF.OutboundTaskPlanList> lst = WmsFactory.Make<RFOperation>().GetOutboundTaskPlanList(userinfo, false);
                info.isSucceed = true;
                info.json = lst;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取的待出库任务:成功";
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
                logModel.LogText = "醇化库RF获取的待出库任务:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 按备料出库计划单维度获取的待出库任务：零料任务
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        WebJsonInfo IRFOperateService.GetTasklistThrowPlanNo2(UserInfo userinfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取的待出库任务2";
            logModel.LogTextLong = "醇化库RF获取的待出库任务2:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetTasklistThrowPlanNo2";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                List<Model.HF.OutboundTaskPlanList> lst = WmsFactory.Make<RFOperation>().GetOutboundTaskPlanList(userinfo, true);
                info.isSucceed = true;
                info.json = lst;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取的待出库任务2:成功";
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
                logModel.LogText = "醇化库RF获取的待出库任务2:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }

        /// <summary>
        /// 醇化库RF获取待合包任务明细
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        WebJsonInfo IRFOperateService.GetTasklistForCombine(UserInfo userinfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF获取待合包任务明细";
            logModel.LogTextLong = "醇化库RF获取待合包任务明细:" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo);
            logModel.CommonName = "/api/wms/RFOperate/GetTasklistForCombine";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                DBHelper.BeginTransaction();
                List<Model.HF.OutboundTaskPlanList> lst = WmsFactory.Make<RFOperation>().GetTasklistForCombine(userinfo);
                info.isSucceed = true;
                info.json = lst;
                DBHelper.EndTransaction();
                logModel.LogText = "醇化库RF获取待合包任务明细:成功";
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
                logModel.LogText = "醇化库RF获取待合包任务明细:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
        /// <summary>
        /// 批量提交合包信息
        /// </summary>
        /// <param name="lstBarcodes"></param>
        /// <returns></returns>
        public WebJsonInfo SubmitMergedCarcodeNew(List<MergebarcodeNewIn> lstBarcodes)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "醇化库RF批量提交合包信息";
            logModel.LogTextLong = "醇化库RF批量提交合包信息:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstBarcodes);
            logModel.CommonName = "/api/wms/RFOperate/SubmitMergedCarcodeNew";
            logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            try
            {
                List<MergebarcodeOut> lst = new List<MergebarcodeOut>();
                foreach (var mergebars in lstBarcodes)
                {
                    MergebarcodeOut onere = new MergebarcodeOut();
                    onere.MergeBatchId = mergebars.MergeBatchId;
                    try
                    {
                        DBHelper.BeginTransaction();
                        string mergebatchId = WmsFactory.Make<RFOperation>().CombineShippedBarcodeNew(mergebars);
                        DBHelper.EndTransaction();
                        onere.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        DBHelper.EndTransaction(false);
                        onere.IsSuccess = false;
                        onere.msg = ex.Message;
                    }
                    finally
                    {
                        lst.Add(onere);
                    }
                }
                info.isSucceed = true;
                info.errmsg = "";
                info.json = lst; 
                logModel.LogText = "醇化库RF批量提交合包信息:成功";
            }
            catch (Exception ex)
            {
                info.isSucceed = false;
                info.errmsg = ex.Message;
                logModel.LogText = "醇化库RF批量提交合包信息:失败";
                logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            }
            logModel.LogTextLong += ",返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            UserLog.Add(logModel);
            return info;
        }
    }
}

