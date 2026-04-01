using NtiCloud.Service.LPS.Model;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Item : IItem
    {
        private static String EMPTYCONTAINERITEM = "EmptyContainer";
        public DataRow Get(string itemCodeOrID)
        {
            // 获取物料编码和名称
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", itemCodeOrID }
            };
            string sql;
//            var sql1 = @"
//select i.*
//        --, ia.att_code type_code, ia.att_name type_name, 
//        --ib.att_code morph_code, ib.att_name morph_name
//  from &DataSource&wms_m_item i 
//  --left join isa_yc_item_att ia on i.att_type_id = ia.t_id
//  --left join isa_yc_item_att ib on i.att_morp_id = ib.t_id
// where i.client_id='&CLIENT_ID&' and i.org_id='&ORG_ID&' 
//   and i.warehouse_id = @warehouse
//   and (i.t_id = @t_id or i.item_code = @t_id) 
//"
            sql = @"
select i.*
  from &DataSource&wms_m_item i 
 where i.client_id='&CLIENT_ID&' and i.org_id='&ORG_ID&' 
   and i.warehouse_id = @warehouse
   and (i.t_id = @t_id or i.item_code = @t_id) 
";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料表");

            return dt.Rows[0];
        }

        public DataRow Get(string warehouseid,string itemCodeOrID)
        {
            // 获取物料编码和名称
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", warehouseid },
                { "t_id", itemCodeOrID }
            };
            string sql;
            //            var sql1 = @"
            //select i.*
            //        --, ia.att_code type_code, ia.att_name type_name, 
            //        --ib.att_code morph_code, ib.att_name morph_name
            //  from &DataSource&wms_m_item i 
            //  --left join isa_yc_item_att ia on i.att_type_id = ia.t_id
            //  --left join isa_yc_item_att ib on i.att_morp_id = ib.t_id
            // where i.client_id='&CLIENT_ID&' and i.org_id='&ORG_ID&' 
            //   and i.warehouse_id = @warehouse
            //   and (i.t_id = @t_id or i.item_code = @t_id) 
            //"
            sql = @"
select i.*
  from &DataSource&wms_m_item i 
 where i.warehouse_id = @warehouse
   and (i.t_id = @t_id or i.item_code = @t_id) 
";

            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料表");

            return dt.Rows[0];
        }
        public DataRow[] GetBy(string itemName)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                //{ "item_name", itemName }
            };
            string sql;
   //and warehouse_id='&WAREHOUSE_ID&'
            sql = @"
select * from &DataSource&wms_m_item  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and item_name like '" + itemName + @"'
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow GetItemCatalog(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_catalog   
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = (select catalog_id from &DataSource&wms_m_item where t_id = @t_id)
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料分类");

            return dt.Rows[0];
        }
       
        public DataRow GetSupplier(string itemID, string supplierID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "item_id", itemID },
                { "supplier_id", itemID }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_supplier  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and item_id = @item_id 
   and bpartner_id = @supplier_id 
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料供应商表");

            return dt.Rows[0];
        }

        /// <summary>
        /// 根据物料ID获取物料信息
        /// </summary>
        /// <param name="itemid">物料ID</param>
        /// <returns></returns>
        public ItemModel GetItem(string itemid) {
            string sql = @" select t_id, client_id, org_id, warehouse_id, item_code, item_code2, item_code3, item_code4, barcode, item_name, item_alias, item_description, need_qc, qc_status, is_enabled, scale, round_value, max_container_qty, pallet_stacking_mode, catalog_id, abc, lot_rule_id, origin, item_grade, given_year, item_length, item_width, item_height, size_uom_id, net_weight, gross_weight, tare, weight_uom_id, default_pack_uom_id, default_stat_uom_id, rate, pic_path, spec, tags, memo, safety_stock_qty, is_control_barcode, aging_time, max_aging_time, is_control_expiry_date, expiry_date_control_type, expiry_days, max_expiry_days, alert_days, stock_in_expiry_minutes, out_first_days, stock_out_expiry_days, owner_id, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                        from &DataSource&wms_m_item where t_id = @t_id";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("t_id", itemid);
            DataTable dt = DBHelper.Select(sql, lstParam, "获取物料信息");
           return DBHelper.ConvertTableToObject<ItemModel>(dt)[0];
        }

        /// <summary>
        /// 获取空托盘组对应的物料信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="emptycontainerSpecs"></param>
        /// <returns></returns>
        public ItemModel GetEmptyContainerItem(string warehouseId,decimal? emptycontainerSpecs) {
            string sql = @" select t_id, client_id, org_id, warehouse_id, item_code, item_code2, item_code3, item_code4, barcode, item_name, item_alias, item_description, need_qc, qc_status, is_enabled, scale, round_value, max_container_qty, pallet_stacking_mode, catalog_id, abc, lot_rule_id, origin, item_grade, given_year, item_length, item_width, item_height, size_uom_id, net_weight, gross_weight, tare, weight_uom_id, default_pack_uom_id, default_stat_uom_id, rate, pic_path, spec, tags, memo, safety_stock_qty, is_control_barcode, aging_time, max_aging_time, is_control_expiry_date, expiry_date_control_type, expiry_days, max_expiry_days, alert_days, stock_in_expiry_minutes, out_first_days, stock_out_expiry_days, owner_id, create_date, created_by, modify_ip, modify_date, last_modified_by, act_status 
                             from &DataSource&wms_m_item
                             where warehouse_id = @warehouse_id and item_code = @item_code";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("warehouse_id",warehouseId);
            lstParam.Add("item_code", EMPTYCONTAINERITEM+(emptycontainerSpecs == null?"":"_"+ ((decimal)emptycontainerSpecs).ToString()));
            DataTable dt = DBHelper.Select(sql, lstParam, "获取空托盘组对应的物料信息",false);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else {
                return DBHelper.ConvertTableToObject<ItemModel>(dt)[0];
            }

        }

    }
}
