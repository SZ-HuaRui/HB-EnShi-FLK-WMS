using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Services.WMS.Base;

namespace NtiCloud.Service.WMS.Tasking
{
    public class WorkTaskItem : WmsObject, IWorkTaskItem
    {
        public DataRow Get(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task_item  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id 
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询任务物料表");

            return dt.Rows[0];
        }

        public DataRow[] GetByTaskID(string taskID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_id", taskID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_task_item  
 where /*client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and*/ warehouse_id = @warehouse 
   and task_id = @task_id 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务物料表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetByRefDocID(string refDocType, string refDocID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_type", refDocType },
                { "ref_doc_id", refDocID }
            };
            string sql;
            sql = @"
select ti.*, 
       t.task_no, t.dependent_task_no
  from &DataSource&wms_b_task_item ti
  left join &DataSource&wms_b_task t on t.t_id = ti.task_id
 where ti.client_id='&CLIENT_ID&' and ti.org_id='&ORG_ID&' 
   and ti.warehouse_id=@warehouse 
   and ti.ref_doc_type = @ref_doc_type 
   and ti.ref_doc_id = @ref_doc_id 
   and ti.act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetByOrderNoOrID(string orderNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ref_doc_type", DocType.DocType_Wave.ToString() },
                { "order_id", orderNoOrID }
            };
            string sql;
            sql = @"
select ti.*, wh.t_id wave_h_id, wd.t_id wave_d_id, oh.t_id outbound_h_id, od.t_id outbound_d_id, 
       t.task_no, t.dependent_task_no
  from &DataSource&wms_b_task_item ti
  left join &DataSource&wms_b_wave_h wh on ti.ref_doc_id = wh.t_id
  left join &DataSource&wms_b_wave_d wd on ti.ref_doc_d_id = wd.t_id
  left join &DataSource&wms_d_outbound_h oh on wd.outbound_h_id = oh.t_id
  left join &DataSource&wms_d_outbound_d od on wd.outbound_d_id = od.t_id
  left join &DataSource&wms_b_task t on t.t_id = ti.task_id
 where ti.client_id='&CLIENT_ID&' and ti.org_id='&ORG_ID&' 
   and ti.warehouse_id=@warehouse 
   and ti.ref_doc_type = @ref_doc_type 
   and oh.t_id = @order_id 
   and ti.act_status = '1'
   and t.act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询任务表", false);

            return dt.AsEnumerable().ToArray();
        }

        public IWorkTaskItem InsertFinishedTaskItem(WorkTaskItemModel model)

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
                { "task_id", model.TaskID },
                { "item_id", model.ItemID },
                { "item_name", model.ItemName },
                { "plan_qty", model.PlanQty.ToString() },
                { "processed_qty", model.ProcessedQty.ToString() },
                { "finished_qty", model.FinishedQty.ToString() },
                { "uom_qty_id", model.UomQtyID },
                { "finished_stat_qty", model.FinishedStatQty.ToString() },
                { "uom_stat_id", model.UomStatID },
                { "container_d_id", model.ContainerDetailID },
                { "ref_doc_type", model.RefDocType },
                { "ref_doc_id", model.RefDocID },
                { "ref_doc_d_id", model.RefDocDetailID },
                { "ref_doc_no", model.RefDocNo },
                { "start_date", model.StartDate.ToString() },
                { "end_date", model.EndDate.ToString() },
                { "process_status", model.ProcessStatus },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_TASK_ITEM_HIS ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       task_id, " +
                "       item_id, " +
                "       item_name, " +
                "       plan_qty, " +
                "       processed_qty, " +
                "       finished_qty, " +
                "       uom_qty_id, " +
                "       finished_stat_qty, " +
                "       uom_stat_id, " +
                "       container_d_id, " +
                "       ref_doc_type, " +
                "       ref_doc_id, " +
                "       ref_doc_d_id, " +
                "       ref_doc_no, " +
                "       start_date, " +
                "       end_date, " +
                "       process_status, " +
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
                "       @task_id, " +
                "       @item_id, " +
                "       @item_name, " +
                "       @plan_qty, " +
                "       @processed_qty, " +
                "       @finished_qty, " +
                "       @uom_qty_id, " +
                "       @finished_stat_qty, " +
                "       @uom_stat_id, " +
                "       @container_d_id, " +
                "       @ref_doc_type, " +
                "       @ref_doc_id, " +
                "       @ref_doc_d_id, " +
                "       @ref_doc_no, " +
                "       date_format(@start_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@end_date, '%Y-%m-%d %H:%i:%s'), " +
                "       @process_status, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "任务物料历史表");

