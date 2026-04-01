using Newtonsoft.Json;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Inbound;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static NtiCloud.Model.WMS.WorkTaskModel;

namespace NtiCloud.Service.WMS.Tasking
{
    public class ItemReplacement : WmsObject
    {
        #region DAL
        public void DeleteBy(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql = @"
delete from &DataSource&wms_d_item_replacement  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id 
";
            DBHelper.RunSql(sql, param, "删除出库物料替换表");
        }

        public DataRow Get(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_d_item_replacement  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id 
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询出库物料替换表");

            return dt.Rows[0];
        }

        public DataRow[] GetByOrderID(string orderID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "id", orderID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_d_item_replacement  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and outbound_h_id = @id 
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询出库物料替换表", false);
            return dt.AsEnumerable().ToArray();
        }

        public ItemReplacement Insert(ItemReplacementModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;
            model.ActStatus = "1";

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "item_id", model.ItemID },
                { "item_code", model.ItemCode },
                { "item_name", model.ItemName },
                { "barcode", model.Barcode },
                { "production_lot_no", model.ProductionLotNo },
                { "task_id", model.TaskID },
                { "task_no", model.TaskNo },
                { "location_id", model.LocationID },
                { "location_no", model.LocationNo },
                { "outbound_h_id", model.OutboundHeadID },
                { "outbound_no", model.OutboundNo },
                { "stat_qty", model.StatQty.ToString() },
                { "uom_stat_id", model.UomStatID },
                { "new_item_id", model.NewItemID },
                { "new_item_code", model.NewItemCode },
                { "new_item_name", model.NewItemName },
                { "new_barcode", model.NewBarcode },
                { "new_task_id", model.NewTaskID },
                { "new_task_no", model.NewTaskNo },
                { "new_location_id", model.NewLocationID },
                { "new_location_no", model.NewLocationNo },
                { "new_stat_qty", model.NewStatQty.ToString() },
                { "inbound_h_id", model.InboundHeadID },
                { "inbound_no", model.InboundNo },
                { "prep_lot_no", model.PrepLotNo },
                { "reason", model.Reason },
                { "abnormal_stat_qty", model.AbnormalStatQty.ToString() },
                { "replacement_status", model.ReplacementStatus },
                { "memo", model.Memo },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_D_ITEM_REPLACEMENT ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       item_id, " +
                "       item_code, " +
                "       item_name, " +
                "       barcode, " +
                "       production_lot_no, " +
                "       task_id, " +
                "       task_no, " +
                "       location_id, " +
                "       location_no, " +
                "       outbound_h_id, " +
                "       outbound_no, " +
                "       stat_qty, " +
                "       uom_stat_id, " +
                "       new_item_id, " +
                "       new_item_code, " +
                "       new_item_name, " +
                "       new_barcode, " +
                "       new_task_id, " +
                "       new_task_no, " +
                "       new_location_id, " +
                "       new_location_no, " +
                "       new_stat_qty, " +
                "       inbound_h_id, " +
                "       inbound_no, " +
                "       prep_lot_no, " +
                "       reason, " +
                "       abnormal_stat_qty, " +
                "       replacement_status, " +
                "       memo, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @item_id, " +
                "       @item_code, " +
                "       @item_name, " +
                "       @barcode, " +
                "       @production_lot_no, " +
                "       @task_id, " +
                "       @task_no, " +
                "       @location_id, " +
                "       @location_no, " +
                "       @outbound_h_id, " +
                "       @outbound_no, " +
                "       @stat_qty, " +
                "       @uom_stat_id, " +
                "       @new_item_id, " +
                "       @new_item_code, " +
                "       @new_item_name, " +
                "       @new_barcode, " +
                "       @new_task_id, " +
                "       @new_task_no, " +
                "       @new_location_id, " +
                "       @new_location_no, " +
                "       @new_stat_qty, " +
                "       @inbound_h_id, " +
                "       @inbound_no, " +
                "       @prep_lot_no, " +
                "       @reason, " +
                "       @abnormal_stat_qty, " +
                "       @replacement_status, " +
                "       @memo, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "出库物料替换表");

            return this;
        }

