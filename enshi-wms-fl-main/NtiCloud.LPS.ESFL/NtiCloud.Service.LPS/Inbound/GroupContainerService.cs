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
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using static NtiCloud.Model.WMS.Inbound.GroupContainerInput;

namespace NtiCloud.Service.WMS.Inbound
{
    public class GroupContainerService : IGroupContainerService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly UserMode userMode;
        private readonly IDBContextProvider dbProvider;
        private readonly UserLoginInfo userLoginInfo;
        public GroupContainerService(IServiceProvider provider,
            Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            userMode = serviceProvider.GetService<IUserService>().GetToken();
            userLoginInfo = userMode.userLoginInfo;
            dbProvider = dbFactory(userLoginInfo.UserDbConnR.dbType);
            Context.Init(serviceProvider, dbFactory);
        }

        public WebJsonInfo Group(GroupModel model)
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
                Monitor.Enter(WmsLock.GroupLock, ref lockWasTaken);
                if (!IsEligible(model, errorInfo))
                {
                    return errorInfo;
                }
                dbProvider.BeginTransaction(userLoginInfo);
                isBeginTransaction = true;
                CreateContainerHeader(model, errorInfo, out string containerHeaderId);
                CreateContainerDetail(model, containerHeaderId, errorInfo);
                CreateTask(model, containerHeaderId, errorInfo, out string taskId);
                CreateTaskItem(model, taskId, errorInfo);
                UpdateReceiptStatus(model, errorInfo);
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
                    Monitor.Exit(WmsLock.GroupLock);
                }
                var errMsg = errorInfo.errcode == 0 ? "组盘入库成功！"
                    : $"组盘入库失败，失败原因：{errorInfo.errmsg}";
                Log log = new Log()
                {
                    Message = errMsg,
                    Action = "/api/wms/GroupContainer/Group",
                    Parameter = JsonConvert.SerializeObject(model),
                    Result = JsonConvert.SerializeObject(errorInfo),
                    Type = "组盘入库"
                };
                WmsLog.Write(serviceProvider, dbProvider, userMode, log);
            }
            return errorInfo;
        }
        public WebJsonInfo GetReceiptNo(string warehouseId)
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
                string sql = $@"select t_id,doc_no
                                  from &DataSource&wms_d_inbound_h
                                 where client_id ='&CLIENT_ID&'
                                   and warehouse_id ='&WAREHOUSE_ID&' 
                                   and (doc_status = @status_new or 
                                        doc_status = @status_executing)";
                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        ["status_new"] = InDocStatus.InDocStatus_New.ToString(),
                        ["status_executing"] = InDocStatus.InDocStatus_Executing.ToString()
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
        public WebJsonInfo GetItemsByReceiptId(string receiptId)
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
                string sql = $@"select d.item_id, i.item_code,i.item_name,i.spec,
                                       d.lot_no,b.full_name,b.t_id supplier_id,
                                       i.max_container_qty,d.finished_qty,
                                       d.processed_qty,u.uom_name,u.t_id uom_id
                                  from &DataSource&wms_d_inbound_d d
                                  join &DataSource&wms_m_item i
                                    on d.item_id = i.t_id
                                  join &DataSource&wms_m_bpartner b
                                    on d.supplier_id = b.t_id
                                  join &DataSource&wms_m_item_uom u 
                                    on d.uom_qty_id = u.t_id 
                                 where d.inbound_id = @receipt_id
                                   and d.finished_qty - d.processed_qty > 0";

                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>(){ ["receipt_id"] = receiptId}
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
        public WebJsonInfo GetItemByItemId(string receiptId, string itemId)
        {
            return GetReceiptDetailByItemLabel(receiptId, itemId, (itemId) => { return itemId; });
        }
        public WebJsonInfo GetReceiptDetailByItemLabel(string receiptId, string label, Func<string, string> func)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };
            if (TokenHelper.IsNullOrEmpty(userMode.token))
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = "Token存在空值，请检查！";
                return errorInfo;
            }
            string itemId = func(label);
            try
            {
                string sql = $@"select d.item_id,i.item_code,i.item_name,i.spec,i.default_stat_uom_id as sta_uom_id,i.rate,
                                       d.lot_no,b.full_name,b.t_id supplier_id,
                                       i.max_container_qty,d.processed_qty,
                                       u.uom_name,u.t_id uom_id,d.finished_qty,d.t_id
                                  from &DataSource&wms_d_inbound_d d
                                  join &DataSource&wms_m_item i
                                    on d.item_id = i.t_id
                                  join &DataSource&wms_m_bpartner b
                                    on d.supplier_id = b.t_id
                                  join &DataSource&wms_m_item_uom u 
                                    on d.uom_qty_id = u.t_id 
                                 where d.inbound_id = @receipt_id
                                   and d.item_id = @item_id";
                List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        ["receipt_id"] = receiptId,
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
        private bool IsEligible(GroupModel model, WebJsonInfo info)
        {
            //检查托盘号是否重复
            //检查收货单状态
            //检查收货单上是否存在该货品
            //检查收货数量
            info.isSucceed = false;
            List<string> sqlList = new List<string>()
            {
                $@"select doc_status
                     from &DataSource&wms_d_inbound_h
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and t_id =@receipt_id",
                $@"select count(1) num
                     from &DataSource&wms_b_container_h
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and container_barcode =@barcode
                      and act_status = '1'",
                $@"select count(1) num
                     from &DataSource&wms_d_inbound_d
                    where client_id ='&CLIENT_ID&'
                      and warehouse_id ='&WAREHOUSE_ID&'
                      and inbound_id =@receipt_id
                      and item_id = @item_id
                      and ifnull(finished_qty,0) - ifnull(processed_qty,0) >= @qty"
            };
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["barcode"] = model.ContainerBarcode,
                ["receipt_id"] = model.ReceiptId,
                ["item_id"] = model.ItemId,
                ["qty"] = model.Qty.ToString()
            };
            List<Dictionary<string, object>> paramList = new List<Dictionary<string, object>>()
            {
                param,param,param
            };
            string sql = string.Join(";", sqlList);
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataSet dataSet = dbProvider.DBSelectAll(userLoginInfo, sql, paramList);
            if (dataSet.Tables[0].Rows.Count != 1)
            {
                info.errcode = 10;
                info.errmsg = "收货单号不存在或不唯一！";
            }
            else if (dataSet.Tables[0].Rows[0]["doc_status"].ToString() != InDocStatus.InDocStatus_New.ToString()
                  && dataSet.Tables[0].Rows[0]["doc_status"].ToString() != InDocStatus.InDocStatus_Executing.ToString())
            {
                info.errcode = 20;
                info.errmsg = "请检查收货单状态！";
            }
            else if (dataSet.Tables[1].Rows[0]["num"].ToString() != "0")
            {
                info.errcode = 30;
                info.errmsg = "该托盘已经扫描过！";
            }
            else if (dataSet.Tables[2].Rows[0]["num"].ToString() != "1")
            {
                info.errcode = 40;
                info.errmsg = "请检查收货单下该物料数量，防止超收！";
            }
            else
            {
                info.isSucceed = true;
                return true;
            }
            return false;
        }
        private void CreateContainerHeader(GroupModel model, WebJsonInfo info, out string id)
        {
            string t_id = id = SnowflakeID.Guid().ToString();
            //获取暂存区货位id
            string flag = model.Qty == model.ContainerCapacity ? "1" : "0";
            string sql = $@"select t_id
                              from &DataSource&wms_m_location
                             where client_id ='&CLIENT_ID&'
                               and warehouse_id ='&WAREHOUSE_ID&'
                               and usage = @location_use";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["location_use"]=LocationUse.LocationUse_InboundBuffer.ToString()
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            DataTable dt = dbProvider.DBSelectAll(userLoginInfo, sql, param).Tables[0];
            if (dt.Rows.Count != 1)
            {
                info.errcode = 110;
                info.isSucceed = false;
                info.errmsg = "系统不存在或存在多个虚拟暂存区货位，请检查！";
                return;
            }
            string bufferLocationId = dt.Rows[0]["t_id"].ToString();
            sql = $@"insert into &DataSource&wms_b_container_h
                       (t_id,client_id,warehouse_id,location_id,container_barcode,is_full,
                        container_type,first_in_date,create_date,created_by,act_status,is_in)
                     values
                       (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@location_id,@barcode,@flag,
                        @container_type,now(),now(),@created_by,'1',0)";
            List<Dictionary<string, object>> param1 = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = t_id,
                    ["location_id"] = bufferLocationId,
                    ["barcode"] = model.ContainerBarcode,
                    ["flag"] = flag,
                    ["container_type"] = ContainerType.ContainerType_Actual.ToString(),
                    ["created_by"] = userLoginInfo.UserName
                }
            };
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param1);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 120;
                info.isSucceed = false;
                info.errmsg = "添加主容器信息失败！";
                return;
            }
        }
        private void CreateContainerDetail(GroupModel model, string containerId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"insert into &DataSource&wms_b_container_d 
                                (t_id,client_id,warehouse_id,inbound_h_id,container_id,supplier_id,
                                 item_id,item_name,lot_no,exist_barcode,qty,uom_qty_id,qc_status,
                                 spec,first_in_date,create_date,created_by,act_status,uom_stat_id,stat_qty)
                            values
                                (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@inbound_id,@container_id,@supplier_id,
                                 @item_id,@item_name,@lot_no,'1',@qty,@uom_id,@qc_status,
                                (select spec from &DataSource&wms_m_item where t_id =@item_id),now(),now(),@created_by,'1',@uom_stat_id,@stat_qty)";
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;

            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = SnowflakeID.Guid().ToString(),
                    ["inbound_id"] = model.ReceiptId,
                    ["container_id"] = containerId,
                    ["supplier_id"] = model.SupplierId,
                    ["item_id"] = model.ItemId,
                    ["item_name"] = model.ItemName,
                    ["lot_no"] = model.Lot,
                    ["qty"] = model.Qty.ToString(),
                    ["uom_id"] = model.UomId,
                    ["qc_status"]= model.QCStatus,
                    ["created_by"] = userLoginInfo.UserName,
                    ["uom_stat_id"]= model.StaUomId,
                    ["stat_qty"] = (model.Qty*model.Spec*model.Ratio).ToString()
                }
            };

            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 210;
                info.isSucceed = false;
                info.errmsg = "添加容器明细信息失败！";
                return;
            }
        }
        private void CreateTask(GroupModel model, string containerId, WebJsonInfo info, out string id)
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
                              ref_doc_type,task_status,start_date,end_date,create_date,created_by,act_status)
                           values
                             (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@task_no,@task_type,@container_id,@barcode,
                              @doc_type,@task_status,now(),now(),now(),@created_by,'1')";
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = t_id,
                    ["task_no"] = taskNo,
                    ["task_type"] = TaskType.TaskType_InBound.ToString(),
                    ["doc_type"] = DocType.DocType_ReceiptGroup.ToString(),
                    ["task_status"] = TaskStatus.TaskStatus_Finished.ToString(),
                    ["container_id"] = containerId,
                    ["barcode"] = model.ContainerBarcode,
                    ["created_by"] = userLoginInfo.UserName
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
        }
        private void CreateTaskItem(GroupModel model, string taskId, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            string sql = $@"insert into &DataSource&wms_b_task_item_his 
                                (t_id,client_id,warehouse_id,task_id,item_id,item_name,finished_qty,
                                 uom_qty_id,start_date,end_date,create_date,created_by,act_status)
                            values
                                (@t_id,'&CLIENT_ID&','&WAREHOUSE_ID&',@task_id,@item_id,@item_name,@qty,
                                 @uom_id,now(),now(),now(),@created_by,'1')";
            sql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, sql).sql;
            List<Dictionary<string, object>> param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    ["t_id"] = SnowflakeID.Guid().ToString(),
                    ["task_id"] = taskId,
                    ["item_id"] = model.ItemId,
                    ["item_name"] = model.ItemName,
                    ["qty"] = model.Qty.ToString(),
                    ["uom_id"] = model.UomId,
                    ["created_by"] = userLoginInfo.UserName
                }
            };
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { sql }, out int rescount, out string resmsg, param);
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 410;
                info.isSucceed = false;
                info.errmsg = "添加任务明细信息失败！";
                return;
            }
        }
        private void UpdateReceiptStatus(GroupModel model, WebJsonInfo info)
        {
            if (info.errcode != 0)
            {
                return;
            }
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                ["inbound_id"] = model.ReceiptId,
                ["item_id"] = model.ItemId,
                ["qty"] = model.Qty.ToString()
            };
            //更新明细数量
            string updateDetailSql = $@"update &DataSource&wms_d_inbound_d
                                           set processed_qty = ifnull(processed_qty, 0) + @qty
                                         where inbound_id = @inbound_id
                                           and item_id = @item_id";
            updateDetailSql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, updateDetailSql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { updateDetailSql },
                                out int rescount, out string resmsg,
                                new List<Dictionary<string, object>>() { param });
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 510;
                info.isSucceed = false;
                info.errmsg = "更新收货单明细失败！";
                return;
            }
            //检查所有明细是否全部完成
            string selectSql = $@"select count(1) num
                                    from &DataSource&wms_d_inbound_d
                                   where inbound_id = @inbound_id
                                     and finished_qty <> processed_qty";
            selectSql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, selectSql).sql;
            string count = dbProvider.DBSelectOne(userLoginInfo, selectSql, param);

            string toStatus = count == "0" ? InDocStatus.InDocStatus_Finished.ToString()
                                           : InDocStatus.InDocStatus_Executing.ToString();
            param.Add("doc_status", toStatus);
            string updateHeaderSql = string.Empty;
            if (toStatus == InDocStatus.InDocStatus_Finished.ToString())
            {
                updateHeaderSql = $@"update &DataSource&wms_d_inbound_h 
                                        set doc_status = @doc_status,
                                            finish_date = now() 
                                      where t_id = @inbound_id";
            }
            else
            {
                updateHeaderSql = $@"update &DataSource&wms_d_inbound_h 
                                        set doc_status = @doc_status 
                                      where t_id = @inbound_id";
            }
            updateHeaderSql = serviceProvider.GetService<IKernelGloballyVariable>().Globally_SQL(userMode, updateHeaderSql).sql;
            dbProvider.DBRunSql(userLoginInfo, new List<string>() { updateHeaderSql },
                                out rescount, out resmsg,
                                new List<Dictionary<string, object>>() { param });
            if (resmsg != string.Empty || rescount != 1)
            {
                info.errcode = 520;
                info.isSucceed = false;
                info.errmsg = "更新收货单状态失败！";
                return;
            }
        }

        #region 成品组盘入库

        public WebJsonInfo GroupProduct(GroupCPInput.GroupCPModel model)
        {
            WebJsonInfo errorInfo = new WebJsonInfo() { errcode = 0, isSucceed = true };

            #region check
            try
            {
                if (model == null) throw new Exception("参数为空！");

                if (string.IsNullOrWhiteSpace(model.ContainerBarcode))
                {
                    throw new Exception("托盘条码为空！");
                }
                if (string.IsNullOrWhiteSpace(model.DockNumber))
                {
                    throw new Exception("入库站台为空！");
                }
                if (string.IsNullOrEmpty(model.ItemId))
                {
                    if (model.BarcodeList == null || model.BarcodeList.Count == 0)
                    {
                        throw new Exception("件烟码为空！");
                    }
                }
                else
                {
                    if (model.Qty == 0)
                    {
                        throw new Exception("组盘数量必须大于0！");
                    }
                }
            }
            catch (Exception ex)
            {
                errorInfo.errcode = 800;
                errorInfo.isSucceed = false;
                errorInfo.errmsg = ex.Message;
                return errorInfo;
            }
            #endregion

            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"托盘条码{model.ContainerBarcode},站台{model.DockNumber}组盘",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "成品组盘",
                CommonName = "api/wms/GroupContainer/GroupProduct",
            }, () => new ProductGroupDal().GroupProduct(model));

        }

        //获取成品牌号
        public WebJsonInfo GetProductList()
        {
            WebJsonInfo info = new WebJsonInfo() { isSucceed = true };

            try
            {
                DataTable dt = new ProductGroupDal().GetProductList();
                info.json = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                info.isSucceed = false;
                info.errcode = 800;
                info.errmsg = ex.Message;
            }

            return info;
        }

        #endregion
    }
}