            return this;
        }
        public IWorkTaskItem Insert(WorkTaskItemModel model)
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
                { "task_id", model.TaskID },
                { "item_id", model.ItemID },
                { "item_name", model.ItemName },
                { "plan_qty", model.PlanQty.ToString() },
                { "processed_qty", model.ProcessedQty.ToString() },
                { "finished_qty", model.FinishedQty.ToString() },
                { "uom_qty_id", model.UomQtyID },
                { "finished_stat_qty", model.FinishedStatQty.ToString() },
                { "uom_stat_id", model.UomStatID },
                { "container_d_id", model.ContainerDetailID },
                { "ref_doc_type", model.RefDocType },
                { "ref_doc_id", model.RefDocID },
                { "ref_doc_d_id", model.RefDocDetailID },
                { "ref_doc_no", model.RefDocNo },
                { "start_date", model.StartDate.ToString() },
                { "end_date", model.EndDate.ToString() },
                { "process_status", model.ProcessStatus },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_TASK_ITEM ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       task_id, " +
                "       item_id, " +
                "       item_name, " +
                "       plan_qty, " +
                "       processed_qty, " +
                "       finished_qty, " +
                "       uom_qty_id, " +
                "       finished_stat_qty, " +
                "       uom_stat_id, " +
                "       container_d_id, " +
                "       ref_doc_type, " +
                "       ref_doc_id, " +
                "       ref_doc_d_id, " +
                "       ref_doc_no, " +
                "       start_date, " +
                "       end_date, " +
                "       process_status, " +
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
                "       @task_id, " +
                "       @item_id, " +
                "       @item_name, " +
                "       @plan_qty, " +
                "       @processed_qty, " +
                "       @finished_qty, " +
                "       @uom_qty_id, " +
                "       @finished_stat_qty, " +
                "       @uom_stat_id, " +
                "       @container_d_id, " +
                "       @ref_doc_type, " +
                "       @ref_doc_id, " +
                "       @ref_doc_d_id, " +
                "       @ref_doc_no, " +
                "       date_format(@start_date, '%Y-%m-%d %H:%i:%s'), " +
                "       date_format(@end_date, '%Y-%m-%d %H:%i:%s'), " +
                "       @process_status, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "任务物料表");

            return this;
        }

        public void Insert(DataRow row)
        {
            Insert(new WorkTaskItemModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                TaskID = row["task_id"].ToString(),
                ItemID = row["item_id"].ToString(),
                ItemName = row["item_name"].ToString(),
                PlanQty = DBHelper.GetDecimal(row["plan_qty"]),
                ProcessedQty = DBHelper.GetDecimal(row["processed_qty"]),
                FinishedQty = DBHelper.GetDecimal(row["finished_qty"]),
                UomQtyID = row["uom_qty_id"].ToString(),
                FinishedStatQty = DBHelper.GetDecimal(row["finished_stat_qty"]),
                UomStatID = row["uom_stat_id"].ToString(),
                ContainerDetailID = row["container_d_id"].ToString(),
                RefDocType = row["ref_doc_type"].ToString(),
                RefDocID = row["ref_doc_id"].ToString(),
                RefDocDetailID = row["ref_doc_d_id"].ToString(),
                RefDocNo = row["ref_doc_no"].ToString(),
                StartDate = DBHelper.GetDateTime(row["start_date"]),
                EndDate = DBHelper.GetDateTime(row["end_date"]),
                ProcessStatus = row["process_status"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

        }
        public void UpdateProcessStatus(string id, string processStatus, string oldProcessStatus = null)
        {
            if (oldProcessStatus == null)
            {
                oldProcessStatus = Get(id)["process_status"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_modified_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "process_status", processStatus},
                { "old_process_status", oldProcessStatus},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task_item  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       process_status = @process_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (process_status = @old_process_status
            or process_status is null)
";
            DBHelper.RunSql(sql, param, "更新工作状态");
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
                { "act_status", actStatus},
                { "old_act_status", oldActStatus},
                { "t_id", id }
            };
            string sql = @"
update &DataSource&wms_b_task_item  
   set modify_date = now(), 
       last_modified_by = @last_modified_by, 
       modify_ip = @modify_ip,
       act_status = @act_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id
   and (act_status = @old_act_status
            or @old_act_status = ' ')
";
            DBHelper.RunSql(sql, param, "更新状态");
        }

        public IWorkTaskItem AddWaveDetailProcessQty(string taskID, decimal sign)
        {
            return AddWaveDetailProcessQty(WorkTask.Get(taskID), sign);
        }
        public IWorkTaskItem AddWaveDetailProcessQty(DataRow taskRow, decimal sign)
        {
            DataRow[] taskItemRows = WorkTaskItem.GetByTaskID(taskRow[t.ID].ToString());
            foreach (DataRow row in taskItemRows)
            {
                Wave.AddDetailProcessedQty(row[ti.RefDocDetailID].ToString(),
                    sign * decimal.Parse("0" + row[ti.PlanQty]));
            }
            return this;
        }
    }
}