        public ItemReplacement Insert(DataRow row)
        {
            Insert(new ItemReplacementModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                ItemID = row["item_id"].ToString(),
                ItemCode = row["item_code"].ToString(),
                ItemName = row["item_name"].ToString(),
                Barcode = row["barcode"].ToString(),
                ProductionLotNo = row["production_lot_no"].ToString(),
                TaskID = row["task_id"].ToString(),
                TaskNo = row["task_no"].ToString(),
                LocationID = row["location_id"].ToString(),
                LocationNo = row["location_no"].ToString(),
                OutboundHeadID = row["outbound_h_id"].ToString(),
                OutboundNo = row["outbound_no"].ToString(),
                StatQty = DBHelper.GetDecimal(row["stat_qty"]),
                UomStatID = row["uom_stat_id"].ToString(),
                NewItemID = row["new_item_id"].ToString(),
                NewItemCode = row["new_item_code"].ToString(),
                NewItemName = row["new_item_name"].ToString(),
                NewBarcode = row["new_barcode"].ToString(),
                NewTaskID = row["new_task_id"].ToString(),
                NewTaskNo = row["new_task_no"].ToString(),
                NewLocationID = row["new_location_id"].ToString(),
                NewLocationNo = row["new_location_no"].ToString(),
                NewStatQty = DBHelper.GetDecimal(row["new_stat_qty"]),
                InboundHeadID = row["inbound_h_id"].ToString(),
                InboundNo = row["inbound_no"].ToString(),
                PrepLotNo = row["prep_lot_no"].ToString(),
                Reason = row["reason"].ToString(),
                AbnormalStatQty = DBHelper.GetDecimal(row["abnormal_stat_qty"]),
                ReplacementStatus = row["replacement_status"].ToString(),
                Memo = row["memo"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

            return this;
        }

        public void UpdateActStatus(string id, string actStatus, string oldActStatus = null)
        {
            if (oldActStatus == null)
            {
                oldActStatus = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "act_status", actStatus },
                { "old_act_status", oldActStatus },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_d_item_replacement  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       act_status = @act_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (@old_act_status = ' ' 
        or act_status = @old_act_status)
";
            DBHelper.RunSql(sql, param, "更新状态，默认值为'1'。'0'：已删除'1'：未删除");
        }

        public void UpdateMemo(string id, string memo, string oldMemo = null)
        {
            if (oldMemo == null)
            {
                oldMemo = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "memo", memo },
                { "old_memo", oldMemo },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_d_item_replacement  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       memo = @memo
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (@old_memo = ' ' 
        or memo = @old_memo)
";
            DBHelper.RunSql(sql, param, "更新备注");
        }
        public void UpdateReason(string id, string reason, string oldReason = null)
        {
            if (oldReason == null)
            {
                oldReason = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "reason", reason },
                { "old_reason", oldReason },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_d_item_replacement  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       reason = @reason
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (@old_reason = ' ' 
        or reason = @old_reason)
";
            DBHelper.RunSql(sql, param, "更新原因");
        }

        public void Update(DataRow row)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "t_id", row["t_id"].ToString() },
                { "item_id", row["item_id"].ToString() },
                { "item_code", row["item_code"].ToString() },
                { "item_name", row["item_name"].ToString() },
                { "barcode", row["barcode"].ToString() },
                { "inbound_h_id", row["inbound_h_id"].ToString() },
                { "inbound_no", row["inbound_no"].ToString() },
                { "prep_lot_no", row["prep_lot_no"].ToString() },
                { "production_lot_no", row["production_lot_no"].ToString() },
                { "task_id", row["task_id"].ToString() },
                { "task_no", row["task_no"].ToString() },
                { "location_id", row["location_id"].ToString() },
                { "location_no", row["location_no"].ToString() },
                { "outbound_h_id", row["outbound_h_id"].ToString() },
                { "outbound_no", row["outbound_no"].ToString() },
                { "stat_qty", row["stat_qty"].ToString() },
                { "uom_stat_id", row["uom_stat_id"].ToString() },
                { "replacement_no", row["replacement_no"].ToString() },
                { "new_item_id", row["new_item_id"].ToString() },
                { "new_item_code", row["new_item_code"].ToString() },
                { "new_item_name", row["new_item_name"].ToString() },
                { "new_barcode", row["new_barcode"].ToString() },
                { "new_task_id", row["new_task_id"].ToString() },
                { "new_task_no", row["new_task_no"].ToString() },
                { "new_location_id", row["new_location_id"].ToString() },
                { "new_location_no", row["new_location_no"].ToString() },
                { "new_stat_qty", row["new_stat_qty"].ToString() },
                { "new_inbound_h_id", row["new_inbound_h_id"].ToString() },
                { "new_inbound_no", row["new_inbound_no"].ToString() },
                { "new_prep_lot_no", row["new_prep_lot_no"].ToString() },
                { "reason", row["reason"].ToString() },
                { "abnormal_stat_qty", row["abnormal_stat_qty"].ToString() },
                { "replacement_status", row["replacement_status"].ToString() },
                { "memo", row["memo"].ToString() },
                { "act_status", row["act_status"].ToString() }
            };
            string sql = @"
update &DataSource&wms_d_item_replacement 
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       item_id= @item_id, 
       item_code= @item_code, 
       item_name= @item_name, 
       barcode= @barcode, 
       inbound_h_id= @inbound_h_id, 
       inbound_no= @inbound_no, 
       prep_lot_no= @prep_lot_no, 
       production_lot_no= @production_lot_no, 
       task_id= @task_id, 
       task_no= @task_no, 
       location_id= @location_id, 
       location_no= @location_no, 
       outbound_h_id= @outbound_h_id, 
       outbound_no= @outbound_no, 
       stat_qty= @stat_qty, 
       uom_stat_id= @uom_stat_id, 
       replacement_no= @replacement_no, 
       new_item_id= @new_item_id, 
       new_item_code= @new_item_code, 
       new_item_name= @new_item_name, 
       new_barcode= @new_barcode, 
       new_task_id= @new_task_id, 
       new_task_no= @new_task_no, 
       new_location_id= @new_location_id, 
       new_location_no= @new_location_no, 
       new_stat_qty= @new_stat_qty, 
       new_inbound_h_id= @new_inbound_h_id, 
       new_inbound_no= @new_inbound_no, 
       new_prep_lot_no= @new_prep_lot_no, 
       reason= @reason, 
       abnormal_stat_qty= @abnormal_stat_qty, 
       replacement_status= @replacement_status, 
       memo= @memo, 
       act_status= @act_status 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "修改出库物料替换表");
        }

        public void UpdateAbnormalStatQty(string id, decimal abnormalStatQty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "abnormal_stat_qty", abnormalStatQty.ToString() },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_d_item_replacement  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       abnormal_stat_qty = @abnormal_stat_qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
