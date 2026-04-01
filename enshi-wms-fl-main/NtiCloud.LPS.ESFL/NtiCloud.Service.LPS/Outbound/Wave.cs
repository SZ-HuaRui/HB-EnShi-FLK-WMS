using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using NtiCloud.Kernel;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Outbound
{
    public class Wave : WmsObject, IWave
    {

        #region DAL

        public IWave AddDetailFinishedQty(string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetWaveDetail(detailID)["finished_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "t_id", detailID},
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql = @"
update &DataSource&wms_b_wave_d set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, finished_qty = finished_qty + @qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and t_id = @t_id and finished_qty = @old_qty";
            DBHelper.RunSql(sql, param, "计算波次明细表已处理量");

            return this;
        }

        public IWave AddDetailProcessedQty(string detailID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetWaveDetail(detailID)["processed_qty"]);
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "t_id", detailID},
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql = @"
update &DataSource&wms_b_wave_d set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, processed_qty = processed_qty + @qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and t_id = @t_id and processed_qty = @old_qty";
            DBHelper.RunSql(sql, param, "计算波次明细表已处理量");

            return this;
        }

        public void DeleteWaveDetails(string detailIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", detailIds },
            };
            string sql = @"
delete from &DataSource&wms_b_wave_d 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse 
   and instr(@ids, t_id) > 0 
";
            DBHelper.RunSql(sql, param, "删除波次明细", DBHelper.OperatedNumType.OneOrMore);
        }

        public void DeleteWaveHead(string headNoOrId)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "head_id", headNoOrId },
            };
            string sql = @"
delete from &DataSource&wms_b_wave_h 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse 
   and (t_id = @head_id or wave_no = @head_id) 
";
            DBHelper.RunSql(sql, param, "删除波次头");
        }

        public void DeleteWaveResult(string headID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "id", headID },
            };
            string sql = @"
delete from isa_t_wave_result 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse 
   and instr(@id, wave_h_id) > 0 
";
            DBHelper.RunSql(sql, param, "删除波次预分配临时结果", DBHelper.OperatedNumType.ZeroOrMore);
        }

        public IWave DeleteWaveTasks(string taskIds)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", taskIds },
            };
            string sql = @"
delete from isa_m_wave_task 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
    and warehouse_id = @warehouse 
    and instr(@ids, t_id) > 0 
";
            DBHelper.RunSql(sql, param, "删除波次任务");

            return this;
        }

        public DataRow GetDependentTask(string depententTaskNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_no", depententTaskNo },
            };
            string selSql = @"
select * from isa_m_wave_task 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
    and warehouse_id =@warehouse 
    and dependence_task_no = @task_no 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "查找依赖的任务", false);
            if (dt.Rows.Count == 0) return null;
            return dt.Rows[0];
        }

        public DataRow[] GetOrderDetailsByOrders(string orderHeadIds)
        {
            // 获取出库单明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", orderHeadIds },
            };
            string selSql = @"
select od.t_id,
       od.client_id,
       od.org_id,
       od.warehouse_id,
       od.outbound_h_id outbound_h_id,
       od.seq_no order_item_seq,
       od.item_id,
       od.item_name,
       od.seq_no,
       0 order_item_seq,
       od.plan_qty,
       0 processed_qty,
       od.finished_qty,
       od.uom_qty_id,
       od.plan_stat_qty,
       od.processed_stat_qty,
       od.finished_stat_qty,
       od.uom_stat_id,
       ' ' batch_no,
       ' ' company,
       ' ' work_created,
       ' ' memo,
       od.create_date,
       od.created_by,
       od.modify_ip,
       od.modify_date,
       od.last_modified_by,
       od.act_status,
       oh.seq_no order_seq,
       oh.doc_no order_no,
       ' ' bom_code,
       ' ' consolidated
  from &DataSource&wms_d_outbound_d od
  left join &DataSource&wms_d_outbound_h oh
    on od.outbound_h_id = oh.t_id
 where od.client_id='&CLIENT_ID&' and od.org_id='&ORG_ID&'
   and od.warehouse_id = @warehouse
   and instr(@ids, od.outbound_h_id) > 0
   and od.act_status = '1'
 order by oh.seq_no, oh.doc_no, od.seq_no
";
            DataTable dt = DBHelper.Select(selSql, param);
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetOrderDetailsNotInWave(string orderHeadIds)
        {
            // 获取出库单明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", orderHeadIds },
            };
            string selSql = @"
select od.t_id,
       od.client_id,
       od.org_id,
       od.warehouse_id,
       od.outbound_h_id outbound_h_id,
       0 order_item_seq,
       od.item_id,
       od.item_name,
       od.plan_qty - nvl(wd.plan_qty, 0) plan_qty,
       0 processed_qty,
       od.finished_qty,
       od.uom_qty_id,
       ' ' batch_no,
       ' ' company,
       ' ' work_created,
       ' ' memo,
       od.create_date,
       od.created_by,
       od.modify_ip,
       od.modify_date,
       od.last_modified_by,
       od.act_status,
       oh.SEQ_NO order_seq,
       oh.doc_no order_no,
       ' ' bom_code,
       ' ' consolidated
  from &DataSource&wms_d_outbound_d od
  left join &DataSource&wms_d_outbound_h oh
    on od.outbound_h_id = oh.t_id
  left join(select outbound_d_id, sum(plan_qty) plan_qty
               from &DataSource&wms_b_wave_d
              group by outbound_d_id) wd
    on od.t_id = wd.outbound_d_id
 where od.client_id='&CLIENT_ID&' and od.org_id='&ORG_ID&'
   and od.warehouse_id = @warehouse
   and od.plan_qty > ifnull(wd.plan_qty, 0)
   and instr(@ids, od.outbound_h_id) > 0
   and od.act_status = '1'
 order by oh.seq_no, oh.doc_no--, od.order_item_seq
";
            DataTable dt = DBHelper.Select(selSql, param);
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow GetWaveDetail(string detailID)
        {
            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
            };
            string selSql = @"
select wd.*, i.item_name, wh.wave_status from &DataSource&wms_b_wave_d wd 
    left join &DataSource&wms_b_wave_h wh on wd.wave_h_id = wh.t_id 
    left join &DataSource&wms_m_item i on wd.item_id = i.t_id 
 where wd.client_id='&CLIENT_ID&' and wd.org_id='&ORG_ID&'
   and wd.warehouse_id = @warehouse
   and wd.t_id = @t_id
   and wd.act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "查询波次明细表");
            return dt.Rows[0];
        }

        public DataRow[] GetWaveDetails(string detailIds)
        {
            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", detailIds },
            };
            string selSql = @"
select wd.*, wh.wave_status from &DataSource&wms_b_wave_d wd 
    left join &DataSource&wms_b_wave_h wh on wd.wave_h_id = wh.t_id 
 where wd.client_id='&CLIENT_ID&' and wd.org_id='&ORG_ID&'
   and wd.warehouse_id = @warehouse
   and instr(@ids, wd.t_id) > 0
   and wd.act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "查找波次明细");
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetWaveDetailsByHeadID(string headID)
        {
            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "head_id", headID },
            };
            /*
            string selSql = @"
select wd.*, i.item_name, wh.wave_status from &DataSource&wms_b_wave_d wd 
    left join &DataSource&wms_b_wave_h wh on wd.wave_h_id = wh.t_id 
    left join &DataSource&wms_m_item i on wd.item_id = i.t_id 
 where wd.client_id='&CLIENT_ID&' and wd.org_id='&ORG_ID&'
   and wd.warehouse_id = @warehouse
   and wd.wave_h_id = @head_id order by wd.order_seq, wd.group_seq, wd.detail_seq
";
            */
            string selSql = @"
select wd.*, i.item_name, wh.wave_status, od.lot_no, od.batches
  from &DataSource&wms_b_wave_d wd
  left join &DataSource&wms_b_wave_h wh on wd.wave_h_id = wh.t_id
  left join &DataSource&wms_m_item i on wd.item_id = i.t_id
  left join &DataSource&wms_d_outbound_d od on wd.outbound_d_id = od.t_id
  left join (select item_id, sum(plan_qty) qty from &DataSource&wms_b_wave_d 
              where wave_h_id = @head_id group by item_id) t
         on wd.item_id = t.item_id
 where wd.client_id='&CLIENT_ID&' and wd.org_id='&ORG_ID&'
   and wd.warehouse_id = @warehouse
   and wd.act_status = '1'
   and wd.wave_h_id = @head_id 
 order by wd.order_seq, wd.group_seq, to_number(wd.detail_seq)
";
            DataTable dt = DBHelper.Select(selSql, param, "查找波次明细");
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetDetailsByItemID(string itemID, bool errorWhenNotFound = true)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "item_id", itemID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_wave_d  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and outbound_h_id = @item_id 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询波次明细表", errorWhenNotFound);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetDetailsByOrderID(string outboundHeadID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "outbound_h_id", outboundHeadID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_wave_d  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and outbound_h_id = @outbound_h_id 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询波次明细表");

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetDetailsByOrderDetailID(string outboundDetailID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "outbound_d_id", outboundDetailID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_wave_d  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and outbound_d_id = @outbound_d_id 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询波次明细表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetWaveHead(string waveNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", waveNoOrID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_wave_h  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and (t_id = @t_id or wave_no = @t_id)
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询波次头表");

            return dt.Rows[0];
        }


        public DataRow GetWaveTask(string waveTaskNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "no_id", waveTaskNoOrID }
            };
            string selSql = $@"
select * from isa_m_wave_task 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
    and warehouse_id =@warehouse 
    and (task_no = @no_id or t_id = @no_id) 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "查找波次任务表");
            return dt.Rows[0];
        }

        public DataRow[] GetWaveTasksByHeaadID(string waveHeadID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "head_id", waveHeadID }
            };
            string selSql = $@"
select * from isa_m_wave_task 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
    and warehouse_id =@warehouse 
    and wave_h_id = @head_id 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "查询波次任务");
            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetHeadByOrderID(string outboundHeadID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "outbound_h_id", outboundHeadID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_b_wave_h  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = (
        select distinct wave_h_id from &DataSource&wms_b_wave_d  
         where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
           and warehouse_id = @warehouse 
           and outbound_h_id = @outbound_h_id 
           and act_status = '1')
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(sql, param, "查询波次头表");

            return dt.Rows[0];
        }

        /// <summary>
        /// 取波次任务记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DataRow[] GetWaveTasks(string ids)
        {
            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "ids", ids },
            };
            string selSql = @"
select * from isa_m_wave_task wt
 where wt.client_id='&CLIENT_ID&' and wt.org_id='&ORG_ID&'
    and warehouse_id =@warehouse 
    and instr(@ids, t_id) > 0 
   and act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "波次任务表");
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        /// <summary>
        /// 根据波次头表ID取波次任务记录
        /// </summary>
        /// <param name="headID"></param>
        /// <returns></returns>
        public DataRow[] GetWaveTasksByHeadID(string headID)
        {
            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "head_id", headID },
            };
            string selSql = @"
select wt.*, we.eq_code equipment_number from isa_m_wave_task wt 
  left join isa_bi_equipment we on wt.equipment_id = we.t_id 
 where wt.client_id='&CLIENT_ID&' and wt.org_id='&ORG_ID&'
   and wt.warehouse_id = @warehouse 
   and wt.wave_h_id = @head_id order by wt.group_no, wt.seq_no
   and wt.act_status = '1'
";
            DataTable dt = DBHelper.Select(selSql, param, "波次任务表");
            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public IWave Insert(WaveDetailModel model)
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
                { "wave_h_id", model.WaveHeadID },
                { "outbound_h_id", model.OutboundHeadID },
                { "outbound_d_id", model.OutboundDetailID },
                { "item_id", model.ItemID },
                { "plan_qty", model.PlanQty.ToString() },
                { "processed_qty", model.ProcessedQty.ToString() },
                { "finished_qty", model.FinishedQty.ToString() },
                { "uom_qty_id", model.UomQtyID },
                { "plan_stat_qty", model.PlanStatQty.ToString() },
                { "processed_stat_qty", model.ProcessedStatQty.ToString() },
                { "finished_stat_qty", model.FinishedStatQty.ToString() },
                { "uom_stat_id", model.UomStatID },
                { "order_seq", model.OrderSeq },
                { "group_seq", model.GroupSeq },
                { "detail_seq", model.DetailSeq },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into &DataSource&WMS_B_WAVE_D ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       wave_h_id, " +
                "       outbound_h_id, " +
                "       outbound_d_id, " +
                "       item_id, " +
                "       plan_qty, " +
                "       processed_qty, " +
                "       finished_qty, " +
                "       uom_qty_id, " +
                "       plan_stat_qty, " +
                "       processed_stat_qty, " +
                "       finished_stat_qty, " +
                "       uom_stat_id, " +
                "       order_seq, " +
                "       group_seq, " +
                "       detail_seq, " +
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
                "       @wave_h_id, " +
                "       @outbound_h_id, " +
                "       @outbound_d_id, " +
                "       @item_id, " +
                "       @plan_qty, " +
                "       @processed_qty, " +
                "       @finished_qty, " +
                "       @uom_qty_id, " +
                "       @plan_stat_qty, " +
                "       @processed_stat_qty, " +
                "       @finished_stat_qty, " +
                "       @uom_stat_id, " +
                "       @order_seq, " +
                "       @group_seq, " +
                "       @detail_seq, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "波次明细表");

            return this;
        }

        public IWave Insert(DataRow row)
        {
            Insert(new WaveDetailModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                WaveHeadID = row["wave_h_id"].ToString(),
                OutboundHeadID = row["outbound_h_id"].ToString(),
                OutboundDetailID = row["outbound_d_id"].ToString(),
                ItemID = row["item_id"].ToString(),
                PlanQty = DBHelper.GetDecimal(row["plan_qty"]),
                ProcessedQty = DBHelper.GetDecimal(row["processed_qty"]),
                FinishedQty = DBHelper.GetDecimal(row["finished_qty"]),
                UomQtyID = row["uom_qty_id"].ToString(),
                PlanStatQty = DBHelper.GetDecimal(row["plan_stat_qty"]),
                ProcessedStatQty = DBHelper.GetDecimal(row["processed_stat_qty"]),
                FinishedStatQty = DBHelper.GetDecimal(row["finished_stat_qty"]),
                UomStatID = row["uom_stat_id"].ToString(),
                OrderSeq = row["order_seq"].ToString(),
                GroupSeq = row["group_seq"].ToString(),
                DetailSeq = row["detail_seq"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

            return this;
        }

        public void InsertWaveHeadByDetail(string waveHeadID, string waveNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "head_id", waveHeadID },
                { "wave_no", waveNo },
            };
            string insSql = @"
insert into &DataSource&wms_b_wave_h
  (t_id,
   client_id,
   org_id,
   warehouse_id,
   wave_no,
   released_by,
   release_date,
   wave_status,
   wave_type,
   order_qty,
   left_task_qty,
   create_date,
   created_by,
   modify_ip,
   modify_date,
   last_modified_by,
   act_status)
  select @head_id,
         '&CLIENT_ID&',
         '&ORG_ID&',
         wd.warehouse_id,
         @wave_no,
         null,
         null,
         'WaveStatus_New',
         'WaveType_Normal',
         sum(plan_qty),
         0,
         now(),
         @username,
         @login_ip,
         now(),
         @username,
         '1'
    from &DataSource&wms_b_wave_d wd
   where wd.client_id='&CLIENT_ID&' and wd.org_id='&ORG_ID&'
     and wd.wave_h_id = @head_id
     and act_status = '1'
   group by wd.wave_h_id, wd.warehouse_id
";
            DBHelper.RunSql(insSql, param, "添加波次头");
        }

        public IWave UpdateWaveType(string headNoOrID, string waveType, string oldType = null)
        {
            if (string.IsNullOrWhiteSpace(oldType))
            {
                oldType = " ";
            }
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "warehouse", Context.WarehouseID },
                { "id_no", headNoOrID },
                { "wave_type", waveType },
                { "old_type", oldType },
            };

            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip = @login_ip,
       last_modified_by = @username, wave_type = @wave_type
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and (t_id = @id_no or wave_no = @id_no) 
   and (wave_type = @old_type or @old_type = ' ')
";
            DBHelper.RunSql(sql, param, "更新波次类型");

            return this;
        }

        public void AddWaveDetailByOrderDetail(
            string waveHeadID, string groupSeq, DataRow orderDetailRow)
        {
            DataRow[] waveDetailRows = GetDetailsByOrderDetailID(orderDetailRow["t_id"].ToString());

            decimal qtyInOtherWave = waveDetailRows
                        .Where(r => r["wave_h_id"].ToString() != waveHeadID)
                        .Sum(r => decimal.Parse("0" + r["plan_qty"]));


            UomModel qtyUomModel = new UomModel()
            {
                Qty = decimal.Parse("0" + orderDetailRow["plan_qty"])
                        - decimal.Parse("0" + orderDetailRow[od.ProcessedQty]),
                UomID = orderDetailRow["uom_qty_id"].ToString()
            };
            if (qtyUomModel.Qty <= 0)
            {
                return;
            }
            qtyUomModel = ItemUom.ConvertToDefaultPackUom(qtyUomModel);
            //if (qtyUomModel.Qty <= qtyInOtherWave)
            //{
            //    return;
            //}
            UomModel statUomModel = ItemUom.ConvertToDefaultStatUom(new UomModel()
            {
                Qty = decimal.Parse("0" + orderDetailRow["plan_stat_qty"])
                        - decimal.Parse("0" + orderDetailRow[od.ProcessedStatQty]),
                UomID = orderDetailRow["uom_stat_id"].ToString()
            });

            Insert(new WaveDetailModel()
            {
                WaveHeadID = waveHeadID,
                OutboundHeadID = orderDetailRow["outbound_h_id"].ToString(),
                OutboundDetailID = orderDetailRow["t_id"].ToString(),
                ItemID = orderDetailRow["item_id"].ToString(),
                OrderSeq = orderDetailRow["order_seq"].ToString(),
                GroupSeq = groupSeq,
                DetailSeq = orderDetailRow["order_item_seq"].ToString(),
                PlanQty = qtyUomModel.Qty,
                ProcessedQty = 0,
                FinishedQty = 0,
                UomQtyID = qtyUomModel.UomID,
                PlanStatQty = statUomModel.Qty,
                UomStatID = statUomModel.UomID,
            });
        }

        public void InsertWaveDetailByOrderDetail(string waveHeadID,
            string groupSeq, string detailSeq, string orderDetailID, decimal qty)
        {
            var t_id = SnowflakeID.Guid().ToString();

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "head_id", waveHeadID },
                { "detail_id", orderDetailID },
                { "t_id", t_id },
                { "group_seq", groupSeq },
                { "detail_seq", detailSeq },
                { "qty", qty.ToString() },
            };
            string insSql = @"
insert into &DataSource&wms_b_wave_d
  (t_id,
   client_id,
   org_id,
   warehouse_id,
   wave_h_id,
   outbound_h_id,
   outbound_d_id,
   item_id,
   order_seq,
   group_seq,
   detail_seq,
   plan_qty,
   processed_qty,
   finished_qty,
   create_date,
   created_by,
   modify_ip,
   modify_date,
   last_modified_by,
   act_status)
  select @t_id,
         '&CLIENT_ID&',
         '&ORG_ID&',
         od.warehouse_id,
         @head_id,
         od.outbound_h_id,
         od.t_id,
         od.item_id,
         oh.seq_no,
         @group_seq,
         @detail_seq,
         case
           when to_number(@qty) <=
                (od.plan_qty - ifnull(wd.qty_in_wave, 0)) then
            to_number(@qty)
           else
            (od.plan_qty - ifnull(wd.qty_in_wave, 0))
         end,
         0,
         0,
         now(),
         @username,
         @login_ip,
         now(),
         @username,
         '1'
    from &DataSource&wms_d_outbound_d od
    left
    join (select outbound_d_id, sum(plan_qty) qty_in_wave
            from &DataSource&wms_b_wave_d t
                where t.wave_h_id<> @head_id
                  and t.act_status = '1'
                group by outbound_d_id) wd
      on od.t_id = wd.outbound_d_id
    left join &DataSource&wms_d_outbound_h oh
      on od.outbound_h_id = oh.t_id
   where od.client_id='&CLIENT_ID&' and od.org_id='&ORG_ID&'
     and od.warehouse_id=@warehouse
     and od.plan_qty > ifnull(wd.qty_in_wave, 0)
     and od.t_id = @detail_id
     and od.act_status = '1'
   order by od.t_id 
";
            DBHelper.RunSql(insSql, param);
        }

        public IWave UpdateDetailProcessedQty(string detailID,
                    decimal processedQty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetWaveDetail(detailID)["processed_qty"]);
            }
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "warehouse", Context.WarehouseID },
                { "t_id", detailID },
                { "qty", processedQty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql = @"
update &DataSource&wms_b_wave_d set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, processed_qty = @qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse
   and t_id = @t_id and processed_qty = @old_qty
";
            DBHelper.RunSql(sql, param, "更新波次明细处理量");
            return this;
        }

        public IWave UpdateHeadLeftTaskQty(string headNoOrID, decimal leftTaskQty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "head_id", headNoOrID },
                { "qty", leftTaskQty.ToString() },
            };
            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, left_task_qty = @qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
     and (t_id = @head_id or wave_no = @head_id)
";
            DBHelper.RunSql(sql, param, "更新波次头表剩余任务数");

            return this;
        }

        public IWave ResetHeadReleaseInfo(string headNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "head_id", headNoOrID },
            };
            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, 
       released_by = null, release_date = null
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
     and (t_id = @head_id or wave_no = @head_id) 
";
            DBHelper.RunSql(sql, param, "重置波次信息");

            return this;
        }

        public IWave UpdateHeadReleaseInfo(string headNoOrID,
            string releasedBy, string releaseDate)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "head_id", headNoOrID },
                { "released_by", releasedBy },
                { "release_date", releaseDate },
            };
            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, 
       released_by = @released_by, release_date = date_format(@release_date, '%Y/%m/%d %H:%i:%s') 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
     and (t_id = @head_id or wave_no = @head_id) and released_by is null 
";
            DBHelper.RunSql(sql, param, "波次头表", DBHelper.OperatedNumType.ZeroOrMore);
            // 没更新到不算错, 发哥要求保留第一次释放人和时间

            return this;
        }

        public IWave UpdateWaveTaskContainer(string taskID, string containerID,
            string containerBarcode, string oldContainerID = null)
        {
            if (oldContainerID == null)
            {
                oldContainerID = GetWaveTask(taskID)["container_id"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "task_id", taskID },
                { "container_barcode", containerBarcode },
                { "container_id", containerID },
                { "old_container_id", oldContainerID },
            };
            string sql = @"
update isa_m_wave_task 
   set modify_date = now(), modify_ip = @login_ip, last_modified_by = @username, 
       container_id = @container_id, container_barcode = @container_barcode
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id =@warehouse 
   and t_id = @task_id
   and container_id = @old_container_id
";
            DBHelper.RunSql(sql, param, "修改波次任务");

            return this;
        }

        public IWave UpdateWaveTaskLocation(string taskID, string locationID, string oldLocationID = null)
        {
            if (oldLocationID == null)
            {
                oldLocationID = GetWaveTask(taskID)["location_id"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "task_id", taskID },
                { "location_id", locationID },
                { "old_location_id", oldLocationID },
            };
            string sql = @"
update isa_m_wave_task 
   set modify_date = now(), modify_ip = @login_ip, last_modified_by = @username, 
       location_id = @location_id
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id =@warehouse 
   and t_id = @task_id
   and location_id = @old_location_id
";
            DBHelper.RunSql(sql, param, "修改波次任务货位");

            return this;
        }

        public IWave UpdateWaveTaskQty(string taskID, decimal operationQty, decimal palletQty)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "task_id", taskID },
                { "operation_qty", operationQty.ToString() },
                { "pallet_qty", palletQty.ToString() },
            };
            string sql = @"
