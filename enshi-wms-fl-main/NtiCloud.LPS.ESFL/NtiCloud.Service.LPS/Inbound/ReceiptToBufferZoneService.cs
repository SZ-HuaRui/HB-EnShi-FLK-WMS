using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS.Inbound;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.Core;
using NtiCloud.Model.WMS.Inbound;
using NtiCloud.Model.WMS.Model;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Inbound
{
    public class ReceiptToBufferZoneService : IReceiptToBufferZoneService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly UserMode userMode;
        private readonly IDBContextProvider dbProvider;
        private readonly UserLoginInfo userLoginInfo;
        public ReceiptToBufferZoneService(IServiceProvider provider,
                                          Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            userMode = serviceProvider.GetService<IUserService>().GetToken();
            userLoginInfo = userMode.userLoginInfo;
            dbProvider = dbFactory(userLoginInfo.UserDbConnR.dbType);
        }
        public WebJsonInfo ToBufferZone(string receiptId)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };
            bool isBeginTransaction = false;
            if (TokenHelper.IsNullOrEmpty(userMode.token))
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = "Token存在空值，请检查！";
                return errorInfo;
            }
            try
            {
                if (!IsEligible(receiptId, errorInfo))
                {
                    return errorInfo;
                }
                dbProvider.BeginTransaction(userLoginInfo);
                isBeginTransaction = true;
                DataTable bufferContainer = GetOrAddBufferContainer(errorInfo);
                RunContainerDetail(receiptId, bufferContainer, errorInfo,out string taskId);
                AddMasterTask(taskId,receiptId,bufferContainer,errorInfo);
                UpdateReceiptStatus(receiptId, errorInfo);
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
                var errMsg = errorInfo.errcode == 0 ? "入库暂存区成功！"
                    : $"入库到暂存区失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "/api/wms/ReceiptToBufferZone/ReceiptToBufferZone",
                    Parameter = receiptId,
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "入库暂存区"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
            }
            return errorInfo;
        }
        private bool IsEligible(string receiptId, WebJsonInfo info)
        {
            info.isSucceed = false;
            List<string> sqlList = new List<string>() 
            {
                $@"select doc_status
                     from &DataSource&wms_d_inbound_h
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and t_id =@receipt_id",
                $@"select count(1) num
                     from &DataSource&wms_d_inbound_d
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and inbound_id = @receipt_id",
                $@"select count(1) num
                     from &DataSource&wms_d_inbound_d
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and inbound_id = @receipt_id
                      and (finished_qty is null or finished_qty <= 0)",
                $@"select max(count(1)) num
                     from &DataSource&wms_d_inbound_d
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and inbound_id = @receipt_id
                    group by item_id",
                $@"select count(1) num
                     from &DataSource&wms_m_location 
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and usage = @location_use",
                $@"select count(1) num
                     from &DataSource&wms_d_inbound_d
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and inbound_id = @receipt_id
                      and (supplier_id is null or lot_no is null)"
            };
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["receipt_id"] = receiptId,
                ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString()
            };
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>() 
            {
                param,param,param,param,param,param
            };
            string sql = string.Join(";", sqlList);
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataSet dataSet = dbProvider.DBSelectAll(userLoginInfo, sql, paramList);
        
            if (dataSet.Tables[0].Rows.Count != 1)
            {
                info.errcode = 10;
                info.errmsg = "收货单号不存在或不唯一！";
            }
            else if(dataSet.Tables[0].Rows[0]["doc_status"].ToString() != InDocStatus.InDocStatus_New.ToString())
            {
                info.errcode = 20;
                info.errmsg = "收货单状态不为未开始！";
            }
            else if (dataSet.Tables[1].Rows[0]["num"].ToString() == "0")
            {
                info.errcode = 30;
                info.errmsg = "该收货单下无明细！";
            }
            else if (dataSet.Tables[2].Rows[0]["num"].ToString() != "0")
            {
                info.errcode = 40;
                info.errmsg = "收货单存在收货数量为0或空的物料！";
            }
            else if (dataSet.Tables[3].Rows[0]["num"].ToString() != "1")
            {
                info.errcode = 50;
                info.errmsg = "收货单存在相同物料！";
            }
            else if (dataSet.Tables[4].Rows[0]["num"].ToString() != "1")
            {
                info.errcode = 60;
                info.errmsg = "当前仓库无暂存区或存在多个暂存区，请维护！";
            }
            else if (dataSet.Tables[5].Rows[0]["num"].ToString() != "0")
            {
                info.errcode = 70;
                info.errmsg = "收货单存在批次号或供应商为空的物料，请维护！";
            }
            else
            {
                info.isSucceed = true;
                return true;
            }
            return false;
        }
        private DataTable GetOrAddBufferContainer(WebJsonInfo info)
        {
            DataTable dt = new DataTable();
            string sql = $@"select count(1)
                              from &DataSource&wms_b_container_h h
                              join &DataSource&wms_m_location l
                                on h.location_id = l.t_id
                             where l.warehouse_id ='&WAREHOUSE_ID&'
                               and l.client_id ='&CLIENT_ID&'
                               and l.usage = @location_use
                               and h.container_type = @container_type";
            Dictionary<string, object> paramOne = new Dictionary<string, object>()
            {
                ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                ["container_type"] = ContainerType.ContainerType_Virtual.ToString()
            };
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                paramOne
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            string count = dbProvider.DBSelectOne(userLoginInfo, sql, paramOne);
            //没有就添加
            if (count == "0")
            {
                AddBufferContainer(info);
                if (info.errcode != 0) return dt; 
            }
            else if (count != "1")
            {
                info.isSucceed = false;
                info.errcode = 80;
                info.errmsg = "当前仓库存在多个暂存区虚拟托盘，请维护！";
                return dt;
            }
            sql = $@"select h.t_id, h.location_id,l.location_no
                       from &DataSource&wms_b_container_h h
                       join &DataSource&wms_m_location l
                         on h.location_id = l.t_id
                      where l.warehouse_id ='&WAREHOUSE_ID&'
                        and l.client_id ='&CLIENT_ID&'
                        and l.usage = @location_use
                        and h.container_type = @container_type";
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dt = dbProvider.DBSelectAll(userLoginInfo, sql,param).Tables[0];
            return dt;
        }
        private void AddBufferContainer(WebJsonInfo info)
        {
            string t_id = SnowflakeID.Guid().ToString();
            string sql = $@"insert into &DataSource&wms_b_container_h
                              (t_id,client_id,warehouse_id,location_id,container_barcode,
                               container_type,created_by,create_date,act_status)
                          values
                              (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',
                              (select t_id from &DataSource&wms_m_location 
                                where client_id ='&CLIENT_ID&' 
                                  and warehouse_id ='&WAREHOUSE_ID&' 
                                  and usage = @location_use),
                              '00000000',@container_type,'System',now(),'1')";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = t_id,
                    ["location_use"] = LocationUse.LocationUse_InboundBuffer.ToString(),
                    ["container_type"] = ContainerType.ContainerType_Virtual.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 90;
                info.isSucceed = false;
                info.errmsg = "添加暂存区虚拟托盘失败！";
            }
        }
        private void RunContainerDetail(string receiptId, DataTable bufferContainer, WebJsonInfo info,out string taskId)
        {
            taskId = SnowflakeID.Guid().ToString();
            if (info.errcode != 0)
            {
                return;
            }
            //获取收货单所有明细
            string sql = $@"select h.doc_no,
                                   d.t_id,
                                   d.item_id,
                                   i.item_name,
                                   i.item_code,
                                   d.lot_no,
                                   d.supplier_id,
                                   d.finished_qty,
                                   d.uom_qty_id
                              from &DataSource&wms_d_inbound_d d
                              join &DataSource&wms_d_inbound_h h
                                on d.inbound_id = h.t_id
                              left join &DataSource&wms_m_item i
                                on d.item_id = i.t_id
                             where d.client_id ='&CLIENT_ID&'
                               and d.warehouse_id ='&WAREHOUSE_ID&'
                               and d.inbound_id = @receipt_id";
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>() 
            {
                new Dictionary<string, object>(){ ["receipt_id"] = receiptId }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, paramList).Tables[0];
            var bufferContainerId = bufferContainer.Rows[0]["t_id"].ToString();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sql = $@" select t_id
                            from &DataSource&wms_b_container_d d
                           where client_id ='&CLIENT_ID&'
                             and warehouse_id ='&WAREHOUSE_ID&'
                             and d.container_id = @container_id
                             and d.item_id = @item_id
                             and supplier_id = @supplier_id
                             and lot_no = @lot_no";
                Dictionary<string, object> param = new Dictionary<string, object>() 
                {
                    ["task_id"] = taskId,
                    ["receipt_no"] = dt.Rows[i]["doc_no"].ToString(),
                    ["receipt_d_id"] = dt.Rows[i]["t_id"].ToString(),
                    ["container_id"] = bufferContainerId,
                    ["item_id"] = dt.Rows[i]["item_id"].ToString(),
                    ["item_name"] = dt.Rows[i]["item_name"].ToString(),
                    ["supplier_id"] = dt.Rows[i]["supplier_id"].ToString(),
                    ["lot_no"] = dt.Rows[i]["lot_no"].ToString(),
                    ["uom_qty_id"] = dt.Rows[i]["uom_qty_id"].ToString(),
                    ["finished_qty"] = dt.Rows[i]["finished_qty"].ToString(),
                    ["created_by"] = userLoginInfo.UserName
                };
                List<Dictionary<string, object>> paramList1 = new List<Dictionary<string, object>>() { param };

                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                DataTable dt1 = dbProvider.DBSelectAll(userLoginInfo, sql, paramList1).Tables[0];
                if(dt1.Rows.Count == 0)
                {
                    param["t_id"] = SnowflakeID.Guid().ToString();
                    sql = $@"insert into &DataSource&wms_b_container_d
                               (t_id,client_id,warehouse_id,container_id,item_id,item_name,qty,
                                uom_qty_id,supplier_id,lot_no,created_by,create_date,act_status)
                             values
                               (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@container_id,@item_id,@item_name,@finished_qty,
                                @uom_qty_id,@supplier_id,@lot_no,@created_by,now(),'1')";
                }else if(dt1.Rows.Count == 1)
                {
                    param["t_id"] = dt1.Rows[0]["t_id"].ToString();
                    sql = $@"update &DataSource&wms_b_container_d 
                                set qty = qty + @finished_qty
                              where t_id =@t_id";
                }
                else
                {
                    info.isSucceed = false;
                    info.errcode = 100;
                    info.errmsg = "请检查暂存区库存信息，该信息不唯一！";
                    return;
                }
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                dbProvider.DBRunSql(userLoginInfo,new List<string>(){ sql },out int rescount,out string resmsg,paramList1);
                if (resmsg != string.Empty || rescount != 1)
                {
                    info.isSucceed = false;
                    info.errcode = 110;
                    info.errmsg = "更新暂存区库存失败！";
                    return;
                }
                param["task_item_t_id"] = SnowflakeID.Guid().ToString();
                sql = $@"insert into &DataSource&wms_b_task_item_his
                           (t_id,client_id, warehouse_id,task_id,item_id,item_name,finished_qty,uom_qty_id,
                            container_d_id,ref_doc_d_id,ref_doc_no,start_date,end_date,create_date,created_by,act_status)
                         values
                           (@task_item_t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@task_id,@item_id,@item_name,@finished_qty,@uom_qty_id,
                            @t_id,@receipt_d_id,@receipt_no,now(),now(),now(),@created_by,'1')";
                sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
                dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount1, out string resmsg1, paramList1);
                if (resmsg1 != string.Empty || rescount1 != 1)
                {
                    info.isSucceed = false;
                    info.errcode = 120;
                    info.errmsg = "添加子任务失败！";
                    return;
                }
            }
        }
        private void AddMasterTask(string taskId,string receiptId, DataTable bufferContainer, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"select doc_no from &DataSource&wms_d_inbound_h where t_id = @receipt_id";
            Dictionary<string, object> param = new Dictionary<string, object>() { ["receipt_id"] = receiptId };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;

            var bufferLocationId = bufferContainer.Rows[0]["location_id"].ToString();
            var bufferLocationNo = bufferContainer.Rows[0]["location_no"].ToString();
            var bufferContainerId = bufferContainer.Rows[0]["t_id"].ToString();
            var receiptNo = dbProvider.DBSelectOne(userLoginInfo, sql, param);
            var taskNo = serviceProvider.GetService<ISequenceService>()
                                           .GetSequenceNo(new SequenceInput.GetSequenceModel() { SeqNameOrId = "TaskNo" })
                                           .json.ToString();
            
            sql = $@"insert into &DataSource&wms_b_task_his
                       (t_id,client_id, warehouse_id,task_no,task_type,container_id,ref_doc_id,ref_doc_no,ref_doc_type,
                        dest_addr_id,dest_addr_text,task_status,start_date,end_date,create_date,created_by,act_status)
                     values
                       (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@task_no,@task_type,@container_id,@receipt_id,@receipt_no,@doc_type,
                        @location_id,@location_no,'TaskStatus_Finished',now(),now(),now(),@created_by,'1')";
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = taskId,
                    ["task_no"] = taskNo,
                    ["container_id"] = bufferContainerId,
                    ["task_type"] = TaskType.TaskType_InBound.ToString(),
                    ["doc_type"] = DocType.DocType_ReceiptToBufferZone.ToString(),
                    ["receipt_id"] = receiptId,
                    ["receipt_no"] = receiptNo,
                    ["location_id"] = bufferLocationId,
                    ["location_no"] = bufferLocationNo,
                    ["created_by"] = userLoginInfo.UserName
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, paramList);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 130;
                info.isSucceed = false;
                info.errmsg = $@"添加主任务失败！具体信息为:{resmsg}。";
            }
        }
        private void UpdateReceiptStatus(string receiptId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"update &DataSource&wms_d_inbound_h 
                               set doc_status = @doc_status,
                                   start_date =now(),
                                   finish_date =now() 
                             where t_id = @receipt_id";
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["receipt_id"] = receiptId,
                    ["doc_status"] = InDocStatus.InDocStatus_Finished.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, paramList);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.isSucceed = false;
                info.errcode = 140;
                info.errmsg = $@"更新收货单主表状态失败！具体信息为:{resmsg}。";
            }
        }
    }
}
