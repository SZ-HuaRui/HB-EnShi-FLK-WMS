using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Common.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.CrossTruck
{
    public class CrossTruck : WmsObject, ICrossTruck
    {
        #region
        /// <summary>
        /// 外部-越库-立库，波次后库存不足则创建越库单
        /// </summary>
        /// <param name="OutOrderCode">出库单单据</param>
        /// <returns></returns>
        public WebJsonInfo CreateCrossTruck(string OutOrderCode)
        {
            try
            {
                string o_msg = "";
                string sql = "";
                string selectOne = "";
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @"select count(1) from WMS_D_OUTBOUND_H h where h.DOC_NO=@DOC_NO";
                param.Clear();
                param.Add("DOC_NO", OutOrderCode);
                selectOne = DBHelper.SelectOne(sql,param,"查询出库单据是否存在",false);
                if(Convert.ToInt32(selectOne)<1)
                {
                    throw new WmsException(500, "出库单不存在");
                }

                sql = @"select SERVICE_TYPE from WMS_D_OUTBOUND_H h where h.DOC_NO=@DOC_NO";
                param.Clear();
                param.Add("DOC_NO", OutOrderCode);
                selectOne = DBHelper.SelectOne(sql, param, "查询出库单业务类型", false);

                // ToDo:根据配置判断单据是否可以越库 
                if(OutOrderCode == "不允许越库")
                {
                    throw new WmsException(500, "出库单不允许越库");
                }

                // ToDo:判断是否短收
                if(OutOrderCode == "短收")
                {
                    //未短收直接退出
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "", json = "单据未短收" };
                }

                // ToDo:产生新越库单据，需要优化

                string outboundHID = null;
                sql = "select * from WMS_D_OUTBOUND_H h where h.doc_no=@doc_no";
                param.Clear();
                param.Add("doc_no", OutOrderCode);
                DataTable dtOutbound = DBHelper.Select(sql, param, "查询需要越库的出库单");
                string msg = InsertOutBoundH(out outboundHID, dtOutbound);

                if(!string.IsNullOrEmpty(msg))
                {
                    throw new WmsException(500, msg);
                }

                string oldOutboundID = dtOutbound.Rows[0]["t_id"].ToString();

                sql = "select * from WMS_D_OUTBOUND_D d where d.OUTBOUND_H_ID=@t_id";
                param.Clear();
                param.Add("t_id", oldOutboundID);
                DataTable dtOutDetail = DBHelper.Select(sql, param, "查询需要越库的出库明细", false);
                msg = InsertOutBoundD(outboundHID, dtOutDetail);

                if (!string.IsNullOrEmpty(msg))
                {
                    throw new WmsException(500, msg);
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        /// <summary>
        /// 插入出库明细
        /// </summary>
        /// <param name="outboundHID">出库主档ID</param>
        /// <param name="r">查询的需要越库的所有出库明细</param>
        /// <returns>成功返回null，错误返回错误信息</returns>
        private string InsertOutBoundH(out string outboundHID, DataTable outBound)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (outBound == null || outBound.Rows.Count < 1)
            {
                outboundHID = null;
                return "越库单出库模板不存在";
            }

            DataRow r = outBound.Rows[0];

            string sql = @"insert into WMS_D_OUTBOUND_H (t_id, 
                            client_id, 
                            org_id, 
                            warehouse_id, 
                            doc_no, 
                            doc_type, 
                            service_type, 
                            ref_doc_no, 
                            plan_no, 
                            bom_code, 
                            doc_status, 
                            source, 
                            start_date, 
                            finish_date, 
                            supplier_id, 
                            manufacturer_id, 
                            owner_id, 
                            src_factory, 
                            src_inventory, 
                            src_location, 
                            dest_factory, 
                            dest_inventory, 
                            dest_location, 
                            shipper, 
                            zone_id, 
                            dock_id, 
                            license_plate, 
                            deliver, 
                            deliver_tel, 
                            trans_permit, 
                            contract, 
                            net_weight, 
                            gross_weight, 
                            truck_weight, 
                            uom_weight_id, 
                            src_addr, 
                            dest_addr, 
                            left_task_count, 
                            is_auto_created, 
                            memo, 
                            create_date, 
                            created_by, 
                            modify_ip, 
                            modify_date, 
                            last_modified_by, 
                            act_status)
                          values(
                            @t_id, 
                            @client_id, 
                            @org_id, 
                            @warehouse_id, 
                            @doc_no, 
                            @doc_type, 
                            @service_type, 
                            @ref_doc_no, 
                            @plan_no, 
                            @bom_code, 
                            @doc_status, 
                            @source, 
                            @start_date, 
                            @finish_date, 
                            @supplier_id, 
                            @manufacturer_id, 
                            @owner_id, 
                            @src_factory, 
                            @src_inventory, 
                            @src_location, 
                            @dest_factory, 
                            @dest_inventory, 
                            @dest_location, 
                            @shipper, 
                            @zone_id, 
                            @dock_id, 
                            @license_plate, 
                            @deliver, 
                            @deliver_tel, 
                            @trans_permit, 
                            @contract, 
                            @net_weight, 
                            @gross_weight, 
                            @truck_weight, 
                            @uom_weight_id, 
                            @src_addr, 
                            @dest_addr, 
                            @left_task_count, 
                            @is_auto_created, 
                            @memo, 
                            @create_date, 
                            @created_by, 
                            @modify_ip, 
                            @modify_date, 
                            @last_modified_by, 
                            @act_status)";

            Dictionary<string, object> param = new Dictionary<string, object>();
            outboundHID = SnowflakeID.Guid().ToString();
            param.Add("t_id", outboundHID);
            param.Add("client_id", r["client_id"] == null ? "" : r["client_id"].ToString());
            param.Add("org_id", r["org_id"] == null ? "" : r["org_id"].ToString());
            param.Add("warehouse_id", r["warehouse_id"] == null ? "" : r["warehouse_id"].ToString());
            param.Add("doc_no", r["doc_no"] == null ? "" : r["doc_no"].ToString());
            param.Add("doc_type", r["doc_type"] == null ? "" : r["doc_type"].ToString());
            param.Add("service_type", r["service_type"] == null ? "" : r["service_type"].ToString());
            param.Add("ref_doc_no", r["ref_doc_no"] == null ? "" : r["ref_doc_no"].ToString());
            param.Add("plan_no", r["plan_no"] == null ? "" : r["plan_no"].ToString());
            param.Add("bom_code", r["bom_code"] == null ? "" : r["bom_code"].ToString());
            param.Add("doc_status", r["doc_status"] == null ? "" : r["doc_status"].ToString());
            param.Add("source", r["source"] == null ? "" : r["source"].ToString());
            param.Add("start_date", r["start_date"] == null ? "" : r["start_date"].ToString());
            param.Add("finish_date", r["finish_date"] == null ? "" : r["finish_date"].ToString());
            param.Add("supplier_id", r["supplier_id"] == null ? "" : r["supplier_id"].ToString());
            param.Add("manufacturer_id", r["manufacturer_id"] == null ? "" : r["manufacturer_id"].ToString());
            param.Add("owner_id", r["owner_id"] == null ? "" : r["owner_id"].ToString());
            param.Add("src_factory", r["src_factory"] == null ? "" : r["src_factory"].ToString());
            param.Add("src_inventory", r["src_inventory"] == null ? "" : r["src_inventory"].ToString());
            param.Add("src_location", r["src_location"] == null ? "" : r["src_location"].ToString());
            param.Add("dest_factory", r["dest_factory"] == null ? "" : r["dest_factory"].ToString());
            param.Add("dest_inventory", r["dest_inventory"] == null ? "" : r["dest_inventory"].ToString());
            param.Add("dest_location", r["dest_location"] == null ? "" : r["dest_location"].ToString());
            param.Add("shipper", r["shipper"] == null ? "" : r["shipper"].ToString());
            param.Add("zone_id", r["zone_id"] == null ? "" : r["zone_id"].ToString());
            param.Add("dock_id", r["dock_id"] == null ? "" : r["dock_id"].ToString());
            param.Add("license_plate", r["license_plate"] == null ? "" : r["license_plate"].ToString());
            param.Add("deliver", r["deliver"] == null ? "" : r["deliver"].ToString());
            param.Add("deliver_tel", r["deliver_tel"] == null ? "" : r["deliver_tel"].ToString());
            param.Add("trans_permit", r["trans_permit"] == null ? "" : r["trans_permit"].ToString());
            param.Add("contract", r["contract"] == null ? "" : r["contract"].ToString());
            param.Add("net_weight", r["net_weight"] == null ? "" : r["net_weight"].ToString());
            param.Add("gross_weight", r["gross_weight"] == null ? "" : r["gross_weight"].ToString());
            param.Add("truck_weight", r["truck_weight"] == null ? "" : r["truck_weight"].ToString());
            param.Add("src_addr", r["src_addr"] == null ? "" : r["src_addr"].ToString());
            param.Add("dest_addr", r["dest_addr"] == null ? "" : r["dest_addr"].ToString());
            param.Add("left_task_count", r["left_task_count"] == null ? "" : r["left_task_count"].ToString());
            param.Add("is_auto_created", r["is_auto_created"] == null ? "" : r["is_auto_created"].ToString());
            param.Add("memo", r["memo"] == null ? "" : r["memo"].ToString());
            param.Add("create_date", r["create_date"] == null ? "" : r["create_date"].ToString());
            param.Add("created_by", r["created_by"] == null ? "" : r["created_by"].ToString());
            param.Add("modify_ip", r["modify_ip"] == null ? "" : r["modify_ip"].ToString());
            param.Add("modify_date", r["modify_date"] == null ? "" : r["modify_date"].ToString());
            param.Add("act_status", r["act_status"] == null ? "" : r["act_status"].ToString());

            int res = DBHelper.RunSql(sql, param, "插入越库单失败");
            if (res > 0)
            {
                return null;
            }
            else
            {
                return "插入越库单头失败：" + r["t_id"].ToString();
            }
        }


        /// <summary>
        /// 插入出库明细
        /// </summary>
        /// <param name="outboundHID">出库主档ID</param>
        /// <param name="r">查询的需要越库的所有出库明细</param>
        /// <returns>成功返回null，错误返回错误信息</returns>
        private string InsertOutBoundD(string outboundHID,DataTable outDetail)
        {
            WebJsonInfo info = new WebJsonInfo();
            if (outDetail == null || outDetail.Rows.Count<1)
            {
                return "越库明细的出库明细模板不存在";
            }

            foreach(DataRow r in outDetail.Rows)
            {
                string sql = @"insert into WMS_D_OUTBOUND_D (t_id, 
                            client_id, 
                            org_id, 
                            warehouse_id, 
                            outbound_h_id, 
                            item_id, 
                            item_name, 
                            spec, 
                            given_year, 
                            lot_no, 
                            exist_barcode, 
                            is_full, 
                            seq_no, 
                            owner_id, 
                            plan_qty, 
                            processed_qty, 
                            finished_qty, 
                            uom_qty_id, 
                            plan_stat_qty, 
                            processed_stat_qty, 
                            finished_stat_qty, 
                            uom_stat_id, 
                            src_factory, 
                            src_inventory, 
                            src_location, 
                            dest_factory, 
                            dest_inventory, 
                            dest_location, 
                            memo, 
                            create_date, 
                            created_by, 
                            modify_ip, 
                            modify_date, 
                            last_modified_by, 
                            act_status)
                          values(
                            @t_id, 
                            @client_id, 
                            @org_id, 
                            @warehouse_id, 
                            @outbound_h_id, 
                            @item_id, 
                            @item_name, 
                            @spec, 
                            @given_year, 
                            @lot_no, 
                            @exist_barcode, 
                            @is_full, 
                            @seq_no, 
                            @owner_id, 
                            @plan_qty, 
                            @processed_qty, 
                            @finished_qty, 
                            @uom_qty_id, 
                            @plan_stat_qty, 
                            @processed_stat_qty, 
                            @finished_stat_qty, 
                            @uom_stat_id, 
                            @src_factory, 
                            @src_inventory, 
                            @src_location, 
                            @dest_factory, 
                            @dest_inventory, 
                            @dest_location, 
                            @memo, 
                            @create_date, 
                            @created_by, 
                            @modify_ip, 
                            @modify_date, 
                            @last_modified_by, 
                            @act_status)";

                Dictionary<string, object> param = new Dictionary<string, object>();
                string t_id = SnowflakeID.Guid().ToString();
                param.Add("t_id", t_id);
                param.Add("client_id", r["client_id"] == null ? "" : r["client_id"].ToString());
                param.Add("org_id", r["org_id"] == null ? "" : r["org_id"].ToString());
                param.Add("warehouse_id", r["warehouse_id"] == null ? "" : r["warehouse_id"].ToString());
                param.Add("outbound_h_id", r["outbound_h_id"] == null ? "" : r["outbound_h_id"].ToString());
                param.Add("item_id", r["item_id"] == null ? "" : r["item_id"].ToString());
                param.Add("item_name", r["item_name"] == null ? "" : r["item_name"].ToString());
                param.Add("spec", r["spec"] == null ? "" : r["spec"].ToString());
                param.Add("given_year", r["given_year"] == null ? "" : r["given_year"].ToString());
                param.Add("lot_no", r["lot_no"] == null ? "" : r["lot_no"].ToString());
                param.Add("exist_barcode", r["exist_barcode"] == null ? "" : r["exist_barcode"].ToString());
                param.Add("is_full", r["is_full"] == null ? "" : r["is_full"].ToString());
                param.Add("seq_no", r["seq_no"] == null ? "" : r["seq_no"].ToString());
                param.Add("owner_id", r["owner_id"] == null ? "" : r["owner_id"].ToString());
                param.Add("plan_qty", r["plan_qty"] == null ? "" : r["plan_qty"].ToString());
                param.Add("processed_qty", r["processed_qty"] == null ? "" : r["processed_qty"].ToString());
                param.Add("finished_qty", r["finished_qty"] == null ? "" : r["finished_qty"].ToString());
                param.Add("uom_qty_id", r["uom_qty_id"] == null ? "" : r["uom_qty_id"].ToString());
                param.Add("plan_stat_qty", r["plan_stat_qty"] == null ? "" : r["plan_stat_qty"].ToString());
                param.Add("processed_stat_qty", r["processed_stat_qty"] == null ? "" : r["processed_stat_qty"].ToString());
                param.Add("finished_stat_qty", r["finished_stat_qty"] == null ? "" : r["finished_stat_qty"].ToString());
                param.Add("uom_stat_id", r["uom_stat_id"] == null ? "" : r["uom_stat_id"].ToString());
                param.Add("src_factory", r["src_factory"] == null ? "" : r["src_factory"].ToString());
                param.Add("src_inventory", r["src_inventory"] == null ? "" : r["src_inventory"].ToString());
                param.Add("src_location", r["src_location"] == null ? "" : r["src_location"].ToString());
                param.Add("dest_factory", r["dest_factory"] == null ? "" : r["dest_factory"].ToString());
                param.Add("dest_inventory", r["dest_inventory"] == null ? "" : r["dest_inventory"].ToString());
                param.Add("dest_location", r["dest_location"] == null ? "" : r["dest_location"].ToString());
                param.Add("memo", r["memo"] == null ? "" : r["memo"].ToString());
                param.Add("create_date", r["create_date"] == null ? "" : r["create_date"].ToString());
                param.Add("created_by", r["created_by"] == null ? "" : r["created_by"].ToString());
                param.Add("modify_ip", r["modify_ip"] == null ? "" : r["modify_ip"].ToString());
                param.Add("modify_date", r["modify_date"] == null ? "" : r["modify_date"].ToString());
                param.Add("last_modified_by", r["last_modified_by"] == null ? "" : r["last_modified_by"].ToString());
                param.Add("act_status", r["act_status"] == null ? "" : r["act_status"].ToString());

                int res = DBHelper.RunSql(sql, param, "插入越库单失明细败");
                if (res > 0)
                {
                    continue;
                }
                else
                {
                    return "插入越库明细失败："+ r["t_id"].ToString();
                }
            }
            
            return null;
        }
    }
}
