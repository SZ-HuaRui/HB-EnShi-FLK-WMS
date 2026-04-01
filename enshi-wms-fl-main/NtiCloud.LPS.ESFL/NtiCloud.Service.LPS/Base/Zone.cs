using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Zone
    {
        public DataRow GetParent(string zoneNoOrID)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "zone", zoneNoOrID },
            };
            string sql = @"
select * from &DataSource&wms_m_zone 
where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
and warehouse_id='&WAREHOUSE_ID&'
and (t_id = @zone or zone_code = @zone) 
";

            DataTable dt = DBHelper.Select(sql, param, "查询区域");

            return dt.Rows[0];
        }

    }
}