update isa_m_wave_task 
   set modify_date = now(), modify_ip = @login_ip, last_modified_by = @username, 
       operation_qty = @operation_qty, pallet_qty = @pallet_qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id =@warehouse 
   and t_id = @task_id
";
            DBHelper.RunSql(sql, param, "修改波次任务数量");

            return this;
        }


        public IWave UpdateWaveTaskState(string waveTaskNo, string state)
        {
            LogHelper.LogInformation("修改波次任务状态");

            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "task_no", waveTaskNo },
                { "state", state },
                //{ "old_state", oldState },
            };
            string sql = @"
update isa_m_wave_task set wave_status = @state 
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id =@warehouse 
   and task_no = @task_no
";
            DBHelper.RunSql(sql, param, "修改波次任务状态");

            return this;
        }

        public IWave UpdateWaveTaskSource(string waveTaskID, Address addr, string oldSourceID = null)
        {
            if (oldSourceID == null)
            {
                oldSourceID = GetWaveTask(waveTaskID)["source_id"].ToString();
            }

            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "task_id", waveTaskID },
                { "source_id", addr.ID },
                { "source_type", addr.Type },
                { "source_text", addr.Text },
                { "old_source_id", oldSourceID },
                //{ "old_state", oldState },
            };
            string sql = @"
update isa_m_wave_task 
   set modify_date = now(), modify_ip = @login_ip, last_modified_by = @username, 
       source_id = @source_id, source_type = @source_type,
       source_text = @source_text
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id =@warehouse 
   and t_id = @task_id
   and source_id = @old_source_id
";
            DBHelper.RunSql(sql, param, "修改波次任务源地址");

            return this;
        }

        public IWave UpdateWaveTaskTarget(string waveTaskID, Address addr, string oldTargetID = null)
        {
            if (oldTargetID == null)
            {
                oldTargetID = GetWaveTask(waveTaskID)["target_address_id"].ToString();
            }

            // 获取波次明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "task_id", waveTaskID },
                { "target_id", addr.ID },
                { "target_type", addr.Type },
                { "target_text", addr.Text },
                { "old_target_id", oldTargetID },
                //{ "old_state", oldState },
            };
            string sql = @"
update isa_m_wave_task 
   set modify_date = now(), modify_ip = @login_ip, last_modified_by = @username, 
       target_address_id = @target_id, target_address_type = @target_type,
       target_address_text = @target_text
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id =@warehouse 
   and t_id = @task_id
   and target_address_id = @old_target_id
";
            DBHelper.RunSql(sql, param, "修改波次任务目标地址");

            return this;
        }

        #endregion

        public IWave AddHeadLeftTaskQty(string headNoOrID, decimal qty, decimal? oldQty = null)
        {
            if (oldQty == null)
            {
                oldQty = decimal.Parse("0" + GetWaveHead(headNoOrID)["left_task_qty"]);
            }

            LogHelper.LogInformation("计算波次头表剩余任务数");
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "head_id", headNoOrID },
                { "qty", qty.ToString() },
                { "old_qty", oldQty.ToString() },
            };
            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, left_task_qty = ifnull(left_task_qty, 0) + @qty
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
     and (t_id = @head_id or wave_no = @head_id) 
   and left_task_qty = @old_qty 
";
            DBHelper.RunSql(sql, param, "计算波次头表剩余任务数");

            return this;
        }

        public IWave AddWaveTask(string taskNo, string groupNo, string seqNo,
            string priority, string waveDetailID, decimal leftQty, string locationNoOrID)
        {
            if (string.IsNullOrWhiteSpace(taskNo) || string.IsNullOrWhiteSpace(groupNo)
                || string.IsNullOrWhiteSpace(waveDetailID) || string.IsNullOrWhiteSpace(locationNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow waveDtlRow = GetWaveDetail(waveDetailID);

            string groupSeq = string.Empty;
            string lastOrder = string.Empty;

            //if (groupNo == "auto")
            //{
            //    groupNo = WmsHelper.GetSequence("SWMS_GroupNo");
            //}
            groupNo = waveDtlRow["order_seq"].ToString();

            //ILocation location = WmsFactory.MakeLocation();
            DataRow locRow = Location.Get(locationNoOrID);

            //if (locRow["location_state"].ToString() != "LocationState_Stored")
            //{
            //throw new WmsException(-659, "货位没有存储物料");
            //}
            //if (locRow["location_state"].ToString() != "LocationState_Stored")
            //{
            //    throw new WmsException(-659, "货位状态表明该货位没有存储物料或物料不可用");
            //}
            if (locRow["storage_unit_useable_status"].ToString() != "LocationFlag_Usable"
                    && locRow["storage_unit_useable_status"].ToString() != "LocationFlag_OutOnly")
            {
                throw new WmsException(-659, "货位标记表明该货位不能进行出库");
            }

            //Location.ModifyLocationStockState(locRow["t_id"].ToString(),
            //    "LocationState_Out", "LocationState_Stored");

            decimal planQty = decimal.Parse("0" + waveDtlRow["plan_qty"]);
            decimal processedQty = decimal.Parse("0" + waveDtlRow["processed_qty"]);
            if (leftQty > planQty - processedQty)
            {
                throw new WmsException(-659, "数量已超过要出库数量!");
            }

            // 更新波次明细表
            AddDetailProcessedQty(waveDtlRow["t_id"].ToString(), leftQty,
                decimal.Parse("0" + waveDtlRow["processed_qty"].ToString()));

            // 更新订单明细表中的已处理量
            //OutboundOrder.AddDetailProcessedQty(row["order_d_id"].ToString(),
            //    leftQty, decimal.Parse("0" + row["processed_qty"].ToString()));

            // 更新波次头表
            UpdateHeadStatus(waveDtlRow[wd.WaveHeadID].ToString(), WaveStatus.WaveStatus_Editable.ToString());

            DataRow orderDetailRow = OutboundOrder.GetDetail(waveDtlRow[wd.OutboundDetailID].ToString());
            DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
            DataRow[] ctnDetails = Container.Get4StockingOut(new ContainerDetailModel()
            {
                ContainerID = containerRow["m_container_id"].ToString(),
                ItemID = orderDetailRow["item_id"].ToString(),
                IsFull = orderDetailRow["is_full"].ToString(),
                StatQty = decimal.Parse("0" + orderDetailRow["plan_finance_qty"].ToString()),
                InboundHeadID = orderDetailRow["outbound_h_id"].ToString(),
                LotNo = orderDetailRow["lot_no"].ToString(),
                GivenYear = DBHelper.GetDecimal(orderDetailRow["givenyear"]),
            });
            ctnDetails = ctnDetails.OrderBy(r => r["first_inbound_dt"].ToString()).ToArray();

            if (ctnDetails.Length <= 0)
            {
                throw new WmsException(212, "无法在货位上找到适用库存，无法继续处理");
            }

            decimal sumQty = 0;
            foreach (DataRow ctnDtlRow in ctnDetails)
            {
                if (leftQty == 0) break;

                decimal actualQty = decimal.Parse("0" + ctnDtlRow["actual_qty"]);
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);
                decimal availQty = actualQty - outQty - lockQty;
                decimal qty = 0;

                if (availQty == 0)
                {
                    continue;
                }
                if (leftQty > availQty)
                {
                    leftQty -= availQty;
                    qty = availQty;
                }
                else
                {
                    qty = leftQty;
                    leftQty = 0;
                }
                if (qty <= 0)
                {
                    throw new WmsException(211, "货位上库存不足，程序停止处理!");
                }

                sumQty += qty;

                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow["out_qty"]));
                string isFull = ctnDtlRow["is_full"].ToString();
                decimal financeQty = qty * decimal.Parse("0" + ctnDtlRow["speci"].ToString());
                isFull = string.IsNullOrWhiteSpace(isFull) ? "1" : isFull;
                if (isFull == "0")
                {
                    financeQty = decimal.Parse("0" + ctnDtlRow["finance_qty"].ToString());
                }

                //string taskType = TaskType.Outbound;
                string taskType = TaskType.TaskType_OutBound.ToString();
                //string taskType = string.IsNullOrWhiteSpace(target)
                //                    ? TaskType.Outbound : TaskType.StockUp;

                string targetID = "";
                string targetText = "";
                string targetType = "";
                //if (!string.IsNullOrWhiteSpace(target))
                //{
                //    DataRow targetLocRow = Location.Get(target);
                //    targetID = targetLocRow["t_id"].ToString();
                //    targetType = "AddressType_Location";
                //    targetText = targetLocRow["location_name"].ToString();
                //}
                DataRow equipRow = Equipment.GetStackByLocation(locRow["t_id"].ToString());

                string execType = string.Empty;
                execType = GetTaskExecType(taskType);

                decimal palletQty = qty;
                if (decimal.TryParse(containerRow["packing_type"].ToString(), out decimal packingType))
                {
                    if (packingType > 0)
                    {
                        palletQty = decimal.Ceiling(qty / packingType);
                    }
                }

                if (taskNo == "auto")
                {
                    taskNo = WmsHelper.GetSequence("TaskNo");
                }

                //DataRow dockRow = Dock.GetByZone(locRow["isa_bi_storage_zone_id"].ToString());
                InsertWaveTask(new WaveTaskModel()
                {
                    ID = SnowflakeID.Guid().ToString(),
                    WaveHeadID = waveDtlRow["wave_h_id"].ToString(),
                    WaveDetailID = waveDtlRow["t_id"].ToString(),
                    OutboundHeadID = waveDtlRow["outbound_h_id"].ToString(),
                    OutboundDetailID = waveDtlRow["outbound_d_id"].ToString(),
                    TaskTypeCode = taskType,
                    TaskExecTypeCode = execType,
                    TaskNo = taskNo, // WmsHelper.GetSequence("TaskNo"),
                    EquipmentID = equipRow?["t_id"].ToString(),
                    GroupNo = groupNo,
                    SeqNo = waveDtlRow["detail_seq"].ToString(),
                    //Priority = decimal.Parse(WmsConfig.Get("Task:Priority:Outbound")),
                    Priority = decimal.Parse("0" + priority),
                    //ContainerID = locRow["container_id"].ToString(),
                    //ContainerID = inventoryRows[0]["t_id"].ToString(),
                    ContainerID = ctnDtlRow["t_id"].ToString(),
                    ContainerBarcode = containerRow["container_barcode"].ToString(),
                    ItemID = waveDtlRow["item_id"].ToString(),
                    OperationQty = qty,
                    StateCode = "WaveStatus_Editable",
                    LocationID = locRow["t_id"].ToString(),
                    BatchNo = ctnDtlRow["external_lotno"].ToString(),
                    BomCode = "",
                    SourceID = locRow["t_id"].ToString(),
                    SourceType = "AddressType_Location",
                    SourceText = locRow["location_no"].ToString(),
                    TargetID = targetID,
                    TargetType = targetType,
                    TargetText = targetText,
                    Executable = 1,
                    DependenceTaskNo = "",
                    LedText = "",
                    //PalletQty = Container.GetInventoryByLocation(
                    //            locRow["t_id"].ToString(), waveDtlRow["item_id"].ToString()),
                    PalletQty = palletQty,
                });

                taskNo = "auto";
                // 保存波次任务
                //InsertWaveTask(row, "Task_OutBound", locationNoOrID,
                //    groupNo, qty, taskNo, seqNo, priority);
            }
            if (leftQty > 0)
            {
                throw new WmsException(418, "货位上物料数量不足!");
            }


            return this;
        }

        //public IWave AddWaveTask(string waveDetailID, string locationNoOrID, decimal leftQty)
        public decimal AddWaveTask(WorkTaskModel task, decimal leftQty)
        {
            //DataRow detailRow = GetWaveDetail(waveDetailID);
            //Wave.GenTasksForOutbound(new DataRow[] { detailRow }, detailRow,
            //    new Address(orderRow[oh.DestAddrID].ToString(),
            //            orderRow[oh.DestAddrType].ToString(), orderRow[oh.DestAddrText].ToString()),
            //    locRow);
            DataRow detailRow = GetWaveDetail(task.RefDocDetailID);
            decimal processedQty = decimal.Parse("0" + detailRow[wd.ProcessedQty]);
            decimal planQty = decimal.Parse("0" + detailRow[wd.PlanQty]);
            if (processedQty + leftQty > planQty)
            {
                throw new WmsException(500, "当前分配数量加上已分配数量已超出需要数量！");
            }
            detailRow[wd.PlanQty] = processedQty + leftQty;
            decimal seqNo = decimal.Parse("0" + detailRow[wd.DetailSeq]);
            decimal taskNum = Wave.GenTasksForOutbound(new DataRow[] { detailRow }, detailRow,
                new Address(task.DestAddrID, task.DestAddrType, task.DestAddrText),
                Location.Get(task.SrcAddrID), ref seqNo);
            AddHeadLeftTaskQty(detailRow[wd.WaveHeadID].ToString(), taskNum);

            processedQty = decimal.Parse("0" + detailRow[wd.ProcessedQty]);
            planQty = decimal.Parse("0" + detailRow[wd.PlanQty]);
            return planQty - processedQty;
        }

        public IWave AppendWave(string waveHeadID, string orderIds)
        {
            throw new NotImplementedException();
        }

        public decimal ComputeOutQty(decimal availQty, decimal leftQty)
        {
            throw new NotImplementedException();
        }

        public string ComputeTaskType(decimal availQty, decimal outQty)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 预分配库存
        /// </summary>
        /// <param name="headNoOrID"></param>
        /// <param name="target"></param>
        /// <param name="locationGroup">货位组，如果为空，表明每个货位组都需要计算，最后不提交事务。</param>
        /// <returns></returns>
        public IWave PrecomputeWave(string headNoOrID, string target, string locationGroup)
        {
            if (string.IsNullOrWhiteSpace(headNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow headRow = GetWaveHead(headNoOrID);

            if (string.IsNullOrWhiteSpace(locationGroup))
            {
                PrecomputeWave(headRow, target);
                return this;
            }

            ComputeWave(headRow, target, locationGroup, out List<string> allocatedLocations);
            return this;
        }

        private class WaveResult
        {
            public string locationGroupFirst { get; set; } = "";

            public decimal ItemNumInP1 { get; set; } = 0;

            public decimal QtyInP1 { get; set; } = 0;

            public decimal ItemNumInP2 { get; set; } = 0;

            public decimal QtyInP2 { get; set; } = 0;

            public decimal ItemNumInOther { get; set; } = 0;

            public decimal QtyInOther { get; set; } = 0;
        }

        private string PrecomputeWave(DataRow headRow, string target)
        {
            DeleteWaveResult(headRow["t_id"].ToString());

            DBHelper.EndTransaction(true);
            DBHelper.BeginTransaction();

            // 取货位组
            DataRow[] groups = Location.GetLocationGroup();
            groups = groups.Where(r => "P1,P2".Contains(r["locgp_code"].ToString())).ToArray();
            /*
            WaveResult[] results = new WaveResult[]
            {
                new WaveResult(){ locationGroupFirst = "P1" },
                new WaveResult(){ locationGroupFirst = "P2" },
            };
            */

            // 根据货位组循环
            foreach (DataRow row in groups)
            {
                ComputeWave(headRow, target, row["t_id"].ToString(), out List<string> locations);

                DataRow resultRow = GetWaveResult(headRow, row);

                DBHelper.EndTransaction(false);
                DBHelper.BeginTransaction();

                SaveWaveResult(resultRow);
                //SaveWaveResult(headRow, row);

                DBHelper.EndTransaction(true);
                DBHelper.BeginTransaction();
                /*
                DataRow[] resultRows;
                WaveResult result = results.First(r => row["locgp_code"].ToString() == r.locationGroupFirst);
                foreach (DataRow r in resultRows)
                {
                    if (r["locgp_code"].ToString() == "P1")
                    {

                    }

                }
                */
            }

            // 保存结果

            // 返回

            return "";
        }

        private DataRow GetWaveResult(DataRow headRow, DataRow row)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "t_id", SnowflakeID.Guid().ToString() },
                { "head_id", headRow["t_id"].ToString() },
                { "group_name", row["locgp_name"].ToString() + "优先" },
                { "group_code", row["locgp_code"].ToString() },
                { "group_id", row["t_id"].ToString() },

            };
            string sql = @"
select @t_id t_id, '&CLIENT_ID&' client_id, '&ORG_ID&' org_id, 
       @warehouse warehouse_id, @head_id wave_h_id, @group_name prior_group, 
       @group_code locgp_code, @group_id locgp_id, ifnull(p1_cnt, 0) p1_item_count, 
       ifnull(p1_qty, 0) p1_qty, ifnull(p2_cnt, 0) p2_item_count, ifnull(p2_qty, 0) p2_qty, 
       ifnull(other_cnt, 0) other_item_count, ifnull(other_qty, 0) other_qty, 
       sysdate create_date, @username created_by, @login_ip modify_ip, 
       sysdate modify_date, @username last_modified_by, '1' act_status
  from (
select case when lg.locgp_code = 'P1' or lg.locgp_code = 'P2' then lg.locgp_code else 'Other' end locgp_code, count(distinct item_id) cnt, sum(operation_qty) qty from isa_m_wave_task wt
  left join isa_bi_storage_loc l on wt.source_id = l.t_id
  left join WMS_BI_STORAGE_LOCGP lg on l.isa_bi_storage_locgp_id2 = lg.t_id 
 where wt.warehouse_id = @warehouse 
   and wave_h_id = @head_id 
   and act_status = '1'
 group by lg.locgp_code) 
 pivot (sum(qty) qty, sum(cnt) cnt
   for locgp_code in('P1' as P1,
                     'P2' as P2, 
                     'Other' as Other))  
