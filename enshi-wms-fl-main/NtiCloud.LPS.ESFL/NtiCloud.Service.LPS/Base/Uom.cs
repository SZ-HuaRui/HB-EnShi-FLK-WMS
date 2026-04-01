using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Uom
    {
        public DataRow GetByCode(string uomCode)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "uom_code", uomCode }
            };
            string sql;
            sql = @"
select * from &DataSource&wms_m_uom  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and act_status = '1'
   and warehouse_id = @warehouse 
   and uom_code = @uom_code 
";
            DataTable dt;
            dt = DBHelper.Select(sql, param, "查询计量单位");

            return dt.Rows[0];
        }
    }
}
