using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS.Inbound;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS.Model;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Inbound
{
    public class ReceiptPlanService : IReceiptPlanService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly UserMode userMode;
        private readonly IDBContextProvider dbProvider;
        private readonly UserLoginInfo userLoginInfo;
        public ReceiptPlanService(IServiceProvider provider,Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            userMode = serviceProvider.GetService<IUserService>().GetToken();
            userLoginInfo = userMode.userLoginInfo;
            dbProvider = dbFactory(userLoginInfo.UserDbConnR.dbType);
        }
        public WebJsonInfo Excute(string planId)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };
            if (TokenHelper.IsNullOrEmpty(userMode.token))
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = "Token存在空值，请检查！";
                return errorInfo;
            }
            try
            {
                if (HasExcutingPlan(planId))
                {
                    errorInfo.errcode = 100;
                    errorInfo.isSucceed = false;
                    errorInfo.errmsg = "该机台存在执行中计划，请检查！";
                    return errorInfo;
                }
                UpdateStatusToExcuting(planId, errorInfo);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 900;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            finally
            {
                var errMsg = errorInfo.errcode == 0 ? "修改计划状态执行中成功！"
                        : $"修改计划状态执行中失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "/api/wms/ReceiptPlan/Excute",
                    Parameter = planId,
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "更新计划状态"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
            }
            return errorInfo;
        }
        public WebJsonInfo Pause(string planId)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };
            if (TokenHelper.IsNullOrEmpty(userMode.token))
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = "Token存在空值，请检查！";
                return errorInfo;
            }
            try
            {
                string planStatus = GetPlanStatus(planId);
                if (planStatus != "PlanStatus_Executing")
                {
                    errorInfo.errcode = 100;
                    errorInfo.isSucceed = false;
                    errorInfo.errmsg = "计划状态为执行中，才可暂停！";
                }
                UpdateStatusToPausing(planId, errorInfo);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 900;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            finally
            {
                var errMsg = errorInfo.errcode == 0 ? "修改计划状态暂停中成功！"
                        : $"修改计划状态暂停中失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "/api/wms/ReceiptPlan/Pause",
                    Parameter = planId,
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "更新计划状态"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
            }
            return errorInfo;
        }
        public WebJsonInfo Finish(string planId)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };
            if (TokenHelper.IsNullOrEmpty(userMode.token))
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = "Token存在空值，请检查！";
                return errorInfo;
            }
            try
            {
                string planStatus = GetPlanStatus(planId);
                if (planStatus == "PlanStatus_NotExecute")
                {
                    errorInfo.errcode = 100;
                    errorInfo.isSucceed = false;
                    errorInfo.errmsg = "未开始计划不可直接完成！";
                }
                UpdateStatusToFinished(planId, errorInfo);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 900;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            finally
            {
                
                var errMsg = errorInfo.errcode == 0 ? "修改计划状态完成成功！"
                        : $"修改计划状态完成失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "/api/wms/ReceiptPlan/Finish",
                    Parameter = planId,
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "更新计划状态"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
                
            }
            return errorInfo;
        }
        private bool HasExcutingPlan(string planId)
        {
            
            string sql = $@"select count(1)
                              from &DataSource&wms_d_prod_plan_in
                             where client_id ='&CLIENT_ID&'
                               and warehouse_id ='&WAREHOUSE_ID&'
                               and plan_status = @status_executing
                               and equipment_id = (select equipment_id
                                                     from &DataSource&wms_d_prod_plan_in
                                                    where t_id = @t_id)";
            var param = new Dictionary<string, object>()
            {
                ["t_id"] = planId,
                ["status_executing"] = PlanStatus.PlanStatus_Executing.ToString()
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            string count = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            return count == "0" ? false : true;
        }
        private string GetPlanStatus(string planId)
        {
            Dictionary<string, object> param = new Dictionary<string, object>() { ["t_id"] = planId };
            string sql = $@"select plan_status
                              from &DataSource&wms_d_prod_plan_in
                             where t_id = @t_id";
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            return dbProvider.DBSelectOne(userLoginInfo, sql, param);
        }
        private void UpdateStatusToExcuting(string planId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"update &DataSource&wms_d_prod_plan_in set plan_status = @status_executing where t_id = @t_id";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                { 
                    ["t_id"] = planId, 
                    ["status_executing"] = PlanStatus.PlanStatus_Executing.ToString() 
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 110;
                info.isSucceed = false;
                info.errmsg = $@"修改计划状态为执行中失败，具体错误信息:{resmsg}。";
            }
        }
        private void UpdateStatusToPausing(string planId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"update &DataSource&wms_d_prod_plan_in set plan_status = @status_pausing where t_id = @t_id";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                { 
                    ["t_id"] = planId ,
                    ["status_pausing"] = PlanStatus.PlanStatus_Pausing.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 110;
                info.isSucceed = false;
                info.errmsg = $@"修改计划状态为暂停中失败，具体错误信息:{resmsg}。";
            }
        }
        private void UpdateStatusToFinished(string planId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"update &DataSource&wms_d_prod_plan_in set plan_status =@status_finished,end_time =now() where t_id = @t_id";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                { 
                    ["t_id"] = planId,
                    ["status_finished"] = PlanStatus.PlanStatus_Finished.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 110;
                info.isSucceed = false;
                info.errmsg = $@"修改计划状态为已完成失败，具体错误信息:{resmsg}。";
            }
        }
    }
}