";
            DataTable dt = DBHelper.Select(
                sql, param, "获取波次预分配临时结果");

            return dt.Rows[0];
        }

        private void Insert(WaveResultModel model)
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
                { "wave_h_id", model.WaveHeadID },
                { "prior_group", model.PriorGroup },
                { "locgp_code", model.LocgpCode },
                { "locgp_id", model.LocgpID },
                { "p1_item_count", model.P1ItemCount.ToString() },
                { "p1_qty", model.P1Qty.ToString() },
                { "p2_item_count", model.P2ItemCount.ToString() },
                { "p2_qty", model.P2Qty.ToString() },
                { "other_item_count", model.OtherItemCount.ToString() },
                { "other_qty", model.OtherQty.ToString() },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into WMS_T_WAVE_RESULT ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       wave_h_id, " +
                "       prior_group, " +
                "       locgp_code, " +
                "       locgp_id, " +
                "       p1_item_count, " +
                "       p1_qty, " +
                "       p2_item_count, " +
                "       p2_qty, " +
                "       other_item_count, " +
                "       other_qty, " +
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
                "       @wave_h_id, " +
                "       @prior_group, " +
                "       @locgp_code, " +
                "       @locgp_id, " +
                "       @p1_item_count, " +
                "       @p1_qty, " +
                "       @p2_item_count, " +
                "       @p2_qty, " +
                "       @other_item_count, " +
                "       @other_qty, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "波次运算临时结果");

        }

        private void SaveWaveResult(DataRow row)
        {
            Insert(new WaveResultModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                WaveHeadID = row["wave_h_id"].ToString(),
                PriorGroup = row["prior_group"].ToString(),
                LocgpCode = row["locgp_code"].ToString(),
                LocgpID = row["locgp_id"].ToString(),
                P1ItemCount = DBHelper.GetDecimal(row["p1_item_count"]),
                P1Qty = DBHelper.GetDecimal(row["p1_qty"]),
                P2ItemCount = DBHelper.GetDecimal(row["p2_item_count"]),
                P2Qty = DBHelper.GetDecimal(row["p2_qty"]),
                OtherItemCount = DBHelper.GetDecimal(row["other_item_count"]),
                OtherQty = DBHelper.GetDecimal(row["other_qty"]),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });
        }

        /*
        private void SaveWaveResult(DataRow headRow, DataRow row)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "t_id", SnowflakeID.Guid().ToString() },
                { "head_id", headRow["t_id"].ToString() },
                { "group_name", row["locgp_name"].ToString() },
                { "group_code", row["locgp_code"].ToString() },
                { "group_id", headRow["t_id"].ToString() },

            };
            string sql = @"
insert into isa_t_wave_result
   (t_id, client_id, org_id, warehouse_id, wave_h_id, prior_group, locgp_code, 
    locgp_id, p1_item_count, p1_qty, p2_item_count, p2_qty, other_item_count, 
    other_qty, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status)
select @t_id, '&CLIENT_ID&', '&ORG_ID&', @warehouse, @head_id, @group_name, @group_code,
       @group_id, ifnull(p1_cnt, 0), ifnull(p1_qty, 0), ifnull(p2_cnt, 0), ifnull(p2_qty, 0), 
       ifnull(other_cnt, 0), ifnull(other_qty, 0), sysdate,
       @username, @login_ip, sysdate, @username, '1'
  from (
select lg.locgp_code, count(distinct item_id) cnt, sum(operation_qty) qty from isa_m_wave_task wt
  left join isa_bi_storage_loc l on wt.source_id = l.t_id
  left join WMS_BI_STORAGE_LOCGP lg on l.isa_bi_storage_locgp_id2 = lg.t_id 
 where wt.warehouse_id = @warehouse 
   and wave_h_id = @head_id 
 group by lg.locgp_code) 
 pivot (sum(qty) qty, sum(cnt) cnt
   for locgp_code in('P1' as P1,
                     'P2' as P2, 
                     'Other' as Other))  
";
            DBHelper.RunSql(sql, param, "保存波次预分配临时结果", DBHelper.OperatedNumType.OnlyOne);

            return;
        }
        */

        public IWave SaveBarcodeNumber(string waveNo, string jsonItemList)
        {
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(jsonItemList);
            DataTable dt = ds.Tables[0];
            DataRow[] itemRows = dt.AsEnumerable().ToArray();
            string ids = string.Join(',', itemRows.Select(r => r["id"].ToString()).ToArray());

            DataRow[] taskRows = WorkTask.GetByRefDocNoOrID("Wave", waveNo);

            string toBeRemoved = string.Join(',', taskRows
                .Where(r => !ids.Contains(r["t_id"].ToString()))
                .Select(r => r["t_id"].ToString()).ToArray());

            if (!string.IsNullOrWhiteSpace(toBeRemoved))
            {
                WorkTask.RecycleTasks(toBeRemoved);
            }

            foreach (DataRow row in itemRows)
            {
                decimal qty = decimal.Parse("0" + row["qty"]);
                string newCtnDtlID = row["container_detail_id"].ToString();
                if (string.IsNullOrWhiteSpace(row["id"].ToString()))
                {
                    DataRow ctnDtlRow = Container.GetDetail(newCtnDtlID);
                    string location = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");
                    if (string.IsNullOrWhiteSpace(location))
                    {
                        throw new WmsException(450, "找不到码段货位!");
                    }
                    DataRow locRow = Location.Get(location);
                    WorkTask.AddOutboundTask(new WorkTaskModel()
                    {
                        SeqNo = 0,
                        //ItemID = ctnDtlRow["item_id"].ToString(),
                        RefDocNo = waveNo,
                        SrcAddrID = locRow["t_id"].ToString(),
                        SrcAddrType = "AddressType_Location",
                        SrcAddrText = location,
                        RefDocType = "RefDocType_Wave",
                        TaskType = "TaskType_Barcode",
                        IsExecutable = "0",
                    }, new ContainerDetailModel()
                    {
                        LotNo = ctnDtlRow["external_lotno"].ToString(),
                        ExistBarcode = "ExistBarcode_Exist",
                        ID = newCtnDtlID,
                    }, ref qty);
                    continue;
                }

                DataRow task = WorkTask.Get(row["id"].ToString());
                string ctnDtlID = task["isa_bi_container_detail_id"].ToString();
                decimal opQty = decimal.Parse("0" + task["operation_qty"]);
                if (newCtnDtlID != ctnDtlID || opQty != qty)
                {
                    Container.CancelStockingOut(ctnDtlID, opQty)
                        .Prepare4StockingOut(newCtnDtlID, qty);

                    WorkTask.UpdateOperationQty(row["id"].ToString(),
                        qty, opQty);
                }
            }

            return this;
        }

        private void ComputeWave(DataRow headRow, string target,
            string locationGroup, out List<string> allocatedLocations)
        {
            allocatedLocations = new List<string>();

            DataRow[] detailRows = CheckBeforeCompute(headRow);

            string groupSeq = string.Empty;
            string lotNo = string.Empty;
            decimal lot = 1;

            // 更新波次头表
            UpdateHeadStatus(headRow["t_id"].ToString(),
                WaveStatus.WaveStatus_Computing.ToString(),
                headRow[wh.WaveStatus].ToString());

            decimal seqNo = 1;
            string saftyPeriod = WmsConfig.GetOrDefault("Inventory:SaftyPeriod", "120");
            foreach (DataRow row in detailRows)
            {
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "I_WAREHOUSEID", row["warehouse_id"].ToString() },
                    { "ITEMID", row["item_id"].ToString() },
                    { "I_ITEMID", row["item_id"].ToString() },
                    { "BOMID", row["item_id"].ToString() },
                    { "I_STORAGE_LOCGP1", "" },
                    { "I_STORAGE_LOCGP2", locationGroup },
                    { "I_EXPIRY_DATE", saftyPeriod },
                    { "I_ZONE", "" },
                    { "I_HEADID", "" },
                    { "I_DETAILID", "" },
                    { "I_PACKING_TYPE", "" },
                    { "I_ROOM_NO", "" },
                };

                if (WmsConfig.GetOrDefault("Outbound:ComputeWaveByLot", "0") == "0")
                {
                    ComputeWaveDetail(param, detailRows, allocatedLocations, row, ref seqNo);
                }
                else
                {
                    if (lotNo != row[od.LotNo].ToString())
                    {
                        lotNo = row[od.LotNo].ToString();
                        lot = GetMinLotSeq(lotNo);
                    }
                    ComputeWaveDetailByLotNew(param, detailRows, allocatedLocations, row, lot, ref seqNo);
                }
            }
            // 更新波次头表
            UpdateHeadStatus(headRow["t_id"].ToString(), "WaveStatus_Editable");

            ComputeWave4BarcodeNumber(headRow["wave_no"].ToString(), detailRows);

        }

        readonly string updateStatusForCompactLocation = WmsConfig.GetOrDefault("Outbound:UpdateStatusForCompactLocation", "0");
        //private void ComputeWaveDetail(string target, string locationGroup, 
        //    List<string> allocatedLocations, ref string lastLocation, string saftyPeriod, DataRow row)
        private void ComputeWaveDetail(Dictionary<string, object> param,
            DataRow[] detailRows, List<string> allocatedLocations,
            DataRow detailRow, ref decimal seqNo)
        {
            //string groupNo = detailRow["order_seq"].ToString();
            decimal leftQty = decimal.Parse("0" + detailRow["plan_qty"].ToString())
                            - decimal.Parse("0" + detailRow["processed_qty"].ToString());

            DataRow[] orderHeadRows = OutboundOrder.GetOrderHeads(detailRow["outbound_h_id"].ToString());
            DataRow orderDetailRow = OutboundOrder.GetDetail(detailRow["outbound_d_id"].ToString());
            string targetType = orderHeadRows[0]["target_type"].ToString();
            if (targetType == "Address_Dock")
            {
                DataRow dockRow = Dock.GetDock(orderHeadRows[0]["target_id"].ToString());
                targetType = dockRow["type_code"].ToString();
            }
            else if (targetType == "Address_Station")
            {
                DataRow equipRow = Equipment.GetEquipment(orderHeadRows[0]["target_id"].ToString());
                targetType = equipRow["type_code"].ToString();
            }
            else
            {
                targetType = "";
            }
            //Address dest = new Address()
            //{
            //    Type = orderHeadRows[0]["dest_addr_type"].ToString(),
            //    ID = orderHeadRows[0]["dest_addr_id"].ToString(),
            //    Text = orderHeadRows[0]["dest_addr_text"].ToString(),
            //};
            Address dest = new Address(orderHeadRows[0]["dest_addr_type"].ToString(),
                orderHeadRows[0]["dest_addr_id"].ToString());

            string itemID = string.Empty;
            string partialFlag = "0";
            string lastLocation = "";
            DataRow itemRow = Item.Get(detailRow["item_id"].ToString());
            decimal pickingZoneQty = 0;

            while (leftQty > 0)
            {
                string location = null;

                /*
                if (targetType == "Equipment_StationOut_Picking" ||
                    targetType == "Dock_Picking")
                {
                    leftQty = PickingZone.HandlePickingZoneBeforeAllocation(detailRow, groupNo, leftQty);
                    if (leftQty <= 0) break;
                }
                */

                DataRow locRow = null;
                try
                {
                    param["LOTNO"] = orderDetailRow[od.LotNo].ToString();
                    param["I_BATCH_NO"] = orderDetailRow[od.LotNo].ToString();
                    param["I_BARCODE"] = orderDetailRow[od.ExistBarcode].ToString();
                    param["I_LASTLOCATION_NO"] = lastLocation;
                    param["I_FRACTION"] = partialFlag;
                    param["I_SPEC"] = orderDetailRow[od.Spec].ToString();
                    param["SPECS"] = orderDetailRow[od.Spec].ToString();
                    param["PLANLOTNO"] = orderHeadRows.First()[oh.PrepProdOrder].ToString();

                    if (WmsConfig.GetOrDefault("Outbound:UsingTestAllocator", "0") == "0")
                    {
                        location = LocationAllocator.AllocateInventory(param,
                                    WmsConfig.GetOrDefault("Outbound:AllocationRule", "work_task_4"));
                    }
                    else
                    {
                        location = new LocationAllocator().AllocateInventoryByStoreProc(param);
                    }

                    locRow = Location.Get(location);
                    location = locRow[l.ID].ToString();
                }
                catch (WmsException ex)
                {
                    if (ex.ErrorCode == -605 && PickingZone != null)
                    {
                        pickingZoneQty = PickingZone.GetInventory(detailRow["item_id"].ToString());
                        if (pickingZoneQty <= leftQty || (
                                targetType != "Equipment_StationOut_Picking" &&
                                targetType != "Dock_Picking" && itemRow["robot_enabled"].ToString() == "1"))
                        {
                            throw new WmsException(-605, $"货位分配失败！" +
                                $"已分配{allocatedLocations.Count()}个货位，" +
                                $"可能是没有可用物料{detailRow["item_name"]}！", ex);
                        }
                    }
                    else throw;
                }

                if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    if (locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
                    {
                        throw new WmsException(500, $"分配到的货位{locRow[l.LocationCode]}不可用");
                    }
                    allocatedLocations.Add(location);
                    Location.UpdateStatus(locRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Out.ToString(), locRow[l.LocationStatus].ToString());
                }
                if (updateStatusForCompactLocation == "1" &&
                    locRow[l.LocationType].ToString() == LocationType.LocationType_Compact.ToString())
                {
                    if (locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
                    {
                        throw new WmsException(500, $"分配到的货位{locRow[l.LocationCode]}不可用");
                    }
                    allocatedLocations.Add(location);
                    Location.UpdateStatus(locRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Out.ToString(), locRow[l.LocationStatus].ToString());
                }

                GenTasksForOutbound(detailRows, detailRow, dest, locRow, ref seqNo);

                leftQty = decimal.Parse("0" + detailRow[wd.PlanQty].ToString())
                            - decimal.Parse("0" + detailRow[wd.ProcessedQty].ToString());
            }
        }

        /// <summary>
        /// 按批次计算
        /// </summary>
        /// <param name="param"></param>
        /// <param name="detailRows"></param>
        /// <param name="allocatedLocations"></param>
        /// <param name="detailRow"></param>
        /// <param name="seqNo"></param>
        private void ComputeWaveDetailByLotNew(Dictionary<string, object> param,
            DataRow[] detailRows, List<string> allocatedLocations,
            DataRow detailRow, decimal startLot, ref decimal seqNo)
        {
            //string groupNo = detailRow["order_seq"].ToString();
            decimal planQty = decimal.Parse("0" + detailRow["plan_qty"]);
            decimal leftQty = planQty - decimal.Parse("0" + detailRow["processed_qty"]);
            if (leftQty <= 0)
            {
                return;
            }
            planQty = detailRows.Where(r => r[wd.ItemID].ToString() == detailRow[wd.ItemID].ToString())
                .Sum(r => decimal.Parse("0" + r[wd.PlanQty]));

            decimal batches = decimal.Parse("0" + detailRow["batches"]);
            decimal qtyPerLot = planQty / batches;
            decimal lot = startLot;
            decimal lotNum = 0;

            DataRow[] orderHeadRows = OutboundOrder.GetOrderHeads(detailRow["outbound_h_id"].ToString());
            DataRow orderDetailRow = OutboundOrder.GetDetail(detailRow["outbound_d_id"].ToString());
            string targetType = orderHeadRows[0]["target_type"].ToString();
            if (targetType == "Address_Dock")
            {
                DataRow dockRow = Dock.GetDock(orderHeadRows[0]["target_id"].ToString());
                targetType = dockRow["type_code"].ToString();
            }
            else if (targetType == "Address_Station")
            {
                DataRow equipRow = Equipment.GetEquipment(orderHeadRows[0]["target_id"].ToString());
                targetType = equipRow["type_code"].ToString();
            }
            else
            {
                targetType = "";
            }
            Address dest = new Address()
            {
                Type = orderHeadRows[0]["dest_addr_type"].ToString(),
                ID = orderHeadRows[0]["dest_addr_id"].ToString(),
                Text = orderHeadRows[0]["dest_addr_text"].ToString(),
            };

            string itemID = string.Empty;
            string partialFlag = "0";
            string lastLocation = "";
            DataRow itemRow = Item.Get(detailRow["item_id"].ToString());
            decimal pickingZoneQty = 0;

            for (int i = 1; i <= batches; i++)
            {
                //for (int j = 0; j < qtyPerLot; j++)
                decimal outNum = 0;
                while (true)
                {
                    string containerID = null;
                    string location = null;
                    DataRow locRow = null;
                    try
                    {
                        param["LOTNO"] = orderDetailRow[od.LotNo].ToString();
                        param["I_BATCH_NO"] = orderDetailRow[od.LotNo].ToString();
                        param["I_BARCODE"] = orderDetailRow[od.ExistBarcode].ToString();
                        param["I_LASTLOCATION_NO"] = lastLocation;
                        param["I_FRACTION"] = partialFlag;
                        param["I_SPEC"] = orderDetailRow[od.Spec].ToString();
                        param["I_LOT_SEQ"] = lot.ToString();
                        param["SPECS"] = orderDetailRow[od.Spec].ToString();
                        param["PLANLOTNO"] = orderHeadRows.First()[oh.PrepProdOrder].ToString();

                        containerID = new LocationAllocator().AllocateInventoryByStoreProc(param);

                        if (string.IsNullOrWhiteSpace(containerID))
                        {
                            if (outNum <= 0)
                            {
                                throw new WmsException(-605, $"货位分配失败！" +
                                    $"物料[{detailRow["item_name"]}]没有找到可用库存！");
                            }
                            // 检查数目
                            if (outNum < qtyPerLot)
                            {
                                throw new WmsException(-605, $"货位分配失败！" +
                                    $"物料[{detailRow["item_name"]}]批次[{lot}]" +
                                    $"可用库存[{outNum}]不足,出库计划数量为[{qtyPerLot}]！");
                            }
                            break;
                        }

                        locRow = Location.GetByContainer(containerID);
                        if (locRow[l.LocationCode].ToString() == "00000000"
                            || locRow[l.Usage].ToString() != LocationUse.LocationUse_Stock.ToString())
                        {
                            throw new WmsException(-607, $"货位分配失败！" +
                                $"物料[{detailRow["item_name"]}]不在存储区，可能在暂存区！");
                        }
                        location = locRow[l.ID].ToString();
                    }
                    catch (WmsException ex)
                    {
                        if (ex.ErrorCode == -606)
                        {
                            if (outNum <= 0)
                            {
                                throw new WmsException(-605, $"货位分配失败！" +
                                    $"物料[{detailRow["item_name"]}]没有找到可用库存！");
                            }
                            break;
                        }
                        else throw;
                    }
                    DataRow ctnRow = Container.GetHead(containerID);
                    if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                    {
                        if (locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
                        {
                            throw new WmsException(500, $"分配到的货位{locRow[l.LocationCode]}不可用");
                        }
                        allocatedLocations.Add(location);
                        Location.UpdateStatus(locRow[l.ID].ToString(),
                            LocationStatus.LocationStatus_Out.ToString(), locRow[l.LocationStatus].ToString());
                    }
                    else
                    {
                        Container.UpdateHeadIsOut(containerID, "2", ctnRow[ch.IsOut].ToString());
                    }

                    DataRow[] ctnDtlRows = Container.GetDetailByHeadID(containerID);
                    //DataRow ctnDtlRow = ctnDtlRows.Where(r => 
                    //    r[cd.ItemID].ToString() == detailRow[wd.ItemID].ToString()).First();

                    GenTasksForOutbound(detailRow, dest, ctnRow, ctnDtlRows, ref seqNo);
                    outNum++;

                    if (ctnDtlRows.Length == 1)
                    {
                        // 非合包
                        List<DataRow> todoRows = detailRows.Where(r =>
                            r[wd.ItemID].ToString() == detailRow[wd.ItemID].ToString()).ToList();
                        todoRows.ForEach(r => UpdateProcessedQtyFromPlanQty(r));
                    }
                    else
                    {
                        // 由于李月流说合包还是分开容器头和明细，所以下面的分支应该走不到
                        throw new WmsException(500, "检测到数据库中存在错误数据！");
                        // 合包
                        foreach (DataRow ctnDtlRow in ctnDtlRows)
                        {
                            if (ctnDtlRow[cd.ItemID].ToString() == detailRow[wd.ItemID].ToString())
                            {
                                UpdateProcessedQtyFromPlanQty(detailRow);
                            }
                            else
                            {
                                List<DataRow> todoRows = detailRows.Where(r =>
                                    r[wd.ItemID].ToString() == ctnDtlRow[cd.ItemID].ToString()).ToList();
                                if (todoRows.Count == 1)
                                {
                                    UpdateProcessedQtyFromPlanQty(todoRows.First());
                                }
                            }
                        }
                    }
                }
                lot++;
            }
        }
        /// <summary>
        /// 按托盘来生成任务
        /// </summary>
        /// <param name="detailRow"></param>
        /// <param name="dest"></param>
        /// <param name="ctnRow"></param>
        /// <param name="ctnDtlRows"></param>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        public decimal GenTasksForOutbound(DataRow detailRow, Address dest,
            DataRow ctnRow, DataRow[] ctnDtlRows, ref decimal seqNo)
        {
            DataRow headRow = Wave.GetWaveHead(detailRow["wave_h_id"].ToString());

            // 初始化任务路径
            DataRow itemCatalog = Item.GetItemCatalog(detailRow["item_id"].ToString());
            Route outboundRoute = new Route();
            outboundRoute.Init(itemCatalog["t_id"].ToString(), RoutePurpose.RoutePurpose_Outbound.ToString());
            Route pickingRoute = new Route();
            pickingRoute.Init(itemCatalog["t_id"].ToString(), RoutePurpose.RoutePurpose_Picking.ToString());

            // 取得货位库存
            DataRow[] inventoryRows = Container.GetInventoryDetailsByContainer(ctnRow[ch.ID].ToString());

            // 匹配库存与波次明细
            List<WorkTaskItemModel> taskItemList = new List<WorkTaskItemModel>();
            foreach (DataRow row in ctnDtlRows)
            {
                taskItemList.Add(new WorkTaskItemModel()
                {
                    ItemID = row[cd.ItemID].ToString(),
                    PlanQty = decimal.Parse(row[cd.Qty].ToString()),
                    ContainerDetailID = row[cd.ID].ToString(),
                    RefDocDetailID = detailRow[wd.ID].ToString(),
                });
            }

            // 判断是否需要拣选
            Route route = outboundRoute;

            // 保存任务
            string priority = WmsConfig.GetOrDefault("Task:Priority:Outbound", "100");
            //Address src = new Address(ctnRow[ch.LocationID].ToString());
            Address src = new Address(AddressType.AddressType_Location, ctnRow[ch.LocationID].ToString());
            // 匹配任务路径
            if (!route.MatchRoute(src, dest, out DataRow[] sequences))
            {
                throw new WmsException(500, "无法找到合适的配置来生成任务");
            }
            // 保存任务
            decimal taskNum = WorkTask.SaveTask(sequences, new WorkTaskModel()
            {
                //SeqNo = Decimal.Parse(detailRow["detail_seq"].ToString()),
                SeqNo = seqNo++,
                Priority = decimal.Parse(priority),
                ContainerID = inventoryRows[0]["t_id"].ToString(),
                ContainerBarcode = inventoryRows[0]["container_barcode"].ToString(),
                SrcAddrID = src.ID,
                SrcAddrType = src.Type,
                SrcAddrText = src.Text,
                DestAddrID = dest.ID,
                DestAddrType = dest.Type,
                DestAddrText = dest.Text,
                RefDocType = DocType.DocType_Wave.ToString(),
                RefDocID = headRow[wh.ID].ToString(),
                RefDocNo = headRow[wh.WaveNo].ToString(),
                LedText = BuildLedText(detailRow, inventoryRows[0]["container_barcode"].ToString()),
            }, taskItemList);

            return taskNum;
        }

        private void UpdateProcessedQtyFromPlanQty(DataRow r)
        {
            if (decimal.Parse("0" + r[wd.ProcessedQty]) == decimal.Parse("0" + r[wd.PlanQty]))
            {
                return;
            }
            r[wd.ProcessedQty] = r[wd.PlanQty];
            UpdateDetailProcessedQty(r[wd.ID].ToString(), decimal.Parse("0" + r[wd.PlanQty]));
        }

        /// <summary>
        /// 按批次计算
        /// </summary>
        /// <param name="param"></param>
        /// <param name="detailRows"></param>
        /// <param name="allocatedLocations"></param>
        /// <param name="detailRow"></param>
        /// <param name="seqNo"></param>
        private void ComputeWaveDetailByLot(Dictionary<string, object> param,
            DataRow[] detailRows, List<string> allocatedLocations,
            DataRow detailRow, decimal startLot, ref decimal seqNo)
        {
            //string groupNo = detailRow["order_seq"].ToString();
            decimal planQty = decimal.Parse("0" + detailRow["plan_qty"]);
            decimal leftQty = planQty - decimal.Parse("0" + detailRow["processed_qty"]);

            decimal batches = decimal.Parse("0" + detailRow["batches"]);
            decimal qtyPerLot = planQty / batches;
            decimal lot = startLot;
            decimal lotNum = 0;

            DataRow[] orderHeadRows = OutboundOrder.GetOrderHeads(detailRow["outbound_h_id"].ToString());
            DataRow orderDetailRow = OutboundOrder.GetDetail(detailRow["outbound_d_id"].ToString());
            string targetType = orderHeadRows[0]["target_type"].ToString();
            if (targetType == "Address_Dock")
            {
                DataRow dockRow = Dock.GetDock(orderHeadRows[0]["target_id"].ToString());
                targetType = dockRow["type_code"].ToString();
            }
            else if (targetType == "Address_Station")
            {
                DataRow equipRow = Equipment.GetEquipment(orderHeadRows[0]["target_id"].ToString());
                targetType = equipRow["type_code"].ToString();
            }
            else
            {
                targetType = "";
            }
            Address dest = new Address()
            {
                Type = orderHeadRows[0]["dest_addr_type"].ToString(),
                ID = orderHeadRows[0]["dest_addr_id"].ToString(),
                Text = orderHeadRows[0]["dest_addr_text"].ToString(),
            };

            string itemID = string.Empty;
            string partialFlag = "0";
            string lastLocation = "";
            DataRow itemRow = Item.Get(detailRow["item_id"].ToString());
            decimal pickingZoneQty = 0;

            while (leftQty > 0)
            {
                string location = null;

                /*
                if (targetType == "Equipment_StationOut_Picking" ||
                    targetType == "Dock_Picking")
                {
                    leftQty = PickingZone.HandlePickingZoneBeforeAllocation(detailRow, groupNo, leftQty);
                    if (leftQty <= 0) break;
                }
                */

                if (decimal.Parse("0" + orderDetailRow[od.SeqNo]) == 9)
                {
                    location = null;
                }
                DataRow locRow = null;
                try
                {
                    param["LOTNO"] = orderDetailRow[od.LotNo].ToString();
                    param["I_BATCH_NO"] = orderDetailRow[od.LotNo].ToString();
                    param["I_BARCODE"] = orderDetailRow[od.ExistBarcode].ToString();
                    param["I_LASTLOCATION_NO"] = lastLocation;
                    param["I_FRACTION"] = partialFlag;
                    param["I_SPEC"] = orderDetailRow[od.Spec].ToString();
                    param["I_LOT_SEQ"] = lot.ToString();
                    param["SPECS"] = orderDetailRow[od.Spec].ToString();
                    param["PLANLOTNO"] = orderHeadRows.First()[oh.PrepProdOrder].ToString();

                    //if (orderDetailRow[od.ItemID].ToString() == "CE51811ECBE04B5098916D1F2BA3CAEA")
                    //{
                    //    location = null;
                    //}
                    if (WmsConfig.GetOrDefault("Outbound:UsingTestAllocator", "0") == "0")
                    {
                        location = LocationAllocator.AllocateInventory(param,
                                    WmsConfig.GetOrDefault("Outbound:AllocationRule", "work_task_4"));
                    }
                    else
                    {
                        location = new LocationAllocator().AllocateInventoryByStoreProc(param);
                    }

                    if (string.IsNullOrWhiteSpace(location))
                    {
                        throw new WmsException(-605, $"货位分配失败！" +
                            $"物料[{detailRow["item_name"]}]没有找到可用库存！");
                    }

                    locRow = Location.Get(location);
                    if (locRow[l.LocationCode].ToString() == "00000000"
                        || locRow[l.Usage].ToString() != LocationUse.LocationUse_Stock.ToString())
                    {
                        throw new WmsException(-607, $"货位分配失败！" +
                            $"物料[{detailRow["item_name"]}]不在存储区，可能在暂存区！");
                    }
                    location = locRow[l.ID].ToString();
                }
                catch (WmsException ex)
                {
                    if (ex.ErrorCode == -605 && PickingZone != null)
                    {
                        pickingZoneQty = PickingZone.GetInventory(detailRow["item_id"].ToString());
                        if (pickingZoneQty <= leftQty || (
                                targetType != "Equipment_StationOut_Picking" &&
                                targetType != "Dock_Picking" && itemRow["robot_enabled"].ToString() == "1"))
                        {
                            throw new WmsException(-605, $"货位分配失败！" +
                                $"已分配{allocatedLocations.Count()}个货位，" +
                                $"可能是没有可用物料[{detailRow["item_name"]}]！", ex);
                        }
                    }
                    else if (ex.ErrorCode == -606)
                    {
                        throw new WmsException(-606, $"货位分配失败！" +
                            $"物料[{detailRow["item_name"]}]没有找到可用库存！");
                    }
                    else throw;
                }

                if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    if (locRow[l.LocationStatus].ToString() != LocationStatus.LocationStatus_Stored.ToString())
                    {
                        throw new WmsException(500, $"分配到的货位{locRow[l.LocationCode]}不可用");
                    }
                    allocatedLocations.Add(location);
                    Location.UpdateStatus(locRow[l.ID].ToString(),
                        LocationStatus.LocationStatus_Out.ToString(), locRow[l.LocationStatus].ToString());
                }

                //GenTasksForOutbound(detailRows, detailRow, dest, locRow, ref seqNo, lot);
                GenTasksForOutbound(detailRows, detailRow, dest, locRow, ref seqNo);

                lotNum++;
                if (lotNum >= qtyPerLot)
                {
                    lot++;
                    lotNum = 0;
                }
                leftQty = decimal.Parse("0" + detailRow[wd.PlanQty].ToString())
                            - decimal.Parse("0" + detailRow[wd.ProcessedQty].ToString());
            }
        }

        private decimal GetMinLotSeq(string lotNo)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "lot_no", lotNo },
            };
            //            string selSql = @"
            //select min(lot_seq) lot_seq from &DataSource&wms_b_container_d cd 
            // where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&'
            //   and cd.lot_no = @lot_no
            //   and cd.warehouse_id = @warehouse
            //   and cd.act_status = '1'
            //";
            string selSql = @"
