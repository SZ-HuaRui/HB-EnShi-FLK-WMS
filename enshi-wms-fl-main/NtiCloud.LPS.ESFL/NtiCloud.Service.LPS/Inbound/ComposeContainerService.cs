using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS.Inbound;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.Core;
using NtiCloud.Model.WMS.Inbound;
using NtiCloud.Model.WMS.Model;
using NtiCloud.Service.LPS.Common;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using static NtiCloud.Model.WMS.Inbound.ComposeContainerInput;

namespace NtiCloud.Service.WMS.Inbound
{
    public class ComposeContainerService : IComposeContainerService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly UserMode userMode;
        private readonly IDBContextProvider dbProvider;
        private readonly UserLoginInfo userLoginInfo;
        public ComposeContainerService(IServiceProvider provider,
            Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            userMode = serviceProvider.GetService<IUserService>().GetToken();
            userLoginInfo = userMode.userLoginInfo;
            dbProvider = dbFactory(userLoginInfo.UserDbConnR.dbType);
        }
        public WebJsonInfo Compose(ComposeModel model)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };
            bool isBeginTransaction = false;
            bool lockWasTaken = false;
            if (TokenHelper.IsNullOrEmpty(userMode.token))
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = "Token存在空值，请检查！";
                return errorInfo;
            }
            try
            {
                Monitor.Enter(WmsLock.ComposeLock, ref lockWasTaken);
                if (!HasContainerDetail(model))
                {
                    errorInfo.isSucceed = false;
                    errorInfo.errcode = 10;
                    errorInfo.errmsg = "该托盘无明细！";
                    return errorInfo;
                }
                if (IsExistsBarcode(model,out var bomName))
                {
                    errorInfo.isSucceed = false;
                    errorInfo.errcode = 20;
                    errorInfo.errmsg = string.IsNullOrEmpty(bomName) ?
                        $@"该托盘条码已经扫描！库内存在多个托盘号为{model.ContainerBarcode}的托盘！":
                        $@"该托盘条码已经扫描！bom单号为{bomName}！";
                    return errorInfo;
                }
                if (!HasEnoughInventory(model, out string itemName))
                {
                    errorInfo.isSucceed = false;
                    errorInfo.errcode = 20;
                    errorInfo.errmsg = $@"{itemName}暂存区库存不足！";
                    return errorInfo;
                }
                string fullFlag = CalculateFullFlag(model);
                //开启事务
                dbProvider.BeginTransaction(userLoginInfo);
                isBeginTransaction = true;
                CreateContainerHeader(model, fullFlag, errorInfo, out string containerHeaderId);
                CreateContainerDetail(model, containerHeaderId, errorInfo);
                CreateTask(model, containerHeaderId, errorInfo, out string taskId);
                CreateTaskItem(model, taskId, errorInfo);
                UpdateInventory(model, errorInfo);
                if (isBeginTransaction)
                {
                    bool commit = errorInfo.errcode == 0 ? true : false;
                    dbProvider.EndTransaction(userLoginInfo, commit);
                    isBeginTransaction = false;
                }
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
                    Monitor.Exit(WmsLock.ComposeLock);
                }
                var errMsg = errorInfo.errcode == 0 ? "配盘入库成功！"
                    : $"配盘入库失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "/api/wms/GroupContainer/Compose",
                    Parameter = JsonConvert.SerializeObject(model),
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "配盘入库"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
            }
            return errorInfo;
        }
        public WebJsonInfo GetZone()
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
                string sql = $@"select t_id,zone_name
                                  from &DataSource&wms_m_zone
                                 where client_id ='&CLIENT_ID&'
                                   and warehouse_id ='&WAREHOUSE_ID&'";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql).Tables[0];
                errorInfo.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 200;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            return errorInfo;
        }
        public WebJsonInfo GetItem(string warehouseId)
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
                string sql = $@"select distinct item_id,item_name
                                  from &DataSource&wms_m_bom_h
                                 where client_id ='&CLIENT_ID&'
                                   and warehouse_id ='&WAREHOUSE_ID&'
                                   and bom_status = @bom_status
                              order by item_name";
                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        ["bom_status"] = BOMStatus.BOMStatus_Using.ToString()
                    }
                };
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
                errorInfo.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 200;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            return errorInfo;
        }
        public WebJsonInfo GetBomHeaderByItem(string itemId)
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
                string sql = $@"select h.t_id,h.bom_code,h.bom_name,h.item_id,i.item_code,h.item_name,s.section_code
                                  from &DataSource&wms_m_bom_h h
                                  join &DataSource&wms_m_item i
                                    on h.item_id = i.t_id
                                  join &DataSource&wms_m_section s
                                    on h.prod_section = s.t_id
                                 where h.item_id = @item_id
                                   and h.act_status = '1'";
                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>(){ ["item_id"] = itemId }
                };
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
                errorInfo.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 200;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            return errorInfo;
        }
        public WebJsonInfo GetBomDetailByItem(string bomHeaderId)
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
                string sql = $@" select d.item_id, i.item_code, i.item_name,d.qty,uom.t_id uom_id,uom.uom_name
                                   from &DataSource&wms_m_bom_d d
                                   join &DataSource&wms_m_item i
                                     on d.item_id = i.t_id
                                   join &DataSource&wms_m_item_uom uom
                                     on d.uom_qty_id = uom.t_id
                                  where d.bom_h_id = @bom_h_id";

                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>(){ ["bom_h_id"] = bomHeaderId }
                };
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
                errorInfo.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 200;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            return errorInfo;
        }
        public WebJsonInfo GetItemLot(string itemId)
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
                string sql = $@"select distinct lot_no
                                  from &DataSource&wms_inventory_v
                                 where item_id = @item_id
                                   and qty - out_qty > 0
                                   and location_id in
                                       (select t_id
                                          from &DataSource&wms_m_location
                                         where usage =@location_use)";
                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        ["item_id"] = itemId,
                        ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString()
                    }
                };
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
                errorInfo.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 200;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            return errorInfo;
        }
        public WebJsonInfo GetItemSupplier(string itemId)
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
                string sql = $@"select distinct v.supplier_id,b.full_name,s.rate
                                  from &DataSource&wms_inventory_v v
                                  join &DataSource&wms_m_bpartner b
                                    on v.supplier_id = b.t_id
                                  join &DataSource&wms_m_item_supplier s
                                    on v.item_id = s.item_id
                                   and v.supplier_id = s.bpartner_id
                                 where v.item_id = @item_id
                                   and v.qty - v.out_qty > 0
                                   and v.location_id in
                                       (select t_id
                                          from &DataSource&wms_m_location
                                         where usage =@location_use)";
                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                        ["item_id"] = itemId
                    }
                };
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
                errorInfo.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 200;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
            }
            return errorInfo;
        }
        private bool IsExistsBarcode(ComposeModel model,out string bomName)
        {
            bomName = string.Empty;
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["barcode"] = model.ContainerBarcode
            };

            string sql = $@"select count(1)
                              from &DataSource&wms_b_container_h
                             where client_id ='&CLIENT_ID&'
                               and warehouse_id ='&WAREHOUSE_ID&'
                               and container_barcode =@barcode
                               and act_status = '1'";
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            string count = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            if (count == "0") 
            {
                return false;
            }else if(count == "1")
            {
                sql = $@"select b.bom_name 
                           from &DataSource&wms_b_container_h c 
                      left join &DataSource&wms_m_bom_h b 
                        on c.bom_id = b.t_id
                     where c.client_id ='&CLIENT_ID&'
                       and c.warehouse_id ='&WAREHOUSE_ID&'
                       and c.container_barcode =@barcode
                       and c.act_status = '1'";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                bomName = dbProvider.DBSelectOne(userLoginInfo, sql, param);
                return true;
            }
            else
            {
                return true;
            }
        }
        private bool HasContainerDetail(ComposeModel model)
        {
            return model.ContainerDetailList.Count == 0 ? false : true;
        }
        private bool HasEnoughInventory(ComposeModel model, out string itemName)
        {
            foreach (var item in model.ContainerDetailList)
            {
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    ["item_id"] = item.ItemId,
                    ["lot_no"] = item.Lot,
                    ["supplier_id"] = item.SupplierId,
                    ["qty"] = item.Qty.ToString(),
                    ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                    ["container_type"] = ContainerType.ContainerType_Virtual.ToString()
                };

                string sql = $@"select count(1)
                              from &DataSource&wms_b_container_d d
                              join &DataSource&wms_b_container_h h
                                on d.container_id = h.t_id
                              join &DataSource&wms_m_location l
                                on h.location_id = l.t_id
                             where l.client_id ='&CLIENT_ID&'
                               and l.warehouse_id ='&WAREHOUSE_ID&'
                               and l.usage = @location_use
                               and h.container_type = @container_type
                               and d.item_id = @item_id
                               and d.supplier_id = @supplier_id
                               and d.lot_no = @lot_no
                               and d.qty - ifnull(out_qty,0) >= @qty";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                string count = dbProvider.DBSelectOne(userLoginInfo, sql, param);
                if (count != "1")
                {
                    itemName = item.ItemName;
                    return false;
                }
            }
            itemName = string.Empty;
            return true;
        }
        private string CalculateFullFlag(ComposeModel model)
        {
            string bomHeaderId = model.BomId;
            Dictionary<string, decimal?> dict = new Dictionary<string, decimal?>();

            foreach (var item in model.ContainerDetailList)
            {
                string key = item.ItemId;
                decimal? value = item.Qty;
                dict[key] = value;
            }
            string sql = $@" select d.item_id, d.qty
                               from &DataSource&wms_m_bom_d d
                              where d.bom_h_id = @bom_h_id";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>(){ ["bom_h_id"] = bomHeaderId }
            };

            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string itemId = dt.Rows[i]["item_id"].ToString();
                if (!dict.ContainsKey(itemId))
                {
                    return "0";
                }
                decimal qty = Convert.ToDecimal(dt.Rows[i]["qty"].ToString());
                if (qty != dict[itemId])
                {
                    return "0";
                }
            }
            return "1";
        }
        private void CreateContainerHeader(ComposeModel model, string flag, WebJsonInfo info, out string id)
        {
            string t_id = id = SnowflakeID.Guid().ToString();
            //获取暂存区货位id
            string sql = $@"select t_id
                              from &DataSource&wms_m_location
                             where client_id ='&CLIENT_ID&'
                               and warehouse_id ='&WAREHOUSE_ID&'
                               and usage = @location_use";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                     ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];

            if (dt.Rows.Count != 1)
            {
                info.errcode = 30;
                info.isSucceed = false;
                info.errmsg = "系统不存在或存在多个虚拟暂存区货位，请检查！";
                return;
            }
            string bufferLocationId = dt.Rows[0]["t_id"].ToString();
            sql = $@"insert into &DataSource&wms_b_container_h
                       (t_id,client_id,warehouse_id,location_id,container_barcode,is_full,ALLOW_CALLED,
                        container_type,bom_id,first_in_date,memo,create_date,created_by,act_status)
                     values
                       (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@location_id,@barcode,@flag,'1',
                        @container_type,@bom_id,now(),@zone_id,now(),@created_by,'1')";
            List<Dictionary<string, object>> param1 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = t_id,
                    ["location_id"] = bufferLocationId,
                    ["barcode"] = model.ContainerBarcode,
                    ["flag"] = flag,
                    ["bom_id"] = model.BomId,
                    ["created_by"] = userLoginInfo.UserName,
                    ["container_type"]= ContainerType.ContainerType_Actual.ToString(),
                    ["zone_id"] = model.ZoneId
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
        private void CreateContainerDetail(ComposeModel model, string containerId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            var detail = model.ContainerDetailList;
            List<string> sqlList = new List<string>();
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>();

            foreach (var item in detail)
            {
                string sql = $@"insert into &DataSource&wms_b_container_d 
                                  (t_id,client_id,warehouse_id,container_id,supplier_id,bom_id,item_id,item_name,qc_status,
                                   lot_no,owner_lot_no,exist_barcode,qty,uom_qty_id,ratio,first_in_date,create_date,created_by,act_status)
                                values
                                  (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@container_id,@supplier_id,@bom_id,@item_id,@item_name,@qc_status,
                                   @lot_no,@owner_lot_no,'1',@qty,@uom_id,@rate,now(),now(),@created_by,'1')";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                sqlList.Add(sql);
                paramList.Add(
                    new Dictionary<string, object>()
                    {
                        ["t_id"] = SnowflakeID.Guid().ToString(),
                        ["container_id"] = containerId,
                        ["supplier_id"] = item.SupplierId,
                        ["bom_id"] = model.BomId,
                        ["item_id"] = item.ItemId,
                        ["item_name"] = item.ItemName,
                        ["qc_status"] = QcStatus.QcStatus_CheckPassed.ToString(),
                        ["lot_no"] = item.Lot,
                        ["qty"] = item.Qty.ToString(),
                        ["uom_id"] = item.UomId,
                        ["rate"] = item.Rate.ToString(),
                        ["owner_lot_no"] = item.ItemLable.ToString(),
                        ["created_by"] = userLoginInfo.UserName
                    }
                );
            }
            dbProvider.DBRunSql(userLoginInfo, sqlList, out int rescount, out string resmsg, paramList);
            if (resmsg != string.Empty || rescount != detail.Count)
            {
                info.errcode = 40;
                info.isSucceed = false;
                info.errmsg = "添加容器明细信息失败！";
                return;
            }
        }
        private void CreateTask(ComposeModel model, string containerId, WebJsonInfo info, out string id)
        {
            string t_id = id = SnowflakeID.Guid().ToString();
            if (info.errcode != 0)
            {
                return;
            }
            string taskNo = serviceProvider.GetService<ISequenceService>()
                                           .GetSequenceNo(new SequenceInput.GetSequenceModel() { SeqNameOrId = "TaskNo" })
                                           .json.ToString();
            string sql = $@"insert into &DataSource&wms_b_task_his
                             (t_id,client_id,warehouse_id,task_no,task_type,container_id,container_barcode,
                              task_status,ref_doc_type,start_date,end_date,create_date,created_by,act_status)
                           values
                             (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@task_no,@task_type,@container_id,@barcode,
                              @task_status,@doc_type,now(),now(),now(),@created_by,'1')";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = t_id,
                    ["task_no"] = taskNo,
                    ["task_type"] = TaskType.TaskType_InBound.ToString(),
                    ["task_status"] = TaskStatus.TaskStatus_Finished.ToString(),
                    ["doc_type"] = DocType.DocType_ReceiptCompose.ToString(),
                    ["container_id"] = containerId,
                    ["barcode"] = model.ContainerBarcode,
                    ["created_by"] = userLoginInfo.UserName
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 50;
                info.isSucceed = false;
                info.errmsg = "添加主任务失败！";
                return;
            }
        }
        private void CreateTaskItem(ComposeModel model, string taskId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            var detail = model.ContainerDetailList;
            List<string> sqlList = new List<string>();
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>();

            foreach (var item in detail)
            {
                string sql = $@"insert into &DataSource&wms_b_task_item_his 
                                  (t_id,client_id,warehouse_id,task_id,item_id,item_name,finished_qty,uom_qty_id,
                                   start_date,end_date,create_date,created_by,act_status)
                                values
                                  (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@task_id,@item_id,@item_name,@qty,@uom_id,
                                   now(),now(),now(),@created_by,'1')";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                sqlList.Add(sql);
                paramList.Add(
                    new Dictionary<string, object>()
                    {
                        ["t_id"] = SnowflakeID.Guid().ToString(),
                        ["task_id"] = taskId,
                        ["item_id"] = item.ItemId,
                        ["item_name"] = item.ItemName,
                        ["qty"] = item.Qty.ToString(),
                        ["uom_id"] = item.UomId,
                        ["created_by"] = userLoginInfo.UserName
                    }
                );
            }
            dbProvider.DBRunSql(userLoginInfo, sqlList, out int rescount, out string resmsg, paramList);
            if (resmsg != string.Empty || rescount != detail.Count)
            {
                info.errcode = 60;
                info.isSucceed = false;
                info.errmsg = "添加任务明细信息失败！";
                return;
            }
        }
        private void UpdateInventory(ComposeModel model, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            //查找暂存区虚拟托盘ID
            string sql = $@"select h.t_id
                              from &DataSource&wms_b_container_h h
                              join &DataSource&wms_m_location l
                                on h.location_id = l.t_id
                             where h.client_id ='&CLIENT_ID&'
                               and l.warehouse_id ='&WAREHOUSE_ID&'
                               and l.usage = @location_use
                               and h.container_type = @container_type";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                    ["container_type"] = ContainerType.ContainerType_Virtual.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;

            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
            if (dt.Rows.Count != 1)
            {
                info.errcode = 70;
                info.isSucceed = false;
                info.errmsg = "系统不存在或存在多个虚拟暂存区托盘，请检查！";
                return;
            }
            string bufferContainerId = dt.Rows[0]["t_id"].ToString();

            var detail = model.ContainerDetailList;
            List<string> sqlList = new List<string>();
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>();

            foreach (var item in detail)
            {
                string updateSql = $@"update &DataSource&wms_b_container_d
                                         set qty = qty - @qty
                                       where client_id ='&CLIENT_ID&'
                                         and warehouse_id ='&WAREHOUSE_ID&'
                                         and container_id =@container_id
                                         and item_id = @item_id
                                         and supplier_id = @supplier_id
                                         and lot_no = @lot_no";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, updateSql).sql;
                sqlList.Add(sql);
                paramList.Add(new Dictionary<string, object>()
                {
                    ["container_id"] = bufferContainerId,
                    ["item_id"] = item.ItemId,
                    ["supplier_id"] = item.SupplierId,
                    ["lot_no"] = item.Lot,
                    ["qty"] = item.Qty.ToString()
                });
            }
            dbProvider.DBRunSql(userLoginInfo, sqlList, out int rescount, out string resmsg, paramList);
            if (resmsg != string.Empty || rescount != detail.Count)
            {
                info.errcode = 71;
                info.isSucceed = false;
                info.errmsg = "更新库存信息失败！";
                return;
            }
        }
    }
}
