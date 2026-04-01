using NtiCloud.Service.WMS.Common;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Bom : WmsObject, IBom
    {
        public DataRow GetHead(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_bom_h  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and t_id = @t_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询物料清单主表");

            return dt.Rows[0];
        }

        public DataRow[] GetDetails(string bomCodeOrID)
        {
            // 获取出库单明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "bom_id", bomCodeOrID }
            };
            string selSql = @"
select d.t_id, d.client_id, d.org_id, d.warehouse_id, 
       d.bom_h_id, d.bom_d_id, d.seq_no, d.level_no, 
       d.item_id, d.item_code, d.item_name, d.qty, 
       d.uom_qty_id, d.qty_percent, d.memo, 
       d.create_date, d.created_by, d.modify_ip, 
       d.modify_date, d.last_modified_by, d.act_status,
       h.bom_code
  from &DataSource&wms_m_bom_d d
  join &DataSource&wms_m_bom_h h
    on d.bom_h_id = h.t_id
 where d.client_id='&CLIENT_ID&'
   and d.org_id='&ORG_ID&'
   and (h.t_id = @bom_id or h.bom_code = @bom_id)
   and d.warehouse_id = @warehouse
   and d.act_status = '1'
 order by d.seq_no
";
            DataTable dt = DBHelper.Select(selSql, param, "BOM明细表");

            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetDetailV1(string bomID)
        {
            // 获取出库单明细
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "bom_id", bomID }
            };
            string selSql = @"
select d.t_id,d.client_id,d.org_id,d.warehouse_id,d.bom_h_id,i.item_id,
           i.item_name,i.qty,d.order_seq,h.bom_code 
 from &DataSource&swms_bi_bom_d d 
  join &DataSource&swms_bi_bom_h h on d.bom_h_id = h.t_id 
  join (select * from ( 
               select a.t_id, a.warehouse_id ,a.bom_d_id,  
                          a.bom_h_id,  a.item_id,a.item_name, a.qty, 
                          a.level_number, row_number() 
                          over(PARTITION by a.bom_d_id order by a.level_number) ranknum 
                 from &DataSource&swms_bi_bom_item a ) where ranknum = 1) i on d.t_id = i.bom_d_id 
 where ifnull(d.client_id, ' ')= @S0X and ifnull(d.org_id, ' ')= @S1X 
    and h.bom_code = @S3X and d.warehouse_id = @S2X order by d.order_seq
";
            DataTable dt = DBHelper.Select(selSql, param, "BOM明细表");

            DataRow[] rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            return rows;
        }

        public DataRow[] GetFirstLevelDetail(string bomCodeOrID)
        {
            DataRow[] rows = GetDetails(bomCodeOrID);

            if (rows.GroupBy(r => r[bd.ItemID].ToString())
                .Select(r => (new { ItemID = r.Key, Count = r.Count() }))
                .Where(r => r.Count > 1)
                .Count() > 0)
            {
                throw new WmsException(-611,
                    $"配方单{rows[0]["bom_code"]}物料[{rows[0]["item_name"]}]重复!");
            }
            return rows.Where(r => decimal.Parse("0" + r[bd.LevelNo]) == 0).ToArray();
        }
    }
}