select min(lot_seq) lot_seq from &DataSource&wms_b_container_d cd
  left join &DataSource&wms_b_container_h ch on cd.container_id = ch.t_id
  left join &DataSource&wms_m_location l on ch.location_id = l.t_id
 where cd.lot_no = @lot_no
   and cd.warehouse_id = @warehouse
   and l.location_no != '00000000'
   and cd.act_status = '1'
   and l.location_status in ('LocationStatus_Stored', 'LocationStatus_Partial')
";
            DataTable dt = DBHelper.Select(selSql, param, "查询最小批次序号");
            string lotSeq = dt.Rows[0]["lot_seq"].ToString();
            if (string.IsNullOrWhiteSpace(lotSeq))
            {
                throw new WmsException(500, $"无法找到批次{lotNo}的最小批次序号，可能未入库或已出完");
            }

            return decimal.Parse(lotSeq);
        }

        readonly string filterContainer = WmsConfig.GetOrDefault("Outbound:FilterInventoryByLot", "0");
        readonly string generateOutboundPurposeTaskOnly = WmsConfig.GetOrDefault("Outbound:GenerateOutboundPurposeTaskOnly", "0");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="detailRows"></param>
        /// <param name="detailRow"></param>
        /// <param name="dest"></param>
        /// <param name="locRow"></param>
        /// <returns></returns>
        public decimal GenTasksForOutbound(DataRow[] detailRows,
            //DataRow detailRow, Address dest, DataRow locRow, ref decimal seqNo, decimal lot = 0)
            DataRow detailRow, Address dest, DataRow locRow, ref decimal seqNo)
        {
            DataRow headRow = Wave.GetWaveHead(detailRow["wave_h_id"].ToString());

            // 初始化任务路径
            DataRow itemCatalog = Item.GetItemCatalog(detailRow["item_id"].ToString());
            Route outboundRoute = new Route();
            outboundRoute.Init(itemCatalog["t_id"].ToString(), RoutePurpose.RoutePurpose_Outbound.ToString());
            Route pickingRoute = new Route();
            pickingRoute.Init(itemCatalog["t_id"].ToString(), RoutePurpose.RoutePurpose_Picking.ToString());

            // 取得货位库存
            DataRow[] inventoryRows = Container.GetInventoryDetails(locRow);
            //DataRow[] inv1 = inventoryRows.Where(r => r["item_id"].ToString() == detailRow[wd.ItemID].ToString()).ToArray();
            //DataRow[] inv2 = inventoryRows.Where(r => (r["item_id"].ToString() == detailRow[wd.ItemID].ToString()
            //  && r["lot_no"].ToString() == detailRow["lot_no"].ToString()
            //  )).ToArray();
            //DataRow[] inv3 = inventoryRows.Where(r => (r["item_id"].ToString() == detailRow[wd.ItemID].ToString()
            // && r["lot_no"].ToString() == detailRow["lot_no"].ToString()
            // && decimal.Parse("0" + r["lot_seq"]) == lot)).ToArray();

            if (filterContainer == "1")
            {
                // 20200907 平库分配错误问题
                //inventoryRows = inventoryRows.Where(r => r["item_id"].ToString() == detailRow[wd.ItemID].ToString()
                //  && r["lot_no"].ToString() == detailRow["lot_no"].ToString()
                //  //&& r["spec"].ToString() == detailRow["spec"].ToString()
                //  ).OrderBy(r => decimal.Parse("0" + r["lot_seq"])).ToArray();
                // 20200925 改算法了，下面都不要了
                //LogHelper.LogDebug(JsonConvert.SerializeObject(inventoryRows));
                //inventoryRows = inventoryRows.Where(r => (r["item_id"].ToString() == detailRow[wd.ItemID].ToString()
                //  && r["lot_no"].ToString() == detailRow["lot_no"].ToString()
                //  && decimal.Parse("0" + r["lot_seq"]) == lot
                //  && r["is_out"].ToString() != "2")
                //  ).ToArray();
                //if (inventoryRows.Length <= 0)
                //{
                //    DataRow orderDetailRow = OutboundOrder.GetDetail(detailRow[wd.OutboundDetailID].ToString());
                //    throw new WmsException(500, $"查找物料[{orderDetailRow[od.ItemName]}]可用库存时出现错误！"
                //        + $"批次[{detailRow["lot_no"]}]批次序号[{lot}]");
                //}
                //inventoryRows = new DataRow[] { inventoryRows.First() };
            }

            //inventoryRows = Container.GetInventoryDetailsByContainer(ctnRows[0][ch.ID].ToString());

            // 匹配库存与波次明细
            List<WorkTaskItemModel> taskItemList = AllocateItems(detailRows, inventoryRows);
            if (taskItemList.Count() <= 0)
            {
                throw new WmsException(416, $"货位{locRow["location_no"]}分配物料错误！");
            }

            // 判断是否需要拣选
            Route route = outboundRoute;

            if (generateOutboundPurposeTaskOnly == "1")
            {
                if (locRow[l.LocationType].ToString() == LocationType.LocationType_3DRack.ToString())
                {
                    foreach (DataRow row in inventoryRows)
                    {
                        if (decimal.Parse("0" + row["avail_qty"]) > 0)
                        {
                            route = pickingRoute;
                            break;
                        }
                    }
                }
            }
            // 保存任务
            string priority = "100";
            DataRow orderHeadRow = OutboundOrder.GetOrder(detailRow[wd.OutboundHeadID].ToString());
            if (orderHeadRow[oh.OutboundType].ToString() == OutServiceType.OutServiceType_Critical.ToString())
            {
                priority = WmsConfig.GetOrDefault("Task:Priority:EmergencyOutbound", "100");
            }
            else
            {
                priority = WmsConfig.GetOrDefault("Task:Priority:Outbound", "100");
            }
            //Address src = new Address(locRow[l.ID].ToString());
            Address src = new Address(AddressType.AddressType_Location, locRow[l.ID].ToString());
            // 匹配任务路径
            //Address src = new Address(workTask.SrcAddrID, workTask.SrcAddrType, workTask.SrcAddrText);
            //Address dest = new Address(workTask.DestAddrID, workTask.DestAddrType, workTask.DestAddrText);
            if (!route.MatchRoute(src, dest, out DataRow[] sequences))
            {
                throw new WmsException(500, "无法找到合适的配置来生成任务");
            }
            if (filterContainer == "1")
            {
                Container.UpdateHeadIsOut(inventoryRows.First()["t_id"].ToString(), "2");
            }
            // 保存任务
            decimal taskNum = WorkTask.SaveTask(sequences, new WorkTaskModel()
            {
                //SeqNo = Decimal.Parse(detailRow["detail_seq"].ToString()),
                SeqNo = seqNo++,
                Priority = decimal.Parse(priority),
                ContainerID = inventoryRows[0]["t_id"].ToString(),
                ContainerBarcode = inventoryRows[0]["container_barcode"].ToString(),
                SrcAddrID = src.ID,
                SrcAddrType = src.Type,
                SrcAddrText = src.Text,
                DestAddrID = dest.ID,
                DestAddrType = dest.Type,
                DestAddrText = dest.Text,
                RefDocType = DocType.DocType_Wave.ToString(),
                RefDocID = headRow[wh.ID].ToString(),
                RefDocNo = headRow[wh.WaveNo].ToString(),
                LedText = BuildLedText(detailRow, inventoryRows[0]["container_barcode"].ToString()),
                ZoneID = locRow[l.ZoneID].ToString(),
            }, taskItemList);
            //WorkTask.SaveTask(route, dest, detailRow, inventoryRows, taskItemList);

            //AddHeadLeftTaskQty(headRow[wh.ID].ToString(), taskNum);

            return taskNum;
        }
        // 合肥用
        private string BuildLedText(DataRow waveDetailRow, string barcode)
        {
            string ifBuildLedText = WmsConfig.GetOrDefault("Outbound:BuildLedText", "1");
            if (ifBuildLedText == "0")
            {
                return "";
            }
            DataRow orderHeadRow = OutboundOrder.GetOrder(waveDetailRow[wd.OutboundHeadID].ToString());
            DataRow orderDetailRow = OutboundOrder.GetDetail(waveDetailRow[wd.OutboundDetailID].ToString());
            string itemName = orderDetailRow[od.ItemName].ToString();
            DataRow container = Container.GetContainerByBarcode(barcode);

            List<string> itemNames = itemName.Split('.').ToList();
            if (itemNames.Count > 3)
            {
                itemNames.RemoveAt(0);
                itemNames.RemoveAt(0);
                itemNames.RemoveAt(0);
            }
            itemName = string.Join('.', itemNames);

            string prodLotNo = orderHeadRow[oh.ProdLotNo].ToString();
            int lastIndex = orderHeadRow[oh.BomCode].ToString().LastIndexOf("#");
            string bomNo = lastIndex < orderHeadRow[oh.BomCode].ToString().Length ? orderHeadRow[oh.BomCode].ToString().Substring(lastIndex+1):string.Empty;
          
            StringBuilder sbDisText = new StringBuilder();
            sbDisText.Append($"{prodLotNo}({bomNo})");
            sbDisText.Append("$$");
            sbDisText.Append($"{itemName}({container["spec"].ToString()})kg");
            sbDisText.Append("$$");
            sbDisText.Append(orderHeadRow[oh.BomCode].ToString());

            return sbDisText.ToString();
        }

        private string BuildLedText(string itemID)
        {
            DataRow itemRow = Item.Get(itemID);
            string itemName = itemRow[i.ItemName].ToString();

            List<string> itemNames = itemName.Split('.').ToList();
            itemNames.RemoveAt(0);
            itemNames.RemoveAt(0);
            itemNames.RemoveAt(0);
            itemName = string.Join('.', itemNames);
            return itemName;
        }


        public List<WorkTaskItemModel> AllocateItems(DataRow[] detailRows, DataRow[] inventoryRows)
        {
            Dictionary<string, WorkTaskItemModel> taskItemDict = new Dictionary<string, WorkTaskItemModel>();
            //DataRow waveHeadRow = GetWaveHead(detailRows[0][wd.WaveHeadID].ToString());

            foreach (DataRow dtlRow in detailRows)
            {
                decimal planQty = decimal.Parse("0" + dtlRow[wd.PlanQty].ToString())
                                - decimal.Parse("0" + dtlRow[wd.ProcessedQty].ToString());
                if (planQty <= 0)
                {
                    continue;
                }
                decimal leftQty = planQty;
                DataRow orderDtlRow = OutboundOrder.GetDetail(dtlRow["outbound_d_id"].ToString());

                //DataRow[] selectedRows = inventoryRows.Where(
                //    r => r["item_id"].ToString() == dtlRow["item_id"].ToString()
                //        && r["lot_no"].ToString() == orderDtlRow["lot_no"].ToString()).ToArray();
                DataRow[] selectedRows = inventoryRows.Where(
                    r => r["item_id"].ToString() == dtlRow["item_id"].ToString()).ToArray();

                foreach (DataRow invRow in selectedRows)
                {
                    decimal availQty = decimal.Parse(invRow["avail_qty"].ToString());
                    if (availQty <= 0)
                    {
                        continue;
                    }

                    decimal outQty = availQty < leftQty ? availQty : leftQty;

                    //Container.AddDetailOutQty(invRow[ti.ContainerDetailID].ToString(),
                    //    outQty, decimal.Parse("0" + invRow["out_qty"]));

                    leftQty -= outQty;

                    invRow["avail_qty"] = availQty - outQty;

                    // 更新波次明细表中的已处理量
                    UpdateDetailProcessedQty(dtlRow["t_id"].ToString(),
                        outQty + decimal.Parse("0" + dtlRow[wd.ProcessedQty].ToString()));

                    dtlRow[wd.ProcessedQty] = decimal.Parse("0" + dtlRow[wd.ProcessedQty]) + outQty;

                    // 更新订单明细表中的已处理量
                    //OutboundOrder.AddDetailProcessedQty(dtlRow[wd.OutboundDetailID].ToString(), outQty);

                    //qtyDict[ti.ContainerDetailID] += outQty;
                    //qtyDict.Add(invRow["container_d_id"].ToString(), outQty);
                    //if (qtyDict.ContainsKey(invRow[ti.ContainerDetailID].ToString()))
                    //{
                    //    qtyDict[invRow[ti.ContainerDetailID].ToString()] += outQty;
                    //}
                    //else
                    //{
                    //    qtyDict[invRow[ti.ContainerDetailID].ToString()] = outQty;
                    //}
                    //DataRow itemRow = Item.Get(invRow[cd.ItemID].ToString());
                    string key = invRow[ti.ContainerDetailID].ToString()
                                    + dtlRow[wd.ID].ToString();
                    if (taskItemDict.ContainsKey(key))
                    {
                        WorkTaskItemModel taskItem = taskItemDict[key];
                        taskItem.PlanQty += outQty;
                    }
                    else
                    {
                        taskItemDict[key] = new WorkTaskItemModel()
                        {
                            ItemID = invRow[cd.ItemID].ToString(),
                            //ItemID = itemRow["t_id"].ToString(),
                            //ItemName = itemRow["item_name"].ToString(),
                            PlanQty = outQty,
                            //UomQtyID = itemRow["default_pack_uom_id"].ToString(),
                            //UomStatID = itemRow["default_stat_uom_id"].ToString(),
                            ContainerDetailID = invRow[ti.ContainerDetailID].ToString(),
                            RefDocDetailID = dtlRow[wd.ID].ToString(),
                            //RefDocType = "DocType_Wave",
                            //RefDocID = dtlRow["wave_h_id"].ToString(),
                            //RefDocNo = waveHeadRow[wh.WaveNo].ToString(),
                            //ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),
                        };
                    }
                    if (leftQty <= 0)
                    {
                        break;
                    }
                }
            }

            return taskItemDict.Values.ToList();
        }

        private void ComputeWaveDetail_bak1(Dictionary<string, object> param, string target,
            List<string> allocatedLocations, ref string lastLocation, DataRow row)
        {
            string groupNo = row["order_seq"].ToString();
            decimal planQty = decimal.Parse("0" + row["plan_qty"].ToString())
                            - decimal.Parse("0" + row["processed_qty"].ToString());
            decimal leftQty = planQty;

            DataRow[] orderHeadRows = OutboundOrder.GetOrderHeads(row["outbound_h_id"].ToString());
            DataRow orderDetailRow = OutboundOrder.GetDetail(row["outbound_d_id"].ToString());
            string targetType = orderHeadRows[0]["target_type"].ToString();
            if (targetType == "Address_Dock")
            {
                DataRow dockRow = Dock.GetDock(orderHeadRows[0]["target_id"].ToString());
                targetType = dockRow["type_code"].ToString();
            }
            else if (targetType == "Address_Station")
            {
                DataRow equipRow = Equipment.GetEquipment(orderHeadRows[0]["target_id"].ToString());
                targetType = equipRow["type_code"].ToString();
            }
            else
            {
                targetType = "";
            }

            string itemID = string.Empty;
            string partialFlag = "0";
            DataRow itemRow = Item.Get(row["item_id"].ToString());
            decimal pickingZoneQty = 0;

            if (string.IsNullOrWhiteSpace(itemID) || itemID != itemRow["t_id"].ToString())
            {
                lastLocation = "";
            }

            while (leftQty > 0)
            {
                string location = null;

                if (targetType == "Equipment_StationOut_Picking" ||
                    targetType == "Dock_Picking")
                {
                    leftQty = PickingZone.HandlePickingZoneBeforeAllocation(row, groupNo, leftQty);
                    if (leftQty <= 0) break;
                }

                DataRow locRow = null;
                try
                {
                    param = param.Concat(new Dictionary<string, object>()
                    {
                        { "I_BARCODE", orderDetailRow["exist_barcode"].ToString() },
                        { "I_BATCH_NO", orderDetailRow["lot_no"].ToString() },
                        { "I_LASTLOCATION_NO", lastLocation },
                        { "I_FRACTION", partialFlag },
                    }).ToDictionary(k => k.Key, k => k.Value);

                    location = LocationAllocator.AllocateInventory(param);
                    locRow = Location.Get(location);
                    location = locRow["t_id"].ToString();
                }
                catch (WmsException ex)
                {
                    if (ex.ErrorCode == -605)
                    {
                        pickingZoneQty = PickingZone.GetInventory(row["item_id"].ToString());
                        if (pickingZoneQty <= leftQty || (
                                targetType != "Equipment_StationOut_Picking" &&
                                targetType != "Dock_Picking" && itemRow["robot_enabled"].ToString() == "1"))
                        {
                            throw new WmsException(-605, $"货位分配失败！" +
                                $"已分配{allocatedLocations.Count()}个货位，" +
                                $"可能是没有可用物料{row["item_name"]}！", ex);
                        }
                    }
                    else throw;
                }

                DataRow inventoryRow = null;
                if (location != null)
                {
                    allocatedLocations.Add(location);
                    DataRow[] inventoryRows = Container.GetInventoryDetails(location,
                        row["item_id"].ToString(), orderDetailRow["lot_no"].ToString());
                    if (inventoryRows.Length > 1)
                    {
                        throw new WmsException(417, $"货位{locRow["location_no"]}存在多笔容器明细，无法处理！");
                    }
                    if (inventoryRows.Length < 1)
                    {
                        throw new WmsException(416, $"货位{locRow["location_no"]}不存在指定物料，无法处理！");
                    }
                    inventoryRow = inventoryRows[0];
                    /*
                    inventoryRow = Container.GetDetailByLocation(
                        location, row["item_id"].ToString(),
                        orderDetailRow["lot_no"].ToString());
                    */
                }

                decimal qty = leftQty;
                //if (targetType == "Equipment_StationOut_Picking" ||
                //    targetType == "Dock_Picking" || itemRow["robot_enabled"].ToString() != "1")
                if (targetType == "Equipment_StationOut_Picking" ||
                    targetType == "Dock_Picking")
                {
                    leftQty = PickingZone.HandlePickingZoneAfterAllocation(row, inventoryRow, groupNo, leftQty);
                }
                decimal availQty = decimal.Parse(inventoryRow["avail_qty"].ToString());
                if (partialFlag == "0" && locRow["storage_unit_use"].ToString() == "LocationUse_RackStock")
                {
                    if (leftQty < availQty)
                    {
                        partialFlag = "1";
                        continue;
                    }
                }
                if (qty == leftQty)
                {
                    decimal outQty = availQty < leftQty ? availQty : leftQty;
                    // 处理码段库存
                    /*
                    if (orderDetailRow["inventory_class"].ToString() == "WorkInProcess")
                    {
                        ProcessBarcodeNumber(orderDetailRow, itemRow, outQty);
                    }
                    */
                    // 保存波次任务
                    string taskType = string.IsNullOrWhiteSpace(target)
                                        // TODO ? TaskType.Outbound : TaskType.StockUp;
                                        ? TaskType.TaskType_OutBound.ToString() : "";

                    string taskNo = WmsHelper.GetSequence("TaskNo");
                    string priority = WmsConfig.Get("Task:Priority:Outbound");
                    DataRow equipRow = Equipment.GetStackByLocation(locRow["t_id"].ToString());

                    if (decimal.Parse("0" + inventoryRow["qty"]) < outQty)
                    {
                        LogHelper.LogDebug("qty={0}", inventoryRow["qty"]);
                        LogHelper.LogDebug("decimal.Parse(qty)={0}", decimal.Parse("0" + inventoryRow["qty"]));
                        LogHelper.LogDebug("outQty={0}", outQty);
                        throw new WmsException(418, "货位上物料数量不足!");
                    }

                    string targetID = "";
                    string targetText = "";
                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        DataRow targetLocRow = Location.Get(target);
                        targetID = targetLocRow["t_id"].ToString();
                        targetType = "AddressType_Location";
                        targetText = targetLocRow["location_name"].ToString();
                    }

                    string execType = string.Empty;
                    execType = GetTaskExecType(taskType);

                    InsertWaveTask(new WaveTaskModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),
                        WaveHeadID = row["wave_h_id"].ToString(),
                        WaveDetailID = row["t_id"].ToString(),
                        OutboundHeadID = row["outbound_h_id"].ToString(),
                        OutboundDetailID = row["outbound_d_id"].ToString(),
                        TaskTypeCode = taskType,
                        TaskExecTypeCode = execType,
                        TaskNo = taskNo, // WmsHelper.GetSequence("TaskNo"),
                        EquipmentID = equipRow?["t_id"].ToString(),
                        GroupNo = groupNo,
                        SeqNo = row["detail_seq"].ToString(),
                        //Priority = decimal.Parse(WmsConfig.Get("Task:Priority:Outbound")),
                        Priority = decimal.Parse(priority),
                        //ContainerID = locRow["container_id"].ToString(),
                        //ContainerID = inventoryRows[0]["t_id"].ToString(),
                        ContainerID = inventoryRow["container_d_id"].ToString(),
                        ContainerBarcode = inventoryRow["container_barcode"].ToString(),
                        ItemID = itemRow["t_id"].ToString(),
                        OperationQty = outQty,
                        StateCode = "WaveStatus_Editable",
                        LocationID = locRow["t_id"].ToString(),
                        BatchNo = inventoryRow["batch_no"].ToString(),
                        BomCode = "",
                        SourceID = locRow["t_id"].ToString(),
                        SourceType = "AddressType_Location",
                        SourceText = locRow["location_no"].ToString(),
                        TargetID = targetID,
                        TargetType = targetType,
                        TargetText = targetText,
                        Executable = 1,
                        DependenceTaskNo = "",
                        LedText = "",
                        PalletQty = Container.GetInventoryByLocation(
                            locRow["t_id"].ToString(), itemRow["t_id"].ToString()),
                    });
                    //InsertWaveTask(row, "TaskType_OutBound", locationID, groupNo, outQty);
                    //InsertWaveTask(row, taskType, location, groupNo, outQty);
                    Container.AddDetailOutQty(inventoryRow["container_d_id"].ToString(),
                        outQty, decimal.Parse("0" + inventoryRow["out_qty"]));
                    leftQty -= outQty;
                }
            }
            // 更新波次明细表
            UpdateDetailProcessedQty(row["t_id"].ToString(),
                planQty + decimal.Parse("0" + row["processed_qty"].ToString()));

            // 更新订单明细表中的已处理量
            OutboundOrder.AddDetailProcessedQty(row["outbound_d_id"].ToString(),
                planQty + decimal.Parse("0" + row["processed_qty"].ToString()));
        }

        private void ComputeWave4BarcodeNumber(string waveNo, DataRow[] detailRows)
        {
            string location = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");
            if (string.IsNullOrWhiteSpace(location))
            {
                return;
            }
            DataRow locRow = Location.Get(location);

            var invRows =
                detailRows.Where(r => r["inventory_class"].ToString() == "WorkInProcess")
                          .GroupBy(r => new
                          {
                              ItemID = r["item_id"].ToString(),
                              BatchNo = r["lot_no"].ToString(),
                              OrderHeadID = r["outbound_h_id"].ToString(),
                              OrderDetailID = r["outbound_d_id"].ToString(),
                          })
                          .Select(r => new
                          {
                              r.Key.ItemID,
                              r.Key.BatchNo,
                              r.Key.OrderHeadID,
                              r.Key.OrderDetailID,
                              PlanQty = r.Sum(r => decimal.Parse("0" + r["plan_qty"]))
                          })
                          .ToArray();

            foreach (var row in invRows)
            {
                decimal leftQty = row.PlanQty;
                DataRow orderHeadRow = OutboundOrder.GetOrder(row.OrderHeadID);
                DataRow orderDetailRow = OutboundOrder.GetDetail(row.OrderDetailID);
                DataRow itemRow = Item.Get(row.ItemID);

                while (leftQty > 0)
                {
                    WorkTask.AddOutboundTask(new WorkTaskModel()
                    {
                        SeqNo = 0,
                        // TODO ItemID = row.ItemID,
                        //RefDocNo = orderHeadRow["doc_no"].ToString(),
                        RefDocNo = waveNo,
                        //RefDocID = row.OrderDetailID,
                        SrcAddrID = locRow["t_id"].ToString(),
                        SrcAddrType = "AddressType_Location",
                        SrcAddrText = location,
                        RefDocType = "RefDocType_Wave",
                        TaskType = "TaskType_Barcode",
                        IsExecutable = "1",
                    }, new ContainerDetailModel()
                    {
                        LotNo = row.BatchNo,
                        ExistBarcode = "ExistBarcode_Exist",
                    }, ref leftQty);

                    if (leftQty > 0)
                    {
                        throw new WmsException(419, $"{itemRow["item_name"]}码段数量不足!");
                    }
                }
            }
        }

        private void ProcessBarcodeNumber(DataRow orderDetailRow, DataRow itemRow, decimal qty)
        {
            string barcodeLocCode = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");
            DataRow barcodeLoc = Location.Get(barcodeLocCode);
            DataRow containerRow = Container.GetHead(barcodeLoc["location_no"].ToString());

            DataRow[] barcodeDetails = Container.GetInventoryDetails(
                //containerRow["m_container_id"].ToString(),
                barcodeLoc["t_id"].ToString(),
                itemRow["t_id"].ToString(),
                orderDetailRow["lot_no"].ToString());

            barcodeDetails = barcodeDetails.OrderBy(r => r["first_inbound_dt"].ToString()).ToArray();

            decimal leftQty = qty;
            foreach (DataRow row in barcodeDetails)
            {
                decimal availQty = decimal.Parse(row["avail_qty"].ToString());
                decimal outQty = availQty < leftQty ? availQty : leftQty;
                Container.AddDetailOutQty(row["container_d_id"].ToString(),
                    outQty, decimal.Parse("0" + row["out_qty"].ToString()));
                leftQty -= outQty;
                if (leftQty <= 0)
                {
                    break;
                }
            }
            if (leftQty > 0)
            {
                throw new WmsException(419, $"{itemRow["item_name"]}码段数量不足!");
            }
        }

        private DataRow[] CheckBeforeCompute(DataRow headRow)
        {
            // 检查参数
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Finished.ToString()))
            {
                throw new WmsException(411, "波次已经完成，无需再次分配!");
            }
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Computing.ToString()))
            {
                throw new WmsException(412, "波次正在进行预分配计算! 请勿同时重复操作！");
            }

            DataRow[] detailRows;
            try
            {
                detailRows = GetWaveDetailsByHeadID(headRow["t_id"].ToString());
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode == DBHelper.NotFound)
                {
                    throw new WmsException(413, "该波次无明细可分配!");
                }
                throw;
            }

            int cnt = detailRows.Where(r => decimal.Parse("0" + r["plan_qty"])
                                            > decimal.Parse("0" + r["processed_qty"])).Count();
            if (cnt == 0)
            {
                throw new WmsException(413, "该波次无明细可分配!");
            }

            DataRow orderHeadRow = OutboundOrder.GetOrder(detailRows.First()[wd.OutboundHeadID].ToString());
            WebJsonInfo retInfo = WorkTaskInterface.GetBatchNo(
                orderHeadRow[oh.WarehouseID].ToString(),
                orderHeadRow[oh.DestAddrText].ToString(),
                orderHeadRow[oh.ProdLotNo].ToString());
            if (!retInfo.isSucceed || retInfo.errcode != 0)
            {
                throw new WmsException(500, "向WCS查询生产批次出错：" + retInfo.errmsg);
            }
            string check = WmsConfig.GetOrDefault("Outbound:CheckProductionLot", "0");
            if (check == "1")
            {
                string prodLotNo = orderHeadRow[oh.ProdLotNo].ToString();
                if (string.IsNullOrWhiteSpace(retInfo.json.ToString()))
                {
                    throw new WmsException(500, "从WCS获取的生产批次不能为空!");
                }
                if (string.IsNullOrWhiteSpace(prodLotNo))
                {
                    throw new WmsException(500, "出库单的生产批次不能为空!");
                }
                // WCS获取的批次信息不含有特殊符号，如#
                string batchNo = retInfo.json.ToString().Replace("#", "");
                if (batchNo != retInfo.json.ToString())
                {
                    throw new WmsException(500, "WCS的当前生产批次和本出库单的生产批次不一致!");
                }
            }
            return detailRows;
        }

        public IWave ComputeWave(string headNoOrID, string type)
        {
            if (string.IsNullOrWhiteSpace(headNoOrID)
                || string.IsNullOrWhiteSpace(type))
            {
                throw new WmsException(500, "参数不能为空，停止处理");
            }


            DataRow headRow = GetWaveHead(headNoOrID);

            //ComputeWave(headNoOrID, out List<string> allocatedLocations);
            ComputeWave(headRow, "", "", out List<string> allocatedLocations);
            return this;
        }

        public IWave ComputeWaveWithRollbackLocationState(string headNoOrID, string type)
        {
            if (string.IsNullOrWhiteSpace(headNoOrID)
                || string.IsNullOrWhiteSpace(type))
            {
                throw new WmsException(500, "参数不能为空，停止处理");
            }

            List<string> allocatedLocations = new List<string>();
            DataRow headRow = GetWaveHead(headNoOrID);

            try
            {
                //ComputeWave(headNoOrID, out allocatedLocations);
                ComputeWave(headRow, "", "", out allocatedLocations);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);

                DBHelper.EndTransaction(false);
                Location.RestoreLocationStockState(allocatedLocations, out List<string> failureList,
                new Dictionary<string, object>(){
                    { LocationStatus.LocationStatus_Out.ToString(), LocationStatus.LocationStatus_Stored.ToString() }
                });
                if (failureList.Count > 0)
                {
                    throw new WmsException(-406, $"货位[{string.Join(",", failureList)}]状态未能恢复!", ex);
                }
                DBHelper.BeginTransaction();
                throw;
            }
            return this;
        }

        public IWave ComputeWave(string headNoOrID, out List<string> allocatedLocations)
        {
            allocatedLocations = new List<string>();

            // 检查参数
            DataRow headRow = GetWaveHead(headNoOrID);

            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Finished.ToString()))
            {
                throw new WmsException(-630, "波次已经完成，无法进行计算!");
            }
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Computing.ToString()))
            {
                throw new WmsException(-630, "波次正在进行计算! 请勿同时重复计算！");
            }
            headNoOrID = headRow["t_id"].ToString();
            /*
            // 波次可以多次计算/释放，所以不能简单的用状态来判断 
            if (!headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_New.ToString()))
            {
                info.ErrorCode = -630;
                throw new Exception("该波次的状态表明不能进行计算!");
            }
            */
            DataRow[] detailRows;
            try
            {
                detailRows = GetWaveDetailsByHeadID(headNoOrID);
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode == DBHelper.NotFound)
                {
                    throw new WmsException(-634, "该波次无明细可计算!");
                }
                throw;
            }

            int cnt = detailRows.Where(r => decimal.Parse("0" + r["plan_qty"])
                                            > decimal.Parse("0" + r["processed_qty"])).Count();
            if (cnt == 0)
            {
                throw new WmsException(-634, "该波次无明细可计算!");
            }
            /*
            info = new PYOutboundOrder().GetOrderHeadsByWave(headID, out DataRow[] orderHeadRows);
            if (info.ErrorCode != 0) return info;
            if (orderHeadRows.Length == 0)
            {
                info.ErrorCode = -653;
                throw new Exception("找不到订单!");
            }

            if (orderHeadRows[0].Table.AsEnumerable().GroupBy(s => s["target_id"].ToString()).Count() > 1)
            {
                info.ErrorCode = -651;
                throw new Exception("您选中的出库单中含有多个目标地址，不能进行波次计算!");
            }
            */

            //var service = ServiceLocator.GetService<ISequenceService>();

            string groupSeq = string.Empty;
            string lastOrder = string.Empty;

            //IOutboundOrder order = OutboundOrder;
            //IOutboundRule rule = OutboundRule;
            //IContainer container = WmsFactory.MakeContainer();
            //CPPickingZone pickingZone = new CPPickingZone();

            // 更新波次头表
            UpdateHeadStatus(headNoOrID, WaveStatus.WaveStatus_Computing.ToString());

            foreach (DataRow row in detailRows)
            {
                ComputeWaveDetail(allocatedLocations, row);
            }
            // 更新波次头表
            UpdateHeadStatus(headNoOrID, "WaveStatus_Editable");

            return this;
        }

        private void ComputeWaveDetail(List<string> allocatedLocations, DataRow row)
        {
            /*
            //if (!groupSeq.Equals(row["group_seq"].ToString())
            //    || lastOrder != row["outbound_h_id"].ToString())
            if (groupSeq != row["group_seq"].ToString()
                //|| lastOrder != row["order_seq"].ToString())
                || lastOrder != row["item_id"].ToString())
            {
                groupNo = WmsHelper.GetSequence("SWMS_GroupNo");
                if (groupNo.Contains("错误"))
                {
                    throw new WmsException(-611, groupNo);
                }
                groupSeq = row["group_seq"].ToString();
                //lastOrder = row["order_seq"].ToString();
                lastOrder = row["item_id"].ToString();
            }
            */
            string groupNo = row["order_seq"].ToString();
            decimal planQty = decimal.Parse("0" + row["plan_qty"].ToString())
                            - decimal.Parse("0" + row["processed_qty"].ToString());
            decimal leftQty = planQty;

            DataRow[] orderHeadRows = OutboundOrder.GetOrderHeads(row["outbound_h_id"].ToString());
            string targetType = orderHeadRows[0]["target_type"].ToString();
            if (targetType == "Address_Dock")
            {
                DataRow dockRow = Dock.GetDock(orderHeadRows[0]["target_id"].ToString());
                targetType = dockRow["type_code"].ToString();
            }
            else if (targetType == "Address_Station")
            {
                DataRow equipRow = Equipment.GetEquipment(orderHeadRows[0]["target_id"].ToString());
                targetType = equipRow["type_code"].ToString();
            }
            else
            {
                targetType = "";
            }

            DataRow itemRow = Item.Get(row["item_id"].ToString());
            decimal pickingZoneQty = 0;
            while (leftQty > 0)
            {
                string locationID = null;

                if (targetType == "Equipment_StationOut_Picking" ||
                    targetType == "Dock_Picking")
                {
                    leftQty = PickingZone.HandlePickingZoneBeforeAllocation(row, groupNo, leftQty);
                    if (leftQty <= 0) break;
                }

                try
                {
                    //locationID = OutboundRule.AllocateLocation(row["warehouse_id"].ToString(),
                    //    row["item_id"].ToString(), "Location_TaskOutBound",
                    //    "UserDefined_WaveLocationAssign_Mode1",
                    //    "CheckState_CheckAvoid;CheckState_CheckPassed",
                    //    orderHeadRows[0]["shipper"].ToString(), string.Empty);
                    locationID = LocationAllocator.AllocateInventory(
                        row["warehouse_id"].ToString(),
                        row["item_id"].ToString(),
                        "None");

                    DataRow locRow = Location.Get(locationID);
                    locationID = locRow["t_id"].ToString();
                }
                catch (WmsException ex)
                {
                    if (ex.ErrorCode == -605)
                    {
                        pickingZoneQty = PickingZone.GetInventory(row["item_id"].ToString());
                        if (pickingZoneQty <= leftQty || (
                                targetType != "Equipment_StationOut_Picking" &&
                                targetType != "Dock_Picking" && itemRow["robot_enabled"].ToString() == "1"))
                        {
                            throw new WmsException(-605, $"货位分配失败！" +
                                $"已分配{allocatedLocations.Count()}个货位，" +
                                $"可能是没有可用物料{row["item_name"]}！", ex);
                        }
                    }
                    else throw;
                }

                DataRow inventoryRow = null;
                if (locationID != null)
                {
                    allocatedLocations.Add(locationID);
                    DataRow[] inventoryRows = Container.GetInventoryDetails(locationID, row["item_id"].ToString());
                    if (inventoryRows.Length > 1)
                    {
                        throw new WmsException(-643, $"货位{locationID}存在多个托盘，无法处理！");
                    }
                    if (inventoryRows.Length < 1)
                    {
                        throw new WmsException(-643, $"货位{locationID}找不到托盘，无法处理！");
                    }
                    inventoryRow = inventoryRows[0];
                }

                decimal qty = leftQty;
                //if (targetType == "Equipment_StationOut_Picking" ||
                //    targetType == "Dock_Picking" || itemRow["robot_enabled"].ToString() != "1")
                if (targetType == "Equipment_StationOut_Picking" ||
                    targetType == "Dock_Picking")
                {
                    leftQty = PickingZone.HandlePickingZoneAfterAllocation(row, inventoryRow, groupNo, leftQty);
                }
                if (qty == leftQty)
                {
                    // 保存波次任务
                    decimal availQty = decimal.Parse(inventoryRow["avail_qty"].ToString());
                    decimal outQty = availQty < leftQty ? availQty : leftQty;
                    //InsertWaveTask(row, "TaskType_OutBound", locationID, groupNo, outQty);
                    InsertWaveTask(row, TaskType.TaskType_OutBound.ToString(), locationID, groupNo, outQty);
                    leftQty -= outQty;
                }
            }
            // 更新波次明细表
            UpdateDetailProcessedQty(row["t_id"].ToString(),
                planQty + decimal.Parse("0" + row["processed_qty"].ToString()));

            // 更新订单明细表中的已处理量
            OutboundOrder.AddDetailProcessedQty(row["outbound_d_id"].ToString(),
                planQty + decimal.Parse("0" + row["processed_qty"].ToString()));
        }

        public IWave ComputeWaveDetail(string detailID, string room)
        {
            if (string.IsNullOrWhiteSpace(detailID)
                || string.IsNullOrWhiteSpace(room))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow row = GetWaveDetail(detailID);
            decimal planQty = decimal.Parse("0" + row["plan_qty"]);
            decimal processedQty = decimal.Parse("0" + row["processed_qty"]);
            if (planQty <= processedQty)
            {
                throw new WmsException(420, "该明细已无数量可处理!");
            }

            string headNoOrID = row["wave_h_id"].ToString();
            DataRow headRow = GetWaveHead(headNoOrID);

            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Finished.ToString()))
            {
                throw new WmsException(-630, "波次已经完成，无法进行处理!");
            }
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Computing.ToString()))
            {
                throw new WmsException(-630, "波次正在进行计算! 无法进行处理！");
            }

            UpdateHeadStatus(headNoOrID, WaveStatus.WaveStatus_Computing.ToString());

            List<string> allocatedLocations = new List<string>();

            string saftyPeriod = WmsConfig.GetOrDefault("Inventory:SaftyPeriod", "120");
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "I_WAREHOUSEID", row["warehouse_id"].ToString() },
                { "I_ITEMID", row["item_id"].ToString() },
                { "I_STORAGE_LOCGP1", "" },
                { "I_STORAGE_LOCGP2", "" },
                { "I_EXPIRY_DATE", saftyPeriod },
                { "I_ZONE", "" },
                { "I_HEADID", "" },
                { "I_DETAILID", "" },
                { "I_PACKING_TYPE", "" },
                { "I_ROOM_NO", room },
            };

            decimal seqNo = 1;
            ComputeWaveDetail(param, null, allocatedLocations, row, ref seqNo);

            UpdateHeadStatus(headNoOrID, "WaveStatus_Editable");

            return this;
        }

        public void AddWaveDetailByBomNew(DataRow orderHeadRow,
            string waveHeadID, ref decimal groupSeq)
        {
            // 取订单明细
            DataRow[] orderDetailRows = GetOrderDetailsByOrders(orderHeadRow[oh.ID].ToString());
            orderDetailRows = orderDetailRows.OrderBy(r => decimal.Parse("0" + r[od.SeqNo])).ToArray();

            // 取BOM明细
            DataRow[] bomRows = Bom.GetFirstLevelDetail(orderHeadRow[oh.BomCode].ToString());

            decimal count = decimal.Parse("0" + orderDetailRows.FirstOrDefault()[od.PlanQty]);
            while (count > 0)
            {
                groupSeq++;
                decimal lastOrderSeq = -1;
                string lastOrderHeadID = string.Empty;
                foreach (DataRow orderDetailRow in orderDetailRows)
                {
                    OutboundOrder.AddDetailProcessedQty(orderDetailRow[od.ID].ToString(),
                        decimal.Parse("0" + orderDetailRow[od.PlanQty]));

                    //decimal orderSeq = decimal.Parse("0" + orderDetailRow[od.SeqNo].ToString());
                    //string orderHeadID = orderDetailRow[od.OutboundHeadID].ToString();
                    //if (orderSeq != lastOrderSeq || !orderHeadID.Equals(lastOrderHeadID))
                    //{
                    //    lastOrderSeq = orderSeq;
                    //    lastOrderHeadID = orderHeadID;
                    //    groupSeq++;
                    //}

                    decimal qty = decimal.Parse("0" + orderDetailRow[od.PlanQty].ToString());
                    if (qty > 0)
                    {
                        DataRow[] rows = bomRows.Where(r => r[bd.ItemID].ToString() == orderDetailRow[od.ItemID].ToString()).ToArray();
                        decimal plan_qty = 0;
                        if (rows.Length == 0)
                        {
                            plan_qty = qty;
                        }
                        else
                        {
                            plan_qty = decimal.Parse(rows[0]["qty"].ToString());
                        }

                        InsertWaveDetailByOrderDetail(waveHeadID,
                                groupSeq.ToString(), orderDetailRow[od.SeqNo].ToString(),
                            orderDetailRow[od.ID].ToString(), plan_qty);

                        qty -= plan_qty;
                        orderDetailRow[od.PlanQty] = qty;
                    }
                }
                count = decimal.Parse("0" + orderDetailRows.FirstOrDefault()[od.PlanQty]);
            }
        }

        public void AddWaveDetailByBom(DataRow[] todoRows,
            string waveHeadID, ref decimal groupSeq)
        {
            if (todoRows.Length <= 0) return;

            // 取BOM明细
            DataRow[] bomRows = OutboundOrder
                .GetBomDetail(todoRows[0]["bom_code"].ToString());

            bool cont = true;

            var t_id = SnowflakeID.Guid();
            while (cont)
            {
                cont = false;
                for (int i = 0; i < todoRows.Length; i++)
                {
                    decimal qty = decimal.Parse("0" + todoRows[i]["plan_qty"].ToString());
                    if (qty > 0)
                    {
                        DataRow[] row = bomRows[0].Table.Select("item_id = '" + todoRows[i]["item_id"].ToString() + "'");
                        if (row.Length > 1)
                        {
                            throw new WmsException(-611,
                                $"配方单{row[0]["bom_code"]}物料[{row[0]["item_name"]}]重复!");
                        }
                        decimal plan_qty = 0;
                        if (row.Length == 0)
                        {
                            plan_qty = qty;
                        }
                        else
                        {
                            plan_qty = decimal.Parse(row[0]["qty"].ToString());
                        }

                        InsertWaveDetailByOrderDetail(waveHeadID,
                                groupSeq.ToString(), todoRows[i]["order_item_seq"].ToString(),
                            todoRows[i]["t_id"].ToString(), plan_qty);

                        qty -= plan_qty;
                        todoRows[i]["plan_qty"] = qty;
                        if (qty > 0) cont = true;
                    }
                }
                groupSeq++;
            }

            //return this;
        }

        public void AddWaveDetailByOrderDetail(string orderHeadIds,
            string waveHeadID, decimal groupSeq)
        {
            // 取订单明细
            //DataRow[] orderDetailRows = GetOrderDetailsNotInWave(orderHeadIds);
            DataRow[] orderDetailRows = GetOrderDetailsByOrders(orderHeadIds);

            decimal lastOrderSeq = -1;
            string lastOrderHeadID = string.Empty;
            List<DataRow> todoRows = new List<DataRow>();
            foreach (DataRow row in orderDetailRows)
            {
                OutboundOrder.AddDetailProcessedQty(row[od.ID].ToString(),
                    decimal.Parse("0" + row[od.PlanQty]));
                decimal orderSeq = decimal.Parse("0" + row["order_seq"].ToString());
                string orderHeadID = row["outbound_h_id"].ToString();
                decimal itemSeq = decimal.Parse("0" + row["order_item_seq"].ToString());
                if (orderSeq != lastOrderSeq || !orderHeadID.Equals(lastOrderHeadID))
                {
                    lastOrderSeq = orderSeq;
                    lastOrderHeadID = orderHeadID;
                    groupSeq++;
                    if (todoRows.Count > 0)
                    {
                        // 根据BOM添加波次明细
                        todoRows.ToArray();
                        AddWaveDetailByBom(todoRows.ToArray(), waveHeadID, ref groupSeq);

                        todoRows.Clear();
                    }
                }
                //if (string.IsNullOrWhiteSpace(row["bom_code"].ToString()))
                if (string.IsNullOrWhiteSpace(row["consolidated"].ToString())
                    || row["consolidated"].ToString() != "1")
                {

                    //InsertWaveDetailByOrderDetail(waveHeadID,
                    //    groupSeq.ToString(), row["order_item_seq"].ToString(),
                    //    row["t_id"].ToString(), decimal.Parse(row["plan_qty"].ToString()));
                    AddWaveDetailByOrderDetail(waveHeadID, groupSeq.ToString(), row);
                    continue;
                }
                todoRows.Add(row);
            }
            if (todoRows.Count > 0)
            {
                // 根据BOM添加波次明细
                AddWaveDetailByBom(todoRows.ToArray(), waveHeadID, ref groupSeq);
            }
        }

        public void AddWaveDetailByOrderDetailNew(DataRow orderHeadRow,
            string waveHeadID, decimal groupSeq)
        {
            // 取订单明细
            //DataRow[] orderDetailRows = GetOrderDetailsNotInWave(orderHeadRow[oh.ID].ToString());
            DataRow[] orderDetailRows = GetOrderDetailsByOrders(orderHeadRow[oh.ID].ToString());

            decimal lastOrderSeq = -1;
            string lastOrderHeadID = string.Empty;
            foreach (DataRow row in orderDetailRows)
            {
                OutboundOrder.AddDetailProcessedQty(row[od.ID].ToString(),
                    decimal.Parse("0" + row[od.PlanQty]));

                decimal orderSeq = decimal.Parse("0" + orderHeadRow[oh.SeqNo].ToString());
                string orderHeadID = row["outbound_h_id"].ToString();
                if (orderSeq != lastOrderSeq || !orderHeadID.Equals(lastOrderHeadID))
                {
                    lastOrderSeq = orderSeq;
                    lastOrderHeadID = orderHeadID;
                    groupSeq++;
                }
                AddWaveDetailByOrderDetail(waveHeadID, groupSeq.ToString(), row);
            }
        }

        public IWave CreateWaveV1(string waveNo, string orderIds)
        {
            // 检查参数
            var order = OutboundOrder;
            DataRow[] orderHeadRows = order.GetOrderHeads(orderIds);
            foreach (DataRow row in orderHeadRows)
            {
                // TODO 下面对目标地址和车牌的设置改为用配置
                //if (string.IsNullOrWhiteSpace(row["target_id"].ToString()))
                //{
                //    throw new WmsException(-603,
                //        "订单" + row["doc_no"].ToString() + "没有设置目标地址！");
                //}
                //if (string.IsNullOrWhiteSpace(row["license_plate"].ToString()))
                //{
                //    throw new WmsException(430,
                //        "订单" + row["doc_no"].ToString() + "没有设置车牌！");
                //}
                //if (row["wave_status"].ToString().Contains("Order_New,Order_InWave"))
                //if (row[oh.OutboundStatus].ToString().Contains("OutDocStatus_New,OutDocStatus_Preallocation"))
                // 上面备份的语句表明以前的程序有错，但是从来没有出过错，下面的程序还有必要存在么？
                if (!new List<string> {
                    OutDocStatus.OutDocStatus_New.ToString(),
                    OutDocStatus.OutDocStatus_Preallocation.ToString(),
                }.Contains(row[oh.OutboundStatus].ToString()))
                {
                    throw new WmsException(431,
                        "订单" + row["doc_no"].ToString() + "的状态不允许创建！");
                }
                //if (row["outservice_type"].ToString() != "YXOutServiceType_TransferOutOfStock"
                //    && row["doc_type"].ToString() == "NoneCode")
                //{
                //    throw new WmsException(432, "订单" + row["doc_no"].ToString() 
                //        + "的业务类型和单据类型有冲突！只有调拨出库可以出无码烟！");
                //}
                // 20191213 方向东说客户不需要改检查了
                /*
                if (row["outservice_type"].ToString() != "YXOutServiceType_TransferOutOfStock")
                {
                    DataRow[] detailRows = OutboundOrder.GetDetails(row["t_id"].ToString());
                    detailRows = detailRows.Where(
                        r => r["inventory_class"].ToString() == "NoneCode").ToArray();
                    if (detailRows.Length > 0)
                    {
                        throw new WmsException(432, "订单" + row["doc_no"].ToString()
                            + "的业务类型和库存类别有冲突！只有调拨出库可以出无码烟！");
                    }
                }
                */
            }

            //if (orderHeadRows.Select(r => r["target_id"].ToString()).Distinct().Count() > 1)
            if (orderHeadRows.Select(r => r["license_plate"].ToString()).Distinct().Count() > 1)
            {
                throw new WmsException(603, "所选订单具有多个车牌！");
            }

            try
            {
                DataRow waveHeadRow = GetWaveHead(waveNo);
                throw new WmsException(601, "波次已存在!");
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode != DBHelper.NotFound) throw ex;
            }

            // 业务处理
            // 更新订单头表
            order.UpdateOrderHeadState(orderIds, OutDocStatus.OutDocStatus_InWave.ToString());

            string waveHeadID = SnowflakeID.Guid().ToString();
            decimal groupSeq = 0;

            AddWaveDetailByOrderDetail(orderIds, waveHeadID, groupSeq);

            // 插入波次头表
            InsertWaveHeadByDetail(waveHeadID, waveNo);

            return this;
        }

        public IWave CreateWave(string waveNo, string orderIds)
        {
            if (string.IsNullOrWhiteSpace(waveNo)
                || string.IsNullOrWhiteSpace(orderIds))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            // 检查参数
            var order = OutboundOrder;
            DataRow[] orderHeadRows = order.GetOrderHeads(orderIds);
            foreach (DataRow row in orderHeadRows)
            {
                // TODO 下面对目标地址和车牌的设置改为用配置
                //if (string.IsNullOrWhiteSpace(row["target_id"].ToString()))
                //{
                //    throw new WmsException(-603,
                //        "订单" + row["doc_no"].ToString() + "没有设置目标地址！");
                //}
                //if (string.IsNullOrWhiteSpace(row["license_plate"].ToString()))
                //{
                //    throw new WmsException(430,
                //        "订单" + row["doc_no"].ToString() + "没有设置车牌！");
                //}
                //if (row["wave_status"].ToString().Contains("Order_New,Order_InWave"))
                //if (row[oh.OutboundStatus].ToString().Contains("OutDocStatus_New,OutDocStatus_Preallocation"))
                // 上面备份的语句表明以前的程序有错，但是从来没有出过错，下面的程序还有必要存在么？
                if (!new List<string> {
                    OutDocStatus.OutDocStatus_New.ToString(),
                    OutDocStatus.OutDocStatus_Preallocation.ToString(),
                }.Contains(row[oh.OutboundStatus].ToString()))
                {
                    throw new WmsException(431,
                        "订单" + row["doc_no"].ToString() + "的状态不允许创建！");
                }
                //if (row["outservice_type"].ToString() != "YXOutServiceType_TransferOutOfStock"
                //    && row["doc_type"].ToString() == "NoneCode")
                //{
                //    throw new WmsException(432, "订单" + row["doc_no"].ToString() 
                //        + "的业务类型和单据类型有冲突！只有调拨出库可以出无码烟！");
                //}
                // 20191213 方向东说客户不需要改检查了
                /*
                if (row["outservice_type"].ToString() != "YXOutServiceType_TransferOutOfStock")
                {
                    DataRow[] detailRows = OutboundOrder.GetDetails(row["t_id"].ToString());
                    detailRows = detailRows.Where(
                        r => r["inventory_class"].ToString() == "NoneCode").ToArray();
                    if (detailRows.Length > 0)
                    {
                        throw new WmsException(432, "订单" + row["doc_no"].ToString()
                            + "的业务类型和库存类别有冲突！只有调拨出库可以出无码烟！");
                    }
                }
                */
            }

            //if (orderHeadRows.Select(r => r["target_id"].ToString()).Distinct().Count() > 1)
            if (orderHeadRows.Select(r => r["license_plate"].ToString()).Distinct().Count() > 1)
            {
                throw new WmsException(603, "所选订单具有多个车牌！");
            }

            try
            {
                DataRow waveHeadRow = GetWaveHead(waveNo);
                throw new WmsException(601, "波次已存在!");
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode != DBHelper.NotFound) throw ex;
            }

            // 业务处理
            // 更新订单头表
            //order.UpdateOrderHeadState(orderIds, OutDocStatus.OutDocStatus_InWave.ToString());

            string waveHeadID = SnowflakeID.Guid().ToString();
            decimal groupSeq = 0;

            foreach (DataRow row in orderHeadRows)
            {
                OutboundOrder.UpdateOrderHeadState(row[oh.ID].ToString(),
                    OutDocStatus.OutDocStatus_InWave.ToString(),
                    row[oh.OutboundStatus].ToString());
                if (row[oh.WaveCreationMode].ToString() == WaveCreationMode.WaveCreationMode_Normal.ToString())
                {
                    AddWaveDetailByOrderDetail(orderIds, waveHeadID, groupSeq);
                }
                else if (row[oh.WaveCreationMode].ToString() == WaveCreationMode.WaveCreationMode_ByBOM.ToString())
                {
                    string mode = WmsConfig.GetOrDefault("Outbound:WaveOrderByBOM", "ABAB");

                    if (mode == "ABAB")
                    {
                        AddWaveDetailByBomNew(row, waveHeadID, ref groupSeq);
                    }
                    else
                    {
                        AddWaveDetailByOrderDetail(orderIds, waveHeadID, groupSeq);
                    }
                }
                else
                {
                    throw new WmsException(500, "未实现的波次创建方式");
                }

            }

            // 插入波次头表
            InsertWaveHeadByDetail(waveHeadID, waveNo);

            return this;
        }

        public IWave DeleteWave(string waveHeadNoOrID)
        {
            if (string.IsNullOrWhiteSpace(waveHeadNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow waveHeadRow = GetWaveHead(waveHeadNoOrID);

            DataRow[] detailRows = null;
            try
            {
                detailRows = GetWaveDetailsByHeadID(waveHeadRow["t_id"].ToString());
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode != DBHelper.NotFound)
                {
                    throw ex;
                }
            }

            if (detailRows?.Length > 0)
            {
                string[] detailIds = detailRows.Select(r => r["t_id"].ToString()).ToArray();

                RemoveWaveDetails(string.Join(",", detailIds));
            }

            //DeleteWaveHead(waveHeadNoOrID);
            UpdateHeadActStatus(waveHeadNoOrID, "0", "1");

            return this;
        }

        public IWave FinishWave(string waveHeadNoOrID)
        {
            DataRow waveHeadRow = GetWaveHead(waveHeadNoOrID);

            //if (waveHeadRow["wave_status"].ToString().Trim() == WaveStatus.WaveStatus_Finished.ToString())
            //{
            //    return this;
            //}
            if (waveHeadRow[wh.WaveStatus].ToString() != WaveStatus.WaveStatus_Released.ToString())
            {
                throw new WmsException(100, "波次状态不正确");
            }

            UpdateHeadStatus(waveHeadNoOrID, WaveStatus.WaveStatus_Finished.ToString(),
                WaveStatus.WaveStatus_Released.ToString());

            var needNotify = WmsConfig.GetOrDefault("Outbound:NotifyWcsWhenWaveFinished", "0");
            if (needNotify == "1")
            {
                WorkTaskInterface.NotifyWaveFinished(waveHeadRow["wave_no"].ToString());
            }

            return this;
        }

        public IWave FinishWaveWhenTasksFinished(string waveNoOrID)
        {
            if (WorkTask.ExistsUnfinishedTaskInRefDoc(DocType.DocType_Wave.ToString(), waveNoOrID))
            {
                return this;
            }

            DataRow[] details = GetWaveDetailsByHeadID(waveNoOrID);
            foreach (DataRow row in details)
            {
                if (decimal.Parse("0" + row[wd.FinishedQty])
                    < decimal.Parse("0" + row[wd.PlanQty]))
                {
                    return this;
                }
            }

            FinishWave(waveNoOrID);

            return this;
        }
        private bool IsQtyFinished(string waveHeadID)
        {
            DataRow[] detailRows = GetWaveDetailsByHeadID(waveHeadID);
            foreach (DataRow detailRow in detailRows)
            {
                if (decimal.Parse("0" + detailRow[od.FinishedQty]) <
                                    decimal.Parse("" + detailRow[od.PlanQty]))
                {
                    return false;
                }
            }
            return true;
        }

        private void FinishWaveWhenQtyFinishedAndTaskFinished(string waveHeadID)
        {
            if (!WorkTask.IsFinished(WorkTask.GetByRefDocID(waveHeadID)))
            {
                return;
            }

            if (!IsQtyFinished(waveHeadID))
            {
                return;
            }

            FinishWave(waveHeadID);
        }

        public IWave FinishDetail(DataRow detailRow, decimal qty)
        {
            if (WmsConfig.GetOrDefault("Outbound:ComputeWaveByLot", "0") == "1")
            {
                decimal planQty = decimal.Parse("0" + detailRow[wd.PlanQty]);
                decimal finishedQty = decimal.Parse("0" + detailRow[wd.FinishedQty]);
                if (finishedQty >= planQty)
                {
                    DataRow[] detailRows = GetDetailsByItemID(detailRow[wd.ItemID].ToString());
                    detailRows = detailRows.Where(
                        r => r[wd.ID].ToString() != detailRow[wd.ID].ToString()
                        && decimal.Parse("0" + r[wd.PlanQty])
                            < decimal.Parse("0" + r[wd.FinishedQty])).ToArray();
                    if (detailRows.Length > 0)
                    {
                        detailRow = detailRows[0];
                    }
                }
            }

            AddDetailProcessedQty(detailRow[wd.ID].ToString(), qty);
            AddDetailFinishedQty(detailRow[wd.ID].ToString(), qty);
            OutboundOrder.FinishDetail(detailRow[wd.OutboundHeadID].ToString(),
                detailRow[wd.OutboundDetailID].ToString(), qty);

            FinishWaveWhenQtyFinishedAndTaskFinished(detailRow[wd.WaveHeadID].ToString());

            return this;
        }

        public IWave FinishDetail(string detailID, decimal qty)
        {
            DataRow detailRow = GetWaveDetail(detailID);

            FinishDetail(detailRow, qty);

            return this;
        }


        public IWave InsertWaveTask(DataRow waveDetailRow,
            string taskType, string locationID, string groupNo, decimal planQty, string taskNo)
        {
            //var t_id = SnowflakeID.Guid();

            //string priority = WmsConfig.GetStrategy("", "Outbound");
            string priority = WmsConfig.Get("Task:Priority:Outbound");

            //var service = ServiceLocator.GetService<ISequenceService>();
            //var taskNo = service.GetSeqNo(CurrentUser.Info.SessionKey, "TaskNo").Result;
            //if (taskNo.Contains("错误"))
            //{
            //    throw new WmsException(-611, taskNo);
            //}

            InsertWaveTask(waveDetailRow, taskType, locationID, groupNo, planQty,
                taskNo, waveDetailRow["detail_seq"].ToString(), priority);

            return this;
        }

        public Wave InsertWaveTask(DataRow waveDetailRow,
            string taskType, string locationNoOrID, string groupNo, decimal planQty,
            string taskNo, string seqNo, string priority)
        {
            DataRow locRow = Location.Get(locationNoOrID);
            /*
            if (locRow["location_state"].ToString() != "LocationState_Out")
            {
                throw new WmsException(-659, "货位状态不符");
            }
            if (locRow["location_state"].ToString() != "LocationState_Stored")
            {
                return new ResultInfo(-659, "货位状态表明该货位没有存储物料或物料不可用");
            }
            if (locRow["location_flag"].ToString() != "LocationFlag_Enabled"
                    && locRow["location_flag"].ToString() != "LocationFlag_Freeze")
            {
                return new ResultInfo(-659, "货位标记表明该货位不能进行出库");
            }
            */

            DataRow equipRow = Equipment.GetStackByLocation(locationNoOrID);

            DataRow itemRow = Item.Get(waveDetailRow["item_id"].ToString());

            DataRow[] inventoryRows = Container.GetInventoryDetails(
                locRow["t_id"].ToString(), waveDetailRow["item_id"].ToString());
            if (inventoryRows.Length == 0)
            {
                throw new WmsException(416,
                    $"货位{locRow["location_number"]}上不存在指定物料{itemRow["item_name"]}!");
            }
            else if (inventoryRows.Length > 1)
            {
                throw new WmsException(417, $"货位{locRow["location_number"]}存在多笔容器明细，无法处理！");
            }
            if (decimal.Parse("0" + inventoryRows[0]["qty"]) < planQty)
            {
                LogHelper.LogDebug("qty={0}", inventoryRows[0]["qty"]);
                LogHelper.LogDebug("decimal.Parse(qty)={0}", decimal.Parse("0" + inventoryRows[0]["qty"]));
                LogHelper.LogDebug("planQty={0}", planQty);
                throw new WmsException(418, "货位上物料数量不足!");
            }

            /*
                .GetContainerDetail(locRow["container_id"].ToString());
            DataRow[] containerDetailRows = WmsFactory.MakeContainer()

            int idx = containerDetailRows.Select(r => r["item_id"].ToString())
                            .ToList().IndexOf(waveDetailRow["item_id"].ToString());
            if (idx < 0)
            {
                throw new WmsException(-658, "货位上不存在指定物料!");
            }
            if (decimal.Parse("0" + containerDetailRows[idx]["qty"]) < planQty)
            {
                LogHelper.LogDebug("qty={0}", containerDetailRows[idx]["qty"]);
                LogHelper.LogDebug("decimal.Parse(qty)={0}", decimal.Parse("0" + containerDetailRows[idx]["qty"]));
                LogHelper.LogDebug("planQty={0}", planQty);
                throw new WmsException(-660, "货位上物料数量不足!");
            }
            */

            DataRow[] orderHeadRow = OutboundOrder
                .GetOrderHeads(waveDetailRow["outbound_h_id"].ToString());

            /*
            string priority = WmsConfig.GetStrategy("", "Outbound");

            var service = ServiceLocator.GetService<ISequenceService>();
            var taskNo = service.GetSeqNo(CurrentUser.Info.SessionKey, "TaskNo").Result;
            if (taskNo.Contains("错误"))
            {
                info.ErrorCode = -611;
                throw new Exception(taskNo);
            }
            */

            string targetID = orderHeadRow[0]["target_id"].ToString();
            string targetType = orderHeadRow[0]["target_type"].ToString();
            string targetText = orderHeadRow[0]["target_text"].ToString();
            bool isPicking = false;
            if (targetType == "Address_Dock")
            {
                DataRow dockRow = Dock.GetDock(targetID);
                if (dockRow["type_code"].ToString().Trim() == "Dock_Picking")
                {
                    isPicking = true;
                }
            }
            else
            {
                DataRow row = Equipment.GetEquipment(targetID);
                if (row?["type_code"].ToString().Trim() == "Equipment_StationOut_Picking")
                {
                    isPicking = true;
                }
            }
            //if (itemRow["robot_enabled"].ToString() != "1" && !isPicking)
            if (!isPicking)
            {
                //DataRow[] equipRows = Equipment.GetPickingStation();
                //if (equipRows.Length <= 0)
                //{
                //    throw new WmsException(-667, "找不到可用的人工拣选站台!");
                //}
                //targetID = equipRows[0]["t_id"].ToString();
                //targetType = "Address_Station";
                //targetText = equipRows[0]["equipment_number"].ToString();
            }
            //string addr = targetType.Equals("Address_Dock") ? "月台" : "站台";
            //string ledtext = $"品规：{itemRow["item_name"]};" +
            //$"数量：{planQty};单位：{itemRow["cname"]};{addr}：{targetText}";
            string ledtext = "";

            string execType = string.Empty;
            execType = GetTaskExecType(taskType);

            InsertWaveTask(new WaveTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),
                WaveHeadID = waveDetailRow["wave_h_id"].ToString(),
                WaveDetailID = waveDetailRow["t_id"].ToString(),
                OutboundHeadID = waveDetailRow["outbound_h_id"].ToString(),
                OutboundDetailID = waveDetailRow["outbound_d_id"].ToString(),
                TaskTypeCode = taskType,
                TaskExecTypeCode = execType,
                TaskNo = taskNo, // WmsHelper.GetSequence("TaskNo"),
                EquipmentID = equipRow?["t_id"].ToString(),
                GroupNo = groupNo,
                SeqNo = seqNo, //waveDetailRow["detail_seq"].ToString(),
                               //Priority = decimal.Parse(WmsConfig.Get("Task:Priority:Outbound")),
                Priority = decimal.Parse(priority),
                //ContainerID = locRow["container_id"].ToString(),
                //ContainerID = inventoryRows[0]["t_id"].ToString(),
                ContainerID = inventoryRows[0]["container_d_id"].ToString(),
                ContainerBarcode = inventoryRows[0]["container_barcode"].ToString(),
                ItemID = itemRow["t_id"].ToString(),
                OperationQty = planQty,
                StateCode = "WaveStatus_Editable",
                LocationID = locRow["t_id"].ToString(),
                BatchNo = inventoryRows[0]["batch_no"].ToString(),
                BomCode = orderHeadRow[0]["bom_code"].ToString(),
                SourceID = locRow["t_id"].ToString(),
                SourceType = "AddressType_Location",
                SourceText = locRow["location_no"].ToString(),
                TargetID = targetID,
                TargetType = targetType,
                TargetText = targetText,
                Executable = 1,
                DependenceTaskNo = "",
                LedText = ledtext,
                PalletQty = Container.GetInventoryByLocation(
                    locRow["t_id"].ToString(), itemRow["t_id"].ToString()),
            });

            return this;
        }

        public IWave InsertWaveTask(WaveTaskModel model)
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
                { "wave_h_id", model.WaveHeadID },
                { "wave_d_id", model.WaveDetailID },
                { "outbound_h_id", model.OutboundHeadID },
                { "outbound_d_id", model.OutboundDetailID },
                { "task_type_code", model.TaskTypeCode },
                { "task_exec_type_code", model.TaskExecTypeCode },
                { "task_no", model.TaskNo },
                { "equipment_id", model.EquipmentID },
                { "group_no", model.GroupNo },
                { "seq_no", model.SeqNo },
                { "priority", model.Priority.ToString() },
                { "container_id", model.ContainerID },
                { "container_barcode", model.ContainerBarcode },
                { "item_id", model.ItemID },
                { "operation_qty", model.OperationQty.ToString() },
                { "pallet_qty", model.PalletQty.ToString() },
                { "wave_status", model.StateCode },
                { "location_id", model.LocationID },
                { "batch_no", model.BatchNo },
                { "bom_code", model.BomCode },
                { "source_id", model.SourceID },
                { "source_type", model.SourceType },
                { "source_text", model.SourceText },
                { "target_id", model.TargetID },
                { "target_type", model.TargetType },
                { "target_text", model.TargetText },
                { "executable", model.Executable.ToString() },
                { "dependence_task_no", model.DependenceTaskNo },
                { "led_text", model.LedText },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into WMS_M_WAVE_TASK ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       wave_h_id, " +
                "       wave_d_id, " +
                "       outbound_h_id, " +
                "       outbound_d_id, " +
                "       task_type_code, " +
                "       task_exec_type_code, " +
                "       task_no, " +
                "       equipment_id, " +
                "       group_no, " +
                "       seq_no, " +
                "       priority, " +
                "       container_id, " +
                "       container_barcode, " +
                "       item_id, " +
                "       operation_qty, " +
                "       pallet_qty, " +
                "       wave_status, " +
                "       location_id, " +
                "       batch_no, " +
                "       bom_code, " +
                "       source_id, " +
                "       source_type, " +
                "       source_text, " +
                "       target_id, " +
                "       target_type, " +
                "       target_text, " +
                "       executable, " +
                "       dependence_task_no, " +
                "       led_text, " +
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
                "       @wave_h_id, " +
                "       @wave_d_id, " +
                "       @outbound_h_id, " +
                "       @outbound_d_id, " +
                "       @task_type_code, " +
                "       @task_exec_type_code, " +
                "       @task_no, " +
                "       @equipment_id, " +
                "       @group_no, " +
                "       @seq_no, " +
                "       @priority, " +
                "       @container_id, " +
                "       @container_barcode, " +
                "       @item_id, " +
                "       @operation_qty, " +
                "       @pallet_qty, " +
                "       @wave_status, " +
                "       @location_id, " +
                "       @batch_no, " +
                "       @bom_code, " +
                "       @source_id, " +
                "       @source_type, " +
                "       @source_text, " +
                "       @target_id, " +
                "       @target_type, " +
                "       @target_text, " +
                "       @executable, " +
                "       @dependence_task_no, " +
                "       @led_text, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "波次任务表");

            return this;
        }

        private string GetTaskExecType(string taskType)
        {
            if (taskType == TaskType.TaskType_OutBound.ToString())
            {
                return WmsConfig.GetOrDefault("Wave:NormalOutboundExecutor", "WCS");
            }
            /* TODO
            switch (taskType)
            {
                case TaskType.Outbound:
                    return WmsConfig.GetOrDefault("Wave:NormalOutboundExecutor", "WCS");
                    break;
                case TaskType.Filling:
                    return WmsConfig.GetOrDefault("Wave:FillingOutsideExecutor", "WCS");
                    break;
                case TaskType.Picking:
                    return WmsConfig.GetOrDefault("Wave:PickingOutsideExecutor", "RF");
                    break;
                case TaskType.StockUp:
                    return WmsConfig.GetOrDefault("Wave:NormalOutboundExecutor", "RF");
            }
            */
            return "";
        }

        public IWave InsertWaveTask(DataRow waveDetailRow,
            string taskType, string locationID, string groupNo, decimal planQty)
        {
            string taskNo = WmsHelper.GetSequence("TaskNo");
            InsertWaveTask(waveDetailRow, taskType, locationID, groupNo, planQty, taskNo);

            return this;
        }

        public IWave MergeWaveDetail(string groupNo, string detailIds)
        {
            throw new NotImplementedException();
        }

        public WaveTaskModel AddWaveTask(WaveTaskModel task,
            ContainerDetailModel detailModel, ref decimal leftQty)
        {
            DataRow containerRow = Container.GetHeadsByLocation(task.SourceID)[0];
            DataRow[] ctnDtlRows = null;
            if (string.IsNullOrWhiteSpace(detailModel.ID))
            {
                ctnDtlRows = Container
                .GetDetails(containerRow["m_container_id"].ToString(), task.ItemID)
                //.OrderBy(r => r["first_inbound_dt"].ToString())
                .ToArray();

                string byBatch = WmsConfig.GetOrDefault("Outbound:OutboundByBatch", "0");
                detailModel.LotNo ??= "";
                if (byBatch == "1")
                {
                    ctnDtlRows = ctnDtlRows.Where(r
                        => r["external_lotno"].ToString() == detailModel.LotNo
                        ).ToArray();
                }
                string byBarcode = WmsConfig.GetOrDefault("Outbound:OutboundByBarcode", "0");
                detailModel.ExistBarcode ??= "";
                if (byBarcode == "1")
                {
                    ctnDtlRows = ctnDtlRows.Where(r
                        => r["exist_barcode"].ToString() == detailModel.ExistBarcode
                        ).ToArray();
                }

                string orderFields = WmsConfig.GetOrDefault("Outbound:OrderFields", "first_inbound_dt");
                DataView dv = ctnDtlRows.CopyToDataTable().DefaultView;
                dv.Sort = orderFields;
                ctnDtlRows = dv.ToTable().AsEnumerable().ToArray();
            }
            else
            {
                DataRow row = Container.GetDetail(detailModel.ID);
                ctnDtlRows = new DataRow[] { row };
            }

            decimal sumQty = 0;
            foreach (DataRow ctnDtlRow in ctnDtlRows)
            {
                if (leftQty == 0) break;

                decimal actualQty = decimal.Parse("0" + ctnDtlRow["actual_qty"]);
                decimal outQty = decimal.Parse("0" + ctnDtlRow["out_qty"]);
                decimal lockQty = decimal.Parse("0" + ctnDtlRow["lock_qty"]);
                decimal availQty = actualQty - outQty - lockQty;
                decimal qty = 0;

                if (availQty == 0)
                {
                    continue;
                }
                if (leftQty > availQty)
                {
                    leftQty -= availQty;
                    qty = availQty;
                }
                else
                {
                    qty = leftQty;
                    leftQty = 0;
                }
                if (qty <= 0)
                {
                    throw new WmsException(111, "货位上物料数量不足，无法继续处理");
                }

                sumQty += qty;

                Container.AddDetailOutQty(ctnDtlRow["t_id"].ToString(),
                    qty, decimal.Parse("0" + ctnDtlRow["out_qty"]));

                task.ID = null;
                task.TaskNo = WmsHelper.GetSequence("TaskNo");
                task.OperationQty = qty;
                task.ContainerBarcode = containerRow["container_barcode"].ToString();
                task.StateCode = "TaskState_NotExecute";
                InsertWaveTask(task);
            }

            return task;
        }

        public IWave ModifyWaveTask(string taskNoOrID, decimal qty, string locationID)
        {
            if (string.IsNullOrWhiteSpace(locationID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }

            DataRow taskRow = GetWaveTask(taskNoOrID);
            if (taskRow["wave_status"].ToString() != "WaveStatus_Editable")
            {
                throw new WmsException(440, "波次任务状态不正确！无法继续!");
            }
            //if (taskRow["location_id"].ToString() == locationID)
            //{
            //    throw new WmsException(441, "请选择不同的货位!");
            //}

            DataRow waveDetailRow = GetWaveDetail(taskRow["wave_d_id"].ToString());

            decimal oldQty = decimal.Parse("0" + taskRow["operation_qty"]);
            Container.CancelStockingOut(taskRow["container_id"].ToString(), oldQty);

            ContainerDetailModel containerDetail = new ContainerDetailModel();
            containerDetail.InboundHeadID ??= "";
            containerDetail.ItemID = taskRow["item_id"].ToString();
            containerDetail.OutQty = qty;

            string oneRow4OneBatch = WmsConfig.GetOrDefault("Wave:OneRow4OneBatch", "0");
            containerDetail.LotNo ??= "";
            if (oneRow4OneBatch == "1")
            {
                DataRow ordDtlRow = OutboundOrder.GetDetail(taskRow["outbound_d_id"].ToString());
                containerDetail.LotNo = ordDtlRow["lot_no"].ToString();
            }

            string taskID = taskRow["t_id"].ToString();

            Container.Prepare4StockingOut(locationID, containerDetail);

            //Address addr = new Address(locationID);
            Address addr = new Address(AddressType.AddressType_Location, locationID);
            UpdateWaveTaskSource(taskID, addr, taskRow["source_id"].ToString());
            UpdateWaveTaskContainer(taskID, containerDetail.ID,
                addr.Text, taskRow["container_id"].ToString());
            UpdateWaveTaskLocation(taskID, locationID, taskRow["location_id"].ToString());

            DataRow ctnRow = Container.GetHead(containerDetail.ContainerID);
            decimal palletQty = qty;
            if (decimal.TryParse(ctnRow["packing_type"].ToString(), out decimal packingType))
            {
                if (packingType > 0)
                {
                    palletQty = decimal.Ceiling(qty / packingType);
                }
            }
            UpdateWaveTaskQty(taskID, qty, palletQty);

            AddDetailProcessedQty(waveDetailRow["t_id"].ToString(), qty - oldQty,
                decimal.Parse("0" + waveDetailRow["processed_qty"]));

            return this;
        }

        /// <summary>
        /// 2019/11/27 之前没有处理数量
        /// </summary>
        /// <param name="taskNoOrID"></param>
        /// <param name="qty"></param>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public IWave ModifyWaveTaskV1(string taskNoOrID, decimal qty, string locationID)
        {
            DataRow taskRow = GetWaveTask(taskNoOrID);
            if (taskRow["wave_status"].ToString() != "WaveStatus_Editable")
            {
                throw new WmsException(440, "波次任务状态不正确！无法继续!");
            }
            //if (taskRow["location_id"].ToString() == locationID)
            //{
            //    throw new WmsException(441, "请选择不同的货位!");
            //}

            DataRow waveDetailRow = GetWaveDetail(taskRow["wave_d_id"].ToString());

            ContainerDetailModel containerDetail = new ContainerDetailModel();
            containerDetail.InboundHeadID ??= "";
            containerDetail.ItemID = taskRow["item_id"].ToString();
            containerDetail.OutQty = qty;

            string oneRow4OneBatch = WmsConfig.GetOrDefault("Wave:OneRow4OneBatch", "0");
            containerDetail.LotNo ??= "";
            if (oneRow4OneBatch == "1")
            {
                DataRow ordDtlRow = OutboundOrder.GetDetail(taskRow["outbound_d_id"].ToString());
                containerDetail.LotNo = ordDtlRow["lot_no"].ToString();
            }

            string taskID = taskRow["t_id"].ToString();

            Container
                .CancelStockingOut(taskRow["container_id"].ToString(), qty)
                .Prepare4StockingOut(locationID, containerDetail);

            //Address addr = new Address(locationID);
            Address addr = new Address(AddressType.AddressType_Location, locationID);
            UpdateWaveTaskSource(taskID, addr, taskRow["source_id"].ToString());
            UpdateWaveTaskContainer(taskID, containerDetail.ID,
                addr.Text, taskRow["container_id"].ToString());
            UpdateWaveTaskLocation(taskID, locationID, taskRow["location_id"].ToString());

            // TODO 如果可以修改波次任务的数量，这里还要加上相应的处理

            return this;
        }

        public void InsertWorkTask(DataRow waveTaskRow, string id = null)
        {
            DataRow waveHeadRow = Wave.GetWaveHead(waveTaskRow["wave_h_id"].ToString());

            DataRow[] orderHeadRow = OutboundOrder.
                GetOrderHeads(waveTaskRow["outbound_h_id"].ToString());

            if (id == null)
            {
                id = SnowflakeID.Guid().ToString();
            }

            DataRow locRow = Location.Get(waveTaskRow["source_id"].ToString());

            string taskType = waveTaskRow["task_type_code"].ToString();
            if (!string.IsNullOrWhiteSpace(waveTaskRow["target_id"].ToString()))
            {
                taskType = "TaskType_Out4StockUp";
            }


            WorkTaskModel task = new WorkTaskModel()
            {
                ID = id,
                ZoneID = locRow["isa_bi_storage_zone_id"].ToString(),
                //TaskType = waveTaskRow["task_type_code"].ToString(),
                TaskType = taskType,
                //TaskExeTypeCode = waveTaskRow["task_exec_type_code"].ToString(),
                TaskNo = waveTaskRow["task_no"].ToString(),
                ActualEquipmentID = waveTaskRow["equipment_id"].ToString(),
                //GroupNo = waveTaskRow["group_no"].ToString(),
                //SeqNo = waveTaskRow["seq_no"].ToString(),
                SeqNo = decimal.Parse("0" + (waveTaskRow["seq_no"])),
                Priority = DBHelper.GetDecimal(waveTaskRow["priority"]),
                //ContainerID = waveTaskRow["container_id"].ToString(),
                //ContainerDetailID = waveTaskRow["container_id"].ToString(),
                ContainerBarcode = waveTaskRow["container_barcode"].ToString(),
                //ItemID = waveTaskRow["item_id"].ToString(),
                //OperationQty = DBHelper.GetDecimal(waveTaskRow["operation_qty"]),
                //PickQty = 0,
                //BatchNo = waveTaskRow["batch_no"].ToString(),
                //BomCode = waveTaskRow["bom_code"].ToString(),
                SrcAddrID = waveTaskRow["source_id"].ToString(),
                RefDocType = "RefDocType_Wave",
                SrcAddrType = waveTaskRow["source_type"].ToString(),
                SrcAddrText = waveTaskRow["source_text"].ToString(),
                DestAddrID = waveTaskRow["target_id"].ToString(),
                DestAddrType = waveTaskRow["target_type"].ToString(),
                DestAddrText = waveTaskRow["target_text"].ToString(),
                RefDocID = waveTaskRow["wave_d_id"].ToString(),
                RefDocNo = waveHeadRow["wave_no"].ToString(),
                //OrderID = waveTaskRow["outbound_d_id"].ToString(),
                //OrderNo = waveTaskRow["outbound_h_id"].ToString(),
                //SourceBillDetailID = waveTaskRow["order_d_id"].ToString(),
                //SourceBillDetailNo = orderHeadRow[0]["order_no"].ToString(),
                //TaskState = "TaskState_NotExecute",
                TaskStatus = "TaskState_WaitingExecute",
                IsExecutable = waveTaskRow["executable"].ToString(),
                //DependenceTaskNo = waveTaskRow["dependence_task_no"].ToString(),
                //LedText = waveTaskRow["led_text"].ToString(),
            };
            // TODO to process task_item table

            WorkTask.Insert(task);

            if (taskType == "TaskType_Out4StockUp")
            {
                DataRow ctnDtlRow = Container.GetDetail(waveTaskRow["container_id"].ToString());
                locRow = Location.Get(task.DestAddrID);
                DataRow containerRow = Container.GetHead(locRow["location_no"].ToString());
                DataRow orderDetailRow = OutboundOrder.GetDetail(waveTaskRow["outbound_d_id"].ToString());

                string ctnDetailID = SnowflakeID.Guid().ToString();
                ContainerDetailModel ctnDetailModel = new ContainerDetailModel()
                {
                    ID = ctnDetailID,
                    //ZoneID = locRow["isa_bi_storage_zone_id"].ToString(),
                    ContainerID = containerRow["m_container_id"].ToString(),
                    InboundHeadID = "",
                    ItemID = orderDetailRow["item_id"].ToString(),
                    ItemName = orderDetailRow["item_name"].ToString(),
                    UomQtyID = orderDetailRow["isa_wi_uom_qtyid"].ToString(),
                    UomStatID = orderDetailRow["isa_wi_uom_financeid"].ToString(),
                    LotNo = orderDetailRow["lot_no"].ToString(),
                    GivenYear = decimal.Parse("0" + orderDetailRow["givenyear"]),
                    Spec = orderDetailRow["speci"].ToString(),
                    //ExistBarcode = row["exist_barcode"].ToString(),
                    ExistBarcode = orderDetailRow["exist_barcode"].ToString(),
                    FirstInDate = (DateTime)ctnDtlRow["first_inbound_dt"],
                    QCStatus = ctnDtlRow[cd.QCStatus].ToString(),
                };

                decimal planQty = decimal.Parse("0" + waveTaskRow["operation_qty"]);
                decimal allocatedQty = Container.PreparePutaway(ctnDetailModel,
                    planQty, decimal.Parse("0" + locRow["qty_limit"]));
                if (allocatedQty <= 0)
                {
                    throw new WmsException(102, "入库数量错误，无法继续处理");
                }

                Container.AddDetailOutQty(ctnDetailModel.ID, planQty);

                task.ID = SnowflakeID.Guid().ToString();
                task.TaskType = "TaskType_OutBound";
                task.DependentTaskNo = task.TaskNo;
                task.TaskNo = WmsHelper.GetSequence("TaskNo");
                task.SrcAddrID = task.DestAddrID;
                task.SrcAddrText = task.DestAddrText;
                task.SrcAddrType = task.DestAddrType;
                task.DestAddrID = task.DestAddrText = task.DestAddrType = "";
                // TODO task.ContainerDetailID = ctnDetailModel.ID;
                //task.Priority = DBHelper.GetDecimal(waveTaskRow["priority"]);
                WorkTask.Insert(task);
            }
        }

        private void CheckQtyWithTask(DataRow waveHeadRow)
        {
            // 检查波次中的数量和任务中的数量是否一致
            DataRow[] taskItemRows = WorkTaskItem.GetByRefDocID(
                DocType.DocType_Wave.ToString(), waveHeadRow["t_id"].ToString());
            DataRow[] waveDetailRows = GetWaveDetailsByHeadID(waveHeadRow["t_id"].ToString());
            decimal detailTotal;
            decimal itemTotal;
            foreach (DataRow waveDetailRow in waveDetailRows)
            {
                //var waveDetailRow = Wave.GetDetail
                detailTotal = decimal.Parse("0" + waveDetailRow["plan_qty"]);
                itemTotal = taskItemRows
                    .Where(r => r[ti.RefDocDetailID].ToString() == waveDetailRow["t_id"].ToString()
                                && string.IsNullOrWhiteSpace(r[t.DependentTaskNo].ToString()))
                    .Sum(r => decimal.Parse("0" + r["plan_qty"]));
                if (detailTotal != itemTotal)
                {
                    LogHelper.LogInformation($"order[{detailTotal}], TaskItem[{itemTotal}]");
                    throw new WmsException(230, waveDetailRow["item_name"] + "预分配的数量与入库明细数量不符");
                }
            }
        }

        private readonly string checkQtyWhenRelease = WmsConfig.GetOrDefault("Outbound:CheckQtyWhenRelease", "1");
        //public IWave ReleaseWave(string headNoOrID)
        public IWave ReleaseWave(string headNoOrID, string stockUpLocID, out WebJsonInfo info)
        {
            if (string.IsNullOrWhiteSpace(headNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow headRow = GetWaveHead(headNoOrID);
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Computing.ToString()))
            {
                throw new WmsException(-630, "波次正在进行预分配! 不能确认!");
            }
            /*
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Released.ToString()))
            {
                return new ResultInfo(-631, "波次已经释放，不能再次释放!");
            }
            */

            //decimal leftTaskQty = decimal.Parse("0" + headRow["left_task_qty"].ToString());

            List<string> idList = new List<string>();
            DataRow[] taskRows = WorkTask.GetByRefDocID(headRow["t_id"].ToString());
            CheckTaskAtDestAddrWhenRelease(taskRows);

            foreach (DataRow taskRow in taskRows)
            {
                WorkTask.StartTask(taskRow);
                idList.Add(taskRow[t.ID].ToString());
            }
            AddHeadLeftTaskQty(headRow[wh.ID].ToString(), taskRows.Length);

            if (checkQtyWhenRelease == "1")
            {
                CheckQtyWithTask(headRow);
            }

            // 更新波次头表
            UpdateHeadReleaseInfo(headNoOrID,
                Context.UserInfo.UserName, DateTime.Now.ToString());
            UpdateHeadStatus(headNoOrID,
                WaveStatus.WaveStatus_Released.ToString(),
                headRow["wave_status"].ToString());

            // 更新出库订单状态
            DataRow[] detailRows = GetWaveDetailsByHeadID(headRow["t_id"].ToString());
            string[] orderIds = detailRows.Select(r => r["outbound_h_id"].ToString()).ToArray();
            OutboundOrder.UpdateOrderHeadState(string.Join(",", orderIds),
                OutDocStatus.OutDocStatus_Allocated.ToString(), OutDocStatus.OutDocStatus_Preallocation.ToString());

            //UpdateHeadLeftTaskQty(headNoOrID, leftTaskQty);

            info = WorkTaskInterface.Send(idList);

            return this;
        }

        private readonly string checkTaskAtDestAddrWhenRelease 
            = WmsConfig.GetOrDefault("Outbound:CheckTaskAtDestAddrWhenRelease", "1");
        private void CheckTaskAtDestAddrWhenRelease(DataRow[] taskRows)
        {
            if (checkTaskAtDestAddrWhenRelease == "0")
            {
                return;
            }
            List<Address> addrList = taskRows.Select(r => new Address(r[t.DestAddrID].ToString(),
                r[t.DestAddrType].ToString(), r[t.DestAddrText].ToString())
            ).Distinct().ToList();
            foreach (var addr in addrList)
            {
                if (WorkTask.ExistsUnfinishedTaskAtDestAddress(addr))
                {
                    throw new WmsException(500, $"目标地址{addr.Text}还有任务未完成！");
                }
            }
        }

        // 20200713备份
        //public IWave ReleaseWave(string headNoOrID)
        public IWave ReleaseWaveV2(string headNoOrID, string stockUpLocID)
        {
            DataRow headRow = GetWaveHead(headNoOrID);

            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Computing.ToString()))
            {
                throw new WmsException(-630, "波次正在进行预分配! 不能确认!");
            }
            /*
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Released.ToString()))
            {
                return new ResultInfo(-631, "波次已经释放，不能再次释放!");
            }
            */

            decimal leftTaskQty = decimal.Parse("0" + headRow["left_task_qty"].ToString());

            DataRow[] rows = GetWaveTasksByHeadID(headRow["t_id"].ToString());

            if (rows.Length == 0)
            {
                throw new WmsException(-632, "波次尚无任务，不能确认! 请确认已经进行库存分配!");
            }
            if (rows.Where(p => p["task_type_code"].ToString() != "" // TODO TaskType.Picking
                && string.IsNullOrWhiteSpace(p["location_id"].ToString())).Count() > 0)
            {
                throw new WmsException(-633, "该波次存在未指定货位的任务，无法继续执行！");
            }

            // 更新波次头表
            //info = UpdateHeadState(headID, WaveStatus.WaveStatus_Released.ToString());
            UpdateHeadReleaseInfo(headNoOrID,
                Context.UserInfo.UserName, DateTime.Now.ToString());

            UpdateHeadStatus(headNoOrID, WaveStatus.WaveStatus_Released.ToString(), headRow["wave_status"].ToString());

            // 更新出库订单状态
            DataRow[] detailRows = GetWaveDetailsByHeadID(headRow["t_id"].ToString());

            string[] orderIds = detailRows.Select(r => r["outbound_h_id"].ToString()).ToArray();

            OutboundOrder.UpdateOrderHeadState(string.Join(",", orderIds),
                OutDocStatus.OutDocStatus_Allocated.ToString(), OutDocStatus.OutDocStatus_Preallocation.ToString());

            DataRow[] taskRows = GetWaveTasksByHeadID(headRow["t_id"].ToString());
            decimal detailTotal = detailRows.Sum(r => decimal.Parse("0" + r["plan_qty"]));
            decimal taskTotal = taskRows.Sum(r => decimal.Parse("0" + r["operation_qty"]));
            if (detailTotal != taskTotal)
            {
                throw new WmsException(470, "预分配的数量与波次计划数量不符");
            }

            string barcodeNum = WmsConfig.GetOrDefault("Barcode:ProcessBarcodeNumber", "0");
            if (barcodeNum == "1")
            {
                var detailTotals = detailRows
                    .Where(r => r["inventory_class"].ToString() == "WorkInProcess")
                    .GroupBy(r => new
                    {
                        ItemID = r["item_id"].ToString(),
                    })
                    .Select(r => new
                    {
                        r.Key.ItemID,
                        PlanQty = r.Sum(r => decimal.Parse("0" + r["plan_qty"]))
                    }).ToArray();

                taskRows = WorkTask.GetByRefDocNoOrID("Wave", headRow["wave_no"].ToString());

                foreach (var total in detailTotals)
                {
                    taskTotal = taskRows
                        .Where(r => r["item_id"].ToString() == total.ItemID)
                        .Sum(r => decimal.Parse("0" + r["operation_qty"]));

                    if (total.PlanQty != taskTotal)
                    {
                        throw new WmsException(471, "预分配的烟码分离数量与波次中的烟码分离数量不符");
                    }
                }

            }

            //IWorkTask workTask = WmsFactory.MakeWorkTask();
            List<string> idList = new List<string>();

            foreach (DataRow row in rows)
            {
                if (row["wave_status"].ToString() == WaveStatus.WaveStatus_Released.ToString())
                {
                    continue;
                }

                UpdateWaveTaskState(row["task_no"].ToString(), WaveStatus.WaveStatus_Released.ToString());

                var id = SnowflakeID.Guid().ToString();
                // 保存工作任务
                if (!string.IsNullOrWhiteSpace(stockUpLocID))
                {
                    DataRow targetLocRow = Location.Get(stockUpLocID);
                    row["target_id"] = targetLocRow["t_id"].ToString();
                    row["target_type"] = "AddressType_Location";
                    row["target_text"] = targetLocRow["location_no"].ToString();
                }

                InsertWorkTask(row, id);

                leftTaskQty++;
                idList.Add(id);

                //if (1 == 0 && row["task_type_code"].ToString() == TaskType.Filling)
                // TODO
                {
                    id = SnowflakeID.Guid().ToString();
                    /*
                    string dependentNo = row["task_no"].ToString();
                    id = SnowflakeID.Guid().ToString();
                    row["task_type_code"] = "Task_Picking";
                    row["task_exec_type_code"] = GetTaskExecType("Task_Picking");
                    row["equipment_id"] = "";
                    row["task_no"] = WmsHelper.GetSequence("TaskNo");

                    row["source_id"] = row["target_id"];
                    row["source_type"] = row["target_type"];
                    row["source_text"] = row["target_text"];

                    DataRow[] orderRows = WmsFactory.MakeOutboundOrder().GetOrderHeads(row["outbound_h_id"].ToString());
                    row["target_id"] = orderRows[0]["target_id"].ToString();
                    row["target_type"] = orderRows[0]["target_type"].ToString();
                    row["target_text"] = orderRows[0]["target_text"].ToString();

                    DataRow itemRow = WmsFactory.MakeItem()
                        .GetItem(row["item_id"].ToString());

                    //string addr = orderRows[0]["target_type"].ToString().Equals("Address_Dock") ? "月台" : "站台";
                    //string ledtext = $"品规：{itemRow["item_name"]};" +
                    //    $"数量：{row["operation_qty"]};单位：{itemRow["cname"]};{addr}：{orderRows[0]["target_text"]}";
                    //row["led_text"] = ledtext;
                    row["dependence_task_no"] = dependentNo;

                    workTask.InsertTask(row, id);
                    */
                    Container.AddContainerOutQty(row["container_id"].ToString().Trim(),
                        decimal.Parse("0" + row["operation_qty"].ToString()));
                    //InsertPickingTask(row, id);
                    leftTaskQty++;
                    idList.Add(id);
                }
                /*
                info = UpdateDetailFinishedQty(row["t_id"].ToString(),
                    1 + decimal.Parse("0" + row["finished_qty"].ToString()));
                if (info.ErrorCode != 0) throw new Exception(info.ErrMsg);
                */

                UpdateHeadLeftTaskQty(headNoOrID, leftTaskQty);

            }

            WorkTaskInterface.Send(idList);

            return this;
        }

        // 20191128 备份
        public IWave ReleaseWaveV1(string headNoOrID)
        {
            DataRow headRow = GetWaveHead(headNoOrID);

            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Computing.ToString()))
            {
                throw new WmsException(-630, "波次正在进行预分配! 不能确认!");
            }
            /*
            if (headRow["wave_status"].ToString().Equals(WaveStatus.WaveStatus_Released.ToString()))
            {
                return new ResultInfo(-631, "波次已经释放，不能再次释放!");
            }
            */

            decimal leftTaskQty = decimal.Parse("0" + headRow["left_task_qty"].ToString());

            DataRow[] rows = GetWaveTasksByHeadID(headRow["t_id"].ToString());

            if (rows.Length == 0)
            {
                throw new WmsException(-632, "波次尚无任务，不能确认! 请确认已经进行库存分配!");
            }
            if (rows.Where(p => p["task_type_code"].ToString() != "" // TODO TaskType.Picking
                && string.IsNullOrWhiteSpace(p["location_id"].ToString())).Count() > 0)
            {
                throw new WmsException(-633, "该波次存在未指定货位的任务，无法继续执行！");
            }

            // 更新波次头表
            //info = UpdateHeadState(headID, WaveStatus.WaveStatus_Released.ToString());
            UpdateHeadReleaseInfo(headNoOrID,
                Context.UserInfo.UserName, DateTime.Now.ToString());

            UpdateHeadStatus(headNoOrID, WaveStatus.WaveStatus_Released.ToString(), headRow["wave_status"].ToString());

            // 更新出库订单状态
            DataRow[] detailRows = GetWaveDetailsByHeadID(headRow["t_id"].ToString());

            string[] orderIds = detailRows.Select(r => r["outbound_h_id"].ToString()).ToArray();

            OutboundOrder.UpdateOrderHeadState(string.Join(",", orderIds),
                OutDocStatus.OutDocStatus_Allocated.ToString(), OutDocStatus.OutDocStatus_Preallocation.ToString());

            //IWorkTask workTask = WmsFactory.MakeWorkTask();
            List<string> idList = new List<string>();

            foreach (DataRow row in rows)
            {
                if (row["wave_status"].ToString() == WaveStatus.WaveStatus_Released.ToString())
                {
                    continue;
                }

                UpdateWaveTaskState(row["task_no"].ToString(), WaveStatus.WaveStatus_Released.ToString());

                var id = SnowflakeID.Guid().ToString();
                // 保存工作任务
                InsertWorkTask(row, id);

                leftTaskQty++;
                idList.Add(id);

                if (1 == 0 && row["task_type_code"].ToString() == "") // TODO TaskType.Filling)
                {
                    id = SnowflakeID.Guid().ToString();
                    /*
                    string dependentNo = row["task_no"].ToString();
                    id = SnowflakeID.Guid().ToString();
                    row["task_type_code"] = "Task_Picking";
                    row["task_exec_type_code"] = GetTaskExecType("Task_Picking");
                    row["equipment_id"] = "";
                    row["task_no"] = WmsHelper.GetSequence("TaskNo");

                    row["source_id"] = row["target_id"];
                    row["source_type"] = row["target_type"];
                    row["source_text"] = row["target_text"];

                    DataRow[] orderRows = WmsFactory.MakeOutboundOrder().GetOrderHeads(row["outbound_h_id"].ToString());
                    row["target_id"] = orderRows[0]["target_id"].ToString();
                    row["target_type"] = orderRows[0]["target_type"].ToString();
                    row["target_text"] = orderRows[0]["target_text"].ToString();

                    DataRow itemRow = WmsFactory.MakeItem()
                        .GetItem(row["item_id"].ToString());

                    //string addr = orderRows[0]["target_type"].ToString().Equals("Address_Dock") ? "月台" : "站台";
                    //string ledtext = $"品规：{itemRow["item_name"]};" +
                    //    $"数量：{row["operation_qty"]};单位：{itemRow["cname"]};{addr}：{orderRows[0]["target_text"]}";
                    //row["led_text"] = ledtext;
                    row["dependence_task_no"] = dependentNo;

                    workTask.InsertTask(row, id);
                    */
                    Container.AddContainerOutQty(row["container_id"].ToString().Trim(),
                        decimal.Parse("0" + row["operation_qty"].ToString()));
                    //InsertPickingTask(row, id);
                    leftTaskQty++;
                    idList.Add(id);
                }
                /*
                info = UpdateDetailFinishedQty(row["t_id"].ToString(),
                    1 + decimal.Parse("0" + row["finished_qty"].ToString()));
                if (info.ErrorCode != 0) throw new Exception(info.ErrMsg);
                */

                UpdateHeadLeftTaskQty(headNoOrID, leftTaskQty);

            }

            WorkTaskInterface.Send(idList);

            return this;
        }

        public IWave RemoveWave(string waveHeadNoOrID)
        {
            if (string.IsNullOrWhiteSpace(waveHeadNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            //CancelRelease(waveHeadNoOrID);

            CancelPreallocation(waveHeadNoOrID);

            UpdateHeadStatus(waveHeadNoOrID, WaveStatus.WaveStatus_New.ToString());

            DeleteWave(waveHeadNoOrID);

            return this;
        }

        public IWave CancelRelease(string waveHeadNoOrID)
        {
            if (string.IsNullOrWhiteSpace(waveHeadNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow waveHead = GetWaveHead(waveHeadNoOrID);
            string waveID = waveHead["t_id"].ToString();
            // TODO 历史表处理
            //if (WorkTask.ExistsFinishedTaskInRefDoc(waveHead["wave_no"].ToString()))
            //{
            //    throw new WmsException(461, "该波次已有任务已完成，无法取消！");
            //}

            if (WorkTask.RefDocIsStarted(
                DocType.DocType_Wave.ToString(), waveHead["wave_no"].ToString()))
            {
                throw new WmsException(462, "该波次已有任务开始执行，无法取消！");
            }

            if (ExistsItemReplacement(waveHead))
            {
                throw new WmsException(500, "单据正在进行物料替换，无法撤销");
            }

            RestoreEditableStatus(waveID);
            //UpdateHeadState(waveID,
            //    "WaveStatus_Editable", waveHead["wave_status"].ToString());
            //waveHead["wave_status"] = "WaveStatus_Editable";
            //ResetHeadReleaseInfo(waveID);

            //DataRow[] taskRows = WorkTask.GetByRefDocNoOrID(
            //    DocType.DocType_Wave.ToString(), waveHead["wave_no"].ToString());
            //string taskIds = string.Join(',',
            //    taskRows
            //    .Where(r => r["task_type"].ToString() != "" // TODO TaskType.Barcode
            //            && string.IsNullOrWhiteSpace(r["dependent_task_no"].ToString()))
            //    .Select(r => r["t_id"].ToString()).ToArray());
            DataRow[] taskRows = WorkTask.GetByRefDocNoOrID(DocType.DocType_Wave.ToString(), waveID);
            string taskIds = string.Join(',', taskRows.Select(r => r["t_id"].ToString()).ToArray());

            if (!string.IsNullOrWhiteSpace(taskIds))
            {
                foreach (DataRow row in taskRows)
                {
                    WorkTask
                        .UpdateTaskStatus(row["t_id"].ToString(),
                            TaskStatus.TaskStatus_Create.ToString(),
                            row["task_status"].ToString())
                        .UpdateIsExecutable(row["t_id"].ToString(), "0");
                }
                //WorkTask.RecycleTasks(taskIds);
            }

            //DataRow[] detailRows = GetWaveDetailsByHeadID(waveID);

            //string[] orderIds = detailRows.Select(r => r["outbound_h_id"].ToString()).ToArray();

            //OutboundOrder.UpdateOrderHeadState(string.Join(",", orderIds),
            //    OutDocStatus.OutDocStatus_Preallocation.ToString(),
            //    OutDocStatus.OutDocStatus_Allocated.ToString());

            return this;
        }

        private bool ExistsItemReplacement(DataRow waveHead)
        {
            DataRow[] detailRows = GetWaveDetailsByHeadID(waveHead[wh.ID].ToString());
            List<string> idList = detailRows.Select(r => r[wd.OutboundHeadID].ToString()).Distinct().ToList();
            ItemReplacement itemReplacement = new ItemReplacement();
            foreach (string id in idList)
            {
                if (itemReplacement.ExistsReplacingOrder(id))
                {
                    return true;
                }
            }
            return false;
        }

        public IWave RestoreEditableStatus(string waveHeadID)
        {
            UpdateHeadStatus(waveHeadID,
                WaveStatus.WaveStatus_Editable.ToString());
            //UpdateHeadStatus(waveHeadID,
            //    WaveStatus.WaveStatus_Editable.ToString(),
            //    WaveStatus.WaveStatus_Released.ToString());
            ResetHeadReleaseInfo(waveHeadID);

            DataRow[] detailRows = GetWaveDetailsByHeadID(waveHeadID);

            string[] orderIds = detailRows
                .Select(r => r[wd.OutboundHeadID].ToString()).Distinct().ToArray();

            //OutboundOrder.UpdateOrderHeadState(string.Join(",", orderIds),
            //    OutDocStatus.OutDocStatus_Preallocation.ToString(),
            //    OutDocStatus.OutDocStatus_Allocated.ToString());
            OutboundOrder.UpdateOrderHeadState(string.Join(",", orderIds),
                OutDocStatus.OutDocStatus_Preallocation.ToString());

            return this;
        }
        /// <summary>
        /// 重置波次，如果波次是由订单自动创建的，则删除波次，否则波次置为新建状态
        /// </summary>
        /// <param name="waveNoOrID"></param>
        /// <returns></returns>
        public IWave ResetWave(string waveNoOrID)
        {
            DataRow headRow = GetWaveHead(waveNoOrID);
            return ResetWave(headRow);
        }

        public IWave ResetWave(DataRow headRow)
        {
            UpdateHeadStatus(headRow[wh.ID].ToString(),
                WaveStatus.WaveStatus_New.ToString(),
                WaveStatus.WaveStatus_Editable.ToString());

            if (headRow[wh.WaveType].ToString() == WaveType.WaveType_AutoFromOrder.ToString())
            {
                DeleteWave(headRow[wh.ID].ToString());
            }
            return this;
        }

        public IWave CancelPreallocation(string waveHeadNoOrID)
        {
            if (string.IsNullOrWhiteSpace(waveHeadNoOrID))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            DataRow waveHeadRow = GetWaveHead(waveHeadNoOrID);

            if (waveHeadRow["wave_status"].ToString() == WaveStatus.WaveStatus_Released.ToString())
            {
                //throw new WmsException(-654, "波次已释放，无法删除!");
                throw new WmsException(-654, "波次已确认，无法删除!");
            }
            if (waveHeadRow["wave_status"].ToString() == WaveStatus.WaveStatus_Finished.ToString())
            {
                throw new WmsException(-655, "波次已完成，无法删除!");
            }
            if (waveHeadRow["wave_status"].ToString() == WaveStatus.WaveStatus_Computing.ToString())
            {
                throw new WmsException(-655, "波次正在预分配，请在预分配完成后再试!");
            }

            DataRow[] waveTaskRows = null;
            try
            {
                //waveTaskRows = GetWaveTasksByHeadID(waveHeadRow["t_id"].ToString());
                waveTaskRows = WorkTask.GetByRefDocID(waveHeadRow["t_id"].ToString());
            }
            catch (WmsException ex)
            {
                if (ex.ErrorCode != DBHelper.NotFound)
                {
                    throw ex;
                }
            }

            if (waveTaskRows?.Length > 0)
            {
                //string[] taskIds = waveTaskRows.Select(r => r["t_id"].ToString()).ToArray();

                //RemoveWaveTasks(string.Join(",", taskIds));
                //WorkTask.RecycleTasks(string.Join(",", taskIds));
                WorkTask.RemoveWaveTasks(waveTaskRows);
                // 修改波次任务数
                //Wave.AddHeadLeftTaskQty(waveHeadRow[wh.ID].ToString(), -waveTaskRows.Length);
            }


            string location = WmsConfig.GetOrDefault("Barcode:BarcodeLocationCode", "");
            if (!string.IsNullOrWhiteSpace(location))
            {
                DataRow[] tasks = WorkTask.GetByRefDocNoOrID("Wave", waveHeadRow["wave_no"].ToString());
                foreach (DataRow task in tasks)
                {
                    Container.CancelStockingOut(
                        task["isa_bi_container_detail_id"].ToString(),
                        decimal.Parse("0" + task["operation_qty"]));

                    WorkTask.DeleteWorkTask(task["t_id"].ToString());
                }
            }

            return this;
        }

        public IWave RemoveWaveDetails(string detailIds)
        {
            if (string.IsNullOrWhiteSpace(detailIds))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            LogHelper.LogInformation("删除波次明细!");

            DataRow[] detailRows = GetWaveDetails(detailIds);

            List<string> headIds = detailRows.Select(r => r["wave_h_id"].ToString()).Distinct().ToList();
            if (headIds.Count > 1)
            {
                throw new WmsException(-624, "所传参数属于多个波次，无法删除!");
            }

            DataRow headRow = GetWaveHead(headIds.FirstOrDefault());

            //if (headRow["wave_status"].ToString() != WaveStatus.New)
            if (headRow["wave_status"].ToString() != WaveStatus.WaveStatus_New.ToString())
            {
                throw new WmsException(-625, $"波次状态表明其无法删除!");
            }

            //List<string> orderIds = new List<string>();
            HashSet<string> orderIds = new HashSet<string>();

            foreach (DataRow row in detailRows)
            {
                if (decimal.Parse("0" + row["processed_qty"].ToString()) > 0
                    || decimal.Parse("0" + row["finished_qty"].ToString()) > 0)
                {
                    //throw new WmsException(-626, $"明细已存在波次任务，无法删除!");
                    throw new WmsException(-626, $"明细中的数量表明该波次已有任务完成或已存在波次任务，无法删除!");
                }

                orderIds.Add(row["outbound_h_id"].ToString());
                //if (!orderIds.Contains(row["outbound_h_id"].ToString()))
                //{
                //    orderIds.Add(row["outbound_h_id"].ToString());
                //}
            }

            //DeleteWaveDetails(detailIds);
            UpdateDetailActStatus(detailIds, "0");

            foreach (string id in orderIds)
            {
                DataRow[] rows = null;
                try
                {
                    rows = GetWaveDetailsByHeadID(id);
                }
                catch (WmsException ex)
                {
                    if (ex.ErrorCode != DBHelper.NotFound)
                    {
                        throw ex;
                    }
                }

                if (rows?.Length > 0)
                {
                    continue;
                }

                OutboundOrder.UpdateOrderHeadState(id, OutDocStatus.OutDocStatus_New.ToString());
            }

            return this;
        }

        private void RemovePickingTasks(DataRow[] taskRows)
        {
            return;
#pragma warning disable CS0162 // 检测到无法访问的代码
            if (taskRows == null) return;
#pragma warning restore CS0162 // 检测到无法访问的代码

            foreach (DataRow row in taskRows)
            {
                if (row["wave_status"].ToString() == WaveStatus.WaveStatus_Finished.ToString())
                {
                    throw new WmsException(-622, $"波次任务{row["task_no"]}已完成，不能被删除!");
                }
                if (row["wave_status"].ToString() == WaveStatus.WaveStatus_Released.ToString())
                {
                    throw new WmsException(-622, $"波次任务{row["task_no"]}已释放，不能被删除!");
                }

                // TODO 若托盘的待出数量之前更新过，此处要恢复

                string id = row["t_id"].ToString();

                AddDetailProcessedQty(row["wave_d_id"].ToString(), -(decimal)row["operation_qty"]);

                OutboundOrder
                    .AddDetailProcessedQty(row["order_d_id"].ToString(), -(decimal)row["operation_qty"]);

                Container.AddContainerOutQty(row["container_id"].ToString(), -(decimal)row["operation_qty"]);

                DeleteWaveTasks(id);
            }
        }

        private void RemoveOutboundOrFillingTasks(DataRow[] taskRows)
        {
            if (taskRows == null) return;

            foreach (DataRow row in taskRows)
            {
                if (row["wave_status"].ToString() == WaveStatus.WaveStatus_Finished.ToString())
                {
                    throw new WmsException(-622, $"任务{row["task_no"]}已完成，不能被删除!");
                }
                if (row["wave_status"].ToString() == WaveStatus.WaveStatus_Released.ToString())
                {
                    throw new WmsException(-622, $"任务{row["task_no"]}已释放，不能被删除!");
                }

                DataRow dependentTask = GetDependentTask(row["task_no"].ToString());
                if (dependentTask != null)
                {
                    throw new WmsException(-645, $"波次任务{row["task_no"]}存在依赖任务，不能被删除!");
                }

                //if (row["task_type_code"].ToString().Trim() == "Task_Picking")
                //{
                //Location.ModifyLocationStockState(
                //    row["location_id"].ToString(), "LocationState_Stored", "LocationState_Out");
                //}

                string id = row["t_id"].ToString();

                //DecreaseDetailProcessedQty(row["wave_d_id"].ToString());
                AddDetailProcessedQty(row["wave_d_id"].ToString(), -(decimal)row["operation_qty"]);

                decimal opQty = (decimal)row["operation_qty"];
                OutboundOrder
                    .AddDetailProcessedQty(row["outbound_d_id"].ToString(), -opQty);

                Container.AddDetailOutQty(row["container_id"].ToString(), -opQty);

                DeleteWaveTasks(id);
            }
        }

        public IWave RemoveWaveTasks(string taskIds)
        {
            if (string.IsNullOrWhiteSpace(taskIds))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            LogHelper.LogInformation("删除波次任务!");
            DataRow[] taskRows = GetWaveTasks(taskIds);
            if (taskRows.Count() <= 0)
            {
                throw new WmsException(-620, "找不到波次任务");
            }
            //List<string> headIds = taskRows.Select(r => r["wave_h_id"].ToString()).Distinct().ToList();
            //if (headIds.Count > 1)
            //{
            //    throw new WmsException(-621, "所传参数属于多个波次，无法删除!");
            //}
            RemoveWaveTasks(taskRows);
            //CheckAndUpdateStatus(headIds);
            return this;
        }

        private void RemoveWaveTasks(DataRow[] taskRows)
        {
            List<string> headIds = taskRows.Select(r => r["wave_h_id"].ToString()).Distinct().ToList();
            if (headIds.Count > 1)
            {
                throw new WmsException(-621, "所传参数属于多个波次，无法删除!");
            }

            DataRow[] pickingRows = taskRows
                .Where(r => r["task_type_code"].ToString() == "").ToArray(); // TODO TaskType.Picking).ToArray();
            // TODO taskRows.Where(r = >)
            RemovePickingTasks(pickingRows);

            DataRow[] leftRows = taskRows
                .Where(r => r["task_type_code"].ToString() != "").ToArray(); // TODO TaskType.Picking).ToArray();
            RemoveOutboundOrFillingTasks(leftRows);

            CheckAndUpdateStatus(headIds);
        }

        public IWave RemoveWaveTasksV1(string taskIds)
        {
            if (string.IsNullOrWhiteSpace(taskIds))
            {
                throw new WmsException(500, ErrMsg.ParamIsNull);
            }
            LogHelper.LogInformation("删除波次任务!");

            DataRow[] taskRows = GetWaveTasks(taskIds);
            if (taskRows.Count() <= 0)
            {
                throw new WmsException(-620, "找不到波次任务");
            }

            List<string> headIds = taskRows.Select(r => r["wave_h_id"].ToString()).Distinct().ToList();
            if (headIds.Count > 1)
            {
                throw new WmsException(-621, "所传参数属于多个波次，无法删除!");
            }

            DataRow[] pickingRows = taskRows
                .Where(r => r["task_type_code"].ToString() == "").ToArray(); // TODO TaskType.Picking).ToArray();
            // TODO taskRows.Where(r = >)
            RemovePickingTasks(pickingRows);

            DataRow[] leftRows = taskRows
                .Where(r => r["task_type_code"].ToString() != "").ToArray(); // TODO TaskType.Picking).ToArray();
            RemoveOutboundOrFillingTasks(leftRows);

            CheckAndUpdateStatus(headIds);

            return this;
        }

        private void CheckAndUpdateStatus(List<string> waveHeadIds)
        {
            foreach (string id in waveHeadIds)
            {
                DataRow[] rows = null;
                try
                {
                    rows = GetWaveTasksByHeadID(id);
                }
                catch (WmsException ex)
                {
                    if (ex.ErrorCode != DBHelper.NotFound)
                    {
                        throw ex;
                    }
                }

                if (rows == null || rows.Length == 0)
                {
                    UpdateHeadStatus(id, WaveStatus.WaveStatus_New.ToString());
                }
            }
        }

        public IWave SplitWaveDetail(string groupNo, decimal num, string detailIds)
        {
            throw new NotImplementedException();
        }

        public IWave UpdateHeadStatus(string headNoOrID, string status, string oldStatus = null)
        {
            if (string.IsNullOrWhiteSpace(oldStatus))
            {
                DataRow row = GetWaveHead(headNoOrID);
                oldStatus = row["wave_status"].ToString();
            }
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "warehouse", Context.WarehouseID },
                { "id_no", headNoOrID },
                { "state", status },
                { "old_state", oldStatus },
            };

            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, wave_status = @state
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse 
   and (t_id = @id_no or wave_no = @id_no) and wave_status = @old_state 
";
            DBHelper.RunSql(sql, param, "更新波次头表状态");

            return this;
        }
        public IWave UpdateHeadActStatus(string headNoOrID, string status, string oldStatus = null)
        {
            if (string.IsNullOrWhiteSpace(oldStatus))
            {
                DataRow row = GetWaveHead(headNoOrID);
                oldStatus = row[wh.ActStatus].ToString();
            }
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "warehouse", Context.WarehouseID },
                { "id_no", headNoOrID },
                { "act_status", status },
                { "old_status", oldStatus },
            };

            string sql = @"
update &DataSource&wms_b_wave_h set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, act_status = @act_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse 
   and (t_id = @id_no or wave_no = @id_no) and act_status = @old_status 
";
            DBHelper.RunSql(sql, param, "修改波次主表记录状态");
            return this;
        }
        public IWave UpdateDetailActStatus(string waveDetailIds, string status)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "username", Context.UserInfo.UserName },
                { "login_ip", Context.UserInfo.LoginIP },
                { "warehouse", Context.WarehouseID },
                { "ids", waveDetailIds },
                { "act_status", status },
            };

            string sql = @"
update &DataSource&wms_b_wave_d set modify_date = now(), modify_ip  = @login_ip,
       last_modified_by = @username, act_status = @act_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&'
   and warehouse_id = @warehouse 
   and instr(@ids, t_id) > 0 
";
            DBHelper.RunSql(sql, param, "修改波次明细表记录状态", DBHelper.OperatedNumType.OneOrMore);
            return this;
        }
    }
}
