using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Dock : IDock
    {
        public DataRow GetDock(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "t_id", id }
            };
            string sql;
            sql = @"
select * from isa_bi_dock  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and t_id = @t_id 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询月台");

            return dt.Rows[0];
        }

        public DataRow GetByZone(string zoneID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "zone_id", zoneID }
            };
            string sql;
            sql = @"
select * from isa_bi_dock  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and isa_bi_warehouse_id = @warehouse 
   and isa_bi_storage_zone_id = @zone_id 
   and act_status = '1'
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "通过区域查询月台");

            return dt.Rows[0];
        }

        public DataRow[] GetDockByType(string type)
        {
            throw new NotImplementedException();
        }
    }
}
