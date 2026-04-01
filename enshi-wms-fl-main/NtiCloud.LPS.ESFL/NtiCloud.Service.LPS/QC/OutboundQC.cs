using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.QC
{
    public class OutboundQC : IOutboundQC
    {
        public void DeleteDetailBy(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql = @"
delete from isa_b_qcout_d  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id 

";
            DBHelper.RunSql(sql, param, "删除出库质检明细");
        }

        public DataRow GetDetailBy(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from isa_b_qcout_d  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id 

";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询出库质检明细");

            return dt.Rows[0];
        }

        public IOutboundQC Insert(OutboundQCDetailModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "zone_id", model.ZoneID },
                { "qcout_hid", model.QCOutHeadID },
                { "item_id", model.ItemID },
                { "item_name", model.ItemName },
                { "location_id", model.LocationID },
                { "finance_qty", model.FinanceQty.ToString() },
                { "uom_financeid", model.UomFinanceID },
                { "plan_qty", model.PlanQty.ToString() },
                { "uom_qtyid", model.UomQtyID },
                { "speci", model.Spec },
                { "finance_qty_pcs", model.FinanceQtyPcs.ToString() },
                { "actual_qty", model.ActualQty.ToString() },
                { "actual_finance_qty", model.ActualFinanceQty.ToString() },
                { "appearance", model.Appearance },
                { "humidity", model.Humidity },
                { "temperature", model.Temperature },
                { "mildew", model.Mildew },
                { "pest", model.Pest },
                { "oilpressure", model.Oilpressure },
                { "color", model.Color },
                { "compaction", model.Compaction },
                { "check_result", model.CheckResult },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus },
                { "outbound_did", model.OutboundDetailID },
            };
            string sql =
                "insert into WMS_B_QCOUT_D ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       zone_id, " +
                "       qcout_hid, " +
                "       item_id, " +
                "       item_name, " +
                "       location_id, " +
                "       finance_qty, " +
                "       uom_financeid, " +
                "       plan_qty, " +
                "       uom_qtyid, " +
                "       speci, " +
                "       finance_qty_pcs, " +
                "       actual_qty, " +
                "       actual_finance_qty, " +
                "       appearance, " +
                "       humidity, " +
                "       temperature, " +
                "       mildew, " +
                "       pest, " +
                "       oilpressure, " +
                "       color, " +
                "       compaction, " +
                "       check_result, " +
                "       create_date, " +
                "       created_by, " +
                "       modify_ip, " +
                "       modify_date, " +
                "       last_modified_by, " +
                "       outbound_did, " +
                "       act_status) " +
                "values ( " +
                "       @t_id, " +
                "       '&CLIENT_ID&', " +
                "       '&ORG_ID&', " +
                "       @warehouse_id, " +
                "       @zone_id, " +
                "       @qcout_hid, " +
                "       @item_id, " +
                "       @item_name, " +
                "       @location_id, " +
                "       @finance_qty, " +
                "       @uom_financeid, " +
                "       @plan_qty, " +
                "       @uom_qtyid, " +
                "       @speci, " +
                "       @finance_qty_pcs, " +
                "       @actual_qty, " +
                "       @actual_finance_qty, " +
                "       @appearance, " +
                "       @humidity, " +
                "       @temperature, " +
                "       @mildew, " +
                "       @pest, " +
                "       @oilpressure, " +
                "       @color, " +
                "       @compaction, " +
                "       @check_result, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @outbound_did, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "出库质检明细");

            return this;
        }

        public IOutboundQC InsertDetail(DataRow row)
        {
            Insert(new OutboundQCDetailModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                ZoneID = row["zone_id"].ToString(),
                QCOutHeadID = row["qcout_hid"].ToString(),
                ItemID = row["item_id"].ToString(),
                ItemName = row["item_name"].ToString(),
                LocationID = row["location_id"].ToString(),
                FinanceQty = DBHelper.GetDecimal(row["finance_qty"]),
                UomFinanceID = row["uom_financeid"].ToString(),
                PlanQty = DBHelper.GetDecimal(row["plan_qty"]),
                UomQtyID = row["uom_qtyid"].ToString(),
                Spec = row["speci"].ToString(),
                FinanceQtyPcs = DBHelper.GetDecimal(row["finance_qty_pcs"]),
                ActualQty = DBHelper.GetDecimal(row["actual_qty"]),
                ActualFinanceQty = DBHelper.GetDecimal(row["actual_finance_qty"]),
                Appearance = row["appearance"].ToString(),
                Humidity = row["humidity"].ToString(),
                Temperature = row["temperature"].ToString(),
                Mildew = row["mildew"].ToString(),
                Pest = row["pest"].ToString(),
                Oilpressure = row["oilpressure"].ToString(),
                Color = row["color"].ToString(),
                Compaction = row["compaction"].ToString(),
                CheckResult = row["check_result"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString(),
                OutboundDetailID = row["outbound_did"].ToString(),
            });

            return this;
        }

        public void DeleteHeadBy(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql = @"
delete from isa_b_qcout_h  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id 

";
            DBHelper.RunSql(sql, param, "删除出库质检主档");
        }

        public DataRow GetHeadBy(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from isa_b_qcout_h  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id 

";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询出库质检主档");

            return dt.Rows[0];
        }

        public IOutboundQC Insert(OutboundQCHeadModel model)
        {
            model.ID = model.ID ?? SnowflakeID.Guid().ToString();
            model.WarehouseID = model.WarehouseID ?? Context.WarehouseID;
            model.CreatedBy = model.CreatedBy ?? Context.UserInfo.UserName;
            model.ModifyIP = model.ModifyIP ?? Context.UserInfo.LoginIP;
            model.LastModifiedBy = model.LastModifiedBy ?? Context.UserInfo.UserName;

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "t_id", model.ID },
                { "warehouse_id", model.WarehouseID },
                { "zone_id", model.ZoneID },
                { "qcout_doc_no", model.QcoutDocNo },
                { "outbound_hid", model.OutboundHid },
                { "service_type", model.ServiceType },
                { "qcout_status", model.QcoutStatus },
                { "status", model.Status },
                { "finance_qty", model.FinanceQty.ToString() },
                { "uom_financeid", model.UomFinanceid },
                { "qty", model.Qty.ToString() },
                { "uom_qtyid", model.UomQtyid },
                { "handling_user_id", model.HandlingUserID },
                { "handling_date", model.HandlingDate },
                { "remarks", model.Remarks },
                { "create_date", model.CreateDate.ToString() },
                { "created_by", model.CreatedBy },
                { "modify_ip", model.ModifyIP },
                { "modify_date", model.ModifyDate.ToString() },
                { "last_modified_by", model.LastModifiedBy },
                { "act_status", model.ActStatus }
            };
            string sql =
                "insert into WMS_B_QCOUT_H ( " +
                "       t_id, " +
                "       client_id, " +
                "       org_id, " +
                "       warehouse_id, " +
                "       zone_id, " +
                "       qcout_doc_no, " +
                "       outbound_hid, " +
                "       service_type, " +
                "       qcout_status, " +
                "       status, " +
                "       finance_qty, " +
                "       uom_financeid, " +
                "       qty, " +
                "       uom_qtyid, " +
                "       handling_user_id, " +
                "       handling_date, " +
                "       remarks, " +
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
                "       @zone_id, " +
                "       @qcout_doc_no, " +
                "       @outbound_hid, " +
                "       @service_type, " +
                "       @qcout_status, " +
                "       @status, " +
                "       @finance_qty, " +
                "       @uom_financeid, " +
                "       @qty, " +
                "       @uom_qtyid, " +
                "       @handling_user_id, " +
                "       @handling_date, " +
                "       @remarks, " +
                "       now(), " +
                "       @created_by, " +
                "       @modify_ip, " +
                "       now(), " +
                "       @last_modified_by, " +
                "       @act_status) ";

            DBHelper.RunSql(sql, param, "出库质检主档");

            return this;
        }

        public IOutboundQC Insert(DataRow row)
        {
            Insert(new OutboundQCHeadModel()
            {
                ID = row["t_id"].ToString(),
                ClientID = row["client_id"].ToString(),
                OrgID = row["org_id"].ToString(),
                WarehouseID = row["warehouse_id"].ToString(),
                ZoneID = row["zone_id"].ToString(),
                QcoutDocNo = row["qcout_doc_no"].ToString(),
                OutboundHid = row["outbound_hid"].ToString(),
                ServiceType = row["service_type"].ToString(),
                QcoutStatus = row["qcout_status"].ToString(),
                Status = row["status"].ToString(),
                FinanceQty = DBHelper.GetDecimal(row["finance_qty"]),
                UomFinanceid = row["uom_financeid"].ToString(),
                Qty = DBHelper.GetDecimal(row["qty"]),
                UomQtyid = row["uom_qtyid"].ToString(),
                HandlingUserID = row["handling_user_id"].ToString(),
                HandlingDate = row["handling_date"].ToString(),
                Remarks = row["remarks"].ToString(),
                CreateDate = DBHelper.GetDateTime(row["create_date"]),
                CreatedBy = row["created_by"].ToString(),
                ModifyIP = row["modify_ip"].ToString(),
                ModifyDate = DBHelper.GetDateTime(row["modify_date"]),
                LastModifiedBy = row["last_modified_by"].ToString(),
                ActStatus = row["act_status"].ToString()
            });

            return this;
        }

        public void UpdateQCOutStatus(string id, string qcOutStatus, string oldQCOutStatus = null)
        {
            if (oldQCOutStatus == null)
            {
                oldQCOutStatus = GetHeadBy(id)["qcout_status"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "qcout_status", qcOutStatus},
                { "old_qcout_status", oldQCOutStatus},
                { "t_id", id }
            };
            string sql = @"
update isa_b_qcout_h  
   set last_edited_date = now(), 
       last_edited_by = @last_edited_by, 
       modify_ip = @modify_ip,
       qcout_status = @qcout_status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id
   and qcout_status = @old_qcout_status
";
            DBHelper.RunSql(sql, param, "更新出库质检状态");
        }

        public void UpdateStatus(string id, string status, string oldStatus = null)
        {
            if (oldStatus == null)
            {
                oldStatus = GetHeadBy(id)["status"].ToString();
            }

            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "last_edited_by", Context.UserInfo.UserName },
                { "modify_ip", Context.UserInfo.LoginIP },
                { "status", status},
                { "old_status", oldStatus},
                { "t_id", id }
            };
            string sql = @"
update isa_b_qcout_h  
   set last_edited_date = now(), 
       last_edited_by = @last_edited_by, 
       modify_ip = @modify_ip,
       status = @status
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id
   and status = @old_status
";
            DBHelper.RunSql(sql, param, "更新提交状态");
        }

    }
}