";
            DBHelper.RunSql(sql, param, "更新异常数量");
        }


        public void UpdateReplacementStatus(string id, string replacementStatus, string oldReplacementStatus = null)
        {
            if (oldReplacementStatus == null)
            {
                oldReplacementStatus = " ";
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "replacement_status", replacementStatus },
                { "old_replacement_status", oldReplacementStatus },
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_d_item_replacement  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       replacement_status = @replacement_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (@old_replacement_status = ' ' 
        or replacement_status = @old_replacement_status)
";
            DBHelper.RunSql(sql, param, "更新替换状态");
        }
        #endregion
        private DataRow GetOutboundHead(string waveDetailID)
        {
            DataRow waveDetail = Wave.GetWaveDetail(waveDetailID);
            return OutboundOrder.GetOrder(waveDetail[wd.OutboundHeadID].ToString());
        }


        private Model.WorkTaskModel RenewTask(List<string> allocatedLocations, DataRow taskRow, string location = null)
        {
            DataRow[] taskItemRows = WorkTask.GetTaskItemsBy(taskRow[t.ID].ToString());
            DataRow containerDetail = Container.GetDetail(taskItemRows[0][ti.ContainerDetailID].ToString());

            if (string.IsNullOrWhiteSpace(location))
            {
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "I_WAREHOUSEID", taskRow[t.WarehouseID].ToString() },
                    { "ITEMID", taskItemRows[0][ti.ItemID].ToString() },
                    { "I_ITEMID", taskItemRows[0][ti.ItemID].ToString() },
                    { "I_BATCH_NO", containerDetail[cd.LotNo].ToString() },
                    { "I_SPEC", containerDetail[cd.Spec].ToString() },
                    { "I_STORAGE_LOCGP1", "" },
                    { "I_STORAGE_LOCGP2", "" },
                    { "I_EXPIRY_DATE", "" },
                    { "I_ZONE", "" },
                    { "I_HEADID", "" },
                    { "I_DETAILID", "" },
                    { "I_PACKING_TYPE", "" },
                    { "I_ROOM_NO", "" },
                };
                //location = LocationAllocator.AllocateInventory(param,
                //            WmsConfig.GetOrDefault("Outbound:AllocationRule", "work_task_4"));
                //string location = new LocationAllocator().AllocateInventoryV2(param);
                //string location;
                if (WmsConfig.GetOrDefault("Outbound:UsingTestAllocator", "0") == "0")
                {
                    location = LocationAllocator.AllocateInventory(param,
                                WmsConfig.GetOrDefault("Outbound:AllocationRule", "work_task_4"));
                }
                else
                {
                    location = new LocationAllocator().AllocateInventoryByStoreProc(param);
                }
                // TODO
                // 排除堆垛机故障巷道
                // 按模块批次查找同等级
                // 按配方查找同等级
                // 查找同等级
            }

            DataRow locRow = Location.Get(location);
            location = locRow["t_id"].ToString();
            if (Location.IsRackLocation(locRow) &&
                locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
            {
                throw new WmsException(500, "指定货位状态不正确");
            }

            Location.UpdateStatus(locRow[l.ID].ToString(),
                LocationStatus.LocationStatus_Out.ToString(), locRow[l.LocationStatus].ToString());

            allocatedLocations.Add(location);

            // 取得货位库存
            DataRow[] inventoryRows = Container.GetInventoryDetails(locRow);

            List<WorkTaskItemModel> taskItemList = WorkTask.AllocateItems(taskItemRows, inventoryRows);

            DataRow[] taskRows = WorkTask.GetDependentTasks(taskRow);

            RoutePurpose purpose = WorkTask.DetermineRoutePurpose(locRow, inventoryRows);
            DataRow drEq= Equipment.GetEquipment("1027");
            Model.WorkTaskModel task = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),
                SeqNo = DBHelper.GetDecimal(taskRow[t.SeqNo]),
                Priority = DBHelper.GetDecimal(taskRow[t.Priority]),
                ContainerID = inventoryRows[0]["t_id"].ToString(),
                ContainerBarcode = inventoryRows[0]["container_barcode"].ToString(),
                //SrcAddrID = taskRow[t.SrcAddrID].ToString(),
                //SrcAddrType = taskRow[t.SrcAddrType].ToString(),
                //SrcAddrText = taskRow[t.SrcAddrText].ToString(),
                SrcAddrID = locRow[l.ID].ToString(),
                SrcAddrType = AddressType.AddressType_Location.ToString(),
                SrcAddrText = locRow[l.LocationCode].ToString(),
                DestAddrID = drEq["t_id"].ToString(),
                DestAddrType = AddressType.AddressType_Station.ToString(),
                DestAddrText = drEq["equipment_code"].ToString(),
                RefDocType = taskRow[t.RefDocType].ToString(),
                RefDocID = taskRow[t.RefDocID].ToString(),
                RefDocNo = taskRow[t.RefDocNo].ToString(),
            };
            WorkTask.SaveTask(purpose, task, taskItemList);
            return task;
        }

        public string ReplaceItem(DataRow taskRow, List<string> allocatedLocations)
        {
            string taskState = taskRow["task_status"].ToString();
            if (taskRow[t.ExecutorType].ToString() == TaskExecutor.TaskExecutor_WCS.ToString())
            {
                throw new WmsException(500, "非WCS执行的任务不能进行堆垛机故障替换");
            }
            if (taskState == TaskStatus.TaskStatus_Executing.ToString()
                    && taskState == TaskStatus.TaskStatus_WaitingExecute.ToString())
            {
                throw new WmsException(500, "需进行堆垛机故障替换的任务状态不正确");
            }

            string recycleInfo = WorkTaskInterface.Recycle(taskRow["t_id"].ToString());
            if (!string.IsNullOrEmpty(recycleInfo))
            {
                throw new Exception("回收WCS任务失败:" + recycleInfo);
            }
            Model.WorkTaskModel newTask = RenewTask(allocatedLocations, taskRow);
            DataRow taskItemRow = WorkTask.GetTaskItemsBy(taskRow[t.ID].ToString()).First();
            DataRow itemRow = Item.Get(taskItemRow[ti.ItemID].ToString());
            DataRow order = GetOutboundHead(taskItemRow[ti.RefDocDetailID].ToString());
            DataRow containerDetailRow = Container.GetDetail(taskItemRow[ti.ContainerDetailID].ToString());

            //throw new WmsException(500, "Break");
            DataRow newTaskRow = WorkTask.Get(newTask.ID);
            DataRow newTaskItemRow = WorkTask.GetTaskItemsBy(newTask.ID).First();
            DataRow newItemRow = Item.Get(newTaskItemRow[ti.ItemID].ToString());
            DataRow newContainerDetailRow = Container.GetDetail(newTaskItemRow[ti.ContainerDetailID].ToString());
            //PutawayHeaderModel newInboundHead = new Putaway().GetPutawayHeader(newContainerDetailRow[cd.InboundHeadID].ToString());
            PutawayHeaderModel newInboundHead = null;
            if (!string.IsNullOrWhiteSpace(newContainerDetailRow[cd.InboundHeadID].ToString()))
            {
                newInboundHead = new Putaway().GetPutawayHeader(newContainerDetailRow[cd.InboundHeadID].ToString());
            }

            //WorkTask.UpdateTaskAndItemActStatus(taskRow[t.ID].ToString(), "0");
            //WorkTask.RecycleTask(taskRow);
            WorkTask.RevertWaveTypeTask(taskRow);
            //Wave.AddHeadLeftTaskQty(taskRow[t.RefDocID].ToString(), 1);
            //WorkTask.UpdateActStatus(taskRow[t.ID].ToString(), "0", "1");
            //WorkTask.UpdateActStatus(newTaskRow[t.ID].ToString(), "0", "1");
            WorkTask.UpdateTaskStatus(taskRow[t.ID].ToString(),
                TaskStatus.TaskStatus_Replacing.ToString(), TaskStatus.TaskStatus_Create.ToString());
            WorkTask.UpdateTaskStatus(newTaskRow[t.ID].ToString(),
                TaskStatus.TaskStatus_Replacing.ToString(), TaskStatus.TaskStatus_Create.ToString());

            string id = SnowflakeID.Guid().ToString();
            Insert(new ItemReplacementModel()
            {
                ID = id,
                ItemID = itemRow[i.ID].ToString(),
                ItemCode = itemRow[i.ItemCode].ToString(),
                ItemName = itemRow[i.ItemName].ToString(),
                Barcode = taskRow[t.ContainerBarcode].ToString(),
                ProductionLotNo = order["prod_lot_no"].ToString(),
                TaskID = taskRow[t.ID].ToString(),
                TaskNo = taskRow[t.TaskNo].ToString(),
                LocationID = taskRow[t.SrcAddrID].ToString(),
                LocationNo = taskRow[t.SrcAddrText].ToString(),
                OutboundHeadID = order[oh.ID].ToString(),
                OutboundNo = order[oh.DocNo].ToString(),
                StatQty = DBHelper.GetDecimal(containerDetailRow[cd.StatQty]),
                UomStatID = containerDetailRow[cd.UomStatID].ToString(),
                NewItemID = newItemRow[i.ID].ToString(),
                NewItemCode = newItemRow[i.ItemCode].ToString(),
                NewItemName = newItemRow[i.ItemName].ToString(),
                NewBarcode = newTaskRow[t.ContainerBarcode].ToString(),
                NewTaskID = newTaskRow[t.ID].ToString(),
                NewTaskNo = newTaskRow[t.TaskNo].ToString(),
                NewLocationID = newTaskRow[t.SrcAddrID].ToString(),
                NewLocationNo = newTaskRow[t.SrcAddrText].ToString(),
                NewStatQty = DBHelper.GetDecimal(newContainerDetailRow[cd.StatQty]),
                InboundHeadID = newInboundHead?.t_id,
                InboundNo = newInboundHead?.doc_no,
                PrepLotNo = containerDetailRow[cd.LotNo].ToString(), // 备料批次
                Reason = ReplacementReason.ReplacementReason_DeviceFault.ToString(),
                ReplacementStatus = ReplacementStatus.ReplacementStatus_New.ToString(),
            });

            return id;
        }
        /// <summary>
        /// 烟包替换
        /// </summary>
        /// <param name="taskNoOrIds">任务ID，多个用逗号隔开</param>
        /// <returns>烟包替换表中的记录ID列表</returns>
        public List<string> ReplaceItems(string taskNoOrIds)
        {
            // 通知WCS暂停波次
            DataRow[] taskRows = WorkTask.GetWorkTasks(taskNoOrIds);
            List<string> waveNoList = taskRows.Select(r => r[t.RefDocNo].ToString()).Distinct().ToList();
            List<string> idList = new List<string>();
            List<string> allocatedLocations = new List<string>();
            waveNoList.ForEach(waveNo =>
            {
                WebJsonInfo info = WorkTaskInterface.NotifyWavePause(waveNo);
                if (!info.isSucceed || info.errcode != 0)
                {
                    //throw new WmsException(500, $"向WCS申请暂停波次{waveNo}失败!");
                    UserLogModel log = new UserLogModel()
                    {
                        LogText = $"替换物料前向WCS申请暂停波次{waveNo}",
                        LogTextLong = "参数：" + JsonConvert.SerializeObject(taskNoOrIds),
                        TypeCode = "替换物料",
                        CommonName = "ItemReplacement/ReplaceItems",
                    };
                    LogHelper.LogError(JsonConvert.SerializeObject(log));
                    UserLog.Add(log);
                    return;
                }

                DataRow[] recycleRows = taskRows.Where(r => r[t.TaskNo].ToString() == waveNo).ToArray();
                // 通知WCS回收任务
                //info = WorkTaskInterface.Recycle(taskNoOrIds.Split(',').ToList());
                info = WorkTaskInterface.Recycle(recycleRows.Select(r => r[t.ID].ToString()).ToList());
                string taskNos = (string)info.json;
                if (string.IsNullOrWhiteSpace(taskNos))
                {
                    //throw new WmsException(500, "无任务需要替换");
                    return;
                }

                // 保存每次替换的ID，然后返回
                List<string> noList = taskNos.Split(',').ToList();
                foreach (string taskID in noList)
                {
                    DataRow taskRow = WorkTask.Get(taskID);
                    idList.Add(ReplaceItem(taskRow, allocatedLocations));
                }
            });

            return idList;
        }
        public void ReplaceItemsWithRollbackLocationStatus(string taskNoOrIds)
        {
            List<string> allocatedLocations = new List<string>();
            try
            {

                List<string> idList = taskNoOrIds.Split(',').ToList();
                foreach (string taskID in idList)
                {
                    DataRow taskRow = WorkTask.Get(taskID);
                    ReplaceItem(taskRow, allocatedLocations);
                }
            }
            catch (Exception ex)
            {
                DBHelper.EndTransaction(false);
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);

                Location.RestoreLocationStockState(allocatedLocations, out List<string> failureList,
                new Dictionary<string, object>() {
                    { LocationStatus.LocationStatus_Out.ToString(), LocationStatus.LocationStatus_Stored.ToString() }
                });
                DBHelper.BeginTransaction();

                if (failureList.Count > 0)
                {
                    throw new WmsException(-406, $"货位[{string.Join(",", failureList)}]状态未能恢复!", ex);
                }
                throw;
            }
        }

        private void AddItemReplacement(string itemBarcode, string reason, decimal abnormalWeight)
        {
            DataRow containerRow = Container.GetHeadAll(itemBarcode);
            DataRow[] ctnDtlRows = Container.GetDetailsAll(containerRow[ch.ID].ToString());
            if (ctnDtlRows.Length > 1)
            {
                throw new WmsException(500, "该条码下有多种物料，无法替换");
            }
            DataRow[] taskRows = WorkTask.GetByContainer(itemBarcode, TaskType.TaskType_OutBound.ToString());
            if (taskRows.Length > 1)
            {
                throw new WmsException(500, "该条码有多条任务，无法替换");
            }
            DataRow taskRow = taskRows.First();
            if (taskRow[t.TaskStatus].ToString() != TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "条码所属任务还未完成，本功能仅对已完成任务开放");
            }
            DataRow containerDetailRow = ctnDtlRows.First();
            DataRow taskItemRow = WorkTask.GetTaskItemsBy(taskRow[t.ID].ToString()).First();
            DataRow order = GetOutboundHead(taskItemRow[ti.RefDocDetailID].ToString());

            DataRow itemRow = Item.Get(containerDetailRow[cd.ItemID].ToString());
            Insert(new ItemReplacementModel()
            {
                ItemID = itemRow[i.ID].ToString(),
                ItemCode = itemRow[i.ItemCode].ToString(),
                ItemName = itemRow[i.ItemName].ToString(),
                Barcode = taskRow[t.ContainerBarcode].ToString(),
                ProductionLotNo = order["prod_lot_no"].ToString(),
                TaskID = taskRow[t.ID].ToString(),
                TaskNo = taskRow[t.TaskNo].ToString(),
                LocationID = taskRow[t.SrcAddrID].ToString(),
                LocationNo = taskRow[t.SrcAddrText].ToString(),
                OutboundHeadID = order[oh.ID].ToString(),
                OutboundNo = order[oh.DocNo].ToString(),
                StatQty = DBHelper.GetDecimal(containerDetailRow[cd.StatQty]),
                UomStatID = containerDetailRow[cd.UomStatID].ToString(),
                PrepLotNo = containerDetailRow[cd.LotNo].ToString(), // 备料批次
                Reason = reason,
                AbnormalStatQty = abnormalWeight,
                ReplacementStatus = ReplacementStatus.ReplacementStatus_New.ToString(),
            });


        }
        public void AddItemReplacement(AddItemReplacementParam p)
        {
            string errMsg = string.Empty;
            errMsg += string.IsNullOrWhiteSpace(p.ItemBarcode) ? "条码不能为空！" : "";
            errMsg += p.Reason != ReplacementReason.ReplacementReason_MoldyItem.ToString() ? "替换原因错误！" : "";
            errMsg += !decimal.TryParse(p.AbnormalWeight, out decimal abnormalWeight) ? "异常重量请输入数字！" : "";

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                throw new WmsException(500, errMsg);
            }

            AddItemReplacement(p.ItemBarcode, p.Reason, abnormalWeight);
        }
        public void ModifyReplacement(ModifyReplacementParam p)
        {
            if (p.Reason == ReplacementReason.ReplacementReason_DeviceFault.ToString())
            {
                ModifyReplacement4DeviceFault(p);
            }
            else
            {
                ModifyReplacement4MoldyItem(p);
            }
        }
        public void ModifyReplacement4MoldyItem(ModifyReplacementParam p)
        {
            if (p.Reason != ReplacementReason.ReplacementReason_MoldyItem.ToString())
            {
                throw new WmsException(500, "替换原因错误！");
            }
            DataRow replRow = Get(p.ReplacementID);
            List<string> allocatedLocations = new List<string>();
            replRow["memo"] = p.Memo;
            if (string.IsNullOrWhiteSpace(p.AbnormalWeight))
            {
                replRow["abnormal_stat_qty"] = DBNull.Value;
            }
            else
            {
                replRow["abnormal_stat_qty"] = decimal.Parse("0" + p.AbnormalWeight);

            }
            if (string.IsNullOrWhiteSpace(p.LocationNoOrID))
            {
                if (!string.IsNullOrWhiteSpace(replRow["new_location_id"].ToString()))
                {
                    CancelNewTask(replRow["new_task_id"].ToString());
                }
            }
            else
            {
                DataRow locRow = Location.Get(p.LocationNoOrID);
                if (replRow["new_location_id"].ToString() != locRow[l.ID].ToString())
                {
                    if (!string.IsNullOrWhiteSpace(replRow["new_location_id"].ToString()))
                    {
                        CancelNewTask(replRow["new_task_id"].ToString());
                    }

                    ProcessNewLocation(locRow[l.ID].ToString(), replRow, allocatedLocations);
                }
            }
            Update(replRow);
            //UpdateMemo(replRow["t_id"].ToString(), p.Memo);
            //UpdateAbnormalStatQty(replRow["t_id"].ToString(), p.AbnormalWeight);
        }

        private void ProcessNewLocation(string locationID, DataRow replRow, List<string> allocatedLocations)
        {
            Model.WorkTaskModel newTask = RenewTask(allocatedLocations,
                WorkTask.Get(replRow["task_id"].ToString()), locationID);

            DataRow newTaskRow = WorkTask.Get(newTask.ID);
            DataRow newTaskItemRow = WorkTask.GetTaskItemsBy(newTask.ID).First();
            DataRow newItemRow = Item.Get(newTaskItemRow[ti.ItemID].ToString());
            DataRow newContainerDetailRow = Container.GetDetail(newTaskItemRow[ti.ContainerDetailID].ToString());
            PutawayHeaderModel newInboundHead = null;
            if (!string.IsNullOrWhiteSpace(newContainerDetailRow[cd.InboundHeadID].ToString()))
            {
                newInboundHead = new Putaway().GetPutawayHeader(newContainerDetailRow[cd.InboundHeadID].ToString());
            }

            WorkTask.UpdateTaskStatus(newTaskRow[t.ID].ToString(),
                TaskStatus.TaskStatus_Replacing.ToString(), TaskStatus.TaskStatus_Create.ToString());

            replRow["new_item_id"] = newItemRow[i.ID];
            replRow["new_item_code"] = newItemRow[i.ItemCode];
            replRow["new_item_name"] = newItemRow[i.ItemName];
            replRow["new_barcode"] = newTaskRow[t.ContainerBarcode];
            replRow["new_task_id"] = newTaskRow[t.ID];
            replRow["new_task_no"] = newTaskRow[t.TaskNo];
            replRow["new_location_id"] = newTaskRow[t.SrcAddrID];
            replRow["new_location_no"] = newTaskRow[t.SrcAddrText];
            replRow["new_stat_qty"] = newContainerDetailRow[cd.StatQty];
            replRow["inbound_h_id"] = newInboundHead?.t_id;
            replRow["inbound_no"] = newInboundHead?.doc_no;
            replRow["new_prep_lot_no"] = newContainerDetailRow[cd.LotNo];
        }

        public void ModifyReplacement4DeviceFault(ModifyReplacementParam p)
        {
            DataRow replRow = Get(p.ReplacementID);
            DataRow locRow = Location.Get(p.LocationNoOrID);
            List<string> allocatedLocations = new List<string>();
            replRow["memo"] = p.Memo;
            if (string.IsNullOrWhiteSpace(p.AbnormalWeight))
            {
                replRow["abnormal_stat_qty"] = DBNull.Value;
            }
            else
            {
                replRow["abnormal_stat_qty"] = decimal.Parse("0" + p.AbnormalWeight);

            }
            if (replRow["new_location_id"].ToString() != locRow[l.ID].ToString())
            {
                CancelNewTask(replRow["new_task_id"].ToString());
                ProcessNewLocation(locRow[l.ID].ToString(), replRow, allocatedLocations);
            }
            Update(replRow);
            //UpdateMemo(replRow["t_id"].ToString(), p.Memo);
            //UpdateAbnormalStatQty(replRow["t_id"].ToString(), p.AbnormalWeight);
        }
        public void ConfirmItemReplacement(string replacementIds)
        {
            List<string> idList = replacementIds.Split(',').ToList();
            List<string> taskIDList = new List<string>();
            foreach (string id in idList)
            {
                DataRow replRow = Get(id);
                if (replRow["replacement_status"].ToString() != ReplacementStatus.ReplacementStatus_New.ToString())
                {
                    throw new WmsException(500, $"{replRow["item_name"]}[{replRow["barcode"]}]状态不正确，无法确认");
                }
                //WorkTask.UpdateTaskStatus(id, TaskStatus.TaskStatus_WaitingExecute.ToString(),
                //    TaskStatus.TaskStatus_Create.ToString());
                string newTaskID = replRow["new_task_id"].ToString();
                string oldTaskID = replRow["task_id"].ToString();
                //WorkTask.UpdateActStatus(oldTaskID, "1", "0");
                //WorkTask.UpdateActStatus(newTaskID, "1", "0");

                if (!IsMoldyReason(replRow))
                {
                    // 托盘中的备料单信息还需要交换
                    SwapInfo(replRow);

                    WorkTask.UpdateTaskStatus(oldTaskID,
                        TaskStatus.TaskStatus_Create.ToString(), TaskStatus.TaskStatus_Replacing.ToString());
                }
                WorkTask.UpdateTaskStatus(newTaskID,
                    TaskStatus.TaskStatus_Create.ToString(), TaskStatus.TaskStatus_Replacing.ToString());

                WorkTask.StartTask(newTaskID);
                taskIDList.Add(newTaskID);

                WorkTaskItem.AddWaveDetailProcessQty(oldTaskID, 1);
                WorkTask.RemoveWaveTasks(oldTaskID);
                UpdateReplacementStatus(replRow["t_id"].ToString()
                    , ReplacementStatus.ReplacementStatus_Confirmed.ToString()
                    , replRow["replacement_status"].ToString());
            }

            //UpdateActStatus(replRow["t_id"].ToString(), "")

            // 下发任务给WCS
            WorkTaskInterface.Send(taskIDList);
        }

        private void SwapInfo(DataRow replRow)
        {
            DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(replRow["task_id"].ToString());
            DataRow[] newTaskItemRows = WorkTaskItem.GetByTaskID(replRow["new_task_id"].ToString());
            DataRow ctnDtlRow = Container.GetDetail(taskItemRows.First()[ti.ContainerDetailID].ToString());
            DataRow newCtnDtlRow = Container.GetDetail(newTaskItemRows.First()[ti.ContainerDetailID].ToString());
            foreach (DataRow taskItemRow in taskItemRows)
            {
                DataRow ctnDetail = Container.GetDetail(taskItemRow[ti.ContainerDetailID].ToString());
                ctnDetail[cd.LotNo] = newCtnDtlRow[cd.LotNo];
                ctnDetail[cd.OwnerLotNo] = newCtnDtlRow[cd.OwnerLotNo];
                Container.UpdateDetail(ctnDetail);
            }
            foreach (DataRow taskItemRow in newTaskItemRows)
            {
                DataRow ctnDetail = Container.GetDetail(taskItemRow[ti.ContainerDetailID].ToString());
                ctnDetail[cd.LotNo] = ctnDtlRow[cd.LotNo];
                ctnDetail[cd.OwnerLotNo] = ctnDtlRow[cd.OwnerLotNo];
                Container.UpdateDetail(ctnDetail);
            }
        }

        private bool IsMoldyReason(DataRow replacementRow)
        {
            if (replacementRow["reason"].ToString() == ReplacementReason.ReplacementReason_MoldyItem.ToString())
            {
                return true;
            }
            return false;
        }
        private bool IsConfirmed(DataRow replacementRow)
        {
            if (replacementRow["replacement_status"].ToString() == ReplacementStatus.ReplacementStatus_Confirmed.ToString())
            {
                return true;
            }
            return false;
        }
        private bool IsStarted(DataRow replacementRow)
        {
            if (replacementRow["replacement_status"].ToString() == ReplacementStatus.ReplacementStatus_Start.ToString())
            {
                return true;
            }
            return false;
        }
        public void StartReplace(string replacementIds)
        {
            List<string> idList = replacementIds.Split(',').ToList();
            List<string> taskIDList = new List<string>();
            foreach (string id in idList)
            {
                DataRow replRow = Get(id);
                if (!IsMoldyReason(replRow))
                {
                    throw new WmsException(500, $"{replRow["item_name"]}[{replRow["barcode"]}]并非霉变引发的退换，无法启动退换");
                }
                if (!IsConfirmed(replRow))
                {
                    throw new WmsException(500, $"{replRow["item_name"]}[{replRow["barcode"]}]状态不正确，无法启动退换");
                }
                string seqNo = WmsHelper.GetSequence("WMS_ItemReplaceNo");
                replRow["replacement_no"] = seqNo;
                Update(replRow);

                UpdateReplacementStatus(replRow["t_id"].ToString()
                    , ReplacementStatus.ReplacementStatus_Start.ToString()
                    , replRow["replacement_status"].ToString());
            }

            //UpdateActStatus(replRow["t_id"].ToString(), "")

            //WorkTaskInterface.Send(idList);
        }
        public void FinishReplace(string replacementIds)
        {
            List<string> idList = replacementIds.Split(',').ToList();
            List<string> taskIDList = new List<string>();
            foreach (string id in idList)
            {
                DataRow replRow = Get(id);
                if (!IsMoldyReason(replRow))
                {
                    throw new WmsException(500, $"{replRow["item_name"]}[{replRow["barcode"]}]并非霉变引发的退换，无法启动退换");
                }
                if (!IsStarted(replRow))
                {
                    throw new WmsException(500, $"{replRow["item_name"]}[{replRow["barcode"]}]状态不正确，无法确认入库");
                }
                UpdateReplacementStatus(replRow["t_id"].ToString()
                    , ReplacementStatus.ReplacementStatus_Finished.ToString()
                    , replRow["replacement_status"].ToString());
            }

            //UpdateActStatus(replRow["t_id"].ToString(), "")

            //WorkTaskInterface.Send(idList);
        }
        private void CancelNewTask(string newTaskID)
        {
            DataRow newTaskRow = WorkTask.Get(newTaskID);

            WorkTask.UpdateTaskStatus(newTaskRow[t.ID].ToString(),
                TaskStatus.TaskStatus_Create.ToString(), TaskStatus.TaskStatus_Replacing.ToString());

            WorkTaskItem.AddWaveDetailProcessQty(newTaskRow, 1);

            WorkTask.RemoveWaveTasks(newTaskRow[t.ID].ToString());

            WorkTask.MoveToHistory(newTaskID);
        }
        public void CancelItemReplacement(string replacementIds)
        {
            List<string> idList = replacementIds.Split(',').ToList();
            foreach (string id in idList)
            {
                DataRow replRow = Get(id);
                if (replRow["replacement_status"].ToString() != ReplacementStatus.ReplacementStatus_New.ToString())
                {
                    throw new WmsException(500, $"{replRow["item_name"]}[{replRow["barcode"]}]状态不正确，无法取消");
                }
                //DataRow newTaskRow = WorkTask.Get(replRow["new_task_id"].ToString());

                WorkTask.UpdateTaskStatus(replRow["task_id"].ToString(),
                    TaskStatus.TaskStatus_NotExecute.ToString(), TaskStatus.TaskStatus_Replacing.ToString());

                //WorkTask.UpdateTaskStatus(newTaskRow[t.ID].ToString(),
                //    TaskStatus.TaskStatus_Create.ToString(), TaskStatus.TaskStatus_Replacing.ToString());

                //WorkTaskItem.AddWaveDetailProcessQty(newTaskRow, 1);

                //WorkTask.RemoveWaveTasks(newTaskRow[t.ID].ToString());
                //WorkTask.MoveToHistory(replRow["new_task_id"].ToString());
                CancelNewTask(replRow["new_task_id"].ToString());

                UpdateActStatus(id, "0");
            }
        }

        internal bool ExistsReplacingOrder(string orderID)
        {
            DataRow[] rows = GetByOrderID(orderID);
            rows = rows.Where(r => r["replacement_status"].ToString() == "ReplacementStatus_New").ToArray();
            if (rows.Length > 0)
            {
                return true;
            }
            return false;
        }
        public WMS.Model.ItemReplacementModelDB GetItemReplacement(string id) {
            string sql = "select * from  &DataSource&wms_d_item_replacement where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", id);
            return DBHelper.ConvertTableToObject<ItemReplacementModelDB>(DBHelper.Select(sql, lstParam))[0];
        }
        /// <summary>
        /// 获取霉变替换任务清单
        /// </summary>
        /// <returns></returns>
        public List<WMS.Model.ItemReplacementOutModel> GetItemReplaceList() {
            string sql = @"select * from (
                                        select r.t_id,
                                               r.new_item_code as item_code,
                                               r.new_item_name as item_name,
                                               d.spec,
                                               d.container_id,
                                               d.t_id          as container_d_id,
                                               r.create_date
                                          from &DataSource&wms_d_item_replacement r
                                         inner join &DataSource&wms_b_task t
                                            on r.new_task_id = t.t_id
                                         inner join &DataSource&wms_b_container_d d
                                            on t.container_id = d.container_id
                                         where r.reason = 'ReplacementReason_MoldyItem'
                                           and r.replacement_status = 'ReplacementStatus_Start'
                                        union
                                        select r.t_id,
                                               r.new_item_code as item_code,
                                               r.new_item_name,
                                               d.spec ,
                                               d.container_id,
                                               d.t_id as container_d_id,
                                               r.create_date
                                          from &DataSource&wms_d_item_replacement r
                                         inner join &DataSource&wms_b_task_his t
                                            on r.new_task_id = t.t_id
                                         inner join &DataSource&wms_b_container_d d
                                            on t.container_id = d.container_id
                                         where r.reason = 'ReplacementReason_MoldyItem'
                                           and r.replacement_status = 'ReplacementStatus_Start'
                                           ) order by create_date desc
                                        ";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            DataTable dt = DBHelper.Select(sql, lstParams, "", false);
            return DBHelper.ConvertTableToObject<WMS.Model.ItemReplacementOutModel>(dt);
        }
        /// <summary>
        /// 提交烟包替换
        /// <br/>
        /// 将新烟包替换到原烟包备料单号与小批次    
        /// </summary>
        /// <param name="itemreplace"></param>
        /// <returns></returns>
        public ContainerComponent SubmitReplace(NtiCloud.Model.WMS.ItemReplaceInModel itemreplace)
        {
            if (string.IsNullOrEmpty(itemreplace.new_container_code))
            {
                throw new Exception("新烟包条码不能为空");
            }
            if (string.IsNullOrEmpty(itemreplace.container_id)
                || string.IsNullOrEmpty(itemreplace.container_d_id))
            {
                throw new Exception("原烟包ID不能为空");
            }
            if (CheckBarcodeIsIn(itemreplace.new_container_code))
            {
                throw new Exception("新烟包已经在库，不能替换入库");
            }
            if (itemreplace.new_container_code.Length != 46) {
                throw new Exception("请扫描正确的烟包条码");
            }
           
            ItemReplacementModelDB replacemodel =   GetItemReplacement(itemreplace.t_id);
            if (!string.Equals(itemreplace.new_container_code.Substring(19, 22), replacemodel.new_item_code))
            {
                throw new Exception("替换入库烟包物料与原物料不一致,请扫描正确的烟包");
            }
            if (!FinishItemReplace(itemreplace.t_id, itemreplace.new_container_code)) {
                throw new Exception("更新烟包替换状态为已完成失败");
            }

            Container container = new Container();
            ContainerDModel oldCtndm = container.getContainerDetailById(itemreplace.container_d_id);

            ContainerModel ctnM = new ContainerModel();
            ctnM.t_id = SnowflakeID.Guid().ToString();
            ctnM.client_id = oldCtndm.client_id;
            ctnM.warehouse_id = oldCtndm.warehouse_id;
            ctnM.org_id = oldCtndm.org_id;
            Location loc = new Location();
            DataRow dr= loc.GetInboundBufferLocation();
            ctnM.location_id = dr["t_id"].ToString();
            ctnM.container_barcode = itemreplace.new_container_code;
            ctnM.is_full = "1";
            ctnM.is_in = "0";
            ctnM.container_type = ContainerType.ContainerType_Actual.ToString();
            ctnM.first_in_date = DateTime.Now;
            ctnM.is_in = "0";
            ctnM.created_by = Context.UserInfo.UserId;
            ctnM.create_date = DateTime.Now;
            ctnM.modify_ip = Context.UserInfo.LoginIP;
            ctnM.modify_date = DateTime.Now;
            ctnM.act_status = "1";
            container.AddContainerH(ctnM);
            ContainerDModel ctndM = new ContainerDModel();
            ctndM.t_id = SnowflakeID.Guid().ToString();
            ctndM.client_id = oldCtndm.client_id;
            ctndM.warehouse_id = oldCtndm.warehouse_id;
            ctndM.org_id = oldCtndm.org_id;
            ctndM.container_id = ctnM.t_id;
            ctndM.inbound_h_id = oldCtndm.inbound_h_id;
            ctndM.inbound_d_id = oldCtndm.inbound_d_id;
            ctndM.item_id = oldCtndm.item_id;
            ctndM.item_name = oldCtndm.item_name;
            ctndM.lot_no = oldCtndm.lot_no;
            ctndM.owner_lot_no = oldCtndm.owner_lot_no;
            ctndM.wms_lot_no = oldCtndm.wms_lot_no;
            ctndM.lot_seq = oldCtndm.lot_seq;
            ctndM.exist_barcode = oldCtndm.exist_barcode;
            ctndM.qc_status = QcStatus.QcStatus_CheckPassed.ToString() ;
            ctndM.qty = oldCtndm.qty;
            ctndM.stat_qty = oldCtndm.stat_qty;
            ctndM.spec = oldCtndm.spec;
            ctndM.uom_qty_id = oldCtndm.uom_qty_id;
            ctndM.uom_stat_id = oldCtndm.uom_stat_id;
            ctndM.ratio = oldCtndm.ratio;
            ctndM.first_in_date = DateTime.Now;
            ctndM.create_date = DateTime.Now;
            ctndM.created_by = Context.UserInfo.UserId;
            ctndM.modify_ip = Context.UserInfo.LoginIP;
            ctndM.modify_date = DateTime.Now;
            ctndM.act_status = "1";
            container.AddContainerD(ctndM);
           
            #region 处理SN相关记录
            container.AddSNIntoContainer(ctnM, ctndM, ctnM.container_barcode, 1, "", "烟包替换入库", "", replacemodel.replacement_no);
            #endregion
            ContainerComponent re = new ContainerComponent();
            re.container = ctnM;
            List < ContainerDModel > ctndetails= new List<ContainerDModel>();
            ctndetails.Add(ctndM);
            re.containerdetails = ctndetails;
            return re;
        }
        /// <summary>
        /// 检查条码在库存中是否存在
        /// </summary>
        /// <param name="containerno">烟包条码</param>
        /// <returns></returns>
        private bool CheckBarcodeIsIn(string containerno) {
            string sql = @"select count(*) from &DataSource&wms_b_container_h h 
                            where h.container_barcode=@container_barcode 
                            and h.act_status=1";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("container_barcode", containerno);
            return int.Parse(DBHelper.SelectOne(sql, lstParams))>0;
        }
        /// <summary>
        /// 将烟包替换入库记录状态更新为已完成
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newcontainercode"></param>
        /// <returns></returns>
        private bool FinishItemReplace(string id,string newcontainercode) {
            string sql = @"update &DataSource&wms_d_item_replacement r
                            set r.replacement_status=@replacement_status,
                                   r.resendbarcode=@resendbarcode,
                                   r.modify_date=sysdate,
                                   r.modify_ip=@modify_ip,
                                   r.last_modified_by=@last_modified_by
                            where r.t_id=@t_id";
            Dictionary<string,object> lstParams=new Dictionary<string, object>();
            lstParams.Add("replacement_status", ReplacementStatus.ReplacementStatus_Finished.ToString());
            lstParams.Add("resendbarcode", newcontainercode);
            lstParams.Add("modify_ip", Context.UserInfo.LoginIP);
            lstParams.Add("last_modified_by", Context.UserInfo.UserId);
            lstParams.Add("t_id", id);
           return DBHelper.RunSql(sql, lstParams) ==1;
        }
    }
}
