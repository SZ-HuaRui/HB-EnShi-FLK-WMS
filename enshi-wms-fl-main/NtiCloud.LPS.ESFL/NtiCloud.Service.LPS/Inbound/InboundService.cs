using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS.Inbound;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.Core;
using NtiCloud.Model.WMS.Inbound;
using NtiCloud.Model.WMS.Model;
using NtiCloud.Service.LPS.Base;
using NtiCloud.Service.LPS.Common;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace NtiCloud.Service.WMS.Inbound
{
    public class InboundService : IInboundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly UserMode userMode;
        private readonly IDBContextProvider dbProvider;
        private readonly UserLoginInfo userLoginInfo;
        public InboundService(IServiceProvider provider,
            Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            userMode = serviceProvider.GetService<IUserService>().GetUserMode();
            userLoginInfo = userMode.userLoginInfo;
            dbProvider = dbFactory(userLoginInfo.UserDbConnR.dbType);
        }
        public WebJsonInfo PYInbound(string inputJson)
        {
            
            WebJsonInfo errorInfo = new WebJsonInfo();
            bool isBeginTransaction = false;
            bool lockWasTaken = false;
            try
            {
                //解析WCS传过来的参数
                if (!TryParseInputJson(inputJson,out var barcode,out var station))
                {
                    errorInfo.isSucceed = false;
                    errorInfo.errcode = 10;
                    errorInfo.errmsg = $@"条码解析错误，具体条码信息：{inputJson}";
                    return errorInfo;
                }
                //验证条码位数
                if (!ValidateBarcodeLen(barcode))
                {
                    errorInfo.isSucceed = false;
                    errorInfo.errcode = 10;
                    errorInfo.errmsg = $@"烟包条码长度有误，请检查！传入条码为{barcode}。";
                    return errorInfo;
                }
                Monitor.Enter(WmsLock.InboundLock, ref lockWasTaken);
                if(barcode != "99999999")
                {
                    //执行中任务是否含有该烟包条码，库内已存储是否含有该烟包条码
                    if (IsExistsBarcode(barcode, out var result))
                    {
                        errorInfo.isSucceed = false;
                        errorInfo.errcode = 20;
                        errorInfo.errmsg = result;
                        return errorInfo;
                    }
                    //3.查询未开始任务列表中有无该烟包条码相关的任务，有直接返回
                    if (HasTask(barcode))
                    {
                        errorInfo = GetTask(barcode);
                        return errorInfo;
                    }
                }
                //4.解析烟包条码，获取itemId,itemCode
                if (!TryParseContainerBarcode(barcode, out var itemCode, out var itemId)) 
                {
                    errorInfo.isSucceed = false;
                    errorInfo.errcode = 30;
                    errorInfo.errmsg = $@"烟包条码解析错误！具体烟包条码为：{barcode}";
                    return errorInfo;
                }
                if (itemCode != "99999999")
                {
                    //检查暂存区库存是否充足
                    if (!HasEnoughInventory(itemId))
                    {
                        errorInfo.isSucceed = false;
                        errorInfo.errcode = 40;
                        errorInfo.errmsg = $@"暂存区库存不足，请检查！：具体物料代码为：{itemCode}";
                        return errorInfo;
                    }
                    if (!CheckItemState(itemId))
                    {
                        errorInfo.isSucceed = false;
                        errorInfo.errcode = 50;
                        errorInfo.errmsg = $@"请检查该物料的质检状态！具体物料代码为：{itemCode}";
                    }
                }
                dbProvider.BeginTransaction(userLoginInfo);
                isBeginTransaction = true;
                var procName = "";
                var param = new List<Tuple<string, string, ParameterDirection>>() 
                {
                    new Tuple<string, string, ParameterDirection>("", "", ParameterDirection.Input),
                    new Tuple<string, string, ParameterDirection>( "O_LOCATION_NO", "", ParameterDirection.Output),
                    new Tuple<string, string, ParameterDirection>( "O_Result", "", ParameterDirection.Output),
                    new Tuple<string, string, ParameterDirection>( "o_msg", "", ParameterDirection.Output)
                };
                var outputs = dbProvider.DBRunProcedure(userLoginInfo,procName, out int resCount, out string resMsg, param);
                var locationId = string.Empty;

                CreateContainerHeader(locationId,barcode,errorInfo, out string headId);
                CreateContainerDetail(itemId, headId, out var detailId, errorInfo);
                UpdateLocationStatus(locationId, errorInfo);
                if (barcode != "99999999")
                {
                    UpdateInventory(itemId, errorInfo);
                }
                CreateTask(itemId,locationId,station,barcode, headId,detailId, errorInfo);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 900;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            finally
            {
                if (isBeginTransaction)
                {
                    bool isCommit = errorInfo.errcode == 0 ? true : false;
                    dbProvider.EndTransaction(userLoginInfo, isCommit);
                }
                if (lockWasTaken)
                {
                    Monitor.Exit(WmsLock.InboundLock);
                }
                var errMsg = errorInfo.errcode == 0 ? "WCS申请入库成功！"
                    : $"WCS申请入库失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "api/wms/Inbound/PYInbound",
                    Parameter = inputJson,
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "WCS申请入库"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
            }
            return errorInfo;
        }
        private bool TryParseInputJson(string json,out string barcode,out string station)
        {
            JObject inputParameters = JObject.Parse(json);
            barcode = station = string.Empty;
            if (inputParameters.Property("Msg_Params") == null) return false;
            if (inputParameters["Msg_Params"][0]["ContainerBarCode"].Equals(JValue.CreateNull())
               || string.IsNullOrEmpty(inputParameters["Msg_Params"][0]["ContainerBarCode"].ToString()))
                return false;
            if (inputParameters["Msg_Params"][0]["StationNo"].Equals(JValue.CreateNull())
               || string.IsNullOrEmpty(inputParameters["Msg_Params"][0]["StationNo"].ToString()))
                return false;
            station = inputParameters["Msg_Params"][0]["StationNo"].ToString();
            barcode = inputParameters["Msg_Params"][0]["ContainerBarCode"].ToString();
            return true;
        }
        private bool TryParseContainerBarcode(string barcode,out string itemCode,out string itemId)
        {
            itemCode = itemId = string.Empty;
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["client_id"] = WmsPreferences.PYClientId,
                ["warehouse_id"] = WmsPreferences.PYWarehouseId,
            };
            //空托盘组
            if (barcode == "99999999")
            {
                itemCode = barcode;
            }
            //烟包
            else
            {
                string sql = $@"select d.start_position,d.end_position
                                  from wms_m_code_rule_h h
                                  join wms_m_code_rule_d d
                                    on h.t_id = d.rule_h_id
                                 where h.warehouse_id =@warehouse_id
                                   and h.client_id =@client_id
                                   and h.rule_code = 'Container_Barcode'
                                   and h.rule_type = 'BarcodeDecode'
                                   and d.ruledetailcode = 'RuleDetailCode_ItemCode'";
                List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>() { param };
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, paramList).Tables[0];
                if (dt.Columns.Count != 2 || dt.Rows.Count != 1) return false;
                int startPos = Convert.ToInt32(dt.Rows[0]["start_position"].ToString());
                int endPos = Convert.ToInt32(dt.Rows[0]["end_position"].ToString());
                itemCode = barcode.Substring(startPos, (endPos - startPos + 1));
            }
            //查询itemId
            string sql2 = $@"select t_id 
                               from wms_m_item 
                              where client_id =@client_id
                                and warehouse_id =@warehouse_id
                                and item_code =@item_code";
            param.Add("item_code",itemCode);
            itemId = dbProvider.DBSelectOne(userLoginInfo, sql2, param);
            if (string.IsNullOrEmpty(itemId)) return false;
            return true;
        }
        public bool IsExistsBarcode(string barcode,out string result)
        {
            result = string.Empty;
            //检查执行中的任务是否含有该烟包条码
            //检查库内是否含有该烟包条码
            List<string> sqlList = new List<string>()
            {
                $@"select count(1) num
                     from wms_b_task
                    where client_id = @client_id
                      and warehouse_id = @warehouse_id
                      and task_status = @task_status
                      and container_barcode =@container_barcode",
                $@"select count(1) num
                     from wms_b_container_h h
                     join wms_m_location l
                       on h.location_id = l.t_id
                    where h.client_id = @client_id
                      and h.warehouse_id = @warehouse_id
                      and h.container_barcode = @container_barcode
                      and h.container_type = @container_type
                      and l.location_status = @location_status
                      and h.act_status = '1'"
            };
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["client_id"] = WmsPreferences.PYClientId,
                ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                ["task_status"] = TaskStatus.TaskStatus_Executing.ToString(),
                ["container_barcode"] = barcode,
                ["container_type"] = ContainerType.ContainerType_Actual.ToString(),
                ["location_status"] = LocationStatus.LocationStatus_Stored
            };
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>()
            {
                param,param
            };
            string sql = string.Join(";", sqlList);
            DataSet dataSet = dbProvider.DBSelectAll(userLoginInfo, sql, paramList);

            if (dataSet.Tables[0].Rows[0]["num"].ToString() != "0")
            {
                result = $@"存在烟包条码为{barcode}的执行中任务，请检查！";
                return true;
            }
            if (dataSet.Tables[1].Rows[0]["num"].ToString() != "0")
            {
                result = $@"库内存在该烟包条码：{barcode}，请检查库存信息！";
                return true;
            }
            return false;
        }
        private bool HasTask(string barcode)
        {
            string sql = $@"select count(1) num
                             from wms_b_task
                            where client_id = @client_id
                              and warehouse_id = @warehouse_id
                              and task_type = @task_type
                              and task_status = @task_status
                              and container_barcode = @container_barcode";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["client_id"] = WmsPreferences.PYClientId,
                ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                ["task_type"] = TaskType.TaskType_InBound.ToString(),
                ["task_status"] = TaskStatus.TaskStatus_NotExecute.ToString(),
                ["container_barcode"] = barcode
            };
            var count  = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            return count.ToString() == "0" ? false : true;
        }
        private bool HasEnoughInventory(string itemId)
        {
            string sql = $@"select count(1) num
                              from wms_b_container_d
                             where container_id =
                                   (select t_id
                                      from wms_b_container_h
                                     where location_id = (select l.t_id
                                                            from wms_m_location l l
                                                           where l.warehouse_id = @warehouse_id
                                                             and l.client_id = @client_id
                                                             and l.usage = @location_use)
                                       and container_type = @container_type)
                               and item_id = @item_id
                               and qty >= 1";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["client_id"] = WmsPreferences.PYClientId,
                ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                ["container_type"] = ContainerType.ContainerType_Virtual.ToString(),
                ["item_id"] = itemId
            };
            string count = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            return count == "1" ? true : false;
        }
        private WebJsonInfo GetTask(string barcode)
        {
            WebJsonInfo info = new WebJsonInfo();
            string sql = $@"select t.warehouse_id,
                                   t.task_no,
                                   t.task_type,
                                   t.task_status,
                                   t.container_barcode,
                                   t.src_addr_text,
                                   t.dest_addr_text,
                                   t.priority,
                                   item.item_name,
                                   i.plan_qty,
                                   eq.equipment_code
                             from  wms_b_task t 
                             join  wms_b_task_item i
                               on  t.t_id = i.task_id 
                             join  wms_m_item item
                               on  i.item_id = item.t_id
                        left join  wms_m_equipment eq on t.equipment_id = eq.t_id
                            where  t.client_id = @client_id
                              and  t.warehouse_id =@warehouse_id
                              and  t.task_type = @task_type
                              and  t.task_status = @task_status
                              and  t.container_barcode = @container_barcode";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["client_id"] = WmsPreferences.PYClientId,
                ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                ["task_type"] = TaskType.TaskType_InBound.ToString(),
                ["task_status"] = TaskStatus.TaskStatus_NotExecute.ToString(),
                ["container_barcode"] = barcode
            };
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql,new List<Dictionary<string, object>>() { param }).Tables[0];
            if(dt.Columns.Count > 2 && dt.Rows.Count == 1)
            {
                info.errcode = 0;
                info.errmsg = "";
                info.isSucceed = true;
                info.json = new 
                {
                    WarehouseId = dt.Rows[0]["warehouse_id"].ToString(),
                    ZoneId = string.Empty,
                    TaskNum = dt.Rows[0]["task_no"].ToString(),
                    TaskStatus = "Task_NotExecute",
                    ContainerBarcode = barcode,
                    WaveId = string.Empty,
                    GroupNum = string.Empty,
                    SeqNum = string.Empty,
                    SourceAddress = dt.Rows[0]["src_addr_text"].ToString(),
                    TargetAddress = dt.Rows[0]["dest_addr_text"].ToString(),
                    EquipmentNum = dt.Rows[0]["equipment_code"].ToString(),
                    LedText = string.Empty,
                    WmsOrderNo = string.Empty,
                    WmsOrderQty = dt.Rows[0]["plan_qty"].ToString(),
                    Priority = dt.Rows[0]["priority"].ToString(),
                    RowSeq = string.Empty,
                    ItemName = dt.Rows[0]["item_name"].ToString(),
                    ItemCount = dt.Rows[0]["plan_qty"].ToString(),
                    ItemBarcode = string.Empty
                };
            }
            else
            {
                info.errcode = 40;
                info.errmsg = $@"存在任务失败！具体烟包条码为：{barcode}";
                info.isSucceed = false;
            }
            return info;
        }
        private bool ValidateBarcodeLen(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) return false;
            if (barcode == "99999999") return true;
            return barcode.Length == 50;
        }
        private bool CheckItemState(string itemId)
        {
            string sql = $@"select qc_status from wms_m_item where t_id = @t_id";
            var param = new Dictionary<string, object>()
            {
                ["t_id"] = itemId
            };
            var qcStatus = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            if(qcStatus == QcStatus.QcStatus_CheckPassed.ToString() 
                || qcStatus == QcStatus.QcStatus_CheckAvoid.ToString())
            {
                return true;
            }
            return false;
        }
        private void CreateContainerHeader(string locationId,string barcode, WebJsonInfo info, out string id)
        {
            string t_id = id = SnowflakeID.Guid().ToString();
            string sql = $@"insert into wms_b_container_h
                       (t_id,client_id,warehouse_id,location_id,container_barcode,is_full,
                        container_type,first_in_date,create_date,created_by,act_status)
                     values
                       (@t_id,@client_id,@warehouse_id,@location_id,@barcode,'1',
                        @container_type,now(),now(),'WCS','1')";
            List<Dictionary<string, object>> param1 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = t_id,
                    ["client_id"] = WmsPreferences.PYClientId,
                    ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                    ["location_id"] = locationId,
                    ["barcode"] = barcode,
                    ["container_type"]= ContainerType.ContainerType_Actual.ToString(),
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param1);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 31;
                info.isSucceed = false;
                info.errmsg = "添加主容器信息失败！";
                return;
            }
        }
        private void CreateContainerDetail(string itemId, string containerId,out string contaierDetailId, WebJsonInfo info)
        {
            contaierDetailId = string.Empty;
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"select item_name,rate,qc_status,
                                   default_pack_uom_id,default_stat_uom_id
                              from wms_m_item
                             where client_id =@client_id
                               and warehouse_id =@warehouse_id
                               and t_id = @itemId";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["client_id"] = WmsPreferences.PYClientId,
                    ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                    ["itemId"]=itemId
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
            if (dt.Rows.Count != 1)
            {
                info.errcode = 110;
                info.isSucceed = false;
                info.errmsg = "请检查该物料信息！";
                return;
            }
            string itemName = dt.Rows[0]["item_name"].ToString();
            string defaultPackUomId = dt.Rows[0]["default_pack_uom_id"].ToString();
            string defaultStatUomId = dt.Rows[0]["default_stat_uom_id"].ToString();
            string qcStatus = dt.Rows[0]["qc_status"].ToString();
            decimal rate = string.IsNullOrEmpty(dt.Rows[0]["rate"]?.ToString()) 
                ? 0
                : Convert.ToDecimal(dt.Rows[0]["rate"]?.ToString());
            sql = $@"insert into wms_b_container_d 
                                (t_id,client_id,warehouse_id,container_id,item_id,item_name,
                                 exist_barcode,qty,stat_qty,uom_qty_id,uom_stat_id,ratio,qc_status,
                                 first_in_date,create_date,created_by,act_status)
                            values
                                (@t_id,@client_id,@warehouse_id,@container_id,@item_id,@item_name,
                                 '1',@qty,@stat_qty,@uom_qty_id,@uom_stat_id,@qc_status,
                                 now(),now(),'WCS','1')";
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            List<Dictionary<string, object>> param1 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = contaierDetailId = SnowflakeID.Guid().ToString(),
                    ["client_id"] = WmsPreferences.PYClientId,
                    ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                    ["container_id"] = containerId,
                    ["item_id"] = itemId,
                    ["item_name"] = itemName,
                    ["qty"] = 1,
                    ["stat_qty"] = 1 * rate,
                    ["uom_qty_id"] = defaultPackUomId,
                    ["uom_stat_id"] = defaultStatUomId,
                    ["qc_status"]= qcStatus
                }
            };
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param1);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 210;
                info.isSucceed = false;
                info.errmsg = "添加容器明细信息失败！";
                return;
            }
        }
        private void UpdateLocationStatus(string locationId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"update wms_m_location set location_status = @location_status where t_id = @location_id";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["location_id"] = locationId,
                    ["location_status"] = LocationStatus.LocationStatus_In.ToString()
                }
            };
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 210;
                info.isSucceed = false;
                info.errmsg = "更新货位状态为待入库失败！";
                return;
            }
        }
        private void UpdateInventory(string itemId,WebJsonInfo info)
        {
            string sql = $@"select t_id
                              from wms_b_container_d
                             where container_id =
                                   (select t_id
                                      from wms_b_container_h
                                     where location_id = (select l.t_id
                                                            from wms_m_location l l
                                                           where l.warehouse_id = @warehouse_id
                                                             and l.client_id = @client_id
                                                             and l.usage = @location_use)
                                       and container_type = @container_type)
                               and item_id = @item_id
                               and qty >= 1";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["client_id"] = WmsPreferences.PYClientId,
                ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                ["container_type"] = ContainerType.ContainerType_Virtual.ToString(),
                ["item_id"] = itemId
            };
            string tId = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            sql = $@"update wms_b_container_d set qty = qty - 1 where t_id =@t_id";
            List<Dictionary<string, object>> param1 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = tId
                }
            };
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param1);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 210;
                info.isSucceed = false;
                info.errmsg = "更新暂存区库存失败！";
            }
        }
        private void CreateTask(string itemId,string locationId,string station,string barcode,string headId,string detailId,WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string taskNo = serviceProvider.GetService<ISequenceService>()
                            .GetSequenceNo(new SequenceInput.GetSequenceModel() 
                                { SeqNameOrId = "TaskNo" })
                            .json.ToString();
            string taskId = SnowflakeID.Guid().ToString();
            string sql = "select location_no from wms_m_location where t_id = @location_id";
            var locationNo = dbProvider.DBSelectOne(userLoginInfo, sql, 
                new Dictionary<string, object>() { ["location_id"] = locationId });
            //获取优先级
            var sql2 = $@"select config_value
                            from wms_s_config
                           where config_key = 'Inbound'
                             and p_id = (select t_id
                                             from wms_s_config
                                            where client_id =@client_id
                                              and warehouse_id =@warehouse_id 
                                              and config_key = 'Priority')";
            string priority = dbProvider.DBSelectOne(userLoginInfo, sql2,
                new Dictionary<string, object>() {
                    ["client_id"] = WmsPreferences.PYClientId,
                    ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                });
            //获取设备ID
            priority = string.IsNullOrEmpty(priority) ? "100" : priority;

            sql = $@"insert into wms_b_task
                       (t_id,client_id,warehouse_id,task_no,task_type,container_id,container_barcode,priority,
                        src_addr_text,dest_addr_id,dest_addr_text,ref_doc_type,task_status,start_date,create_date,created_by,act_status)
                     values
                       (@t_id,@client_id,@warehouse_id,@task_no,@task_type,@container_id,@barcode,@priority
                        @src_addr_text,@dest_addr_id,@dest_addr_text,@doc_type,@task_status,now(),now(),'WCS','1')";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = taskId,
                    ["client_id"] =WmsPreferences.PYClientId,
                    ["warehouse_id"] =WmsPreferences.PYWarehouseId,
                    ["task_no"] = taskNo,
                    ["task_type"] = TaskType.TaskType_InBound.ToString(),
                    ["container_id"] = headId,
                    ["barcode"] = barcode,
                    ["priority"] = priority,
                    ["src_addr_text"] = station,
                    ["dest_addr_id"] = locationId,
                    ["dest_addr_text"] = locationNo,
                    ["doc_type"] = DocType.DocType_Inbound.ToString(),
                    ["task_status"] = TaskStatus.TaskStatus_NotExecute.ToString(),
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 310;
                info.isSucceed = false;
                info.errmsg = "添加主任务失败！";
                return;
            }
            //添加明细任务
            sql = $@"select item_name,default_pack_uom_id
                              from wms_m_item
                             where client_id =@client_id
                               and warehouse_id =@warehouse_id
                               and t_id = @itemId";
            List<Dictionary<string, object>> param1 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["client_id"] = WmsPreferences.PYClientId,
                    ["warehouse_id"] = WmsPreferences.PYWarehouseId,
                    ["itemId"]=itemId
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param1).Tables[0];

            sql = $@"insert into wms_b_task_item 
                       (t_id,client_id,warehouse_id,task_id,item_id,item_name,plan_qty,
                        container_d_id,uom_qty_id,process_status,start_date,create_date,created_by,act_status)
                     values
                       (@t_id,@client_id,@warehouse_id,@task_id,@item_id,@item_name,@qty,
                        @container_d_id,@uom_qty_id,@process_status,now(),now(),'WCS','1')";
            List<Dictionary<string, object>> param2 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = SnowflakeID.Guid().ToString(),
                    ["client_id"] =WmsPreferences.PYClientId,
                    ["warehouse_id"] =WmsPreferences.PYWarehouseId,
                    ["task_id"] = taskId,
                    ["item_id"] = itemId,
                    ["item_name"] = dt.Rows[0]["item_name"].ToString(),
                    ["qty"] = "1",
                    ["container_d_id"] = detailId,
                    ["uom_qty_id"] = dt.Rows[0]["default_pack_uom_id"].ToString(),
                    ["process_status"] = ProcessStatus.ProcessStatus_New.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount2, out string resmsg2, param2);
            if (resmsg2 != string.Empty || rescount2 != 1)
            {
                info.errcode = 310;
                info.isSucceed = false;
                info.errmsg = "添加明细任务失败！";
                return;
            }
            info.errcode = 0;
            info.errmsg = "";
            info.isSucceed = true;
            info.json = new
            {
                WarehouseId = WmsPreferences.PYWarehouseId,
                ZoneId = string.Empty,
                TaskNum = taskNo,
                TaskStatus = "Task_NotExecute",
                ContainerBarcode = barcode,
                WaveId = string.Empty,
                GroupNum = string.Empty,
                SeqNum = string.Empty,
                SourceAddress = station,
                TargetAddress = locationNo,
                EquipmentNum = "SC"+ Math.Ceiling(Convert.ToDecimal(locationNo.Substring(2,1))/2).ToString(),
                LedText = string.Empty,
                WmsOrderNo = string.Empty,
                WmsOrderQty = "1",
                Priority = priority,
                RowSeq = string.Empty,
                ItemName = dt.Rows[0]["item_name"].ToString(),
                ItemCount ="1",
                ItemBarcode = string.Empty
            };
        }
    }
}
