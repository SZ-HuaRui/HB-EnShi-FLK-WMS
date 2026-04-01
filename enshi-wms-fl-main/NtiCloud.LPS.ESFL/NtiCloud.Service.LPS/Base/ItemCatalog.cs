using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Common.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class ItemCatalog : IItemCatalog
    {
        public DataRow Get(string id)
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
   and t_id = @t_id 
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料分类");

            return dt.Rows[0];
        }

        public DataRow GetParent(string id)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "id", id },
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_item_catalog   
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id = @warehouse 
   and p_id = (select p_id from &DataSource&wms_m_item_catalog
                where t_id = @id)
";
            DataTable dt = DBHelper.Select(sql, param, "查询物料分类", false);

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0];
        }
    }
}
